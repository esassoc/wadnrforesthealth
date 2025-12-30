using System.Linq.Expressions;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.EFModels.Entities;

public static class PersonProjections
{
    // Reusable projection for TreatmentBMPVersionAssessment -> TreatmentBMPVersionAssessmentDto
    public static readonly Expression<Func<Person, PersonSimpleDto>> AsSimpleDto = x => new PersonSimpleDto
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

}