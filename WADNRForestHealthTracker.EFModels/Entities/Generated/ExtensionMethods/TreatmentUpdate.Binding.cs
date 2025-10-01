//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentUpdate]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class TreatmentUpdate
    {
        public int PrimaryKey => TreatmentUpdateID;
        public TreatmentType TreatmentType => TreatmentType.AllLookupDictionary[TreatmentTypeID];
        public TreatmentDetailedActivityType TreatmentDetailedActivityType => TreatmentDetailedActivityType.AllLookupDictionary[TreatmentDetailedActivityTypeID];
        public TreatmentCode? TreatmentCode => TreatmentCodeID.HasValue ? TreatmentCode.AllLookupDictionary[TreatmentCodeID.Value] : null;

        public static class FieldLengths
        {
            public const int TreatmentNotes = 2000;
            public const int TreatmentTypeImportedText = 200;
            public const int TreatmentDetailedActivityTypeImportedText = 200;
        }
    }
}