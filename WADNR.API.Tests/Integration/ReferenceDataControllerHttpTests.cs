using System.Net;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// HTTP integration tests for reference data controllers (Counties, Classifications,
/// ProjectTypes, Tags, DNRUplandRegions, FundSources, InteractionEvents).
/// These are mostly [AllowAnonymous] read-only endpoints.
/// </summary>
[TestClass]
[DoNotParallelize]
public class ReferenceDataControllerHttpTests
{
    #region CountyController

    [TestMethod]
    public async Task CountyList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<CountyController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task CountyList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<CountyController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(-1)]
    public async Task CountyGet_ReturnsExpectedStatus(int countyID)
    {
        var route = RouteHelper.GetRouteFor<CountyController>(c => c.Get(countyID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        if (countyID == -1)
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        else
            Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region ClassificationController

    [TestMethod]
    public async Task ClassificationList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ClassificationController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ClassificationList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ClassificationController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ClassificationGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ClassificationController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region ProjectTypeController

    [TestMethod]
    public async Task ProjectTypeList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectTypeController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectTypeList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectTypeController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ProjectTypeGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectTypeController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region TagController

    [TestMethod]
    public async Task TagList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<TagController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task TagList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<TagController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task TagGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<TagController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region DNRUplandRegionController

    [TestMethod]
    public async Task DNRUplandRegionList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<DNRUplandRegionController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task DNRUplandRegionListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<DNRUplandRegionController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task DNRUplandRegionList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<DNRUplandRegionController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task DNRUplandRegionGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<DNRUplandRegionController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region FundSourceController

    [TestMethod]
    public async Task FundSourceList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundSourceListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundSourceList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task FundSourceGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<FundSourceController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region InteractionEventController

    [TestMethod]
    public async Task InteractionEventList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<InteractionEventController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task InteractionEventList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<InteractionEventController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task InteractionEventGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<InteractionEventController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region FieldDefinitionController

    [TestMethod]
    public async Task FieldDefinitionList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FieldDefinitionController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FieldDefinitionGet_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FieldDefinitionController>(c => c.Get(1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        // AllowAnonymous — may be 200 or 404 depending on data, but not 401
        Assert.AreNotEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion

    #region AgreementStatusController

    [TestMethod]
    public async Task AgreementStatusListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<AgreementStatusController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task AgreementStatusListLookup_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<AgreementStatusController>(c => c.ListLookup());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region AgreementTypeController

    [TestMethod]
    public async Task AgreementTypeListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<AgreementTypeController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task AgreementTypeListLookup_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<AgreementTypeController>(c => c.ListLookup());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region ClassificationSystemController

    [TestMethod]
    public async Task ClassificationSystemList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ClassificationSystemController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ClassificationSystemList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ClassificationSystemController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ClassificationSystemListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ClassificationSystemController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ClassificationSystemListWithClassifications_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ClassificationSystemController>(c => c.ListWithClassifications());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ClassificationSystemGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ClassificationSystemController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region FederalFundCodeController

    [TestMethod]
    public async Task FederalFundCodeListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FederalFundCodeController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FederalFundCodeListLookup_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FederalFundCodeController>(c => c.ListLookup());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region FundSourceAllocationPriorityController

    [TestMethod]
    public async Task FundSourceAllocationPriorityListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationPriorityController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundSourceAllocationPriorityListLookup_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationPriorityController>(c => c.ListLookup());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region FundSourceTypeController

    [TestMethod]
    public async Task FundSourceTypeListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FundSourceTypeController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundSourceTypeListLookup_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FundSourceTypeController>(c => c.ListLookup());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region FundingSourceController

    [TestMethod]
    public async Task FundingSourceList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FundingSourceController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundingSourceList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FundingSourceController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region ProjectPersonRelationshipTypeController

    [TestMethod]
    public async Task ProjectPersonRelationshipTypeList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectPersonRelationshipTypeController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectPersonRelationshipTypeList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectPersonRelationshipTypeController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region ProjectCodeController

    [TestMethod]
    public async Task ProjectCodeList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectCodeController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectCodeList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectCodeController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ProjectCodeListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectCodeController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectCodeGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectCodeController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region ProgramIndexController

    [TestMethod]
    public async Task ProgramIndexGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProgramIndexController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region TaxonomyBranchController

    [TestMethod]
    public async Task TaxonomyBranchList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<TaxonomyBranchController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task TaxonomyBranchList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<TaxonomyBranchController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task TaxonomyBranchListAsLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<TaxonomyBranchController>(c => c.ListAsLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task TaxonomyBranchGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<TaxonomyBranchController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region TaxonomyTrunkController

    [TestMethod]
    public async Task TaxonomyTrunkList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<TaxonomyTrunkController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task TaxonomyTrunkList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<TaxonomyTrunkController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task TaxonomyTrunkListAsLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<TaxonomyTrunkController>(c => c.ListAsLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task TaxonomyTrunkGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<TaxonomyTrunkController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region RelationshipTypeController

    [TestMethod]
    public async Task RelationshipTypeList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<RelationshipTypeController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task RelationshipTypeList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<RelationshipTypeController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task RelationshipTypeListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<RelationshipTypeController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task RelationshipTypeListSummary_Returns200()
    {
        var route = RouteHelper.GetRouteFor<RelationshipTypeController>(c => c.ListSummary());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region OrganizationTypeController

    [TestMethod]
    public async Task OrganizationTypeList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<OrganizationTypeController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task OrganizationTypeList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<OrganizationTypeController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task OrganizationTypeListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<OrganizationTypeController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region ExternalMapLayerController

    [TestMethod]
    public async Task ExternalMapLayerListForProjectMap_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ExternalMapLayerController>(c => c.ListForProjectMap());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ExternalMapLayerListForProjectMap_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ExternalMapLayerController>(c => c.ListForProjectMap());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ExternalMapLayerListForPriorityLandscape_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ExternalMapLayerController>(c => c.ListForPriorityLandscape());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ExternalMapLayerListForOtherMaps_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ExternalMapLayerController>(c => c.ListForOtherMaps());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region FindYourForesterController

    [TestMethod]
    public async Task FindYourForesterListQuestions_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FindYourForesterController>(c => c.ListQuestions());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FindYourForesterListQuestions_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FindYourForesterController>(c => c.ListQuestions());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task FindYourForesterListActiveRoles_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FindYourForesterController>(c => c.ListActiveRoles());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region PriorityLandscapeController

    [TestMethod]
    public async Task PriorityLandscapeList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<PriorityLandscapeController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task PriorityLandscapeList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<PriorityLandscapeController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task PriorityLandscapeListCategories_Returns200()
    {
        var route = RouteHelper.GetRouteFor<PriorityLandscapeController>(c => c.ListCategories());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task PriorityLandscapeGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<PriorityLandscapeController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region FirmaHomePageImageController

    [TestMethod]
    public async Task FirmaHomePageImageList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FirmaHomePageImageController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FirmaHomePageImageList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FirmaHomePageImageController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region TreatmentController

    [TestMethod]
    public async Task TreatmentGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<TreatmentController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region ProjectNoteController

    [TestMethod]
    public async Task ProjectNoteGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectNoteController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region FundSourceAllocationNoteController

    [TestMethod]
    public async Task FundSourceAllocationNoteGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationNoteController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region ProjectDocumentController

    [TestMethod]
    public async Task ProjectDocumentGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectDocumentController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task ProjectDocumentListTypes_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectDocumentController>(c => c.ListTypes());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectDocumentListTypes_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectDocumentController>(c => c.ListTypes());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region ProjectImageController

    [TestMethod]
    public async Task ProjectImageListTimings_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectImageController>(c => c.ListTimings());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectImageListTimings_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectImageController>(c => c.ListTimings());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ProjectImageGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectImageController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region SearchController

    [TestMethod]
    public async Task SearchProjects_Returns200()
    {
        var route = RouteHelper.GetRouteFor<SearchController>(c => c.SearchProjects("test"));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region CostShareController

    [TestMethod]
    public async Task CostShareBlankPdf_Returns200()
    {
        var route = RouteHelper.GetRouteFor<CostShareController>(c => c.BlankCostShareAgreementPdf());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task CostShareBlankPdf_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<CostShareController>(c => c.BlankCostShareAgreementPdf());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    #endregion

    #region SystemInfoController

    [TestMethod]
    public async Task SystemInfoGetSystemInfo_Returns200()
    {
        var result = await AssemblySteps.AdminHttpClient.GetAsync("/");

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: /\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task SystemInfoGetSystemInfo_Returns200_WhenUnauthenticated()
    {
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync("/");

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: /");
    }

    #endregion

    #region FundSourceAllocationController

    [TestMethod]
    public async Task FundSourceAllocationList_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundSourceAllocationList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"AllowAnonymous should succeed.\nRoute: {route}");
    }

    [TestMethod]
    public async Task FundSourceAllocationListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundSourceAllocationGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region GetByID Success Path Tests

    [TestMethod]
    public async Task ClassificationSystemGetByID_Returns200_WhenExists()
    {
        var first = await AssemblySteps.DbContext.ClassificationSystems.AsNoTracking().FirstAsync();
        var route = RouteHelper.GetRouteFor<ClassificationSystemController>(c => c.GetByID(first.ClassificationSystemID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectCodeGetByID_Returns200_WhenExists()
    {
        var first = await AssemblySteps.DbContext.ProjectCodes.AsNoTracking().FirstAsync();
        var route = RouteHelper.GetRouteFor<ProjectCodeController>(c => c.GetByID(first.ProjectCodeID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProgramIndexGetByID_Returns200_WhenExists()
    {
        var first = await AssemblySteps.DbContext.ProgramIndices.AsNoTracking().FirstAsync();
        var route = RouteHelper.GetRouteFor<ProgramIndexController>(c => c.GetByID(first.ProgramIndexID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundSourceAllocationNoteGetByID_Returns200_WhenExists()
    {
        var first = await AssemblySteps.DbContext.FundSourceAllocationNotes.AsNoTracking().FirstOrDefaultAsync();
        if (first == null)
        {
            Assert.Inconclusive("No FundSourceAllocationNote data in test database to test success path.");
            return;
        }

        var route = RouteHelper.GetRouteFor<FundSourceAllocationNoteController>(c => c.GetByID(first.FundSourceAllocationNoteID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FundSourceAllocationNoteInternalGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationNoteInternalController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task FundSourceAllocationNoteInternalGetByID_Returns200_WhenExists()
    {
        var first = await AssemblySteps.DbContext.FundSourceAllocationNoteInternals.AsNoTracking().FirstOrDefaultAsync();
        if (first == null)
        {
            Assert.Inconclusive("No FundSourceAllocationNoteInternal data in test database to test success path.");
            return;
        }

        var route = RouteHelper.GetRouteFor<FundSourceAllocationNoteInternalController>(c => c.GetByID(first.FundSourceAllocationNoteInternalID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectNoteGetByID_Returns200_WhenExists()
    {
        var first = await AssemblySteps.DbContext.ProjectNotes.AsNoTracking().FirstOrDefaultAsync();
        if (first == null)
        {
            Assert.Inconclusive("No ProjectNote data in test database to test success path.");
            return;
        }

        var route = RouteHelper.GetRouteFor<ProjectNoteController>(c => c.GetByID(first.ProjectNoteID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectUpdateConfigurationGetConfiguration_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectUpdateConfigurationController>(c => c.GetConfiguration());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion
}
