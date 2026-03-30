using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectClassifications
{
    public static async Task<List<ProjectClassificationDetailItem>> ListForProjectAsDetailItemAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.ProjectClassifications
            .AsNoTracking()
            .Where(pc => pc.ProjectID == projectID)
            .Select(pc => new ProjectClassificationDetailItem
            {
                ProjectClassificationID = pc.ProjectClassificationID,
                ClassificationID = pc.ClassificationID,
                ClassificationName = pc.Classification.DisplayName,
                ClassificationSystemID = pc.Classification.ClassificationSystemID,
                ClassificationSystemName = pc.Classification.ClassificationSystem.ClassificationSystemName,
                ProjectClassificationNotes = pc.ProjectClassificationNotes
            })
            .OrderBy(pc => pc.ClassificationSystemName)
            .ThenBy(pc => pc.ClassificationName)
            .ToListAsync();
    }

    public static async Task<List<ProjectClassificationExcelRow>> ListAllAsExcelRowAsync(WADNRDbContext dbContext, List<int> projectIDs)
    {
        return await dbContext.ProjectClassifications
            .AsNoTracking()
            .Where(pc => projectIDs.Contains(pc.ProjectID))
            .Select(pc => new ProjectClassificationExcelRow
            {
                ProjectID = pc.ProjectID,
                ProjectName = pc.Project.ProjectName,
                ClassificationName = pc.Classification.DisplayName
            })
            .OrderBy(pc => pc.ProjectID)
            .ThenBy(pc => pc.ClassificationName)
            .ToListAsync();
    }

    public static async Task<List<ProjectClassificationDetailItem>> SaveAllAsync(WADNRDbContext dbContext, int projectID, ProjectClassificationSaveRequest request)
    {
        var existing = await dbContext.ProjectClassifications
            .Where(pc => pc.ProjectID == projectID)
            .ToListAsync();

        var requestIDs = request.Classifications
            .Where(r => r.ProjectClassificationID.HasValue)
            .Select(r => r.ProjectClassificationID!.Value)
            .ToHashSet();

        // Delete classifications not in request
        var toDelete = existing.Where(e => !requestIDs.Contains(e.ProjectClassificationID)).ToList();
        dbContext.ProjectClassifications.RemoveRange(toDelete);

        foreach (var item in request.Classifications)
        {
            if (item.ProjectClassificationID.HasValue)
            {
                // Update existing
                var existingItem = existing.FirstOrDefault(e => e.ProjectClassificationID == item.ProjectClassificationID.Value);
                if (existingItem != null)
                {
                    existingItem.ProjectClassificationNotes = item.ProjectClassificationNotes;
                }
            }
            else
            {
                // Create new
                dbContext.ProjectClassifications.Add(new ProjectClassification
                {
                    ProjectID = projectID,
                    ClassificationID = item.ClassificationID,
                    ProjectClassificationNotes = item.ProjectClassificationNotes
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return await ListForProjectAsDetailItemAsync(dbContext, projectID);
    }
}
