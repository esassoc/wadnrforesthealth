using Microsoft.EntityFrameworkCore;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Integration tests for ProgramController endpoints.
/// </summary>
[TestClass]
[DoNotParallelize]
public class ProgramControllerTests
{
    private int _testProgramID;
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

        // Create test program
        var program = await ProgramHelper.CreateProgramAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID, organizationID: _testOrganizationID);
        _testProgramID = program.ProgramID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await ProgramHelper.DeleteProgramAsync(AssemblySteps.DbContext, _testProgramID);
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
    public async Task List_ReturnsPrograms()
    {
        // Act
        var programs = await Programs.ListAsGridRowAsync(AssemblySteps.DbContext);

        // Assert
        Assert.IsNotNull(programs);
        Assert.IsTrue(programs.Any(p => p.ProgramID == _testProgramID),
            "List should include the test program");
    }

    #endregion

    #region Get Tests

    [TestMethod]
    public async Task Get_ReturnsProgram_WhenExists()
    {
        // Act
        var program = await Programs.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testProgramID);

        // Assert
        Assert.IsNotNull(program);
        Assert.AreEqual(_testProgramID, program.ProgramID);
    }

    [TestMethod]
    public async Task Get_ReturnsNull_WhenNotExists()
    {
        // Act
        var program = await Programs.GetByIDAsDetailAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(program);
    }

    #endregion

    #region Create Tests

    [TestMethod]
    public async Task Create_CreatesProgram_WhenValid()
    {
        // Arrange
        var uniqueSuffix = DateTime.UtcNow.Ticks % 1000000;
        var request = new ProgramUpsertRequest
        {
            ProgramName = $"Test Create Program {uniqueSuffix}",
            ProgramShortName = $"TCP{uniqueSuffix}",
            OrganizationID = _testOrganizationID,
            ProgramIsActive = true,
        };

        int createdID = 0;
        try
        {
            // Act
            var created = await Programs.CreateAsync(
                AssemblySteps.DbContext, request, AssemblySteps.TestAdminPersonID);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual(request.ProgramName, created.ProgramName);
            createdID = created.ProgramID;
        }
        finally
        {
            if (createdID > 0)
            {
                await ProgramHelper.DeleteProgramAsync(AssemblySteps.DbContext, createdID);
            }
        }
    }

    #endregion

    #region Update Tests

    [TestMethod]
    public async Task Update_UpdatesProgram_WhenValid()
    {
        // Arrange
        var original = await Programs.GetByIDAsDetailAsync(AssemblySteps.DbContext, _testProgramID);
        Assert.IsNotNull(original);

        var newName = $"Updated Program {DateTime.UtcNow.Ticks}";
        var request = new ProgramUpsertRequest
        {
            ProgramName = newName,
            ProgramShortName = original.ProgramShortName,
            OrganizationID = original.OrganizationID,
            ProgramIsActive = original.ProgramIsActive,
        };

        // Act
        var updated = await Programs.UpdateAsync(
            AssemblySteps.DbContext, _testProgramID, request, AssemblySteps.TestAdminPersonID);

        // Assert
        Assert.IsNotNull(updated);
        Assert.AreEqual(newName, updated.ProgramName);
    }

    [TestMethod]
    public async Task Update_ThrowsException_WhenNotExists()
    {
        // Arrange
        var request = new ProgramUpsertRequest
        {
            ProgramName = "Test",
            ProgramShortName = "TST",
            OrganizationID = _testOrganizationID,
            ProgramIsActive = true,
        };

        // Act & Assert - UpdateAsync uses FirstAsync which throws if not found
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            await Programs.UpdateAsync(
                AssemblySteps.DbContext, 999999, request, AssemblySteps.TestAdminPersonID));
    }

    #endregion

    #region Delete Tests

    [TestMethod]
    public async Task Delete_DeletesProgram_WhenExists()
    {
        // Arrange - Create a new program specifically for deletion
        var toDelete = await ProgramHelper.CreateProgramAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID, organizationID: _testOrganizationID);
        var deleteID = toDelete.ProgramID;

        // Act
        var deleted = await Programs.DeleteAsync(AssemblySteps.DbContext, deleteID);

        // Assert
        Assert.IsTrue(deleted);
        var retrieved = await ProgramHelper.GetByIDAsync(AssemblySteps.DbContext, deleteID);
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public async Task Delete_ReturnsFalse_WhenNotExists()
    {
        // Act
        var deleted = await Programs.DeleteAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsFalse(deleted);
    }

    [TestMethod]
    public async Task Delete_DeletesProgram_WithProjectAssociations()
    {
        // Arrange - Create a program and a project, then link them
        var programToDelete = await ProgramHelper.CreateProgramAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID, organizationID: _testOrganizationID);
        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);

        try
        {
            AssemblySteps.DbContext.ProjectPrograms.Add(new ProjectProgram
            {
                ProjectID = project.ProjectID,
                ProgramID = programToDelete.ProgramID
            });
            await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

            // Act
            var deleted = await Programs.DeleteAsync(AssemblySteps.DbContext, programToDelete.ProgramID);

            // Assert - Program is deleted
            Assert.IsTrue(deleted);
            var retrievedProgram = await ProgramHelper.GetByIDAsync(AssemblySteps.DbContext, programToDelete.ProgramID);
            Assert.IsNull(retrievedProgram);

            // Assert - Junction record is removed
            var junctionExists = await AssemblySteps.DbContext.ProjectPrograms
                .AnyAsync(pp => pp.ProgramID == programToDelete.ProgramID);
            Assert.IsFalse(junctionExists);

            // Assert - Project still exists
            var retrievedProject = await AssemblySteps.DbContext.Projects
                .AnyAsync(p => p.ProjectID == project.ProjectID);
            Assert.IsTrue(retrievedProject, "Project should still exist after program deletion");
        }
        finally
        {
            await ProjectHelper.DeleteProjectAsync(AssemblySteps.DbContext, project.ProjectID);
        }
    }

    #endregion

    #region Delete Info Tests

    [TestMethod]
    public async Task GetDeleteInfo_ReturnsInfo_WhenProgramExists()
    {
        // Act
        var info = await Programs.GetDeleteInfoAsync(AssemblySteps.DbContext, _testProgramID);

        // Assert
        Assert.IsNotNull(info);
        Assert.AreEqual(0, info.ProjectCount);
        Assert.AreEqual(0, info.TreatmentCount);
        Assert.AreEqual(0, info.TreatmentUpdateCount);
        Assert.AreEqual(0, info.ProjectLocationCount);
    }

    [TestMethod]
    public async Task GetDeleteInfo_ReturnsNull_WhenProgramNotExists()
    {
        // Act
        var info = await Programs.GetDeleteInfoAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsNull(info);
    }

    #endregion

    #region Projects Tests

    [TestMethod]
    public async Task ListProjects_ReturnsEmptyList_WhenNoProjects()
    {
        // Act
        var projects = await Programs.ListProjectsForProgramAsync(
            AssemblySteps.DbContext, _testProgramID);

        // Assert
        Assert.IsNotNull(projects);
        Assert.AreEqual(0, projects.Count);
    }

    [TestMethod]
    public async Task ListProjects_ReturnsProjects_WhenProjectsExist()
    {
        // Arrange - Create a project and link it to the program
        var project = await ProjectHelper.CreateApprovedProjectWithValidLookupsAsync(
            AssemblySteps.DbContext, AssemblySteps.TestAdminPersonID);

        try
        {
            // Add project to program
            AssemblySteps.DbContext.ProjectPrograms.Add(new ProjectProgram
            {
                ProjectID = project.ProjectID,
                ProgramID = _testProgramID
            });
            await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

            // Act
            var projects = await Programs.ListProjectsForProgramAsync(
                AssemblySteps.DbContext, _testProgramID);

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

    #region Notifications Tests

    [TestMethod]
    public async Task ListNotifications_ReturnsEmptyList_WhenNoNotifications()
    {
        // Act
        var notifications = await Programs.ListNotificationsForProgramAsync(
            AssemblySteps.DbContext, _testProgramID);

        // Assert
        Assert.IsNotNull(notifications);
        Assert.AreEqual(0, notifications.Count);
    }

    #endregion

    #region Block List Tests

    [TestMethod]
    public async Task ListBlockListEntries_ReturnsEmptyList_WhenNoEntries()
    {
        // Act
        var entries = await Programs.ListBlockListEntriesAsync(
            AssemblySteps.DbContext, _testProgramID);

        // Assert
        Assert.IsNotNull(entries);
        Assert.AreEqual(0, entries.Count);
    }

    [TestMethod]
    public async Task AddToBlockList_AddsEntry_WhenValid()
    {
        // Arrange
        var request = new AddToBlockListRequest
        {
            ProjectGisIdentifier = $"GIS-{DateTime.UtcNow.Ticks}",
            ProjectName = "Blocked Test Project",
            Notes = "Test block list entry"
        };

        try
        {
            // Act
            await Programs.AddToBlockListAsync(AssemblySteps.DbContext, _testProgramID, request);

            // Assert
            var entries = await Programs.ListBlockListEntriesAsync(
                AssemblySteps.DbContext, _testProgramID);
            Assert.IsTrue(entries.Any(e => e.ProjectGisIdentifier == request.ProjectGisIdentifier));
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectImportBlockList WHERE ProgramID = {_testProgramID}");
        }
    }

    [TestMethod]
    public async Task DeleteBlockListEntry_DeletesEntry_WhenExists()
    {
        // Arrange - Add an entry first
        var request = new AddToBlockListRequest
        {
            ProjectGisIdentifier = $"GIS-DEL-{DateTime.UtcNow.Ticks}",
            ProjectName = "Block List Delete Test",
            Notes = "To be deleted"
        };
        await Programs.AddToBlockListAsync(AssemblySteps.DbContext, _testProgramID, request);

        var entries = await Programs.ListBlockListEntriesAsync(
            AssemblySteps.DbContext, _testProgramID);
        var entryID = entries.First(e => e.ProjectGisIdentifier == request.ProjectGisIdentifier).ProjectImportBlockListID;

        // Act
        var deleted = await Programs.DeleteBlockListEntryAsync(AssemblySteps.DbContext, entryID);

        // Assert
        Assert.IsTrue(deleted);
        var entriesAfter = await Programs.ListBlockListEntriesAsync(
            AssemblySteps.DbContext, _testProgramID);
        Assert.IsFalse(entriesAfter.Any(e => e.ProjectImportBlockListID == entryID));
    }

    [TestMethod]
    public async Task DeleteBlockListEntry_ReturnsFalse_WhenNotExists()
    {
        // Act
        var deleted = await Programs.DeleteBlockListEntryAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsFalse(deleted);
    }

    #endregion

    #region Editor Tests

    [TestMethod]
    public async Task ListEligibleProgramEditors_ReturnsEditors()
    {
        // Act
        var editors = await Programs.ListEligibleProgramEditorsAsync(AssemblySteps.DbContext);

        // Assert - May be empty but should not fail
        Assert.IsNotNull(editors);
    }

    [TestMethod]
    public async Task ListEligibleProgramEditors_IncludesFullUser_WithCanEditProgramRole()
    {
        // Arrange
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        try
        {
            await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, user.PersonID, RoleEnum.CanEditProgram);

            // Act
            var editors = await Programs.ListEligibleProgramEditorsAsync(AssemblySteps.DbContext);

            // Assert
            Assert.IsTrue(editors.Any(e => e.PersonID == user.PersonID),
                "Full user with CanEditProgram role should be eligible");
        }
        finally
        {
            var personEntity = await AssemblySteps.DbContext.People
                .FirstOrDefaultAsync(p => p.PersonID == user.PersonID);
            if (personEntity != null)
            {
                personEntity.GlobalID = null;
                await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
            }
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, user.PersonID);
        }
    }

    [TestMethod]
    public async Task ListEligibleProgramEditors_ExcludesContact_WithCanEditProgramRole()
    {
        // Arrange - Create a contact (no GlobalID)
        var contact = await PersonHelper.CreateContactAsync(AssemblySteps.DbContext);
        try
        {
            await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, contact.PersonID, RoleEnum.CanEditProgram);

            // Act
            var editors = await Programs.ListEligibleProgramEditorsAsync(AssemblySteps.DbContext);

            // Assert
            Assert.IsFalse(editors.Any(e => e.PersonID == contact.PersonID),
                "Contact without GlobalID should not be eligible even with CanEditProgram role");
        }
        finally
        {
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, contact.PersonID);
        }
    }

    [TestMethod]
    public async Task ListEligibleProgramEditors_ExcludesInactiveUser_WithCanEditProgramRole()
    {
        // Arrange
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        try
        {
            await PersonHelper.AddSupplementalRoleAsync(AssemblySteps.DbContext, user.PersonID, RoleEnum.CanEditProgram);

            // Set inactive
            var personEntity = await AssemblySteps.DbContext.People
                .FirstAsync(p => p.PersonID == user.PersonID);
            personEntity.IsActive = false;
            await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

            // Act
            var editors = await Programs.ListEligibleProgramEditorsAsync(AssemblySteps.DbContext);

            // Assert
            Assert.IsFalse(editors.Any(e => e.PersonID == user.PersonID),
                "Inactive user should not be eligible");
        }
        finally
        {
            var personEntity = await AssemblySteps.DbContext.People
                .FirstOrDefaultAsync(p => p.PersonID == user.PersonID);
            if (personEntity != null)
            {
                personEntity.IsActive = true;
                personEntity.GlobalID = null;
                await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
            }
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, user.PersonID);
        }
    }

    [TestMethod]
    public async Task ListEligibleProgramEditors_ExcludesFullUser_WithoutCanEditProgramRole()
    {
        // Arrange - Create a full user with Normal role only (no CanEditProgram)
        var user = await PersonHelper.CreateUserAsync(AssemblySteps.DbContext, RoleEnum.Normal);
        try
        {
            // Act
            var editors = await Programs.ListEligibleProgramEditorsAsync(AssemblySteps.DbContext);

            // Assert
            Assert.IsFalse(editors.Any(e => e.PersonID == user.PersonID),
                "Full user without CanEditProgram role should not be eligible");
        }
        finally
        {
            var personEntity = await AssemblySteps.DbContext.People
                .FirstOrDefaultAsync(p => p.PersonID == user.PersonID);
            if (personEntity != null)
            {
                personEntity.GlobalID = null;
                await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
            }
            await PersonHelper.DeletePersonAsync(AssemblySteps.DbContext, user.PersonID);
        }
    }

    [TestMethod]
    public async Task UpdateEditors_AddsEditors_WhenValid()
    {
        // Arrange - Get a person with CanEditProgram role
        var eligibleEditors = await Programs.ListEligibleProgramEditorsAsync(AssemblySteps.DbContext);
        if (eligibleEditors.Count == 0)
        {
            Assert.Inconclusive("No eligible program editors found in database");
            return;
        }

        var request = new ProgramEditorsUpsertRequest
        {
            PersonIDList = new List<int> { eligibleEditors.First().PersonID }
        };

        try
        {
            // Act
            var result = await Programs.UpdateEditorsAsync(
                AssemblySteps.DbContext, _testProgramID, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProgramPerson WHERE ProgramID = {_testProgramID}");
        }
    }

    [TestMethod]
    public async Task UpdateEditors_RemovesEditors_WhenEmpty()
    {
        // Arrange - Add an editor first
        var eligibleEditors = await Programs.ListEligibleProgramEditorsAsync(AssemblySteps.DbContext);
        if (eligibleEditors.Count == 0)
        {
            Assert.Inconclusive("No eligible program editors found in database");
            return;
        }

        await ProgramHelper.AddEditorAsync(
            AssemblySteps.DbContext, _testProgramID, eligibleEditors.First().PersonID);

        // Act - Update with empty list
        var request = new ProgramEditorsUpsertRequest
        {
            PersonIDList = new List<int>()
        };
        var result = await Programs.UpdateEditorsAsync(
            AssemblySteps.DbContext, _testProgramID, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ValidateEditorsHaveRequiredRole_ReturnsError_WhenNoRole()
    {
        // Arrange - Get a person without CanEditProgram role
        var peopleWithoutRole = await AssemblySteps.DbContext.People
            .AsNoTracking()
            .Where(p => !p.PersonRoles.Any(pr => pr.RoleID == (int)RoleEnum.CanEditProgram))
            .Select(p => p.PersonID)
            .Take(1)
            .ToListAsync();

        if (peopleWithoutRole.Count == 0)
        {
            Assert.Inconclusive("All people have CanEditProgram role");
            return;
        }

        // Act
        var error = await Programs.ValidateEditorsHaveRequiredRoleAsync(
            AssemblySteps.DbContext, peopleWithoutRole);

        // Assert
        Assert.IsNotNull(error);
        Assert.IsTrue(error.Contains("do not have the Program Editor role"));
    }

    #endregion

    #region Notification Tests

    [TestMethod]
    public async Task CreateNotification_CreatesNotification_WhenValid()
    {
        // Arrange - Use first available lookup values from static dictionaries
        var notificationTypeID = ProgramNotificationType.AllLookupDictionary.Keys.First();
        var recurrenceIntervalID = RecurrenceInterval.AllLookupDictionary.Keys.First();

        var request = new ProgramNotificationUpsertRequest
        {
            ProgramNotificationTypeID = notificationTypeID,
            RecurrenceIntervalID = recurrenceIntervalID,
            NotificationEmailText = "Test notification email text"
        };

        try
        {
            // Act
            var result = await Programs.CreateNotificationAsync(
                AssemblySteps.DbContext, _testProgramID, request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(request.NotificationEmailText, result.NotificationEmailText);
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProgramNotificationConfiguration WHERE ProgramID = {_testProgramID}");
        }
    }

    [TestMethod]
    public async Task UpdateNotification_UpdatesNotification_WhenExists()
    {
        // Arrange - Create a notification first using static lookups
        var notificationTypeID = ProgramNotificationType.AllLookupDictionary.Keys.First();
        var recurrenceIntervalID = RecurrenceInterval.AllLookupDictionary.Keys.First();

        var createRequest = new ProgramNotificationUpsertRequest
        {
            ProgramNotificationTypeID = notificationTypeID,
            RecurrenceIntervalID = recurrenceIntervalID,
            NotificationEmailText = "Original text"
        };
        var created = await Programs.CreateNotificationAsync(
            AssemblySteps.DbContext, _testProgramID, createRequest);

        try
        {
            // Act
            var updateRequest = new ProgramNotificationUpsertRequest
            {
                ProgramNotificationTypeID = notificationTypeID,
                RecurrenceIntervalID = recurrenceIntervalID,
                NotificationEmailText = "Updated text"
            };
            var result = await Programs.UpdateNotificationAsync(
                AssemblySteps.DbContext, created.ProgramNotificationConfigurationID, updateRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated text", result.NotificationEmailText);
        }
        finally
        {
            // Cleanup
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProgramNotificationConfiguration WHERE ProgramID = {_testProgramID}");
        }
    }

    [TestMethod]
    public async Task UpdateNotification_ReturnsNull_WhenNotExists()
    {
        // Arrange - Use valid lookup values but non-existent ID
        var notificationTypeID = ProgramNotificationType.AllLookupDictionary.Keys.First();
        var recurrenceIntervalID = RecurrenceInterval.AllLookupDictionary.Keys.First();

        var request = new ProgramNotificationUpsertRequest
        {
            ProgramNotificationTypeID = notificationTypeID,
            RecurrenceIntervalID = recurrenceIntervalID,
            NotificationEmailText = "Test"
        };

        // Act
        var result = await Programs.UpdateNotificationAsync(
            AssemblySteps.DbContext, 999999, request);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task DeleteNotification_DeletesNotification_WhenExists()
    {
        // Arrange - Create a notification first using static lookups
        var notificationTypeID = ProgramNotificationType.AllLookupDictionary.Keys.First();
        var recurrenceIntervalID = RecurrenceInterval.AllLookupDictionary.Keys.First();

        var createRequest = new ProgramNotificationUpsertRequest
        {
            ProgramNotificationTypeID = notificationTypeID,
            RecurrenceIntervalID = recurrenceIntervalID,
            NotificationEmailText = "To be deleted"
        };
        var created = await Programs.CreateNotificationAsync(
            AssemblySteps.DbContext, _testProgramID, createRequest);

        // Act
        var deleted = await Programs.DeleteNotificationAsync(
            AssemblySteps.DbContext, created.ProgramNotificationConfigurationID);

        // Assert
        Assert.IsTrue(deleted);
    }

    [TestMethod]
    public async Task DeleteNotification_ReturnsFalse_WhenNotExists()
    {
        // Act
        var deleted = await Programs.DeleteNotificationAsync(AssemblySteps.DbContext, 999999);

        // Assert
        Assert.IsFalse(deleted);
    }

    #endregion

    #region List by Organization Tests

    [TestMethod]
    public async Task ListAsGridRowByOrganizationID_ReturnsPrograms()
    {
        // Act
        var programs = await Programs.ListAsGridRowByOrganizationIDAsync(
            AssemblySteps.DbContext, _testOrganizationID);

        // Assert
        Assert.IsNotNull(programs);
        Assert.IsTrue(programs.Any(p => p.ProgramID == _testProgramID));
    }

    #endregion

    #region Program Name Uniqueness Tests

    [TestMethod]
    public async Task ValidateUpsertAsync_ReturnsNull_WhenNameUnique()
    {
        // Arrange
        var existingProgram = await AssemblySteps.DbContext.Programs
            .AsNoTracking()
            .FirstAsync(p => p.ProgramID == _testProgramID);

        var dto = new ProgramUpsertRequest
        {
            OrganizationID = existingProgram.OrganizationID,
            ProgramName = $"Unique Name {DateTime.UtcNow.Ticks}",
            ProgramShortName = $"UN{DateTime.UtcNow.Ticks}",
            IsDefaultProgramForImportOnly = false,
            ProgramIsActive = true
        };

        // Act
        var error = await Programs.ValidateUpsertAsync(AssemblySteps.DbContext, dto);

        // Assert
        Assert.IsNull(error);
    }

    [TestMethod]
    public async Task ValidateUpsertAsync_ReturnsError_WhenNameExistsInSameOrg()
    {
        // Arrange - Get an existing program with a name
        var existingProgram = await AssemblySteps.DbContext.Programs
            .AsNoTracking()
            .Where(p => p.ProgramID != _testProgramID && p.ProgramName != null)
            .FirstOrDefaultAsync();

        if (existingProgram?.ProgramName == null)
        {
            Assert.Inconclusive("No other programs with names found");
            return;
        }

        var dto = new ProgramUpsertRequest
        {
            OrganizationID = existingProgram.OrganizationID,
            ProgramName = existingProgram.ProgramName,
            ProgramShortName = $"UN{DateTime.UtcNow.Ticks}",
            IsDefaultProgramForImportOnly = false,
            ProgramIsActive = true
        };

        // Act
        var error = await Programs.ValidateUpsertAsync(AssemblySteps.DbContext, dto);

        // Assert
        Assert.IsNotNull(error);
        Assert.IsTrue(error.Contains("already exists"));
    }

    [TestMethod]
    public async Task ValidateUpsertAsync_ReturnsNull_WhenUpdatingSameProgram()
    {
        // Arrange - Validate against itself should pass
        var existingProgram = await AssemblySteps.DbContext.Programs
            .AsNoTracking()
            .Where(p => p.ProgramName != null)
            .FirstOrDefaultAsync();

        if (existingProgram?.ProgramName == null)
        {
            Assert.Inconclusive("No programs with names found");
            return;
        }

        var dto = new ProgramUpsertRequest
        {
            OrganizationID = existingProgram.OrganizationID,
            ProgramName = existingProgram.ProgramName,
            ProgramShortName = existingProgram.ProgramShortName,
            IsDefaultProgramForImportOnly = false,
            ProgramIsActive = true
        };

        // Act
        var error = await Programs.ValidateUpsertAsync(AssemblySteps.DbContext, dto, existingProgram.ProgramID);

        // Assert
        Assert.IsNull(error);
    }

    #endregion
}
