//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FundSourceNoteInternal
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FundSourceNoteInternalPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FundSourceNoteInternal>
    {
        public FundSourceNoteInternalPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FundSourceNoteInternalPrimaryKey(FundSourceNoteInternal fundSourceNoteInternal) : base(fundSourceNoteInternal){}

        public static implicit operator FundSourceNoteInternalPrimaryKey(int primaryKeyValue)
        {
            return new FundSourceNoteInternalPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FundSourceNoteInternalPrimaryKey(FundSourceNoteInternal fundSourceNoteInternal)
        {
            return new FundSourceNoteInternalPrimaryKey(fundSourceNoteInternal);
        }
    }
}