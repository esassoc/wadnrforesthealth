using System.Net;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// HTTP integration tests for ProjectInternalNoteController CRUD operations.
/// All endpoints require [ProjectEditAsAdminFeature].
/// </summary>
[TestClass]
[DoNotParallelize]
public class ProjectInternalNoteControllerHttpTests
{
    private int _testProjectID;
    private readonly List<int> _createdNoteIDs = new();

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
        try
        {
            await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, _testProjectID);
        }
        catch { /* best effort */ }
    }

    [TestMethod]
    public async Task Create_Returns201_WithValidRequest()
    {
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Create(null!));
        var request = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = "Test internal note from integration test"
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Created, result.StatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");

        var detail = await result.DeserializeContentAsync<ProjectInternalNoteDetail>();
        Assert.IsNotNull(detail);
        Assert.IsTrue(detail.ProjectInternalNoteID > 0);
        _createdNoteIDs.Add(detail.ProjectInternalNoteID);
    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNoteEmpty()
    {
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Create(null!));
        var request = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = ""
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNoteTooLong()
    {
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Create(null!));
        var request = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = new string('x', 8001)
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task Create_ReturnsNotFound_WhenProjectNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Create(null!));
        var request = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = -1,
            Note = "Note for nonexistent project"
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task CreateThenGetThenUpdateThenDelete_FullCRUDCycle()
    {
        // CREATE
        var createRoute = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Create(null!));
        var createRequest = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = "Original note text"
        };

        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, createRequest);
        Assert.AreEqual(HttpStatusCode.Created, createResult.StatusCode);

        var created = await createResult.DeserializeContentAsync<ProjectInternalNoteDetail>();
        Assert.IsNotNull(created);
        var noteID = created.ProjectInternalNoteID;
        _createdNoteIDs.Add(noteID);

        // GET
        var getRoute = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.GetByID(noteID));
        var getResult = await AssemblySteps.AdminHttpClient.GetAsync(getRoute);
        Assert.IsTrue(getResult.IsSuccessStatusCode);

        var fetched = await getResult.DeserializeContentAsync<ProjectInternalNoteDetail>();
        Assert.IsNotNull(fetched);
        Assert.AreEqual("Original note text", fetched.Note);

        // UPDATE
        var updateRoute = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Update(noteID, null!));
        var updateRequest = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = "Updated note text"
        };

        var updateResult = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(updateRoute, updateRequest);
        Assert.IsTrue(updateResult.IsSuccessStatusCode, $"Update failed: {await updateResult.Content.ReadAsStringAsync()}");

        var updated = await updateResult.DeserializeContentAsync<ProjectInternalNoteDetail>();
        Assert.IsNotNull(updated);
        Assert.AreEqual("Updated note text", updated.Note);

        // DELETE
        var deleteRoute = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Delete(noteID));
        var deleteResult = await AssemblySteps.AdminHttpClient.DeleteAsync(deleteRoute);
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResult.StatusCode);

        // Verify deleted
        var verifyResult = await AssemblySteps.AdminHttpClient.GetAsync(getRoute);
        Assert.AreEqual(HttpStatusCode.NotFound, verifyResult.StatusCode);

        _createdNoteIDs.Remove(noteID);
    }

    [TestMethod]
    public async Task Update_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Update(-1, null!));
        var request = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = "Update for nonexistent note"
        };

        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task Delete_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Delete(-1));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task Create_ReturnsBadRequest_WhenNoteWhitespaceOnly()
    {
        // Whitespace-only passes [Required] but fails controller's IsNullOrWhiteSpace check
        var route = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Create(null!));
        var request = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = "   "
        };

        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_WhenNoteTooLong()
    {
        // Create a valid note first
        var createRoute = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Create(null!));
        var createRequest = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = "Valid note for too-long update test"
        };
        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, createRequest);
        Assert.AreEqual(HttpStatusCode.Created, createResult.StatusCode);

        var created = await createResult.DeserializeContentAsync<ProjectInternalNoteDetail>();
        Assert.IsNotNull(created);
        _createdNoteIDs.Add(created.ProjectInternalNoteID);

        // Try updating with a note that exceeds 8000 characters
        var updateRoute = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Update(created.ProjectInternalNoteID, null!));
        var updateRequest = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = new string('x', 8001)
        };

        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(updateRoute, updateRequest);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task Update_ReturnsBadRequest_WhenNoteWhitespaceOnly()
    {
        // Create a valid note first
        var createRoute = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Create(null!));
        var createRequest = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = "Valid note for whitespace update test"
        };
        var createResult = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(createRoute, createRequest);
        Assert.AreEqual(HttpStatusCode.Created, createResult.StatusCode);

        var created = await createResult.DeserializeContentAsync<ProjectInternalNoteDetail>();
        Assert.IsNotNull(created);
        _createdNoteIDs.Add(created.ProjectInternalNoteID);

        // Try updating with whitespace-only note
        var updateRoute = RouteHelper.GetRouteFor<ProjectInternalNoteController>(c => c.Update(created.ProjectInternalNoteID, null!));
        var updateRequest = new ProjectInternalNoteUpsertRequest
        {
            ProjectID = _testProjectID,
            Note = "   "
        };

        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(updateRoute, updateRequest);

        Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
    }
}
