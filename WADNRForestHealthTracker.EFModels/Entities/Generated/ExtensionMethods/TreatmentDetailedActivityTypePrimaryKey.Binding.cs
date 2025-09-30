//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.TreatmentDetailedActivityType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class TreatmentDetailedActivityTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<TreatmentDetailedActivityType>
    {
        public TreatmentDetailedActivityTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public TreatmentDetailedActivityTypePrimaryKey(TreatmentDetailedActivityType treatmentDetailedActivityType) : base(treatmentDetailedActivityType){}

        public static implicit operator TreatmentDetailedActivityTypePrimaryKey(int primaryKeyValue)
        {
            return new TreatmentDetailedActivityTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator TreatmentDetailedActivityTypePrimaryKey(TreatmentDetailedActivityType treatmentDetailedActivityType)
        {
            return new TreatmentDetailedActivityTypePrimaryKey(treatmentDetailedActivityType);
        }
    }
}