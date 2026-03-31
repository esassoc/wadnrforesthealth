//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectDocument]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectDocument
    {
        public int PrimaryKey => ProjectDocumentID;
        public ProjectDocumentType? ProjectDocumentType => ProjectDocumentTypeID.HasValue ? ProjectDocumentType.AllLookupDictionary[ProjectDocumentTypeID.Value] : null;

        public static class FieldLengths
        {
            public const int DisplayName = 200;
            public const int Description = 1000;
        }
    }
}