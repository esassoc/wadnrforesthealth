using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateProjectContactModel : ReportTemplateBaseModel
    {
        private ProjectPerson ProjectPerson { get; set; }
        private Organization Organization { get; set; }

        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ContactType { get; set; }

        public ReportTemplateProjectContactModel(ProjectPerson projectPerson)
        {
            ProjectPerson = projectPerson;
            Organization = ProjectPerson.Person.Organization;

            FullName = $"{ProjectPerson.Person.FirstName} {ProjectPerson.Person.LastName}";
            FirstName = ProjectPerson.Person.FirstName;
            LastName = ProjectPerson.Person.LastName;
            Email = ProjectPerson.Person.Email;
            Phone = ProjectPerson.Person.Phone;
            Address = ProjectPerson.Person.PersonAddress;
            ContactType = ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(
                ProjectPerson.ProjectPersonRelationshipTypeID, out var relType)
                ? relType.ProjectPersonRelationshipTypeDisplayName
                : string.Empty;
        }

        public ReportTemplateOrganizationModel GetOrganization()
        {
            return new ReportTemplateOrganizationModel(Organization);
        }
    }
}
