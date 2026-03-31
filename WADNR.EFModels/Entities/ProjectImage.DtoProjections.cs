using System;
using System.Linq;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectImageProjections
{
    public static readonly Expression<Func<ProjectImage, ProjectImageGridRow>> AsGridRow = x => new ProjectImageGridRow
    {
        ProjectImageID = x.ProjectImageID,
        ProjectID = x.ProjectID,
        FileResourceID = x.FileResourceID,
        FileResourceGuid = x.FileResource.FileResourceGUID,
        Caption = x.Caption,
        Credit = x.Credit,
        IsKeyPhoto = x.IsKeyPhoto,
        ExcludeFromFactSheet = x.ExcludeFromFactSheet,
        ProjectImageTimingID = x.ProjectImageTimingID,
        ProjectImageTimingDisplayName = null, // Resolved client-side
        CreatedDate = x.FileResource.CreateDate,
        OriginalFilename = x.FileResource.OriginalBaseFilename + "." + x.FileResource.OriginalFileExtension,
        ContentLength = x.FileResource.ContentLength
    };

    public static readonly Expression<Func<ProjectImage, ProjectImageDetail>> AsDetail = x => new ProjectImageDetail
    {
        ProjectImageID = x.ProjectImageID,
        ProjectID = x.ProjectID,
        FileResourceID = x.FileResourceID,
        FileResourceGuid = x.FileResource.FileResourceGUID,
        Caption = x.Caption,
        Credit = x.Credit,
        IsKeyPhoto = x.IsKeyPhoto,
        ExcludeFromFactSheet = x.ExcludeFromFactSheet,
        ProjectImageTimingID = x.ProjectImageTimingID,
        ProjectImageTimingDisplayName = null, // Resolved client-side
        CreatedDate = x.FileResource.CreateDate,
        OriginalFilename = x.FileResource.OriginalBaseFilename + "." + x.FileResource.OriginalFileExtension,
        ContentLength = x.FileResource.ContentLength
    };

    public static readonly Expression<Func<ProjectImage, ProjectImageLookupItem>> AsLookupItem = x => new ProjectImageLookupItem
    {
        ProjectImageID = x.ProjectImageID,
        Caption = x.Caption,
        FileResourceGuid = x.FileResource.FileResourceGUID,
        IsKeyPhoto = x.IsKeyPhoto
    };
}
