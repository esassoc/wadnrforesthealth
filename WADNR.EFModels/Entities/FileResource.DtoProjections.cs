using System;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.FileResource;

namespace WADNR.EFModels.Entities;

public static class FileResourceProjections
{
    public static readonly Expression<Func<PriorityLandscapeFileResource, FileResourcePriorityLandscapeDetail>> AsPriorityLandscapeDetail = x => new FileResourcePriorityLandscapeDetail
    {
        FileResourceID = x.FileResource.FileResourceID,
        FileResourceGUID = x.FileResource.FileResourceGUID,
        DisplayName = x.DisplayName,
        Description = x.Description,
        FileResourceMIMETypeDisplayName = x.FileResource.FileResourceMimeType.FileResourceMimeTypeDisplayName,
        CreateDate = x.FileResource.CreateDate
    };

    public static readonly Expression<Func<InteractionEventFileResource, FileResourceInteractionEventDetail>> AsInteractionEventDetail = x => new FileResourceInteractionEventDetail
    {
        InteractionEventFileResourceID = x.InteractionEventFileResourceID,
        FileResourceID = x.FileResource.FileResourceID,
        FileResourceGUID = x.FileResource.FileResourceGUID,
        DisplayName = x.DisplayName,
        Description = x.Description,
        FileResourceMIMETypeDisplayName = x.FileResource.FileResourceMimeType.FileResourceMimeTypeDisplayName,
        CreateDate = x.FileResource.CreateDate
    };
}
