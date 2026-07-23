using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests;

/// <summary>
/// WADNR-2259: a user who can edit contacts but lacks CanViewLandownerInfo (e.g. a program editor)
/// never receives the restricted Private Landowner contacts — they are filtered out of the project
/// detail they load (Projects.GetByIDAsDetailForUserAsync). Because those contacts can't appear in
/// the list they submit, SaveAllAsync must NOT treat their absence as a delete, or the save would
/// silently drop landowner data the user isn't permitted to see.
/// </summary>
[TestClass]
public class ProjectPeopleSaveAllTests
{
    private const int ProjectID = 1;
    private const int PrimaryContactPersonID = 100;
    private const int LandownerPersonID = 200;
    private const int PrimaryContactProjectPersonID = 10;
    private const int LandownerProjectPersonID = 20;

    private static WADNRDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new WADNRDbContext(options);
    }

    private static async Task SeedPrimaryAndLandownerAsync(WADNRDbContext db)
    {
        db.People.Add(new Person { PersonID = PrimaryContactPersonID, FirstName = "Prim", LastName = "Contact" });
        db.People.Add(new Person { PersonID = LandownerPersonID, FirstName = "Land", LastName = "Owner" });
        db.ProjectPeople.Add(new ProjectPerson
        {
            ProjectPersonID = PrimaryContactProjectPersonID,
            ProjectID = ProjectID,
            PersonID = PrimaryContactPersonID,
            ProjectPersonRelationshipTypeID = (int)ProjectPersonRelationshipTypeEnum.PrimaryContact
        });
        db.ProjectPeople.Add(new ProjectPerson
        {
            ProjectPersonID = LandownerProjectPersonID,
            ProjectID = ProjectID,
            PersonID = LandownerPersonID,
            ProjectPersonRelationshipTypeID = (int)ProjectPersonRelationshipTypeEnum.PrivateLandowner
        });
        await db.SaveChangesAsync();
    }

    // Submits only the Primary Contact — mirrors what a program editor's modal sends, since the
    // Private Landowner was filtered out of the project detail they loaded.
    private static ProjectContactSaveRequest PrimaryContactOnlyRequest() => new()
    {
        Contacts =
        [
            new ProjectContactItemRequest
            {
                ProjectPersonID = PrimaryContactProjectPersonID,
                PersonID = PrimaryContactPersonID,
                ProjectPersonRelationshipTypeID = (int)ProjectPersonRelationshipTypeEnum.PrimaryContact
            }
        ]
    };

    [TestMethod]
    public async Task SaveAllAsync_PreservesRestrictedContact_WhenCallerCannotViewLandownerInfo()
    {
        await using var db = NewInMemoryContext();
        await SeedPrimaryAndLandownerAsync(db);

        var result = await ProjectPeople.SaveAllAsync(db, ProjectID, PrimaryContactOnlyRequest(), callerCanViewLandownerInfo: false);

        var landownerStillExists = await db.ProjectPeople.AnyAsync(pp => pp.ProjectPersonID == LandownerProjectPersonID);
        Assert.IsTrue(landownerStillExists,
            "A caller without CanViewLandownerInfo must not delete the Private Landowner they never saw.");

        // The returned list must not echo the restricted contact back to a caller who can't view it.
        Assert.IsFalse(result.Any(p => p.RelationshipTypeID == (int)ProjectPersonRelationshipTypeEnum.PrivateLandowner),
            "The save response must not include Private Landowner contacts for a caller without CanViewLandownerInfo.");
    }

    [TestMethod]
    public void RestrictedRelationshipTypeIDs_ContainsPrivateLandownerOnly()
    {
        // Everything in this fix keys off this shared set mirroring the API's restricted-type flag.
        CollectionAssert.Contains(ProjectPeople.RestrictedRelationshipTypeIDs.ToList(),
            (int)ProjectPersonRelationshipTypeEnum.PrivateLandowner);
        CollectionAssert.DoesNotContain(ProjectPeople.RestrictedRelationshipTypeIDs.ToList(),
            (int)ProjectPersonRelationshipTypeEnum.PrimaryContact);
    }

    [TestMethod]
    public async Task SaveAllAsync_DeletesOmittedRestrictedContact_WhenCallerCanViewLandownerInfo()
    {
        await using var db = NewInMemoryContext();
        await SeedPrimaryAndLandownerAsync(db);

        await ProjectPeople.SaveAllAsync(db, ProjectID, PrimaryContactOnlyRequest(), callerCanViewLandownerInfo: true);

        var landownerStillExists = await db.ProjectPeople.AnyAsync(pp => pp.ProjectPersonID == LandownerProjectPersonID);
        Assert.IsFalse(landownerStillExists,
            "A caller with CanViewLandownerInfo who omits the landowner intends to remove it (normal delete-missing).");
    }
}
