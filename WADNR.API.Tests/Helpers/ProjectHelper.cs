using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Helper methods for creating and managing test projects.
/// </summary>
public static class ProjectHelper
{
    /// <summary>
    /// Creates an approved project with minimal required data for testing update workflows.
    /// </summary>
    public static async Task<Project> CreateApprovedProjectAsync(
        WADNRDbContext dbContext,
        int createPersonID,
        int? programID = null)
    {
        // Generate a unique FhtProjectNumber
        var projectNumber = $"TST{DateTime.UtcNow.Ticks % 1000000:000000}";

        var project = new Project
        {
            ProjectTypeID = 1, // Will need to be set to a valid ProjectTypeID
            ProjectStageID = (int)ProjectStageEnum.Planned,
            ProjectName = $"Test Project {projectNumber}",
            ProjectDescription = "Test project for audit log verification",
            ProjectLocationSimpleTypeID = 1, // Will need to be set to a valid type
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            FhtProjectNumber = projectNumber,
            ProposingPersonID = createPersonID,
            ProposingDate = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            PlannedDate = DateOnly.FromDateTime(DateTime.Today),
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesWithNoAuditingAsync();

        // Add program if specified
        if (programID.HasValue)
        {
            dbContext.ProjectPrograms.Add(new ProjectProgram
            {
                ProjectID = project.ProjectID,
                ProgramID = programID.Value
            });
            await dbContext.SaveChangesWithNoAuditingAsync();
        }

        return project;
    }

    /// <summary>
    /// Creates an approved project using existing lookup data from the database.
    /// </summary>
    public static async Task<Project> CreateApprovedProjectWithValidLookupsAsync(
        WADNRDbContext dbContext,
        int createPersonID)
    {
        // Get a valid ProjectType
        var projectType = await dbContext.ProjectTypes.FirstAsync();

        // Get a valid Program (optional)
        var program = await dbContext.Programs.FirstOrDefaultAsync();

        // Verify the person exists, or get any existing person
        var personExists = await dbContext.People.AnyAsync(p => p.PersonID == createPersonID);
        int? proposingPersonID = null;
        if (personExists)
        {
            proposingPersonID = createPersonID;
        }
        else
        {
            // Get any existing person from the database
            var person = await dbContext.People.FirstOrDefaultAsync();
            if (person != null)
            {
                proposingPersonID = person.PersonID;
            }
        }

        // Generate a unique FhtProjectNumber
        var projectNumber = $"TST{DateTime.UtcNow.Ticks % 1000000:000000}";

        var project = new Project
        {
            ProjectTypeID = projectType.ProjectTypeID,
            ProjectStageID = (int)ProjectStageEnum.Planned,
            ProjectName = $"Test Project {projectNumber}",
            ProjectDescription = "Test project for audit log verification",
            ProjectLocationSimpleTypeID = 1, // 1 = "None" (lookup table hardcoded value)
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            FhtProjectNumber = projectNumber,
            ProposingPersonID = proposingPersonID,
            ProposingDate = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            PlannedDate = DateOnly.FromDateTime(DateTime.Today),
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesWithNoAuditingAsync();

        // Add program if found
        if (program != null)
        {
            dbContext.ProjectPrograms.Add(new ProjectProgram
            {
                ProjectID = project.ProjectID,
                ProgramID = program.ProgramID
            });
            await dbContext.SaveChangesWithNoAuditingAsync();
        }

        return project;
    }

    /// <summary>
    /// Deletes a project and all related data. Uses direct SQL for efficiency.
    /// </summary>
    public static async Task DeleteProjectAsync(WADNRDbContext dbContext, int projectID)
    {
        // Delete in FK-safe order (reverse of creation)
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AuditLog WHERE ProjectID = {projectID}");

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectUpdateHistory WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");

        // Delete all update tables
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.TreatmentUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectLocationUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectLocationStagingUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectUpdateProgram WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectPriorityLandscapeUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectRegionUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectCountyUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectOrganizationUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectPersonUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectFundingSourceUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectFundSourceAllocationRequestUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectImageUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectExternalLinkUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectDocumentUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectNoteUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID})");

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectUpdateBatch WHERE ProjectID = {projectID}");

        // Delete project related data
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Treatment WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectLocation WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectLocationStaging WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectProgram WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectPriorityLandscape WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectRegion WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectCounty WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectOrganization WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectPerson WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectFundingSource WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectFundSourceAllocationRequest WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectImage WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectExternalLink WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectDocument WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectNote WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectClassification WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectTag WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectInternalNote WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.NotificationProject WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.InteractionEventProject WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.AgreementProject WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProjectImportBlockList WHERE ProjectID = {projectID}");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.ProgramNotificationSentProject WHERE ProjectID = {projectID}");

        // Delete InvoicePaymentRequest and related Invoice data
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Invoice WHERE InvoicePaymentRequestID IN (SELECT InvoicePaymentRequestID FROM dbo.InvoicePaymentRequest WHERE ProjectID = {projectID})");
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.InvoicePaymentRequest WHERE ProjectID = {projectID}");

        // Finally delete the project
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.Project WHERE ProjectID = {projectID}");
    }

    /// <summary>
    /// Gets a fresh project instance from the database.
    /// </summary>
    public static async Task<Project?> GetByIDAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);
    }
}
