//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectUpdateProgram
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectUpdateProgramPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectUpdateProgram>
    {
        public ProjectUpdateProgramPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectUpdateProgramPrimaryKey(ProjectUpdateProgram projectUpdateProgram) : base(projectUpdateProgram){}

        public static implicit operator ProjectUpdateProgramPrimaryKey(int primaryKeyValue)
        {
            return new ProjectUpdateProgramPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectUpdateProgramPrimaryKey(ProjectUpdateProgram projectUpdateProgram)
        {
            return new ProjectUpdateProgramPrimaryKey(projectUpdateProgram);
        }
    }
}