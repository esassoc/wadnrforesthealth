using System.Linq.Expressions;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.EFModels.Entities;

public static class FirmaPageProjections
{
    public static readonly Expression<Func<FirmaPage, FirmaPageDetail>> AsDetail = x => new FirmaPageDetail
    {
        FirmaPageID = x.FirmaPageID,
        FirmaPageType = new FirmaPageTypeDetail
        {
            FirmaPageTypeID = x.FirmaPageType.FirmaPageTypeID,
            FirmaPageTypeName = x.FirmaPageType.FirmaPageTypeName,
            FirmaPageTypeDisplayName = x.FirmaPageType.FirmaPageTypeDisplayName,
            FirmaPageRenderTypeID = x.FirmaPageType.FirmaPageRenderTypeID
        },
        FirmaPageContent = x.FirmaPageContent,
    };
}