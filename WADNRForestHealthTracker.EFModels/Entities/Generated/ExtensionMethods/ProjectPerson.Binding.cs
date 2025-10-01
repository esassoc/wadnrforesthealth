//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPerson]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class ProjectPerson
    {
        public int PrimaryKey => ProjectPersonID;
        public ProjectPersonRelationshipType ProjectPersonRelationshipType => ProjectPersonRelationshipType.AllLookupDictionary[ProjectPersonRelationshipTypeID];

        public static class FieldLengths
        {

        }
    }
}