using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static partial class Programs
{
    public static async Task<ProgramGdbExportData> GetGdbExportDataAsync(WADNRDbContext dbContext, int programID)
    {
        var projectPoints = await GetGdbProjectPointsAsync(dbContext, programID);
        var projectLocations = await GetGdbProjectLocationsAsync(dbContext, programID);
        var treatments = await GetGdbTreatmentsAsync(dbContext, programID);

        return new ProgramGdbExportData
        {
            ProjectPoints = projectPoints,
            ProjectLocations = projectLocations,
            Treatments = treatments,
        };
    }

    private static async Task<List<ProgramGdbProjectPointDto>> GetGdbProjectPointsAsync(WADNRDbContext dbContext, int programID)
    {
        var rows = await dbContext.ProjectPrograms
            .AsNoTracking()
            .Where(pp => pp.ProgramID == programID && pp.Project.ProjectLocationPoint != null)
            .Select(pp => new
            {
                pp.Project.ProjectID,
                pp.Project.ProjectName,
                pp.Project.FhtProjectNumber,
                pp.Project.ProjectDescription,
                pp.Project.ProjectStageID,
                ProjectTypeName = pp.Project.ProjectType.ProjectTypeName,
                TaxonomyBranchName = pp.Project.ProjectType.TaxonomyBranch.TaxonomyBranchName,
                TaxonomyTrunkName = pp.Project.ProjectType.TaxonomyBranch.TaxonomyTrunk.TaxonomyTrunkName,
                pp.Project.PlannedDate,
                pp.Project.CompletionDate,
                pp.Project.EstimatedTotalCost,
                Geometry = pp.Project.ProjectLocationPoint!,
                Organizations = pp.Project.ProjectOrganizations.Select(po => new
                {
                    po.Organization.OrganizationName,
                    po.RelationshipType.RelationshipTypeName,
                    po.RelationshipType.IsPrimaryContact,
                }).ToList(),
                ClassificationNames = pp.Project.ProjectClassifications
                    .Select(pc => pc.Classification.DisplayName)
                    .ToList(),
                ProgramNames = pp.Project.ProjectPrograms
                    .Select(prp => prp.Program.ProgramName)
                    .ToList(),
            })
            .ToListAsync();

        return rows.Select(r =>
        {
            var stageName = ProjectStage.AllLookupDictionary.TryGetValue(r.ProjectStageID, out var stage)
                ? stage.ProjectStageDisplayName
                : null;

            var leadImplementer = r.Organizations.FirstOrDefault(o => o.IsPrimaryContact)?.OrganizationName;
            var orgsCsv = string.Join("; ", r.Organizations
                .Select(o => $"{o.OrganizationName} ({o.RelationshipTypeName})"));

            return new ProgramGdbProjectPointDto
            {
                ProjectID = r.ProjectID,
                ProjectName = r.ProjectName,
                FhtProjectNumber = r.FhtProjectNumber,
                ProjectDescription = r.ProjectDescription,
                ProjectStage = stageName,
                ProjectType = r.ProjectTypeName,
                TaxonomyBranch = r.TaxonomyBranchName,
                TaxonomyTrunk = r.TaxonomyTrunkName,
                LeadImplementer = leadImplementer,
                Organizations = string.IsNullOrEmpty(orgsCsv) ? null : orgsCsv,
                Classifications = r.ClassificationNames.Count == 0 ? null : string.Join("; ", r.ClassificationNames),
                Programs = r.ProgramNames.Count == 0 ? null : string.Join("; ", r.ProgramNames),
                PlannedDate = r.PlannedDate,
                CompletionDate = r.CompletionDate,
                EstimatedTotalCost = r.EstimatedTotalCost,
                Geometry = r.Geometry,
            };
        }).ToList();
    }

    private static async Task<List<ProgramGdbProjectLocationDto>> GetGdbProjectLocationsAsync(WADNRDbContext dbContext, int programID)
    {
        var rows = await dbContext.ProjectPrograms
            .AsNoTracking()
            .Where(pp => pp.ProgramID == programID)
            .SelectMany(pp => pp.Project.ProjectLocations)
            .Where(pl => pl.ProjectLocationGeometry != null)
            .Select(pl => new
            {
                pl.ProjectLocationID,
                pl.ProjectID,
                pl.Project.ProjectName,
                pl.Project.FhtProjectNumber,
                pl.ProjectLocationName,
                pl.ProjectLocationTypeID,
                pl.ProjectLocationNotes,
                Geometry = pl.ProjectLocationGeometry,
                Treatments = pl.Treatments.Select(t => new
                {
                    t.TreatmentTypeID,
                    t.TreatmentFootprintAcres,
                    t.TreatmentTreatedAcres,
                }).ToList(),
            })
            .ToListAsync();

        return rows.Select(r =>
        {
            var locationTypeName = ProjectLocationType.AllLookupDictionary.TryGetValue(r.ProjectLocationTypeID, out var locType)
                ? locType.ProjectLocationTypeDisplayName
                : null;

            var treatmentTypeNames = r.Treatments
                .Select(t => TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var tt) ? tt.TreatmentTypeDisplayName : null)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            return new ProgramGdbProjectLocationDto
            {
                ProjectLocationID = r.ProjectLocationID,
                ProjectID = r.ProjectID,
                ProjectName = r.ProjectName,
                FhtProjectNumber = r.FhtProjectNumber,
                ProjectLocationName = r.ProjectLocationName,
                ProjectLocationType = locationTypeName,
                ProjectLocationNotes = r.ProjectLocationNotes,
                TreatmentCount = r.Treatments.Count,
                TreatmentTypes = treatmentTypeNames.Count == 0 ? null : string.Join("; ", treatmentTypeNames!),
                TotalFootprintAcres = r.Treatments.Sum(t => t.TreatmentFootprintAcres),
                TotalTreatedAcres = r.Treatments.Any(t => t.TreatmentTreatedAcres.HasValue)
                    ? r.Treatments.Sum(t => t.TreatmentTreatedAcres ?? 0m)
                    : (decimal?)null,
                Geometry = r.Geometry,
            };
        }).ToList();
    }

    private static async Task<List<ProgramGdbTreatmentDto>> GetGdbTreatmentsAsync(WADNRDbContext dbContext, int programID)
    {
        var rows = await dbContext.ProjectPrograms
            .AsNoTracking()
            .Where(pp => pp.ProgramID == programID)
            .SelectMany(pp => pp.Project.Treatments)
            .Where(t => t.ProjectLocation != null && t.ProjectLocation.ProjectLocationGeometry != null)
            .Select(t => new
            {
                t.TreatmentID,
                t.ProjectID,
                t.Project.ProjectName,
                t.Project.FhtProjectNumber,
                ProjectLocationID = t.ProjectLocationID!.Value,
                ProjectLocationName = t.ProjectLocation!.ProjectLocationName,
                t.TreatmentTypeID,
                t.TreatmentDetailedActivityTypeID,
                t.TreatmentCodeID,
                t.TreatmentStartDate,
                t.TreatmentEndDate,
                t.TreatmentFootprintAcres,
                t.TreatmentTreatedAcres,
                t.CostPerAcre,
                t.TreatmentNotes,
                Geometry = t.ProjectLocation.ProjectLocationGeometry,
            })
            .ToListAsync();

        return rows.Select(r =>
        {
            var treatmentTypeName = TreatmentType.AllLookupDictionary.TryGetValue(r.TreatmentTypeID, out var tt)
                ? tt.TreatmentTypeDisplayName
                : null;
            var detailedTypeName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(r.TreatmentDetailedActivityTypeID, out var dt)
                ? dt.TreatmentDetailedActivityTypeDisplayName
                : null;
            var treatmentCodeName = r.TreatmentCodeID.HasValue && TreatmentCode.AllLookupDictionary.TryGetValue(r.TreatmentCodeID.Value, out var tc)
                ? tc.TreatmentCodeDisplayName
                : null;

            return new ProgramGdbTreatmentDto
            {
                TreatmentID = r.TreatmentID,
                ProjectID = r.ProjectID,
                ProjectName = r.ProjectName,
                FhtProjectNumber = r.FhtProjectNumber,
                ProjectLocationID = r.ProjectLocationID,
                ProjectLocationName = r.ProjectLocationName,
                TreatmentType = treatmentTypeName,
                TreatmentDetailedActivityType = detailedTypeName,
                TreatmentCode = treatmentCodeName,
                TreatmentStartDate = r.TreatmentStartDate,
                TreatmentEndDate = r.TreatmentEndDate,
                TreatmentFootprintAcres = r.TreatmentFootprintAcres,
                TreatmentTreatedAcres = r.TreatmentTreatedAcres,
                CostPerAcre = r.CostPerAcre,
                TreatmentNotes = r.TreatmentNotes,
                Geometry = r.Geometry,
            };
        }).ToList();
    }
}
