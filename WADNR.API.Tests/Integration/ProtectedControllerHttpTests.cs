using System.Net;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// HTTP integration tests for protected controllers that require specific roles
/// (Vendor, Invoice, CustomPage, Role).
/// </summary>
[TestClass]
[DoNotParallelize]
public class ProtectedControllerHttpTests
{
    #region VendorController — [VendorViewFeature]

    [TestMethod]
    public async Task VendorList_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<VendorController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task VendorList_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<VendorController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task VendorGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<VendorController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region InvoiceController — [InvoiceManageFeature] / [AllowAnonymous]

    [TestMethod]
    public async Task InvoiceList_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<InvoiceController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task InvoiceList_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<InvoiceController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task InvoiceGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<InvoiceController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region CustomPageController — [PageContentManageFeature]

    [TestMethod]
    public async Task CustomPageList_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<CustomPageController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task CustomPageList_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<CustomPageController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion

    #region RoleController — [AdminFeature]

    [TestMethod]
    public async Task RoleList_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<RoleController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task RoleList_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<RoleController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task RoleList_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<RoleController>(c => c.List());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(-1)]
    public async Task RoleGet_ReturnsExpectedStatus(int roleID)
    {
        var route = RouteHelper.GetRouteFor<RoleController>(c => c.GetByID(roleID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        if (roleID == -1)
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        else
            Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region CustomRichTextController — [PageContentManageFeature] / [AllowAnonymous]

    [TestMethod]
    public async Task CustomRichTextList_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<CustomRichTextController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task CustomRichTextList_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<CustomRichTextController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task CustomRichTextGet_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<CustomRichTextController>(c => c.Get(1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        // AllowAnonymous — may be 200 or 404, but not 401
        Assert.AreNotEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion

    #region ExternalMapLayerController — [AdminFeature] for CRUD

    [TestMethod]
    public async Task ExternalMapLayerList_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<ExternalMapLayerController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ExternalMapLayerList_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ExternalMapLayerController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ExternalMapLayerList_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<ExternalMapLayerController>(c => c.List());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task ExternalMapLayerGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ExternalMapLayerController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region OrganizationTypeController — [AdminFeature] for CRUD

    [TestMethod]
    public async Task OrganizationTypeGet_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<OrganizationTypeController>(c => c.Get(1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task OrganizationTypeGet_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<OrganizationTypeController>(c => c.Get(1));
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    #endregion

    #region RelationshipTypeController — [AdminFeature] for CRUD

    [TestMethod]
    public async Task RelationshipTypeGet_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<RelationshipTypeController>(c => c.Get(1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task RelationshipTypeGet_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<RelationshipTypeController>(c => c.Get(1));
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    #endregion

    #region FundSourceAllocationNoteInternalController — [FundSourceManageFeature]

    [TestMethod]
    public async Task FundSourceAllocationNoteInternalGetByID_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationNoteInternalController>(c => c.GetByID(1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task FundSourceAllocationNoteInternalGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<FundSourceAllocationNoteInternalController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region ProjectInternalNoteController — [ProjectEditAsAdminFeature]

    [TestMethod]
    public async Task ProjectInternalNoteGetByID_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.GetByID(1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ProjectInternalNoteGetByID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.GetByID(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region UserClaimsController — [LoggedInFeature]

    [TestMethod]
    public async Task UserClaimsGetByGlobalID_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<UserClaimsController>(c => c.GetByGlobalID(AssemblySteps.TestAdminGlobalID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task UserClaimsGetByGlobalID_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<UserClaimsController>(c => c.GetByGlobalID("test"));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task UserClaimsGetByGlobalID_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<UserClaimsController>(c => c.GetByGlobalID("nonexistent-global-id"));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region GisBulkImportController — [GisBulkImportFeature]

    [TestMethod]
    public async Task GisBulkImportListSourceOrganizations_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<GisBulkImportController>(c => c.ListSourceOrganizations());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task GisBulkImportListSourceOrganizations_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<GisBulkImportController>(c => c.ListSourceOrganizations());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion

    #region JobController — [JobManageFeature]

    [TestMethod]
    public async Task JobGetImportHistory_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<JobController>(c => c.GetImportHistory());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task JobGetImportHistory_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<JobController>(c => c.GetImportHistory());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task JobGetImportHistory_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<JobController>(c => c.GetImportHistory());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    #endregion

    #region LoaUploadController — [AdminFeature]

    [TestMethod]
    public async Task LoaUploadGetDashboard_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<LoaUploadController>(c => c.GetDashboard());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task LoaUploadGetDashboard_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<LoaUploadController>(c => c.GetDashboard());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task LoaUploadGetDashboard_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<LoaUploadController>(c => c.GetDashboard());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    #endregion

    #region ProjectUpdateConfigurationController — [AdminFeature]

    [TestMethod]
    public async Task ProjectUpdateConfigurationGet_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<ProjectUpdateConfigurationController>(c => c.GetConfiguration());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ProjectUpdateConfigurationGet_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectUpdateConfigurationController>(c => c.GetConfiguration());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ProjectUpdateConfigurationGet_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<ProjectUpdateConfigurationController>(c => c.GetConfiguration());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    #endregion

    #region ReportTemplateController — [AdminFeature]

    [TestMethod]
    public async Task ReportTemplateList_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<ReportTemplateController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ReportTemplateList_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ReportTemplateController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ReportTemplateList_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<ReportTemplateController>(c => c.List());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task ReportTemplateGet_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ReportTemplateController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task ReportTemplateListModels_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<ReportTemplateController>(c => c.ListModels());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ReportTemplateListByModel_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<ReportTemplateController>(c => c.ListByModel(1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region FindYourForesterController — [FindYourForesterManageFeature]

    [TestMethod]
    public async Task FindYourForesterListAssignablePeople_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<FindYourForesterController>(c => c.ListAssignablePeople());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task FindYourForesterListAssignablePeople_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<FindYourForesterController>(c => c.ListAssignablePeople());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion

    #region InvoicePaymentRequestController — [NormalUserFeature] / [InvoiceManageFeature]

    [TestMethod]
    public async Task InvoicePaymentRequestListInvoices_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<InvoicePaymentRequestController>(c => c.ListInvoices(1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion

    #region SupportRequestController — [LoggedInFeature]

    [TestMethod]
    public async Task SupportRequestCreate_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<SupportRequestController>(c => c.Create(null!));
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsync(route, null);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task SupportRequestCreate_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<SupportRequestController>(c => c.Create(null!));
        var request = new SupportRequestCreate
        {
            SupportRequestTypeID = 1,
            RequestDescription = "Test support request from integration test",
            CurrentPageUrl = "https://test.example.com/page"
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region JobController — [JobManageFeature] additional tests

    [TestMethod]
    public async Task JobCheckFreshness_ReturnsBadRequest_ForUnknownJobName()
    {
        var route = RouteHelper.GetRouteFor<JobController>(c => c.CheckFreshness("Nonexistent Job"));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task JobClearOutdatedImports_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<JobController>(c => c.ClearOutdatedImports());
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, null);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task JobClearOutdatedImports_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<JobController>(c => c.ClearOutdatedImports());
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsync(route, null);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion
}
