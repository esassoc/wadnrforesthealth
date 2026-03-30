using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class RelationshipTypes
{
    public static async Task<List<RelationshipTypeGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.RelationshipTypes
            .AsNoTracking()
            .OrderBy(x => x.RelationshipTypeName)
            .Select(RelationshipTypeProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<List<RelationshipTypeLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        return await dbContext.RelationshipTypes
            .AsNoTracking()
            .OrderBy(x => x.RelationshipTypeName)
            .Select(RelationshipTypeProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<List<RelationshipTypeSummary>> ListAsSummaryAsync(WADNRDbContext dbContext)
    {
        return await dbContext.RelationshipTypes
            .AsNoTracking()
            .OrderBy(x => x.IsPrimaryContact ? 0 : 1)
            .ThenBy(x => x.RelationshipTypeName)
            .Select(RelationshipTypeProjections.AsSummary)
            .ToListAsync();
    }

    public static async Task<RelationshipTypeGridRow?> GetByIDAsGridRowAsync(WADNRDbContext dbContext, int relationshipTypeID)
    {
        return await dbContext.RelationshipTypes
            .AsNoTracking()
            .Where(x => x.RelationshipTypeID == relationshipTypeID)
            .Select(RelationshipTypeProjections.AsGridRow)
            .SingleOrDefaultAsync();
    }

    public static async Task<string?> ValidateUpsertAsync(WADNRDbContext dbContext, RelationshipTypeUpsertRequest dto, int? existingID = null)
    {
        if (dto.CanStewardProjects)
        {
            var existingSteward = await dbContext.RelationshipTypes
                .Where(x => x.CanStewardProjects && (existingID == null || x.RelationshipTypeID != existingID))
                .AnyAsync();
            if (existingSteward)
            {
                return "Only one relationship type can have 'Can Steward Projects' enabled.";
            }
        }

        if (dto.IsPrimaryContact)
        {
            var existingPrimary = await dbContext.RelationshipTypes
                .Where(x => x.IsPrimaryContact && (existingID == null || x.RelationshipTypeID != existingID))
                .AnyAsync();
            if (existingPrimary)
            {
                return "Only one relationship type can have 'Is Primary Contact' enabled.";
            }
        }

        return null;
    }

    public static async Task<RelationshipTypeGridRow?> CreateAsync(WADNRDbContext dbContext, RelationshipTypeUpsertRequest dto)
    {
        // Business rule: force CanOnlyBeRelatedOnceToAProject when CanStewardProjects or IsPrimaryContact
        if (dto.CanStewardProjects || dto.IsPrimaryContact)
        {
            dto.CanOnlyBeRelatedOnceToAProject = true;
        }

        var entity = new RelationshipType
        {
            RelationshipTypeName = dto.RelationshipTypeName,
            RelationshipTypeDescription = dto.RelationshipTypeDescription,
            CanStewardProjects = dto.CanStewardProjects,
            IsPrimaryContact = dto.IsPrimaryContact,
            CanOnlyBeRelatedOnceToAProject = dto.CanOnlyBeRelatedOnceToAProject,
            ShowOnFactSheet = dto.ShowOnFactSheet,
            ReportInAccomplishmentsDashboard = dto.ReportInAccomplishmentsDashboard,
        };
        dbContext.RelationshipTypes.Add(entity);
        await dbContext.SaveChangesAsync();

        await SyncOrganizationTypeRelationshipsAsync(dbContext, entity.RelationshipTypeID, dto.OrganizationTypeIDs);

        return await GetByIDAsGridRowAsync(dbContext, entity.RelationshipTypeID);
    }

    public static async Task<RelationshipTypeGridRow?> UpdateAsync(WADNRDbContext dbContext, int relationshipTypeID, RelationshipTypeUpsertRequest dto)
    {
        var entity = await dbContext.RelationshipTypes
            .FirstAsync(x => x.RelationshipTypeID == relationshipTypeID);

        // Business rule: force CanOnlyBeRelatedOnceToAProject when CanStewardProjects or IsPrimaryContact
        if (dto.CanStewardProjects || dto.IsPrimaryContact)
        {
            dto.CanOnlyBeRelatedOnceToAProject = true;
        }

        entity.RelationshipTypeName = dto.RelationshipTypeName;
        entity.RelationshipTypeDescription = dto.RelationshipTypeDescription;
        entity.CanStewardProjects = dto.CanStewardProjects;
        entity.IsPrimaryContact = dto.IsPrimaryContact;
        entity.CanOnlyBeRelatedOnceToAProject = dto.CanOnlyBeRelatedOnceToAProject;
        entity.ShowOnFactSheet = dto.ShowOnFactSheet;
        entity.ReportInAccomplishmentsDashboard = dto.ReportInAccomplishmentsDashboard;

        await dbContext.SaveChangesAsync();

        await SyncOrganizationTypeRelationshipsAsync(dbContext, entity.RelationshipTypeID, dto.OrganizationTypeIDs);

        return await GetByIDAsGridRowAsync(dbContext, entity.RelationshipTypeID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int relationshipTypeID)
    {
        var hasProjectOrganizations = await dbContext.ProjectOrganizations
            .AnyAsync(po => po.RelationshipTypeID == relationshipTypeID);

        if (hasProjectOrganizations)
        {
            return false;
        }

        // Clean up junction table
        await dbContext.OrganizationTypeRelationshipTypes
            .Where(x => x.RelationshipTypeID == relationshipTypeID)
            .ExecuteDeleteAsync();

        var deletedCount = await dbContext.RelationshipTypes
            .Where(x => x.RelationshipTypeID == relationshipTypeID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    private static async Task SyncOrganizationTypeRelationshipsAsync(WADNRDbContext dbContext, int relationshipTypeID, List<int> organizationTypeIDs)
    {
        // Delete existing
        await dbContext.OrganizationTypeRelationshipTypes
            .Where(x => x.RelationshipTypeID == relationshipTypeID)
            .ExecuteDeleteAsync();

        // Re-insert
        if (organizationTypeIDs.Count > 0)
        {
            var newEntries = organizationTypeIDs.Select(otID => new OrganizationTypeRelationshipType
            {
                RelationshipTypeID = relationshipTypeID,
                OrganizationTypeID = otID,
            });
            dbContext.OrganizationTypeRelationshipTypes.AddRange(newEntries);
            await dbContext.SaveChangesAsync();
        }
    }
}
