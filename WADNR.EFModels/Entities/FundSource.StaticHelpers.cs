using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSources
{
    public static async Task<List<FundSourceGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.FundSources
            .AsNoTracking()
            .OrderByDescending(x => x.FundSourceNumber)
            .Select(FundSourceProjections.AsGridRow)
            .ToListAsync();
        return entities;
    }

    public static async Task<FundSourceDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        var entity = await dbContext.FundSources
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .Select(FundSourceProjections.AsDetail)
            .SingleOrDefaultAsync();
        return entity;
    }

    public static async Task<FundSourceDetail?> CreateAsync(WADNRDbContext dbContext, FundSourceUpsertRequest dto)
    {
        var entity = new FundSource
        {
            FundSourceName = dto.FundSourceName,
            FundSourceNumber = dto.FundSourceNumber,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ConditionsAndRequirements = dto.ConditionsAndRequirements,
            ComplianceNotes = dto.ComplianceNotes,
            CFDANumber = dto.CFDANumber,
            FundSourceTypeID = dto.FundSourceTypeID,
            ShortName = dto.ShortName,
            FundSourceStatusID = dto.FundSourceStatusID,
            OrganizationID = dto.OrganizationID,
            TotalAwardAmount = dto.TotalAwardAmount
        };
        dbContext.FundSources.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.FundSourceID);
    }

    public static async Task<FundSourceDetail?> UpdateAsync(WADNRDbContext dbContext, int fundSourceID, FundSourceUpsertRequest dto)
    {
        var entity = await dbContext.FundSources
            .FirstAsync(x => x.FundSourceID == fundSourceID);

        entity.FundSourceName = dto.FundSourceName;
        entity.FundSourceNumber = dto.FundSourceNumber;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.ConditionsAndRequirements = dto.ConditionsAndRequirements;
        entity.ComplianceNotes = dto.ComplianceNotes;
        entity.CFDANumber = dto.CFDANumber;
        entity.FundSourceTypeID = dto.FundSourceTypeID;
        entity.ShortName = dto.ShortName;
        entity.FundSourceStatusID = dto.FundSourceStatusID;
        entity.OrganizationID = dto.OrganizationID;
        entity.TotalAwardAmount = dto.TotalAwardAmount;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.FundSourceID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        var deletedCount = await dbContext.FundSources
            .Where(x => x.FundSourceID == fundSourceID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<FundSourceAllocationLookupItem>> ListAllocationsAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        return await dbContext.FundSourceAllocations
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .OrderBy(x => x.FundSourceAllocationName)
            .Select(x => new FundSourceAllocationLookupItem
            {
                FundSourceAllocationID = x.FundSourceAllocationID,
                FundSourceAllocationName = x.FundSourceAllocationName ?? string.Empty
            })
            .ToListAsync();
    }

    public static async Task<List<FundSourceProjectGridRow>> ListProjectsAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        return await dbContext.ProjectFundSourceAllocationRequests
            .AsNoTracking()
            .Where(x => x.FundSourceAllocation.FundSourceID == fundSourceID)
            .OrderBy(x => x.Project.ProjectName)
            .Select(x => new FundSourceProjectGridRow
            {
                FundSourceAllocationID = x.FundSourceAllocationID,
                FundSourceAllocationName = x.FundSourceAllocation.FundSourceAllocationName,
                ProjectID = x.ProjectID,
                ProjectName = x.Project.ProjectName ?? string.Empty,
                FhtProjectNumber = x.Project.FhtProjectNumber,
                ProjectStageName = x.Project.ProjectStage != null ? x.Project.ProjectStage.ProjectStageDisplayName : null,
                MatchAmount = x.MatchAmount,
                PayAmount = x.PayAmount,
                TotalAmount = x.TotalAmount
            })
            .ToListAsync();
    }

    public static async Task<List<FundSourceAgreementGridRow>> ListAgreementsAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        return await dbContext.AgreementFundSourceAllocations
            .AsNoTracking()
            .Where(x => x.FundSourceAllocation.FundSourceID == fundSourceID)
            .Select(x => x.Agreement)
            .Distinct()
            .OrderBy(x => x.AgreementTitle)
            .Select(x => new FundSourceAgreementGridRow
            {
                AgreementID = x.AgreementID,
                AgreementTitle = x.AgreementTitle ?? string.Empty,
                AgreementNumber = x.AgreementNumber,
                AgreementTypeAbbrev = x.AgreementType != null ? x.AgreementType.AgreementTypeAbbrev : null,
                OrganizationID = x.OrganizationID,
                OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                AgreementAmount = x.AgreementAmount,
                ProgramIndices = string.Join(", ", x.AgreementFundSourceAllocations
                    .SelectMany(a => a.FundSourceAllocation.FundSourceAllocationProgramIndexProjectCodes)
                    .Select(p => p.ProgramIndex.ProgramIndexCode)
                    .Distinct()),
                ProjectCodes = string.Join(", ", x.AgreementFundSourceAllocations
                    .SelectMany(a => a.FundSourceAllocation.FundSourceAllocationProgramIndexProjectCodes)
                    .Where(p => p.ProjectCode != null)
                    .Select(p => p.ProjectCode!.ProjectCodeName)
                    .Distinct())
            })
            .ToListAsync();
    }

    public static async Task<List<FundSourceBudgetLineItemGridRow>> ListBudgetLineItemsAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        // Get all allocations for this fund source with their budget line items grouped by allocation
        var allocations = await dbContext.FundSourceAllocations
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .Select(x => new
            {
                x.FundSourceAllocationID,
                x.FundSourceAllocationName,
                BudgetLineItems = x.FundSourceAllocationBudgetLineItems.ToList()
            })
            .ToListAsync();

        // CostTypeID values: Personnel=3, Benefits=4, Travel=5, Supplies=2, Contractual=6, IndirectCosts=1
        return allocations.Select(a => new FundSourceBudgetLineItemGridRow
        {
            FundSourceAllocationID = a.FundSourceAllocationID,
            FundSourceAllocationName = a.FundSourceAllocationName,
            PersonnelAmount = a.BudgetLineItems.Where(b => b.CostTypeID == 3).Sum(b => b.FundSourceAllocationBudgetLineItemAmount),
            BenefitsAmount = a.BudgetLineItems.Where(b => b.CostTypeID == 4).Sum(b => b.FundSourceAllocationBudgetLineItemAmount),
            TravelAmount = a.BudgetLineItems.Where(b => b.CostTypeID == 5).Sum(b => b.FundSourceAllocationBudgetLineItemAmount),
            SuppliesAmount = a.BudgetLineItems.Where(b => b.CostTypeID == 2).Sum(b => b.FundSourceAllocationBudgetLineItemAmount),
            ContractualAmount = a.BudgetLineItems.Where(b => b.CostTypeID == 6).Sum(b => b.FundSourceAllocationBudgetLineItemAmount),
            IndirectCostsAmount = a.BudgetLineItems.Where(b => b.CostTypeID == 1).Sum(b => b.FundSourceAllocationBudgetLineItemAmount),
            TotalAmount = a.BudgetLineItems.Sum(b => b.FundSourceAllocationBudgetLineItemAmount)
        }).ToList();
    }

    public static async Task<List<FundSourceFileResourceGridRow>> ListFilesAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        return await dbContext.FundSourceFileResources
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .OrderBy(x => x.DisplayName)
            .Select(x => new FundSourceFileResourceGridRow
            {
                FundSourceFileResourceID = x.FundSourceFileResourceID,
                FileResourceID = x.FileResourceID,
                FileResourceGUID = x.FileResource.FileResourceGUID,
                DisplayName = x.DisplayName,
                Description = x.Description,
                OriginalBaseFilename = x.FileResource.OriginalBaseFilename,
                FileResourceMimeTypeName = x.FileResource.FileResourceMimeType.FileResourceMimeTypeDisplayName,
                CreateDate = x.FileResource.CreateDate
            })
            .ToListAsync();
    }

    public static async Task<List<FundSourceNoteGridRow>> ListNotesAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        return await dbContext.FundSourceNotes
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new FundSourceNoteGridRow
            {
                FundSourceNoteID = x.FundSourceNoteID,
                Note = x.FundSourceNoteText ?? string.Empty,
                CreatedByPersonName = x.CreatedByPerson.FirstName + " " + x.CreatedByPerson.LastName,
                CreateDate = x.CreatedDate,
                UpdatedByPersonName = x.LastUpdatedByPerson != null ? x.LastUpdatedByPerson.FirstName + " " + x.LastUpdatedByPerson.LastName : null,
                UpdateDate = x.LastUpdatedDate
            })
            .ToListAsync();
    }

    public static async Task<List<FundSourceLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FundSources.AsNoTracking()
            .OrderBy(x => x.FundSourceNumber)
            .Select(FundSourceProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<FundSourceFileResource> CreateFileAsync(
        WADNRDbContext dbContext, int fundSourceID, int fileResourceID, string displayName, string? description)
    {
        var entity = new FundSourceFileResource
        {
            FundSourceID = fundSourceID,
            FileResourceID = fileResourceID,
            DisplayName = displayName,
            Description = description
        };

        dbContext.FundSourceFileResources.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public static async Task<List<FundSourceNoteInternalGridRow>> ListInternalNotesAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        return await dbContext.FundSourceNoteInternals
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new FundSourceNoteInternalGridRow
            {
                FundSourceNoteInternalID = x.FundSourceNoteInternalID,
                Note = x.FundSourceNoteText ?? string.Empty,
                CreatedByPersonName = x.CreatedByPerson.FirstName + " " + x.CreatedByPerson.LastName,
                CreateDate = x.CreatedDate,
                UpdatedByPersonName = x.LastUpdatedByPerson != null ? x.LastUpdatedByPerson.FirstName + " " + x.LastUpdatedByPerson.LastName : null,
                UpdateDate = x.LastUpdatedDate
            })
            .ToListAsync();
    }

    // File CRUD
    public static async Task UpdateFileAsync(WADNRDbContext dbContext,
        FundSourceFileResource entity, string displayName, string? description)
    {
        entity.DisplayName = displayName;
        entity.Description = description;
        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteFileAsync(WADNRDbContext dbContext, FundSourceFileResource entity)
    {
        dbContext.FundSourceFileResources.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    // Note CRUD
    public static async Task<FundSourceNote> CreateNoteAsync(WADNRDbContext dbContext, int fundSourceID, string note, int personID)
    {
        var entity = new FundSourceNote
        {
            FundSourceID = fundSourceID,
            FundSourceNoteText = note,
            CreatedByPersonID = personID,
            CreatedDate = DateTime.UtcNow
        };
        dbContext.FundSourceNotes.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public static async Task UpdateNoteAsync(WADNRDbContext dbContext, FundSourceNote entity, string note, int personID)
    {
        entity.FundSourceNoteText = note;
        entity.LastUpdatedByPersonID = personID;
        entity.LastUpdatedDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteNoteAsync(WADNRDbContext dbContext, FundSourceNote entity)
    {
        dbContext.FundSourceNotes.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    // Internal Note CRUD
    public static async Task<FundSourceNoteInternal> CreateNoteInternalAsync(WADNRDbContext dbContext, int fundSourceID, string note, int personID)
    {
        var entity = new FundSourceNoteInternal
        {
            FundSourceID = fundSourceID,
            FundSourceNoteText = note,
            CreatedByPersonID = personID,
            CreatedDate = DateTime.UtcNow
        };
        dbContext.FundSourceNoteInternals.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public static async Task UpdateNoteInternalAsync(WADNRDbContext dbContext, FundSourceNoteInternal entity, string note, int personID)
    {
        entity.FundSourceNoteText = note;
        entity.LastUpdatedByPersonID = personID;
        entity.LastUpdatedDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteNoteInternalAsync(WADNRDbContext dbContext, FundSourceNoteInternal entity)
    {
        dbContext.FundSourceNoteInternals.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
