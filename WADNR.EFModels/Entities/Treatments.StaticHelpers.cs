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

    public static async Task<TreatmentDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int treatmentID)
    {
        var raw = await dbContext.Treatments
            .AsNoTracking()
            .Where(t => t.TreatmentID == treatmentID)
            .Select(t => new
            {
                t.TreatmentID,
                t.ProjectID,
                ProjectName = t.Project.ProjectName,
                t.ProjectLocationID,
                TreatmentAreaName = t.ProjectLocation != null ? t.ProjectLocation.ProjectLocationName : null,
                t.TreatmentTypeID,
                t.TreatmentDetailedActivityTypeID,
                t.TreatmentCodeID,
                t.TreatmentStartDate,
                t.TreatmentEndDate,
                t.TreatmentFootprintAcres,
                t.TreatmentTreatedAcres,
                t.CostPerAcre,
                t.TreatmentNotes,
                t.ProgramID,
                ProgramName = t.Program != null
                    ? (t.Program.IsDefaultProgramForImportOnly
                        ? (t.Program.Organization != null ? t.Program.Organization.OrganizationName : null)
                        : t.Program.ProgramName)
                    : null,
                t.ImportedFromGis
            })
            .SingleOrDefaultAsync();

        if (raw == null) return null;

        return new TreatmentDetail
        {
            TreatmentID = raw.TreatmentID,
            ProjectID = raw.ProjectID,
            ProjectName = raw.ProjectName,
            ProjectLocationID = raw.ProjectLocationID,
            TreatmentAreaName = raw.TreatmentAreaName,
            TreatmentTypeID = raw.TreatmentTypeID,
            TreatmentTypeName = TreatmentType.AllLookupDictionary.TryGetValue(raw.TreatmentTypeID, out var tt)
                ? tt.TreatmentTypeName
                : $"Unknown ({raw.TreatmentTypeID})",
            TreatmentDetailedActivityTypeID = raw.TreatmentDetailedActivityTypeID,
            TreatmentDetailedActivityTypeName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(raw.TreatmentDetailedActivityTypeID, out var tda)
                ? tda.TreatmentDetailedActivityTypeName
                : $"Unknown ({raw.TreatmentDetailedActivityTypeID})",
            TreatmentCodeID = raw.TreatmentCodeID,
            TreatmentCodeName = raw.TreatmentCodeID.HasValue && TreatmentCode.AllLookupDictionary.TryGetValue(raw.TreatmentCodeID.Value, out var tc)
                ? tc.TreatmentCodeName
                : null,
            TreatmentStartDate = raw.TreatmentStartDate,
            TreatmentEndDate = raw.TreatmentEndDate,
            TreatmentFootprintAcres = raw.TreatmentFootprintAcres,
            TreatmentTreatedAcres = raw.TreatmentTreatedAcres,
            CostPerAcre = raw.CostPerAcre,
            TotalCost = (raw.TreatmentTreatedAcres ?? 0) * (raw.CostPerAcre ?? 0),
            TreatmentNotes = raw.TreatmentNotes,
            ProgramID = raw.ProgramID,
            ProgramName = raw.ProgramName,
            ImportedFromGis = raw.ImportedFromGis ?? false
        };
    }

    public static async Task<TreatmentDetail?> CreateAsync(WADNRDbContext dbContext, TreatmentUpsertRequest dto)
    {
        var entity = new Treatment
        {
            ProjectID = dto.ProjectID,
            ProjectLocationID = dto.ProjectLocationID,
            TreatmentTypeID = dto.TreatmentTypeID,
            TreatmentDetailedActivityTypeID = dto.TreatmentDetailedActivityTypeID,
            TreatmentCodeID = dto.TreatmentCodeID,
            TreatmentStartDate = dto.TreatmentStartDate,
            TreatmentEndDate = dto.TreatmentEndDate,
            TreatmentFootprintAcres = dto.TreatmentFootprintAcres,
            TreatmentTreatedAcres = dto.TreatmentTreatedAcres,
            CostPerAcre = dto.CostPerAcre,
            TreatmentNotes = dto.TreatmentNotes,
            ProgramID = dto.ProgramID,
            ImportedFromGis = false
        };
        dbContext.Treatments.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.TreatmentID);
    }

    public static async Task<TreatmentDetail?> UpdateAsync(WADNRDbContext dbContext, int treatmentID, TreatmentUpsertRequest dto)
    {
        var entity = await dbContext.Treatments
            .FirstOrDefaultAsync(t => t.TreatmentID == treatmentID);

        if (entity == null) return null;

        entity.ProjectID = dto.ProjectID;
        entity.ProjectLocationID = dto.ProjectLocationID;
        entity.TreatmentTypeID = dto.TreatmentTypeID;
        entity.TreatmentDetailedActivityTypeID = dto.TreatmentDetailedActivityTypeID;
        entity.TreatmentCodeID = dto.TreatmentCodeID;
        entity.TreatmentStartDate = dto.TreatmentStartDate;
        entity.TreatmentEndDate = dto.TreatmentEndDate;
        entity.TreatmentFootprintAcres = dto.TreatmentFootprintAcres;
        entity.TreatmentTreatedAcres = dto.TreatmentTreatedAcres;
        entity.CostPerAcre = dto.CostPerAcre;
        entity.TreatmentNotes = dto.TreatmentNotes;
        entity.ProgramID = dto.ProgramID;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.TreatmentID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int treatmentID)
    {
        var deletedCount = await dbContext.Treatments
            .Where(t => t.TreatmentID == treatmentID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }
}
