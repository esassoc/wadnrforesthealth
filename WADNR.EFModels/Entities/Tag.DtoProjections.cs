using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class TagProjections
{
    public static readonly Expression<Func<Tag, TagDetail>> AsDetail = x => new TagDetail
    {
        TagID = x.TagID,
        TagName = x.TagName,
        TagDescription = x.TagDescription
    };

    public static readonly Expression<Func<Tag, TagGridRow>> AsGridRow = x => new TagGridRow
    {
        TagID = x.TagID,
        TagName = x.TagName,
        TagDescription = x.TagDescription,
        //todo: Permissions check
        ProjectCount = x.ProjectTags.Count()
    };
}
