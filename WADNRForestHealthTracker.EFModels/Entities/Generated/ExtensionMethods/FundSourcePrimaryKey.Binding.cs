//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSource
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourcePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSource>
    {
        public FundSourcePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourcePrimaryKey(FundSource fundSource) : base(fundSource){}

        public static implicit operator FundSourcePrimaryKey(int primaryKeyValue)
        {
            return new FundSourcePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourcePrimaryKey(FundSource fundSource)
        {
            return new FundSourcePrimaryKey(fundSource);
        }
    }
}