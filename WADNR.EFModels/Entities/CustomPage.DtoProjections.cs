using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class CustomPageProjections
{
    public static readonly Expression<Func<CustomPage, CustomPageDetail>> AsDetail = x => new CustomPageDetail
    {
        CustomPageID = x.CustomPageID,
        CustomPageDisplayName = x.CustomPageDisplayName,
        CustomPageVanityUrl = x.CustomPageVanityUrl,
        CustomPageDisplayTypeID = x.CustomPageDisplayTypeID,
        CustomPageContent = x.CustomPageContent,
        CustomPageNavigationSectionID = x.CustomPageNavigationSectionID
    };
}
