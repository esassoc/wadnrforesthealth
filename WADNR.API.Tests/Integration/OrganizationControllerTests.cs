using Microsoft.EntityFrameworkCore;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Integration tests for OrganizationController endpoints.
/// </summary>
[TestClass]
[DoNotParallelize]
public class OrganizationControllerTests
{
    private int _testOrganizationID;

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Clear any tracked entities from previous tests
        AssemblySteps.DbContext.ChangeTracker.Clear();

        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var org = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _testOrganizationID = org.OrganizationID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, _testOrganizationID);
        }
        catch { /* Ignore cleanup errors */ }
    }

    #region List Tests

    [TestMethod]
    public async Task List_ReturnsOrganizations()
    {
        // Act
        var organizations = await Organizations.ListAsGridRowAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(organizations);
        Assert.IsTrue(organizations.Any(o => o.OrganizationID == _testOrganizationID),
            "List should include the test organization");
    }

    [TestMethod]
    public async Task ListLookup_ReturnsOrganizations()
    {
        // Act
        var organizations = await Organizations.ListAsLookupItemAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(organizations);
        Assert.IsTrue(organizations.Any(o => o.OrganizationID == _testOrganizationID));
    }

    [TestMethod]
    public async Task ListLookupWithShortName_ReturnsOrganizations()
    {
        // Act
        var organizations = await Organizations.ListAsLookupItemWithShortNameAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(organizations);
        Assert.IsTrue(organizations.Any(o => o.OrganizationID == _testOrganizationID));
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_ReturnsOrganization_WhenExists()
    {
        // Act
        var organization = await Organizations.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testOrganizationID);

        // Assert
        Assert.IsNotNull(organization);
        Assert.AreEqual(_testOrganizationID, organization.OrganizationID);
    }

    [TestMethod]
    public async Task Get_ReturnsNull_WhenNotExists()
    {
        // Act
        var organization = await Organizations.GetByIDAsDetailAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(organization);
    }

    #endregion

    #region Create Tests

    [TestMethod]
    public async Task Create_CreatesOrganization_WhenValid()
    {
        // Arrange
        var orgType = await AssemblySteps.DbContext.OrganizationTypes.FirstAsync();
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;

        var request = new OrganizationUpsertRequest
        {
            OrganizationName = $"Test Create Org {uniqueSuffix}",
            OrganizationShortName = $"TCO{uniqueSuffix}",
            OrganizationTypeID = orgType.OrganizationTypeID,
            IsActive = true,
        };

        int createdID = 0;
        try
        {
            // Act
            var created = await Organizations.CreateAsync(
                AssemblySteps.DbContext, request, AssemblySteps.TestAdminPersonID);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual(request.OrganizationName, created.OrganizationName);
            createdID = created.OrganizationID;
        }
        finally
        {
            if (createdID > 0)
            {
                await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, createdID);
            }
        }
    }

    #endregion

    #region Update Tests

    [TestMethod]
    public async Task Update_UpdatesOrganization_WhenValid()
    {
        // Arrange
        var original = await Organizations.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testOrganizationID);
        Assert.IsNotNull(original);

        var newName = $"Updated Org {DateTime.UtcNow.Ticks}";
        var request = new OrganizationUpsertRequest
        {
            OrganizationName = newName,
            OrganizationShortName = original.OrganizationShortName,
            OrganizationTypeID = original.OrganizationTypeID,
            IsActive = original.IsActive,
        };

        // Act
        var updated = await Organizations.UpdateAsync(
            AssemblySteps.DbContext, _testOrganizationID, request, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsNotNull(updated);
        Assert.AreEqual(newName, updated.OrganizationName);
    }

    [TestMethod]
    public async Task Update_ThrowsException_WhenNotExists()
    {
        // Arrange
        var request = new OrganizationUpsertRequest
        {
            OrganizationName = "Test",
            OrganizationShortName = "TST",
            OrganizationTypeID = 1,
            IsActive = true,
        };

        // Act & Assert - UpdateAsync uses FirstAsync which throws if not found
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            await Organizations.UpdateAsync(
                AssemblySteps.DbContext, 999999, request, AssemblySteps.TestAdminPersonID));
    }

    #endregion

    #region Delete Tests

    [TestMethod]
    public async Task Delete_DeletesOrganization_WhenExists()
    {
        // Arrange - Create a new organization specifically for deletion
        var toDelete = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        var deleteID = toDelete.OrganizationID;

        // Act
        var deleted = await Organizations.DeleteAsync(AssemblySteps.DbContext, deleteID);

        // Assert
        Assert.IsTrue(deleted);
        var retrieved = await OrganizationHelper.GetByIDAsync(AssemblySteps.DbContext, deleteID);
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public async Task Delete_ReturnsFalse_WhenNotExists()
    {
        // Act
        var deleted = await Organizations.DeleteAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsFalse(deleted);
    }

    #endregion

    #region Programs Tests

    [TestMethod]
    public async Task ListProgramsForOrganization_ReturnsPrograms_WhenExist()
    {
        // Arrange - Create a program for the test organization
        var program = await ProgramHelper.CreateProgramAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID, organizationID: _testOrganizationID);

        try
        {
            // Act
            var programs = await Programs.ListAsGridRowByOrganizationIDAsync(
                AssemblySteps.DbContext, _testOrganizationID);

            // Assert
            Assert.IsNotNull(programs);
            Assert.IsTrue(programs.Any(p => p.ProgramID == program.ProgramID));
        }
        finally
        {
            await ProgramHelper.DeleteProgramAsync(AssemblySteps.DbContext, program.ProgramID);
        }
    }

    #endregion

    #region Agreements Tests

    [TestMethod]
    public async Task ListAgreementsForOrganization_ReturnsAgreements_WhenExist()
    {
        // Arrange - Create an agreement for the test organization
        var agreement = await AgreementHelper.CreateAgreementAsync(
            AssemblySteps.DbContext, organizationID: _testOrganizationID);

        try
        {
            // Act
            var agreements = await Agreements.ListAsGridRowByOrganizationIDAsync(
                AssemblySteps.DbContext, _testOrganizationID);

            // Assert
            Assert.IsNotNull(agreements);
            Assert.IsTrue(agreements.Any(a => a.AgreementID == agreement.AgreementID));
        }
        finally
        {
            await AgreementHelper.DeleteAgreementAsync(AssemblySteps.DbContext, agreement.AgreementID);
        }
    }

    #endregion

    #region Boundary Tests

    [TestMethod]
    public async Task GetBoundary_ReturnsEmptyFeatureCollection_WhenNoBoundary()
    {
        // Act
        var features = await Organizations.GetBoundaryAsFeatureCollectionAsync(
            AssemblySteps.DbContext, _testOrganizationID);

        // Assert
        Assert.IsNotNull(features);
        Assert.AreEqual(0, features.Count);
    }

    [TestMethod]
    public async Task DeleteBoundary_ReturnsTrue_WhenOrganizationExists()
    {
        // DeleteBoundaryAsync returns true if the organization exists,
        // regardless of whether it had a boundary
        // Act
        var deleted = await Organizations.DeleteBoundaryAsync(
            AssemblySteps.DbContext, _testOrganizationID);

        // Assert
        Assert.IsTrue(deleted);
    }

    [TestMethod]
    public async Task DeleteBoundary_ReturnsFalse_WhenOrganizationNotExists()
    {
        // Act
        var deleted = await Organizations.DeleteBoundaryAsync(
            AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsFalse(deleted);
    }

    #endregion

    #region Project Locations Tests

    [TestMethod]
    public async Task GetProjectLocations_ReturnsEmptyFeatureCollection_WhenNoProjects()
    {
        // Act
        var features = await Organizations.GetProjectLocationsAsFeatureCollectionAsync(
            AssemblySteps.DbContext, _testOrganizationID);

        // Assert
        Assert.IsNotNull(features);
        Assert.AreEqual(0, features.Count);
    }

    [TestMethod]
    public async Task GetProjectLocations_ReturnsFeatures_WhenProjectsExist()
    {
        // Arrange - Create a project with location point and org relationship
        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);

        try
        {
            // Add a location point to project
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE dbo.Project SET ProjectLocationPoint = geometry::Point(-122.5, 47.5, 4326) WHERE ProjectID = {project.ProjectID}");

            // Add org relationship
            var relationType = await AssemblySteps.DbContext.RelationshipTypes.FirstAsync();
            AssemblySteps.DbContext.ProjectOrganizations.Add(new ProjectOrganization
            {
                ProjectID = project.ProjectID,
                OrganizationID = _testOrganizationID,
                RelationshipTypeID = relationType.RelationshipTypeID
            });
            await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

            // Act
            var features = await Organizations.GetProjectLocationsAsFeatureCollectionAsync(
                AssemblySteps.DbContext, _testOrganizationID);

            // Assert
            Assert.IsNotNull(features);
            Assert.IsTrue(features.Count >= 1);
        }
        finally
        {
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectOrganization WHERE ProjectID = {project.ProjectID}");
            await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, project.ProjectID);
        }
    }

    #endregion

    #region Logo Tests

    [TestMethod]
    public async Task SetLogo_SetsLogo_WhenFileResourceExists()
    {
        // Arrange - Get an existing file resource from the database
        var existingFileResource = await AssemblySteps.DbContext.FileResources.FirstOrDefaultAsync();

        if (existingFileResource == null)
        {
            Assert.Inconclusive("No file resources found in database");
            return;
        }

        try
        {
            // Act
            await Organizations.SetLogoAsync(AssemblySteps.DbContext, _testOrganizationID, existingFileResource.FileResourceID);

            // Assert
            var org = await OrganizationHelper.GetByIDAsync(AssemblySteps.DbContext, _testOrganizationID);
            Assert.IsNotNull(org);
            Assert.AreEqual(existingFileResource.FileResourceID, org.LogoFileResourceID);
        }
        finally
        {
            // Clear logo
            await Organizations.SetLogoAsync(AssemblySteps.DbContext, _testOrganizationID, null);
        }
    }

    [TestMethod]
    public async Task SetLogo_ClearsLogo_WhenNullPassed()
    {
        // Act
        await Organizations.SetLogoAsync(AssemblySteps.DbContext, _testOrganizationID, null);

        // Assert
        var org = await OrganizationHelper.GetByIDAsync(AssemblySteps.DbContext, _testOrganizationID);
        Assert.IsNotNull(org);
        Assert.IsNull(org.LogoFileResourceID);
    }

    #endregion

    #region Lead Implementers Tests

    [TestMethod]
    public async Task ListLeadImplementersAsLookupItem_ReturnsOrganizations()
    {
        // Arrange - Get a calling user
        var callingUser = await People.GetByIDAsDetailAsync(AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(callingUser);

        // Act
        var organizations = await Organizations.ListLeadImplementersAsLookupItemAsync(
            AssemblySteps.DbContext, callingUser);

        // Assert - May be empty but should not fail
        Assert.IsNotNull(organizations);
    }

    #endregion

    #region Boundary Staging Tests

    [TestMethod]
    public async Task GetStagedBoundaryFeatures_ReturnsEmptyList_WhenNoStaging()
    {
        // Act
        var features = await Organizations.GetStagedBoundaryFeaturesAsync(
            AssemblySteps.DbContext, _testOrganizationID);

        // Assert
        Assert.IsNotNull(features);
        Assert.AreEqual(0, features.Count);
    }

    #endregion

    #region Lookup List Tests

    [TestMethod]
    public async Task ListAsLookupItem_OnlyReturnsActiveOrganizations()
    {
        // Arrange - Create an inactive organization
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var orgType = await AssemblySteps.DbContext.OrganizationTypes.FirstAsync();
        var inactiveOrg = new Organization
        {
            OrganizationName = $"Inactive Org {uniqueSuffix}",
            OrganizationShortName = $"INO{uniqueSuffix}",
            OrganizationTypeID = orgType.OrganizationTypeID,
            IsActive = false
        };
        AssemblySteps.DbContext.Organizations.Add(inactiveOrg);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        var inactiveID = inactiveOrg.OrganizationID;

        try
        {
            // Act
            var organizations = await Organizations.ListAsLookupItemAsync(AssemblySteps.DbContext);

            // Assert
            Assert.IsFalse(organizations.Any(o => o.OrganizationID == inactiveID),
                "Inactive organizations should not appear in lookup list");
            Assert.IsTrue(organizations.Any(o => o.OrganizationID == _testOrganizationID),
                "Active organizations should appear in lookup list");
        }
        finally
        {
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.Organization WHERE OrganizationID = {inactiveID}");
        }
    }

    #endregion

    #region Validation Tests

    [TestMethod]
    public async Task Create_CreatesOrganization_WithAllFields()
    {
        // Arrange
        var orgType = await AssemblySteps.DbContext.OrganizationTypes.FirstAsync();
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var primaryContact = await AssemblySteps.DbContext.People.FirstOrDefaultAsync();

        var request = new OrganizationUpsertRequest
        {
            OrganizationName = $"Full Test Org {uniqueSuffix}",
            OrganizationShortName = $"FTO{uniqueSuffix}",
            OrganizationTypeID = orgType.OrganizationTypeID,
            IsActive = true,
            OrganizationUrl = "https://example.com",
            PrimaryContactPersonID = primaryContact?.PersonID,
        };

        int createdID = 0;
        try
        {
            // Act
            var created = await Organizations.CreateAsync(
                AssemblySteps.DbContext, request, AssemblySteps.TestAdminPersonID);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual(request.OrganizationName, created.OrganizationName);
            Assert.AreEqual(request.OrganizationUrl, created.OrganizationUrl);
            createdID = created.OrganizationID;
        }
        finally
        {
            if (createdID > 0)
            {
                await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, createdID);
            }
        }
    }

    [TestMethod]
    public async Task Update_UpdatesAllFields_WhenValid()
    {
        // Arrange
        var original = await Organizations.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testOrganizationID);
        Assert.IsNotNull(original);

        var uniqueSuffix = DateTime.UtcNow.Ticks;
        var request = new OrganizationUpsertRequest
        {
            OrganizationName = $"Updated Org {uniqueSuffix}",
            OrganizationShortName = $"UO{uniqueSuffix % 100000}",
            OrganizationTypeID = original.OrganizationTypeID,
            IsActive = false, // Change active status
            OrganizationUrl = "https://updated-example.com",
        };

        // Act
        var updated = await Organizations.UpdateAsync(
            AssemblySteps.DbContext, _testOrganizationID, request, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsNotNull(updated);
        Assert.AreEqual(request.OrganizationName, updated.OrganizationName);
        Assert.AreEqual(request.OrganizationUrl, updated.OrganizationUrl);
        Assert.AreEqual(request.IsActive, updated.IsActive);
    }

    #endregion
}
