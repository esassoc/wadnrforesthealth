//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.TreatmentUpdate
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class TreatmentUpdatePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<TreatmentUpdate>
    {
        public TreatmentUpdatePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public TreatmentUpdatePrimaryKey(TreatmentUpdate treatmentUpdate) : base(treatmentUpdate){}

        public static implicit operator TreatmentUpdatePrimaryKey(int primaryKeyValue)
        {
            return new TreatmentUpdatePrimaryKey(primaryKeyValue);
        }

        public static implicit operator TreatmentUpdatePrimaryKey(TreatmentUpdate treatmentUpdate)
        {
            return new TreatmentUpdatePrimaryKey(treatmentUpdate);
        }
    }
}