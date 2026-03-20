using Microsoft.EntityFrameworkCore;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Integration tests for PersonController endpoints.
/// </summary>
[TestClass]
[DoNotParallelize]
public class PersonControllerTests
{
    private int _testPersonID;

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Clear any tracked entities from previous tests
        AssemblySteps.DbContext.ChangeTracker.Clear();

        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        // Create a test contact (person without login)
        var person = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _testPersonID = person.PersonID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, _testPersonID);
        }
        catch { /* Ignore cleanup errors */ }
    }

    #region List Tests

    [TestMethod]
    public async Task List_ReturnsPeople()
    {
        // Act
        var people = await People.ListAsGridRowAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(people);
        Assert.IsTrue(people.Any(p => p.PersonID == _testPersonID),
            "List should include the test person");
    }

    [TestMethod]
    public async Task ListLookup_ReturnsPeople()
    {
        // Act
        var people = await People.ListAsLookupItemAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(people);
        Assert.IsTrue(people.Any(p => p.PersonID == _testPersonID));
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_ReturnsPerson_WhenExists()
    {
        // Act
        var person = await People.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testPersonID);

        // Assert
        Assert.IsNotNull(person);
        Assert.AreEqual(_testPersonID, person.PersonID);
    }

    [TestMethod]
    public async Task Get_ReturnsNull_WhenNotExists()
    {
        // Act
        var person = await People.GetByIDAsDetailAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(person);
    }

    #endregion

    #region Create Contact Tests

    [TestMethod]
    public async Task CreatePerson_CreatesPerson_WhenValid()
    {
        // Arrange
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var org = await AssemblySteps.DbContext.Organizations.FirstAsync();

        var request = new PersonUpsertRequest
        {
            FirstName = $"TestFirst{uniqueSuffix}",
            LastName = $"TestLast{uniqueSuffix}",
            Email = $"testcreate{uniqueSuffix}@example.com",
            OrganizationID = org.OrganizationID,
        };

        int createdID = 0;
        try
        {
            // Act
            var created = await People.CreateAsync(
                AssemblySteps.DbContext, request, AssemblySteps.TestAdminPersonID);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual(request.FirstName, created.FirstName);
            Assert.AreEqual(request.LastName, created.LastName);
            Assert.IsFalse(created.IsUser, "Created person should default to contact (IsUser = false)");
            createdID = created.PersonID;
        }
        finally
        {
            if (createdID > 0)
            {
                await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, createdID);
            }
        }
    }

    #endregion

    #region Update Contact Tests

    [TestMethod]
    public async Task UpdatePerson_UpdatesPerson_WhenValid()
    {
        // Arrange
        var original = await People.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testPersonID);
        Assert.IsNotNull(original);

        var newFirstName = $"UpdatedFirst{DateTime.UtcNow.Ticks}";
        var request = new PersonUpsertRequest
        {
            FirstName = newFirstName,
            LastName = original.LastName,
            Email = original.Email,
            OrganizationID = original.OrganizationID,
            Phone = original.Phone,
        };

        // Act
        var updated = await People.UpdateAsync(
            AssemblySteps.DbContext, _testPersonID, request);

        // Assert
        Assert.IsNotNull(updated);
        Assert.AreEqual(newFirstName, updated.FirstName);
    }

    [TestMethod]
    public async Task UpdatePerson_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var request = new PersonUpsertRequest
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
        };

        // Act
        var result = await People.UpdateAsync(
            AssemblySteps.DbContext, 999999, request);

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region Delete Contact Tests

    [TestMethod]
    public async Task DeletePerson_DeletesContact_WhenExists()
    {
        // Arrange - Create a new contact specifically for deletion
        var toDelete = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        var deleteID = toDelete.PersonID;

        // Act
        await People.DeleteAsync(AssemblySteps.DbContext, deleteID);

        // Assert
        var retrieved = await PersonHelper.GetByIDAsync(AssemblySteps.DbContext, deleteID);
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public async Task DeletePerson_ThrowsException_WhenIsUser()
    {
        // Arrange - Create a user (has IsUser = true)
        var user = await PersonHelper.CreateUserAsync(
            AssemblySteps.DbContext, RoleEnum.Normal);
        var userID = user.PersonID;

        try
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
                await People.DeleteAsync(AssemblySteps.DbContext, userID));
        }
        finally
        {
            // Clean up - need to clear IsUser and GlobalID to delete
            var personEntity = await AssemblySteps.DbContext.People
                .FirstOrDefaultAsync(p => p.PersonID == userID);
            if (personEntity != null)
            {
                personEntity.GlobalID = null;
                personEntity.IsUser = false;
                await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
                await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, userID);
            }
        }
    }

    #endregion

    #region Projects Tests

    [TestMethod]
    public async Task ListProjects_ReturnsEmptyList_WhenNoProjects()
    {
        // Act
        var projects = await Projects.ListForPersonAsGridRowAsync(
            AssemblySteps.DbContext, _testPersonID);

        // Assert
        Assert.IsNotNull(projects);
        Assert.AreEqual(0, projects.Count);
    }

    #endregion

    #region Agreements Tests

    [TestMethod]
    public async Task ListAgreements_ReturnsEmptyList_WhenNoAgreements()
    {
        // Act
        var agreements = await Agreements.ListForPersonAsGridRowAsync(
            AssemblySteps.DbContext, _testPersonID);

        // Assert
        Assert.IsNotNull(agreements);
        Assert.AreEqual(0, agreements.Count);
    }

    #endregion

    #region Roles Tests

    [TestMethod]
    public async Task UpdateRoles_UpdatesRoles_WhenValid()
    {
        // Arrange - Create a full user to test role updates
        var user = await PersonHelper.CreateUserAsync(
            AssemblySteps.DbContext, RoleEnum.Normal);
        var userID = user.PersonID;

        try
        {
            var request = new PersonRolesUpsertRequest
            {
                BaseRoleID = (int)RoleEnum.ProjectSteward,
                SupplementalRoleIDs = new List<int>()
            };

            // Act
            var updated = await People.UpdateRolesAsync(
                AssemblySteps.DbContext, userID, request);

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual((int)RoleEnum.ProjectSteward, updated.BaseRole?.RoleID);
        }
        finally
        {
            // Clean up
            var personEntity = await AssemblySteps.DbContext.People
                .FirstOrDefaultAsync(p => p.PersonID == userID);
            if (personEntity != null)
            {
                personEntity.GlobalID = null;
                await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
                await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, userID);
            }
        }
    }

    #endregion

    #region Toggle Active Tests

    [TestMethod]
    public async Task ToggleActive_TogglesActiveStatus()
    {
        // Arrange - Get current status
        var original = await People.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testPersonID);
        Assert.IsNotNull(original);
        var originalIsActive = original.IsActive;

        // Act
        var updated = await People.ToggleActiveAsync(
            AssemblySteps.DbContext, _testPersonID);

        // Assert
        Assert.IsNotNull(updated);
        Assert.AreEqual(!originalIsActive, updated.IsActive);

        // Toggle back
        await People.ToggleActiveAsync(AssemblySteps.DbContext, _testPersonID);
    }

    #endregion

    #region Additional List Tests

    [TestMethod]
    public async Task ListWadnrAsLookupItem_ReturnsPeopleWithOrganization()
    {
        // Act
        var people = await People.ListWadnrAsLookupItemAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(people);
        // These are WADNR people, test person may or may not be included
    }

    [TestMethod]
    public async Task ListLookup_WadnrOnly_ReturnsPeople()
    {
        // Act
        var people = await People.ListAsLookupItemAsync(AssemblySteps.DbContext, wadnrOnly: true);

        // Assert
        Assert.IsNotNull(people);
        // WADNR-only list, may or may not include test person
    }

    #endregion

    #region Get By GlobalID Tests

    [TestMethod]
    public async Task GetByGlobalIDAsDetail_ReturnsNull_WhenNotExists()
    {
        // Act
        var person = await People.GetByGlobalIDAsDetailAsync(
            AssemblySteps.DbContext, "nonexistent-global-id-12345");

        // Assert
        Assert.IsNull(person);
    }

    [TestMethod]
    public async Task GetByGlobalIDAsDetail_ReturnsPerson_WhenExists()
    {
        // Arrange - Create a user with GlobalID
        var user = await PersonHelper.CreateUserAsync(
            AssemblySteps.DbContext, RoleEnum.Normal);
        var userID = user.PersonID;
        var globalID = user.GlobalID;

        try
        {
            Assert.IsNotNull(globalID);

            // Act
            var person = await People.GetByGlobalIDAsDetailAsync(
                AssemblySteps.DbContext, globalID);

            // Assert
            Assert.IsNotNull(person);
            Assert.AreEqual(userID, person.PersonID);
        }
        finally
        {
            // Clean up
            var personEntity = await AssemblySteps.DbContext.People
                .FirstOrDefaultAsync(p => p.PersonID == userID);
            if (personEntity != null)
            {
                personEntity.GlobalID = null;
                await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
                await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, userID);
            }
        }
    }

    #endregion

    #region Notifications Tests

    [TestMethod]
    public async Task ListNotificationsForPerson_ReturnsEmptyList_WhenNoNotifications()
    {
        // Act
        var notifications = await People.ListNotificationsForPersonAsGridRowAsync(
            AssemblySteps.DbContext, _testPersonID);

        // Assert
        Assert.IsNotNull(notifications);
        Assert.AreEqual(0, notifications.Count);
    }

    #endregion

    #region Stewardship Tests

    [TestMethod]
    public async Task ListStewardshipRegions_ReturnsRegions()
    {
        // Act
        var regions = await People.ListStewardshipRegionsAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(regions);
        // Should have some regions defined
    }

    [TestMethod]
    public async Task UpdateStewardshipAreas_UpdatesAreas_WhenValid()
    {
        // Arrange - Create a full user to test stewardship updates
        var user = await PersonHelper.CreateUserAsync(
            AssemblySteps.DbContext, RoleEnum.ProjectSteward);
        var userID = user.PersonID;

        try
        {
            // Get a region to assign
            var region = await AssemblySteps.DbContext.DNRUplandRegions.FirstAsync();

            var request = new PersonStewardshipAreasUpsertRequest
            {
                DNRUplandRegionIDs = new List<int> { region.DNRUplandRegionID }
            };

            // Act
            var updated = await People.UpdateStewardshipAreasAsync(
                AssemblySteps.DbContext, userID, request);

            // Assert
            Assert.IsNotNull(updated);
        }
        finally
        {
            // Clean up
            var personEntity = await AssemblySteps.DbContext.People
                .FirstOrDefaultAsync(p => p.PersonID == userID);
            if (personEntity != null)
            {
                personEntity.GlobalID = null;
                await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
                await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, userID);
            }
        }
    }

    #endregion

    #region List Filters Tests

    [TestMethod]
    public async Task ListAsGridRow_IncludesInactivePersons()
    {
        // Arrange - Set the test person to inactive
        await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE dbo.Person SET IsActive = 0 WHERE PersonID = {_testPersonID}");

        try
        {
            // Act
            var people = await People.ListAsGridRowAsync(AssemblySteps.DbContext);

            // Assert - ListAsGridRow should include inactive persons
            Assert.IsTrue(people.Any(p => p.PersonID == _testPersonID),
                "ListAsGridRow should include inactive persons");
        }
        finally
        {
            // Cleanup - restore active status
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE dbo.Person SET IsActive = 1 WHERE PersonID = {_testPersonID}");
        }
    }

    #endregion

    #region IsUser Tests

    [TestMethod]
    public async Task CreatePerson_AsUser_SetsIsUserTrue()
    {
        // Arrange
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var org = await AssemblySteps.DbContext.Organizations.FirstAsync();

        var request = new PersonUpsertRequest
        {
            FirstName = $"TestUser{uniqueSuffix}",
            LastName = $"TestLast{uniqueSuffix}",
            Email = $"testuser{uniqueSuffix}@example.com",
            OrganizationID = org.OrganizationID,
            IsUser = true,
        };

        int createdID = 0;
        try
        {
            var created = await People.CreateAsync(
                AssemblySteps.DbContext, request, AssemblySteps.TestAdminPersonID);

            Assert.IsNotNull(created);
            Assert.IsTrue(created.IsUser, "Person created with IsUser=true should be a user");
            createdID = created.PersonID;
        }
        finally
        {
            if (createdID > 0)
            {
                await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, createdID);
            }
        }
    }

    #endregion

    #region API Key Tests

    [TestMethod]
    public async Task GetApiKeyByPersonID_ReturnsNull_WhenNoApiKey()
    {
        // Act
        var apiKey = await People.GetApiKeyByPersonIDAsync(
            AssemblySteps.DbContext, _testPersonID);

        // Assert
        Assert.IsNull(apiKey);
    }

    [TestMethod]
    public async Task GenerateApiKey_GeneratesKey_WhenValid()
    {
        // Arrange - Create a user for API key generation
        var user = await PersonHelper.CreateUserAsync(
            AssemblySteps.DbContext, RoleEnum.Normal);
        var userID = user.PersonID;

        try
        {
            // Act
            var apiKey = await People.GenerateApiKeyAsync(
                AssemblySteps.DbContext, userID);

            // Assert
            Assert.IsNotNull(apiKey);
            Assert.IsTrue(Guid.TryParse(apiKey, out _), "API key should be a valid GUID");

            // Verify it can be retrieved
            var retrievedKey = await People.GetApiKeyByPersonIDAsync(
                AssemblySteps.DbContext, userID);
            Assert.AreEqual(apiKey, retrievedKey);
        }
        finally
        {
            // Clean up
            var personEntity = await AssemblySteps.DbContext.People
                .FirstOrDefaultAsync(p => p.PersonID == userID);
            if (personEntity != null)
            {
                personEntity.GlobalID = null;
                personEntity.ApiKey = null;
                await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
                await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, userID);
            }
        }
    }

    #endregion
}
