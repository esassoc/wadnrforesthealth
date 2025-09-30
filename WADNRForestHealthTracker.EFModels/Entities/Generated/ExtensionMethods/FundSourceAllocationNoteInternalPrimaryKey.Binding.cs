//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationNoteInternal
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationNoteInternalPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationNoteInternal>
    {
        public FundSourceAllocationNoteInternalPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationNoteInternalPrimaryKey(FundSourceAllocationNoteInternal fundSourceAllocationNoteInternal) : base(fundSourceAllocationNoteInternal){}

        public static implicit operator FundSourceAllocationNoteInternalPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationNoteInternalPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationNoteInternalPrimaryKey(FundSourceAllocationNoteInternal fundSourceAllocationNoteInternal)
        {
            return new FundSourceAllocationNoteInternalPrimaryKey(fundSourceAllocationNoteInternal);
        }
    }
}