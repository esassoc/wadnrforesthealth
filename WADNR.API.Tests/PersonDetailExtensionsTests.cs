using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests;

[TestClass]
public class PersonDetailExtensionsTests
{
    private static PersonDetail MakePerson(
        int baseRoleID = (int)RoleEnum.Normal,
        int[]? supplementalRoleIDs = null)
    {
        return new PersonDetail
        {
            PersonID = 100,
            BaseRole = new RoleLookupItem { RoleID = baseRoleID, RoleName = $"Role{baseRoleID}" },
            SupplementalRoleList = supplementalRoleIDs?
                .Select(id => new RoleLookupItem { RoleID = id, RoleName = $"Role{id}" })
                .ToList() ?? [],
        };
    }

    #region CanViewAdminLimitedProjects

    [TestMethod]
    public void CanViewAdminLimitedProjects_ReturnsTrue_WhenHasElevatedAccess()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.Admin);
        Assert.IsTrue(person.CanViewAdminLimitedProjects());
    }

    [TestMethod]
    public void CanViewAdminLimitedProjects_ReturnsTrue_WhenHasCanEditProgramRole()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.Normal,
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram]);
        Assert.IsTrue(person.CanViewAdminLimitedProjects());
    }

    [TestMethod]
    public void CanViewAdminLimitedProjects_ReturnsFalse_WhenNormalWithNoSupplemental()
    {
        var person = MakePerson();
        Assert.IsFalse(person.CanViewAdminLimitedProjects());
    }

    [TestMethod]
    public void CanViewAdminLimitedProjects_ReturnsFalse_WhenNull()
    {
        PersonDetail? person = null;
        Assert.IsFalse(person.CanViewAdminLimitedProjects());
    }

    #endregion
}
