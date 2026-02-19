using System.Globalization;
using System.Text.Json;
using HtmlDiff;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

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
        // Load project scalars only — each Generate*DiffAsync sub-method loads
        // only the specific collection(s) it needs, avoiding a 7-include join.
        var project = await dbContext.Projects
            .FirstOrDefaultAsync(p => p.ProjectID == batch.ProjectID);

        if (project == null) return;

        // Load the update data
        var projectUpdate = await dbContext.ProjectUpdates
            .Include(pu => pu.FocusArea)
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        // Generate each section's diff (legacy HTML)
        batch.BasicsDiffLog = await GenerateBasicsDiffAsync(dbContext, project, projectUpdate, batch);
        batch.OrganizationsDiffLog = await GenerateOrganizationsDiffAsync(dbContext, project, batch);
        batch.ExternalLinksDiffLog = await GenerateExternalLinksDiffAsync(dbContext, project, batch);
        batch.NotesDiffLog = await GenerateNotesDiffAsync(dbContext, project, batch);
        batch.ExpectedFundingDiffLog = await GenerateExpectedFundingDiffAsync(dbContext, project, batch);

        // Store structured JSON diffs (covers all 13 steps)
        var structuredDiffs = await GetAllStepDiffsAsync(dbContext, batch.ProjectUpdateBatchID);
        batch.StructuredDiffLogJson = JsonSerializer.Serialize(structuredDiffs);
    }

    #region Basics Diff

    private static async Task<string?> GenerateBasicsDiffAsync(
        WADNRDbContext dbContext,
        Project project,
        ProjectUpdate? projectUpdate,
        ProjectUpdateBatch batch)
    {
        if (projectUpdate == null) return null;

        // Resolve FocusArea names via projection
        var originalFocusAreaName = project.FocusAreaID.HasValue
            ? await dbContext.FocusAreas.AsNoTracking()
                .Where(f => f.FocusAreaID == project.FocusAreaID.Value)
                .Select(f => f.FocusAreaName).FirstOrDefaultAsync()
            : null;
        var updateFocusAreaName = projectUpdate.FocusAreaID.HasValue
            ? await dbContext.FocusAreas.AsNoTracking()
                .Where(f => f.FocusAreaID == projectUpdate.FocusAreaID.Value)
                .Select(f => f.FocusAreaName).FirstOrDefaultAsync()
            : null;

        // Resolve Lead Implementer via projection
        var originalLeadName = await dbContext.ProjectOrganizations.AsNoTracking()
            .Where(po => po.ProjectID == project.ProjectID && po.RelationshipType.IsPrimaryContact)
            .Select(po => po.Organization.OrganizationName)
            .FirstOrDefaultAsync();
        var updateLeadName = await dbContext.ProjectOrganizationUpdates.AsNoTracking()
            .Where(po => po.ProjectUpdateBatchID == batch.ProjectUpdateBatchID && po.RelationshipType.IsPrimaryContact)
            .Select(po => po.Organization.OrganizationName)
            .FirstOrDefaultAsync();

        // Resolve Programs via projection
        var originalPrograms = await dbContext.ProjectPrograms.AsNoTracking()
            .Where(pp => pp.ProjectID == project.ProjectID)
            .Select(pp => pp.Program.ProgramName)
            .Where(n => n != null)
            .OrderBy(n => n)
            .ToListAsync();
        var updatePrograms = await dbContext.ProjectUpdatePrograms.AsNoTracking()
            .Where(pp => pp.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .Select(pp => pp.Program.ProgramName)
            .Where(n => n != null)
            .OrderBy(n => n)
            .ToListAsync();

        var originalHtml = RenderBasicsHtml(project, originalFocusAreaName, originalLeadName, originalPrograms);
        var updatedHtml = RenderBasicsUpdateHtml(projectUpdate, project.ProjectName, updateFocusAreaName, updateLeadName, updatePrograms);

        if (originalHtml == updatedHtml) return null;

        var diff = new HtmlDiff.HtmlDiff(originalHtml, updatedHtml);
        return diff.Build();
    }

    private static string RenderBasicsHtml(Project project, string? focusAreaName, string? leadImplementerName, List<string?> programNames)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");

        AppendRow(sb, "Project Stage", ProjectStage.AllLookupDictionary.TryGetValue(project.ProjectStageID, out var stage) ? stage.ProjectStageDisplayName : "");
        AppendRow(sb, "Lead Implementer Organization", leadImplementerName ?? "(none)");
        AppendRow(sb, "Project Initiation date", project.PlannedDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Completion Date", project.CompletionDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Project Description", project.ProjectDescription ?? "");
        AppendRow(sb, "Expiration Date", project.ExpirationDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Percentage Match", project.PercentageMatch?.ToString() ?? "");
        AppendRow(sb, "Focus Area", focusAreaName ?? "(none)");
        AppendRow(sb, "Programs", programNames.Any() ? string.Join(", ", programNames) : "(none)");

        sb.AppendLine("</table>");
        return sb.ToString();
    }

    private static string RenderBasicsUpdateHtml(ProjectUpdate projectUpdate, string projectName, string? focusAreaName, string? leadImplementerName, List<string?> programNames)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table class='diff-table'>");

        AppendRow(sb, "Project Stage", ProjectStage.AllLookupDictionary.TryGetValue(projectUpdate.ProjectStageID, out var stage) ? stage.ProjectStageDisplayName : "");
        AppendRow(sb, "Lead Implementer Organization", leadImplementerName ?? "(none)");
        AppendRow(sb, "Project Initiation date", projectUpdate.PlannedDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Completion Date", projectUpdate.CompletionDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Project Description", projectUpdate.ProjectDescription ?? "");
        AppendRow(sb, "Expiration Date", projectUpdate.ExpirationDate?.ToString("M/d/yyyy") ?? "");
        AppendRow(sb, "Percentage Match", projectUpdate.PercentageMatch?.ToString() ?? "");
        AppendRow(sb, "Focus Area", focusAreaName ?? "(none)");
        AppendRow(sb, "Programs", programNames.Any() ? string.Join(", ", programNames) : "(none)");

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

        var projectOrgs = await dbContext.ProjectOrganizations
            .Include(po => po.Organization)
            .Include(po => po.RelationshipType)
            .Where(po => po.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalHtml = RenderOrganizationsHtml(projectOrgs);
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

        var projectLinks = await dbContext.ProjectExternalLinks
            .Where(l => l.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalHtml = RenderExternalLinksHtml(projectLinks);
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

        var projectNotes = await dbContext.ProjectNotes
            .Where(n => n.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalHtml = RenderNotesHtml(projectNotes);
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

        var projectFunding = await dbContext.ProjectFundingSources
            .Where(f => f.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalHtml = RenderFundingHtml(projectFunding);
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
    /// All step keys in kebab-case order for bulk diff generation.
    /// </summary>
    private static readonly string[] AllStepKeys =
    [
        "basics", "organizations", "contacts", "expected-funding", "external-links",
        "documents-notes", "location-simple", "location-detailed", "photos",
        "priority-landscapes", "dnr-upland-regions", "counties", "treatments"
    ];

    /// <summary>
    /// Generates structured diffs for ALL 13 steps at once.
    /// Loads project + batch once to avoid 13 redundant DB round-trips.
    /// Returns a dictionary keyed by kebab-case step name.
    /// </summary>
    public static async Task<Dictionary<string, StepDiffResponse>> GetAllStepDiffsAsync(
        WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
        {
            return AllStepKeys.ToDictionary(k => k, _ => new StepDiffResponse { HasChanges = false });
        }

        // Load project scalars only — each Get*StepDiffAsync sub-method loads
        // only the specific collection(s) it needs, avoiding a 13-include Cartesian product.
        var project = await dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProjectID == batch.ProjectID);

        if (project == null)
        {
            return AllStepKeys.ToDictionary(k => k, _ => new StepDiffResponse { HasChanges = false });
        }

        var result = new Dictionary<string, StepDiffResponse>();
        foreach (var key in AllStepKeys)
        {
            result[key] = key switch
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
                _ => new StepDiffResponse { HasChanges = false }
            };
        }

        return result;
    }

    /// <summary>
    /// Gets real-time diff for a specific step by comparing update batch data to current project data.
    /// Returns structured sections describing the changes.
    /// </summary>
    public static async Task<StepDiffResponse> GetStepDiffAsync(WADNRDbContext dbContext, int projectUpdateBatchID, string stepKey)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
        {
            return new StepDiffResponse { HasChanges = false };
        }

        // Load project scalars only — the per-step diff method loads its own collection(s).
        var project = await dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProjectID == batch.ProjectID);

        if (project == null)
        {
            return new StepDiffResponse { HasChanges = false };
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
            _ => new StepDiffResponse { HasChanges = false }
        };
    }

    private static async Task<StepDiffResponse> GetBasicsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var projectUpdate = await dbContext.ProjectUpdates
            .AsNoTracking()
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        if (projectUpdate == null) return new StepDiffResponse { HasChanges = false };

        var originalFocusAreaName = project.FocusAreaID.HasValue
            ? await dbContext.FocusAreas.AsNoTracking()
                .Where(f => f.FocusAreaID == project.FocusAreaID.Value)
                .Select(f => f.FocusAreaName).FirstOrDefaultAsync()
            : null;
        var updateFocusAreaName = projectUpdate.FocusAreaID.HasValue
            ? await dbContext.FocusAreas.AsNoTracking()
                .Where(f => f.FocusAreaID == projectUpdate.FocusAreaID.Value)
                .Select(f => f.FocusAreaName).FirstOrDefaultAsync()
            : null;

        var originalLeadName = await dbContext.ProjectOrganizations.AsNoTracking()
            .Where(po => po.ProjectID == project.ProjectID && po.RelationshipType.IsPrimaryContact)
            .Select(po => po.Organization.OrganizationName)
            .FirstOrDefaultAsync();
        var updateLeadName = await dbContext.ProjectOrganizationUpdates.AsNoTracking()
            .Where(po => po.ProjectUpdateBatchID == batch.ProjectUpdateBatchID && po.RelationshipType.IsPrimaryContact)
            .Select(po => po.Organization.OrganizationName)
            .FirstOrDefaultAsync();

        var originalPrograms = await dbContext.ProjectPrograms.AsNoTracking()
            .Where(pp => pp.ProjectID == project.ProjectID)
            .Select(pp => pp.Program.ProgramName)
            .Where(n => n != null)
            .OrderBy(n => n)
            .ToListAsync();
        var updatePrograms = await dbContext.ProjectUpdatePrograms.AsNoTracking()
            .Where(pp => pp.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .Select(pp => pp.Program.ProgramName)
            .Where(n => n != null)
            .OrderBy(n => n)
            .ToListAsync();

        var origStageName = ProjectStage.AllLookupDictionary.TryGetValue(project.ProjectStageID, out var stage) ? stage.ProjectStageDisplayName : "";
        var updateStageName = ProjectStage.AllLookupDictionary.TryGetValue(projectUpdate.ProjectStageID, out var uStage) ? uStage.ProjectStageDisplayName : "";

        var fields = new List<DiffField>
        {
            new() { Label = "Project Stage", OriginalValue = origStageName, UpdatedValue = updateStageName },
            new() { Label = "Lead Implementer Organization", OriginalValue = originalLeadName ?? "(none)", UpdatedValue = updateLeadName ?? "(none)" },
            new() { Label = "Project Initiation Date", OriginalValue = project.PlannedDate?.ToString("M/d/yyyy") ?? "", UpdatedValue = projectUpdate.PlannedDate?.ToString("M/d/yyyy") ?? "" },
            new() { Label = "Completion Date", OriginalValue = project.CompletionDate?.ToString("M/d/yyyy") ?? "", UpdatedValue = projectUpdate.CompletionDate?.ToString("M/d/yyyy") ?? "" },
            new() { Label = "Project Description", OriginalValue = project.ProjectDescription ?? "", UpdatedValue = projectUpdate.ProjectDescription ?? "" },
            new() { Label = "Expiration Date", OriginalValue = project.ExpirationDate?.ToString("M/d/yyyy") ?? "", UpdatedValue = projectUpdate.ExpirationDate?.ToString("M/d/yyyy") ?? "" },
            new() { Label = "Percentage Match", OriginalValue = project.PercentageMatch?.ToString() ?? "", UpdatedValue = projectUpdate.PercentageMatch?.ToString() ?? "" },
            new() { Label = "Focus Area", OriginalValue = originalFocusAreaName ?? "(none)", UpdatedValue = updateFocusAreaName ?? "(none)" },
            new() { Label = "Programs", OriginalValue = originalPrograms.Any() ? string.Join(", ", originalPrograms) : "(none)", UpdatedValue = updatePrograms.Any() ? string.Join(", ", updatePrograms) : "(none)" },
        };

        var hasChanges = fields.Any(f => f.OriginalValue != f.UpdatedValue);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections = [new DiffSection { Type = "fields", Fields = fields }]
        };
    }

    private static async Task<StepDiffResponse> GetOrganizationsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateOrgs = await dbContext.ProjectOrganizationUpdates
            .AsNoTracking()
            .Include(po => po.Organization)
            .Include(po => po.RelationshipType)
            .Where(po => po.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectOrgs = await dbContext.ProjectOrganizations
            .AsNoTracking()
            .Include(po => po.Organization)
            .Include(po => po.RelationshipType)
            .Where(po => po.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalRows = projectOrgs
            .OrderBy(o => o.RelationshipType?.RelationshipTypeName).ThenBy(o => o.Organization?.OrganizationName)
            .Select(o => new List<string> { o.Organization?.OrganizationName ?? "(unknown)", o.RelationshipType?.RelationshipTypeName ?? "(unknown)" })
            .ToList();

        var updatedRows = updateOrgs
            .OrderBy(o => o.RelationshipType?.RelationshipTypeName).ThenBy(o => o.Organization?.OrganizationName)
            .Select(o => new List<string> { o.Organization?.OrganizationName ?? "(unknown)", o.RelationshipType?.RelationshipTypeName ?? "(unknown)" })
            .ToList();

        var hasChanges = !RowsEqual(originalRows, updatedRows);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "table",
                    Headers = ["Organization", "Relationship Type"],
                    OriginalRows = originalRows,
                    UpdatedRows = updatedRows
                }
            ]
        };
    }

    private static async Task<StepDiffResponse> GetContactsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateContacts = await dbContext.ProjectPersonUpdates
            .AsNoTracking()
            .Include(pp => pp.Person)
            .Where(pp => pp.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectPeople = await dbContext.ProjectPeople
            .AsNoTracking()
            .Include(pp => pp.Person)
            .Where(pp => pp.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalRows = projectPeople
            .OrderBy(p => p.Person?.LastName).ThenBy(p => p.Person?.FirstName)
            .Select(p =>
            {
                var name = p.Person != null ? $"{p.Person.FirstName} {p.Person.LastName}" : "(unknown)";
                var relType = ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(p.ProjectPersonRelationshipTypeID, out var rt) ? rt.ProjectPersonRelationshipTypeDisplayName : "(unknown)";
                return new List<string> { name, relType };
            })
            .ToList();

        var updatedRows = updateContacts
            .OrderBy(p => p.Person?.LastName).ThenBy(p => p.Person?.FirstName)
            .Select(p =>
            {
                var name = p.Person != null ? $"{p.Person.FirstName} {p.Person.LastName}" : "(unknown)";
                var relType = ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(p.ProjectPersonRelationshipTypeID, out var rt) ? rt.ProjectPersonRelationshipTypeDisplayName : "(unknown)";
                return new List<string> { name, relType };
            })
            .ToList();

        var hasChanges = !RowsEqual(originalRows, updatedRows);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "table",
                    Headers = ["Contact Name", "Relationship"],
                    OriginalRows = originalRows,
                    UpdatedRows = updatedRows
                }
            ]
        };
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

        var allocations = await dbContext.ProjectFundSourceAllocationRequests
            .AsNoTracking()
            .Include(a => a.FundSourceAllocation)
            .Where(a => a.ProjectID == project.ProjectID)
            .ToListAsync();

        var updateAllocations = await dbContext.ProjectFundSourceAllocationRequestUpdates
            .AsNoTracking()
            .Include(a => a.FundSourceAllocation)
            .Where(a => a.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var fields = new List<DiffField>
        {
            new()
            {
                Label = "Estimated Total Cost",
                OriginalValue = project.EstimatedTotalCost?.ToString("C", CultureInfo.GetCultureInfo("en-US")) ?? "(none)",
                UpdatedValue = projectUpdate?.EstimatedTotalCost?.ToString("C", CultureInfo.GetCultureInfo("en-US")) ?? "(none)"
            },
            new()
            {
                Label = "Funding Source Notes",
                OriginalValue = project.ProjectFundingSourceNotes ?? "(none)",
                UpdatedValue = projectUpdate?.ProjectFundingSourceNotes ?? "(none)"
            }
        };

        var projectFundingSources = await dbContext.ProjectFundingSources
            .AsNoTracking()
            .Where(f => f.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalFundingSources = projectFundingSources
            .OrderBy(f => GetFundingSourceName(f.FundingSourceID))
            .Select(f => GetFundingSourceName(f.FundingSourceID))
            .ToList();
        var updatedFundingSources = updateFunding
            .OrderBy(f => GetFundingSourceName(f.FundingSourceID))
            .Select(f => GetFundingSourceName(f.FundingSourceID))
            .ToList();

        var originalAllocRows = allocations
            .OrderBy(a => a.FundSourceAllocation?.FundSourceAllocationName)
            .Select(a => new List<string>
            {
                a.FundSourceAllocation?.FundSourceAllocationName ?? "(unknown)",
                a.TotalAmount?.ToString("C", CultureInfo.GetCultureInfo("en-US")) ?? "(none)"
            })
            .ToList();
        var updatedAllocRows = updateAllocations
            .OrderBy(a => a.FundSourceAllocation?.FundSourceAllocationName)
            .Select(a => new List<string>
            {
                a.FundSourceAllocation?.FundSourceAllocationName ?? "(unknown)",
                a.TotalAmount?.ToString("C", CultureInfo.GetCultureInfo("en-US")) ?? "(none)"
            })
            .ToList();

        var hasChanges = fields.Any(f => f.OriginalValue != f.UpdatedValue)
            || !originalFundingSources.SequenceEqual(updatedFundingSources)
            || !RowsEqual(originalAllocRows, updatedAllocRows);

        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection { Type = "fields", Fields = fields },
                new DiffSection
                {
                    Title = "Funding Sources",
                    Type = "list",
                    OriginalItems = originalFundingSources,
                    UpdatedItems = updatedFundingSources
                },
                new DiffSection
                {
                    Title = "Fund Source Allocations",
                    Type = "table",
                    Headers = ["Allocation", "Total"],
                    OriginalRows = originalAllocRows,
                    UpdatedRows = updatedAllocRows
                }
            ]
        };
    }

    private static async Task<StepDiffResponse> GetExternalLinksStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateLinks = await dbContext.ProjectExternalLinkUpdates
            .AsNoTracking()
            .Where(l => l.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectLinks = await dbContext.ProjectExternalLinks
            .AsNoTracking()
            .Where(l => l.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalRows = projectLinks
            .OrderBy(l => l.ExternalLinkLabel)
            .Select(l => new List<string> { l.ExternalLinkLabel ?? "(no label)", l.ExternalLinkUrl ?? "(no url)" })
            .ToList();

        var updatedRows = updateLinks
            .OrderBy(l => l.ExternalLinkLabel)
            .Select(l => new List<string> { l.ExternalLinkLabel ?? "(no label)", l.ExternalLinkUrl ?? "(no url)" })
            .ToList();

        var hasChanges = !RowsEqual(originalRows, updatedRows);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "table",
                    Headers = ["Label", "URL"],
                    OriginalRows = originalRows,
                    UpdatedRows = updatedRows
                }
            ]
        };
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

        var projectDocs = await dbContext.ProjectDocuments
            .AsNoTracking()
            .Where(d => d.ProjectID == project.ProjectID)
            .ToListAsync();
        var projectNotes = await dbContext.ProjectNotes
            .AsNoTracking()
            .Where(n => n.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalDocRows = projectDocs
            .OrderBy(d => d.DisplayName)
            .Select(d => new List<string> { d.DisplayName ?? "(untitled)", d.Description ?? "(no description)" })
            .ToList();
        var updatedDocRows = updateDocs
            .OrderBy(d => d.DisplayName)
            .Select(d => new List<string> { d.DisplayName ?? "(untitled)", d.Description ?? "(no description)" })
            .ToList();

        var originalNoteRows = projectNotes
            .OrderBy(n => n.CreateDate)
            .Select(n => new List<string> { n.Note ?? "(empty)", n.CreateDate.ToString("g") })
            .ToList();
        var updatedNoteRows = updateNotes
            .OrderBy(n => n.CreateDate)
            .Select(n => new List<string> { n.Note ?? "(empty)", n.CreateDate.ToString("g") })
            .ToList();

        var hasChanges = !RowsEqual(originalDocRows, updatedDocRows) || !RowsEqual(originalNoteRows, updatedNoteRows);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Title = "Documents",
                    Type = "table",
                    Headers = ["Title", "Description"],
                    OriginalRows = originalDocRows,
                    UpdatedRows = updatedDocRows
                },
                new DiffSection
                {
                    Title = "Notes",
                    Type = "table",
                    Headers = ["Note", "Created"],
                    OriginalRows = originalNoteRows,
                    UpdatedRows = updatedNoteRows
                }
            ]
        };
    }

    private static async Task<StepDiffResponse> GetLocationSimpleStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var projectUpdate = await dbContext.ProjectUpdates
            .AsNoTracking()
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        if (projectUpdate == null) return new StepDiffResponse { HasChanges = false };

        var origLat = project.ProjectLocationPoint?.Coordinate.Y.ToString("F4") ?? "(none)";
        var origLon = project.ProjectLocationPoint?.Coordinate.X.ToString("F4") ?? "(none)";
        var origLocType = ProjectLocationSimpleType.AllLookupDictionary.TryGetValue(project.ProjectLocationSimpleTypeID, out var lt) ? lt.ProjectLocationSimpleTypeName : "(none)";

        var updateLat = projectUpdate.ProjectLocationPoint?.Coordinate.Y.ToString("F4") ?? "(none)";
        var updateLon = projectUpdate.ProjectLocationPoint?.Coordinate.X.ToString("F4") ?? "(none)";
        var updateLocType = ProjectLocationSimpleType.AllLookupDictionary.TryGetValue(projectUpdate.ProjectLocationSimpleTypeID, out var ult) ? ult.ProjectLocationSimpleTypeName : "(none)";

        var fields = new List<DiffField>
        {
            new() { Label = "Latitude", OriginalValue = origLat, UpdatedValue = updateLat },
            new() { Label = "Longitude", OriginalValue = origLon, UpdatedValue = updateLon },
            new() { Label = "Location Type", OriginalValue = origLocType, UpdatedValue = updateLocType },
            new() { Label = "Notes", OriginalValue = project.ProjectLocationNotes ?? "(none)", UpdatedValue = projectUpdate.ProjectLocationNotes ?? "(none)" },
        };

        var hasChanges = fields.Any(f => f.OriginalValue != f.UpdatedValue);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections = [new DiffSection { Type = "fields", Fields = fields }]
        };
    }

    private static async Task<StepDiffResponse> GetLocationDetailedStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateLocs = await dbContext.ProjectLocationUpdates
            .AsNoTracking()
            .Where(l => l.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectLocations = await dbContext.ProjectLocations
            .AsNoTracking()
            .Where(l => l.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalRows = projectLocations
            .OrderBy(l => l.ProjectLocationName)
            .Select(l =>
            {
                var locType = ProjectLocationType.AllLookupDictionary.TryGetValue(l.ProjectLocationTypeID, out var lt) ? lt.ProjectLocationTypeDisplayName : "(unknown)";
                return new List<string> { l.ProjectLocationName ?? "(unnamed)", locType, l.ProjectLocationNotes ?? "(none)" };
            })
            .ToList();

        var updatedRows = updateLocs
            .OrderBy(l => l.ProjectLocationUpdateName)
            .Select(l =>
            {
                var locType = ProjectLocationType.AllLookupDictionary.TryGetValue(l.ProjectLocationTypeID, out var lt) ? lt.ProjectLocationTypeDisplayName : "(unknown)";
                return new List<string> { l.ProjectLocationUpdateName ?? "(unnamed)", locType, l.ProjectLocationUpdateNotes ?? "(none)" };
            })
            .ToList();

        var hasChanges = !RowsEqual(originalRows, updatedRows);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "table",
                    Headers = ["Name", "Type", "Notes"],
                    OriginalRows = originalRows,
                    UpdatedRows = updatedRows
                }
            ]
        };
    }

    private static async Task<StepDiffResponse> GetPhotosStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updatePhotos = await dbContext.ProjectImageUpdates
            .AsNoTracking()
            .Where(i => i.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectImages = await dbContext.ProjectImages
            .AsNoTracking()
            .Where(i => i.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalRows = projectImages
            .OrderBy(i => i.Caption)
            .Select(i => new List<string> { i.Caption ?? "(no caption)", i.Credit ?? "(no credit)", i.IsKeyPhoto ? "Yes" : "No" })
            .ToList();

        var updatedRows = updatePhotos
            .OrderBy(i => i.Caption)
            .Select(i => new List<string> { i.Caption ?? "(no caption)", i.Credit ?? "(no credit)", i.IsKeyPhoto ? "Yes" : "No" })
            .ToList();

        var hasChanges = !RowsEqual(originalRows, updatedRows);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "table",
                    Headers = ["Caption", "Credit", "Key Photo"],
                    OriginalRows = originalRows,
                    UpdatedRows = updatedRows
                }
            ]
        };
    }

    private static async Task<StepDiffResponse> GetPriorityLandscapesStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updatePl = await dbContext.ProjectPriorityLandscapeUpdates
            .AsNoTracking()
            .Include(pl => pl.PriorityLandscape)
            .Where(pl => pl.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectPl = await dbContext.ProjectPriorityLandscapes
            .AsNoTracking()
            .Include(pl => pl.PriorityLandscape)
            .Where(pl => pl.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalItems = projectPl
            .OrderBy(p => p.PriorityLandscape?.PriorityLandscapeName)
            .Select(p => p.PriorityLandscape?.PriorityLandscapeName ?? "(unknown)")
            .ToList();

        var updatedItems = updatePl
            .OrderBy(p => p.PriorityLandscape?.PriorityLandscapeName)
            .Select(p => p.PriorityLandscape?.PriorityLandscapeName ?? "(unknown)")
            .ToList();

        var hasChanges = !originalItems.SequenceEqual(updatedItems);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "list",
                    OriginalItems = originalItems,
                    UpdatedItems = updatedItems
                }
            ]
        };
    }

    private static async Task<StepDiffResponse> GetDnrUplandRegionsStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateRegions = await dbContext.ProjectRegionUpdates
            .AsNoTracking()
            .Include(r => r.DNRUplandRegion)
            .Where(r => r.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectRegions = await dbContext.ProjectRegions
            .AsNoTracking()
            .Include(r => r.DNRUplandRegion)
            .Where(r => r.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalItems = projectRegions
            .OrderBy(r => r.DNRUplandRegion?.DNRUplandRegionName)
            .Select(r => r.DNRUplandRegion?.DNRUplandRegionName ?? "(unknown)")
            .ToList();

        var updatedItems = updateRegions
            .OrderBy(r => r.DNRUplandRegion?.DNRUplandRegionName)
            .Select(r => r.DNRUplandRegion?.DNRUplandRegionName ?? "(unknown)")
            .ToList();

        var hasChanges = !originalItems.SequenceEqual(updatedItems);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "list",
                    OriginalItems = originalItems,
                    UpdatedItems = updatedItems
                }
            ]
        };
    }

    private static async Task<StepDiffResponse> GetCountiesStepDiffAsync(WADNRDbContext dbContext, Project project, ProjectUpdateBatch batch)
    {
        var updateCounties = await dbContext.ProjectCountyUpdates
            .AsNoTracking()
            .Include(c => c.County)
            .Where(c => c.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectCounties = await dbContext.ProjectCounties
            .AsNoTracking()
            .Include(c => c.County)
            .Where(c => c.ProjectID == project.ProjectID)
            .ToListAsync();

        var originalItems = projectCounties
            .OrderBy(c => c.County?.CountyName)
            .Select(c => c.County?.CountyName ?? "(unknown)")
            .ToList();

        var updatedItems = updateCounties
            .OrderBy(c => c.County?.CountyName)
            .Select(c => c.County?.CountyName ?? "(unknown)")
            .ToList();

        var hasChanges = !originalItems.SequenceEqual(updatedItems);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "list",
                    OriginalItems = originalItems,
                    UpdatedItems = updatedItems
                }
            ]
        };
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

        var originalRows = projectTreatments
            .OrderBy(t => t.ProjectLocation?.ProjectLocationName)
            .Select(t =>
            {
                var locName = t.ProjectLocation?.ProjectLocationName ?? "(unknown)";
                var typeName = TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var tt) ? tt.TreatmentTypeDisplayName : "(unknown)";
                var actName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(t.TreatmentDetailedActivityTypeID, out var at) ? at.TreatmentDetailedActivityTypeDisplayName : "(unknown)";
                var acres = t.TreatmentTreatedAcres?.ToString("F2") ?? "(none)";
                return new List<string> { locName, typeName, actName, acres };
            })
            .ToList();

        var updatedRows = updateTreatments
            .OrderBy(t => t.ProjectLocationUpdate?.ProjectLocationUpdateName)
            .Select(t =>
            {
                var locName = t.ProjectLocationUpdate?.ProjectLocationUpdateName ?? "(unknown)";
                var typeName = TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var tt) ? tt.TreatmentTypeDisplayName : "(unknown)";
                var actName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(t.TreatmentDetailedActivityTypeID, out var at) ? at.TreatmentDetailedActivityTypeDisplayName : "(unknown)";
                var acres = t.TreatmentTreatedAcres?.ToString("F2") ?? "(none)";
                return new List<string> { locName, typeName, actName, acres };
            })
            .ToList();

        var hasChanges = !RowsEqual(originalRows, updatedRows);
        return new StepDiffResponse
        {
            HasChanges = hasChanges,
            Sections =
            [
                new DiffSection
                {
                    Type = "table",
                    Headers = ["Location", "Type", "Activity", "Acres"],
                    OriginalRows = originalRows,
                    UpdatedRows = updatedRows
                }
            ]
        };
    }

    /// <summary>Compares two row collections for equality.</summary>
    private static bool RowsEqual(List<List<string>> a, List<List<string>> b)
    {
        if (a.Count != b.Count) return false;
        for (var i = 0; i < a.Count; i++)
        {
            if (!a[i].SequenceEqual(b[i])) return false;
        }
        return true;
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

        var summary = new ProjectUpdateDiffSummary
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

        if (!string.IsNullOrEmpty(batch.StructuredDiffLogJson))
        {
            summary.StructuredStepDiffs = JsonSerializer.Deserialize<Dictionary<string, StepDiffResponse>>(
                batch.StructuredDiffLogJson);
        }

        return summary;
    }

    #endregion
}

