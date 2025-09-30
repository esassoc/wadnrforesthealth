//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationProgramIndexProjectCode
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationProgramIndexProjectCodePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationProgramIndexProjectCode>
    {
        public FundSourceAllocationProgramIndexProjectCodePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationProgramIndexProjectCodePrimaryKey(FundSourceAllocationProgramIndexProjectCode fundSourceAllocationProgramIndexProjectCode) : base(fundSourceAllocationProgramIndexProjectCode){}

        public static implicit operator FundSourceAllocationProgramIndexProjectCodePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationProgramIndexProjectCodePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationProgramIndexProjectCodePrimaryKey(FundSourceAllocationProgramIndexProjectCode fundSourceAllocationProgramIndexProjectCode)
        {
            return new FundSourceAllocationProgramIndexProjectCodePrimaryKey(fundSourceAllocationProgramIndexProjectCode);
        }
    }
}