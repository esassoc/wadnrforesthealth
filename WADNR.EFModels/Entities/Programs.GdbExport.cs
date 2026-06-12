using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static partial class Programs
{
    // Explicit culture so Linux containers (which often default to invariant/POSIX)
    // render currency as $10,000.00 rather than ¤10000.00.
    private static readonly CultureInfo GdbExportCurrencyCulture = CultureInfo.GetCultureInfo("en-US");

    /// <summary>
    /// Normalizes an area geometry to a single MultiPolygon so it can be written to a FileGDB
    /// polygon feature class. ProjectLocationGeometry can be a GeometryCollection — for example,
    /// MakeValid() on a self-intersecting polygon commonly yields polygons plus dangling lines —
    /// and ogr2ogr's OpenFileGDB driver rejects those with "ERROR 6: Unsupported geometry type"
    /// even with -nlt PROMOTE_TO_MULTI. Extracting the polygonal parts yields a layer of one
    /// supported geometry type. Non-polygonal artifacts (the dangling lines/points) are dropped;
    /// a geometry with no polygonal content at all is left unchanged (vanishingly rare for an area).
    /// </summary>
    private static Geometry NormalizeToMultiPolygonForGdb(Geometry geometry)
    {
        if (geometry == null || geometry is MultiPolygon)
        {
            return geometry;
        }

        var polygons = PolygonExtracter.GetPolygons(geometry);
        if (polygons.Count == 0)
        {
            return geometry;
        }

        var multiPolygon = geometry.Factory.CreateMultiPolygon(polygons.Cast<Polygon>().ToArray());
        multiPolygon.SRID = geometry.SRID;
        return multiPolygon;
    }

    public static async Task<ProgramGdbExportData> GetGdbExportDataAsync(WADNRDbContext dbContext, int programID, string webUrl)
    {
        var projectPoints = await GetGdbProjectPointsAsync(dbContext, programID, webUrl);
        var projectLocations = await GetGdbProjectLocationsAsync(dbContext, programID);
        var treatments = await GetGdbTreatmentsAsync(dbContext, programID);

        return new ProgramGdbExportData
        {
            ProjectPoints = projectPoints,
            ProjectLocations = projectLocations,
            Treatments = treatments,
        };
    }

    private static async Task<List<ProgramGdbProjectPointDto>> GetGdbProjectPointsAsync(WADNRDbContext dbContext, int programID, string webUrl)
    {
        var baseUrl = (webUrl ?? string.Empty).TrimEnd('/');

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
                pp.Project.ProjectLocationNotes,
                pp.Project.ProjectGisIdentifier,
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
                CountyNames = pp.Project.ProjectCounties
                    .Select(pc => pc.County.CountyName)
                    .ToList(),
                RegionNames = pp.Project.ProjectRegions
                    .Select(pr => pr.DNRUplandRegion.DNRUplandRegionName)
                    .ToList(),
                PriorityLandscapeNames = pp.Project.ProjectPriorityLandscapes
                    .Select(ppl => ppl.PriorityLandscape.PriorityLandscapeName)
                    .ToList(),
                FundingSources = pp.Project.ProjectFundSourceAllocationRequests
                    .Select(fsar => new
                    {
                        FundSourceName = fsar.FundSourceAllocation.FundSource.FundSourceName,
                        fsar.TotalAmount,
                    }).ToList(),
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

            var fundingSourcesCsv = string.Join("; ", r.FundingSources
                .OrderBy(fs => fs.FundSourceName)
                .Select(fs => fs.TotalAmount.HasValue
                    ? $"{fs.FundSourceName}: {fs.TotalAmount.Value.ToString("C2", GdbExportCurrencyCulture)}"
                    : fs.FundSourceName));

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
                ProjectLocationNotes = r.ProjectLocationNotes,
                ProjectGisIdentifier = r.ProjectGisIdentifier,
                Counties = r.CountyNames.Count == 0 ? null : string.Join("; ", r.CountyNames.OrderBy(n => n)),
                Regions = r.RegionNames.Count == 0 ? null : string.Join("; ", r.RegionNames.OrderBy(n => n)),
                PriorityLandscapes = r.PriorityLandscapeNames.Count == 0 ? null : string.Join("; ", r.PriorityLandscapeNames.OrderBy(n => n)),
                FundingSources = string.IsNullOrEmpty(fundingSourcesCsv) ? null : fundingSourcesCsv,
                ProjectDetailUrl = $"{baseUrl}/projects/{r.ProjectID}",
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
                pl.Project.ProjectStageID,
                ProjectTypeName = pl.Project.ProjectType.ProjectTypeName,
                pl.ProjectLocationName,
                pl.ProjectLocationTypeID,
                pl.ProjectLocationNotes,
                pl.ArcGisObjectID,
                pl.ArcGisGlobalID,
                pl.ImportedFromGisUpload,
                SourceProgramName = pl.Program != null ? pl.Program.ProgramName : null,
                Geometry = pl.ProjectLocationGeometry,
                Treatments = pl.Treatments.Select(t => new
                {
                    t.TreatmentTypeID,
                    t.TreatmentFootprintAcres,
                    t.TreatmentTreatedAcres,
                    t.CostPerAcre,
                }).ToList(),
            })
            .ToListAsync();

        return rows.Select(r =>
        {
            var stageName = ProjectStage.AllLookupDictionary.TryGetValue(r.ProjectStageID, out var stage)
                ? stage.ProjectStageDisplayName
                : null;

            var locationTypeName = ProjectLocationType.AllLookupDictionary.TryGetValue(r.ProjectLocationTypeID, out var locType)
                ? locType.ProjectLocationTypeDisplayName
                : null;

            var treatmentTypeNames = r.Treatments
                .Select(t => TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var tt) ? tt.TreatmentTypeDisplayName : null)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            // Same rule as Treatments.TotalCost: (TreatedAcres ?? Footprint) × CostPerAcre, summed.
            var treatmentsWithCost = r.Treatments.Where(t => t.CostPerAcre.HasValue).ToList();
            decimal? totalCost = treatmentsWithCost.Count == 0
                ? null
                : treatmentsWithCost.Sum(t => (t.TreatmentTreatedAcres ?? t.TreatmentFootprintAcres) * t.CostPerAcre!.Value);

            return new ProgramGdbProjectLocationDto
            {
                ProjectLocationID = r.ProjectLocationID,
                ProjectID = r.ProjectID,
                ProjectName = r.ProjectName,
                FhtProjectNumber = r.FhtProjectNumber,
                ProjectStage = stageName,
                ProjectType = r.ProjectTypeName,
                ProjectLocationName = r.ProjectLocationName,
                ProjectLocationType = locationTypeName,
                ProjectLocationNotes = r.ProjectLocationNotes,
                ArcGisObjectID = r.ArcGisObjectID,
                ArcGisGlobalID = r.ArcGisGlobalID,
                ImportedFromGisUpload = r.ImportedFromGisUpload,
                SourceProgram = r.SourceProgramName,
                TreatmentCount = r.Treatments.Count,
                TreatmentTypes = treatmentTypeNames.Count == 0 ? null : string.Join("; ", treatmentTypeNames!),
                TotalFootprintAcres = r.Treatments.Sum(t => t.TreatmentFootprintAcres),
                TotalTreatedAcres = r.Treatments.Any(t => t.TreatmentTreatedAcres.HasValue)
                    ? r.Treatments.Sum(t => t.TreatmentTreatedAcres ?? 0m)
                    : (decimal?)null,
                TotalCost = totalCost,
                Geometry = NormalizeToMultiPolygonForGdb(r.Geometry),
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
                t.TreatmentTypeImportedText,
                t.TreatmentDetailedActivityTypeID,
                t.TreatmentDetailedActivityTypeImportedText,
                t.TreatmentCodeID,
                t.TreatmentStartDate,
                t.TreatmentEndDate,
                t.TreatmentFootprintAcres,
                t.TreatmentTreatedAcres,
                t.CostPerAcre,
                t.ImportedFromGis,
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

            // TreatedAcres × CostPerAcre when both available; fall back to footprint × cost.
            decimal? totalCost = r.CostPerAcre.HasValue
                ? (r.TreatmentTreatedAcres ?? r.TreatmentFootprintAcres) * r.CostPerAcre.Value
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
                TreatmentTypeImportedText = r.TreatmentTypeImportedText,
                TreatmentDetailedActivityType = detailedTypeName,
                TreatmentDetailedActivityTypeImportedText = r.TreatmentDetailedActivityTypeImportedText,
                TreatmentCode = treatmentCodeName,
                TreatmentStartDate = r.TreatmentStartDate,
                TreatmentEndDate = r.TreatmentEndDate,
                TreatmentFootprintAcres = r.TreatmentFootprintAcres,
                TreatmentTreatedAcres = r.TreatmentTreatedAcres,
                CostPerAcre = r.CostPerAcre,
                TotalCost = totalCost,
                ImportedFromGis = r.ImportedFromGis,
                TreatmentNotes = r.TreatmentNotes,
                Geometry = NormalizeToMultiPolygonForGdb(r.Geometry),
            };
        }).ToList();
    }
}
