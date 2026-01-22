using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProgramProjections
{
    public static readonly Expression<Func<Program, ProgramDetail>> AsDetail = x => new ProgramDetail
    {
        ProgramID = x.ProgramID,
        ProgramName = x.ProgramName,
        ProgramShortName = x.ProgramShortName,
        ProgramIsActive = x.ProgramIsActive,
        IsDefaultProgramForImportOnly = x.IsDefaultProgramForImportOnly,
        ProgramNotes = x.ProgramNotes,

        // Parent Organization
        OrganizationID = x.OrganizationID,
        OrganizationName = x.Organization != null ? x.Organization.OrganizationName : null,

        // Primary Contact
        PrimaryContactPersonID = x.ProgramPrimaryContactPersonID,
        PrimaryContactPersonFullName = x.ProgramPrimaryContactPerson != null
            ? x.ProgramPrimaryContactPerson.FirstName + " " + x.ProgramPrimaryContactPerson.LastName
            : null,
        PrimaryContactPersonOrganization = x.ProgramPrimaryContactPerson != null && x.ProgramPrimaryContactPerson.Organization != null
            ? x.ProgramPrimaryContactPerson.Organization.OrganizationName
            : null,

        // Program Document File
        ProgramFileResourceID = x.ProgramFileResourceID,
        ProgramFileResourceUrl = x.ProgramFileResource != null
            ? x.ProgramFileResource.FileResourceGUID.ToString()
            : null,
        ProgramFileName = x.ProgramFileResource != null
            ? x.ProgramFileResource.OriginalBaseFilename + x.ProgramFileResource.OriginalFileExtension
            : null,

        // Program Example Geospatial File
        ProgramExampleGeospatialUploadFileResourceID = x.ProgramExampleGeospatialUploadFileResourceID,
        ProgramExampleGeospatialUploadFileResourceUrl = x.ProgramExampleGeospatialUploadFileResource != null
            ? x.ProgramExampleGeospatialUploadFileResource.FileResourceGUID.ToString()
            : null,
        ProgramExampleGeospatialUploadFileName = x.ProgramExampleGeospatialUploadFileResource != null
            ? x.ProgramExampleGeospatialUploadFileResource.OriginalBaseFilename + x.ProgramExampleGeospatialUploadFileResource.OriginalFileExtension
            : null,

        // Program Editors (People who can edit this program)
        ProgramEditors = x.ProgramPeople
            .OrderBy(pp => pp.Person.LastName)
            .ThenBy(pp => pp.Person.FirstName)
            .Select(pp => new PersonLookupItem
            {
                PersonID = pp.Person.PersonID,
                FullName = pp.Person.FirstName + " " + pp.Person.LastName
            }).ToList(),

        // Counts
        ProjectCount = x.ProjectPrograms
            .Count(pp => pp.Project.ProjectApprovalStatusID == Projects.ApprovedStatusId &&
                         !pp.Project.ProjectType.LimitVisibilityToAdmin),

        // GDB Import Basics (from GisUploadSourceOrganization)
        GdbImportBasics = x.GisUploadSourceOrganization != null ? new GdbImportBasics
        {
            GisUploadSourceOrganizationID = x.GisUploadSourceOrganization.GisUploadSourceOrganizationID,
            ProjectTypeDefaultName = x.GisUploadSourceOrganization.ProjectTypeDefaultName,
            TreatmentTypeDefaultName = x.GisUploadSourceOrganization.TreatmentTypeDefaultName,
            ImportIsFlattened = x.GisUploadSourceOrganization.ImportIsFlattened,
            AdjustProjectTypeBasedOnTreatmentTypes = x.GisUploadSourceOrganization.AdjustProjectTypeBasedOnTreatmentTypes,
            ProjectStageDefaultID = x.GisUploadSourceOrganization.ProjectStageDefaultID,
            ProjectStageDefaultName = null, // Populated in static helper
            DataDeriveProjectStage = x.GisUploadSourceOrganization.DataDeriveProjectStage,
            DefaultLeadImplementerOrganizationName = x.GisUploadSourceOrganization.DefaultLeadImplementerOrganization != null
                ? x.GisUploadSourceOrganization.DefaultLeadImplementerOrganization.OrganizationName
                : null,
            ImportAsDetailedLocationInsteadOfTreatments = x.GisUploadSourceOrganization.ImportAsDetailedLocationInsteadOfTreatments,
            ImportAsDetailedLocationInAdditionToTreatments = x.GisUploadSourceOrganization.ImportAsDetailedLocationInAdditionToTreatments,
            ProjectDescriptionDefaultText = x.GisUploadSourceOrganization.ProjectDescriptionDefaultText,
            ApplyStartDateToProject = x.GisUploadSourceOrganization.ApplyStartDateToProject,
            ApplyCompletedDateToProject = x.GisUploadSourceOrganization.ApplyCompletedDateToProject,
            ApplyStartDateToTreatments = x.GisUploadSourceOrganization.ApplyStartDateToTreatments,
            ApplyEndDateToTreatments = x.GisUploadSourceOrganization.ApplyEndDateToTreatments
        } : null
    };

    public static readonly Expression<Func<Program, ProgramGridRow>> AsGridRow = x => new ProgramGridRow
    {
        ProgramID = x.ProgramID,
        ProgramName = x.ProgramName ?? "(default)",
        ProgramShortName = x.ProgramShortName,
        IsActive = x.ProgramIsActive,
        IsDefaultProgramForImportOnly = x.IsDefaultProgramForImportOnly,
        Organization = x.Organization != null ? new OrganizationLookupItem
        {
            OrganizationID = x.Organization.OrganizationID,
            OrganizationName = x.Organization.OrganizationName
        } : null,
        ProjectCount = x.ProjectPrograms
            .Count(pp => pp.Project.ProjectApprovalStatusID == Projects.ApprovedStatusId &&
                         !pp.Project.ProjectType.LimitVisibilityToAdmin)
    };

    public static readonly Expression<Func<Program, ProgramLookupItem>> AsLookupItem = x => new ProgramLookupItem
    {
        ProgramID = x.ProgramID,
        ProgramName = x.DisplayName
    };
}