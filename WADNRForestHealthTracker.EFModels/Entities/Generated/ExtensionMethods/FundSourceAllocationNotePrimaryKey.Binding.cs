//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceAllocationNote
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceAllocationNotePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceAllocationNote>
    {
        public FundSourceAllocationNotePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceAllocationNotePrimaryKey(FundSourceAllocationNote fundSourceAllocationNote) : base(fundSourceAllocationNote){}

        public static implicit operator FundSourceAllocationNotePrimaryKey(int primaryKeyValue)
        {
            return new FundSourceAllocationNotePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceAllocationNotePrimaryKey(FundSourceAllocationNote fundSourceAllocationNote)
        {
            return new FundSourceAllocationNotePrimaryKey(fundSourceAllocationNote);
        }
    }
}