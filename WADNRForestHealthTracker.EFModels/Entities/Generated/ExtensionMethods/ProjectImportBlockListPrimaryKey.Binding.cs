//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectImportBlockList
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectImportBlockListPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectImportBlockList>
    {
        public ProjectImportBlockListPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectImportBlockListPrimaryKey(ProjectImportBlockList projectImportBlockList) : base(projectImportBlockList){}

        public static implicit operator ProjectImportBlockListPrimaryKey(int primaryKeyValue)
        {
            return new ProjectImportBlockListPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectImportBlockListPrimaryKey(ProjectImportBlockList projectImportBlockList)
        {
            return new ProjectImportBlockListPrimaryKey(projectImportBlockList);
        }
    }
}