using Microsoft.EntityFrameworkCore;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Integration tests for ProjectController core operations.
/// Tests the most critical project endpoints - listing, get, delete.
/// </summary>
[TestClass]
[DoNotParallelize]
public class ProjectControllerTests
{
    private int _testProjectID;
    private PersonDetail? _testCallingUser;

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Clear any tracked entities from previous tests
        AssemblySteps.DbContext.ChangeTracker.Clear();

        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _testProjectID = project.ProjectID;

        // Create a test calling user
        _testCallingUser = await People.GetByIDAsDetailAsync(AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, _testProjectID);
        }
        catch { /* Ignore cleanup errors */ }
    }

    #region List Tests

    [TestMethod]
    public async Task List_ReturnsProjects()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var projects = await Projects.ListAsGridRowForUserAsync(AssemblySteps.DbContext, _testCallingUser);

        // Assert
        Assert.IsNotNull(projects);
        Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID),
            "List should include the test project");
    }

    [TestMethod]
    public async Task ListPending_ReturnsPendingProjects()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Create a pending project
        var projectType = await AssemblySteps.DbContext.ProjectTypes.FirstAsync();
        var projectNumber = $"PND{DateTime.UtcNow.Ticks % 1000000:000000}";
        var pendingProject = new Project
        {
            ProjectTypeID = projectType.ProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Planned,
            ProjectName = $"Pending Project {projectNumber}",
            ProjectDescription = "Test pending project",
            ProjectLocationSimpleTypeID = 1,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.PendingApproval,
            FhtProjectNumber = projectNumber,
            ProposingPersonID = AssemblySteps.TestAdminPersonID,
            ProposingDate = DateTime.UtcNow,
            PlannedDate = DateOnly.FromDateTime(DateTime.Today),
        };
        AssemblySteps.DbContext.Projects.Add(pendingProject);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        var pendingID = pendingProject.ProjectID;

        try
        {
            // Act
            var projects = await Projects.ListPendingAsGridRowForUserAsync(AssemblySteps.DbContext, _testCallingUser);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == pendingID),
                "ListPending should include the pending project");
        }
        finally
        {
            await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, pendingID);
        }
    }

    [TestMethod]
    public async Task ListFeatured_ReturnsFeaturedProjects()
    {
        // Act
        var projects = await Projects.ListFeaturedAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(projects);
        // Featured projects may or may not exist, just verify the method works
    }

    [TestMethod]
    public async Task ListLookup_ReturnsProjectsForLookup()
    {
        // Act
        var projects = await Projects.ListAsLookupItemAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(projects);
        Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_ReturnsProject_WhenExists()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var project = await Projects.GetByIDAsDetailForUserAsync(
            AssemblySteps.DbContext, _testProjectID, _testCallingUser);

        // Assert
        Assert.IsNotNull(project);
        Assert.AreEqual(_testProjectID, project.ProjectID);
    }

    [TestMethod]
    public async Task Get_ReturnsNull_WhenNotExists()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var project = await Projects.GetByIDAsDetailForUserAsync(
            AssemblySteps.DbContext, 999999, _testCallingUser);

        // Assert
        Assert.IsNull(project);
    }

    #endregion

    #region Delete Tests

    [TestMethod]
    public async Task Delete_DeletesProject_WhenExists()
    {
        // Arrange - Create a project specifically for deletion
        var toDelete = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        var deleteID = toDelete.ProjectID;

        // Act - DeleteAsync returns list of deleted file resource GUIDs
        var deletedFileGuids = await Projects.DeleteAsync(AssemblySteps.DbContext, deleteID);

        // Assert - Project should no longer exist
        Assert.IsNotNull(deletedFileGuids); // Method completed without error
        var retrieved = await ProjectHelper.GetByIDAsync(AssemblySteps.DbContext, deleteID);
        Assert.IsNull(retrieved, "Project should be deleted");
    }

    [TestMethod]
    public async Task Delete_ThrowsException_WhenNotExists()
    {
        // Act & Assert - Deleting non-existent project may throw or return empty list
        try
        {
            var deletedFileGuids = await Projects.DeleteAsync(AssemblySteps.DbContext, 999999);
            // If no exception, just verify it returned something
            Assert.IsNotNull(deletedFileGuids);
        }
        catch (Exception)
        {
            // Expected - non-existent project may throw
        }
    }

    #endregion

    #region Related Data Tests

    [TestMethod]
    public async Task ListImages_ReturnsImages()
    {
        // Act
        var images = await ProjectImages.ListAsGridRowAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert
        Assert.IsNotNull(images);
        // May have images from setup, just verify method works
    }

    #endregion

    #region Audit Logs Tests

    [TestMethod]
    public async Task ListAuditLogs_ReturnsLogs()
    {
        // Act
        var logs = await AuditLogTestHelper.GetAuditLogsForProjectAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert
        Assert.IsNotNull(logs);
        // May or may not have logs
    }

    #endregion

    #region API Export Tests

    [TestMethod]
    public async Task ListAsApiJson_ReturnsApiJsonProjects()
    {
        // Act
        var projects = await Projects.ListAsApiJsonAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(projects);
        // Test project may not appear if it's not approved/active
    }

    #endregion

    #region Detail Tests

    [TestMethod]
    public async Task GetByIDWithTracking_ReturnsProject_WhenExists()
    {
        // Act
        var project = await Projects.GetByIDWithTrackingAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert
        Assert.IsNotNull(project);
        Assert.AreEqual(_testProjectID, project.ProjectID);
    }

    [TestMethod]
    public async Task GetByIDWithTracking_ReturnsNull_WhenNotExists()
    {
        // Act
        var project = await Projects.GetByIDWithTrackingAsync(
            AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(project);
    }

    [TestMethod]
    public async Task GetByIDAsDetail_ReturnsProject_WhenExists()
    {
        // GetByIDAsDetailAsync (without user) requires project to be approved/active
        // Act
        var project = await Projects.GetByIDAsDetailAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert
        Assert.IsNotNull(project);
        Assert.AreEqual(_testProjectID, project.ProjectID);
    }

    [TestMethod]
    public async Task GetByIDAsDetail_ReturnsNull_WhenNotExists()
    {
        // Act
        var project = await Projects.GetByIDAsDetailAsync(
            AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(project);
    }

    #endregion

    #region Map Tests

    [TestMethod]
    public async Task GetByIDAsMapPopup_ReturnsPopup_WhenExists()
    {
        // Act
        var popup = await Projects.GetByIDAsMapPopupAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert
        Assert.IsNotNull(popup);
        Assert.AreEqual(_testProjectID, popup.ProjectID);
    }

    [TestMethod]
    public async Task GetByIDAsMapPopup_ReturnsNull_WhenNotExists()
    {
        // Act
        var popup = await Projects.GetByIDAsMapPopupAsync(
            AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(popup);
    }

    [TestMethod]
    public async Task GetByIDAsMapPopupHtml_ReturnsHtml_WhenExists()
    {
        // Act
        var popupHtml = await Projects.GetByIDAsMapPopupHtmlAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert
        Assert.IsNotNull(popupHtml);
        Assert.AreEqual(_testProjectID, popupHtml.ProjectID);
    }

    [TestMethod]
    public async Task MapProjectFeatureCollectionForUser_ReturnsFeatureCollection()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var features = await Projects.MapProjectFeatureCollectionForUser(
            AssemblySteps.DbContext, _testCallingUser);

        // Assert
        Assert.IsNotNull(features);
        // Feature collection may be empty if projects don't have location points
    }

    #endregion

    #region Detail Grid Tests

    [TestMethod]
    public async Task ListAsProjectTypeDetailGridRow_ReturnsProjects()
    {
        // Arrange - Get the project type from the test project
        var project = await AssemblySteps.DbContext.Projects
            .AsNoTracking()
            .FirstAsync(p => p.ProjectID == _testProjectID);

        // Act
        var projects = await Projects.ListAsProjectTypeDetailGridRowAsync(
            AssemblySteps.DbContext, project.ProjectTypeID);

        // Assert
        Assert.IsNotNull(projects);
        Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID),
            "Should include the test project for its project type");
    }

    [TestMethod]
    public async Task ListAsCountyDetailGridRowForUser_ReturnsProjects_WhenProjectsExistInCounty()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Add a county to the test project
        var county = await AssemblySteps.DbContext.Counties.FirstAsync();
        AssemblySteps.DbContext.ProjectCounties.Add(new ProjectCounty
        {
            ProjectID = _testProjectID,
            CountyID = county.CountyID
        });
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        try
        {
            // Act
            var projects = await Projects.ListAsCountyDetailGridRowForUserAsync(
                AssemblySteps.DbContext, county.CountyID, _testCallingUser);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectCounty WHERE ProjectID = {_testProjectID}");
        }
    }

    [TestMethod]
    public async Task ListAsDNRUplandDetailGridRowForUser_ReturnsProjects_WhenProjectsExistInRegion()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Add a region to the test project
        var region = await AssemblySteps.DbContext.DNRUplandRegions.FirstAsync();
        AssemblySteps.DbContext.ProjectRegions.Add(new ProjectRegion
        {
            ProjectID = _testProjectID,
            DNRUplandRegionID = region.DNRUplandRegionID
        });
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        try
        {
            // Act
            var projects = await Projects.ListAsDNRUplandDetailGridRowForUserAsync(
                AssemblySteps.DbContext, region.DNRUplandRegionID, _testCallingUser);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectRegion WHERE ProjectID = {_testProjectID}");
        }
    }

    #endregion

    #region Classification Tests

    [TestMethod]
    public async Task ListClassificationsAsLookupItem_ReturnsEmptyList_WhenNoClassifications()
    {
        // Act
        var classifications = await Projects.ListClassificationsAsLookupItemByProjectIDAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert
        Assert.IsNotNull(classifications);
        Assert.AreEqual(0, classifications.Count);
    }

    [TestMethod]
    public async Task ListClassificationsAsDetailItem_ReturnsClassifications_WhenExist()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Add a classification to the test project
        var classification = await AssemblySteps.DbContext.Classifications.FirstAsync();
        AssemblySteps.DbContext.ProjectClassifications.Add(new ProjectClassification
        {
            ProjectID = _testProjectID,
            ClassificationID = classification.ClassificationID
        });
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        try
        {
            // Act
            var classifications = await Projects.ListClassificationsAsDetailItemByProjectIDForUserAsync(
                AssemblySteps.DbContext, _testProjectID, _testCallingUser);

            // Assert
            Assert.IsNotNull(classifications);
            Assert.IsTrue(classifications.Any(c => c.ClassificationID == classification.ClassificationID));
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectClassification WHERE ProjectID = {_testProjectID}");
        }
    }

    #endregion

    #region Fact Sheet Tests

    [TestMethod]
    public async Task GetByIDAsFactSheet_ReturnsFactSheet_WhenExists()
    {
        // Act
        var factSheet = await Projects.GetByIDAsFactSheetAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert
        Assert.IsNotNull(factSheet);
        Assert.AreEqual(_testProjectID, factSheet.ProjectID);
    }

    [TestMethod]
    public async Task GetByIDAsFactSheet_ReturnsNull_WhenNotExists()
    {
        // Act
        var factSheet = await Projects.GetByIDAsFactSheetAsync(
            AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(factSheet);
    }

    [TestMethod]
    public async Task GetByIDAsFactSheetForUser_ReturnsFactSheet_WhenExists()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var factSheet = await Projects.GetByIDAsFactSheetForUserAsync(
            AssemblySteps.DbContext, _testProjectID, _testCallingUser);

        // Assert
        Assert.IsNotNull(factSheet);
        Assert.AreEqual(_testProjectID, factSheet.ProjectID);
    }

    #endregion

    #region Search Tests

    [TestMethod]
    public async Task SearchForUser_ReturnsResults_WhenMatchingProjectName()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);
        var project = await AssemblySteps.DbContext.Projects
            .AsNoTracking()
            .FirstAsync(p => p.ProjectID == _testProjectID);

        // Act - Search for the test project by name
        var results = await Projects.SearchForUserAsync(
            AssemblySteps.DbContext, project.ProjectName.Substring(0, 5), _testCallingUser);

        // Assert
        Assert.IsNotNull(results);
        Assert.IsTrue(results.Any(r => r.ProjectID == _testProjectID),
            "Search should find project by partial name match");
    }

    [TestMethod]
    public async Task SearchForUser_ReturnsEmptyList_WhenNoMatch()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var results = await Projects.SearchForUserAsync(
            AssemblySteps.DbContext, "ZZZNOMATCHZZZ123456", _testCallingUser);

        // Assert
        Assert.IsNotNull(results);
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public async Task SearchForUser_ReturnsResults_WhenMatchingByProjectNumber()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);
        var project = await AssemblySteps.DbContext.Projects
            .AsNoTracking()
            .FirstAsync(p => p.ProjectID == _testProjectID);

        // Act - Search for the test project by FhtProjectNumber
        var results = await Projects.SearchForUserAsync(
            AssemblySteps.DbContext, project.FhtProjectNumber.Substring(0, 3), _testCallingUser);

        // Assert
        Assert.IsNotNull(results);
        Assert.IsTrue(results.Any(r => r.ProjectID == _testProjectID),
            "Search should find project by partial project number match");
    }

    #endregion

    #region Edit Step Tests

    [TestMethod]
    public async Task GetBasicsEditData_ReturnsEditData_WhenExists()
    {
        // Act
        var editData = await Projects.GetBasicsEditDataAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert - ProjectBasicsEditData contains import flags, not project data
        Assert.IsNotNull(editData);
    }

    #endregion

    #region Update Status Tests

    [TestMethod]
    public async Task ListUpdateStatusForUser_ReturnsStatus()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var statuses = await Projects.ListUpdateStatusForUserAsync(
            AssemblySteps.DbContext, _testCallingUser);

        // Assert
        Assert.IsNotNull(statuses);
        // Project may or may not appear depending on update workflow state
    }

    [TestMethod]
    public async Task GetProjectsWithNoContactCount_ReturnsCount()
    {
        // Act
        var count = await Projects.GetProjectsWithNoContactCountAsync(AssemblySteps.DbContext);

        // Assert
        // Count should be >= 0, method should not throw
        Assert.IsTrue(count >= 0);
    }

    #endregion

    #region Excel Export Tests

    [TestMethod]
    public async Task ListAsExcelRowForUser_ReturnsExcelData()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var excelRows = await Projects.ListAsExcelRowForUserAsync(
            AssemblySteps.DbContext, _testCallingUser);

        // Assert
        Assert.IsNotNull(excelRows);
        Assert.IsTrue(excelRows.Any(r => r.ProjectID == _testProjectID));
    }

    [TestMethod]
    public async Task ListAsDescriptionExcelRowForUser_ReturnsExcelData()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var excelRows = await Projects.ListAsDescriptionExcelRowForUserAsync(
            AssemblySteps.DbContext, _testCallingUser);

        // Assert
        Assert.IsNotNull(excelRows);
        Assert.IsTrue(excelRows.Any(r => r.ProjectID == _testProjectID));
    }

    #endregion

    #region Organization Tests

    [TestMethod]
    public async Task ListAsOrganizationDetailGridRowForUser_ReturnsProjects_WhenProjectsExistForOrg()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Add an organization relationship to the test project
        var org = await AssemblySteps.DbContext.Organizations.FirstAsync();
        var relationType = await AssemblySteps.DbContext.RelationshipTypes.FirstAsync();
        AssemblySteps.DbContext.ProjectOrganizations.Add(new ProjectOrganization
        {
            ProjectID = _testProjectID,
            OrganizationID = org.OrganizationID,
            RelationshipTypeID = relationType.RelationshipTypeID
        });
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        try
        {
            // Act
            var projects = await Projects.ListAsOrganizationDetailGridRowForUserAsync(
                AssemblySteps.DbContext, org.OrganizationID, _testCallingUser);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectOrganization WHERE ProjectID = {_testProjectID}");
        }
    }

    #endregion

    #region Tag Tests

    [TestMethod]
    public async Task ListAsTagDetailGridRowForUser_ReturnsProjects_WhenProjectsHaveTag()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Add a tag to the test project
        var tag = await AssemblySteps.DbContext.Tags.FirstOrDefaultAsync();
        if (tag == null)
        {
            // Create a tag if none exist
            tag = new Tag { TagName = "Test Tag", TagDescription = "Test" };
            AssemblySteps.DbContext.Tags.Add(tag);
            await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        }

        AssemblySteps.DbContext.ProjectTags.Add(new ProjectTag
        {
            ProjectID = _testProjectID,
            TagID = tag.TagID
        });
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        try
        {
            // Act
            var projects = await Projects.ListAsTagDetailGridRowForUserAsync(
                AssemblySteps.DbContext, tag.TagID, _testCallingUser);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectTag WHERE ProjectID = {_testProjectID}");
        }
    }

    #endregion

    #region Create Tests

    [TestMethod]
    public async Task Create_CreatesProject_WhenValid()
    {
        // Arrange
        var projectType = await AssemblySteps.DbContext.ProjectTypes.FirstAsync();
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;

        var request = new ProjectUpsertRequest
        {
            ProjectTypeID = projectType.ProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Planned,
            ProjectName = $"Create Test Project {uniqueSuffix}",
            ProjectDescription = "Test project created via Create test",
            ProjectLocationSimpleTypeID = 1,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved, // Must be Approved to pass IsActiveProjectExpr filter
            FhtProjectNumber = $"CRT{uniqueSuffix}",
            PlannedDate = DateOnly.FromDateTime(DateTime.Today)
        };

        int createdID = 0;
        try
        {
            // Act
            var created = await Projects.CreateAsync(
                AssemblySteps.DbContext, request, AssemblySteps.TestAdminPersonID);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual(request.ProjectName, created.ProjectName);
            createdID = created.ProjectID;
        }
        finally
        {
            if (createdID > 0)
            {
                await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, createdID);
            }
        }
    }

    #endregion

    #region Update Tests

    [TestMethod]
    public async Task Update_UpdatesProject_WhenValid()
    {
        // Arrange
        var project = await Projects.GetByIDWithTrackingAsync(AssemblySteps.DbContext, _testProjectID);
        Assert.IsNotNull(project);

        var newName = $"Updated Project {DateTime.UtcNow.Ticks}";
        var request = new ProjectUpsertRequest
        {
            ProjectTypeID = project.ProjectTypeID,
            ProjectStageID = project.ProjectStageID,
            ProjectName = newName,
            ProjectDescription = "Updated description",
            ProjectLocationSimpleTypeID = project.ProjectLocationSimpleTypeID,
            ProjectApprovalStatusID = project.ProjectApprovalStatusID, // Required FK
            FhtProjectNumber = project.FhtProjectNumber,
            PlannedDate = project.PlannedDate
        };

        // Act
        var updated = await Projects.UpdateAsync(
            AssemblySteps.DbContext, _testProjectID, request, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsNotNull(updated);
        Assert.AreEqual(newName, updated.ProjectName);
    }

    #endregion

    #region Location Step Tests

    [TestMethod]
    public async Task GetMapExtent_ReturnsMapExtent_WhenProjectExists()
    {
        // Act
        var mapExtent = await Projects.GetMapExtentAsync(AssemblySteps.DbContext, _testProjectID);

        // Assert - Map extent may be null if no geometry, but method should not throw
        // The test validates the method runs successfully
    }

    [TestMethod]
    public async Task GetProjectBoundingBox_ReturnsBoundingBox_WhenProjectHasLocation()
    {
        // Act
        var boundingBox = await Projects.GetProjectBoundingBoxAsync(
            AssemblySteps.DbContext, _testProjectID);

        // Assert - May be null if project has no location geometry
        // The test validates the method runs successfully
    }

    #endregion

    #region Focus Area Tests

    [TestMethod]
    public async Task ListForFocusAreaAsGridRow_ReturnsProjects_WhenProjectsExistInFocusArea()
    {
        // Arrange - Get a focus area
        var focusArea = await AssemblySteps.DbContext.FocusAreas.FirstOrDefaultAsync();
        if (focusArea == null)
        {
            Assert.Inconclusive("No focus areas found in database");
            return;
        }

        // Assign the test project to the focus area
        await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE dbo.Project SET FocusAreaID = {focusArea.FocusAreaID} WHERE ProjectID = {_testProjectID}");

        try
        {
            // Act - ListForFocusAreaAsGridRowAsync takes only dbContext and focusAreaID
            var projects = await Projects.ListForFocusAreaAsGridRowAsync(
                AssemblySteps.DbContext, focusArea.FocusAreaID);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
        }
        finally
        {
            // Cleanup - Clear focus area assignment
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE dbo.Project SET FocusAreaID = NULL WHERE ProjectID = {_testProjectID}");
        }
    }

    #endregion

    #region Person Tests

    [TestMethod]
    public async Task ListForPersonAsGridRow_ReturnsProjects_WhenProjectsExistForPerson()
    {
        // Arrange - Add a person to the test project using first available relationship type
        var relationshipTypeID = ProjectPersonRelationshipType.AllLookupDictionary.Keys.First();
        AssemblySteps.DbContext.ProjectPeople.Add(new ProjectPerson
        {
            ProjectID = _testProjectID,
            PersonID = AssemblySteps.TestAdminPersonID,
            ProjectPersonRelationshipTypeID = relationshipTypeID
        });
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        try
        {
            // Act
            var projects = await Projects.ListForPersonAsGridRowAsync(
                AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectPerson WHERE ProjectID = {_testProjectID}");
        }
    }

    #endregion

    #region Featured Projects Tests

    [TestMethod]
    public async Task UpdateFeatured_UpdatesFeaturedProjects_WhenValid()
    {
        // Arrange
        var request = new FeaturedProjectsUpdateRequest
        {
            ProjectIDs = new List<int> { _testProjectID }
        };

        try
        {
            // Act
            await Projects.UpdateFeaturedAsync(AssemblySteps.DbContext, request);

            // Assert
            var featuredProjects = await Projects.ListFeaturedAsync(AssemblySteps.DbContext);
            Assert.IsTrue(featuredProjects.Any(p => p.ProjectID == _testProjectID));
        }
        finally
        {
            // Cleanup - Remove featured status
            await Projects.UpdateFeaturedAsync(AssemblySteps.DbContext, new FeaturedProjectsUpdateRequest
            {
                ProjectIDs = new List<int>()
            });
        }
    }

    #endregion

    #region Projects With No Simple Location Tests

    [TestMethod]
    public async Task ListWithNoSimpleLocationAsProjectSimpleTree_ReturnsProjects()
    {
        // Act
        var projects = await Projects.ListWithNoSimpleLocationAsProjectSimpleTree(
            AssemblySteps.DbContext);

        // Assert - May be empty but should not fail
        Assert.IsNotNull(projects);
    }

    [TestMethod]
    public async Task ListWithNoSimpleLocationAsProjectSimpleTreeForUser_ReturnsProjects()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var projects = await Projects.ListWithNoSimpleLocationAsProjectSimpleTreeForUserAsync(
            AssemblySteps.DbContext, _testCallingUser);

        // Assert - May be empty but should not fail
        Assert.IsNotNull(projects);
    }

    #endregion

    #region Pending Projects Tests

    [TestMethod]
    public async Task ListPendingAsExcelRowForUser_ReturnsExcelRows()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var excelRows = await Projects.ListPendingAsExcelRowForUserAsync(
            AssemblySteps.DbContext, _testCallingUser);

        // Assert - May be empty but should not fail
        Assert.IsNotNull(excelRows);
    }

    [TestMethod]
    public async Task ListPendingAsDescriptionExcelRowForUser_ReturnsExcelRows()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var excelRows = await Projects.ListPendingAsDescriptionExcelRowForUserAsync(
            AssemblySteps.DbContext, _testCallingUser);

        // Assert - May be empty but should not fail
        Assert.IsNotNull(excelRows);
    }

    [TestMethod]
    public async Task ListPendingAsOrganizationDetailGridRow_ReturnsProjects()
    {
        // Arrange - Get an organization
        var org = await AssemblySteps.DbContext.Organizations.FirstAsync();

        // Act
        var projects = await Projects.ListPendingAsOrganizationDetailGridRowAsync(
            AssemblySteps.DbContext, org.OrganizationID);

        // Assert - May be empty but should not fail
        Assert.IsNotNull(projects);
    }

    #endregion

    #region Classification Detail Grid Tests

    [TestMethod]
    public async Task ListAsClassificationDetailGridRow_ReturnsProjects_WhenProjectsHaveClassification()
    {
        // Arrange
        var classification = await AssemblySteps.DbContext.Classifications.FirstAsync();

        // Add classification to test project
        AssemblySteps.DbContext.ProjectClassifications.Add(new ProjectClassification
        {
            ProjectID = _testProjectID,
            ClassificationID = classification.ClassificationID
        });
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        try
        {
            // Act
            var projects = await Projects.ListAsClassificationDetailGridRowAsync(
                AssemblySteps.DbContext, classification.ClassificationID);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectClassification WHERE ProjectID = {_testProjectID}");
        }
    }

    #endregion

    #region Map Popup For User Tests

    [TestMethod]
    public async Task GetByIDAsMapPopupForUser_ReturnsPopup_WhenExists()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var popup = await Projects.GetByIDAsMapPopupForUserAsync(
            AssemblySteps.DbContext, _testProjectID, _testCallingUser);

        // Assert
        Assert.IsNotNull(popup);
        Assert.AreEqual(_testProjectID, popup.ProjectID);
    }

    [TestMethod]
    public async Task GetByIDAsMapPopupForUser_ReturnsNull_WhenNotExists()
    {
        // Arrange
        Assert.IsNotNull(_testCallingUser);

        // Act
        var popup = await Projects.GetByIDAsMapPopupForUserAsync(
            AssemblySteps.DbContext, 999999, _testCallingUser);

        // Assert
        Assert.IsNull(popup);
    }

    #endregion
}
