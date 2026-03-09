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

    public static readonly Expression<Func<CustomPage, CustomPageMenuItem>> AsMenuItem = x => new CustomPageMenuItem
    {
        CustomPageID = x.CustomPageID,
        CustomPageDisplayName = x.CustomPageDisplayName,
        CustomPageVanityUrl = x.CustomPageVanityUrl
    };

    public static readonly Expression<Func<CustomPage, CustomPageGridRow>> AsGridRow = x => new CustomPageGridRow
    {
        CustomPageID = x.CustomPageID,
        CustomPageDisplayName = x.CustomPageDisplayName,
        CustomPageVanityUrl = x.CustomPageVanityUrl,
        CustomPageDisplayTypeID = x.CustomPageDisplayTypeID,
        CustomPageDisplayTypeName = null,
        CustomPageNavigationSectionID = x.CustomPageNavigationSectionID,
        CustomPageNavigationSectionName = null,
        HasContent = x.CustomPageContent != null && x.CustomPageContent.Length > 0,
        CustomPageContent = x.CustomPageContent
    };
}
