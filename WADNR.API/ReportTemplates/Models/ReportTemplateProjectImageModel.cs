using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateProjectImageModel : ReportTemplateBaseModel
    {
        public string ImageTiming { get; set; }
        public string ImageCaption { get; set; }
        public string ImageCredit { get; set; }
        public string Image { get; set; }
        public bool IsKeyPhoto { get; set; }

        public ReportTemplateProjectImageModel(ProjectImage projectImage)
        {
            ImageTiming = projectImage.ProjectImageTimingID.HasValue
                && ProjectImageTiming.AllLookupDictionary.TryGetValue(projectImage.ProjectImageTimingID.Value, out var timing)
                ? timing.ProjectImageTimingDisplayName
                : string.Empty;
            ImageCaption = projectImage.Caption;
            ImageCredit = projectImage.Credit;
            Image = $"{projectImage.FileResource.FileResourceGUID}.{projectImage.FileResource.OriginalFileExtension}";
            IsKeyPhoto = projectImage.IsKeyPhoto;
        }
    }
}
