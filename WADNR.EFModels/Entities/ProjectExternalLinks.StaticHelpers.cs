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
            .Select(l => new ProjectExternalLinkGridRow
            {
                ProjectExternalLinkID = l.ProjectExternalLinkID,
                ExternalLinkLabel = l.ExternalLinkLabel,
                ExternalLinkUrl = l.ExternalLinkUrl
            })
            .OrderBy(l => l.ExternalLinkLabel)
            .ToListAsync();

        return links;
    }
}
