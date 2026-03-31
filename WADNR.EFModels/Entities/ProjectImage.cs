using System;

namespace WADNR.EFModels.Entities;

public partial class ProjectImage
{
    public DateTime CreatedDate => FileResource.CreateDate;

    public Guid FileResourceGuid => FileResource.FileResourceGUID;

    public string CaptionOnFullView
    {
        get
        {
            var creditString = string.IsNullOrWhiteSpace(Credit) ? string.Empty : $"\r\nCredit: {Credit}";
            var timingString = ProjectImageTiming != null ? $"(Timing: {ProjectImageTiming.ProjectImageTimingDisplayName}) " : string.Empty;
            return $"{Caption}\r\n{timingString}{creditString}";
        }
    }
}
