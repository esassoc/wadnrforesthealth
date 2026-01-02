using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class PersonProjections
{
    public static readonly Expression<Func<Person, PersonDetail>> AsDetail = x => new PersonDetail
    {
        PersonID = x.PersonID,
        FirstName = x.FirstName,
        LastName = x.LastName,
        Email = x.Email,
        Phone = x.Phone,
        CreateDate = x.CreateDate,
        UpdateDate = x.UpdateDate,
        LastActivityDate = x.LastActivityDate,
        IsActive = x.IsActive,
        OrganizationID = x.OrganizationID,
        WebServiceAccessToken = x.WebServiceAccessToken,
    };

    public static readonly Expression<Func<Person, PersonLookupItem>> AsLookupItem = x => new PersonLookupItem
    {
        PersonID = x.PersonID,
        FullName = x.FirstName + " " + x.LastName
    };
}