using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectExternalLinks
{
    public static async Task<List<ProjectExternalLinkGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var links = await dbContext.ProjectExternalLinks
            .AsNoTracking()
            .Where(l => l.ProjectID == projectID)
            .Select(ProjectExternalLinkProjections.AsGridRow)
            .OrderBy(l => l.ExternalLinkLabel)
            .ToListAsync();

        return links;
    }

    public static async Task<List<ProjectExternalLinkGridRow>> SaveAllAsync(WADNRDbContext dbContext, int projectID, ProjectExternalLinkSaveRequest request)
    {
        var existing = await dbContext.ProjectExternalLinks
            .Where(l => l.ProjectID == projectID)
            .ToListAsync();

        var requestIDs = request.ExternalLinks
            .Where(r => r.ProjectExternalLinkID.HasValue)
            .Select(r => r.ProjectExternalLinkID!.Value)
            .ToHashSet();

        // Delete links not in request
        var toDelete = existing.Where(e => !requestIDs.Contains(e.ProjectExternalLinkID)).ToList();
        dbContext.ProjectExternalLinks.RemoveRange(toDelete);

        foreach (var item in request.ExternalLinks)
        {
            if (item.ProjectExternalLinkID.HasValue)
            {
                // Update existing
                var existingLink = existing.FirstOrDefault(e => e.ProjectExternalLinkID == item.ProjectExternalLinkID.Value);
                if (existingLink != null)
                {
                    existingLink.ExternalLinkLabel = item.ExternalLinkLabel;
                    existingLink.ExternalLinkUrl = item.ExternalLinkUrl;
                }
            }
            else
            {
                // Create new
                dbContext.ProjectExternalLinks.Add(new ProjectExternalLink
                {
                    ProjectID = projectID,
                    ExternalLinkLabel = item.ExternalLinkLabel,
                    ExternalLinkUrl = item.ExternalLinkUrl
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return await ListForProjectAsGridRowAsync(dbContext, projectID);
    }
}
