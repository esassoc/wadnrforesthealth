using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.EFModels.Entities;

public static class FundSourceAllocations
{
    public static async Task<List<FundSourceAllocationGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var rows = await dbContext.FundSourceAllocations
            .AsNoTracking()
            .OrderBy(x => x.FundSource.FundSourceNumber)
            .ThenBy(x => x.FundSourceAllocationName)
            .Select(FundSourceAllocationProjections.AsGridRow)
            .ToListAsync();

        foreach (var row in rows)
        {
            if (row.FundSourceStatusID != null && FundSourceStatus.AllLookupDictionary.TryGetValue(row.FundSourceStatusID.Value, out var status))
            {
                row.FundSourceStatusName = status.FundSourceStatusName;
            }
            if (row.DivisionID != null && Division.AllLookupDictionary.TryGetValue(row.DivisionID.Value, out var division))
            {
                row.DivisionName = division.DivisionDisplayName;
            }
        }

        return rows;
    }

    public static async Task<List<FundSourceAllocationGridRow>> ListForFundSourceAsGridRowAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        var rows = await dbContext.FundSourceAllocations
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .OrderBy(x => x.FundSourceAllocationName)
            .Select(FundSourceAllocationProjections.AsGridRow)
            .ToListAsync();

        foreach (var row in rows)
        {
            if (row.FundSourceStatusID != null && FundSourceStatus.AllLookupDictionary.TryGetValue(row.FundSourceStatusID.Value, out var status))
            {
                row.FundSourceStatusName = status.FundSourceStatusName;
            }
            if (row.DivisionID != null && Division.AllLookupDictionary.TryGetValue(row.DivisionID.Value, out var division))
            {
                row.DivisionName = division.DivisionDisplayName;
            }
        }

        return rows;
    }

    public static async Task<FundSourceAllocationDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        var detail = await dbContext.FundSourceAllocations
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (detail != null)
        {
            // Resolve static lookup names post-query (can't be used in EF projections)
            if (detail.DivisionID != null && Division.AllLookupDictionary.TryGetValue(detail.DivisionID.Value, out var division))
            {
                detail.DivisionName = division.DivisionDisplayName;
            }

            if (detail.FundSourceStatusID != null && FundSourceStatus.AllLookupDictionary.TryGetValue(detail.FundSourceStatusID.Value, out var status))
            {
                detail.FundSourceStatusName = status.FundSourceStatusName;
            }
        }

        return detail;
    }

    public static async Task<List<FundSourceAllocationLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FundSourceAllocations
            .AsNoTracking()
            .OrderBy(x => x.FundSource.FundSourceNumber)
            .ThenBy(x => x.FundSourceAllocationName)
            .Select(FundSourceAllocationProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<List<FundSourceAllocationDNRUplandRegionDetailGridRow>> ListByDnrUplandRegionActiveAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {
        return await dbContext.FundSourceAllocations
            .Include(x => x.FundSource)
            .Where(x => x.DNRUplandRegionID == dnrUplandRegionID && x.FundSource.FundSourceStatusID == (int)FundSourceStatusEnum.Active)
            .OrderByDescending(x => x.FundSourceAllocationPriority != null ? x.FundSourceAllocationPriority.FundSourceAllocationPriorityNumber : 0)
            .Select(FundSourceAllocationProjections.AsDnrUplandRegionDetailGridRow)
            .ToListAsync();
    }

    public static async Task<List<FundSourceAllocationBudgetLineItemGridRow>> ListBudgetLineItemsAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        var items = await dbContext.FundSourceAllocationBudgetLineItems
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationProjections.AsBudgetLineItemGridRow)
            .ToListAsync();

        foreach (var item in items)
        {
            if (CostType.AllLookupDictionary.TryGetValue(item.CostTypeID, out var costType))
            {
                item.CostTypeName = costType.CostTypeDisplayName;
            }
        }

        return items.OrderBy(x => CostType.AllLookupDictionary.TryGetValue(x.CostTypeID, out var ct) ? ct.SortOrder : 999).ToList();
    }

    public static async Task<List<FundSourceAllocationProjectGridRow>> ListProjectsAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        var rows = await dbContext.ProjectFundSourceAllocationRequests
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationProjections.AsProjectGridRow)
            .OrderBy(x => x.FhtProjectNumber)
            .ToListAsync();

        // Resolve ProjectStage names post-query
        var projectStageMap = await dbContext.Projects
            .AsNoTracking()
            .Where(p => rows.Select(r => r.ProjectID).Contains(p.ProjectID))
            .Select(p => new { p.ProjectID, p.ProjectStageID })
            .ToDictionaryAsync(p => p.ProjectID, p => p.ProjectStageID);

        foreach (var row in rows)
        {
            if (projectStageMap.TryGetValue(row.ProjectID, out var stageID) &&
                ProjectStage.AllLookupDictionary.TryGetValue(stageID, out var stage))
            {
                row.ProjectStageName = stage.ProjectStageName;
            }
        }

        return rows;
    }

    public static async Task<List<FundSourceAllocationAgreementGridRow>> ListAgreementsAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        return await dbContext.AgreementFundSourceAllocations
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationProjections.AsAgreementGridRow)
            .OrderBy(x => x.AgreementNumber)
            .ToListAsync();
    }

    public static async Task<List<FundSourceAllocationChangeLogGridRow>> ListChangeLogsAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        return await dbContext.FundSourceAllocationChangeLogs
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationProjections.AsChangeLogGridRow)
            .OrderByDescending(x => x.ChangeDate)
            .ToListAsync();
    }

    public static async Task<List<FundSourceAllocationBudgetLineItemGridRow>> SaveBudgetLineItemsAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID,
        FundSourceAllocationBudgetLineItemUpsertRequest request)
    {
        var existing = await dbContext.FundSourceAllocationBudgetLineItems
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .ToListAsync();

        var requestedByCostType = request.Items.ToDictionary(i => i.CostTypeID);
        var existingByCostType = existing.ToDictionary(e => e.CostTypeID);

        // Remove items not in request
        var toRemove = existing.Where(e => !requestedByCostType.ContainsKey(e.CostTypeID)).ToList();
        dbContext.FundSourceAllocationBudgetLineItems.RemoveRange(toRemove);

        foreach (var item in request.Items)
        {
            if (existingByCostType.TryGetValue(item.CostTypeID, out var existingItem))
            {
                // Update existing
                existingItem.FundSourceAllocationBudgetLineItemAmount = item.Amount;
                existingItem.FundSourceAllocationBudgetLineItemNote = item.Note;
            }
            else
            {
                // Add new
                dbContext.FundSourceAllocationBudgetLineItems.Add(new FundSourceAllocationBudgetLineItem
                {
                    FundSourceAllocationID = fundSourceAllocationID,
                    CostTypeID = item.CostTypeID,
                    FundSourceAllocationBudgetLineItemAmount = item.Amount,
                    FundSourceAllocationBudgetLineItemNote = item.Note
                });
            }
        }

        await dbContext.SaveChangesAsync();
        return await ListBudgetLineItemsAsync(dbContext, fundSourceAllocationID);
    }

    public static async Task<List<FundSourceAllocationFileGridRow>> ListFilesAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        return await dbContext.FundSourceAllocationFileResources
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .OrderBy(x => x.DisplayName)
            .Select(FundSourceAllocationProjections.AsFileGridRow)
            .ToListAsync();
    }

    public static async Task<FundSourceAllocationFileResource> CreateFileAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID, int fileResourceID, string displayName, string? description)
    {
        var entity = new FundSourceAllocationFileResource
        {
            FundSourceAllocationID = fundSourceAllocationID,
            FileResourceID = fileResourceID,
            DisplayName = displayName,
            Description = description
        };

        dbContext.FundSourceAllocationFileResources.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public static async Task UpdateFileAsync(WADNRDbContext dbContext,
        FundSourceAllocationFileResource entity, string displayName, string? description)
    {
        entity.DisplayName = displayName;
        entity.Description = description;
        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteFileAsync(WADNRDbContext dbContext, FundSourceAllocationFileResource entity)
    {
        dbContext.FundSourceAllocationFileResources.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<FundSourceAllocationExpenditureGridRow>> ListExpendituresAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        var items = await dbContext.FundSourceAllocationExpenditures
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationProjections.AsExpenditureGridRow)
            .OrderBy(x => x.CalendarYear)
            .ThenBy(x => x.CalendarMonth)
            .ToListAsync();

        foreach (var item in items)
        {
            if (item.CostTypeID != null && CostType.AllLookupDictionary.TryGetValue(item.CostTypeID.Value, out var costType))
            {
                item.CostTypeName = costType.CostTypeDisplayName;
            }
        }

        return items;
    }

    public static async Task<List<FundSourceAllocationExpenditureSummary>> ListExpenditureSummaryAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        var expenditures = await dbContext.FundSourceAllocationExpenditures
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .GroupBy(x => x.CostTypeID)
            .Select(g => new { CostTypeID = g.Key, TotalAmount = g.Sum(x => x.ExpenditureAmount) })
            .ToListAsync();

        return expenditures.Select(e => new FundSourceAllocationExpenditureSummary
        {
            CostTypeName = e.CostTypeID != null && CostType.AllLookupDictionary.TryGetValue(e.CostTypeID.Value, out var ct)
                ? ct.CostTypeDisplayName
                : "Unknown",
            TotalAmount = e.TotalAmount
        }).OrderByDescending(x => x.TotalAmount).ToList();
    }

    public static async Task<List<FundSourceAllocationProgramIndexProjectCodeItem>> ListProgramIndexProjectCodesAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        return await dbContext.FundSourceAllocationProgramIndexProjectCodes
            .AsNoTracking()
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .Select(FundSourceAllocationProjections.AsProgramIndexProjectCodeItem)
            .OrderBy(x => x.ProgramIndexCode)
            .ThenBy(x => x.ProjectCodeName)
            .ToListAsync();
    }

    public static async Task<List<FundSourceAllocationProgramIndexProjectCodeItem>> SaveProgramIndexProjectCodesAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID,
        FundSourceAllocationProgramIndexProjectCodeSaveRequest request)
    {
        var existing = await dbContext.FundSourceAllocationProgramIndexProjectCodes
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .ToListAsync();

        var requestedPairs = request.Pairs
            .Select(p => (p.ProgramIndexID, p.ProjectCodeID))
            .ToHashSet();
        var existingPairs = existing
            .Select(e => (e.ProgramIndexID, e.ProjectCodeID))
            .ToHashSet();

        // Delete pairs not in request
        var toDelete = existing
            .Where(e => !requestedPairs.Contains((e.ProgramIndexID, e.ProjectCodeID)))
            .ToList();
        dbContext.FundSourceAllocationProgramIndexProjectCodes.RemoveRange(toDelete);

        // Add new pairs
        foreach (var (programIndexID, projectCodeID) in requestedPairs.Except(existingPairs))
        {
            dbContext.FundSourceAllocationProgramIndexProjectCodes.Add(
                new FundSourceAllocationProgramIndexProjectCode
                {
                    FundSourceAllocationID = fundSourceAllocationID,
                    ProgramIndexID = programIndexID,
                    ProjectCodeID = projectCodeID
                });
        }

        await dbContext.SaveChangesAsync();
        return await ListProgramIndexProjectCodesAsync(dbContext, fundSourceAllocationID);
    }

    public static async Task<FundSourceAllocationDetail?> CreateAsync(WADNRDbContext dbContext, FundSourceAllocationUpsertRequest dto)
    {
        var entity = new FundSourceAllocation
        {
            FundSourceAllocationName = dto.FundSourceAllocationName,
            FundSourceID = dto.FundSourceID,
            AllocationAmount = dto.AllocationAmount,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            OrganizationID = dto.OrganizationID,
            DNRUplandRegionID = dto.DNRUplandRegionID,
            DivisionID = dto.DivisionID,
            FundSourceManagerID = dto.FundSourceManagerID,
            FederalFundCodeID = dto.FederalFundCodeID,
            FundSourceAllocationPriorityID = dto.FundSourceAllocationPriorityID,
            FundSourceAllocationSourceID = dto.FundSourceAllocationSourceID,
            HasFundFSPs = dto.HasFundFSPs,
            LikelyToUse = dto.LikelyToUse,
        };
        dbContext.FundSourceAllocations.Add(entity);
        await dbContext.SaveChangesAsync();

        if (dto.ProgramManagerPersonIDs?.Any() == true)
        {
            dbContext.FundSourceAllocationProgramManagers.AddRange(
                dto.ProgramManagerPersonIDs.Select(pid => new FundSourceAllocationProgramManager
                {
                    FundSourceAllocationID = entity.FundSourceAllocationID,
                    PersonID = pid,
                }));
        }
        if (dto.LikelyToUse == true && dto.LikelyToUsePersonIDs?.Any() == true)
        {
            dbContext.FundSourceAllocationLikelyPeople.AddRange(
                dto.LikelyToUsePersonIDs.Select(pid => new FundSourceAllocationLikelyPerson
                {
                    FundSourceAllocationID = entity.FundSourceAllocationID,
                    PersonID = pid,
                }));
        }

        // Create default $0 budget line items for each valid cost type (matches legacy behavior)
        var lineItemCostTypes = CostType.All.Where(ct => ct.IsValidInvoiceLineItemCostType).ToList();
        dbContext.FundSourceAllocationBudgetLineItems.AddRange(
            lineItemCostTypes.Select(ct => new FundSourceAllocationBudgetLineItem
            {
                FundSourceAllocationID = entity.FundSourceAllocationID,
                CostTypeID = ct.CostTypeID,
                FundSourceAllocationBudgetLineItemAmount = 0
            }));

        await dbContext.SaveChangesAsync();

        return await GetByIDAsDetailAsync(dbContext, entity.FundSourceAllocationID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int fundSourceAllocationID)
    {
        var deletedCount = await dbContext.FundSourceAllocations
            .Where(x => x.FundSourceAllocationID == fundSourceAllocationID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task UpdateAsync(
        WADNRDbContext dbContext, int fundSourceAllocationID,
        FundSourceAllocationUpsertRequest request, int callingUserPersonID)
    {
        var entity = await dbContext.FundSourceAllocations
            .Include(x => x.FundSourceAllocationProgramManagers)
            .Include(x => x.FundSourceAllocationLikelyPeople)
            .SingleAsync(x => x.FundSourceAllocationID == fundSourceAllocationID);

        // If allocation amount changed, create change log entry
        if (entity.AllocationAmount != request.AllocationAmount)
        {
            dbContext.FundSourceAllocationChangeLogs.Add(new FundSourceAllocationChangeLog
            {
                FundSourceAllocationID = fundSourceAllocationID,
                FundSourceAllocationAmountOldValue = entity.AllocationAmount,
                FundSourceAllocationAmountNewValue = request.AllocationAmount,
                FundSourceAllocationAmountNote = request.AllocationAmountChangeNote,
                ChangePersonID = callingUserPersonID,
                ChangeDate = DateTime.UtcNow
            });
        }

        // Update scalar fields
        entity.FundSourceAllocationName = request.FundSourceAllocationName;
        entity.FundSourceID = request.FundSourceID;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.AllocationAmount = request.AllocationAmount;
        entity.FederalFundCodeID = request.FederalFundCodeID;
        entity.OrganizationID = request.OrganizationID;
        entity.DNRUplandRegionID = request.DNRUplandRegionID;
        entity.DivisionID = request.DivisionID;
        entity.FundSourceManagerID = request.FundSourceManagerID;
        entity.FundSourceAllocationPriorityID = request.FundSourceAllocationPriorityID;
        entity.FundSourceAllocationSourceID = request.FundSourceAllocationSourceID;
        entity.HasFundFSPs = request.HasFundFSPs;
        entity.LikelyToUse = request.LikelyToUse;

        // Sync Program Managers
        var existingPMIDs = entity.FundSourceAllocationProgramManagers.Select(pm => pm.PersonID).ToHashSet();
        var requestedPMIDs = request.ProgramManagerPersonIDs.ToHashSet();

        var pmToRemove = entity.FundSourceAllocationProgramManagers.Where(pm => !requestedPMIDs.Contains(pm.PersonID)).ToList();
        dbContext.FundSourceAllocationProgramManagers.RemoveRange(pmToRemove);

        foreach (var personID in requestedPMIDs.Except(existingPMIDs))
        {
            entity.FundSourceAllocationProgramManagers.Add(new FundSourceAllocationProgramManager
            {
                FundSourceAllocationID = fundSourceAllocationID,
                PersonID = personID
            });
        }

        // Sync Likely To Use People
        var existingLTUIDs = entity.FundSourceAllocationLikelyPeople.Select(lp => lp.PersonID).ToHashSet();
        var requestedLTUIDs = request.LikelyToUse == true ? request.LikelyToUsePersonIDs.ToHashSet() : new HashSet<int>();

        var ltuToRemove = entity.FundSourceAllocationLikelyPeople.Where(lp => !requestedLTUIDs.Contains(lp.PersonID)).ToList();
        dbContext.FundSourceAllocationLikelyPeople.RemoveRange(ltuToRemove);

        foreach (var personID in requestedLTUIDs.Except(existingLTUIDs))
        {
            entity.FundSourceAllocationLikelyPeople.Add(new FundSourceAllocationLikelyPerson
            {
                FundSourceAllocationID = fundSourceAllocationID,
                PersonID = personID
            });
        }

        await dbContext.SaveChangesAsync();
    }
}
