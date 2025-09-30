//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceType>
    {
        public FundSourceTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceTypePrimaryKey(FundSourceType fundSourceType) : base(fundSourceType){}

        public static implicit operator FundSourceTypePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceTypePrimaryKey(FundSourceType fundSourceType)
        {
            return new FundSourceTypePrimaryKey(fundSourceType);
        }
    }
}