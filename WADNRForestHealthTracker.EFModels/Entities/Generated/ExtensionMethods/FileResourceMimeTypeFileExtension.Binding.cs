//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResourceMimeTypeFileExtension]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class FileResourceMimeTypeFileExtension
    {
        public int PrimaryKey => FileResourceMimeTypeFileExtensionID;
        public FileResourceMimeType FileResourceMimeType => FileResourceMimeType.AllLookupDictionary[FileResourceMimeTypeID];

        public static class FieldLengths
        {
            public const int FileResourceMimeTypeFileExtensionText = 100;
        }
    }
}