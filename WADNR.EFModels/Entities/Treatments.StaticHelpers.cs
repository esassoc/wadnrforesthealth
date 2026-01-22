using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Treatments
{
    public static async Task<List<TreatmentGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
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
                t.CostPerAcre,
                t.TreatmentNotes,
                t.TreatmentCodeID,
                t.ImportedFromGis,
                TreatmentAreaName = t.ProjectLocation != null ? t.ProjectLocation.ProjectLocationName : null,
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
                TreatmentAreaName = t.TreatmentAreaName,
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
                CostPerAcre = t.CostPerAcre,
                TotalCost = (t.TreatmentTreatedAcres ?? 0) * (t.CostPerAcre ?? 0),
                TreatmentNotes = t.TreatmentNotes,
                ProgramName = t.ProgramName,
                TreatmentCodeName = t.TreatmentCodeID.HasValue && TreatmentCode.AllLookupDictionary.TryGetValue(t.TreatmentCodeID.Value, out var tc)
                    ? tc.TreatmentCodeName
                    : null,
                ImportedFromGis = t.ImportedFromGis ?? false
            })
            .OrderBy(t => t.TreatmentStartDate)
            .ThenBy(t => t.TreatmentTypeName)
            .ToList();

        return treatments;
    }
}
