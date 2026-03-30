using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AgreementTypes
{
    public static async Task<List<AgreementTypeLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.AgreementTypes
            .AsNoTracking()
            .Select(AgreementTypeProjections.AsLookupItem)
            .OrderBy(x => x.AgreementTypeName)
            .ToListAsync();
    }
}
