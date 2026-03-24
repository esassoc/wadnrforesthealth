using System.Net;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class OrganizationControllerHttpTests
{
    private int _testOrganizationID;

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var organization = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _testOrganizationID = organization.OrganizationID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, _testOrganizationID);
        }
        catch { }
    }

    #region List Tests

    [TestMethod]
    public async Task List_Returns200_WithOrganizations()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var organizations = await result.DeserializeContentAsync<List<OrganizationGridRow>>();
        Assert.IsNotNull(organizations);
        Assert.IsTrue(organizations.Any(o => o.OrganizationID == _testOrganizationID));
    }

    [TestMethod]
    public async Task ListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListLookupWithShortName_Returns200()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.ListLookupWithShortName());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_Returns200_WhenExists()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.Get(_testOrganizationID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var organization = await result.DeserializeContentAsync<OrganizationDetail>();
        Assert.IsNotNull(organization);
        Assert.AreEqual(_testOrganizationID, organization.OrganizationID);
    }

    [TestMethod]
    public async Task Get_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Related Data Tests

    [TestMethod]
    public async Task ListProgramsForOrganization_Returns200()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.ListProgramsForOrganization(_testOrganizationID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListAgreementsForOrganization_Returns200()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.ListAgreementsForOrganization(_testOrganizationID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task GetBoundary_Returns200()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.GetBoundary(_testOrganizationID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task GetProjectLocations_Returns200()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.GetProjectLocations(_testOrganizationID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Authorization Tests

    [TestMethod]
    public async Task List_Returns200_WhenUnauthenticated_BecauseAllowAnonymous()
    {
        // OrganizationController.List() has [AllowAnonymous]
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"AllowAnonymous endpoint should succeed unauthenticated.\nRoute: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task Get_Returns200_WhenUnauthenticated_BecauseAllowAnonymous()
    {
        // OrganizationController.Get() has [AllowAnonymous]
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.Get(_testOrganizationID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"AllowAnonymous endpoint should succeed unauthenticated.\nRoute: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion
}
