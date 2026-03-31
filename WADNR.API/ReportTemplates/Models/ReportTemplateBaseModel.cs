using System.Globalization;

namespace WADNR.API.ReportTemplates.Models
{
    public abstract class ReportTemplateBaseModel
    {
        protected static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");
    }
}
