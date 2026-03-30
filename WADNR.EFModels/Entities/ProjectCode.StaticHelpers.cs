using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProjectCode;

namespace WADNR.EFModels.Entities;

public static class ProjectCodes
{
    public static async Task<List<ProjectCodeGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ProjectCodes
            .AsNoTracking()
            .OrderBy(x => x.ProjectCodeName)
            .Select(ProjectCodeProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<ProjectCodeDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectCodeID)
    {
        return await dbContext.ProjectCodes
            .AsNoTracking()
            .Where(x => x.ProjectCodeID == projectCodeID)
            .Select(ProjectCodeProjections.AsDetail)
            .SingleOrDefaultAsync();
    }

    public static async Task<List<ProjectCodeLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ProjectCodes
            .AsNoTracking()
            .OrderBy(x => x.ProjectCodeName)
            .Select(ProjectCodeProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<List<ProjectCodeLookupItem>> SearchAsLookupItemAsync(WADNRDbContext dbContext, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<ProjectCodeLookupItem>();
        }

        var term = searchTerm.ToLower();
        return await dbContext.ProjectCodes
            .AsNoTracking()
            .Where(x =>
                x.ProjectCodeName.ToLower().Contains(term) ||
                (x.ProjectCodeTitle != null && x.ProjectCodeTitle.ToLower().Contains(term)))
            .OrderBy(x => x.ProjectCodeName)
            .Take(20)
            .Select(ProjectCodeProjections.AsLookupItem)
            .ToListAsync();
    }
}
