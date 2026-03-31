using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ReportTemplateProjections
{
    public static Expression<Func<ReportTemplate, ReportTemplateDetail>> AsDetail => x => new ReportTemplateDetail
    {
        ReportTemplateID = x.ReportTemplateID,
        FileResourceID = x.FileResourceID,
        FileResourceGuid = x.FileResource.FileResourceGUID.ToString(),
        OriginalFileName = x.FileResource.OriginalBaseFilename + (x.FileResource.OriginalFileExtension.StartsWith(".") ? "" : ".") + x.FileResource.OriginalFileExtension,
        DisplayName = x.DisplayName,
        Description = x.Description,
        ReportTemplateModelID = x.ReportTemplateModelID,
        ReportTemplateModelTypeID = x.ReportTemplateModelTypeID,
        IsSystemTemplate = x.IsSystemTemplate
    };

    public static Expression<Func<ReportTemplate, ReportTemplateGridRow>> AsGridRow => x => new ReportTemplateGridRow
    {
        ReportTemplateID = x.ReportTemplateID,
        DisplayName = x.DisplayName,
        Description = x.Description,
        ReportTemplateModelID = x.ReportTemplateModelID,
        IsSystemTemplate = x.IsSystemTemplate,
        FileResourceGuid = x.FileResource.FileResourceGUID.ToString(),
        OriginalFileName = x.FileResource.OriginalBaseFilename + (x.FileResource.OriginalFileExtension.StartsWith(".") ? "" : ".") + x.FileResource.OriginalFileExtension
    };
}
