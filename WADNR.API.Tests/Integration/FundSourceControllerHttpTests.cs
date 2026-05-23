using System.Net;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
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
    public async Task ListAllocations_Returns401Or403_WhenAnonymous()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListAllocations(_testFundSourceID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden,
            $"Expected 401/403 for anonymous access, got {result.StatusCode}");
    }

    [TestMethod]
    public async Task ListAllocations_Returns404_WhenFundSourceMissing()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListAllocations(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }
}
