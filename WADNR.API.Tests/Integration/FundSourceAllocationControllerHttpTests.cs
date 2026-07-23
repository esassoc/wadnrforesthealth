using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class FundSourceAllocationControllerHttpTests
{
    private int _testOrganizationID;
    private int _testFundSourceID;
    private int _testAllocationID;

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var organization = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _testOrganizationID = organization.OrganizationID;

        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;

        var fundSource = new FundSource
        {
            FundSourceName = $"FSA Test FS {uniqueSuffix}",
            FundSourceStatusID = (int)FundSourceStatusEnum.Active,
            OrganizationID = _testOrganizationID,
            TotalAwardAmount = 100000,
        };
        AssemblySteps.DbContext.FundSources.Add(fundSource);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _testFundSourceID = fundSource.FundSourceID;

        var allocation = new FundSourceAllocation
        {
            FundSourceID = _testFundSourceID,
            FundSourceAllocationName = $"FSA Test Alloc {uniqueSuffix}",
            AllocationAmount = 1000,
            OrganizationID = _testOrganizationID,
        };
        AssemblySteps.DbContext.FundSourceAllocations.Add(allocation);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _testAllocationID = allocation.FundSourceAllocationID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.FundSourceAllocation WHERE FundSourceID = {_testFundSourceID}");
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.FundSource WHERE FundSourceID = {_testFundSourceID}");
            await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, _testOrganizationID);
        }
        catch { }
    }

    private FundSourceAllocationUpsertRequest BuildValidRequest(decimal? allocationAmount) => new()
    {
        FundSourceAllocationName = $"Test {DateTime.UtcNow.Ticks}",
        FundSourceID = _testFundSourceID,
        OrganizationID = _testOrganizationID,
        AllocationAmount = allocationAmount,
        StartDate = DateOnly.FromDateTime(DateTime.Today),
        EndDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
    };

    [TestMethod]
    public async Task Create_Returns400_WhenAllocationAmountIsNull()
    {
        var route = RouteHelper.GetRouteTemplateFor(typeof(FundSourceAllocationController),
            typeof(FundSourceAllocationController).GetMethod(nameof(FundSourceAllocationController.Create))!);
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, BuildValidRequest(null));

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode,
            $"Expected 400 for null AllocationAmount. Body: {await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task Create_Returns400_WhenAllocationAmountIsZero()
    {
        var route = RouteHelper.GetRouteTemplateFor(typeof(FundSourceAllocationController),
            typeof(FundSourceAllocationController).GetMethod(nameof(FundSourceAllocationController.Create))!);
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, BuildValidRequest(0m));

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode,
            $"Expected 400 for zero AllocationAmount. Body: {await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task Create_Returns400_WhenAllocationAmountIsNegative()
    {
        var route = RouteHelper.GetRouteTemplateFor(typeof(FundSourceAllocationController),
            typeof(FundSourceAllocationController).GetMethod(nameof(FundSourceAllocationController.Create))!);
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, BuildValidRequest(-100m));

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode,
            $"Expected 400 for negative AllocationAmount. Body: {await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task Update_Returns400_WhenAllocationAmountIsNull()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationController>(
            c => c.Update(_testAllocationID, null!));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, BuildValidRequest(null));

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode,
            $"Expected 400 for null AllocationAmount on update. Body: {await result.Content.ReadAsStringAsync()}");
    }
}
