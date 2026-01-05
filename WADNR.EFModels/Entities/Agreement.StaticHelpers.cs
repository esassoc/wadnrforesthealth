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
}
