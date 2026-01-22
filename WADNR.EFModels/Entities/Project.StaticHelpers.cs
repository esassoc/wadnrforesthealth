using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NetTopologySuite.Features;
using System.Linq;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Projects
{
    // Shared EF-usable definition and constant for active projects
    public const int ApprovedStatusId = (int)ProjectApprovalStatusEnum.Approved;

    public static readonly Expression<Func<Project, bool>> IsActiveProjectExpr =
        p => p.ProjectApprovalStatusID == ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin;

    // Reusable projection for the project's lead implementer organization (primary contact org)
    public static readonly Expression<Func<Project, OrganizationLookupItem?>> LeadImplementerOrganizationExpr = p =>
        p.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault();

    public static async Task<List<ProjectCountyDetailGridRow>> ListAsCountyDetailGridRowAsync(WADNRDbContext dbContext, int countyID)
    {
        return await dbContext.Projects
            .Where(p => p.ProjectCounties.Any(pc => pc.CountyID == countyID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Select(ProjectProjections.AsProjectCountyDetailGridRow)
            .ToListAsync();
    }

    public static async Task<List<ProjectProjectTypeDetailGridRow>> ListAsProjectTypeDetailGridRowAsync(WADNRDbContext dbContext, int projectTypeID)
    {
        var rows = await dbContext.Projects
            .Where(p => p.ProjectTypeID == projectTypeID)
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectProjectTypeDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    public static async Task<List<ProjectDNRUplandRegionDetailGridRow>> ListAsDNRUplandDetailGridRowAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {

        var rows = await dbContext.Projects
            .Where(p => p.ProjectRegions.Any(x => x.DNRUplandRegionID == dnrUplandRegionID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsDnrUplandRegionDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalTreatedAcresByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalTreatedAcres = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : 0m;
        }

        return rows;
    }

    public static async Task<ProjectDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectID)
    {
        var entity = await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (entity == null) return null;

        // Populate FundingSources from the static lookup table
        var fundingSourceIds = await dbContext.ProjectFundingSources
            .AsNoTracking()
            .Where(pfs => pfs.ProjectID == projectID)
            .Select(pfs => pfs.FundingSourceID)
            .ToListAsync();

        entity.FundingSources = fundingSourceIds
            .Select(id => FundingSource.AllLookupDictionary.TryGetValue(id, out var fs) ? fs.FundingSourceDisplayName : null)
            .Where(name => name != null)
            .Cast<string>()
            .OrderBy(name => name)
            .ToList();

        return entity;
    }

    public static async Task<ProjectDetail?> CreateAsync(WADNRDbContext dbContext, ProjectUpsertRequest dto, int callingPersonID)
    {
        var entity = new Project
        {
            ProjectTypeID = dto.ProjectTypeID,
            ProjectStageID = dto.ProjectStageID,
            ProjectName = dto.ProjectName,
            ProjectDescription = dto.ProjectDescription,
            CompletionDate = dto.CompletionDate,
            EstimatedTotalCost = dto.EstimatedTotalCost,
            IsFeatured = false,
            ProjectLocationSimpleTypeID = dto.ProjectLocationSimpleTypeID,
            ProjectApprovalStatusID = dto.ProjectApprovalStatusID,
            ProposingPersonID = dto.ProposingPersonID,
            ProposingDate = dto.ProposingDate,
            SubmissionDate = dto.SubmissionDate,
            ApprovalDate = dto.ApprovalDate,
            FocusAreaID = dto.FocusAreaID,
            ExpirationDate = dto.ExpirationDate,
            FhtProjectNumber = dto.FhtProjectNumber
        };
        dbContext.Projects.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProjectID);
    }

    public static async Task<ProjectDetail?> UpdateAsync(WADNRDbContext dbContext, int projectID, ProjectUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.Projects
            .FirstAsync(x => x.ProjectID == projectID);

        entity.ProjectTypeID = dto.ProjectTypeID;
        entity.ProjectStageID = dto.ProjectStageID;
        entity.ProjectName = dto.ProjectName;
        entity.ProjectDescription = dto.ProjectDescription;
        entity.CompletionDate = dto.CompletionDate;
        entity.EstimatedTotalCost = dto.EstimatedTotalCost;
        entity.ProjectLocationSimpleTypeID = dto.ProjectLocationSimpleTypeID;
        entity.ProjectApprovalStatusID = dto.ProjectApprovalStatusID;
        entity.ProposingPersonID = dto.ProposingPersonID;
        entity.ProposingDate = dto.ProposingDate;
        entity.SubmissionDate = dto.SubmissionDate;
        entity.ApprovalDate = dto.ApprovalDate;
        entity.FocusAreaID = dto.FocusAreaID;
        entity.ExpirationDate = dto.ExpirationDate;
        entity.FhtProjectNumber = dto.FhtProjectNumber;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProjectID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int projectID)
    {
        var deletedCount = await dbContext.Projects
            .Where(x => x.ProjectID == projectID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<ProjectGridRow>> ListAsGridRowAsync(
        IQueryable<Project> projectsQuery,
        WADNRDbContext dbContext)
    {
        var projects = await projectsQuery
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalTreatedAcresByProjectAsync(dbContext);

        foreach (var p in projects)
        {
            p.TotalTreatedAcres = totalsByProjectId.TryGetValue(p.ProjectID, out var total) ? total : 0m;
        }

        return projects;
    }

    // Convenience overload for all projects
    public static Task<List<ProjectGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
        => ListAsGridRowAsync(dbContext.Projects, dbContext);

    public static async Task<List<ProjectSimpleTree>> ListWithNoSimpleLocationAsProjectSimpleTree(
        WADNRDbContext dbContext)
    {
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(x => x.ProjectLocationPoint == null)
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectSimpleTree)
            .ToListAsync();

        return projects;
    }

    private static async Task<Dictionary<int, decimal>> GetTotalTreatedAcresByProjectAsync(WADNRDbContext dbContext)
    {
        var totals = await dbContext.vTotalTreatedAcresByProjects
            .AsNoTracking()
            .ToListAsync();

        return totals.ToDictionary(t => t.ProjectID, t => t.TotalTreatedAcres ?? 0m);
    }

    public static async Task<ProjectMapPopup?> GetByIDAsMapPopupAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsMapPopup)
            .SingleOrDefaultAsync();
    }

    private sealed class ProjectTypeDetailProjectMapPointBaseRow
    {
        public int ProjectID { get; init; }
        public int ProjectStageID { get; init; }
        public int ProjectTypeID { get; init; }
        public int? OrganizationID { get; init; }
        public NetTopologySuite.Geometries.Geometry ProjectLocationPoint { get; init; } = null!;
    }

    public static async Task<FeatureCollection> MapProjectFeatureCollection(IQueryable<Project> projectsThatShouldShowOnMap)
    {
        var baseRows = await projectsThatShouldShowOnMap
            .AsNoTracking()
            .Where(x => x.ProjectLocationPoint != null)
            .Select(x => new ProjectTypeDetailProjectMapPointBaseRow
            {
                ProjectID = x.ProjectID,
                ProjectStageID = x.ProjectStageID,
                ProjectTypeID = x.ProjectTypeID,
                OrganizationID = x.ProjectOrganizations
                    .Where(po => po.RelationshipType.IsPrimaryContact)
                    .Select(po => (int?)po.OrganizationID)
                    .FirstOrDefault(),
                ProjectLocationPoint = x.ProjectLocationPoint!
            })
            .ToListAsync();

        if (baseRows.Count == 0)
        {
            return new FeatureCollection();
        }

        var projectIds = baseRows.Select(r => r.ProjectID).ToList();

        var programPairs = await projectsThatShouldShowOnMap
            .AsNoTracking()
            .Where(p => projectIds.Contains(p.ProjectID))
            .SelectMany(p => p.ProjectPrograms.Select(pp => new { p.ProjectID, pp.ProgramID }))
            .ToListAsync();

        var programCsvByProjectId = programPairs
            .GroupBy(x => x.ProjectID)
            .ToDictionary(
                g => g.Key,
                g => string.Join(",", g.Select(x => x.ProgramID).Distinct().OrderBy(id => id)));

        var classificationPairs = await projectsThatShouldShowOnMap
            .AsNoTracking()
            .Where(p => projectIds.Contains(p.ProjectID))
            .SelectMany(p => p.ProjectClassifications.Select(pc => new { p.ProjectID, pc.ClassificationID }))
            .ToListAsync();

        var classificationCsvByProjectId = classificationPairs
            .GroupBy(x => x.ProjectID)
            .ToDictionary(
                g => g.Key,
                g => string.Join(",", g.Select(x => x.ClassificationID).Distinct().OrderBy(id => id)));

        var featureCollection = new FeatureCollection();

        foreach (var r in baseRows)
        {
            var attributes = new AttributesTable
            {
                { "ProjectID", r.ProjectID },
                { "ProjectStageID", r.ProjectStageID },
                { "ProjectTypeID", r.ProjectTypeID },
                { "OrganizationID", r.OrganizationID ?? 0 },
                { "ProgramID", programCsvByProjectId.TryGetValue(r.ProjectID, out var prog) ? prog : string.Empty },
                { "ClassificationID", classificationCsvByProjectId.TryGetValue(r.ProjectID, out var cls) ? cls : string.Empty },
            };

            featureCollection.Add(new Feature(r.ProjectLocationPoint, attributes));
        }

        return featureCollection;
    }

    public static async Task<List<ProjectClassificationDetailGridRow>> ListAsClassificationDetailGridRowAsync(WADNRDbContext dbContext, int classificationID)
    {
        return await dbContext.ProjectClassifications
            .Where(pc => pc.ClassificationID == classificationID)
            .AsNoTracking()
            .Where(pc => pc.Project.ProjectApprovalStatusID == ApprovedStatusId && !pc.Project.ProjectType.LimitVisibilityToAdmin)
            .OrderBy(pc => pc.Project.ProjectName)
            .Select(ProjectProjections.AsProjectClassificationDetailGridRow)
            .ToListAsync();
    }

    public static async Task<ProjectFactSheet?> GetByIDAsFactSheetAsync(WADNRDbContext dbContext, int projectID)
    {
        var entity = await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsFactSheet)
            .SingleOrDefaultAsync();

        return entity;
    }

    public static async Task<List<ClassificationLookupItem>> ListClassificationsAsLookupItemByProjectIDAsync(WADNRDbContext dbContext, int projectID)
    {
        var classifications = await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .SelectMany(x => x.ProjectClassifications.Select(pc => pc.Classification))
            .Select(c => new ClassificationLookupItem
            {
                ClassificationID = c.ClassificationID,
                DisplayName = c.DisplayName
            })
            .Distinct()
            .OrderBy(x => x.DisplayName)
            .ToListAsync();

        return classifications;
    }

    private static async Task<Dictionary<int, decimal?>> GetTotalFundingByProjectAsync(WADNRDbContext dbContext)
    {
        var totals = await dbContext.ProjectFundSourceAllocationRequests
            .AsNoTracking()
            .GroupBy(r => r.ProjectID)
            .Select(g => new
            {
                ProjectID = g.Key,
                TotalAmount = g.Sum(x => (decimal?)x.TotalAmount)
            })
            .ToListAsync();

        return totals.ToDictionary(x => x.ProjectID, x => x.TotalAmount);
    }

    public static async Task<List<ProjectTagDetailGridRow>> ListAsTagDetailGridRowAsync(WADNRDbContext dbContext, int tagID)
    {
        var rows = await dbContext.Projects
            .Where(p => p.ProjectTags.Any(pt => pt.TagID == tagID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectTagDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    public static async Task<List<ProjectOrganizationDetailGridRow>> ListAsOrganizationDetailGridRowAsync(WADNRDbContext dbContext, int organizationID)
    {
        var rows = await dbContext.Projects
            .Where(p => p.ProjectOrganizations.Any(po => po.OrganizationID == organizationID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectOrganizationDetailGridRow(organizationID))
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    public static async Task<List<TreatmentGridRow>> ListTreatmentsForProjectAsync(WADNRDbContext dbContext, int projectID)
    {
        var rawTreatments = await dbContext.Treatments
            .AsNoTracking()
            .Where(t => t.ProjectID == projectID)
            .Select(t => new
            {
                t.TreatmentID,
                t.TreatmentTypeID,
                t.TreatmentDetailedActivityTypeID,
                t.TreatmentStartDate,
                t.TreatmentEndDate,
                t.TreatmentFootprintAcres,
                t.TreatmentTreatedAcres,
                t.TreatmentNotes,
                t.TreatmentCodeID,
                ProgramName = t.Program != null
                    ? (t.Program.IsDefaultProgramForImportOnly
                        ? (t.Program.Organization != null ? t.Program.Organization.OrganizationName : null)
                        : t.Program.ProgramName)
                    : null
            })
            .ToListAsync();

        var treatments = rawTreatments
            .Select(t => new TreatmentGridRow
            {
                TreatmentID = t.TreatmentID,
                TreatmentTypeName = TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var tt)
                    ? tt.TreatmentTypeName
                    : $"Unknown ({t.TreatmentTypeID})",
                TreatmentDetailedActivityTypeName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(t.TreatmentDetailedActivityTypeID, out var tda)
                    ? tda.TreatmentDetailedActivityTypeName
                    : $"Unknown ({t.TreatmentDetailedActivityTypeID})",
                TreatmentStartDate = t.TreatmentStartDate,
                TreatmentEndDate = t.TreatmentEndDate,
                TreatmentFootprintAcres = t.TreatmentFootprintAcres,
                TreatmentTreatedAcres = t.TreatmentTreatedAcres,
                TreatmentNotes = t.TreatmentNotes,
                ProgramName = t.ProgramName,
                TreatmentCodeName = t.TreatmentCodeID.HasValue && TreatmentCode.AllLookupDictionary.TryGetValue(t.TreatmentCodeID.Value, out var tc)
                    ? tc.TreatmentCodeName
                    : null
            })
            .OrderBy(t => t.TreatmentStartDate)
            .ThenBy(t => t.TreatmentTypeName)
            .ToList();

        return treatments;
    }

    public static async Task<List<InteractionEventGridRow>> ListInteractionEventsForProjectAsync(WADNRDbContext dbContext, int projectID)
    {
        var rawEvents = await dbContext.InteractionEventProjects
            .AsNoTracking()
            .Where(iep => iep.ProjectID == projectID && iep.InteractionEvent != null)
            .Select(iep => new
            {
                InteractionEventID = iep.InteractionEvent!.InteractionEventID,
                InteractionEventTitle = iep.InteractionEvent.InteractionEventTitle,
                InteractionEventDescription = iep.InteractionEvent.InteractionEventDescription,
                InteractionEventDate = iep.InteractionEvent.InteractionEventDate,
                InteractionEventTypeID = iep.InteractionEvent.InteractionEventTypeID,
                StaffPersonID = iep.InteractionEvent.StaffPersonID,
                StaffPersonFirstName = iep.InteractionEvent.StaffPerson != null ? iep.InteractionEvent.StaffPerson.FirstName : null,
                StaffPersonLastName = iep.InteractionEvent.StaffPerson != null ? iep.InteractionEvent.StaffPerson.LastName : null
            })
            .ToListAsync();

        var events = rawEvents
            .Select(e => new InteractionEventGridRow
            {
                InteractionEventID = e.InteractionEventID,
                InteractionEventTitle = e.InteractionEventTitle ?? string.Empty,
                InteractionEventDescription = e.InteractionEventDescription,
                InteractionEventDate = e.InteractionEventDate,
                InteractionEventType = InteractionEventType.AllLookupDictionary.TryGetValue(e.InteractionEventTypeID, out var iet)
                    ? new InteractionEventTypeLookupItem
                    {
                        InteractionEventTypeID = iet.InteractionEventTypeID,
                        InteractionEventTypeDisplayName = iet.InteractionEventTypeDisplayName
                    }
                    : new InteractionEventTypeLookupItem
                    {
                        InteractionEventTypeID = e.InteractionEventTypeID,
                        InteractionEventTypeDisplayName = $"Unknown ({e.InteractionEventTypeID})"
                    },
                StaffPerson = e.StaffPersonID.HasValue
                    ? new PersonLookupItem
                    {
                        PersonID = e.StaffPersonID.Value,
                        FullName = $"{e.StaffPersonFirstName} {e.StaffPersonLastName}"
                    }
                    : null
            })
            .OrderByDescending(e => e.InteractionEventDate)
            .ToList();

        return events;
    }

    public static async Task<List<ProjectOrganizationDetailGridRow>> ListPendingAsOrganizationDetailGridRowAsync(WADNRDbContext dbContext, int organizationID)
    {
        var rows = await dbContext.Projects
            .Where(p => p.ProjectOrganizations.Any(po => po.OrganizationID == organizationID))
            .AsNoTracking()
            .Where(p => p.ProjectApprovalStatusID != ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectOrganizationDetailGridRow(organizationID))
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    public static async Task<List<ProjectDocumentGridRow>> ListDocumentsForProjectAsync(WADNRDbContext dbContext, int projectID)
    {
        var rawDocuments = await dbContext.ProjectDocuments
            .AsNoTracking()
            .Where(d => d.ProjectID == projectID)
            .Select(d => new
            {
                d.ProjectDocumentID,
                d.DisplayName,
                d.Description,
                d.ProjectDocumentTypeID,
                d.FileResourceID,
                FileResourceGuid = d.FileResource.FileResourceGUID.ToString()
            })
            .ToListAsync();

        var documents = rawDocuments
            .Select(d => new ProjectDocumentGridRow
            {
                ProjectDocumentID = d.ProjectDocumentID,
                DisplayName = d.DisplayName,
                Description = d.Description,
                DocumentTypeName = d.ProjectDocumentTypeID.HasValue && ProjectDocumentType.AllLookupDictionary.TryGetValue(d.ProjectDocumentTypeID.Value, out var dt)
                    ? dt.ProjectDocumentTypeDisplayName
                    : null,
                FileResourceID = d.FileResourceID,
                FileResourceGuid = d.FileResourceGuid
            })
            .OrderBy(d => d.DisplayName)
            .ToList();

        return documents;
    }

    public static async Task<List<ProjectNoteGridRow>> ListNotesForProjectAsync(WADNRDbContext dbContext, int projectID)
    {
        var notes = await dbContext.ProjectNotes
            .AsNoTracking()
            .Where(n => n.ProjectID == projectID)
            .Select(n => new ProjectNoteGridRow
            {
                ProjectNoteID = n.ProjectNoteID,
                Note = n.Note,
                CreatedByPersonName = n.CreatePerson != null
                    ? n.CreatePerson.FirstName + " " + n.CreatePerson.LastName
                    : null,
                CreateDate = n.CreateDate,
                UpdatedByPersonName = n.UpdatePerson != null
                    ? n.UpdatePerson.FirstName + " " + n.UpdatePerson.LastName
                    : null,
                UpdateDate = n.UpdateDate
            })
            .OrderByDescending(n => n.CreateDate)
            .ToListAsync();

        return notes;
    }

    public static async Task<List<ProjectExternalLinkGridRow>> ListExternalLinksForProjectAsync(WADNRDbContext dbContext, int projectID)
    {
        var links = await dbContext.ProjectExternalLinks
            .AsNoTracking()
            .Where(l => l.ProjectID == projectID)
            .Select(l => new ProjectExternalLinkGridRow
            {
                ProjectExternalLinkID = l.ProjectExternalLinkID,
                ExternalLinkLabel = l.ExternalLinkLabel,
                ExternalLinkUrl = l.ExternalLinkUrl
            })
            .OrderBy(l => l.ExternalLinkLabel)
            .ToListAsync();

        return links;
    }
}
