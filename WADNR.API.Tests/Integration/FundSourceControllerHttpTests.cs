using System.Net;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class FundSourceControllerHttpTests
{
    private int _testOrganizationID;
    private int _testFundSourceID;
    private int _otherFundSourceID;
    private readonly List<int> _createdAllocationIDs = new();

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var organization = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _testOrganizationID = organization.OrganizationID;

        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;

        var testFundSource = new FundSource
        {
            FundSourceName = $"Test FS {uniqueSuffix}",
            FundSourceStatusID = (int)FundSourceStatusEnum.Active,
            OrganizationID = _testOrganizationID,
            TotalAwardAmount = 100000,
        };
        AssemblySteps.DbContext.FundSources.Add(testFundSource);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _testFundSourceID = testFundSource.FundSourceID;

        var otherFundSource = new FundSource
        {
            FundSourceName = $"Other FS {uniqueSuffix}",
            FundSourceStatusID = (int)FundSourceStatusEnum.Active,
            OrganizationID = _testOrganizationID,
            TotalAwardAmount = 50000,
        };
        AssemblySteps.DbContext.FundSources.Add(otherFundSource);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _otherFundSourceID = otherFundSource.FundSourceID;

        var alloc1 = new FundSourceAllocation { FundSourceID = _testFundSourceID, FundSourceAllocationName = $"Alloc A {uniqueSuffix}", AllocationAmount = 10000 };
        var alloc2 = new FundSourceAllocation { FundSourceID = _testFundSourceID, FundSourceAllocationName = $"Alloc B {uniqueSuffix}", AllocationAmount = 20000 };
        var alloc3 = new FundSourceAllocation { FundSourceID = _otherFundSourceID, FundSourceAllocationName = $"Other Alloc {uniqueSuffix}", AllocationAmount = 5000 };
        AssemblySteps.DbContext.FundSourceAllocations.AddRange(alloc1, alloc2, alloc3);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _createdAllocationIDs.Add(alloc1.FundSourceAllocationID);
        _createdAllocationIDs.Add(alloc2.FundSourceAllocationID);
        _createdAllocationIDs.Add(alloc3.FundSourceAllocationID);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            foreach (var allocationID in _createdAllocationIDs)
            {
                await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM dbo.FundSourceAllocation WHERE FundSourceAllocationID = {allocationID}");
            }
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.FundSourceExternalLink WHERE FundSourceID IN ({_testFundSourceID}, {_otherFundSourceID})");
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.FundSource WHERE FundSourceID IN ({_testFundSourceID}, {_otherFundSourceID})");
            await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, _testOrganizationID);
        }
        catch { }
    }

    [TestMethod]
    public async Task ListAllocations_Returns200_WithGridRowsForFundSource_WhenAdmin()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListAllocations(_testFundSourceID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var rows = await result.DeserializeContentAsync<List<FundSourceAllocationGridRow>>();
        Assert.IsNotNull(rows);
        Assert.AreEqual(2, rows.Count(r => r.FundSourceID == _testFundSourceID));
        Assert.IsFalse(rows.Any(r => r.FundSourceID == _otherFundSourceID),
            "Allocations from a different FundSource should not be included.");
    }

    [TestMethod]
    public async Task ListAllocations_Returns200_WhenNormalUser()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListAllocations(_testFundSourceID));
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"LoggedInFeature should allow any authenticated user. Route: {route}, Status: {result.StatusCode}");
    }

    [TestMethod]
    public async Task ListAllocations_Returns200_WhenAnonymous()
    {
        // Fund source pages are public: allocations must be viewable by anonymous users (WADNR-2269).
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListAllocations(_testFundSourceID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"Allocations endpoint should allow anonymous access. Route: {route}, Status: {result.StatusCode}");
        var rows = await result.DeserializeContentAsync<List<FundSourceAllocationGridRow>>();
        Assert.IsNotNull(rows);
        Assert.AreEqual(2, rows.Count(r => r.FundSourceID == _testFundSourceID));
    }

    [TestMethod]
    public async Task ListAllocations_Returns404_WhenFundSourceMissing()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListAllocations(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task ListProjectLocations_Returns200_WhenAnonymous()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListProjectLocations(_testFundSourceID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"Project-locations endpoint should allow anonymous access. Route: {route}, Status: {result.StatusCode}");
    }

    [TestMethod]
    public async Task ListProjectLocations_Returns404_WhenFundSourceMissing()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListProjectLocations(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task ListProjects_Returns200_WhenAnonymous()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListProjects(_testFundSourceID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"Projects endpoint should allow anonymous access. Route: {route}, Status: {result.StatusCode}");
        var rows = await result.DeserializeContentAsync<List<FundSourceProjectGridRow>>();
        Assert.IsNotNull(rows);
    }

    [TestMethod]
    public async Task ListProjects_Returns404_WhenFundSourceMissing()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListProjects(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task ListImages_Returns200_WhenAnonymous()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListImages(_testFundSourceID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"Images endpoint should allow anonymous access. Route: {route}, Status: {result.StatusCode}");
        var rows = await result.DeserializeContentAsync<List<FundSourceImageGridRow>>();
        Assert.IsNotNull(rows);
    }

    [TestMethod]
    public async Task ListImages_Returns404_WhenFundSourceMissing()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListImages(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task UpdateAbout_Returns200AndPersists_WhenAdmin()
    {
        var aboutHtml = $"<p>Public narrative {DateTime.UtcNow.Ticks}</p>";
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.UpdateAbout(_testFundSourceID, null));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route,
            new FundSourceAboutUpsertRequest { AboutThisFundSource = aboutHtml });

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var updated = await result.DeserializeContentAsync<FundSourceDetail>();
        Assert.IsNotNull(updated);
        Assert.AreEqual(aboutHtml, updated.AboutThisFundSource);

        // Confirm it persisted via the public detail endpoint.
        var getRoute = RouteHelper.GetRouteFor<FundSourceController>(c => c.Get(_testFundSourceID));
        var getResult = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(getRoute);
        var detail = await getResult.DeserializeContentAsync<FundSourceDetail>();
        Assert.AreEqual(aboutHtml, detail.AboutThisFundSource);
    }

    [TestMethod]
    public async Task UpdateAbout_Returns401Or403_WhenAnonymous()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.UpdateAbout(_testFundSourceID, null));
        var result = await AssemblySteps.UnauthenticatedHttpClient.PutAsJsonAsync(route,
            new FundSourceAboutUpsertRequest { AboutThisFundSource = "<p>nope</p>" });

        Assert.IsTrue(result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden,
            $"Expected 401/403 for anonymous edit, got {result.StatusCode}");
    }

    #region External Links

    private static FundSourceExternalLinkSaveRequest LinksRequest(params (int? id, string label, string url)[] links)
    {
        return new FundSourceExternalLinkSaveRequest
        {
            ExternalLinks = links.Select(l => new FundSourceExternalLinkItemRequest
            {
                FundSourceExternalLinkID = l.id,
                ExternalLinkLabel = l.label,
                ExternalLinkUrl = l.url
            }).ToList()
        };
    }

    private async Task<List<FundSourceExternalLinkGridRow>> SaveLinksAsAdminAsync(FundSourceExternalLinkSaveRequest request)
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.SaveAllExternalLinks(_testFundSourceID, null));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);
        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        return await result.DeserializeContentAsync<List<FundSourceExternalLinkGridRow>>();
    }

    [TestMethod]
    public async Task ListExternalLinks_Returns200SortedByLabel_WhenAnonymous()
    {
        await SaveLinksAsAdminAsync(LinksRequest(
            (null, "Zebra", "https://example.com/z"),
            (null, "Alpha", "https://example.com/a")));

        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListExternalLinks(_testFundSourceID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"External links should allow anonymous access. Route: {route}, Status: {result.StatusCode}");
        var rows = await result.DeserializeContentAsync<List<FundSourceExternalLinkGridRow>>();
        CollectionAssert.AreEqual(new[] { "Alpha", "Zebra" }, rows.Select(r => r.ExternalLinkLabel).ToArray());
    }

    [TestMethod]
    public async Task ListExternalLinks_Returns404_WhenFundSourceMissing()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListExternalLinks(-1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task SaveAllExternalLinks_Returns200AndPersists_WhenAdmin()
    {
        var saved = await SaveLinksAsAdminAsync(LinksRequest(
            (null, "Keep", "https://example.com/keep"),
            (null, "Remove", "https://example.com/remove")));
        Assert.AreEqual(2, saved.Count);

        var keep = saved.Single(l => l.ExternalLinkLabel == "Keep");
        var updated = await SaveLinksAsAdminAsync(LinksRequest(
            (keep.FundSourceExternalLinkID, "Kept", "https://example.com/kept"),
            (null, "Added", "https://example.com/added")));

        CollectionAssert.AreEqual(new[] { "Added", "Kept" }, updated.Select(l => l.ExternalLinkLabel).ToArray());
        Assert.AreEqual(keep.FundSourceExternalLinkID, updated.Single(l => l.ExternalLinkLabel == "Kept").FundSourceExternalLinkID);
        Assert.AreEqual(0, await AssemblySteps.DbContext.FundSourceExternalLinks.CountAsync(l => l.ExternalLinkLabel == "Remove" && l.FundSourceID == _testFundSourceID));
        Assert.AreEqual(0, await AssemblySteps.DbContext.FundSourceExternalLinks.CountAsync(l => l.FundSourceID == _otherFundSourceID));
    }

    [TestMethod]
    public async Task SaveAllExternalLinks_PrefixesHttps_WhenSchemeMissing()
    {
        var saved = await SaveLinksAsAdminAsync(LinksRequest(
            (null, "No Scheme", "  example.com/page  "),
            (null, "Http", "http://example.com/http")));

        Assert.AreEqual("https://example.com/page", saved.Single(l => l.ExternalLinkLabel == "No Scheme").ExternalLinkUrl);
        Assert.AreEqual("http://example.com/http", saved.Single(l => l.ExternalLinkLabel == "Http").ExternalLinkUrl);
    }

    [TestMethod]
    public async Task SaveAllExternalLinks_Returns400AndSavesNothing_WhenLabelOrUrlBlank()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.SaveAllExternalLinks(_testFundSourceID, null));

        var blankLabel = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route,
            LinksRequest((null, "Valid", "https://example.com"), (null, "   ", "https://example.com/blank")));
        var blankUrl = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route,
            LinksRequest((null, "Valid", "https://example.com"), (null, "Blank Url", "   ")));

        Assert.AreEqual(HttpStatusCode.BadRequest, blankLabel.StatusCode);
        Assert.AreEqual(HttpStatusCode.BadRequest, blankUrl.StatusCode);
        Assert.AreEqual(0, await AssemblySteps.DbContext.FundSourceExternalLinks.CountAsync(l => l.FundSourceID == _testFundSourceID));
    }

    [TestMethod]
    public async Task SaveAllExternalLinks_Returns401Or403_WhenAnonymous()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.SaveAllExternalLinks(_testFundSourceID, null));
        var result = await AssemblySteps.UnauthenticatedHttpClient.PutAsJsonAsync(route,
            LinksRequest((null, "Nope", "https://example.com")));

        Assert.IsTrue(result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden,
            $"Expected 401/403 for anonymous edit, got {result.StatusCode}");
    }

    [TestMethod]
    public async Task SaveAllExternalLinks_Returns403_WhenNormalUser()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.SaveAllExternalLinks(_testFundSourceID, null));
        var result = await AssemblySteps.NormalHttpClient.PutAsJsonAsync(route,
            LinksRequest((null, "Nope", "https://example.com")));

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task Delete_RemovesExternalLinks_WhenFundSourceHasLinks()
    {
        await SaveLinksAsAdminAsync(LinksRequest((null, "Link", "https://example.com")));

        var deleted = await FundSources.DeleteAsync(AssemblySteps.DbContext, _testFundSourceID);

        Assert.IsTrue(deleted);
        Assert.AreEqual(0, await AssemblySteps.DbContext.FundSourceExternalLinks.CountAsync(l => l.FundSourceID == _testFundSourceID));
    }

    #endregion
}
