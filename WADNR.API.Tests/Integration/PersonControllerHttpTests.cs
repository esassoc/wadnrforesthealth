using System.Net;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class PersonControllerHttpTests
{
    private int _testPersonID;
    private readonly List<int> _createdPersonIDs = [];

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var person = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _testPersonID = person.PersonID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        foreach (var personID in _createdPersonIDs)
        {
            try { await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, personID); } catch { }
        }
        try
        {
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, _testPersonID);
        }
        catch { }
    }

    #region List Tests

    [TestMethod]
    public async Task List_Returns200_WithPeople()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var people = await result.DeserializeContentAsync<List<PersonGridRow>>();
        Assert.IsNotNull(people);
        Assert.IsTrue(people.Count > 0);
    }

    [TestMethod]
    public async Task ListLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.ListLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListWadnrLookup_Returns200()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.ListWadnrLookup());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_Returns200_WhenExists()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Get(_testPersonID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var person = await result.DeserializeContentAsync<PersonDetail>();
        Assert.IsNotNull(person);
        Assert.AreEqual(_testPersonID, person.PersonID);
    }

    [TestMethod]
    public async Task Get_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Related Data Tests

    [TestMethod]
    public async Task ListProjects_Returns200()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.ListProjects(_testPersonID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListAgreements_Returns200()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.ListAgreements(_testPersonID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Authorization Tests

    [TestMethod]
    public async Task List_Returns401_WhenUnauthenticated()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    #endregion

    #region PersonViewFeature Tests

    [TestMethod]
    public async Task Get_Returns200_WhenNormalUserViewsSelf()
    {
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Get(AssemblySteps.TestNormalPersonID));
        var result = await AssemblySteps.NormalHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Normal user should be able to view self. Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task Get_Returns403_WhenUnassignedViewsOtherPerson()
    {
        // Create an Unassigned user
        var unassigned = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Unassigned);
        _createdPersonIDs.Add(unassigned.PersonID);

        // Try to view a different person's record
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Get(_testPersonID));
        var result = await HttpResponseHelper.GetAsUserAsync(route, unassigned.GlobalID!);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            "Unassigned user should not be able to view another person's record.");
    }

    #endregion

    #region PersonEditFeature Tests

    [TestMethod]
    public async Task Update_Returns200_WhenNormalUserEditsSelf()
    {
        // Create a Normal user we can edit
        var normalUser = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        _createdPersonIDs.Add(normalUser.PersonID);

        var request = new PersonUpsertRequest
        {
            FirstName = normalUser.FirstName,
            LastName = normalUser.LastName,
            Email = normalUser.Email,
            OrganizationID = normalUser.OrganizationID,
            IsUser = true,
        };

        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Update(normalUser.PersonID, null!));
        var result = await HttpResponseHelper.PutAsUserAsync(route, normalUser.GlobalID!, request);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"Normal user should be able to edit self. Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task Update_Returns403_WhenNormalUserEditsOtherPerson()
    {
        // Create a Normal user from a non-WADNR org (use any org that isn't 4704)
        var nonWadnrOrgID = await AssemblySteps.DbContext.Organizations
            .Where(o => o.OrganizationID != 4704)
            .Select(o => o.OrganizationID)
            .FirstAsync();
        var normalUser = await PersonHelper.CreateUserAsync(
            AssemblySteps.DbContext,
            RoleEnum.Normal,
            organizationID: nonWadnrOrgID);
        _createdPersonIDs.Add(normalUser.PersonID);

        var request = new PersonUpsertRequest
        {
            FirstName = "Unauthorized",
            LastName = "Edit",
            IsUser = false,
        };

        // Try to edit _testPersonID (a different person)
        var route = RouteHelper.GetRouteFor<PersonController>(c => c.Update(_testPersonID, null!));
        var result = await HttpResponseHelper.PutAsUserAsync(route, normalUser.GlobalID!, request);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode,
            "Normal user from non-WADNR org should not be able to edit another person.");
    }

    #endregion
}
