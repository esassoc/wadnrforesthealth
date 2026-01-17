using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.FileResource;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WADNR.EFModels.Entities;

public static class FileResources
{
    public static async Task<List<FileResourcePriorityLandscapeDetail>> ListForPriorityLandscapeIDAsync(WADNRDbContext dbContext, int priorityLandscapeID)
    {
        return await dbContext.PriorityLandscapeFileResources
            .AsNoTracking()
            .Where(x => x.PriorityLandscapeID == priorityLandscapeID)
            .Select(FileResourceProjections.AsPriorityLandscapeDetail)
            .ToListAsync();
    }

    public static async Task<List<FileResourceInteractionEventDetail>> ListForInteractionEventIDAsync(WADNRDbContext dbContext, int interactionEventID)
    {
        return await dbContext.InteractionEventFileResources
            .AsNoTracking()
            .Where(x => x.InteractionEventID == interactionEventID)
            .Select(FileResourceProjections.AsInteractionEventDetail)
            .ToListAsync();
    }
}
