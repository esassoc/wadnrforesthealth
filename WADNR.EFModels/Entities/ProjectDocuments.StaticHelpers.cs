using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectDocuments
{
    public static async Task<List<ProjectDocumentGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var rawDocuments = await dbContext.ProjectDocuments
            .AsNoTracking()
            .Where(d => d.ProjectID == projectID)
            .Select(d => new
            {
                d.ProjectDocumentID,
                d.DisplayName,
                d.Description,
                d.ProjectDocumentTypeID,
                d.FileResourceID,
                FileResourceGuid = d.FileResource.FileResourceGUID.ToString()
            })
            .ToListAsync();

        var documents = rawDocuments
            .Select(d => new ProjectDocumentGridRow
            {
                ProjectDocumentID = d.ProjectDocumentID,
                DisplayName = d.DisplayName,
                Description = d.Description,
                DocumentTypeName = d.ProjectDocumentTypeID.HasValue && ProjectDocumentType.AllLookupDictionary.TryGetValue(d.ProjectDocumentTypeID.Value, out var dt)
                    ? dt.ProjectDocumentTypeDisplayName
                    : null,
                FileResourceID = d.FileResourceID,
                FileResourceGuid = d.FileResourceGuid
            })
            .OrderBy(d => d.DisplayName)
            .ToList();

        return documents;
    }

    public static async Task<ProjectDocumentDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectDocumentID)
    {
        var detail = await dbContext.ProjectDocuments
            .AsNoTracking()
            .Where(x => x.ProjectDocumentID == projectDocumentID)
            .Select(ProjectDocumentProjections.AsDetail)
            .SingleOrDefaultAsync();

        // Resolve static lookup value client-side
        if (detail?.ProjectDocumentTypeID != null && ProjectDocumentType.AllLookupDictionary.TryGetValue(detail.ProjectDocumentTypeID.Value, out var documentType))
        {
            detail.ProjectDocumentTypeDisplayName = documentType.ProjectDocumentTypeDisplayName;
        }

        return detail;
    }

    public static async Task<ProjectDocument> CreateAsync(WADNRDbContext dbContext, int projectID, string displayName, string? description, int? projectDocumentTypeID, int fileResourceID)
    {
        var projectDocument = new ProjectDocument
        {
            ProjectID = projectID,
            DisplayName = displayName,
            Description = description,
            ProjectDocumentTypeID = projectDocumentTypeID,
            FileResourceID = fileResourceID
        };

        dbContext.ProjectDocuments.Add(projectDocument);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(projectDocument).ReloadAsync();

        return projectDocument;
    }

    public static async Task UpdateAsync(WADNRDbContext dbContext, ProjectDocument projectDocument, ProjectDocumentUpsertRequest request)
    {
        projectDocument.DisplayName = request.DisplayName;
        projectDocument.Description = request.Description;
        projectDocument.ProjectDocumentTypeID = request.ProjectDocumentTypeID;

        await dbContext.SaveChangesAsync();
    }

    public static async Task<Guid> DeleteAsync(WADNRDbContext dbContext, ProjectDocument projectDocument)
    {
        // Get the FileResource GUID before deleting for blob cleanup
        var fileResourceGuid = await dbContext.FileResources
            .Where(f => f.FileResourceID == projectDocument.FileResourceID)
            .Select(f => f.FileResourceGUID)
            .SingleAsync();

        // Delete the ProjectDocument
        dbContext.ProjectDocuments.Remove(projectDocument);
        await dbContext.SaveChangesAsync();

        // Delete the FileResource
        var fileResource = await dbContext.FileResources.FindAsync(projectDocument.FileResourceID);
        if (fileResource != null)
        {
            dbContext.FileResources.Remove(fileResource);
            await dbContext.SaveChangesAsync();
        }

        return fileResourceGuid;
    }

    public static async Task<bool> IsDisplayNameUniqueForProjectAsync(WADNRDbContext dbContext, int projectID, string displayName, int? excludeProjectDocumentID = null)
    {
        var query = dbContext.ProjectDocuments
            .AsNoTracking()
            .Where(x => x.ProjectID == projectID && x.DisplayName == displayName);

        if (excludeProjectDocumentID.HasValue)
        {
            query = query.Where(x => x.ProjectDocumentID != excludeProjectDocumentID.Value);
        }

        return !await query.AnyAsync();
    }

    public static List<ProjectDocumentTypeLookupItem> ListTypesAsLookupItem()
    {
        return ProjectDocumentType.All
            .OrderBy(t => t.ProjectDocumentTypeDisplayName)
            .Select(t => new ProjectDocumentTypeLookupItem
            {
                ProjectDocumentTypeID = t.ProjectDocumentTypeID,
                ProjectDocumentTypeDisplayName = t.ProjectDocumentTypeDisplayName
            })
            .ToList();
    }
}
