using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.FindYourForester;

namespace WADNR.EFModels.Entities;

public static class ForesterWorkUnitProjections
{
    public static readonly Expression<Func<ForesterWorkUnit, ForesterWorkUnitGridRow>> AsGridRow = x => new ForesterWorkUnitGridRow
    {
        ForesterWorkUnitID = x.ForesterWorkUnitID,
        ForesterRoleID = x.ForesterRoleID,
        ForesterRoleDisplayName = string.Empty, // Resolved client-side via lookup dictionary
        ForesterWorkUnitName = x.ForesterWorkUnitName,
        RegionName = x.RegionName,
        PersonID = x.PersonID,
        AssignedPersonName = x.Person != null
            ? x.Person.FirstName + " " + x.Person.LastName
            : null
    };

    public static readonly Expression<Func<ForesterWorkUnit, ForesterContactResult>> AsContactResult = x => new ForesterContactResult
    {
        ForesterWorkUnitID = x.ForesterWorkUnitID,
        ForesterRoleID = x.ForesterRoleID,
        ForesterRoleDisplayName = null, // Resolved client-side via lookup dictionary
        PersonID = x.PersonID,
        FirstName = x.Person != null ? x.Person.FirstName : null,
        LastName = x.Person != null ? x.Person.LastName : null,
        Email = x.Person != null ? x.Person.Email : null,
        Phone = x.Person != null ? x.Person.Phone : null,
        ForesterWorkUnitName = x.ForesterWorkUnitName,
        ForesterRoleDefinition = null, // Resolved client-side via FieldDefinition lookup
    };
}
