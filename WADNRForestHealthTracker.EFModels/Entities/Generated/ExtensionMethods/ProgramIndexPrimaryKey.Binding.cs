//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProgramIndex
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProgramIndexPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProgramIndex>
    {
        public ProgramIndexPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProgramIndexPrimaryKey(ProgramIndex programIndex) : base(programIndex){}

        public static implicit operator ProgramIndexPrimaryKey(int primaryKeyValue)
        {
            return new ProgramIndexPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProgramIndexPrimaryKey(ProgramIndex programIndex)
        {
            return new ProgramIndexPrimaryKey(programIndex);
        }
    }
}