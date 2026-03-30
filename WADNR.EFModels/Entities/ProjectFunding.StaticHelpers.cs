using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectFunding
{
    public static async Task<ProjectFundingDetail> GetForProjectAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new
            {
                p.EstimatedTotalCost,
                p.ProjectFundingSourceNotes
            })
            .FirstAsync();

        var fundingSourceIDs = await dbContext.ProjectFundingSources
            .AsNoTracking()
            .Where(pfs => pfs.ProjectID == projectID)
            .Select(pfs => pfs.FundingSourceID)
            .ToListAsync();

        var allocationRequests = await dbContext.ProjectFundSourceAllocationRequests
            .AsNoTracking()
            .Where(r => r.ProjectID == projectID)
            .Select(r => new FundSourceAllocationRequestItem
            {
                ProjectFundSourceAllocationRequestID = r.ProjectFundSourceAllocationRequestID,
                FundSourceAllocationID = r.FundSourceAllocationID,
                FundSourceAllocationName = r.FundSourceAllocation.FundSourceAllocationName,
                FundSourceName = r.FundSourceAllocation.FundSource.FundSourceName,
                MatchAmount = r.MatchAmount,
                PayAmount = r.PayAmount,
                TotalAmount = r.TotalAmount
            })
            .OrderBy(r => r.FundSourceName)
            .ThenBy(r => r.FundSourceAllocationName)
            .ToListAsync();

        return new ProjectFundingDetail
        {
            EstimatedTotalCost = project.EstimatedTotalCost,
            FundingSourceNotes = project.ProjectFundingSourceNotes,
            SelectedFundingSourceIDs = fundingSourceIDs,
            AllocationRequests = allocationRequests
        };
    }

    public static async Task<ProjectFundingDetail> SaveAllAsync(WADNRDbContext dbContext, int projectID, ProjectFundingSaveRequest request)
    {
        var project = await dbContext.Projects
            .FirstAsync(p => p.ProjectID == projectID);

        project.EstimatedTotalCost = request.EstimatedTotalCost;
        project.ProjectFundingSourceNotes = request.FundingSourceNotes;

        // Sync ProjectFundingSources
        var existingFundingSources = await dbContext.ProjectFundingSources
            .Where(pfs => pfs.ProjectID == projectID)
            .ToListAsync();

        var requestedFundingSourceIDs = request.FundingSourceIDs.ToHashSet();
        var existingFundingSourceIDs = existingFundingSources.Select(pfs => pfs.FundingSourceID).ToHashSet();

        var toDeleteFS = existingFundingSources.Where(pfs => !requestedFundingSourceIDs.Contains(pfs.FundingSourceID)).ToList();
        dbContext.ProjectFundingSources.RemoveRange(toDeleteFS);

        foreach (var fsID in requestedFundingSourceIDs.Except(existingFundingSourceIDs))
        {
            dbContext.ProjectFundingSources.Add(new ProjectFundingSource
            {
                ProjectID = projectID,
                FundingSourceID = fsID
            });
        }

        // Sync ProjectFundSourceAllocationRequests
        var existingAllocations = await dbContext.ProjectFundSourceAllocationRequests
            .Where(r => r.ProjectID == projectID)
            .ToListAsync();

        var requestAllocIDs = request.AllocationRequests
            .Where(r => r.ProjectFundSourceAllocationRequestID.HasValue)
            .Select(r => r.ProjectFundSourceAllocationRequestID!.Value)
            .ToHashSet();

        var toDeleteAlloc = existingAllocations.Where(e => !requestAllocIDs.Contains(e.ProjectFundSourceAllocationRequestID)).ToList();
        dbContext.ProjectFundSourceAllocationRequests.RemoveRange(toDeleteAlloc);

        foreach (var item in request.AllocationRequests)
        {
            if (item.ProjectFundSourceAllocationRequestID.HasValue)
            {
                // Update existing
                var existing = existingAllocations.FirstOrDefault(e => e.ProjectFundSourceAllocationRequestID == item.ProjectFundSourceAllocationRequestID.Value);
                if (existing != null)
                {
                    existing.FundSourceAllocationID = item.FundSourceAllocationID;
                    existing.MatchAmount = item.MatchAmount;
                    existing.PayAmount = item.PayAmount;
                    existing.TotalAmount = item.TotalAmount;
                    existing.UpdateDate = DateTime.UtcNow;
                }
            }
            else
            {
                // Create new
                dbContext.ProjectFundSourceAllocationRequests.Add(new ProjectFundSourceAllocationRequest
                {
                    ProjectID = projectID,
                    FundSourceAllocationID = item.FundSourceAllocationID,
                    MatchAmount = item.MatchAmount,
                    PayAmount = item.PayAmount,
                    TotalAmount = item.TotalAmount,
                    CreateDate = DateTime.UtcNow,
                    ImportedFromTabularData = false
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return await GetForProjectAsync(dbContext, projectID);
    }
}
