//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.TreatmentCode
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class TreatmentCodePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<TreatmentCode>
    {
        public TreatmentCodePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public TreatmentCodePrimaryKey(TreatmentCode treatmentCode) : base(treatmentCode){}

        public static implicit operator TreatmentCodePrimaryKey(int primaryKeyValue)
        {
            return new TreatmentCodePrimaryKey(primaryKeyValue);
        }

        public static implicit operator TreatmentCodePrimaryKey(TreatmentCode treatmentCode)
        {
            return new TreatmentCodePrimaryKey(treatmentCode);
        }
    }
}