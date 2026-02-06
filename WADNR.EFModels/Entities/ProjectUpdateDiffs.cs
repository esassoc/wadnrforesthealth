using HtmlDiff;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace WADNR.EFModels.Entities;

/// <summary>
/// Generates HTML diffs comparing ProjectUpdateBatch data to current Project data.
/// Used during approval to create audit trail of changes.
/// </summary>
public static class ProjectUpdateDiffs
{
    // CSS classes matching legacy implementation
    public const string BackgroundColorForAddedElement = "#cfc";
    public const string BackgroundColorForDeletedElement = "#FEC8C8";
    public const string DisplayCssClassDeletedElement = "deleted-element";
    public const string DisplayCssClassAddedElement = "added-element";

    /// <summary>
    /// Generates all diff logs for a ProjectUpdateBatch and stores them in the batch's diff log columns.
    /// Call this before approval to create an audit trail.
    /// </summary>
    public static async Task GenerateAndStoreDiffsAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectOrganizations).ThenInclude(po => po.Organization)
            .Include(p => p.ProjectOrganizations).ThenInclude(po => po.RelationshipType)
            .Include(p => p.ProjectPeople).ThenInclude(pp => pp.Person)
            .Include(p => p.ProjectExternalLinks)
            .Include(p => p.ProjectNotes)
            .Include(p => p.ProjectFundingSources)
            .FirstOrDefaultAsync(p => p.ProjectID == batch.ProjectID);

        if (project == null) return;

        // Load the update data
        var projectUpdate = await dbContext.ProjectUpdates
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        // Generate each section's diff
        batch.BasicsDiffLog = await GenerateBasicsDiffAsync(dbContext, project, projectUpdate, batch);
        batch.OrganizationsDiffLog = await GenerateOrganizationsDiffAsync(dbContext, project, batch);
        batch.ExternalLinksDiffLog = await GenerateExternalLinksDiffAsync(dbContext, project, batch);
        batch.NotesDiffLog = await GenerateNotesDiffAsync(dbContext, project, batch);
        batch.ExpectedFundingDiffLog = await GenerateExpectedFundingDiffAsync(dbContext, project, batch);
    }

    #region Basics Diff

    private static Task<string?> GenerateBasicsDiffAsync(
        WADNRDbContext dbContext,
        Project project,
        ProjectUpdate? projectUpdate,
        ProjectUpdateBatch batch)
    {
        if (projectUpdate == null) return Task.FromResult<string?>(null);

        var originalHtml = RenderBasicsHtml(project);
        var updatedHtml = RenderBasicsUpdateHtml(projectUpdate, project.ProjectName);

        if (originalHtml == updatedHtml) return Task.FromResult<string?>(null);

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return Task.FromResult<string?>(diff.Build());
    }

    private static string RenderBasicsHtml(Project project)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");

        AppendRow(sb, "Project Stage", project.ProjectStage?.ProjectStageDisplayName ?? "");
        AppendRow(sb, "Project Initiation date", project.PlannedDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Completion Date", project.CompletionDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Project Description", project.ProjectDescription ?? "");
        AppendRow(sb, "Expiration Date", project.ExpirationDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Estimated Total Cost", project.EstimatedTotalCost?.ToString("C") ?? "");

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    private static string RenderBasicsUpdateHtml(ProjectUpdate projectUpdate, string projectName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");

        AppendRow(sb, "Project Stage", ProjectStage.AllLookupDictionary.TryGetValue(projectUpdate.ProjectStageID, out var stage) ? stage.ProjectStageDisplayName : "");
        AppendRow(sb, "Project Initiation date", projectUpdate.PlannedDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Completion Date", projectUpdate.CompletionDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Project Description", projectUpdate.ProjectDescription ?? "");
        AppendRow(sb, "Expiration Date", projectUpdate.ExpirationDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Estimated Total Cost", projectUpdate.EstimatedTotalCost?.ToString("C") ?? "");

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    #endregion

    #region Organizations Diff

    private static async Task<string?> GenerateOrganizationsDiffAsync(
        WADNRDbContext dbContext,
        Project project,
        ProjectUpdateBatch batch)
    {
        var updateOrgs = await dbContext.ProjectOrganizationUpdates
            .Include(po => po.Organization)
            .Include(po => po.RelationshipType)
            .Where(po => po.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var originalHtml = RenderOrganizationsHtml(project.ProjectOrganizations.ToList());
        var updatedHtml = RenderOrganizationUpdatesHtml(updateOrgs);

        if (originalHtml == updatedHtml) return null;

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return diff.Build();
    }

    private static string RenderOrganizationsHtml(List<ProjectOrganization> orgs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Organization</th><th>Relationship Type</th></tr>");

        foreach (var org in orgs.OrderBy(o => o.RelationshipType?.RelationshipTypeName).ThenBy(o => o.Organization?.OrganizationName))
        {
            AppendRow(sb, org.Organization?.OrganizationName ?? "(unknown)", org.RelationshipType?.RelationshipTypeName ?? "(unknown)");
        }

        if (!orgs.Any())
        {
            sb.AppendLine("<tr><td colspan='2'><em>No organizations</em></td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    private static string RenderOrganizationUpdatesHtml(List<ProjectOrganizationUpdate> orgs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Organization</th><th>Relationship Type</th></tr>");

        foreach (var org in orgs.OrderBy(o => o.RelationshipType?.RelationshipTypeName).ThenBy(o => o.Organization?.OrganizationName))
        {
            AppendRow(sb, org.Organization?.OrganizationName ?? "(unknown)", org.RelationshipType?.RelationshipTypeName ?? "(unknown)");
        }

        if (!orgs.Any())
        {
            sb.AppendLine("<tr><td colspan='2'><em>No organizations</em></td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    #endregion

    #region External Links Diff

    private static async Task<string?> GenerateExternalLinksDiffAsync(
        WADNRDbContext dbContext,
        Project project,
        ProjectUpdateBatch batch)
    {
        var updateLinks = await dbContext.ProjectExternalLinkUpdates
            .Where(l => l.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var originalHtml = RenderExternalLinksHtml(project.ProjectExternalLinks.ToList());
        var updatedHtml = RenderExternalLinkUpdatesHtml(updateLinks);

        if (originalHtml == updatedHtml) return null;

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return diff.Build();
    }

    private static string RenderExternalLinksHtml(List<ProjectExternalLink> links)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Label</th><th>URL</th></tr>");

        foreach (var link in links.OrderBy(l => l.ExternalLinkLabel))
        {
            AppendRow(sb, link.ExternalLinkLabel ?? "(no label)", link.ExternalLinkUrl ?? "(no url)");
        }

        if (!links.Any())
        {
            sb.AppendLine("<tr><td colspan='2'><em>No external links</em></td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    private static string RenderExternalLinkUpdatesHtml(List<ProjectExternalLinkUpdate> links)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Label</th><th>URL</th></tr>");

        foreach (var link in links.OrderBy(l => l.ExternalLinkLabel))
        {
            AppendRow(sb, link.ExternalLinkLabel ?? "(no label)", link.ExternalLinkUrl ?? "(no url)");
        }

        if (!links.Any())
        {
            sb.AppendLine("<tr><td colspan='2'><em>No external links</em></td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    #endregion

    #region Notes Diff

    private static async Task<string?> GenerateNotesDiffAsync(
        WADNRDbContext dbContext,
        Project project,
        ProjectUpdateBatch batch)
    {
        var updateNotes = await dbContext.ProjectNoteUpdates
            .Where(n => n.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var originalHtml = RenderNotesHtml(project.ProjectNotes.ToList());
        var updatedHtml = RenderNoteUpdatesHtml(updateNotes);

        if (originalHtml == updatedHtml) return null;

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return diff.Build();
    }

    private static string RenderNotesHtml(List<ProjectNote> notes)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Note</th><th>Created</th></tr>");

        foreach (var note in notes.OrderBy(n => n.CreateDate))
        {
            AppendRow(sb, note.Note ?? "(empty)", note.CreateDate.ToString("g"));
        }

        if (!notes.Any())
        {
            sb.AppendLine("<tr><td colspan='2'><em>No notes</em></td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    private static string RenderNoteUpdatesHtml(List<ProjectNoteUpdate> notes)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Note</th><th>Created</th></tr>");

        foreach (var note in notes.OrderBy(n => n.CreateDate))
        {
            AppendRow(sb, note.Note ?? "(empty)", note.CreateDate.ToString("g"));
        }

        if (!notes.Any())
        {
            sb.AppendLine("<tr><td colspan='2'><em>No notes</em></td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    #endregion

    #region Expected Funding Diff

    private static async Task<string?> GenerateExpectedFundingDiffAsync(
        WADNRDbContext dbContext,
        Project project,
        ProjectUpdateBatch batch)
    {
        var updateFunding = await dbContext.ProjectFundingSourceUpdates
            .Where(f => f.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var originalHtml = RenderFundingHtml(project.ProjectFundingSources.ToList());
        var updatedHtml = RenderFundingUpdatesHtml(updateFunding);

        if (originalHtml == updatedHtml) return null;

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return diff.Build();
    }

    private static string RenderFundingHtml(List<ProjectFundingSource> funding)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Funding Source</th></tr>");

        foreach (var f in funding.OrderBy(f => GetFundingSourceName(f.FundingSourceID)))
        {
            sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(GetFundingSourceName(f.FundingSourceID))}</td></tr>");
        }

        if (!funding.Any())
        {
            sb.AppendLine("<tr><td><em>No funding sources</em></td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    private static string RenderFundingUpdatesHtml(List<ProjectFundingSourceUpdate> funding)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Funding Source</th></tr>");

        foreach (var f in funding.OrderBy(f => GetFundingSourceName(f.FundingSourceID)))
        {
            sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(GetFundingSourceName(f.FundingSourceID))}</td></tr>");
        }

        if (!funding.Any())
        {
            sb.AppendLine("<tr><td><em>No funding sources</em></td></tr>");
        }

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    private static string GetFundingSourceName(int fundingSourceID)
    {
        return FundingSource.AllLookupDictionary.TryGetValue(fundingSourceID, out var fs)
            ? fs.FundingSourceDisplayName
            : "(unknown)";
    }

    #endregion

    #region Helpers

    private static void AppendRow(StringBuilder sb, string col1, string col2)
    {
        sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(col1)}</td><td>{System.Net.WebUtility.HtmlEncode(col2)}</td></tr>");
    }

    #endregion

    #region Per-Step Real-Time Diff Generation

    /// <summary>
    /// Gets real-time diff for a specific step by comparing update batch data to current project data.
    /// Returns a response indicating whether changes exist and the diff HTML.
    /// </summary>
    public static async Task<StepDiffResponse> GetStepDiffAsync(WADNRDbContext dbContext, int projectUpdateBatchID, string stepKey)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
        {
            return new StepDiffResponse { HasChanges = false, DiffHtml = null };
        }

        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.ProjectOrganizations).ThenInclude(po => po.Organization)
            .Include(p => p.ProjectOrganizations).ThenInclude(po => po.RelationshipType)
            .Include(p => p.ProjectPeople).ThenInclude(pp => pp.Person)
            .Include(p => p.ProjectExternalLinks)
            .Include(p => p.ProjectNotes)
            .Include(p => p.ProjectDocuments)
            .Include(p => p.ProjectFundingSources)
            .Include(p => p.ProjectFundSourceAllocationRequests)
            .Include(p => p.ProjectPriorityLandscapes).ThenInclude(pl => pl.PriorityLandscape)
            .Include(p => p.ProjectRegions).ThenInclude(pr => pr.DNRUplandRegion)
            .Include(p => p.ProjectCounties).ThenInclude(pc => pc.County)
            .Include(p => p.ProjectImages)
            .Include(p => p.ProjectLocations)
            .FirstOrDefaultAsync(p => p.ProjectID == batch.ProjectID);

        if (project == null)
        {
            return new StepDiffResponse { HasChanges = false, DiffHtml = null };
        }

        // Normalize PascalCase (e.g., "ExpectedFunding") to kebab-case ("expected-funding")
        var normalizedKey = Regex.Replace(stepKey, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();

        return normalizedKey switch
        {
            "basics" => await GetBasicsStepDiffAsync(dbContext, project, batch),
            "organizations" => await GetOrganizationsStepDiffAsync(dbContext, project, batch),
            "contacts" => await GetContactsStepDiffAsync(dbContext, project, batch),
            "expected-funding" => await GetExpectedFundingStepDiffAsync(dbContext, project, batch),
            "external-links" => await GetExternalLinksStepDiffAsync(dbContext, project, batch),
            "documents-notes" => await GetDocumentsNotesStepDiffAsync(dbContext, project, batch),
            "location-simple" => await GetLocationSimpleStepDiffAsync(dbContext, project, batch),
            "location-detailed" => await GetLocationDetailedStepDiffAsync(dbContext, project, batch),
            "photos" => await GetPhotosStepDiffAsync(dbContext, project, batch),
            "priority-landscapes" => await GetPriorityLandscapesStepDiffAsync(dbContext, project, batch),
            "dnr-upland-regions" => await GetDnrUplandRegionsStepDiffAsync(dbContext, project, batch),
            "counties" => await GetCountiesStepDiffAsync(dbContext, project, batch),
            "treatments" => await GetTreatmentsStepDiffAsync(dbContext, project, batch),
            _ => new StepDiffResponse { HasChanges = false, DiffHtml = null }
        };
    }

    private static async Task<StepDiffResponse> GetBasicsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var projectUpdate = await dbContext.ProjectUpdates
            .AsNoTracking()
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        if (projectUpdate == null) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var originalHtml = RenderBasicsHtml(project);
        var updatedHtml = RenderBasicsUpdateHtml(projectUpdate, project.ProjectName);

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetOrganizationsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateOrgs = await dbContext.ProjectOrganizationUpdates
            .AsNoTracking()
            .Include(po => po.Organization)
            .Include(po => po.RelationshipType)
            .Where(po => po.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var originalHtml = RenderOrganizationsHtml(project.ProjectOrganizations.ToList());
        var updatedHtml = RenderOrganizationUpdatesHtml(updateOrgs);

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetContactsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateContacts = await dbContext.ProjectPersonUpdates
            .AsNoTracking()
            .Include(pp => pp.Person)
            .Where(pp => pp.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Contact</th><th>Relationship</th></tr>");
        foreach (var pp in project.ProjectPeople.OrderBy(p => p.Person?.LastName).ThenBy(p => p.Person?.FirstName))
        {
            var name = pp.Person != null ? $"{pp.Person.FirstName} {pp.Person.LastName}" : "(unknown)";
            var relType = ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(pp.ProjectPersonRelationshipTypeID, out var rt) ? rt.ProjectPersonRelationshipTypeDisplayName : "(unknown)";
            AppendRow(sb, name, relType);
        }
        if (!project.ProjectPeople.Any()) sb.AppendLine("<tr><td colspan='2'><em>No contacts</em></td></tr>");
        sb.AppendLine("</table>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Contact</th><th>Relationship</th></tr>");
        foreach (var pp in updateContacts.OrderBy(p => p.Person?.LastName).ThenBy(p => p.Person?.FirstName))
        {
            var name = pp.Person != null ? $"{pp.Person.FirstName} {pp.Person.LastName}" : "(unknown)";
            var relType = ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(pp.ProjectPersonRelationshipTypeID, out var rt) ? rt.ProjectPersonRelationshipTypeDisplayName : "(unknown)";
            AppendRow(sb, name, relType);
        }
        if (!updateContacts.Any()) sb.AppendLine("<tr><td colspan='2'><em>No contacts</em></td></tr>");
        sb.AppendLine("</table>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetExpectedFundingStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateFunding = await dbContext.ProjectFundingSourceUpdates
            .AsNoTracking()
            .Where(f => f.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectUpdate = await dbContext.ProjectUpdates
            .AsNoTracking()
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Field</th><th>Value</th></tr>");
        AppendRow(sb, "Estimated Total Cost", project.EstimatedTotalCost?.ToString("C") ?? "(none)");
        AppendRow(sb, "Funding Source Notes", project.ProjectFundingSourceNotes ?? "(none)");
        sb.AppendLine("</table>");
        sb.AppendLine("<h5>Funding Sources</h5>");
        sb.AppendLine("<ul>");
        foreach (var f in project.ProjectFundingSources.OrderBy(f => GetFundingSourceName(f.FundingSourceID)))
        {
            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(GetFundingSourceName(f.FundingSourceID))}</li>");
        }
        if (!project.ProjectFundingSources.Any()) sb.AppendLine("<li><em>No funding sources</em></li>");
        sb.AppendLine("</ul>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Field</th><th>Value</th></tr>");
        AppendRow(sb, "Estimated Total Cost", projectUpdate?.EstimatedTotalCost?.ToString("C") ?? "(none)");
        AppendRow(sb, "Funding Source Notes", projectUpdate?.ProjectFundingSourceNotes ?? "(none)");
        sb.AppendLine("</table>");
        sb.AppendLine("<h5>Funding Sources</h5>");
        sb.AppendLine("<ul>");
        foreach (var f in updateFunding.OrderBy(f => GetFundingSourceName(f.FundingSourceID)))
        {
            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(GetFundingSourceName(f.FundingSourceID))}</li>");
        }
        if (!updateFunding.Any()) sb.AppendLine("<li><em>No funding sources</em></li>");
        sb.AppendLine("</ul>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetExternalLinksStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateLinks = await dbContext.ProjectExternalLinkUpdates
            .AsNoTracking()
            .Where(l => l.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var originalHtml = RenderExternalLinksHtml(project.ProjectExternalLinks.ToList());
        var updatedHtml = RenderExternalLinkUpdatesHtml(updateLinks);

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetDocumentsNotesStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateDocs = await dbContext.ProjectDocumentUpdates
            .AsNoTracking()
            .Where(d => d.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        var updateNotes = await dbContext.ProjectNoteUpdates
            .AsNoTracking()
            .Where(n => n.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("<h5>Documents</h5>");
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Title</th><th>Description</th></tr>");
        foreach (var doc in project.ProjectDocuments.OrderBy(d => d.DisplayName))
        {
            AppendRow(sb, doc.DisplayName ?? "(untitled)", doc.Description ?? "(no description)");
        }
        if (!project.ProjectDocuments.Any()) sb.AppendLine("<tr><td colspan='2'><em>No documents</em></td></tr>");
        sb.AppendLine("</table>");
        sb.AppendLine(RenderNotesHtml(project.ProjectNotes.ToList()));
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<h5>Documents</h5>");
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Title</th><th>Description</th></tr>");
        foreach (var doc in updateDocs.OrderBy(d => d.DisplayName))
        {
            AppendRow(sb, doc.DisplayName ?? "(untitled)", doc.Description ?? "(no description)");
        }
        if (!updateDocs.Any()) sb.AppendLine("<tr><td colspan='2'><em>No documents</em></td></tr>");
        sb.AppendLine("</table>");
        sb.AppendLine(RenderNoteUpdatesHtml(updateNotes));
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetLocationSimpleStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var projectUpdate = await dbContext.ProjectUpdates
            .AsNoTracking()
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        if (projectUpdate == null) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Field</th><th>Value</th></tr>");
        var origLat = project.ProjectLocationPoint?.Coordinate.Y.ToString("F6") ?? "(none)";
        var origLon = project.ProjectLocationPoint?.Coordinate.X.ToString("F6") ?? "(none)";
        var origLocType = ProjectLocationSimpleType.AllLookupDictionary.TryGetValue(project.ProjectLocationSimpleTypeID, out var lt) ? lt.ProjectLocationSimpleTypeName : "(none)";
        AppendRow(sb, "Latitude", origLat);
        AppendRow(sb, "Longitude", origLon);
        AppendRow(sb, "Location Type", origLocType);
        AppendRow(sb, "Notes", project.ProjectLocationNotes ?? "(none)");
        sb.AppendLine("</table>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Field</th><th>Value</th></tr>");
        var updateLat = projectUpdate.ProjectLocationPoint?.Coordinate.Y.ToString("F6") ?? "(none)";
        var updateLon = projectUpdate.ProjectLocationPoint?.Coordinate.X.ToString("F6") ?? "(none)";
        var updateLocType = ProjectLocationSimpleType.AllLookupDictionary.TryGetValue(projectUpdate.ProjectLocationSimpleTypeID, out var ult) ? ult.ProjectLocationSimpleTypeName : "(none)";
        AppendRow(sb, "Latitude", updateLat);
        AppendRow(sb, "Longitude", updateLon);
        AppendRow(sb, "Location Type", updateLocType);
        AppendRow(sb, "Notes", projectUpdate.ProjectLocationNotes ?? "(none)");
        sb.AppendLine("</table>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetLocationDetailedStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateLocs = await dbContext.ProjectLocationUpdates
            .AsNoTracking()
            .Where(l => l.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Name</th><th>Type</th><th>Notes</th></tr>");
        foreach (var loc in project.ProjectLocations.OrderBy(l => l.ProjectLocationName))
        {
            var locType = ProjectLocationType.AllLookupDictionary.TryGetValue(loc.ProjectLocationTypeID, out var lt) ? lt.ProjectLocationTypeDisplayName : "(unknown)";
            sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(loc.ProjectLocationName ?? "(unnamed)")}</td><td>{System.Net.WebUtility.HtmlEncode(locType)}</td><td>{System.Net.WebUtility.HtmlEncode(loc.ProjectLocationNotes ?? "(none)")}</td></tr>");
        }
        if (!project.ProjectLocations.Any()) sb.AppendLine("<tr><td colspan='3'><em>No detailed locations</em></td></tr>");
        sb.AppendLine("</table>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Name</th><th>Type</th><th>Notes</th></tr>");
        foreach (var loc in updateLocs.OrderBy(l => l.ProjectLocationUpdateName))
        {
            var locType = ProjectLocationType.AllLookupDictionary.TryGetValue(loc.ProjectLocationTypeID, out var lt) ? lt.ProjectLocationTypeDisplayName : "(unknown)";
            sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(loc.ProjectLocationUpdateName ?? "(unnamed)")}</td><td>{System.Net.WebUtility.HtmlEncode(locType)}</td><td>{System.Net.WebUtility.HtmlEncode(loc.ProjectLocationUpdateNotes ?? "(none)")}</td></tr>");
        }
        if (!updateLocs.Any()) sb.AppendLine("<tr><td colspan='3'><em>No detailed locations</em></td></tr>");
        sb.AppendLine("</table>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetPhotosStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updatePhotos = await dbContext.ProjectImageUpdates
            .AsNoTracking()
            .Where(i => i.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Caption</th><th>Credit</th><th>Key Photo</th></tr>");
        foreach (var img in project.ProjectImages.OrderBy(i => i.Caption))
        {
            var isKey = img.IsKeyPhoto ? "Yes" : "No";
            AppendRow(sb, img.Caption ?? "(no caption)", img.Credit ?? "(no credit)");
            sb.Replace("</tr>", $"<td>{isKey}</td></tr>");
        }
        if (!project.ProjectImages.Any()) sb.AppendLine("<tr><td colspan='3'><em>No photos</em></td></tr>");
        sb.AppendLine("</table>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Caption</th><th>Credit</th><th>Key Photo</th></tr>");
        foreach (var img in updatePhotos.OrderBy(i => i.Caption))
        {
            var isKey = img.IsKeyPhoto ? "Yes" : "No";
            sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(img.Caption ?? "(no caption)")}</td><td>{System.Net.WebUtility.HtmlEncode(img.Credit ?? "(no credit)")}</td><td>{isKey}</td></tr>");
        }
        if (!updatePhotos.Any()) sb.AppendLine("<tr><td colspan='3'><em>No photos</em></td></tr>");
        sb.AppendLine("</table>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetPriorityLandscapesStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updatePl = await dbContext.ProjectPriorityLandscapeUpdates
            .AsNoTracking()
            .Include(pl => pl.PriorityLandscape)
            .Where(pl => pl.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("<ul>");
        foreach (var pl in project.ProjectPriorityLandscapes.OrderBy(p => p.PriorityLandscape?.PriorityLandscapeName))
        {
            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(pl.PriorityLandscape?.PriorityLandscapeName ?? "(unknown)")}</li>");
        }
        if (!project.ProjectPriorityLandscapes.Any())
        {
            sb.AppendLine($"<li><em>No priority landscapes{(string.IsNullOrWhiteSpace(project.NoPriorityLandscapesExplanation) ? "" : $" - {System.Net.WebUtility.HtmlEncode(project.NoPriorityLandscapesExplanation)}")}</em></li>");
        }
        sb.AppendLine("</ul>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<ul>");
        foreach (var pl in updatePl.OrderBy(p => p.PriorityLandscape?.PriorityLandscapeName))
        {
            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(pl.PriorityLandscape?.PriorityLandscapeName ?? "(unknown)")}</li>");
        }
        if (!updatePl.Any())
        {
            sb.AppendLine($"<li><em>No priority landscapes{(string.IsNullOrWhiteSpace(batch.NoPriorityLandscapesExplanation) ? "" : $" - {System.Net.WebUtility.HtmlEncode(batch.NoPriorityLandscapesExplanation)}")}</em></li>");
        }
        sb.AppendLine("</ul>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetDnrUplandRegionsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateRegions = await dbContext.ProjectRegionUpdates
            .AsNoTracking()
            .Include(r => r.DNRUplandRegion)
            .Where(r => r.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("<ul>");
        foreach (var r in project.ProjectRegions.OrderBy(r => r.DNRUplandRegion?.DNRUplandRegionName))
        {
            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(r.DNRUplandRegion?.DNRUplandRegionName ?? "(unknown)")}</li>");
        }
        if (!project.ProjectRegions.Any())
        {
            sb.AppendLine($"<li><em>No DNR upland regions{(string.IsNullOrWhiteSpace(project.NoRegionsExplanation) ? "" : $" - {System.Net.WebUtility.HtmlEncode(project.NoRegionsExplanation)}")}</em></li>");
        }
        sb.AppendLine("</ul>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<ul>");
        foreach (var r in updateRegions.OrderBy(r => r.DNRUplandRegion?.DNRUplandRegionName))
        {
            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(r.DNRUplandRegion?.DNRUplandRegionName ?? "(unknown)")}</li>");
        }
        if (!updateRegions.Any())
        {
            sb.AppendLine($"<li><em>No DNR upland regions{(string.IsNullOrWhiteSpace(batch.NoRegionsExplanation) ? "" : $" - {System.Net.WebUtility.HtmlEncode(batch.NoRegionsExplanation)}")}</em></li>");
        }
        sb.AppendLine("</ul>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetCountiesStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateCounties = await dbContext.ProjectCountyUpdates
            .AsNoTracking()
            .Include(c => c.County)
            .Where(c => c.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("<ul>");
        foreach (var c in project.ProjectCounties.OrderBy(c => c.County?.CountyName))
        {
            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(c.County?.CountyName ?? "(unknown)")}</li>");
        }
        if (!project.ProjectCounties.Any())
        {
            sb.AppendLine($"<li><em>No counties{(string.IsNullOrWhiteSpace(project.NoCountiesExplanation) ? "" : $" - {System.Net.WebUtility.HtmlEncode(project.NoCountiesExplanation)}")}</em></li>");
        }
        sb.AppendLine("</ul>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<ul>");
        foreach (var c in updateCounties.OrderBy(c => c.County?.CountyName))
        {
            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(c.County?.CountyName ?? "(unknown)")}</li>");
        }
        if (!updateCounties.Any())
        {
            sb.AppendLine($"<li><em>No counties{(string.IsNullOrWhiteSpace(batch.NoCountiesExplanation) ? "" : $" - {System.Net.WebUtility.HtmlEncode(batch.NoCountiesExplanation)}")}</em></li>");
        }
        sb.AppendLine("</ul>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    private static async Task<StepDiffResponse> GetTreatmentsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var projectTreatments = await dbContext.Treatments
            .AsNoTracking()
            .Include(t => t.ProjectLocation)
            .Where(t => t.ProjectLocation.ProjectID == project.ProjectID)
            .ToListAsync();

        var updateTreatments = await dbContext.TreatmentUpdates
            .AsNoTracking()
            .Include(t => t.ProjectLocationUpdate)
            .Where(t => t.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Location</th><th>Type</th><th>Activity</th><th>Acres</th></tr>");
        foreach (var t in projectTreatments.OrderBy(t => t.ProjectLocation?.ProjectLocationName))
        {
            var locName = t.ProjectLocation?.ProjectLocationName ?? "(unknown)";
            var typeName = TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var tt) ? tt.TreatmentTypeDisplayName : "(unknown)";
            var actName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(t.TreatmentDetailedActivityTypeID, out var at) ? at.TreatmentDetailedActivityTypeDisplayName : "(unknown)";
            var acres = t.TreatmentTreatedAcres?.ToString("F2") ?? "(none)";
            sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(locName)}</td><td>{System.Net.WebUtility.HtmlEncode(typeName)}</td><td>{System.Net.WebUtility.HtmlEncode(actName)}</td><td>{acres}</td></tr>");
        }
        if (!projectTreatments.Any()) sb.AppendLine("<tr><td colspan='4'><em>No treatments</em></td></tr>");
        sb.AppendLine("</table>");
        var originalHtml = sb.ToString();

        sb.Clear();
        sb.AppendLine("<table class='diff-table'>");
        sb.AppendLine("<tr><th>Location</th><th>Type</th><th>Activity</th><th>Acres</th></tr>");
        foreach (var t in updateTreatments.OrderBy(t => t.ProjectLocationUpdate?.ProjectLocationUpdateName))
        {
            var locName = t.ProjectLocationUpdate?.ProjectLocationUpdateName ?? "(unknown)";
            var typeName = TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var tt) ? tt.TreatmentTypeDisplayName : "(unknown)";
            var actName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(t.TreatmentDetailedActivityTypeID, out var at) ? at.TreatmentDetailedActivityTypeDisplayName : "(unknown)";
            var acres = t.TreatmentTreatedAcres?.ToString("F2") ?? "(none)";
            sb.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(locName)}</td><td>{System.Net.WebUtility.HtmlEncode(typeName)}</td><td>{System.Net.WebUtility.HtmlEncode(actName)}</td><td>{acres}</td></tr>");
        }
        if (!updateTreatments.Any()) sb.AppendLine("<tr><td colspan='4'><em>No treatments</em></td></tr>");
        sb.AppendLine("</table>");
        var updatedHtml = sb.ToString();

        if (originalHtml == updatedHtml) return new StepDiffResponse { HasChanges = false, DiffHtml = null };

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return new StepDiffResponse { HasChanges = true, DiffHtml = diff.Build() };
    }

    #endregion

    #region DTO for API Response

    /// <summary>
    /// Returns a summary of all diffs for display in the UI
    /// </summary>
    public static async Task<ProjectUpdateDiffSummary> GetDiffSummaryAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
        {
            return new ProjectUpdateDiffSummary();
        }

        return new ProjectUpdateDiffSummary
        {
            BasicsDiffHtml = batch.BasicsDiffLog,
            OrganizationsDiffHtml = batch.OrganizationsDiffLog,
            ExternalLinksDiffHtml = batch.ExternalLinksDiffLog,
            NotesDiffHtml = batch.NotesDiffLog,
            ExpectedFundingDiffHtml = batch.ExpectedFundingDiffLog,
            HasBasicsChanges = !string.IsNullOrEmpty(batch.BasicsDiffLog),
            HasOrganizationsChanges = !string.IsNullOrEmpty(batch.OrganizationsDiffLog),
            HasExternalLinksChanges = !string.IsNullOrEmpty(batch.ExternalLinksDiffLog),
            HasNotesChanges = !string.IsNullOrEmpty(batch.NotesDiffLog),
            HasExpectedFundingChanges = !string.IsNullOrEmpty(batch.ExpectedFundingDiffLog)
        };
    }

    #endregion
}

/// <summary>
/// Response for a single step's diff check
/// </summary>
public class StepDiffResponse
{
    public bool HasChanges { get; set; }
    public string? DiffHtml { get; set; }
}

/// <summary>
/// Summary of all diffs for a ProjectUpdateBatch
/// </summary>
public class ProjectUpdateDiffSummary
{
    public string? BasicsDiffHtml { get; set; }
    public string? OrganizationsDiffHtml { get; set; }
    public string? ExternalLinksDiffHtml { get; set; }
    public string? NotesDiffHtml { get; set; }
    public string? ExpectedFundingDiffHtml { get; set; }

    public bool HasBasicsChanges { get; set; }
    public bool HasOrganizationsChanges { get; set; }
    public bool HasExternalLinksChanges { get; set; }
    public bool HasNotesChanges { get; set; }
    public bool HasExpectedFundingChanges { get; set; }

    public bool HasAnyChanges => HasBasicsChanges || HasOrganizationsChanges ||
                                  HasExternalLinksChanges || HasNotesChanges ||
                                  HasExpectedFundingChanges;
}
