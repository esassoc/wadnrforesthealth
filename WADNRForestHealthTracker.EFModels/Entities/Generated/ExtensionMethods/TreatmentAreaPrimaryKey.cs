//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.TreatmentArea
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class TreatmentAreaPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<TreatmentArea>
    {
        public TreatmentAreaPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public TreatmentAreaPrimaryKey(TreatmentArea treatmentArea) : base(treatmentArea){}

        public static implicit operator TreatmentAreaPrimaryKey(int primaryKeyValue)
        {
            return new TreatmentAreaPrimaryKey(primaryKeyValue);
        }

        public static implicit operator TreatmentAreaPrimaryKey(TreatmentArea treatmentArea)
        {
            return new TreatmentAreaPrimaryKey(treatmentArea);
        }
    }
}