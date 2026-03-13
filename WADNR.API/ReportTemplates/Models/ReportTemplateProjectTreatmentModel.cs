using System;
using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateProjectTreatmentModel : ReportTemplateBaseModel
    {
        private Project Project { get; set; }
        private Treatment ProjectTreatment { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public DateOnly? StartDate { get; set; }
        public string StartDateDisplay => StartDate.HasValue ? StartDate.Value.ToString("d") : string.Empty;
        public DateOnly? EndDate { get; set; }
        public string EndDateDisplay => EndDate.HasValue ? EndDate.Value.ToString("d") : string.Empty;
        public decimal FootprintAcres { get; set; }
        public string FootprintAcresDisplay(int decimalPlaces = 3) => ProjectTreatment.TreatmentFootprintAcres.ToString($"N{decimalPlaces}");
        public decimal? TreatedAcres { get; set; }
        public string TreatedAcresDisplay(int decimalPlaces = 3) => ProjectTreatment.TreatmentTreatedAcres.HasValue ? ProjectTreatment.TreatmentTreatedAcres.Value.ToString($"N{decimalPlaces}") : string.Empty;
        public decimal? CostPerAcre { get; set; }
        public string CostPerAcreDisplay(int decimalPlaces = 2) => CostPerAcre.HasValue ? CostPerAcre.Value.ToString($"C{decimalPlaces}", UsCulture) : string.Empty;

        public decimal? TotalCostFootprint
        {
            get
            {
                if (CostPerAcre.HasValue)
                    return ProjectTreatment.TreatmentFootprintAcres * CostPerAcre.Value;

                return null;
            }
        }
        public string TotalCostFootprintDisplay(int decimalPlaces = 2) => TotalCostFootprint.HasValue ? TotalCostFootprint.Value.ToString($"C{decimalPlaces}", UsCulture) : string.Empty;

        public decimal? TotalCostTreated
        {
            get
            {
                if (CostPerAcre.HasValue && ProjectTreatment.TreatmentTreatedAcres.HasValue)
                    return ProjectTreatment.TreatmentTreatedAcres.Value * CostPerAcre.Value;
                return null;
            }
        }
        public string TotalCostTreatedDisplay(int decimalPlaces = 2) => TotalCostTreated.HasValue ? TotalCostTreated.Value.ToString($"C{decimalPlaces}", UsCulture) : string.Empty;

        public ReportTemplateProjectTreatmentModel(Treatment projectTreatment)
        {
            Project = projectTreatment.Project;
            ProjectTreatment = projectTreatment;

            Code = ProjectTreatment.TreatmentCodeID.HasValue
                && TreatmentCode.AllLookupDictionary.TryGetValue(ProjectTreatment.TreatmentCodeID.Value, out var treatmentCode)
                ? treatmentCode.TreatmentCodeDisplayName
                : null;
            Name = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(
                ProjectTreatment.TreatmentDetailedActivityTypeID, out var activityType)
                ? activityType.TreatmentDetailedActivityTypeDisplayName
                : string.Empty;
            StartDate = ProjectTreatment.TreatmentStartDate;
            EndDate = ProjectTreatment.TreatmentEndDate;
            FootprintAcres = ProjectTreatment.TreatmentFootprintAcres;
            TreatedAcres = ProjectTreatment.TreatmentTreatedAcres;
            CostPerAcre = ProjectTreatment.CostPerAcre;
        }
    }
}
