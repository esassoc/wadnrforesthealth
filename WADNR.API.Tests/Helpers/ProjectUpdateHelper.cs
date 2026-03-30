using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Helper methods for creating and managing project update workflows during tests.
/// </summary>
public static class ProjectUpdateHelper
{
    /// <summary>
    /// Starts a project update batch by calling the stored procedure.
    /// </summary>
    public static async Task<ProjectUpdateBatch?> StartBatchAsync(
        WADNRDbContext dbContext,
        int projectID,
        int callingPersonID)
    {
        // Call the stored procedure which returns the new batch ID
        var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "EXEC dbo.pStartProjectUpdateBatch @ProjectID, @CallingPersonID";
            command.Parameters.Add(new SqlParameter("@ProjectID", projectID));
            command.Parameters.Add(new SqlParameter("@CallingPersonID", callingPersonID));

            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            var batchID = Convert.ToInt32(result);

            // Clear the change tracker to get fresh data
            dbContext.ChangeTracker.Clear();

            return await dbContext.ProjectUpdateBatches
                .Include(b => b.Project)
                .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == batchID);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    /// <summary>
    /// Modifies the project description in the update batch.
    /// </summary>
    public static async Task ModifyProjectDescriptionAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        string newDescription)
    {
        // Clear change tracker to ensure fresh data from database
        dbContext.ChangeTracker.Clear();

        var projectUpdate = await dbContext.ProjectUpdates
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == projectUpdateBatchID);

        if (projectUpdate != null)
        {
            projectUpdate.ProjectDescription = newDescription;
            await dbContext.SaveChangesWithNoAuditingAsync();
        }
    }

    /// <summary>
    /// Modifies the project stage in the update batch.
    /// </summary>
    public static async Task ModifyProjectStageAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectStageEnum newStage)
    {
        // Clear change tracker to ensure fresh data from database
        dbContext.ChangeTracker.Clear();

        var projectUpdate = await dbContext.ProjectUpdates
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == projectUpdateBatchID);

        if (projectUpdate != null)
        {
            projectUpdate.ProjectStageID = (int)newStage;
            await dbContext.SaveChangesWithNoAuditingAsync();
        }
    }

    /// <summary>
    /// Adds a program to the update batch.
    /// </summary>
    public static async Task AddProgramAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int programID)
    {
        dbContext.ProjectUpdatePrograms.Add(new ProjectUpdateProgram
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            ProgramID = programID
        });
        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Removes all programs from the update batch.
    /// </summary>
    public static async Task ClearProgramsAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID)
    {
        await dbContext.ProjectUpdatePrograms
            .Where(p => p.ProjectUpdateBatchID == projectUpdateBatchID)
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Adds an organization to the update batch.
    /// </summary>
    public static async Task AddOrganizationAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int organizationID,
        int relationshipTypeID)
    {
        dbContext.ProjectOrganizationUpdates.Add(new ProjectOrganizationUpdate
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            OrganizationID = organizationID,
            RelationshipTypeID = relationshipTypeID
        });
        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Removes all organizations from the update batch.
    /// </summary>
    public static async Task ClearOrganizationsAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID)
    {
        await dbContext.ProjectOrganizationUpdates
            .Where(o => o.ProjectUpdateBatchID == projectUpdateBatchID)
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Adds a note to the update batch.
    /// </summary>
    public static async Task AddNoteAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        string noteText,
        int createPersonID)
    {
        dbContext.ProjectNoteUpdates.Add(new ProjectNoteUpdate
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Note = noteText,
            CreatePersonID = createPersonID,
            CreateDate = DateTime.UtcNow
        });
        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Removes all notes from the update batch.
    /// </summary>
    public static async Task ClearNotesAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID)
    {
        await dbContext.ProjectNoteUpdates
            .Where(n => n.ProjectUpdateBatchID == projectUpdateBatchID)
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Adds a contact/person to the update batch (not audited).
    /// </summary>
    public static async Task AddContactAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int personID,
        int relationshipTypeID)
    {
        dbContext.ProjectPersonUpdates.Add(new ProjectPersonUpdate
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            PersonID = personID,
            ProjectPersonRelationshipTypeID = relationshipTypeID
        });
        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Removes all contacts from the update batch.
    /// </summary>
    public static async Task ClearContactsAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID)
    {
        await dbContext.ProjectPersonUpdates
            .Where(c => c.ProjectUpdateBatchID == projectUpdateBatchID)
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Transitions the batch to Submitted state.
    /// </summary>
    public static async Task SubmitBatchAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
        {
            return;
        }

        batch.ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Submitted;
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        dbContext.ProjectUpdateHistories.Add(new ProjectUpdateHistory
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Submitted,
            TransitionDate = DateTime.UtcNow,
            UpdatePersonID = callingPersonID
        });

        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Approves the batch by calling the workflow method.
    /// This triggers the pCommitProjectUpdateToProject stored procedure and generates audit logs.
    /// </summary>
    public static async Task<WorkflowStateTransitionResponse> ApproveBatchAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int callingPersonID)
    {
        return await ProjectUpdateWorkflowSteps.ApproveAsync(dbContext, projectUpdateBatchID, callingPersonID);
    }

    /// <summary>
    /// Gets a batch by ID with fresh data.
    /// </summary>
    public static async Task<ProjectUpdateBatch?> GetBatchByIDAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID)
    {
        dbContext.ChangeTracker.Clear();

        return await dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);
    }
}
