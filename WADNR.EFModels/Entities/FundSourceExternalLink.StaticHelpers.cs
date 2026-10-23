using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceExternalLinks
{
    public static async Task<List<FundSourceExternalLinkGridRow>> ListForFundSourceAsGridRowAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        var links = await dbContext.FundSourceExternalLinks
            .AsNoTracking()
            .Where(l => l.FundSourceID == fundSourceID)
            .Select(FundSourceExternalLinkProjections.AsGridRow)
            .OrderBy(l => l.ExternalLinkLabel)
            .ToListAsync();

        return links;
    }

    public static async Task<List<FundSourceExternalLinkGridRow>> SaveAllAsync(WADNRDbContext dbContext, int fundSourceID, FundSourceExternalLinkSaveRequest request)
    {
        var existing = await dbContext.FundSourceExternalLinks
            .Where(l => l.FundSourceID == fundSourceID)
            .ToListAsync();

        var requestIDs = request.ExternalLinks
            .Where(r => r.FundSourceExternalLinkID.HasValue)
            .Select(r => r.FundSourceExternalLinkID!.Value)
            .ToHashSet();

        // Delete links not in request
        var toDelete = existing.Where(e => !requestIDs.Contains(e.FundSourceExternalLinkID)).ToList();
        dbContext.FundSourceExternalLinks.RemoveRange(toDelete);

        foreach (var item in request.ExternalLinks)
        {
            if (item.FundSourceExternalLinkID.HasValue)
            {
                // Update existing
                var existingLink = existing.FirstOrDefault(e => e.FundSourceExternalLinkID == item.FundSourceExternalLinkID.Value);
                if (existingLink != null)
                {
                    existingLink.ExternalLinkLabel = item.ExternalLinkLabel;
                    existingLink.ExternalLinkUrl = item.ExternalLinkUrl;
                }
            }
            else
            {
                // Create new
                dbContext.FundSourceExternalLinks.Add(new FundSourceExternalLink
                {
                    FundSourceID = fundSourceID,
                    ExternalLinkLabel = item.ExternalLinkLabel,
                    ExternalLinkUrl = item.ExternalLinkUrl
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return await ListForFundSourceAsGridRowAsync(dbContext, fundSourceID);
    }
}
