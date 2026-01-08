using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocations
{
    public static async Task<List<FundSourceAllocationDNRUplandRegionDetailGridRow>> ListByDnrUplandRegionActiveAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {
        return await dbContext.FundSourceAllocations
            .Include(x => x.FundSource)
            .Where(x => x.DNRUplandRegionID == dnrUplandRegionID && x.FundSource.FundSourceStatusID == (int)FundSourceStatusEnum.Active)
            .OrderByDescending(x => x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityNumber : 0)
            .Select(FundSourceAllocationProjections.AsDnrUplandRegionDetailGridRow)
            .ToListAsync();
    }
}
