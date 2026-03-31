using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateProjectCountyModel : ReportTemplateBaseModel
    {
        private Project Project { get; set; }
        private ProjectCounty ProjectCounty { get; set; }
        private County County { get; set; }

        public string Name { get; set; }

        public ReportTemplateProjectCountyModel(ProjectCounty projectCounty)
        {
            Project = projectCounty.Project;
            ProjectCounty = projectCounty;
            County = projectCounty.County;

            Name = County.CountyName;
        }
    }
}
