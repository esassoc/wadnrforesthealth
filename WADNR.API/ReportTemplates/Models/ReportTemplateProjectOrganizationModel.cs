using System.Collections.Generic;
using System.Linq;
using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateProjectOrganizationModel : ReportTemplateBaseModel
    {
        private ProjectOrganization ProjectOrganization { get; set; }
        private List<Person> People { get; set; }

        public string OrganizationName { get; set; }
        public string OrganizationShortName { get; set; }
        public string OrganizationTypeName { get; set; }
        public string OrganizationTypeAbbreviation { get; set; }
        public string OrganizationRelationshipType { get; set; }

        public ReportTemplateProjectOrganizationModel(ProjectOrganization projectOrganization)
        {
            ProjectOrganization = projectOrganization;
            People = projectOrganization.Organization?.People?.ToList() ?? new List<Person>();

            OrganizationName = ProjectOrganization.Organization?.OrganizationName;
            OrganizationShortName = ProjectOrganization.Organization?.OrganizationShortName;
            OrganizationTypeName = ProjectOrganization.Organization?.OrganizationType?.OrganizationTypeName;
            OrganizationTypeAbbreviation = ProjectOrganization.Organization?.OrganizationType?.OrganizationTypeAbbreviation;
            OrganizationRelationshipType = ProjectOrganization.RelationshipType?.RelationshipTypeName;
        }

        public List<ReportTemplatePersonModel> GetPeople()
        {
            return People.Select(x => new ReportTemplatePersonModel(x)).ToList();
        }

        public ReportTemplatePersonModel GetPrimaryContactPerson()
        {
            return new ReportTemplatePersonModel(ProjectOrganization.Organization?.PrimaryContactPerson);
        }
    }
}
