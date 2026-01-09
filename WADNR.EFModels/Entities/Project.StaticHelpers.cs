using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Projects
{
    // Shared EF-usable definition and constant for active projects
    public const int ApprovedStatusId = (int)ProjectApprovalStatusEnum.Approved;

    public static readonly Expression<Func<Project, bool>> IsActiveProjectExpr =
        p => p.ProjectApprovalStatusID == ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin;

    public static async Task<List<ProjectCountyDetailGridRow>> ListAsCountyDetailGridRowAsync(WADNRDbContext dbContext, int countyID)
    {
        return await dbContext.Projects
            .Where(p => p.ProjectCounties.Any(pc => pc.CountyID == countyID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Select(ProjectProjections.AsProjectCountyDetailGridRow)
            .ToListAsync();
    }

    public static async Task<List<ProjectDNRUplandRegionDetailGridRow>> ListAsDNRUplandDetailGridRowAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {

        var rows = await dbContext.Projects
            .Where(p => p.ProjectRegions.Any(x => x.DNRUplandRegionID == dnrUplandRegionID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsDnrUplandRegionDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalTreatedAcresByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalTreatedAcres = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : 0m;
        }

        return rows;
    }

    public static async Task<ProjectDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectID)
    {
        var entity = await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<ProjectDetail?> CreateAsync(WADNRDbContext dbContext, ProjectUpsertRequest dto, int callingPersonID)
    {
        var entity = new Project
        {
            ProjectTypeID = dto.ProjectTypeID,
            ProjectStageID = dto.ProjectStageID,
            ProjectName = dto.ProjectName,
            ProjectDescription = dto.ProjectDescription,
            CompletionDate = dto.CompletionDate,
            EstimatedTotalCost = dto.EstimatedTotalCost,
            IsFeatured = false,
            ProjectLocationSimpleTypeID = dto.ProjectLocationSimpleTypeID,
            ProjectApprovalStatusID = dto.ProjectApprovalStatusID,
            ProposingPersonID = dto.ProposingPersonID,
            ProposingDate = dto.ProposingDate,
            SubmissionDate = dto.SubmissionDate,
            ApprovalDate = dto.ApprovalDate,
            FocusAreaID = dto.FocusAreaID,
            ExpirationDate = dto.ExpirationDate,
            FhtProjectNumber = dto.FhtProjectNumber
        };
        dbContext.Projects.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProjectID);
    }

    public static async Task<ProjectDetail?> UpdateAsync(WADNRDbContext dbContext, int projectID, ProjectUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.Projects
            .FirstAsync(x => x.ProjectID == projectID);

        entity.ProjectTypeID = dto.ProjectTypeID;
        entity.ProjectStageID = dto.ProjectStageID;
        entity.ProjectName = dto.ProjectName;
        entity.ProjectDescription = dto.ProjectDescription;
        entity.CompletionDate = dto.CompletionDate;
        entity.EstimatedTotalCost = dto.EstimatedTotalCost;
        entity.ProjectLocationSimpleTypeID = dto.ProjectLocationSimpleTypeID;
        entity.ProjectApprovalStatusID = dto.ProjectApprovalStatusID;
        entity.ProposingPersonID = dto.ProposingPersonID;
        entity.ProposingDate = dto.ProposingDate;
        entity.SubmissionDate = dto.SubmissionDate;
        entity.ApprovalDate = dto.ApprovalDate;
        entity.FocusAreaID = dto.FocusAreaID;
        entity.ExpirationDate = dto.ExpirationDate;
        entity.FhtProjectNumber = dto.FhtProjectNumber;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProjectID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int projectID)
    {
        var deletedCount = await dbContext.Projects
            .Where(x => x.ProjectID == projectID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<ProjectGridRow>> ListAsGridRowAsync(
        IQueryable<Project> projectsQuery,
        WADNRDbContext dbContext)
    {
        var projects = await projectsQuery
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalTreatedAcresByProjectAsync(dbContext);

        foreach (var p in projects)
        {
            p.TotalTreatedAcres = totalsByProjectId.TryGetValue(p.ProjectID, out var total) ? total : 0m;
        }

        return projects;
    }

    // Convenience overload for all projects
    public static Task<List<ProjectGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
        => ListAsGridRowAsync(dbContext.Projects, dbContext);

    private static async Task<Dictionary<int, decimal>> GetTotalTreatedAcresByProjectAsync(WADNRDbContext dbContext)
    {
        var totals = await dbContext.vTotalTreatedAcresByProjects
            .AsNoTracking()
            .ToListAsync();

        return totals.ToDictionary(t => t.ProjectID, t => t.TotalTreatedAcres ?? 0m);
    }
}
