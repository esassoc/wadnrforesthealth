using System.Net;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.FocusArea;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class FocusAreaControllerHttpTests
{
    private int _testFocusAreaID;
    private int? _testProjectID;

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var focusArea = await FocusAreaHelper.CreateFocusAreaAsync(AssemblySteps.DbContext);
        _testFocusAreaID = focusArea.FocusAreaID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        if (_testProjectID.HasValue)
        {
            try { await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, _testProjectID.Value); } catch { }
        }
        try
        {
            await FocusAreaHelper.DeleteFocusAreaAsync(AssemblySteps.DbContext, _testFocusAreaID);
        }
        catch { }
    }

    #region List Tests

    [TestMethod]
    public async Task List_Returns200_WithFocusAreas()
    {
        var route = RouteHelper.GetRouteFor<FocusAreaController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var focusAreas = await result.DeserializeContentAsync<List<FocusAreaGridRow>>();
        Assert.IsNotNull(focusAreas);
        Assert.IsTrue(focusAreas.Any(f => f.FocusAreaID == _testFocusAreaID));
    }

    [TestMethod]
    public async Task ListLocations_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FocusAreaController>(c => c.ListLocations());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task GetByID_Returns200_WhenExists()
    {
        var route = RouteHelper.GetRouteFor<FocusAreaController>(c => c.GetByID(_testFocusAreaID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var focusArea = await result.DeserializeContentAsync<FocusAreaDetail>();
        Assert.IsNotNull(focusArea);
        Assert.AreEqual(_testFocusAreaID, focusArea.FocusAreaID);
    }

    [TestMethod]
    public async Task GetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<FocusAreaController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Get Related Data

    [TestMethod]
    public async Task GetLocation_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FocusAreaController>(c => c.GetLocation(_testFocusAreaID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListProjects_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FocusAreaController>(c => c.ListProjects(_testFocusAreaID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Closeout Projects

    [TestMethod]
    public async Task GetByID_ReturnsCloseoutProjects_WhenProjectsExist()
    {
        // Create a project linked to the test focus area with Implementation stage
        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _testProjectID = project.ProjectID;

        // Set the project's FocusAreaID and stage to Implementation (a closeout-eligible stage)
        project.FocusAreaID = _testFocusAreaID;
        project.ProjectStageID = (int)ProjectStageEnum.Implementation;
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        // Fetch the focus area detail
        var route = RouteHelper.GetRouteFor<FocusAreaController>(c => c.GetByID(_testFocusAreaID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var focusArea = await result.DeserializeContentAsync<FocusAreaDetail>();
        Assert.IsNotNull(focusArea);
        Assert.IsTrue(focusArea.CloseoutProjects.Count > 0, "FocusArea should have at least one closeout project.");

        var closeoutProject = focusArea.CloseoutProjects.First(cp => cp.ProjectID == project.ProjectID);
        Assert.IsFalse(string.IsNullOrEmpty(closeoutProject.ProjectStageDisplayName),
            "CloseoutProject should have ProjectStageDisplayName populated from static lookup.");
    }

    #endregion

    #region Authorization Tests

    [TestMethod]
    public async Task List_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FocusAreaController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion
}
