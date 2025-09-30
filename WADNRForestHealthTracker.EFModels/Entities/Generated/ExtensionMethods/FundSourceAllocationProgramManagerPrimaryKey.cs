//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationProgramManager
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationProgramManagerPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationProgramManager>
    {
        public FundSourceAllocationProgramManagerPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationProgramManagerPrimaryKey(FundSourceAllocationProgramManager fundSourceAllocationProgramManager) : base(fundSourceAllocationProgramManager){}

        public static implicit operator FundSourceAllocationProgramManagerPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationProgramManagerPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationProgramManagerPrimaryKey(FundSourceAllocationProgramManager fundSourceAllocationProgramManager)
        {
            return new FundSourceAllocationProgramManagerPrimaryKey(fundSourceAllocationProgramManager);
        }
    }
}