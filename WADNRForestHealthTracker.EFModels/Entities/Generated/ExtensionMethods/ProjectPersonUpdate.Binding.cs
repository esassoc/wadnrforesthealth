//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPersonUpdate]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class ProjectPersonUpdate
    {
        public int PrimaryKey => ProjectPersonUpdateID;
        public ProjectPersonRelationshipType ProjectPersonRelationshipType => ProjectPersonRelationshipType.AllLookupDictionary[ProjectPersonRelationshipTypeID];

        public static class FieldLengths
        {

        }
    }
}