using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class PersonProjections
{
    public static readonly Expression<Func<Person, PersonDetail>> AsDetail = x => new PersonDetail
    {
        PersonID = x.PersonID,
        FirstName = x.FirstName,
        MiddleName = x.MiddleName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone,
        PersonAddress = x.PersonAddress,
        Notes = x.Notes,
        CreateDate = x.CreateDate,
        UpdateDate = x.UpdateDate,
        LastActivityDate = x.LastActivityDate,
        IsActive = x.IsActive,
        ReceiveSupportEmails = x.ReceiveSupportEmails,
        OrganizationID = x.OrganizationID,
        OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null,
        VendorID = x.VendorID,
        VendorName = x.Vendor != null ? x.Vendor.VendorName : null,
        AddedByPersonID = x.AddedByPersonID,
        AddedByPersonName = x.AddedByPerson != null ? x.AddedByPerson.FirstName + " " + x.AddedByPerson.LastName : null,
        PrimaryContactOrganizationCount = x.Organizations.Count,
        ProjectCount = x.ProjectPeople.Count,
        AgreementCount = x.AgreementPeople.Count,
        InteractionEventCount = x.InteractionEventContacts.Count,
        PrimaryContactOrganizations = x.Organizations.Select(o => new OrganizationLookupItem
        {
            OrganizationID = o.OrganizationID,
            OrganizationName = o.OrganizationName
        }).ToList(),
        AssignedPrograms = x.ProgramPeople.Select(pp => new ProgramLookupItem
        {
            ProgramID = pp.Program.ProgramID,
            ProgramName = pp.Program.ProgramName
        }).ToList(),
        Authenticators = x.PersonAllowedAuthenticators
            .Select(a => a.Authenticator.AuthenticatorFullName)
            .ToList()
    };

    public static readonly Expression<Func<Person, PersonLookupItem>> AsLookupItem = x => new PersonLookupItem
    {
        PersonID = x.PersonID,
        FullName = x.FirstName + " " + x.LastName
    };

    public static readonly Expression<Func<Person, PersonGridRow>> AsGridRow = x => new PersonGridRow
    {
        PersonID = x.PersonID,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        OrganizationID = x.OrganizationID,
        OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null,
        OrganizationShortName = x.Organization != null ? x.Organization.OrganizationShortName : null,
        Phone = x.Phone,
        LastActivityDate = x.LastActivityDate,
        IsActive = x.IsActive,
        PrimaryContactOrganizationCount = x.Organizations.Count,
        CreateDate = x.CreateDate,
        AddedByPersonID = x.AddedByPersonID,
        AddedByPersonName = x.AddedByPerson != null ? x.AddedByPerson.FirstName + " " + x.AddedByPerson.LastName : null
    };
}