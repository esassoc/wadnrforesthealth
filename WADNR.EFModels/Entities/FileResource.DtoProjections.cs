using System;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.FileResource;

namespace WADNR.EFModels.Entities;

public static class FileResourceProjections
{
    public static readonly Expression<Func<PriorityLandscapeFileResource, FileResourceDetail>> AsDetail = x => new FileResourceDetail
    {
        FileResourceID = x.FileResource.FileResourceID,
        FileResourceGUID = x.FileResource.FileResourceGUID,
        DisplayName = x.DisplayName,
        Description = x.Description,
        FileResourceMIMETypeDisplayName = x.FileResource.FileResourceMimeType.FileResourceMimeTypeDisplayName,
        CreateDate = x.FileResource.CreateDate
    };
}
