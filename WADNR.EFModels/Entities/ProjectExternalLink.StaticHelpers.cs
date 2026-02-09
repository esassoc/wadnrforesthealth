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
}
