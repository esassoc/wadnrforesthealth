using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.FileResource;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WADNR.EFModels.Entities;

public static class FileResources
{
    public static async Task<List<FileResourceDetail>> ListForPriorityLandscapeIDAsync(WADNRDbContext dbContext, int priorityLandscapeID)
    {
        return await dbContext.PriorityLandscapeFileResources
            .AsNoTracking()
            .Where(x => x.PriorityLandscapeID == priorityLandscapeID)
            .Select(FileResourceProjections.AsDetail)
            .ToListAsync();
    }
}
