using Microsoft.EntityFrameworkCore;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Integration tests verifying that the pCommitProjectUpdateToProject stored procedure
/// creates the correct audit log entries when an update batch is approved.
/// </summary>
[TestClass]
[DoNotParallelize] // Tests must run sequentially - they share database state
public class ProjectUpdateAuditLogTests
{
    private int _testProjectID;
    private DateTime _testStartTime;

    // Audit Event Types from the database
    private const int AuditEventTypeAdded = 1;
    private const int AuditEventTypeDeleted = 2;
    private const int AuditEventTypeModified = 3;

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Clear any tracked entities from previous tests
        AssemblySteps.DbContext.ChangeTracker.Clear();

        // Use UTC to match GETUTCDATE() in the sproc
        _testStartTime = DateTime.UtcNow.AddSeconds(-1); // Small buffer for timing

        // Set the current user for audit logging
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        // Create a test project in Approved status
        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext,
            AssemblySteps.TestAdminPersonID);
        _testProjectID = project.ProjectID;

        // Clear any existing audit logs for this project
        await AuditLogTestHelper.ClearAuditLogsForProjectAsync(
            AssemblySteps.DbContext,
            _testProjectID);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        // Clean up test project and all related data
        await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, _testProjectID);
    }

    #region Project Scalar Field Tests

    [TestMethod]
    public async Task ApproveUpdate_ProjectDescriptionChange_CreatesModifiedAuditLog()
    {
        // Arrange
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        const string newDescription = "Updated description for audit log testing";
        await ProjectUpdateHelper.ModifyProjectDescriptionAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, newDescription);

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Act
        var result = await ProjectUpdateHelper.ApproveBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsTrue(result.Success, $"Approve failed: {result.ErrorMessage}");

        var auditLogs = await AuditLogTestHelper.GetAuditLogsForTableAndEventTypeAsync(
            AssemblySteps.DbContext, _testProjectID, "Project", AuditEventTypeModified, _testStartTime);

        var descriptionLog = auditLogs.FirstOrDefault(al => al.ColumnName == "ProjectDescription");
        Assert.IsNotNull(descriptionLog, "Should have audit log for ProjectDescription change");
        Assert.AreEqual(newDescription, descriptionLog.NewValue);
        Assert.AreEqual(AssemblySteps.TestAdminPersonID, descriptionLog.PersonID);
    }

    [TestMethod]
    public async Task ApproveUpdate_ProjectStageChange_CreatesModifiedAuditLogWithDescription()
    {
        // Arrange
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        await ProjectUpdateHelper.ModifyProjectStageAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, ProjectStageEnum.Implementation);

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Act
        var result = await ProjectUpdateHelper.ApproveBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsTrue(result.Success, $"Approve failed: {result.ErrorMessage}");

        var auditLogs = await AuditLogTestHelper.GetAuditLogsForTableAndEventTypeAsync(
            AssemblySteps.DbContext, _testProjectID, "Project", AuditEventTypeModified, _testStartTime);

        var stageLog = auditLogs.FirstOrDefault(al => al.ColumnName == "ProjectStageID");
        Assert.IsNotNull(stageLog, "Should have audit log for ProjectStageID change");
        Assert.AreEqual(((int)ProjectStageEnum.Implementation).ToString(), stageLog.NewValue);
        Assert.IsNotNull(stageLog.AuditDescription, "ProjectStageID change should have AuditDescription");
        Assert.IsTrue(stageLog.AuditDescription.Contains("Implementation"),
            $"AuditDescription should contain stage name: {stageLog.AuditDescription}");
    }

    #endregion

    #region Program Tests

    [TestMethod]
    public async Task ApproveUpdate_AddProgram_CreatesAddedAuditLog()
    {
        // Arrange - Start with no programs
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        // Get a program that isn't already on the project
        var existingProgramIDs = await AssemblySteps.DbContext.ProjectUpdatePrograms
            .Where(p => p.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .Select(p => p.ProgramID)
            .ToListAsync();

        var newProgram = await AssemblySteps.DbContext.Programs
            .Where(p => !existingProgramIDs.Contains(p.ProgramID))
            .FirstOrDefaultAsync();

        if (newProgram == null)
        {
            Assert.Inconclusive("No available programs to add for testing");
            return;
        }

        await ProjectUpdateHelper.AddProgramAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, newProgram.ProgramID);

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Act
        var result = await ProjectUpdateHelper.ApproveBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsTrue(result.Success, $"Approve failed: {result.ErrorMessage}");

        var auditLogs = await AuditLogTestHelper.GetAuditLogsForTableAndEventTypeAsync(
            AssemblySteps.DbContext, _testProjectID, "ProjectProgram", AuditEventTypeAdded, _testStartTime);

        var addedLog = auditLogs.FirstOrDefault(al =>
            al.ColumnName == "ProgramID" &&
            al.NewValue == newProgram.ProgramID.ToString());

        Assert.IsNotNull(addedLog, "Should have audit log for added ProjectProgram");
    }

    [TestMethod]
    public async Task ApproveUpdate_RemoveProgram_CreatesDeletedAuditLog()
    {
        // Arrange - Ensure there's a program to remove
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        // Check if there are programs in the batch
        var batchPrograms = await AssemblySteps.DbContext.ProjectUpdatePrograms
            .Where(p => p.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        if (batchPrograms.Count == 0)
        {
            // Add a program first, then remove it
            var program = await AssemblySteps.DbContext.Programs.FirstAsync();
            await ProjectUpdateHelper.AddProgramAsync(
                AssemblySteps.DbContext, batch.ProjectUpdateBatchID, program.ProgramID);

            // Update the batch to commit the program, then start a new batch
            await ProjectUpdateHelper.SubmitBatchAsync(
                AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);
            await ProjectUpdateHelper.ApproveBatchAsync(
                AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

            // Clear audit logs and start a new batch
            await AuditLogTestHelper.ClearAuditLogsForProjectAsync(AssemblySteps.DbContext, _testProjectID);
            _testStartTime = DateTime.UtcNow.AddSeconds(-1);

            batch = await ProjectUpdateHelper.StartBatchAsync(
                AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
            Assert.IsNotNull(batch, "Failed to start second update batch");
        }

        // Now remove all programs from the update
        await ProjectUpdateHelper.ClearProgramsAsync(AssemblySteps.DbContext, batch.ProjectUpdateBatchID);

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Act
        var result = await ProjectUpdateHelper.ApproveBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsTrue(result.Success, $"Approve failed: {result.ErrorMessage}");

        var auditLogs = await AuditLogTestHelper.GetAuditLogsForTableAndEventTypeAsync(
            AssemblySteps.DbContext, _testProjectID, "ProjectProgram", AuditEventTypeDeleted, _testStartTime);

        Assert.IsTrue(auditLogs.Count > 0, "Should have audit log(s) for deleted ProjectProgram");
        Assert.IsTrue(auditLogs.All(al => al.ColumnName == "*ALL"),
            "Deleted audit logs should have ColumnName = '*ALL'");
    }

    #endregion

    #region Organization Tests

    [TestMethod]
    public async Task ApproveUpdate_AddOrganization_CreatesAddedAuditLogs()
    {
        // Arrange
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        // Get an organization to add
        var organization = await AssemblySteps.DbContext.Organizations.FirstAsync();
        var relationshipType = await AssemblySteps.DbContext.RelationshipTypes.FirstAsync();

        await ProjectUpdateHelper.AddOrganizationAsync(
            AssemblySteps.DbContext,
            batch.ProjectUpdateBatchID,
            organization.OrganizationID,
            relationshipType.RelationshipTypeID);

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Act
        var result = await ProjectUpdateHelper.ApproveBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsTrue(result.Success, $"Approve failed: {result.ErrorMessage}");

        var auditLogs = await AuditLogTestHelper.GetAuditLogsForTableAndEventTypeAsync(
            AssemblySteps.DbContext, _testProjectID, "ProjectOrganization", AuditEventTypeAdded, _testStartTime);

        // Consolidated: single entry per org with ColumnName = 'OrganizationID'
        // AuditDescription contains both org name and relationship type
        var orgLog = auditLogs.FirstOrDefault(al => al.ColumnName == "OrganizationID");

        Assert.IsNotNull(orgLog, "Should have audit log for OrganizationID");
        Assert.IsNotNull(orgLog.AuditDescription, "OrganizationID audit log should have AuditDescription");
        Assert.IsTrue(orgLog.AuditDescription.Contains(organization.OrganizationName),
            $"AuditDescription should contain org name. Was: {orgLog.AuditDescription}");
        Assert.IsTrue(orgLog.AuditDescription.Contains(relationshipType.RelationshipTypeName),
            $"AuditDescription should contain relationship type name. Was: {orgLog.AuditDescription}");
    }

    #endregion

    #region Note Tests

    [TestMethod]
    public async Task ApproveUpdate_AddNote_CreatesAddedAuditLogs()
    {
        // Arrange
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        const string noteText = "Test note for audit log verification";
        await ProjectUpdateHelper.AddNoteAsync(
            AssemblySteps.DbContext,
            batch.ProjectUpdateBatchID,
            noteText,
            AssemblySteps.TestAdminPersonID);

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Act
        var result = await ProjectUpdateHelper.ApproveBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsTrue(result.Success, $"Approve failed: {result.ErrorMessage}");

        var auditLogs = await AuditLogTestHelper.GetAuditLogsForTableAndEventTypeAsync(
            AssemblySteps.DbContext, _testProjectID, "ProjectNote", AuditEventTypeAdded, _testStartTime);

        var noteLog = auditLogs.FirstOrDefault(al => al.ColumnName == "Note");
        Assert.IsNotNull(noteLog, "Should have audit log for Note");
        Assert.AreEqual(noteText, noteLog.NewValue);
    }

    #endregion

    #region Negative Tests

    [TestMethod]
    public async Task ApproveUpdate_ProjectPerson_NotAudited()
    {
        // Arrange - ProjectPerson is in IgnoredTables list, should not be audited
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        // Get a person and use a lookup enum for the relationship type
        var person = await AssemblySteps.DbContext.People.FirstAsync();

        await ProjectUpdateHelper.AddContactAsync(
            AssemblySteps.DbContext,
            batch.ProjectUpdateBatchID,
            person.PersonID,
            (int)ProjectPersonRelationshipTypeEnum.PrimaryContact);

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Act
        var result = await ProjectUpdateHelper.ApproveBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsTrue(result.Success, $"Approve failed: {result.ErrorMessage}");

        // ProjectPerson should NOT have audit logs (it's in IgnoredTables)
        var auditLogs = await AuditLogTestHelper.GetAuditLogsForTableAsync(
            AssemblySteps.DbContext, _testProjectID, "ProjectPerson", _testStartTime);

        Assert.AreEqual(0, auditLogs.Count,
            "ProjectPerson should NOT be audited (it's in IgnoredTables list in the sproc)");
    }

    [TestMethod]
    public async Task StartBatch_CreatesWorkflowStateAuditLog()
    {
        // Arrange - Starting a batch via the workflow layer creates a "Created" audit entry
        await AuditLogTestHelper.ClearAuditLogsForProjectAsync(AssemblySteps.DbContext, _testProjectID);
        var startTime = DateTime.UtcNow.AddSeconds(-1);

        // Act - Use the workflow method (not the test helper) so the EF audit entry is created
        var batch = await ProjectUpdateWorkflowSteps.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsNotNull(batch, "Failed to start update batch");

        var auditLogs = await AuditLogTestHelper.GetAuditLogsForProjectAsync(
            AssemblySteps.DbContext, _testProjectID, startTime);

        Assert.AreEqual(1, auditLogs.Count,
            $"Starting a batch should create exactly 1 workflow state audit log. Found {auditLogs.Count} logs.");

        var log = auditLogs.First();
        Assert.AreEqual("Project Update", log.TableName);
        Assert.AreEqual("Created", log.NewValue);
    }

    [TestMethod]
    public async Task ApproveUpdate_NoChanges_NoAuditLogs()
    {
        // Arrange - Start and approve a batch without making any changes
        var batch = await ProjectUpdateHelper.StartBatchAsync(
            AssemblySteps.DbContext, _testProjectID, AssemblySteps.TestAdminPersonID);
        Assert.IsNotNull(batch, "Failed to start update batch");

        await ProjectUpdateHelper.SubmitBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Act
        var result = await ProjectUpdateHelper.ApproveBatchAsync(
            AssemblySteps.DbContext, batch.ProjectUpdateBatchID, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsTrue(result.Success, $"Approve failed: {result.ErrorMessage}");

        // When no changes are made, Modified event type should have 0 logs for Project table
        // (child tables may still have delete/insert logs due to full delete-and-reinsert pattern)
        var modifiedLogs = await AuditLogTestHelper.GetAuditLogsForTableAndEventTypeAsync(
            AssemblySteps.DbContext, _testProjectID, "Project", AuditEventTypeModified, _testStartTime);

        Assert.AreEqual(0, modifiedLogs.Count,
            $"Expected 0 Modified audit logs for Project table when no changes made. Found {modifiedLogs.Count}.");
    }

    #endregion
}
