using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectDocumentProjections
{
    public static Expression<Func<ProjectDocument, ProjectDocumentDetail>> AsDetail => x => new ProjectDocumentDetail
    {
        ProjectDocumentID = x.ProjectDocumentID,
        ProjectID = x.ProjectID,
        DisplayName = x.DisplayName,
        Description = x.Description,
        ProjectDocumentTypeID = x.ProjectDocumentTypeID,
        ProjectDocumentTypeDisplayName = null, // Resolved client-side (static lookup)
        FileResourceID = x.FileResourceID,
        FileResourceGuid = x.FileResource.FileResourceGUID.ToString(),
        OriginalFileName = x.FileResource.OriginalBaseFilename + "." + x.FileResource.OriginalFileExtension
    };
}
