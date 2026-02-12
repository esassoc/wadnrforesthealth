using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplatePersonModel : ReportTemplateBaseModel
    {
        private Person Person { get; set; }
        private Organization Organization { get; set; }

        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public ReportTemplatePersonModel(Person person)
        {
            Person = person;
            if (Person != null)
            {
                Organization = Person.Organization;

                FullName = $"{Person.FirstName} {Person.LastName}";
                FirstName = Person.FirstName;
                LastName = Person.LastName;
                Email = Person.Email;
                Phone = Person.Phone;
                Address = Person.PersonAddress;
            }
        }

        public ReportTemplateOrganizationModel GetOrganization()
        {
            return new ReportTemplateOrganizationModel(Organization);
        }
    }
}
