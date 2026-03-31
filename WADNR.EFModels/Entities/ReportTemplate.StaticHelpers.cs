using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ReportTemplates
{
    public static async Task<List<ReportTemplateGridRow>> ListAsGridRowsAsync(WADNRDbContext dbContext)
    {
        var rows = await dbContext.ReportTemplates
            .AsNoTracking()
            .Select(ReportTemplateProjections.AsGridRow)
            .ToListAsync();

        foreach (var row in rows)
        {
            if (ReportTemplateModel.AllLookupDictionary.TryGetValue(row.ReportTemplateModelID, out var model))
            {
                row.ReportTemplateModelDisplayName = model.ReportTemplateModelDisplayName;
            }
        }

        return rows;
    }

    public static async Task<ReportTemplateDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int reportTemplateID)
    {
        var detail = await dbContext.ReportTemplates
            .AsNoTracking()
            .Where(x => x.ReportTemplateID == reportTemplateID)
            .Select(ReportTemplateProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (detail != null)
        {
            if (ReportTemplateModel.AllLookupDictionary.TryGetValue(detail.ReportTemplateModelID, out var model))
            {
                detail.ReportTemplateModelDisplayName = model.ReportTemplateModelDisplayName;
            }

            if (ReportTemplateModelType.AllLookupDictionary.TryGetValue(detail.ReportTemplateModelTypeID, out var modelType))
            {
                detail.ReportTemplateModelTypeDisplayName = modelType.ReportTemplateModelTypeDisplayName;
            }
        }

        return detail;
    }

    public static async Task<ReportTemplate?> GetByIDWithTrackingAsync(WADNRDbContext dbContext, int reportTemplateID)
    {
        return await dbContext.ReportTemplates
            .Include(x => x.FileResource)
            .SingleOrDefaultAsync(x => x.ReportTemplateID == reportTemplateID);
    }

    public static async Task<ReportTemplate?> GetByIDAsync(WADNRDbContext dbContext, int reportTemplateID)
    {
        return await dbContext.ReportTemplates
            .AsNoTracking()
            .Include(x => x.FileResource)
            .SingleOrDefaultAsync(x => x.ReportTemplateID == reportTemplateID);
    }

    public static async Task<List<ReportTemplateLookupItem>> ListByModelIDAsLookupItemsAsync(WADNRDbContext dbContext, int reportTemplateModelID)
    {
        return await dbContext.ReportTemplates
            .AsNoTracking()
            .Where(x => x.ReportTemplateModelID == reportTemplateModelID)
            .Select(x => new ReportTemplateLookupItem
            {
                ReportTemplateID = x.ReportTemplateID,
                DisplayName = x.DisplayName,
                ReportTemplateModelID = x.ReportTemplateModelID
            })
            .OrderBy(x => x.DisplayName)
            .ToListAsync();
    }

    public static List<ReportTemplateModelLookupItem> ListModelsAsLookupItems()
    {
        return ReportTemplateModel.All
            .OrderBy(x => x.ReportTemplateModelDisplayName)
            .Select(x => new ReportTemplateModelLookupItem
            {
                ReportTemplateModelID = x.ReportTemplateModelID,
                ReportTemplateModelName = x.ReportTemplateModelName,
                ReportTemplateModelDisplayName = x.ReportTemplateModelDisplayName
            })
            .ToList();
    }
}
