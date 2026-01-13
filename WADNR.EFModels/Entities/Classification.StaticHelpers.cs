using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Classifications
{
    public static async Task<List<ClassificationGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
        => await ClassificationProjections.AsGridRow(dbContext.Classifications.AsNoTracking())
            .OrderBy(x => x.ClassificationSortOrder).ThenBy(x => x.DisplayName)
            .ToListAsync();

    public static async Task<List<ClassificationWithProjectCount>> ListAsWithProjectCountAsync(WADNRDbContext dbContext)
        => await ClassificationProjections.AsWithProjectCount(dbContext.Classifications.AsNoTracking())
            .OrderBy(x => x.ClassificationSortOrder).ThenBy(x => x.DisplayName)
            .ToListAsync();

    public static async Task<ClassificationDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int id)
        => await ClassificationProjections.AsDetail(dbContext.Classifications.AsNoTracking().Where(x => x.ClassificationID == id))
            .SingleOrDefaultAsync();

    public static async Task<ClassificationDetail?> CreateAsync(WADNRDbContext dbContext, ClassificationUpsertRequest dto)
    {
        var entity = new Classification
        {
            ClassificationSystemID = dto.ClassificationSystemID,
            DisplayName = dto.DisplayName,
            ClassificationDescription = dto.ClassificationDescription,
            ThemeColor = dto.ThemeColor,
            GoalStatement = dto.GoalStatement,
            KeyImageFileResourceID = dto.KeyImageFileResourceID,
            ClassificationSortOrder = dto.ClassificationSortOrder
        };
        dbContext.Classifications.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ClassificationID);
    }

    public static async Task<ClassificationDetail?> UpdateAsync(WADNRDbContext dbContext, int id, ClassificationUpsertRequest dto)
    {
        var entity = await dbContext.Classifications.FirstAsync(x => x.ClassificationID == id);
        entity.ClassificationSystemID = dto.ClassificationSystemID;
        entity.DisplayName = dto.DisplayName;
        entity.ClassificationDescription = dto.ClassificationDescription;
        entity.ThemeColor = dto.ThemeColor;
        entity.GoalStatement = dto.GoalStatement;
        entity.KeyImageFileResourceID = dto.KeyImageFileResourceID;
        entity.ClassificationSortOrder = dto.ClassificationSortOrder;
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ClassificationID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int id)
    {
        var deleted = await dbContext.Classifications.Where(x => x.ClassificationID == id).ExecuteDeleteAsync();
        return deleted > 0;
    }
}
