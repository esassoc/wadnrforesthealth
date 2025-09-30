//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceFileResource
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceFileResourcePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceFileResource>
    {
        public FundSourceFileResourcePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceFileResourcePrimaryKey(FundSourceFileResource fundSourceFileResource) : base(fundSourceFileResource){}

        public static implicit operator FundSourceFileResourcePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceFileResourcePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceFileResourcePrimaryKey(FundSourceFileResource fundSourceFileResource)
        {
            return new FundSourceFileResourcePrimaryKey(fundSourceFileResource);
        }
    }
}