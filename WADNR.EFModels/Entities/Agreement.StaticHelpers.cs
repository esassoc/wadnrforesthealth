using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Agreements
{
    public static async Task<List<AgreementGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.Agreements
            .AsNoTracking()
            .Select(AgreementProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<AgreementDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int agreementID)
    {
        var entity = await dbContext.Agreements
            .AsNoTracking()
            .Where(x => x.AgreementID == agreementID)
            .Select(AgreementProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<AgreementDetail?> CreateAsync(WADNRDbContext dbContext, AgreementUpsertRequest dto, int callingPersonID)
    {
        var entity = new Agreement
        {
            AgreementTypeID = dto.AgreementTypeID,
            AgreementTitle = dto.AgreementTitle,
            AgreementNumber = dto.AgreementNumber,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            AgreementAmount = dto.AgreementAmount,
            ExpendedAmount = dto.ExpendedAmount,
            BalanceAmount = dto.BalanceAmount,
            DNRUplandRegionID = dto.DNRUplandRegionID,
            FirstBillDueOn = dto.FirstBillDueOn,
            Notes = dto.Notes,
            OrganizationID = dto.OrganizationID,
            AgreementStatusID = dto.AgreementStatusID,
            AgreementFileResourceID = dto.AgreementFileResourceID
        };
        dbContext.Agreements.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.AgreementID);
    }

    public static async Task<AgreementDetail?> UpdateAsync(WADNRDbContext dbContext, int agreementID, AgreementUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.Agreements
            .FirstAsync(x => x.AgreementID == agreementID);

        entity.AgreementTypeID = dto.AgreementTypeID;
        entity.AgreementTitle = dto.AgreementTitle;
        entity.AgreementNumber = dto.AgreementNumber;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.AgreementAmount = dto.AgreementAmount;
        entity.ExpendedAmount = dto.ExpendedAmount;
        entity.BalanceAmount = dto.BalanceAmount;
        entity.DNRUplandRegionID = dto.DNRUplandRegionID;
        entity.FirstBillDueOn = dto.FirstBillDueOn;
        entity.Notes = dto.Notes;
        entity.OrganizationID = dto.OrganizationID;
        entity.AgreementStatusID = dto.AgreementStatusID;
        entity.AgreementFileResourceID = dto.AgreementFileResourceID;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.AgreementID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int agreementID)
    {
        var deletedCount = await dbContext.Agreements
            .Where(x => x.AgreementID == agreementID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<FundSourceAllocationLookupItem>> ListFundSourceAllocationsAsLookupItemByAgreementIDAsync(
        WADNRDbContext dbContext,
        int agreementID)
    {
        var items = await dbContext.AgreementFundSourceAllocations
            .AsNoTracking()
            .Where(x => x.AgreementID == agreementID)
            .Select(x => x.FundSourceAllocation)
            .Select(FundSourceAllocationProjections.AsLookupItem)
            .Distinct()
            .OrderBy(x => x.FundSourceAllocationName)
            .ToListAsync();

        return items;
    }

    public static async Task<List<ProjectLookupItem>> ListProjectsAsLookupItemByAgreementIDAsync(
        WADNRDbContext dbContext,
        int agreementID)
    {
        var items = await dbContext.AgreementProjects
            .AsNoTracking()
            .Where(x => x.AgreementID == agreementID)
            .Select(x => x.Project)
            .Where(Projects.IsActiveProjectExpr)
            .Select(ProjectProjections.AsLookupItem)
            .Distinct()
            .OrderBy(x => x.ProjectName)
            .ToListAsync();

        return items;
    }

    public static async Task<List<AgreementContactGridRow>> ListContactsAsGridRowByAgreementIDAsync(
        WADNRDbContext dbContext,
        int agreementID)
    {
        //MP 1/20/26 This feels excessive, as we're doing this only to facilitate the AgreementPersonRole entity.
        //Should maybe remove AgreementPersonRole as a lookup item and maybe that would make it no longer think it's a computed field?
        //But I need to keep moving and for now this is fine.
        var rawItems = await dbContext.AgreementPeople
            .AsNoTracking()
            .Where(x => x.AgreementID == agreementID)
            .Select(AgreementProjections.AsContactGridRowRaw)
            .ToListAsync();

        var items = rawItems
            .Select(AgreementProjections.ToContactGridRow)
            .OrderBy(x => x.Person.LastName)
            .ThenBy(x => x.Person.FirstName)
            .ThenBy(x => x.AgreementRole.AgreementPersonRoleName)
            .ToList();

        return items;
    }

    public static async Task<List<FundSourceAllocationLookupItem>> UpdateFundSourceAllocationsAsync(
        WADNRDbContext dbContext,
        int agreementID,
        AgreementFundSourceAllocationsUpdateRequest request)
    {
        await dbContext.AgreementFundSourceAllocations
            .Where(x => x.AgreementID == agreementID)
            .ExecuteDeleteAsync();

        var newRows = request.FundSourceAllocationIDs.Select(id => new AgreementFundSourceAllocation
        {
            AgreementID = agreementID,
            FundSourceAllocationID = id
        }).ToList();

        dbContext.AgreementFundSourceAllocations.AddRange(newRows);
        await dbContext.SaveChangesAsync();

        return await ListFundSourceAllocationsAsLookupItemByAgreementIDAsync(dbContext, agreementID);
    }

    public static async Task<List<ProjectLookupItem>> UpdateProjectsAsync(
        WADNRDbContext dbContext,
        int agreementID,
        AgreementProjectsUpdateRequest request)
    {
        await dbContext.AgreementProjects
            .Where(x => x.AgreementID == agreementID)
            .ExecuteDeleteAsync();

        var newRows = request.ProjectIDs.Select(id => new AgreementProject
        {
            AgreementID = agreementID,
            ProjectID = id
        }).ToList();

        dbContext.AgreementProjects.AddRange(newRows);
        await dbContext.SaveChangesAsync();

        return await ListProjectsAsLookupItemByAgreementIDAsync(dbContext, agreementID);
    }

    public static async Task<AgreementContactGridRow> CreateContactAsync(
        WADNRDbContext dbContext,
        int agreementID,
        AgreementContactUpsertRequest request)
    {
        var entity = new AgreementPerson
        {
            AgreementID = agreementID,
            PersonID = request.PersonID,
            AgreementPersonRoleID = request.AgreementPersonRoleID
        };
        dbContext.AgreementPeople.Add(entity);
        await dbContext.SaveChangesAsync();

        var raw = await dbContext.AgreementPeople
            .AsNoTracking()
            .Where(x => x.AgreementPersonID == entity.AgreementPersonID)
            .Select(AgreementProjections.AsContactGridRowRaw)
            .SingleAsync();

        return AgreementProjections.ToContactGridRow(raw);
    }

    public static async Task<AgreementContactGridRow> UpdateContactAsync(
        WADNRDbContext dbContext,
        int agreementPersonID,
        AgreementContactUpsertRequest request)
    {
        var entity = await dbContext.AgreementPeople
            .FirstAsync(x => x.AgreementPersonID == agreementPersonID);

        entity.PersonID = request.PersonID;
        entity.AgreementPersonRoleID = request.AgreementPersonRoleID;
        await dbContext.SaveChangesAsync();

        var raw = await dbContext.AgreementPeople
            .AsNoTracking()
            .Where(x => x.AgreementPersonID == agreementPersonID)
            .Select(AgreementProjections.AsContactGridRowRaw)
            .SingleAsync();

        return AgreementProjections.ToContactGridRow(raw);
    }

    public static async Task<bool> DeleteContactAsync(WADNRDbContext dbContext, int agreementPersonID)
    {
        var deletedCount = await dbContext.AgreementPeople
            .Where(x => x.AgreementPersonID == agreementPersonID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<AgreementGridRow>> ListAsGridRowByOrganizationIDAsync(WADNRDbContext dbContext, int organizationID)
    {
        var entities = await dbContext.Agreements
            .AsNoTracking()
            .Where(x => x.OrganizationID == organizationID)
            .Select(AgreementProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<List<AgreementGridRow>> ListForPersonAsGridRowAsync(WADNRDbContext dbContext, int personID)
    {
        // Get distinct agreement IDs first to avoid DISTINCT on entity with potential geometry columns
        var agreementIDs = await dbContext.AgreementPeople
            .AsNoTracking()
            .Where(ap => ap.PersonID == personID)
            .Select(ap => ap.AgreementID)
            .Distinct()
            .ToListAsync();

        var agreements = await dbContext.Agreements
            .AsNoTracking()
            .Where(a => agreementIDs.Contains(a.AgreementID))
            .Select(AgreementProjections.AsGridRow)
            .ToListAsync();

        return agreements;
    }

    public static async Task<List<AgreementExcelRow>> ListAsExcelRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.Agreements
            .AsNoTracking()
            .Select(AgreementProjections.AsExcelRow)
            .ToListAsync();
    }

    public static async Task<List<AgreementExcelRow>> ListAsExcelRowByOrganizationIDAsync(WADNRDbContext dbContext, int organizationID)
    {
        return await dbContext.Agreements
            .AsNoTracking()
            .Where(x => x.OrganizationID == organizationID)
            .Select(AgreementProjections.AsExcelRow)
            .ToListAsync();
    }

    public static async Task<List<AgreementExcelRow>> ListForPersonAsExcelRowAsync(WADNRDbContext dbContext, int personID)
    {
        var agreementIDs = await dbContext.AgreementPeople
            .AsNoTracking()
            .Where(ap => ap.PersonID == personID)
            .Select(ap => ap.AgreementID)
            .Distinct()
            .ToListAsync();

        return await dbContext.Agreements
            .AsNoTracking()
            .Where(a => agreementIDs.Contains(a.AgreementID))
            .Select(AgreementProjections.AsExcelRow)
            .ToListAsync();
    }
}
