//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResource]
namespace WADNR.EFModels.Entities
{
    public partial class FileResource
    {
        public int PrimaryKey => FileResourceID;
        public FileResourceMimeType FileResourceMimeType => FileResourceMimeType.AllLookupDictionary[FileResourceMimeTypeID];

        public static class FieldLengths
        {
            public const int OriginalBaseFilename = 255;
            public const int OriginalFileExtension = 255;
        }
    }
}