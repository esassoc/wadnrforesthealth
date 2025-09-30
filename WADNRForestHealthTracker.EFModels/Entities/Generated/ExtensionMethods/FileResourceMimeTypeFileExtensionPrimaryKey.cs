//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FileResourceMimeTypeFileExtension
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FileResourceMimeTypeFileExtensionPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FileResourceMimeTypeFileExtension>
    {
        public FileResourceMimeTypeFileExtensionPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FileResourceMimeTypeFileExtensionPrimaryKey(FileResourceMimeTypeFileExtension fileResourceMimeTypeFileExtension) : base(fileResourceMimeTypeFileExtension){}

        public static implicit operator FileResourceMimeTypeFileExtensionPrimaryKey(int primaryKeyValue)
        {
            return new FileResourceMimeTypeFileExtensionPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FileResourceMimeTypeFileExtensionPrimaryKey(FileResourceMimeTypeFileExtension fileResourceMimeTypeFileExtension)
        {
            return new FileResourceMimeTypeFileExtensionPrimaryKey(fileResourceMimeTypeFileExtension);
        }
    }
}