using System.Net;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Controllers;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

[TestClass]
[DoNotParallelize]
public class AgreementControllerHttpTests
{
    private int _testAgreementID;
    private int _testOrganizationID;
    private readonly List<int> _createdAgreementIDs = new();
    private readonly List<int> _createdPersonIDs = new();

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var organization = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _testOrganizationID = organization.OrganizationID;

        var agreement = await AgreementHelper.CreateAgreementAsync(AssemblySteps.DbContext, organizationID: _testOrganizationID);
        _testAgreementID = agreement.AgreementID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        foreach (var id in _createdAgreementIDs)
        {
            try { await AgreementHelper.DeleteAgreementAsync(AssemblySteps.DbContext, id); } catch { }
        }
        foreach (var id in _createdPersonIDs)
        {
            try { await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, id); } catch { }
        }
        try
        {
            await AgreementHelper.DeleteAgreementAsync(AssemblySteps.DbContext, _testAgreementID);
            await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, _testOrganizationID);
        }
        catch { }
    }

    #region List Tests

    [TestMethod]
    public async Task List_Returns200_WithAgreements()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.List());
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var agreements = await result.DeserializeContentAsync<List<AgreementGridRow>>();
        Assert.IsNotNull(agreements);
        Assert.IsTrue(agreements.Any(a => a.AgreementID == _testAgreementID));
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_Returns200_WhenExists()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.Get(_testAgreementID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var agreement = await result.DeserializeContentAsync<AgreementDetail>();
        Assert.IsNotNull(agreement);
        Assert.AreEqual(_testAgreementID, agreement.AgreementID);
    }

    [TestMethod]
    public async Task Get_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.Get(-1));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Related Data Tests

    [TestMethod]
    public async Task ListFundSourceAllocations_Returns200()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.ListFundSourceAllocations(_testAgreementID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListProjects_Returns200()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.ListProjects(_testAgreementID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task ListContacts_Returns200()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.ListContacts(_testAgreementID));
        var result = await AssemblySteps.AdminHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion

    #region Create Tests

    [TestMethod]
    public async Task Create_Returns201_WithValidRequest()
    {
        var agreementType = await AssemblySteps.DbContext.AgreementTypes.FirstAsync();
        var agreementStatus = await AssemblySteps.DbContext.AgreementStatuses.FirstAsync();

        var request = new AgreementUpsertRequest
        {
            AgreementTitle = $"HTTP Create Test {DateTime.UtcNow.Ticks}",
            AgreementNumber = $"HTTP-{DateTime.UtcNow.Ticks % 100000}",
            AgreementTypeID = agreementType.AgreementTypeID,
            AgreementStatusID = agreementStatus.AgreementStatusID,
            OrganizationID = _testOrganizationID,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
        };

        var route = RouteHelper.GetRouteTemplateFor(typeof(AgreementController),
            typeof(AgreementController).GetMethod(nameof(AgreementController.Create))!);
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Created, result.StatusCode,
            $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");

        var created = await result.DeserializeContentAsync<AgreementDetail>();
        Assert.IsNotNull(created);
        Assert.AreEqual(request.AgreementTitle, created.AgreementTitle);
        Assert.IsTrue(created.AgreementID > 0);
        _createdAgreementIDs.Add(created.AgreementID);
    }

    [TestMethod]
    public async Task Create_Returns401_WhenUnauthenticated()
    {
        var request = new AgreementUpsertRequest
        {
            AgreementTitle = "Should not be created",
            AgreementTypeID = 1,
            OrganizationID = _testOrganizationID,
        };

        var route = RouteHelper.GetRouteTemplateFor(typeof(AgreementController),
            typeof(AgreementController).GetMethod(nameof(AgreementController.Create))!);
        var result = await AssemblySteps.UnauthenticatedHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task Create_Returns403_ForNormalUser()
    {
        // AgreementManageFeature requires Admin/EsaAdmin or CanManageFundSourcesAndAgreements supplemental role
        var request = new AgreementUpsertRequest
        {
            AgreementTitle = "Should not be created",
            AgreementTypeID = 1,
            OrganizationID = _testOrganizationID,
        };

        var route = RouteHelper.GetRouteTemplateFor(typeof(AgreementController),
            typeof(AgreementController).GetMethod(nameof(AgreementController.Create))!);
        var result = await AssemblySteps.NormalHttpClient.PostAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
    }

    #endregion

    #region Update Tests

    [TestMethod]
    public async Task Update_Returns200_WithUpdatedTitle()
    {
        var original = await AssemblySteps.AdminHttpClient.GetAsync(
            RouteHelper.GetRouteFor<AgreementController>(c => c.Get(_testAgreementID)));
        var detail = await original.DeserializeContentAsync<AgreementDetail>();

        var newTitle = $"Updated via HTTP {DateTime.UtcNow.Ticks}";
        var request = new AgreementUpsertRequest
        {
            AgreementTitle = newTitle,
            AgreementNumber = detail.AgreementNumber,
            AgreementTypeID = detail.AgreementType.AgreementTypeID,
            AgreementStatusID = detail.AgreementStatus?.AgreementStatusID,
            OrganizationID = detail.ContributingOrganization.OrganizationID,
            StartDate = detail.StartDate,
            EndDate = detail.EndDate,
        };

        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.Update(_testAgreementID, request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var updated = await result.DeserializeContentAsync<AgreementDetail>();
        Assert.IsNotNull(updated);
        Assert.AreEqual(newTitle, updated.AgreementTitle);
    }

    [TestMethod]
    public async Task Update_Returns404_WhenNotExists()
    {
        var request = new AgreementUpsertRequest
        {
            AgreementTitle = "Test",
            AgreementTypeID = 1,
            OrganizationID = _testOrganizationID,
        };

        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.Update(-1, request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Delete Tests

    [TestMethod]
    public async Task Delete_Returns204_WhenExists()
    {
        // Create a separate agreement to delete
        var toDelete = await AgreementHelper.CreateAgreementAsync(AssemblySteps.DbContext, organizationID: _testOrganizationID);

        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.Delete(toDelete.AgreementID));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
    }

    [TestMethod]
    public async Task Delete_Returns404_WhenNotExists()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.Delete(-1));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Update Fund Source Allocations Tests

    [TestMethod]
    public async Task UpdateFundSourceAllocations_Returns200()
    {
        var request = new AgreementFundSourceAllocationsUpdateRequest
        {
            FundSourceAllocationIDs = new List<int>()
        };

        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.UpdateFundSourceAllocations(_testAgreementID, request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var allocations = await result.DeserializeContentAsync<List<FundSourceAllocationLookupItem>>();
        Assert.IsNotNull(allocations);
    }

    #endregion

    #region Update Projects Tests

    [TestMethod]
    public async Task UpdateProjects_Returns200()
    {
        var request = new AgreementProjectsUpdateRequest
        {
            ProjectIDs = new List<int>()
        };

        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.UpdateProjects(_testAgreementID, request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var projects = await result.DeserializeContentAsync<List<ProjectLookupItem>>();
        Assert.IsNotNull(projects);
    }

    #endregion

    #region Contact CRUD Tests

    [TestMethod]
    public async Task CreateContact_Returns200_WithValidRequest()
    {
        var person = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(person.PersonID);
        var agreementPersonRoleID = AgreementPersonRole.All.First().AgreementPersonRoleID;

        var request = new AgreementContactUpsertRequest
        {
            PersonID = person.PersonID,
            AgreementPersonRoleID = agreementPersonRoleID,
        };

        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.CreateContact(_testAgreementID, request));
        var result = await AssemblySteps.AdminHttpClient.PostAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var contact = await result.DeserializeContentAsync<AgreementContactGridRow>();
        Assert.IsNotNull(contact);
        Assert.AreEqual(person.PersonID, contact.Person.PersonID);
    }

    [TestMethod]
    public async Task UpdateContact_Returns200_WhenValid()
    {
        // Create a contact to update
        var person1 = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        var person2 = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(person1.PersonID);
        _createdPersonIDs.Add(person2.PersonID);
        var roleID = AgreementPersonRole.All.First().AgreementPersonRoleID;

        await AgreementHelper.AddContactAsync(AssemblySteps.DbContext, _testAgreementID, person1.PersonID, roleID);

        // Get the created contact's AgreementPersonID
        var contacts = await AssemblySteps.DbContext.AgreementPeople.AsNoTracking()
            .Where(ap => ap.AgreementID == _testAgreementID && ap.PersonID == person1.PersonID)
            .FirstAsync();

        var request = new AgreementContactUpsertRequest
        {
            PersonID = person2.PersonID,
            AgreementPersonRoleID = roleID,
        };

        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.UpdateContact(_testAgreementID, contacts.AgreementPersonID, request));
        var result = await AssemblySteps.AdminHttpClient.PutAsJsonAsync(route, request);

        Assert.IsTrue(result.IsSuccessStatusCode, $"Route: {route}\n{await result.Content.ReadAsStringAsync()}");
        var updated = await result.DeserializeContentAsync<AgreementContactGridRow>();
        Assert.IsNotNull(updated);
        Assert.AreEqual(person2.PersonID, updated.Person.PersonID);
    }

    [TestMethod]
    public async Task DeleteContact_Returns204_WhenExists()
    {
        var person = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        _createdPersonIDs.Add(person.PersonID);
        var roleID = AgreementPersonRole.All.First().AgreementPersonRoleID;

        await AgreementHelper.AddContactAsync(AssemblySteps.DbContext, _testAgreementID, person.PersonID, roleID);

        var contact = await AssemblySteps.DbContext.AgreementPeople.AsNoTracking()
            .Where(ap => ap.AgreementID == _testAgreementID && ap.PersonID == person.PersonID)
            .FirstAsync();

        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.DeleteContact(_testAgreementID, contact.AgreementPersonID));
        var result = await AssemblySteps.AdminHttpClient.DeleteAsync(route);

        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
    }

    #endregion

    #region Authorization Tests

    [TestMethod]
    public async Task List_Returns200_WhenUnauthenticated_BecauseAllowAnonymous()
    {
        // AgreementController.List() has [AllowAnonymous]
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.List());
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"AllowAnonymous endpoint should succeed unauthenticated.\nRoute: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    [TestMethod]
    public async Task Get_Returns200_WhenUnauthenticated_BecauseAllowAnonymous()
    {
        var route = RouteHelper.GetRouteFor<AgreementController>(c => c.Get(_testAgreementID));
        var result = await AssemblySteps.UnauthenticatedHttpClient.GetAsync(route);

        Assert.IsTrue(result.IsSuccessStatusCode,
            $"AllowAnonymous endpoint should succeed unauthenticated.\nRoute: {route}\n{await result.Content.ReadAsStringAsync()}");
    }

    #endregion
}
