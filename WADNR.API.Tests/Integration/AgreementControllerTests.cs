using Microsoft.EntityFrameworkCore;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.Agreement;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Integration tests for AgreementController endpoints.
/// </summary>
[TestClass]
[DoNotParallelize]
public class AgreementControllerTests
{
    private int _testAgreementID;
    private int _testOrganizationID;

    [TestInitialize]
    public async Task TestInitialize()
    {
        // Clear any tracked entities from previous tests
        AssemblySteps.DbContext.ChangeTracker.Clear();

        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        // Create test organization first
        var org = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _testOrganizationID = org.OrganizationID;

        // Create test agreement
        var agreement = await AgreementHelper.CreateAgreementAsync(
            AssemblySteps.DbContext,
            organizationID: _testOrganizationID);
        _testAgreementID = agreement.AgreementID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await AgreementHelper.DeleteAgreementAsync(AssemblySteps.DbContext, _testAgreementID);
        }
        catch { /* Ignore cleanup errors */ }

        try
        {
            await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, _testOrganizationID);
        }
        catch { /* Ignore cleanup errors */ }
    }

    #region List Tests

    [TestMethod]
    public async Task List_ReturnsOk()
    {
        // Arrange - agreement created in TestInitialize

        // Act
        var agreements = await Agreements.ListAsGridRowAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(agreements);
        Assert.IsTrue(agreements.Any(a => a.AgreementID == _testAgreementID),
            "List should include the test agreement");
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_ReturnsAgreement_WhenExists()
    {
        // Act
        var agreement = await Agreements.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testAgreementID);

        // Assert
        Assert.IsNotNull(agreement);
        Assert.AreEqual(_testAgreementID, agreement.AgreementID);
    }

    [TestMethod]
    public async Task Get_ReturnsNull_WhenNotExists()
    {
        // Act
        var agreement = await Agreements.GetByIDAsDetailAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(agreement);
    }

    #endregion

    #region Create Tests

    [TestMethod]
    public async Task Create_CreatesAgreement_WhenValid()
    {
        // Arrange
        var agreementType = await AssemblySteps.DbContext.AgreementTypes.FirstAsync();
        var agreementStatus = await AssemblySteps.DbContext.AgreementStatuses.FirstAsync();

        var request = new AgreementUpsertRequest
        {
            AgreementTitle = $"Test Create Agreement {DateTime.UtcNow.Ticks}",
            AgreementNumber = $"CREATE-{DateTime.UtcNow.Ticks % 100000}",
            AgreementTypeID = agreementType.AgreementTypeID,
            AgreementStatusID = agreementStatus.AgreementStatusID,
            OrganizationID = _testOrganizationID,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
        };

        int createdID = 0;
        try
        {
            // Act
            var created = await Agreements.CreateAsync(
                AssemblySteps.DbContext, request, AssemblySteps.TestAdminPersonID);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual(request.AgreementTitle, created.AgreementTitle);
            createdID = created.AgreementID;

            var retrieved = await Agreements.GetByIDAsDetailAsync(AssemblySteps.DbContext, createdID);
            Assert.IsNotNull(retrieved);
        }
        finally
        {
            // Cleanup
            if (createdID > 0)
            {
                await AgreementHelper.DeleteAgreementAsync(AssemblySteps.DbContext, createdID);
            }
        }
    }

    #endregion

    #region Update Tests

    [TestMethod]
    public async Task Update_UpdatesAgreement_WhenValid()
    {
        // Arrange
        var original = await Agreements.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testAgreementID);
        Assert.IsNotNull(original);

        var newTitle = $"Updated Agreement {DateTime.UtcNow.Ticks}";
        var request = new AgreementUpsertRequest
        {
            AgreementTitle = newTitle,
            AgreementNumber = original.AgreementNumber,
            AgreementTypeID = original.AgreementType.AgreementTypeID,
            AgreementStatusID = original.AgreementStatus?.AgreementStatusID,
            OrganizationID = original.ContributingOrganization.OrganizationID,
            StartDate = original.StartDate,
            EndDate = original.EndDate,
        };

        // Act
        var updated = await Agreements.UpdateAsync(
            AssemblySteps.DbContext, _testAgreementID, request, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsNotNull(updated);
        Assert.AreEqual(newTitle, updated.AgreementTitle);
    }

    [TestMethod]
    public async Task Update_ThrowsException_WhenNotExists()
    {
        // Arrange
        var request = new AgreementUpsertRequest
        {
            AgreementTitle = "Test",
            AgreementTypeID = 1,
            OrganizationID = _testOrganizationID,
        };

        // Act & Assert - UpdateAsync uses FirstAsync which throws if not found
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            await Agreements.UpdateAsync(
                AssemblySteps.DbContext, 999999, request, AssemblySteps.TestAdminPersonID));
    }

    #endregion

    #region Delete Tests

    [TestMethod]
    public async Task Delete_DeletesAgreement_WhenExists()
    {
        // Arrange - Create a new agreement specifically for deletion
        var toDelete = await AgreementHelper.CreateAgreementAsync(
            AssemblySteps.DbContext, organizationID: _testOrganizationID);
        var deleteID = toDelete.AgreementID;

        // Act
        var deleted = await Agreements.DeleteAsync(AssemblySteps.DbContext, deleteID);

        // Assert
        Assert.IsTrue(deleted);
        var retrieved = await AgreementHelper.GetByIDAsync(AssemblySteps.DbContext, deleteID);
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public async Task Delete_ReturnsFalse_WhenNotExists()
    {
        // Act
        var deleted = await Agreements.DeleteAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsFalse(deleted);
    }

    #endregion

    #region Fund Source Allocations Tests

    [TestMethod]
    public async Task ListFundSourceAllocations_ReturnsEmptyList_WhenNoAllocations()
    {
        // Act
        var allocations = await Agreements.ListFundSourceAllocationsAsLookupItemByAgreementIDAsync(
            AssemblySteps.DbContext, _testAgreementID);

        // Assert
        Assert.IsNotNull(allocations);
        Assert.AreEqual(0, allocations.Count);
    }

    #endregion

    #region Projects Tests

    [TestMethod]
    public async Task ListProjects_ReturnsEmptyList_WhenNoProjects()
    {
        // Act
        var projects = await Agreements.ListProjectsAsLookupItemByAgreementIDAsync(
            AssemblySteps.DbContext, _testAgreementID);

        // Assert
        Assert.IsNotNull(projects);
        Assert.AreEqual(0, projects.Count);
    }

    [TestMethod]
    public async Task ListProjects_ReturnsProjects_WhenProjectsExist()
    {
        // Arrange - Create a project and link it to the agreement
        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);

        try
        {
            await AgreementHelper.AddProjectAsync(AssemblySteps.DbContext, _testAgreementID, project.ProjectID);

            // Act
            var projects = await Agreements.ListProjectsAsLookupItemByAgreementIDAsync(
                AssemblySteps.DbContext, _testAgreementID);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == project.ProjectID));
        }
        finally
        {
            await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, project.ProjectID);
        }
    }

    #endregion

    #region Contacts Tests

    [TestMethod]
    public async Task ListContacts_ReturnsEmptyList_WhenNoContacts()
    {
        // Act
        var contacts = await Agreements.ListContactsAsGridRowByAgreementIDAsync(
            AssemblySteps.DbContext, _testAgreementID);

        // Assert
        Assert.IsNotNull(contacts);
        Assert.AreEqual(0, contacts.Count);
    }

    [TestMethod]
    public async Task CreateContact_CreatesContact_WhenValid()
    {
        // Arrange
        var person = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        var agreementPersonRoleID = AgreementPersonRole.All.First().AgreementPersonRoleID;

        try
        {
            var request = new AgreementContactUpsertRequest
            {
                PersonID = person.PersonID,
                AgreementPersonRoleID = agreementPersonRoleID,
            };

            // Act
            var created = await Agreements.CreateContactAsync(
                AssemblySteps.DbContext, _testAgreementID, request);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual(person.PersonID, created.Person.PersonID);

            // Verify it's in the list
            var contacts = await Agreements.ListContactsAsGridRowByAgreementIDAsync(
                AssemblySteps.DbContext, _testAgreementID);
            Assert.IsTrue(contacts.Any(c => c.Person.PersonID == person.PersonID));
        }
        finally
        {
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, person.PersonID);
        }
    }

    [TestMethod]
    public async Task DeleteContact_DeletesContact_WhenExists()
    {
        // Arrange
        var person = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        var agreementPersonRoleID = AgreementPersonRole.All.First().AgreementPersonRoleID;

        try
        {
            await AgreementHelper.AddContactAsync(
                AssemblySteps.DbContext, _testAgreementID, person.PersonID, agreementPersonRoleID);

            var contactsBefore = await Agreements.ListContactsAsGridRowByAgreementIDAsync(
                AssemblySteps.DbContext, _testAgreementID);
            var contactToDelete = contactsBefore.First(c => c.Person.PersonID == person.PersonID);

            // Act
            var deleted = await Agreements.DeleteContactAsync(
                AssemblySteps.DbContext, contactToDelete.AgreementPersonID);

            // Assert
            Assert.IsTrue(deleted);
            var contactsAfter = await Agreements.ListContactsAsGridRowByAgreementIDAsync(
                AssemblySteps.DbContext, _testAgreementID);
            Assert.IsFalse(contactsAfter.Any(c => c.Person.PersonID == person.PersonID));
        }
        finally
        {
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, person.PersonID);
        }
    }

    #endregion

    #region API Export Tests

    [TestMethod]
    public async Task ListAsApiJson_ReturnsAgreements()
    {
        // Act
        var agreements = await Agreements.ListAsApiJsonAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(agreements);
        Assert.IsTrue(agreements.Any(a => a.AgreementID == _testAgreementID));
    }

    #endregion

    #region List For Person Tests

    [TestMethod]
    public async Task ListForPerson_ReturnsAgreements_WhenPersonHasAgreements()
    {
        // Arrange - Add a contact to the test agreement
        var person = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        var agreementPersonRoleID = AgreementPersonRole.All.First().AgreementPersonRoleID;

        try
        {
            await AgreementHelper.AddContactAsync(
                AssemblySteps.DbContext, _testAgreementID, person.PersonID, agreementPersonRoleID);

            // Act
            var agreements = await Agreements.ListForPersonAsGridRowAsync(
                AssemblySteps.DbContext, person.PersonID);

            // Assert
            Assert.IsNotNull(agreements);
            Assert.IsTrue(agreements.Any(a => a.AgreementID == _testAgreementID),
                "ListForPerson should return agreements where person is a contact");
        }
        finally
        {
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, person.PersonID);
        }
    }

    #endregion

    #region Excel Export Tests

    [TestMethod]
    public async Task ListAsExcelRow_ReturnsExcelData()
    {
        // Act
        var excelRows = await Agreements.ListAsExcelRowAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(excelRows);
        // Just verify the method returns data (no AgreementID in excel row)
        Assert.IsTrue(excelRows.Count > 0);
    }

    [TestMethod]
    public async Task ListAsExcelRowByOrganizationID_ReturnsExcelData()
    {
        // Act
        var excelRows = await Agreements.ListAsExcelRowByOrganizationIDAsync(
            AssemblySteps.DbContext, _testOrganizationID);

        // Assert
        Assert.IsNotNull(excelRows);
        // Just verify the method returns data for this organization
        Assert.IsTrue(excelRows.Count > 0);
    }

    [TestMethod]
    public async Task ListForPersonAsExcelRow_ReturnsEmptyList_WhenNoAgreements()
    {
        // Arrange - Create a person without any agreement associations
        var person = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);

        try
        {
            // Act
            var excelRows = await Agreements.ListForPersonAsExcelRowAsync(
                AssemblySteps.DbContext, person.PersonID);

            // Assert
            Assert.IsNotNull(excelRows);
            Assert.AreEqual(0, excelRows.Count);
        }
        finally
        {
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, person.PersonID);
        }
    }

    #endregion

    #region Update Contact Tests

    [TestMethod]
    public async Task UpdateContact_UpdatesContact_WhenValid()
    {
        // Arrange - Create a contact first
        var person1 = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        var person2 = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        var agreementPersonRoleID = AgreementPersonRole.All.First().AgreementPersonRoleID;

        try
        {
            // Create the contact
            var createRequest = new AgreementContactUpsertRequest
            {
                PersonID = person1.PersonID,
                AgreementPersonRoleID = agreementPersonRoleID,
            };
            var created = await Agreements.CreateContactAsync(
                AssemblySteps.DbContext, _testAgreementID, createRequest);

            // Act - Update the contact to use person2
            var updateRequest = new AgreementContactUpsertRequest
            {
                PersonID = person2.PersonID,
                AgreementPersonRoleID = agreementPersonRoleID,
            };
            var updated = await Agreements.UpdateContactAsync(
                AssemblySteps.DbContext, created.AgreementPersonID, updateRequest);

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual(person2.PersonID, updated.Person.PersonID);
        }
        finally
        {
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, person1.PersonID);
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, person2.PersonID);
        }
    }

    [TestMethod]
    public async Task UpdateContact_ThrowsException_WhenNotExists()
    {
        // Arrange
        var request = new AgreementContactUpsertRequest
        {
            PersonID = 1,
            AgreementPersonRoleID = 1,
        };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            await Agreements.UpdateContactAsync(
                AssemblySteps.DbContext, 999999, request));
    }

    #endregion

    #region Update Fund Source Allocations Tests

    [TestMethod]
    public async Task UpdateFundSourceAllocations_UpdatesAllocations_WhenValid()
    {
        // Arrange - Get a valid fund source allocation
        var fundSourceAllocation = await AssemblySteps.DbContext.FundSourceAllocations.FirstOrDefaultAsync();

        if (fundSourceAllocation == null)
        {
            // Skip if no fund source allocations exist
            Assert.Inconclusive("No fund source allocations exist in the database");
            return;
        }

        var request = new AgreementFundSourceAllocationsUpdateRequest
        {
            FundSourceAllocationIDs = new List<int> { fundSourceAllocation.FundSourceAllocationID }
        };

        try
        {
            // Act
            var allocations = await Agreements.UpdateFundSourceAllocationsAsync(
                AssemblySteps.DbContext, _testAgreementID, request);

            // Assert
            Assert.IsNotNull(allocations);
            Assert.IsTrue(allocations.Any(a => a.FundSourceAllocationID == fundSourceAllocation.FundSourceAllocationID));
        }
        finally
        {
            // Cleanup - remove all allocations
            await Agreements.UpdateFundSourceAllocationsAsync(
                AssemblySteps.DbContext, _testAgreementID,
                new AgreementFundSourceAllocationsUpdateRequest { FundSourceAllocationIDs = new List<int>() });
        }
    }

    #endregion

    #region Update Projects Tests

    [TestMethod]
    public async Task UpdateProjects_UpdatesProjects_WhenValid()
    {
        // Arrange - Create a project
        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);

        try
        {
            var request = new AgreementProjectsUpdateRequest
            {
                ProjectIDs = new List<int> { project.ProjectID }
            };

            // Act
            var projects = await Agreements.UpdateProjectsAsync(
                AssemblySteps.DbContext, _testAgreementID, request);

            // Assert
            Assert.IsNotNull(projects);
            Assert.IsTrue(projects.Any(p => p.ProjectID == project.ProjectID));

            // Cleanup projects from agreement
            await Agreements.UpdateProjectsAsync(
                AssemblySteps.DbContext, _testAgreementID,
                new AgreementProjectsUpdateRequest { ProjectIDs = new List<int>() });
        }
        finally
        {
            await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, project.ProjectID);
        }
    }

    #endregion
}
