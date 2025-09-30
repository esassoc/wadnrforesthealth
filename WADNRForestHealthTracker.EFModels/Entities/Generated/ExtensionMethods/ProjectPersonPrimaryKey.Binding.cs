//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectPerson
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectPersonPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectPerson>
    {
        public ProjectPersonPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectPersonPrimaryKey(ProjectPerson projectPerson) : base(projectPerson){}

        public static implicit operator ProjectPersonPrimaryKey(int primaryKeyValue)
        {
            return new ProjectPersonPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectPersonPrimaryKey(ProjectPerson projectPerson)
        {
            return new ProjectPersonPrimaryKey(projectPerson);
        }
    }
}