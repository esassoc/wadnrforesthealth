//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceStatus
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceStatusPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceStatus>
    {
        public FundSourceStatusPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceStatusPrimaryKey(FundSourceStatus fundSourceStatus) : base(fundSourceStatus){}

        public static implicit operator FundSourceStatusPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceStatusPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceStatusPrimaryKey(FundSourceStatus fundSourceStatus)
        {
            return new FundSourceStatusPrimaryKey(fundSourceStatus);
        }
    }
}