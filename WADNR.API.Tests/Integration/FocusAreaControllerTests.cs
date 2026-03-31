using Microsoft.EntityFrameworkCore;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.FocusArea;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Integration tests for FocusAreaController endpoints.
/// </summary>
[TestClass]
[DoNotParallelize]
public class FocusAreaControllerTests
{
    private int _testFocusAreaID;

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Clear any tracked entities from previous tests
        AssemblySteps.DbContext.ChangeTracker.Clear();

        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var focusArea = await FocusAreaHelper.CreateFocusAreaAsync(AssemblySteps.DbContext);
        _testFocusAreaID = focusArea.FocusAreaID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await FocusAreaHelper.DeleteFocusAreaAsync(AssemblySteps.DbContext, _testFocusAreaID);
        }
        catch { /* Ignore cleanup errors */ }
    }

    #region List Tests

    [TestMethod]
    public async Task List_ReturnsFocusAreas()
    {
        // Act
        var focusAreas = await FocusAreas.ListAsGridRowAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(focusAreas);
        Assert.IsTrue(focusAreas.Any(f => f.FocusAreaID == _testFocusAreaID),
            "List should include the test focus area");
    }

    [TestMethod]
    public async Task ListLocations_ReturnsFeatureCollection()
    {
        // Act
        var features = await FocusAreas.ListLocationsAsFeatureCollectionAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(features);
        // The test focus area has no geometry, so it won't be in the feature collection
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task GetByID_ReturnsFocusArea_WhenExists()
    {
        // Act
        var focusArea = await FocusAreas.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testFocusAreaID);

        // Assert
        Assert.IsNotNull(focusArea);
        Assert.AreEqual(_testFocusAreaID, focusArea.FocusAreaID);
    }

    [TestMethod]
    public async Task GetByID_ReturnsNull_WhenNotExists()
    {
        // Act
        var focusArea = await FocusAreas.GetByIDAsDetailAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(focusArea);
    }

    #endregion

    #region Create Tests

    [TestMethod]
    public async Task Create_CreatesFocusArea_WhenValid()
    {
        // Arrange
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var region = await AssemblySteps.DbContext.DNRUplandRegions.FirstAsync();

        var request = new FocusAreaUpsertRequest
        {
            FocusAreaName = $"Test Create Focus Area {uniqueSuffix}",
            FocusAreaStatusID = 1, // Active
            DNRUplandRegionID = region.DNRUplandRegionID,
        };

        int createdID = 0;
        try
        {
            // Act
            var created = await FocusAreas.CreateAsync(AssemblySteps.DbContext, request);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual(request.FocusAreaName, created.FocusAreaName);
            createdID = created.FocusAreaID;
        }
        finally
        {
            if (createdID > 0)
            {
                await FocusAreaHelper.DeleteFocusAreaAsync(AssemblySteps.DbContext, createdID);
            }
        }
    }

    #endregion

    #region Update Tests

    [TestMethod]
    public async Task Update_UpdatesFocusArea_WhenValid()
    {
        // Arrange
        var original = await FocusAreas.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testFocusAreaID);
        Assert.IsNotNull(original);

        var newName = $"Updated Focus Area {DateTime.UtcNow.Ticks}";
        var request = new FocusAreaUpsertRequest
        {
            FocusAreaName = newName,
            FocusAreaStatusID = original.FocusAreaStatusID,
            DNRUplandRegionID = original.DNRUplandRegionID,
        };

        // Act
        var updated = await FocusAreas.UpdateAsync(
            AssemblySteps.DbContext, _testFocusAreaID, request);

        // Assert
        Assert.IsNotNull(updated);
        Assert.AreEqual(newName, updated.FocusAreaName);
    }

    [TestMethod]
    public async Task Update_ThrowsException_WhenNotExists()
    {
        // Arrange
        var region = await AssemblySteps.DbContext.DNRUplandRegions.FirstAsync();
        var request = new FocusAreaUpsertRequest
        {
            FocusAreaName = "Test",
            FocusAreaStatusID = 1,
            DNRUplandRegionID = region.DNRUplandRegionID,
        };

        // Act & Assert - UpdateAsync uses FirstAsync which throws if not found
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            await FocusAreas.UpdateAsync(
                AssemblySteps.DbContext, 999999, request));
    }

    #endregion

    #region Delete Tests

    [TestMethod]
    public async Task Delete_DeletesFocusArea_WhenExists()
    {
        // Arrange - Create a new focus area specifically for deletion
        var toDelete = await FocusAreaHelper.CreateFocusAreaAsync(AssemblySteps.DbContext);
        var deleteID = toDelete.FocusAreaID;

        // Act
        var (success, errorMessage) = await FocusAreas.DeleteAsync(AssemblySteps.DbContext, deleteID);

        // Assert
        Assert.IsTrue(success, errorMessage);
        var retrieved = await FocusAreaHelper.GetByIDAsync(AssemblySteps.DbContext, deleteID);
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public async Task Delete_ReturnsFalse_WhenNotExists()
    {
        // Act
        var (success, errorMessage) = await FocusAreas.DeleteAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsFalse(success);
    }

    #endregion

    #region Location Tests

    [TestMethod]
    public async Task GetLocation_ReturnsEmptyFeatureCollection_WhenNoLocation()
    {
        // Act
        var features = await FocusAreas.GetLocationAsFeatureCollectionAsync(
            AssemblySteps.DbContext, _testFocusAreaID);

        // Assert
        Assert.IsNotNull(features);
        Assert.AreEqual(0, features.Count);
    }

    [TestMethod]
    public async Task DeleteLocation_ReturnsTrue_WhenFocusAreaExists()
    {
        // DeleteLocationAsync returns true if the focus area exists,
        // regardless of whether it had a location
        // Act
        var deleted = await FocusAreas.DeleteLocationAsync(
            AssemblySteps.DbContext, _testFocusAreaID);

        // Assert
        Assert.IsTrue(deleted);
    }

    [TestMethod]
    public async Task DeleteLocation_ReturnsFalse_WhenFocusAreaNotExists()
    {
        // Act
        var deleted = await FocusAreas.DeleteLocationAsync(
            AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsFalse(deleted);
    }

    #endregion

    #region Projects Tests

    [TestMethod]
    public async Task ListProjects_ReturnsEmptyList_WhenNoProjects()
    {
        // Act
        var projects = await Projects.ListForFocusAreaAsGridRowAsync(
            AssemblySteps.DbContext, _testFocusAreaID);

        // Assert
        Assert.IsNotNull(projects);
        Assert.AreEqual(0, projects.Count);
    }

    #endregion

    #region Staged Features Tests

    [TestMethod]
    public async Task GetStagedFeatures_ReturnsEmptyList_WhenNoStagedFeatures()
    {
        // Act
        var features = await FocusAreas.GetStagedFeaturesAsync(
            AssemblySteps.DbContext, _testFocusAreaID);

        // Assert
        Assert.IsNotNull(features);
        Assert.AreEqual(0, features.Count);
    }

    #endregion

    #region Region List Tests

    [TestMethod]
    public async Task ListForRegionAsGridRow_ReturnsFocusAreas_WhenExistInRegion()
    {
        // Arrange - Get the test focus area's region
        var focusArea = await AssemblySteps.DbContext.FocusAreas
            .AsNoTracking()
            .FirstAsync(f => f.FocusAreaID == _testFocusAreaID);

        // Act
        var focusAreas = await FocusAreas.ListForRegionAsGridRowAsync(
            AssemblySteps.DbContext, focusArea.DNRUplandRegionID);

        // Assert
        Assert.IsNotNull(focusAreas);
        Assert.IsTrue(focusAreas.Any(f => f.FocusAreaID == _testFocusAreaID));
    }

    [TestMethod]
    public async Task ListForRegionAsGridRow_ReturnsEmptyList_WhenNoFocusAreasInRegion()
    {
        // Arrange - Use a region ID that likely has no focus areas
        // We'll use a negative ID which won't exist
        var nonExistentRegionID = -999;

        // Act
        var focusAreas = await FocusAreas.ListForRegionAsGridRowAsync(
            AssemblySteps.DbContext, nonExistentRegionID);

        // Assert
        Assert.IsNotNull(focusAreas);
        Assert.AreEqual(0, focusAreas.Count);
    }

    #endregion

    #region Additional List Tests

    [TestMethod]
    public async Task ListAsGridRow_ReturnsAllFocusAreas()
    {
        // Arrange - Create a second focus area
        var secondFocusArea = await FocusAreaHelper.CreateFocusAreaAsync(AssemblySteps.DbContext);

        try
        {
            // Act
            var focusAreas = await FocusAreas.ListAsGridRowAsync(AssemblySteps.DbContext);

            // Assert - Both focus areas should be in the list
            Assert.IsTrue(focusAreas.Any(f => f.FocusAreaID == _testFocusAreaID));
            Assert.IsTrue(focusAreas.Any(f => f.FocusAreaID == secondFocusArea.FocusAreaID));
        }
        finally
        {
            await FocusAreaHelper.DeleteFocusAreaAsync(AssemblySteps.DbContext, secondFocusArea.FocusAreaID);
        }
    }

    #endregion

    #region Additional Location Tests

    [TestMethod]
    public async Task ApproveSinglePolygon_SavesLocation_WhenValidWkt()
    {
        // Arrange - Create a simple polygon WKT
        // Using a small polygon in Washington state coordinates
        var wkt = "POLYGON((-122.3 47.6, -122.3 47.7, -122.2 47.7, -122.2 47.6, -122.3 47.6))";

        try
        {
            // Act
            var success = await FocusAreas.ApproveSinglePolygonAsync(
                AssemblySteps.DbContext, _testFocusAreaID, wkt);

            // Assert
            Assert.IsTrue(success);

            // Verify the location was saved
            var features = await FocusAreas.GetLocationAsFeatureCollectionAsync(
                AssemblySteps.DbContext, _testFocusAreaID);
            Assert.AreEqual(1, features.Count);
        }
        finally
        {
            // Cleanup
            await FocusAreas.DeleteLocationAsync(AssemblySteps.DbContext, _testFocusAreaID);
        }
    }

    #endregion
}
