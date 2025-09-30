//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.Treatment
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class TreatmentPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<Treatment>
    {
        public TreatmentPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public TreatmentPrimaryKey(Treatment treatment) : base(treatment){}

        public static implicit operator TreatmentPrimaryKey(int primaryKeyValue)
        {
            return new TreatmentPrimaryKey(primaryKeyValue);
        }

        public static implicit operator TreatmentPrimaryKey(Treatment treatment)
        {
            return new TreatmentPrimaryKey(treatment);
        }
    }
}