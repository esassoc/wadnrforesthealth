//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ProjectPersonRelationshipType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ProjectPersonRelationshipTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ProjectPersonRelationshipType>
    {
        public ProjectPersonRelationshipTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ProjectPersonRelationshipTypePrimaryKey(ProjectPersonRelationshipType projectPersonRelationshipType) : base(projectPersonRelationshipType){}

        public static implicit operator ProjectPersonRelationshipTypePrimaryKey(int primaryKeyValue)
        {
            return new ProjectPersonRelationshipTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ProjectPersonRelationshipTypePrimaryKey(ProjectPersonRelationshipType projectPersonRelationshipType)
        {
            return new ProjectPersonRelationshipTypePrimaryKey(projectPersonRelationshipType);
        }
    }
}