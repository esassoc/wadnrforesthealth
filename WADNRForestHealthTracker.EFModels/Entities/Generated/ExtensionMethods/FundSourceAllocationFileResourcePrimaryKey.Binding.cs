//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationFileResource
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationFileResourcePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationFileResource>
    {
        public FundSourceAllocationFileResourcePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationFileResourcePrimaryKey(FundSourceAllocationFileResource fundSourceAllocationFileResource) : base(fundSourceAllocationFileResource){}

        public static implicit operator FundSourceAllocationFileResourcePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationFileResourcePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationFileResourcePrimaryKey(FundSourceAllocationFileResource fundSourceAllocationFileResource)
        {
            return new FundSourceAllocationFileResourcePrimaryKey(fundSourceAllocationFileResource);
        }
    }
}