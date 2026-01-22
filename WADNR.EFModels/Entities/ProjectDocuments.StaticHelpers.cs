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
}
