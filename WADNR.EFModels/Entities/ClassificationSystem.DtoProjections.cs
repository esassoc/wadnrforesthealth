using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ClassificationSystem;

namespace WADNR.EFModels.Entities;

public static class ClassificationSystemProjections
{
    public static Expression<Func<ClassificationSystem, ClassificationSystemGridRow>> AsGridRow => x => new ClassificationSystemGridRow
    {
        ClassificationSystemID = x.ClassificationSystemID,
        ClassificationSystemName = x.ClassificationSystemName,
        ClassificationCount = x.Classifications.Count
    };

    public static Expression<Func<ClassificationSystem, ClassificationSystemDetail>> AsDetail => x => new ClassificationSystemDetail
    {
        ClassificationSystemID = x.ClassificationSystemID,
        ClassificationSystemName = x.ClassificationSystemName,
        ClassificationSystemDefinition = x.ClassificationSystemDefinition,
        ClassificationSystemListPageContent = x.ClassificationSystemListPageContent,
        ClassificationCount = x.Classifications.Count
    };

    public static Expression<Func<ClassificationSystem, ClassificationSystemLookupItem>> AsLookupItem => x => new ClassificationSystemLookupItem
    {
        ClassificationSystemID = x.ClassificationSystemID,
        ClassificationSystemName = x.ClassificationSystemName
    };
}
