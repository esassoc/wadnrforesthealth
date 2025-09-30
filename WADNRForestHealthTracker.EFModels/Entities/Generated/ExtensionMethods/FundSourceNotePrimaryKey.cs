//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceNote
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceNotePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceNote>
    {
        public FundSourceNotePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceNotePrimaryKey(FundSourceNote fundSourceNote) : base(fundSourceNote){}

        public static implicit operator FundSourceNotePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceNotePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceNotePrimaryKey(FundSourceNote fundSourceNote)
        {
            return new FundSourceNotePrimaryKey(fundSourceNote);
        }
    }
}