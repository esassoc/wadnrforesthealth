using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectImages
{
    public static async Task<List<ProjectImageGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.ProjectImages
            .AsNoTracking()
            .Where(x => x.ProjectID == projectID)
            .OrderByDescending(x => x.IsKeyPhoto)
            .ThenByDescending(x => x.FileResource.CreateDate)
            .Select(ProjectImageProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<ProjectImageDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectImageID)
    {
        return await dbContext.ProjectImages
            .AsNoTracking()
            .Where(x => x.ProjectImageID == projectImageID)
            .Select(ProjectImageProjections.AsDetail)
            .SingleOrDefaultAsync();
    }
}
