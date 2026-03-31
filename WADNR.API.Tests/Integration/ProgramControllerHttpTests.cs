using System.Net;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class ProgramControllerHttpTests
{
    private int _testProgramID;
    private int _testOrganizationID;

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var organization = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _testOrganizationID = organization.OrganizationID;

        var program = await ProgramHelper.CreateProgramAsync(AssemblySteps.DbContext, _testOrganizationID);
        _testProgramID = program.ProgramID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await ProgramHelper.DeleteProgramAsync(AssemblySteps.DbContext, _testProgramID);
            await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, _testOrganizationID);
        }
        catch { }
    }

    #region List Tests

    [TestMethod]
    public async Task List_Returns200_WithPrograms()
    {
        var route = RouteHelper.GetRouteFor<ProgramController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var programs = await result.DeserializeContentAsync<List<ProgramGridRow>>();
        Assert.IsNotNull(programs);
        Assert.IsTrue(programs.Any(p => p.ProgramID == _testProgramID));
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_Returns200_WhenExists()
    {
        var route = RouteHelper.GetRouteFor<ProgramController>(c => c.Get(_testProgramID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var program = await result.DeserializeContentAsync<ProgramDetail>();
        Assert.IsNotNull(program);
        Assert.AreEqual(_testProgramID, program.ProgramID);
    }

    [TestMethod]
    public async Task Get_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProgramController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Related Data Tests

    [TestMethod]
    public async Task ListProjects_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProgramController>(c => c.ListProjects(_testProgramID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListNotifications_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProgramController>(c => c.ListNotifications(_testProgramID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListBlockList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProgramController>(c => c.ListBlockListEntries(_testProgramID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Authorization Tests — ProgramViewFeature is AllowAnonymous

    [TestMethod]
    public async Task List_Returns200_WhenUnauthenticated_BecauseProgramViewFeature()
    {
        // ProgramViewFeature implements IAllowAnonymous — public access
        var route = RouteHelper.GetRouteFor<ProgramController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ProgramViewFeature (AllowAnonymous) should succeed unauthenticated.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    [TestMethod]
    public async Task Get_Returns200Or404_WhenUnauthenticated_BecauseProgramViewFeature()
    {
        // ProgramViewFeature implements IAllowAnonymous — public access
        var route = RouteHelper.GetRouteFor<ProgramController>(c => c.Get(_testProgramID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        // Should not be 401 — may be 200 (found) or 404 (not found), but never Unauthorized
        Assert.AreNotEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"ProgramViewFeature endpoint should not return 401.\nRoute: {route}");
    }

    #endregion
}
