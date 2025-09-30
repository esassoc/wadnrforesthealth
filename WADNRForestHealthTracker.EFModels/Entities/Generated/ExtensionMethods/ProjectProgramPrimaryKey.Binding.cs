//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectProgram
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectProgramPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectProgram>
    {
        public ProjectProgramPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectProgramPrimaryKey(ProjectProgram projectProgram) : base(projectProgram){}

        public static implicit operator ProjectProgramPrimaryKey(int primaryKeyValue)
        {
            return new ProjectProgramPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectProgramPrimaryKey(ProjectProgram projectProgram)
        {
            return new ProjectProgramPrimaryKey(projectProgram);
        }
    }
}