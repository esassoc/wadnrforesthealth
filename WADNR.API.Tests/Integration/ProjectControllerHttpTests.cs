using System.Net;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class ProjectControllerHttpTests
{
    private int _testProjectID;
    private readonly List<int> _createdProjectIDs = new();
    private readonly List<int> _createdPersonIDs = new();

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
        foreach (var id in _createdProjectIDs)
        {
            try { await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, id); } catch { }
        }
        try
        {
            await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, _testProjectID);
        }
        catch { }
        foreach (var id in _createdPersonIDs)
        {
            try { await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, id); } catch { }
        }
    }

    #region List Tests

    [TestMethod]
    public async Task List_Returns200_WithProjects()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var projects = await result.DeserializeContentAsync<List<ProjectGridRow>>();
        Assert.IsNotNull(projects);
        Assert.IsTrue(projects.Any(p => p.ProjectID == _testProjectID));
    }

    [TestMethod]
    public async Task ListFeatured_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListFeatured());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListPending_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListPending());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListUpdateStatus_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListUpdateStatus());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_Returns200_WhenExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.Get(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var project = await result.DeserializeContentAsync<ProjectDetail>();
        Assert.IsNotNull(project);
        Assert.AreEqual(_testProjectID, project.ProjectID);
    }

    [TestMethod]
    public async Task Get_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Related Data Tests

    [TestMethod]
    public async Task GetFactSheet_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.GetForFactSheet(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task GetMapPopup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.GetAsMapPopup(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListImages_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListImages(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListClassifications_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListClassifications(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Delete Tests

    [TestMethod]
    public async Task Delete_Returns204_WhenExists()
    {
        // Create a separate project to delete (AdminFeature)
        var toDelete = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.Delete(toDelete.ProjectID));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
    }

    [TestMethod]
    public async Task Delete_Returns403_ForNormalUser()
    {
        // ProjectController.Delete() has [AdminFeature]
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.Delete(_testProjectID));
        var result = await AssemblySteps.NormalHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task Delete_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.Delete(-1));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Admin Direct Edit Tests

    [TestMethod]
    public async Task SaveBasics_Returns200_ForAdmin()
    {
        var project = await ProjectHelper.GetByIDAsync(AssemblySteps.DbContext, _testProjectID);
        Assert.IsNotNull(project);

        var request = new ProjectBasicsSaveRequest
        {
            ProjectTypeID = project.ProjectTypeID,
            ProjectName = $"Updated via HTTP {DateTime.UtcNow.Ticks}",
            ProjectDescription = project.ProjectDescription,
            ProjectStageID = project.ProjectStageID,
            PlannedDate = project.PlannedDate,
            ProgramIDs = new List<int>(),
        };

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SaveBasics(_testProjectID, request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task SaveBasics_Returns403_ForNormalUser()
    {
        // ProjectEditAsAdminFeature excludes Normal users
        var request = new ProjectBasicsSaveRequest
        {
            ProjectTypeID = 1,
            ProjectName = "Should not save",
            ProjectStageID = 1,
            ProgramIDs = new List<int>(),
        };

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SaveBasics(_testProjectID, request));
        var result = await AssemblySteps.NormalHttpClient.PutAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task UpdateFeatured_Returns204_ForAdmin()
    {
        var request = new FeaturedProjectsUpdateRequest
        {
            ProjectIDs = new List<int> { _testProjectID }
        };

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.UpdateFeatured(request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        // NoContent = 204
        Assert.IsTrue(result.StatusCode == HttpStatusCode.NoContent || result.IsSuccessStatusCode,
            $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task UpdateFeatured_Returns403_ForNormalUser()
    {
        var request = new FeaturedProjectsUpdateRequest
        {
            ProjectIDs = new List<int>()
        };

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.UpdateFeatured(request));
        var result = await AssemblySteps.NormalHttpClient.PutAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    #endregion

    #region Create Workflow - Create Project from Basics Step

    [TestMethod]
    public async Task CreateProjectFromBasicsStep_Returns201_WithValidRequest()
    {
        var projectType = await AssemblySteps.DbContext.ProjectTypes.FirstAsync();

        var request = new ProjectBasicsStepRequest
        {
            ProjectName = $"HTTP Create Test {DateTime.UtcNow.Ticks}",
            ProjectDescription = "Test project created via HTTP",
            ProjectTypeID = projectType.ProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Planned,
            PlannedDate = DateOnly.FromDateTime(DateTime.Today),
            ProgramIDs = new List<int>(),
        };

        var route = "projects/create-workflow/steps/basics";
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Created, result.StatusCode,
            $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");

        var created = await result.DeserializeContentAsync<ProjectBasicsStep>();
        Assert.IsNotNull(created);
        Assert.IsTrue(created.ProjectID > 0);
        _createdProjectIDs.Add(created.ProjectID!.Value);
    }

    [TestMethod]
    public async Task CreateProjectFromBasicsStep_Returns401_WhenUnauthenticated()
    {
        var request = new ProjectBasicsStepRequest
        {
            ProjectName = "Should not be created",
            ProjectTypeID = 1,
            ProjectStageID = 1,
            ProgramIDs = new List<int>(),
        };

        var route = "projects/create-workflow/steps/basics";
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion

    #region Create Workflow State Transitions

    [TestMethod]
    public async Task SubmitCreate_Returns200_ForDraftProject()
    {
        var draft = await ProjectHelper.CreateDraftProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(draft.ProjectID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SubmitCreateForApproval(draft.ProjectID, null));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var response = await result.DeserializeContentAsync<WorkflowStateTransitionResponse>();
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success, $"Submit should succeed: {response.ErrorMessage}");
    }

    [TestMethod]
    public async Task ApproveCreate_Returns200_ForPendingProject()
    {
        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ApproveCreate(pending.ProjectID, null));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var response = await result.DeserializeContentAsync<WorkflowStateTransitionResponse>();
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success, $"Approve should succeed: {response.ErrorMessage}");
    }

    [TestMethod]
    public async Task ApproveCreate_Returns403_ForNormalUser()
    {
        // ProjectApproveFeature excludes Normal users
        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ApproveCreate(pending.ProjectID, null));
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task ReturnCreate_Returns200_ForPendingProject()
    {
        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        var request = new WorkflowStateTransitionRequest { Comment = "Needs more detail" };
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ReturnCreate(pending.ProjectID, request));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var response = await result.DeserializeContentAsync<WorkflowStateTransitionResponse>();
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success, $"Return should succeed: {response.ErrorMessage}");
    }

    [TestMethod]
    public async Task ReturnCreate_Returns403_ForNormalUser()
    {
        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ReturnCreate(pending.ProjectID, null));
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task RejectCreate_Returns200_ForPendingProject()
    {
        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        var request = new WorkflowStateTransitionRequest { Comment = "Does not meet criteria" };
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.RejectCreate(pending.ProjectID, request));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var response = await result.DeserializeContentAsync<WorkflowStateTransitionResponse>();
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success, $"Reject should succeed: {response.ErrorMessage}");
    }

    [TestMethod]
    public async Task RejectCreate_Returns403_ForNormalUser()
    {
        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.RejectCreate(pending.ProjectID, null));
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task WithdrawCreate_Returns200_ForPendingProject()
    {
        var pending = await ProjectHelper.CreatePendingApprovalProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);
        _createdProjectIDs.Add(pending.ProjectID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.WithdrawCreate(pending.ProjectID, null));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var response = await result.DeserializeContentAsync<WorkflowStateTransitionResponse>();
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success, $"Withdraw should succeed: {response.ErrorMessage}");
    }

    #endregion

    #region Update Workflow Tests

    [TestMethod]
    public async Task StartUpdateBatch_Returns200()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.StartUpdateBatch(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, null);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var batch = await result.DeserializeContentAsync<ProjectUpdateBatchDetail>();
        Assert.IsNotNull(batch);
    }

    [TestMethod]
    public async Task DeleteUpdateBatch_Returns204_WhenBatchExists()
    {
        // Start a batch first
        var startRoute = RouteHelper.GetRouteFor<ProjectController>(c => c.StartUpdateBatch(_testProjectID));
        var startResult = await AssemblySteps.AdminHttpClient.PostAsync(startRoute, null);
        Assert.IsTrue(startResult.IsSuccessStatusCode, $"Start batch failed: {await startResult.Content.ReadAsStringAsync()}");

        // Delete the batch
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.DeleteUpdateBatch(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode,
            $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task SubmitUpdate_Returns200_WhenBatchExists()
    {
        // Start a batch via the helper (uses the stored procedure)
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SubmitUpdateForApproval(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, null);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var response = await result.DeserializeContentAsync<WorkflowStateTransitionResponse>();
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success, $"Submit should succeed: {response.ErrorMessage}");
    }

    [TestMethod]
    public async Task ApproveUpdate_Returns200_WhenBatchSubmitted()
    {
        // Start and submit a batch
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ApproveUpdate(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.PostAsync(route, null);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var response = await result.DeserializeContentAsync<WorkflowStateTransitionResponse>();
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success, $"Approve should succeed: {response.ErrorMessage}");
    }

    [TestMethod]
    public async Task ApproveUpdate_Returns403_ForNormalUser()
    {
        // ProjectApproveFeature excludes Normal users
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ApproveUpdate(_testProjectID));
        var result = await AssemblySteps.NormalHttpClient.PostAsync(route, null);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [TestMethod]
    public async Task ReturnUpdate_Returns403_ForNormalUser()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ReturnUpdate(_testProjectID, null));
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, new { });

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    #endregion

    #region Authorization Tests

    [TestMethod]
    public async Task ListFeatured_Returns200_WhenUnauthenticated_BecauseAllowAnonymous()
    {
        // ProjectController.ListFeatured() has [AllowAnonymous]
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListFeatured());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"AllowAnonymous endpoint should succeed unauthenticated.\nRoute: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListLookup_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ListLookup());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task GetNoContactCount_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.GetNoContactCount());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task Delete_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.Delete(_testProjectID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task Get_ReturnsAllPermissionFlagsFalse_ForUnassignedUser()
    {
        // Unassigned user viewing an approved project via [ProjectViewFeature] (allows anonymous)
        // PopulatePermissionFlagsAsync should return early with all flags false
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Unassigned);
        _createdPersonIDs.Add(user.PersonID);

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.Get(_testProjectID));
        var result = await HttpResponseHelper.GetAsUserAsync(route, user.GlobalID!);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"ProjectViewFeature should allow Unassigned user to view approved project.\nRoute: {route}\nStatus: {result.StatusCode}\n{await result.Content.ReadAsStringAsync()}");

        var project = await result.DeserializeContentAsync<ProjectDetail>();
        Assert.IsNotNull(project);
        Assert.IsFalse(project.UserIsAdmin, "Unassigned user should not be admin");
        Assert.IsFalse(project.UserCanDelete, "Unassigned user should not be able to delete");
        Assert.IsFalse(project.UserCanApprove, "Unassigned user should not be able to approve");
        Assert.IsFalse(project.UserCanDirectEdit, "Unassigned user should not be able to direct-edit");
        Assert.IsFalse(project.UserCanViewCostSharePDFs, "Unassigned user should not view cost share PDFs");
    }

    #endregion

    #region SaveBasics Failure Tests

    [TestMethod]
    public async Task SaveBasics_Returns404_WhenProjectNotExists()
    {
        var request = new ProjectBasicsSaveRequest
        {
            ProjectTypeID = 1,
            ProjectName = "Should not save",
            ProjectStageID = 1,
            ProgramIDs = new List<int>(),
        };

        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SaveBasics(-1, request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Create Workflow Failure Tests

    [TestMethod]
    public async Task SubmitCreate_ReturnsBadRequest_WhenAlreadyApproved()
    {
        // _testProjectID is approved — can't submit an already-approved project
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.SubmitCreateForApproval(_testProjectID, null));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task ApproveCreate_ReturnsBadRequest_WhenAlreadyApproved()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ApproveCreate(_testProjectID, null));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task ReturnCreate_ReturnsBadRequest_WhenAlreadyApproved()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.ReturnCreate(_testProjectID, null));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task RejectCreate_ReturnsBadRequest_WhenAlreadyApproved()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.RejectCreate(_testProjectID, null));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task WithdrawCreate_ReturnsBadRequest_WhenAlreadyApproved()
    {
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.WithdrawCreate(_testProjectID, null));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, new WorkflowStateTransitionRequest());

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    #endregion

    #region Update Workflow Failure Tests

    [TestMethod]
    public async Task StartUpdateBatch_ReturnsBadRequest_WhenBatchAlreadyExists()
    {
        // Start a batch first
        var startRoute = RouteHelper.GetRouteFor<ProjectController>(c => c.StartUpdateBatch(_testProjectID));
        var startResult = await AssemblySteps.AdminHttpClient.PostAsync(startRoute, null);
        Assert.IsTrue(startResult.IsSuccessStatusCode, $"First batch start failed: {await startResult.Content.ReadAsStringAsync()}");

        // Try starting another — should fail with BadRequest (InvalidOperationException)
        var result = await AssemblySteps.AdminHttpClient.PostAsync(startRoute, null);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task DeleteUpdateBatch_Returns404_WhenNoBatchExists()
    {
        // _testProjectID has no active batch
        var route = RouteHelper.GetRouteFor<ProjectController>(c => c.DeleteUpdateBatch(_testProjectID));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion
}
