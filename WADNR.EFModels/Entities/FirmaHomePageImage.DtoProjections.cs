using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FirmaHomePageImageProjections
{
    public static readonly Expression<Func<FirmaHomePageImage, FirmaHomePageImageDetail>> AsDetail = x => new FirmaHomePageImageDetail
    {
        FirmaHomePageImageID = x.FirmaHomePageImageID,
        FileResourceGUID = x.FileResource.FileResourceGUID,
        Caption = x.Caption,
        SortOrder = x.SortOrder,
        ContentLength = x.FileResource.ContentLength,
    };
}
