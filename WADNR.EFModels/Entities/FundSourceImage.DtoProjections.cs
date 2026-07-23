using System;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceImageProjections
{
    public static readonly Expression<Func<FundSourceImage, FundSourceImageGridRow>> AsGridRow = x => new FundSourceImageGridRow
    {
        FundSourceImageID = x.FundSourceImageID,
        FundSourceID = x.FundSourceID,
        FileResourceID = x.FileResourceID,
        FileResourceGuid = x.FileResource.FileResourceGUID,
        Caption = x.Caption,
        Credit = x.Credit,
        IsKeyPhoto = x.IsKeyPhoto,
        CreatedDate = x.FileResource.CreateDate,
        OriginalFilename = x.FileResource.OriginalBaseFilename + "." + x.FileResource.OriginalFileExtension,
        ContentLength = x.FileResource.ContentLength
    };

    public static readonly Expression<Func<FundSourceImage, FundSourceImageDetail>> AsDetail = x => new FundSourceImageDetail
    {
        FundSourceImageID = x.FundSourceImageID,
        FundSourceID = x.FundSourceID,
        FileResourceID = x.FileResourceID,
        FileResourceGuid = x.FileResource.FileResourceGUID,
        Caption = x.Caption,
        Credit = x.Credit,
        IsKeyPhoto = x.IsKeyPhoto,
        CreatedDate = x.FileResource.CreateDate,
        OriginalFilename = x.FileResource.OriginalBaseFilename + "." + x.FileResource.OriginalFileExtension,
        ContentLength = x.FileResource.ContentLength
    };
}
