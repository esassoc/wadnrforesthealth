using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectTypes
{
    public static async Task<List<ProjectTypeGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
        => await ProjectTypeProjections.AsGridRow(dbContext.ProjectTypes.AsNoTracking())
            .OrderBy(x => x.ProjectTypeSortOrder).ThenBy(x => x.ProjectTypeName)
            .ToListAsync();

    public static async Task<List<ProjectTypeLookupItem>> ListAsLookupAsync(WADNRDbContext dbContext)
        => await ProjectTypeProjections.AsLookup(dbContext.ProjectTypes.AsNoTracking())
            .OrderBy(x => x.ProjectTypeName)
            .ToListAsync();

    public static async Task<ProjectTypeDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int id)
        => await ProjectTypeProjections.AsDetail(dbContext.ProjectTypes.AsNoTracking().Where(x => x.ProjectTypeID == id))
            .SingleOrDefaultAsync();

    public static async Task<List<ProjectTypeTaxonomy>> ListTaxonomyAsync(WADNRDbContext dbContext, PersonDetail? callingUser)
    {
        var canViewAdminLimited = callingUser.CanViewAdminLimitedProjects();
        return await ProjectTypeProjections.AsTaxonomy(dbContext.ProjectTypes.AsNoTracking(), canViewAdminLimited)
            .OrderBy(x => x.ProjectTypeName)
            .ToListAsync();
    }

    public static async Task<ProjectTypeDetail?> CreateAsync(WADNRDbContext dbContext, ProjectTypeUpsertRequest dto)
    {
        var entity = new ProjectType
        {
            TaxonomyBranchID = dto.TaxonomyBranchID,
            ProjectTypeName = dto.ProjectTypeName,
            ProjectTypeDescription = dto.ProjectTypeDescription,
            ProjectTypeCode = dto.ProjectTypeCode,
            ThemeColor = dto.ThemeColor,
            ProjectTypeSortOrder = dto.ProjectTypeSortOrder,
            LimitVisibilityToAdmin = dto.LimitVisibilityToAdmin
        };
        dbContext.ProjectTypes.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProjectTypeID);
    }

    public static async Task<ProjectTypeDetail?> UpdateAsync(WADNRDbContext dbContext, int id, ProjectTypeUpsertRequest dto)
    {
        var entity = await dbContext.ProjectTypes.FirstAsync(x => x.ProjectTypeID == id);
        entity.TaxonomyBranchID = dto.TaxonomyBranchID;
        entity.ProjectTypeName = dto.ProjectTypeName;
        entity.ProjectTypeDescription = dto.ProjectTypeDescription;
        entity.ProjectTypeCode = dto.ProjectTypeCode;
        entity.ThemeColor = dto.ThemeColor;
        entity.ProjectTypeSortOrder = dto.ProjectTypeSortOrder;
        entity.LimitVisibilityToAdmin = dto.LimitVisibilityToAdmin;
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProjectTypeID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int id)
    {
        var deleted = await dbContext.ProjectTypes.Where(x => x.ProjectTypeID == id).ExecuteDeleteAsync();
        return deleted > 0;
    }
}
