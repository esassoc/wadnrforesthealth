//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProgramPerson
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProgramPersonPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProgramPerson>
    {
        public ProgramPersonPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProgramPersonPrimaryKey(ProgramPerson programPerson) : base(programPerson){}

        public static implicit operator ProgramPersonPrimaryKey(int primaryKeyValue)
        {
            return new ProgramPersonPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProgramPersonPrimaryKey(ProgramPerson programPerson)
        {
            return new ProgramPersonPrimaryKey(programPerson);
        }
    }
}