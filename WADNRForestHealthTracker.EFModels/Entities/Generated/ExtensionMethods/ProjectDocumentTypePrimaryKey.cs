//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectDocumentType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectDocumentTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectDocumentType>
    {
        public ProjectDocumentTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectDocumentTypePrimaryKey(ProjectDocumentType projectDocumentType) : base(projectDocumentType){}

        public static implicit operator ProjectDocumentTypePrimaryKey(int primaryKeyValue)
        {
            return new ProjectDocumentTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectDocumentTypePrimaryKey(ProjectDocumentType projectDocumentType)
        {
            return new ProjectDocumentTypePrimaryKey(projectDocumentType);
        }
    }
}