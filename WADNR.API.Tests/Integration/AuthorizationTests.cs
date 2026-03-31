using System.Net;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Tests that authorization attributes correctly block/allow access for different roles.
/// Uses representative endpoints for each authorization attribute.
/// </summary>
[TestClass]
[DoNotParallelize]
public class AuthorizationTests
{
    private int _testProjectID;
    private readonly List<int> _createdPersonIDs = new();
    private readonly List<int> _createdProjectIDs = new();
    private readonly List<int> _createdOrganizationIDs = new();

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _testProjectID = project.ProjectID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try { await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, _testProjectID); } catch { }
        foreach (var id in _createdProjectIDs)
        {
            try { await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, id); } catch { }
        }
        foreach (var id in _createdPersonIDs)
        {
            try { await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, id); } catch { }
        }
        foreach (var id in _createdOrganizationIDs)
        {
            try { await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, id); } catch { }
        }
    }

    #region AllowAnonymous — should succeed without authentication

    [TestMethod]
    public async Task AllowAnonymous_FieldDefinitionGet_Returns200_WhenUnauthenticated()
    {
        // FieldDefinitionController.Get() has [AllowAnonymous]
        var route = RouteHelper.GetRouteFor<FieldDefinitionController>(c => c.Get(1));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        // AllowAnonymous endpoints should not return 401
        Assert.AreNotEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"AllowAnonymous endpoint returned 401.\nRoute: {route}");
    }

    [TestMethod]
    public async Task AllowAnonymous_OrganizationList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<OrganizationController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"AllowAnonymous endpoint should succeed.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    [TestMethod]
    public async Task AllowAnonymous_AgreementList_Returns200_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"AllowAnonymous endpoint should succeed.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion

    #region ProjectViewFeature — allows anonymous but filters visibility

    [TestMethod]
    public async Task ProjectViewFeature_List_SucceedsForUnauthenticated()
    {
        // ProjectController.List() has [ProjectViewFeature] which implements IAllowAnonymous
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreNotEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"ProjectViewFeature should allow anonymous access.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ProjectViewFeature_List_SucceedsForAdmin()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ProjectViewFeature should succeed for Admin.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion

    #region Protected endpoints — should return 401 when unauthenticated

    [TestMethod]
    public async Task AdminFeature_RoleList_Returns401_WhenUnauthenticated()
    {
        // RoleController.List() has [AdminFeature]
        var route = RouteHelper.GetRouteFor<RoleController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"AdminFeature endpoint should return 401 for unauthenticated users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task NormalUserFeature_PersonLookup_Returns401_WhenUnauthenticated()
    {
        // PersonController.ListLookup() has [NormalUserFeature]
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.ListLookup());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"NormalUserFeature endpoint should return 401 for unauthenticated users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task UserManageFeature_PersonList_Returns401_WhenUnauthenticated()
    {
        // PersonController.List() has [UserManageFeature]
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"UserManageFeature endpoint should return 401 for unauthenticated users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task NormalUserFeature_ProjectLookup_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListLookup());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"NormalUserFeature endpoint should return 401 for unauthenticated users.\nRoute: {route}");
    }

    #endregion

    #region AdminFeature — should return 403 for normal users, 200 for admin

    [TestMethod]
    public async Task AdminFeature_RoleList_Returns403_ForNormalUser()
    {
        // RoleController.List() has [AdminFeature] — Normal users should get 403
        var route = RouteHelper.GetRouteFor<RoleController>(c => c.List());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"AdminFeature endpoint should return 403 for Normal users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task AdminFeature_RoleList_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<RoleController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"AdminFeature endpoint should succeed for Admin.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region NormalUserFeature — should succeed for both Admin and Normal users

    [TestMethod]
    public async Task NormalUserFeature_PersonLookup_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"NormalUserFeature endpoint should succeed for Admin.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    [TestMethod]
    public async Task NormalUserFeature_PersonLookup_Returns200_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.ListLookup());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"NormalUserFeature endpoint should succeed for Normal users.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion

    #region ProjectPendingViewFeature — Normal + Admin

    [TestMethod]
    public async Task ProjectPendingViewFeature_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListPending());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"ProjectPendingViewFeature should return 401 for unauthenticated users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ProjectPendingViewFeature_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListPending());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ProjectPendingViewFeature should succeed for Admin.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    [TestMethod]
    public async Task ProjectPendingViewFeature_Returns200_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListPending());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ProjectPendingViewFeature should succeed for Normal users.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion

    #region ProjectEditFeature — Normal + Admin (write endpoint)

    [TestMethod]
    public async Task ProjectEditFeature_Returns401_WhenUnauthenticated()
    {
        // POST projects/create-workflow/steps/basics has [ProjectEditFeature]
        var request = new ProjectBasicsStepRequest
        {
            ProjectName = "Auth test", ProjectTypeID = 1, ProjectStageID = 1, ProgramIDs = new()
        };
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsJsonAsync(
            "projects/create-workflow/steps/basics", request);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ProjectEditFeature_Returns200_ForNormalUser()
    {
        // Normal users have [ProjectEditFeature] — test a GET with this attribute
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListUpdateStatus());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ProjectEditFeature should succeed for Normal users.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion

    #region ProjectApproveFeature — Admin only (ProjectSteward, Admin, EsaAdmin)

    [TestMethod]
    public async Task ProjectApproveFeature_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ApproveCreate(_testProjectID, null));
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsJsonAsync(
            route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ProjectApproveFeature_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ApproveCreate(_testProjectID, null));
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(
            route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"ProjectApproveFeature should return 403 for Normal users.\nRoute: {route}");
    }

    #endregion

    #region ProjectEditAsAdminFeature — Admin only (ProjectSteward, Admin, EsaAdmin)

    [TestMethod]
    public async Task ProjectEditAsAdminFeature_Returns401_WhenUnauthenticated()
    {
        var request = new ProjectBasicsSaveRequest
        {
            ProjectTypeID = 1, ProjectName = "Auth test", ProjectStageID = 1, ProgramIDs = new()
        };
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SaveBasics(_testProjectID, request));
        var result = await AssemblySteps.UnauthenticatedHttpClient.PutAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ProjectEditAsAdminFeature_Returns403_ForNormalUser()
    {
        var request = new ProjectBasicsSaveRequest
        {
            ProjectTypeID = 1, ProjectName = "Auth test", ProjectStageID = 1, ProgramIDs = new()
        };
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SaveBasics(_testProjectID, request));
        var result = await AssemblySteps.NormalHttpClient.PutAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"ProjectEditAsAdminFeature should return 403 for Normal users.\nRoute: {route}");
    }

    #endregion

    #region AgreementManageFeature — Admin only (or CanManageFundSourcesAndAgreements supplemental)

    [TestMethod]
    public async Task AgreementManageFeature_Returns401_WhenUnauthenticated()
    {
        var request = new AgreementUpsertRequest { AgreementTitle = "Auth test", AgreementTypeID = 1, OrganizationID = 1 };
        var route = RouteHelper.GetRouteTemplateFor(typeof(AgreementController),
            typeof(AgreementController).GetMethod(nameof(AgreementController.Create))!);
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task AgreementManageFeature_Returns403_ForNormalUser()
    {
        var request = new AgreementUpsertRequest { AgreementTitle = "Auth test", AgreementTypeID = 1, OrganizationID = 1 };
        var route = RouteHelper.GetRouteTemplateFor(typeof(AgreementController),
            typeof(AgreementController).GetMethod(nameof(AgreementController.Create))!);
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"AgreementManageFeature should return 403 for Normal users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task AgreementManageFeature_PassesAuth_ForUserWithFundSourceAgreementSupplementalRole()
    {
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(user.PersonID);
        await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, user.PersonID,
            RoleEnum.CanManageFundSourcesAndAgreements);

        var request = new AgreementUpsertRequest { AgreementTitle = "SupRole Auth test", AgreementTypeID = 1, OrganizationID = 1 };
        var route = RouteHelper.GetRouteTemplateFor(typeof(AgreementController),
            typeof(AgreementController).GetMethod(nameof(AgreementController.Create))!);
        var result = await HttpResponseHelper.PostAsUserAsync(route, user.GlobalID!, request);

        Assert.AreNotEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"AgreementManageFeature should allow users with CanManageFundSourcesAndAgreements.\nRoute: {route}");
        Assert.AreNotEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"AgreementManageFeature should not return 401 for authenticated user.\nRoute: {route}");
    }

    #endregion

    #region FundSourceManageFeature — Admin only (or CanManageFundSourcesAndAgreements supplemental)

    [TestMethod]
    public async Task FundSourceManageFeature_Returns401_WhenUnauthenticated()
    {
        // FundSourceAllocationController.Create() has [FundSourceManageFeature]
        var route = RouteHelper.GetRouteTemplateFor(typeof(FundSourceAllocationController),
            typeof(FundSourceAllocationController).GetMethod(nameof(FundSourceAllocationController.Create))!);
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsJsonAsync(route, new { });

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task FundSourceManageFeature_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteTemplateFor(typeof(FundSourceAllocationController),
            typeof(FundSourceAllocationController).GetMethod(nameof(FundSourceAllocationController.Create))!);
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, new { });

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"FundSourceManageFeature should return 403 for Normal users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task FundSourceManageFeature_PassesAuth_ForUserWithFundSourceAgreementSupplementalRole()
    {
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(user.PersonID);
        await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, user.PersonID,
            RoleEnum.CanManageFundSourcesAndAgreements);

        var route = RouteHelper.GetRouteTemplateFor(typeof(FundSourceAllocationController),
            typeof(FundSourceAllocationController).GetMethod(nameof(FundSourceAllocationController.Create))!);
        var result = await HttpResponseHelper.PostAsUserAsync(route, user.GlobalID!, new { });

        Assert.AreNotEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"FundSourceManageFeature should allow users with CanManageFundSourcesAndAgreements.\nRoute: {route}");
        Assert.AreNotEqual(HttpStatusCode.Unauthorized, result.StatusCode,
            $"FundSourceManageFeature should not return 401 for authenticated user.\nRoute: {route}");
    }

    #endregion

    #region PageContentManageFeature — Admin only (or CanManagePageContent supplemental)

    [TestMethod]
    public async Task PageContentManageFeature_Returns401_WhenUnauthenticated()
    {
        // CustomPageController.List() has [PageContentManageFeature]
        var route = RouteHelper.GetRouteFor<CustomPageController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task PageContentManageFeature_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<CustomPageController>(c => c.List());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"PageContentManageFeature should return 403 for Normal users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task PageContentManageFeature_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<CustomPageController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"PageContentManageFeature should succeed for Admin.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    [TestMethod]
    public async Task PageContentManageFeature_Returns200_ForUserWithPageContentSupplementalRole()
    {
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(user.PersonID);
        await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, user.PersonID,
            RoleEnum.CanManagePageContent);

        var route = RouteHelper.GetRouteFor<CustomPageController>(c => c.List());
        var result = await HttpResponseHelper.GetAsUserAsync(route, user.GlobalID!);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"PageContentManageFeature should allow users with CanManagePageContent.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region VendorViewFeature — Admin, EsaAdmin, ProjectSteward

    [TestMethod]
    public async Task VendorViewFeature_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<VendorController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task VendorViewFeature_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<VendorController>(c => c.List());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"VendorViewFeature should return 403 for Normal users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task VendorViewFeature_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<VendorController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"VendorViewFeature should succeed for Admin.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion

    #region InvoiceManageFeature — ProjectSteward, Admin, EsaAdmin

    [TestMethod]
    public async Task InvoiceManageFeature_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<InvoiceController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task InvoiceManageFeature_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<InvoiceController>(c => c.List());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"InvoiceManageFeature should return 403 for Normal users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task InvoiceManageFeature_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<InvoiceController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"InvoiceManageFeature should succeed for Admin.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion

    #region ExcelDownloadFeature — Admin, EsaAdmin, ProjectSteward

    [TestMethod]
    public async Task ExcelDownloadFeature_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.ExcelDownload());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ExcelDownloadFeature_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.ExcelDownload());
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"ExcelDownloadFeature should return 403 for Normal users.\nRoute: {route}");
    }

    [TestMethod]
    public async Task ExcelDownloadFeature_Returns200_ForAdmin()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.ExcelDownload());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ExcelDownloadFeature should succeed for Admin.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion

    #region FocusAreaManageFeature — ProjectSteward, Admin, EsaAdmin (write endpoint)

    [TestMethod]
    public async Task FocusAreaManageFeature_Returns401_WhenUnauthenticated()
    {
        // FocusAreaController.Create() has [FocusAreaManageFeature]
        var route = RouteHelper.GetRouteTemplateFor(typeof(FocusAreaController),
            typeof(FocusAreaController).GetMethod(nameof(FocusAreaController.Create))!);
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsJsonAsync(route, new { });

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task FocusAreaManageFeature_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteTemplateFor(typeof(FocusAreaController),
            typeof(FocusAreaController).GetMethod(nameof(FocusAreaController.Create))!);
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, new { });

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"FocusAreaManageFeature should return 403 for Normal users.\nRoute: {route}");
    }

    #endregion

    #region ProgramManageFeature — Admin, EsaAdmin (write endpoint)

    [TestMethod]
    public async Task ProgramManageFeature_Returns401_WhenUnauthenticated()
    {
        // ProgramController.Create() has [ProgramManageFeature]
        var route = RouteHelper.GetRouteTemplateFor(typeof(ProgramController),
            typeof(ProgramController).GetMethod(nameof(ProgramController.Create))!);
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsJsonAsync(route, new { });

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task ProgramManageFeature_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteTemplateFor(typeof(ProgramController),
            typeof(ProgramController).GetMethod(nameof(ProgramController.Create))!);
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, new { });

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"ProgramManageFeature should return 403 for Normal users.\nRoute: {route}");
    }

    #endregion

    #region UserManageFeature — supplemental role

    [TestMethod]
    public async Task UserManageFeature_Returns200_ForUserWithContactManageSupplementalRole()
    {
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(user.PersonID);
        await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, user.PersonID,
            RoleEnum.CanAddEditUsersContactsOrganizations);

        var route = RouteHelper.GetRouteFor<PersonController>(c => c.List());
        var result = await HttpResponseHelper.GetAsUserAsync(route, user.GlobalID!);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"UserManageFeature should allow users with CanAddEditUsersContactsOrganizations.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region HealthCheck — baseline connectivity test

    [TestMethod]
    public async Task HealthCheck_Returns200()
    {
        var result = await AssemblySteps.AdminHttpClient.GetAsync("healthz");

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"Health check should return 200.\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task HealthCheck_Returns200_WhenUnauthenticated()
    {
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync("healthz");

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"Health check should be accessible without auth.\nStatus: {result.StatusCode}");
    }

    #endregion

    #region PersonViewFeature — self-view, role-based, forbidden

    [TestMethod]
    public async Task PersonViewFeature_Get_Returns200_ForSelfView()
    {
        // Any authenticated user (even Normal) can view their own profile
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(user.PersonID);

        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Get(user.PersonID));
        var result = await HttpResponseHelper.GetAsUserAsync(route, user.GlobalID!);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"PersonViewFeature should allow self-view.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task PersonViewFeature_Get_Returns403_ForUnassignedUserViewingOtherPerson()
    {
        // Unassigned user trying to view someone else's profile — should get 403
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Unassigned);
        _createdPersonIDs.Add(user.PersonID);

        var contact = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(contact.PersonID);

        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Get(contact.PersonID));
        var result = await HttpResponseHelper.GetAsUserAsync(route, user.GlobalID!);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"PersonViewFeature should return 403 for Unassigned user viewing another person.\nRoute: {route}");
    }

    [TestMethod]
    public async Task PersonViewFeature_Get_Returns403_ForUnknownGlobalID()
    {
        // Authenticated with a GlobalID that doesn't match any person in the database
        var contact = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(contact.PersonID);

        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Get(contact.PersonID));
        var result = await HttpResponseHelper.GetAsUserAsync(route, Guid.NewGuid().ToString());

        // person == null path in PersonViewFeature → 403
        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"PersonViewFeature should return 403 for unknown GlobalID.\nRoute: {route}");
    }

    #endregion

    #region ProjectEditFeature — entity-scoped: Normal user who doesn't own the project

    [TestMethod]
    public async Task ProjectEditFeature_Returns403_ForNormalUserWhoDoesNotOwnProject()
    {
        // Create a draft project owned by admin
        var draft = await ProjectHelper.CreateDraftProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(draft.ProjectID);

        // Create a Normal user in a DIFFERENT org so IsMyProject returns false
        var isolatedOrg = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _createdOrganizationIDs.Add(isolatedOrg.OrganizationID);
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal,
            organizationID: isolatedOrg.OrganizationID);
        _createdPersonIDs.Add(user.PersonID);

        // Try to submit someone else's draft project — [ProjectEditFeature] entity check should fail
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SubmitCreateForApproval(draft.ProjectID, null));
        var result = await HttpResponseHelper.PostAsUserAsync(route, user.GlobalID!, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"ProjectEditFeature should return 403 for Normal user who doesn't own the project.\nRoute: {route}");
    }

    #endregion

    #region ProjectApproveFeature — entity-scoped: ProjectSteward without stewardship match

    [TestMethod]
    public async Task ProjectApproveFeature_Returns403_ForStewardWithoutStewardshipMatch()
    {
        // Create a ProjectSteward who doesn't steward the test project's area
        var steward = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.ProjectSteward);
        _createdPersonIDs.Add(steward.PersonID);

        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ApproveCreate(pending.ProjectID, null));
        var result = await HttpResponseHelper.PostAsUserAsync(route, steward.GlobalID!, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"ProjectApproveFeature should return 403 for steward without stewardship match.\nRoute: {route}");
    }

    #endregion

    #region ProjectEditAsAdminFeature — entity-scoped: ProjectSteward without stewardship match

    [TestMethod]
    public async Task ProjectEditAsAdminFeature_Returns403_ForStewardWithoutStewardshipMatch()
    {
        // Create a ProjectSteward who doesn't steward the test project's area
        var steward = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.ProjectSteward);
        _createdPersonIDs.Add(steward.PersonID);

        var request = new ProjectBasicsSaveRequest
        {
            ProjectTypeID = 1, ProjectName = "Should not save", ProjectStageID = 1, ProgramIDs = new()
        };
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SaveBasics(_testProjectID, request));
        var result = await HttpResponseHelper.PutAsUserAsync(route, steward.GlobalID!, request);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"ProjectEditAsAdminFeature should return 403 for steward without stewardship match.\nRoute: {route}");
    }

    #endregion

    #region PersonEditFeature — self-edit, admin, supplemental role, steward, WADNR org, forbidden

    [TestMethod]
    public async Task PersonEditFeature_Returns200_ForSelfEdit()
    {
        // Normal user editing their own profile — should succeed
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(user.PersonID);

        var request = new PersonUpsertRequest
        {
            FirstName = "Updated", LastName = user.LastName, Email = user.Email,
            OrganizationID = user.OrganizationID
        };
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Update(user.PersonID, request));
        var result = await HttpResponseHelper.PutAsUserAsync(route, user.GlobalID!, request);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"PersonEditFeature should allow self-edit.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task PersonEditFeature_Returns200_ForAdmin()
    {
        // Admin editing another person — should succeed
        var contact = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(contact.PersonID);

        var request = new PersonUpsertRequest
        {
            FirstName = "AdminUpdated", LastName = contact.LastName, Email = contact.Email,
            OrganizationID = contact.OrganizationID
        };
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Update(contact.PersonID, request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"PersonEditFeature should allow Admin to edit any person.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    [TestMethod]
    public async Task PersonEditFeature_Returns200_ForProjectSteward()
    {
        // ProjectSteward editing another person — should succeed
        var steward = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.ProjectSteward);
        _createdPersonIDs.Add(steward.PersonID);

        var contact = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(contact.PersonID);

        var request = new PersonUpsertRequest
        {
            FirstName = "StewardUpdated", LastName = contact.LastName, Email = contact.Email,
            OrganizationID = contact.OrganizationID
        };
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Update(contact.PersonID, request));
        var result = await HttpResponseHelper.PutAsUserAsync(route, steward.GlobalID!, request);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"PersonEditFeature should allow ProjectSteward to edit others.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task PersonEditFeature_Returns200_ForUserWithContactManageSupplementalRole()
    {
        // Normal user with CanAddEditUsersContactsOrganizations supplemental role
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(user.PersonID);
        await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, user.PersonID,
            RoleEnum.CanAddEditUsersContactsOrganizations);

        var contact = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(contact.PersonID);

        var request = new PersonUpsertRequest
        {
            FirstName = "SupRoleUpdated", LastName = contact.LastName, Email = contact.Email,
            OrganizationID = contact.OrganizationID
        };
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Update(contact.PersonID, request));
        var result = await HttpResponseHelper.PutAsUserAsync(route, user.GlobalID!, request);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"PersonEditFeature should allow users with CanAddEditUsersContactsOrganizations.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task PersonEditFeature_Returns200_ForNormalUserFromWADNROrg()
    {
        // Normal user from WADNR org (ID 4704) editing another person — should succeed
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal,
            organizationID: 4704);
        _createdPersonIDs.Add(user.PersonID);

        var contact = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(contact.PersonID);

        var request = new PersonUpsertRequest
        {
            FirstName = "WADNROrgUpdated", LastName = contact.LastName, Email = contact.Email,
            OrganizationID = contact.OrganizationID
        };
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Update(contact.PersonID, request));
        var result = await HttpResponseHelper.PutAsUserAsync(route, user.GlobalID!, request);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"PersonEditFeature should allow Normal user from WADNR org to edit others.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task PersonEditFeature_Returns403_ForNormalUserEditingOtherPerson()
    {
        // Normal user in a non-WADNR org trying to edit someone else — should get 403
        var isolatedOrg = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _createdOrganizationIDs.Add(isolatedOrg.OrganizationID);
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal,
            organizationID: isolatedOrg.OrganizationID);
        _createdPersonIDs.Add(user.PersonID);

        var contact = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(contact.PersonID);

        var request = new PersonUpsertRequest
        {
            FirstName = "ShouldNotSave", LastName = contact.LastName, Email = contact.Email,
            OrganizationID = contact.OrganizationID
        };
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Update(contact.PersonID, request));
        var result = await HttpResponseHelper.PutAsUserAsync(route, user.GlobalID!, request);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"PersonEditFeature should return 403 for Normal user editing another person.\nRoute: {route}");
    }

    #endregion

    #region ProjectPendingViewFeature — entity-scoped: Normal user and project they don't own

    [TestMethod]
    public async Task ProjectPendingViewFeature_Returns403_ForNormalUserOnPendingProjectTheyDontOwn()
    {
        // Create a pending project owned by the admin
        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        // Create a Normal user in a DIFFERENT org so IsMyProject returns false
        var isolatedOrg = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _createdOrganizationIDs.Add(isolatedOrg.OrganizationID);
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal,
            organizationID: isolatedOrg.OrganizationID);
        _createdPersonIDs.Add(user.PersonID);

        // Normal user trying to submit someone else's pending project for approval
        // This hits [ProjectEditFeature] with entity-scoped checks on a pending project
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SubmitCreateForApproval(pending.ProjectID, null));
        var result = await HttpResponseHelper.PostAsUserAsync(route, user.GlobalID!, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            $"Normal user should be forbidden from submitting pending project they don't own.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    [TestMethod]
    public async Task ProjectPendingViewFeature_ListPending_Returns200_ForProjectSteward()
    {
        // ProjectSteward has elevated project access — should succeed on list endpoint
        var steward = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.ProjectSteward);
        _createdPersonIDs.Add(steward.PersonID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListPending());
        var result = await HttpResponseHelper.GetAsUserAsync(route, steward.GlobalID!);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ProjectPendingViewFeature should succeed for ProjectSteward.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    [TestMethod]
    public async Task ProjectPendingViewFeature_ListPending_Returns200_ForCanEditProgramUser()
    {
        // CanEditProgram supplemental role user — should succeed on list endpoint
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(user.PersonID);
        await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, user.PersonID, RoleEnum.CanEditProgram);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListPending());
        var result = await HttpResponseHelper.GetAsUserAsync(route, user.GlobalID!);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ProjectPendingViewFeature should succeed for CanEditProgram user.\nRoute: {route}\nStatus: {result.StatusCode}");
    }

    #endregion
}
