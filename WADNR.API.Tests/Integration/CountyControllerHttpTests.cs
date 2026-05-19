using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class CountyControllerHttpTests
{
    private int _testCountyID;
    private string? _originalContent;

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var county = await AssemblySteps.DbContext.Counties
            .OrderBy(x => x.CountyID)
            .FirstAsync();
        _testCountyID = county.CountyID;
        _originalContent = county.CountyContent;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        var county = await AssemblySteps.DbContext.Counties.SingleOrDefaultAsync(x => x.CountyID == _testCountyID);
        if (county != null)
        {
            county.CountyContent = _originalContent;
            await AssemblySteps.DbContext.SaveChangesAsync();
        }
    }

    [TestMethod]
    public async Task UpdateContent_Returns200_PersistsContent_WhenAdmin()
    {
        var route = RouteHelper.GetRouteFor<CountyController>(c => c.UpdateContent(_testCountyID, null!));
        var request = new CountyContentUpsertRequest { CountyContent = "<p>Hello from test</p>" };

        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var detail = await result.DeserializeContentAsync<CountyDetail>();
        Assert.IsNotNull(detail);
        Assert.AreEqual("<p>Hello from test</p>", detail.CountyContent);

        AssemblySteps.DbContext.ChangeTracker.Clear();
        var reloaded = await AssemblySteps.DbContext.Counties.SingleAsync(x => x.CountyID == _testCountyID);
        Assert.AreEqual("<p>Hello from test</p>", reloaded.CountyContent);
    }

    [TestMethod]
    public async Task UpdateContent_Returns404_WhenCountyMissing()
    {
        var route = RouteHelper.GetRouteFor<CountyController>(c => c.UpdateContent(-1, null!));
        var request = new CountyContentUpsertRequest { CountyContent = "<p>x</p>" };

        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task UpdateContent_Returns403_WhenNormalUser()
    {
        var route = RouteHelper.GetRouteFor<CountyController>(c => c.UpdateContent(_testCountyID, null!));
        var request = new CountyContentUpsertRequest { CountyContent = "<p>nope</p>" };

        var result = await AssemblySteps.NormalHttpClient.PutAsJsonAsync(route, request);

        Assert.IsTrue(result.StatusCode == HttpStatusCode.Forbidden || result.StatusCode == HttpStatusCode.Unauthorized,
            $"Expected 401/403, got {result.StatusCode}");
    }

    [TestMethod]
    public async Task Get_ReturnsCountyContent_AfterUpdate()
    {
        var updateRoute = RouteHelper.GetRouteFor<CountyController>(c => c.UpdateContent(_testCountyID, null!));
        var updateRequest = new CountyContentUpsertRequest { CountyContent = "<p>roundtrip</p>" };
        var updateResult = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(updateRoute, updateRequest);
        Assert.IsTrue(updateResult.IsSuccessStatusCode);

        var getRoute = RouteHelper.GetRouteFor<CountyController>(c => c.Get(_testCountyID));
        var getResult = await AssemblySteps.AdminHttpClient.GetAsync(getRoute);
        Assert.IsTrue(getResult.IsSuccessStatusCode);

        var detail = await getResult.DeserializeContentAsync<CountyDetail>();
        Assert.IsNotNull(detail);
        Assert.AreEqual("<p>roundtrip</p>", detail.CountyContent);
    }
}
