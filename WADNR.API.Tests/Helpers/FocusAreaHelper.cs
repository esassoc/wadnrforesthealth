using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Helper methods for creating and managing test focus areas.
/// </summary>
public static class FocusAreaHelper
{
    /// <summary>
    /// Creates a focus area with minimal required data for testing.
    /// </summary>
    public static async Task<FocusArea> CreateFocusAreaAsync(
        WADNRDbContext dbContext,
        string? name = null,
        int? focusAreaStatusID = null,
        int? dnrUplandRegionID = null)
    {
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;

        // Get valid lookup IDs if not provided
        // FocusAreaStatusID: 1=Active, 2=Inactive (hardcoded lookup)
        var statusID = focusAreaStatusID ?? 1;
        var regionID = dnrUplandRegionID ?? (await dbContext.DNRUplandRegions.FirstAsync()).DNRUplandRegionID;

        var focusArea = new FocusArea
        {
            FocusAreaName = name ?? $"Test Focus Area {uniqueSuffix}",
            FocusAreaStatusID = statusID,
            DNRUplandRegionID = regionID,
        };

        dbContext.FocusAreas.Add(focusArea);
        await dbContext.SaveChangesWithNoAuditingAsync();

        return focusArea;
    }

    /// <summary>
    /// Gets an existing focus area by ID with fresh data.
    /// </summary>
    public static async Task<FocusArea?> GetByIDAsync(WADNRDbContext dbContext, int focusAreaID)
    {
        return await dbContext.FocusAreas
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.FocusAreaID == focusAreaID);
    }

    /// <summary>
    /// Deletes a focus area and all related data.
    /// </summary>
    public static async Task DeleteFocusAreaAsync(WADNRDbContext dbContext, int focusAreaID)
    {
        // Delete related data in FK-safe order
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.FocusAreaLocationStaging WHERE FocusAreaID = {focusAreaID}");

        // Update Projects to remove FK reference
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE dbo.Project SET FocusAreaID = NULL WHERE FocusAreaID = {focusAreaID}");

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM dbo.FocusArea WHERE FocusAreaID = {focusAreaID}");
    }

    /// <summary>
    /// Gets an existing focus area for testing (does not create).
    /// </summary>
    public static async Task<FocusArea?> GetFirstFocusAreaAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FocusAreas
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}
