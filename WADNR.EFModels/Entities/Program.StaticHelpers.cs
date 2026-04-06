using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Programs
{
    public static async Task<List<ProgramGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        var entities = await dbContext.Programs
            .AsNoTracking()
            .Select(ProgramProjections.AsGridRow)
            .OrderBy(x => x.ProgramName)
            .ToListAsync();
        return entities;
    }

    public static async Task<ProgramDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int programID)
    {
        var entity = await dbContext.Programs
            .AsNoTracking()
            .Where(x => x.ProgramID == programID)
            .Select(ProgramProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (entity == null) return null;

        // Populate lookup values for GDB Import Basics
        if (entity.GdbImportBasics != null)
        {
            if (ProjectStage.AllLookupDictionary.TryGetValue(entity.GdbImportBasics.ProjectStageDefaultID, out var projectStage))
            {
                entity.GdbImportBasics.ProjectStageDefaultName = projectStage.ProjectStageName;
            }

            // Fetch GDB Default Mappings
            entity.GdbDefaultMappings = await ListGdbDefaultMappingsForProgramAsync(dbContext, entity.GdbImportBasics.GisUploadSourceOrganizationID);

            // Fetch GDB Crosswalk Values
            entity.GdbCrosswalkValues = await ListGdbCrosswalkValuesForProgramAsync(dbContext, entity.GdbImportBasics.GisUploadSourceOrganizationID);
        }

        return entity;
    }

    public static async Task<List<GdbDefaultMappingItem>> ListGdbDefaultMappingsForProgramAsync(WADNRDbContext dbContext, int gisUploadSourceOrganizationID)
    {
        var rawMappings = await dbContext.GisDefaultMappings
            .AsNoTracking()
            .Where(m => m.GisUploadSourceOrganizationID == gisUploadSourceOrganizationID)
            .Select(m => new
            {
                m.GisDefaultMappingID,
                m.FieldDefinitionID,
                m.GisDefaultMappingColumnName
            })
            .ToListAsync();

        var mappings = rawMappings
            .Select(m => new GdbDefaultMappingItem
            {
                GisDefaultMappingID = m.GisDefaultMappingID,
                FieldDefinitionID = m.FieldDefinitionID,
                FieldDefinitionDisplayName = FieldDefinition.AllLookupDictionary.TryGetValue(m.FieldDefinitionID, out var fd)
                    ? fd.FieldDefinitionDisplayName
                    : $"Unknown ({m.FieldDefinitionID})",
                GisDefaultMappingColumnName = m.GisDefaultMappingColumnName
            })
            .OrderBy(m => m.FieldDefinitionDisplayName)
            .ToList();

        return mappings;
    }

    public static async Task<List<GdbCrosswalkItem>> ListGdbCrosswalkValuesForProgramAsync(WADNRDbContext dbContext, int gisUploadSourceOrganizationID)
    {
        var rawCrosswalks = await dbContext.GisCrossWalkDefaults
            .AsNoTracking()
            .Where(c => c.GisUploadSourceOrganizationID == gisUploadSourceOrganizationID)
            .Select(c => new
            {
                c.GisCrossWalkDefaultID,
                c.FieldDefinitionID,
                c.GisCrossWalkSourceValue,
                c.GisCrossWalkMappedValue
            })
            .ToListAsync();

        var crosswalks = rawCrosswalks
            .Select(c => new GdbCrosswalkItem
            {
                GisCrossWalkDefaultID = c.GisCrossWalkDefaultID,
                FieldDefinitionID = c.FieldDefinitionID,
                FieldDefinitionDisplayName = FieldDefinition.AllLookupDictionary.TryGetValue(c.FieldDefinitionID, out var fd)
                    ? fd.FieldDefinitionDisplayName
                    : $"Unknown ({c.FieldDefinitionID})",
                GisCrossWalkSourceValue = c.GisCrossWalkSourceValue,
                GisCrossWalkMappedValue = c.GisCrossWalkMappedValue
            })
            .OrderBy(c => c.FieldDefinitionDisplayName)
            .ThenBy(c => c.GisCrossWalkSourceValue)
            .ToList();

        return crosswalks;
    }

    public static async Task<string?> ValidateUpsertAsync(WADNRDbContext dbContext, ProgramUpsertRequest dto, int? currentProgramID = null)
    {
        if (dto.IsDefaultProgramForImportOnly) return null;

        var duplicateName = await dbContext.Programs
            .AnyAsync(p => p.OrganizationID == dto.OrganizationID
                && p.ProgramName == dto.ProgramName
                && (currentProgramID == null || p.ProgramID != currentProgramID));
        if (duplicateName)
            return $"A program with the name '{dto.ProgramName}' already exists for this organization.";

        var duplicateShortName = await dbContext.Programs
            .AnyAsync(p => p.OrganizationID == dto.OrganizationID
                && p.ProgramShortName == dto.ProgramShortName
                && (currentProgramID == null || p.ProgramID != currentProgramID));
        if (duplicateShortName)
            return $"A program with the short name '{dto.ProgramShortName}' already exists for this organization.";

        return null;
    }

    public static async Task<ProgramDetail?> CreateAsync(WADNRDbContext dbContext, ProgramUpsertRequest dto, int callingPersonID)
    {
        var entity = new Program
        {
            OrganizationID = dto.OrganizationID,
            ProgramName = dto.IsDefaultProgramForImportOnly ? null : dto.ProgramName,
            ProgramShortName = dto.IsDefaultProgramForImportOnly ? null : dto.ProgramShortName,
            ProgramPrimaryContactPersonID = dto.ProgramPrimaryContactPersonID,
            ProgramIsActive = dto.ProgramIsActive,
            IsDefaultProgramForImportOnly = dto.IsDefaultProgramForImportOnly,
            ProgramNotes = dto.ProgramNotes,
            ProgramFileResourceID = dto.ProgramFileResourceID,
            ProgramExampleGeospatialUploadFileResourceID = dto.ProgramExampleGeospatialUploadFileResourceID,
            ProgramCreateDate = DateTime.UtcNow,
            ProgramCreatePersonID = callingPersonID
        };
        dbContext.Programs.Add(entity);
        await dbContext.SaveChangesAsync();

        // Create GisUploadSourceOrganization with sensible defaults (matches legacy ProgramController.New behavior)
        var leadImplementerRelationshipType = await dbContext.RelationshipTypes
            .FirstAsync(rt => rt.IsPrimaryContact);

        var gisUploadSourceOrganization = new GisUploadSourceOrganization
        {
            GisUploadSourceOrganizationName = dto.ProgramName ?? entity.Organization?.OrganizationName ?? "Default",
            AdjustProjectTypeBasedOnTreatmentTypes = false,
            ProjectStageDefaultID = (int)ProjectStageEnum.Implementation,
            DataDeriveProjectStage = false,
            DefaultLeadImplementerOrganizationID = dto.OrganizationID,
            RelationshipTypeForDefaultOrganizationID = leadImplementerRelationshipType.RelationshipTypeID,
            ImportAsDetailedLocationInsteadOfTreatments = false,
            ApplyCompletedDateToProject = true,
            ApplyStartDateToProject = true,
            ProgramID = entity.ProgramID,
            ImportAsDetailedLocationInAdditionToTreatments = false,
            ApplyStartDateToTreatments = false,
            ApplyEndDateToTreatments = false,
            ImportIsFlattened = false,
        };
        dbContext.GisUploadSourceOrganizations.Add(gisUploadSourceOrganization);
        await dbContext.SaveChangesAsync();

        return await GetByIDAsDetailAsync(dbContext, entity.ProgramID);
    }

    public static async Task<ProgramDetail?> UpdateAsync(WADNRDbContext dbContext, int programID, ProgramUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.Programs
            .Include(x => x.Organization)
            .FirstAsync(x => x.ProgramID == programID);

        // map fields from dto to entity
        entity.OrganizationID = dto.OrganizationID;
        entity.ProgramName = dto.IsDefaultProgramForImportOnly ? null : dto.ProgramName;
        entity.ProgramShortName = dto.IsDefaultProgramForImportOnly ? null : dto.ProgramShortName;
        entity.ProgramPrimaryContactPersonID = dto.ProgramPrimaryContactPersonID;
        entity.ProgramIsActive = dto.ProgramIsActive;
        entity.IsDefaultProgramForImportOnly = dto.IsDefaultProgramForImportOnly;
        entity.ProgramNotes = dto.ProgramNotes;
        entity.ProgramFileResourceID = dto.ProgramFileResourceID;
        entity.ProgramExampleGeospatialUploadFileResourceID = dto.ProgramExampleGeospatialUploadFileResourceID;

        entity.ProgramLastUpdatedDate = DateTime.UtcNow;
        entity.ProgramLastUpdatedByPersonID = callingPersonID;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProgramID);
    }

    public static async Task<ProgramDeleteInfo?> GetDeleteInfoAsync(WADNRDbContext dbContext, int programID)
    {
        if (!await dbContext.Programs.AnyAsync(x => x.ProgramID == programID))
            return null;

        return new ProgramDeleteInfo
        {
            ProjectCount = await dbContext.ProjectPrograms.CountAsync(x => x.ProgramID == programID),
            TreatmentCount = await dbContext.Treatments.CountAsync(x => x.ProgramID == programID),
            TreatmentUpdateCount = await dbContext.TreatmentUpdates.CountAsync(x => x.ProgramID == programID),
            ProjectLocationCount = await dbContext.ProjectLocations.CountAsync(x => x.ProgramID == programID),
        };
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int programID)
    {
        if (!await dbContext.Programs.AnyAsync(x => x.ProgramID == programID))
            return false;

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        // Subquery helpers to avoid fetching IDs into memory (each becomes a SQL subquery)
        var gisSourceOrgIDsQuery = dbContext.GisUploadSourceOrganizations
            .Where(g => g.ProgramID == programID)
            .Select(g => g.GisUploadSourceOrganizationID);

        var uploadAttemptIDsQuery = dbContext.GisUploadAttempts
            .Where(x => gisSourceOrgIDsQuery.Contains(x.GisUploadSourceOrganizationID))
            .Select(x => x.GisUploadAttemptID);

        var gisFeatureIDsQuery = dbContext.GisFeatures
            .Where(x => uploadAttemptIDsQuery.Contains(x.GisUploadAttemptID))
            .Select(x => x.GisFeatureID);

        var notificationConfigIDsQuery = dbContext.ProgramNotificationConfigurations
            .Where(x => x.ProgramID == programID)
            .Select(x => x.ProgramNotificationConfigurationID);

        var sentIDsQuery = dbContext.ProgramNotificationSents
            .Where(x => notificationConfigIDsQuery.Contains(x.ProgramNotificationConfigurationID))
            .Select(x => x.ProgramNotificationSentID);

        // Remove project associations (junction table only — projects remain intact)
        await dbContext.ProjectPrograms
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();

        // Delete GIS-imported project data (matches legacy DeleteFull behavior)
        await dbContext.Treatments
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();
        await dbContext.TreatmentUpdates
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();
        await dbContext.ProjectLocations
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();

        // Cascade GIS config children (deepest first, using subqueries)
        await dbContext.GisCrossWalkDefaults
            .Where(x => gisSourceOrgIDsQuery.Contains(x.GisUploadSourceOrganizationID))
            .ExecuteDeleteAsync();
        await dbContext.GisDefaultMappings
            .Where(x => gisSourceOrgIDsQuery.Contains(x.GisUploadSourceOrganizationID))
            .ExecuteDeleteAsync();

        // GisExcludeIncludeColumn children first
        var excludeIncludeColumnIDsQuery = dbContext.GisExcludeIncludeColumns
            .Where(x => gisSourceOrgIDsQuery.Contains(x.GisUploadSourceOrganizationID))
            .Select(x => x.GisExcludeIncludeColumnID);

        await dbContext.GisExcludeIncludeColumnValues
            .Where(x => excludeIncludeColumnIDsQuery.Contains(x.GisExcludeIncludeColumnID))
            .ExecuteDeleteAsync();
        await dbContext.GisExcludeIncludeColumns
            .Where(x => gisSourceOrgIDsQuery.Contains(x.GisUploadSourceOrganizationID))
            .ExecuteDeleteAsync();

        // GisUploadAttempt children
        await dbContext.GisFeatureMetadataAttributes
            .Where(x => gisFeatureIDsQuery.Contains(x.GisFeatureID))
            .ExecuteDeleteAsync();
        await dbContext.GisFeatures
            .Where(x => uploadAttemptIDsQuery.Contains(x.GisUploadAttemptID))
            .ExecuteDeleteAsync();
        await dbContext.GisUploadAttemptGisMetadataAttributes
            .Where(x => uploadAttemptIDsQuery.Contains(x.GisUploadAttemptID))
            .ExecuteDeleteAsync();

        // Null out GisUploadAttempt FKs before deleting the upload attempts
        await dbContext.Projects
            .Where(x => x.CreateGisUploadAttemptID != null && uploadAttemptIDsQuery.Contains(x.CreateGisUploadAttemptID.Value))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.CreateGisUploadAttemptID, (int?)null));
        await dbContext.Projects
            .Where(x => x.LastUpdateGisUploadAttemptID != null && uploadAttemptIDsQuery.Contains(x.LastUpdateGisUploadAttemptID.Value))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.LastUpdateGisUploadAttemptID, (int?)null));

        // Null out GisUploadAttempt FKs on remaining Treatments/TreatmentUpdates
        // (rows from other programs may still reference these upload attempts)
        await dbContext.Treatments
            .Where(x => x.CreateGisUploadAttemptID != null && uploadAttemptIDsQuery.Contains(x.CreateGisUploadAttemptID.Value))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.CreateGisUploadAttemptID, (int?)null));
        await dbContext.Treatments
            .Where(x => x.UpdateGisUploadAttemptID != null && uploadAttemptIDsQuery.Contains(x.UpdateGisUploadAttemptID.Value))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.UpdateGisUploadAttemptID, (int?)null));
        await dbContext.TreatmentUpdates
            .Where(x => x.CreateGisUploadAttemptID != null && uploadAttemptIDsQuery.Contains(x.CreateGisUploadAttemptID.Value))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.CreateGisUploadAttemptID, (int?)null));
        await dbContext.TreatmentUpdates
            .Where(x => x.UpdateGisUploadAttemptID != null && uploadAttemptIDsQuery.Contains(x.UpdateGisUploadAttemptID.Value))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.UpdateGisUploadAttemptID, (int?)null));

        await dbContext.GisUploadAttempts
            .Where(x => gisSourceOrgIDsQuery.Contains(x.GisUploadSourceOrganizationID))
            .ExecuteDeleteAsync();
        await dbContext.GisUploadSourceOrganizations
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();

        // Cascade notification children (deepest first, using subqueries)
        await dbContext.ProgramNotificationSentProjects
            .Where(x => sentIDsQuery.Contains(x.ProgramNotificationSentID))
            .ExecuteDeleteAsync();
        await dbContext.ProgramNotificationSents
            .Where(x => notificationConfigIDsQuery.Contains(x.ProgramNotificationConfigurationID))
            .ExecuteDeleteAsync();
        await dbContext.ProgramNotificationConfigurations
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();

        // Delete remaining owned children
        await dbContext.ProjectImportBlockLists
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();
        await dbContext.ProjectUpdatePrograms
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();
        await dbContext.ProgramPeople
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();

        // Delete the program (use ExecuteDeleteAsync to stay consistent and avoid change tracker conflicts)
        await dbContext.Programs
            .Where(x => x.ProgramID == programID)
            .ExecuteDeleteAsync();
        await transaction.CommitAsync();
        return true;
    }

    public static async Task<List<PersonLookupItem>> ListEligibleProgramEditorsAsync(WADNRDbContext dbContext)
    {
        var canEditProgramRoleID = (int)RoleEnum.CanEditProgram;

        var eligiblePersonIDs = await dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => pr.RoleID == canEditProgramRoleID
                         && pr.Person.IsActive
                         && pr.Person.IsUser)
            .Select(pr => pr.PersonID)
            .Distinct()
            .ToListAsync();

        var people = await dbContext.People
            .AsNoTracking()
            .Where(p => eligiblePersonIDs.Contains(p.PersonID))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(PersonProjections.AsLookupItem)
            .ToListAsync();

        return people;
    }

    public static async Task<string?> ValidateEditorsHaveRequiredRoleAsync(WADNRDbContext dbContext, List<int> personIDList)
    {
        var canEditProgramRoleID = (int)RoleEnum.CanEditProgram;

        var personsWithRole = await dbContext.PersonRoles
            .AsNoTracking()
            .Where(pr => personIDList.Contains(pr.PersonID) && pr.RoleID == canEditProgramRoleID)
            .Select(pr => pr.PersonID)
            .Distinct()
            .ToListAsync();

        var peopleWithoutRole = personIDList.Except(personsWithRole).ToList();

        if (peopleWithoutRole.Count > 0)
        {
            var names = await dbContext.People
                .AsNoTracking()
                .Where(p => peopleWithoutRole.Contains(p.PersonID))
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Select(p => p.FirstName + " " + p.LastName)
                .ToListAsync();

            return $"The following user(s) do not have the Program Editor role and cannot be assigned as program editors: {string.Join(", ", names)}.";
        }

        return null;
    }

    public static async Task<List<PersonWithOrganizationLookupItem>> UpdateEditorsAsync(WADNRDbContext dbContext, int programID, ProgramEditorsUpsertRequest request)
    {
        var desiredPersonIDs = request.PersonIDList.Distinct().ToHashSet();

        // Load existing junction records (tracked, so changes are audited)
        var existing = await dbContext.ProgramPeople
            .Where(pp => pp.ProgramID == programID)
            .ToListAsync();

        // Remove editors no longer in the list
        var toRemove = existing.Where(pp => !desiredPersonIDs.Contains(pp.PersonID)).ToList();
        dbContext.ProgramPeople.RemoveRange(toRemove);

        // Add editors not yet in the list
        var existingPersonIDs = existing.Select(pp => pp.PersonID).ToHashSet();
        var toAdd = desiredPersonIDs.Where(id => !existingPersonIDs.Contains(id)).ToList();
        foreach (var personID in toAdd)
        {
            dbContext.ProgramPeople.Add(new ProgramPerson
            {
                ProgramID = programID,
                PersonID = personID
            });
        }

        if (toRemove.Count > 0 || toAdd.Count > 0)
        {
            await dbContext.SaveChangesAsync();
        }

        // Return updated list using PersonProjections
        var editorPersonIDs = await dbContext.ProgramPeople
            .AsNoTracking()
            .Where(pp => pp.ProgramID == programID)
            .Select(pp => pp.PersonID)
            .ToListAsync();

        return await dbContext.People
            .AsNoTracking()
            .Where(p => editorPersonIDs.Contains(p.PersonID))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(PersonProjections.AsLookupItemWithOrganization)
            .ToListAsync();
    }

    public static async Task<List<ProjectImportBlockListGridRow>> ListBlockListEntriesAsync(WADNRDbContext dbContext, int programID)
    {
        return await dbContext.ProjectImportBlockLists
            .AsNoTracking()
            .Where(x => x.ProgramID == programID)
            .Select(x => new ProjectImportBlockListGridRow
            {
                ProjectImportBlockListID = x.ProjectImportBlockListID,
                ProgramID = x.ProgramID,
                ProjectID = x.ProjectID,
                ProjectName = x.ProjectName,
                ProjectGisIdentifier = x.ProjectGisIdentifier,
                Notes = x.Notes,
            })
            .OrderBy(x => x.ProjectName)
            .ToListAsync();
    }

    public static async Task AddToBlockListAsync(WADNRDbContext dbContext, int programID, AddToBlockListRequest request)
    {
        var entry = new ProjectImportBlockList
        {
            ProgramID = programID,
            ProjectGisIdentifier = request.ProjectGisIdentifier,
            ProjectName = request.ProjectName,
            ProjectID = request.ProjectID,
            Notes = request.Notes,
        };
        dbContext.ProjectImportBlockLists.Add(entry);
        await dbContext.SaveChangesAsync();
    }

    public static async Task<bool> DeleteBlockListEntryAsync(WADNRDbContext dbContext, int projectImportBlockListID)
    {
        var deletedCount = await dbContext.ProjectImportBlockLists
            .Where(x => x.ProjectImportBlockListID == projectImportBlockListID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<ProgramGridRow>> ListAsGridRowByOrganizationIDAsync(WADNRDbContext dbContext, int organizationID)
    {
        var entities = await dbContext.Programs
            .AsNoTracking()
            .Where(x => x.OrganizationID == organizationID)
            .Select(ProgramProjections.AsGridRow)
            .OrderBy(x => x.ProgramName)
            .ToListAsync();
        return entities;
    }

    public static async Task<List<ProjectProgramDetailGridRow>> ListProjectsForProgramAsync(WADNRDbContext dbContext, int programID)
    {
        var projects = await dbContext.ProjectPrograms
            .AsNoTracking()
            .Where(pp => pp.ProgramID == programID &&
                         pp.Project.ProjectApprovalStatusID == Projects.ApprovedStatusId &&
                         !pp.Project.ProjectType.LimitVisibilityToAdmin)
            .Select(pp => new ProjectProgramDetailGridRow
            {
                ProjectID = pp.Project.ProjectID,
                ProjectGisIdentifier = pp.Project.ProjectGisIdentifier,
                FhtProjectNumber = pp.Project.FhtProjectNumber ?? string.Empty,
                ProjectName = pp.Project.ProjectName,
                ProjectTypeName = pp.Project.ProjectType.ProjectTypeName,
                ProjectStage = new ProjectStageLookupItem
                {
                    ProjectStageID = pp.Project.ProjectStage.ProjectStageID,
                    ProjectStageName = pp.Project.ProjectStage.ProjectStageName
                },
                Programs = string.Join(", ", pp.Project.ProjectPrograms.Select(p => p.Program.ProgramName ?? "(default)"))
            })
            .OrderBy(p => p.ProjectName)
            .ToListAsync();

        return projects;
    }

    public static async Task<List<ProgramNotificationGridRow>> ListNotificationsForProgramAsync(WADNRDbContext dbContext, int programID)
    {
        var rawNotifications = await dbContext.ProgramNotificationConfigurations
            .AsNoTracking()
            .Where(pnc => pnc.ProgramID == programID)
            .Select(pnc => new
            {
                pnc.ProgramNotificationConfigurationID,
                pnc.ProgramNotificationTypeID,
                pnc.RecurrenceIntervalID,
                pnc.NotificationEmailText
            })
            .ToListAsync();

        var notifications = rawNotifications
            .Select(pnc => new ProgramNotificationGridRow
            {
                ProgramNotificationConfigurationID = pnc.ProgramNotificationConfigurationID,
                ProgramNotificationTypeID = pnc.ProgramNotificationTypeID,
                ProgramNotificationTypeDisplayName = ProgramNotificationType.AllLookupDictionary[pnc.ProgramNotificationTypeID].ProgramNotificationTypeDisplayName,
                RecurrenceIntervalID = pnc.RecurrenceIntervalID,
                RecurrenceIntervalInYears = RecurrenceInterval.AllLookupDictionary[pnc.RecurrenceIntervalID].RecurrenceIntervalInYears,
                NotificationEmailText = pnc.NotificationEmailText
            })
            .OrderBy(n => n.ProgramNotificationTypeDisplayName)
            .ToList();

        return notifications;
    }

    public static async Task<ProgramNotificationGridRow> CreateNotificationAsync(WADNRDbContext dbContext, int programID, ProgramNotificationUpsertRequest request)
    {
        var entity = new ProgramNotificationConfiguration
        {
            ProgramID = programID,
            ProgramNotificationTypeID = request.ProgramNotificationTypeID,
            RecurrenceIntervalID = request.RecurrenceIntervalID,
            NotificationEmailText = request.NotificationEmailText
        };
        dbContext.ProgramNotificationConfigurations.Add(entity);
        await dbContext.SaveChangesAsync();

        return new ProgramNotificationGridRow
        {
            ProgramNotificationConfigurationID = entity.ProgramNotificationConfigurationID,
            ProgramNotificationTypeID = entity.ProgramNotificationTypeID,
            ProgramNotificationTypeDisplayName = ProgramNotificationType.AllLookupDictionary[entity.ProgramNotificationTypeID].ProgramNotificationTypeDisplayName,
            RecurrenceIntervalID = entity.RecurrenceIntervalID,
            RecurrenceIntervalInYears = RecurrenceInterval.AllLookupDictionary[entity.RecurrenceIntervalID].RecurrenceIntervalInYears,
            NotificationEmailText = entity.NotificationEmailText
        };
    }

    public static async Task<ProgramNotificationGridRow?> UpdateNotificationAsync(WADNRDbContext dbContext, int notificationConfigID, ProgramNotificationUpsertRequest request)
    {
        var entity = await dbContext.ProgramNotificationConfigurations
            .FirstOrDefaultAsync(x => x.ProgramNotificationConfigurationID == notificationConfigID);

        if (entity == null) return null;

        entity.ProgramNotificationTypeID = request.ProgramNotificationTypeID;
        entity.RecurrenceIntervalID = request.RecurrenceIntervalID;
        entity.NotificationEmailText = request.NotificationEmailText;

        await dbContext.SaveChangesAsync();

        return new ProgramNotificationGridRow
        {
            ProgramNotificationConfigurationID = entity.ProgramNotificationConfigurationID,
            ProgramNotificationTypeID = entity.ProgramNotificationTypeID,
            ProgramNotificationTypeDisplayName = ProgramNotificationType.AllLookupDictionary[entity.ProgramNotificationTypeID].ProgramNotificationTypeDisplayName,
            RecurrenceIntervalID = entity.RecurrenceIntervalID,
            RecurrenceIntervalInYears = RecurrenceInterval.AllLookupDictionary[entity.RecurrenceIntervalID].RecurrenceIntervalInYears,
            NotificationEmailText = entity.NotificationEmailText
        };
    }

    public static async Task<bool> DeleteNotificationAsync(WADNRDbContext dbContext, int notificationConfigID)
    {
        var sentIDs = await dbContext.ProgramNotificationSents
            .Where(s => s.ProgramNotificationConfigurationID == notificationConfigID)
            .Select(s => s.ProgramNotificationSentID)
            .ToListAsync();

        if (sentIDs.Count > 0)
        {
            await dbContext.ProgramNotificationSentProjects
                .Where(p => sentIDs.Contains(p.ProgramNotificationSentID))
                .ExecuteDeleteAsync();

            await dbContext.ProgramNotificationSents
                .Where(s => s.ProgramNotificationConfigurationID == notificationConfigID)
                .ExecuteDeleteAsync();
        }

        var deletedCount = await dbContext.ProgramNotificationConfigurations
            .Where(x => x.ProgramNotificationConfigurationID == notificationConfigID)
            .ExecuteDeleteAsync();

        return deletedCount > 0;
    }

    public static async Task<GdbImportBasics?> UpdateGdbImportBasicsAsync(WADNRDbContext dbContext, int programID, GdbImportBasicsUpsertRequest request)
    {
        var sourceOrg = await dbContext.GisUploadSourceOrganizations
            .FirstOrDefaultAsync(s => s.ProgramID == programID);

        if (sourceOrg == null) return null;

        sourceOrg.ProjectTypeDefaultName = request.ProjectTypeDefaultName;
        sourceOrg.TreatmentTypeDefaultName = request.TreatmentTypeDefaultName;
        sourceOrg.ImportIsFlattened = request.ImportIsFlattened;
        sourceOrg.AdjustProjectTypeBasedOnTreatmentTypes = request.AdjustProjectTypeBasedOnTreatmentTypes;
        sourceOrg.ProjectStageDefaultID = request.ProjectStageDefaultID;
        sourceOrg.DataDeriveProjectStage = request.DataDeriveProjectStage;
        sourceOrg.DefaultLeadImplementerOrganizationID = request.DefaultLeadImplementerOrganizationID;
        sourceOrg.RelationshipTypeForDefaultOrganizationID = request.RelationshipTypeForDefaultOrganizationID;
        sourceOrg.ImportAsDetailedLocationInsteadOfTreatments = request.ImportAsDetailedLocationInsteadOfTreatments;
        sourceOrg.ImportAsDetailedLocationInAdditionToTreatments = request.ImportAsDetailedLocationInAdditionToTreatments;
        sourceOrg.ProjectDescriptionDefaultText = request.ProjectDescriptionDefaultText;
        sourceOrg.ApplyStartDateToProject = request.ApplyStartDateToProject;
        sourceOrg.ApplyCompletedDateToProject = request.ApplyCompletedDateToProject;
        sourceOrg.ApplyStartDateToTreatments = request.ApplyStartDateToTreatments;
        sourceOrg.ApplyEndDateToTreatments = request.ApplyEndDateToTreatments;

        await dbContext.SaveChangesAsync();

        return await GetGdbImportBasicsAsync(dbContext, sourceOrg);
    }

    private static async Task<GdbImportBasics> GetGdbImportBasicsAsync(WADNRDbContext dbContext, GisUploadSourceOrganization sourceOrg)
    {
        string? stageName = null;
        if (ProjectStage.AllLookupDictionary.TryGetValue(sourceOrg.ProjectStageDefaultID, out var projectStage))
        {
            stageName = projectStage.ProjectStageName;
        }

        var orgName = await dbContext.Organizations
            .Where(o => o.OrganizationID == sourceOrg.DefaultLeadImplementerOrganizationID)
            .Select(o => o.OrganizationName)
            .FirstOrDefaultAsync();

        return new GdbImportBasics
        {
            GisUploadSourceOrganizationID = sourceOrg.GisUploadSourceOrganizationID,
            ProjectTypeDefaultName = sourceOrg.ProjectTypeDefaultName,
            TreatmentTypeDefaultName = sourceOrg.TreatmentTypeDefaultName,
            ImportIsFlattened = sourceOrg.ImportIsFlattened,
            AdjustProjectTypeBasedOnTreatmentTypes = sourceOrg.AdjustProjectTypeBasedOnTreatmentTypes,
            ProjectStageDefaultID = sourceOrg.ProjectStageDefaultID,
            ProjectStageDefaultName = stageName,
            DataDeriveProjectStage = sourceOrg.DataDeriveProjectStage,
            DefaultLeadImplementerOrganizationID = sourceOrg.DefaultLeadImplementerOrganizationID,
            DefaultLeadImplementerOrganizationName = orgName,
            RelationshipTypeForDefaultOrganizationID = sourceOrg.RelationshipTypeForDefaultOrganizationID,
            ImportAsDetailedLocationInsteadOfTreatments = sourceOrg.ImportAsDetailedLocationInsteadOfTreatments,
            ImportAsDetailedLocationInAdditionToTreatments = sourceOrg.ImportAsDetailedLocationInAdditionToTreatments,
            ProjectDescriptionDefaultText = sourceOrg.ProjectDescriptionDefaultText,
            ApplyStartDateToProject = sourceOrg.ApplyStartDateToProject,
            ApplyCompletedDateToProject = sourceOrg.ApplyCompletedDateToProject,
            ApplyStartDateToTreatments = sourceOrg.ApplyStartDateToTreatments,
            ApplyEndDateToTreatments = sourceOrg.ApplyEndDateToTreatments
        };
    }

    public static async Task<List<GdbDefaultMappingItem>?> UpdateGdbDefaultMappingsAsync(WADNRDbContext dbContext, int programID, GdbDefaultMappingUpsertRequest request)
    {
        var sourceOrg = await dbContext.GisUploadSourceOrganizations
            .FirstOrDefaultAsync(s => s.ProgramID == programID);

        if (sourceOrg == null) return null;

        // Remove existing mappings
        var existing = await dbContext.GisDefaultMappings
            .Where(m => m.GisUploadSourceOrganizationID == sourceOrg.GisUploadSourceOrganizationID)
            .ToListAsync();
        dbContext.GisDefaultMappings.RemoveRange(existing);

        // Add new mappings
        foreach (var mapping in request.Mappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.GisDefaultMappingColumnName)) continue;

            dbContext.GisDefaultMappings.Add(new GisDefaultMapping
            {
                GisUploadSourceOrganizationID = sourceOrg.GisUploadSourceOrganizationID,
                FieldDefinitionID = mapping.FieldDefinitionID,
                GisDefaultMappingColumnName = mapping.GisDefaultMappingColumnName
            });
        }

        await dbContext.SaveChangesAsync();
        return await ListGdbDefaultMappingsForProgramAsync(dbContext, sourceOrg.GisUploadSourceOrganizationID);
    }

    public static async Task<List<GdbCrosswalkItem>?> UpdateGdbCrosswalkValuesAsync(WADNRDbContext dbContext, int programID, GdbCrosswalkUpsertRequest request)
    {
        var sourceOrg = await dbContext.GisUploadSourceOrganizations
            .FirstOrDefaultAsync(s => s.ProgramID == programID);

        if (sourceOrg == null) return null;

        // Remove existing crosswalks
        var existing = await dbContext.GisCrossWalkDefaults
            .Where(c => c.GisUploadSourceOrganizationID == sourceOrg.GisUploadSourceOrganizationID)
            .ToListAsync();
        dbContext.GisCrossWalkDefaults.RemoveRange(existing);

        // Add new crosswalks
        foreach (var crosswalk in request.Crosswalks)
        {
            if (string.IsNullOrWhiteSpace(crosswalk.GisCrossWalkSourceValue)) continue;

            dbContext.GisCrossWalkDefaults.Add(new GisCrossWalkDefault
            {
                GisUploadSourceOrganizationID = sourceOrg.GisUploadSourceOrganizationID,
                FieldDefinitionID = crosswalk.FieldDefinitionID,
                GisCrossWalkSourceValue = crosswalk.GisCrossWalkSourceValue,
                GisCrossWalkMappedValue = crosswalk.GisCrossWalkMappedValue
            });
        }

        await dbContext.SaveChangesAsync();
        return await ListGdbCrosswalkValuesForProgramAsync(dbContext, sourceOrg.GisUploadSourceOrganizationID);
    }
}