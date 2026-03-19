using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Test implementation of IAuditUserProvider that allows setting the current user.
/// </summary>
public class TestAuditUserProvider : IAuditUserProvider
{
    private int _personID;

    public TestAuditUserProvider(int personID)
    {
        _personID = personID;
    }

    public void SetPersonID(int personID)
    {
        _personID = personID;
    }

    public int GetCurrentPersonID()
    {
        return _personID;
    }
}
