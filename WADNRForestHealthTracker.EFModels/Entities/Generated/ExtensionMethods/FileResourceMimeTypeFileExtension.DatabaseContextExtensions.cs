//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResourceMimeTypeFileExtension]
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public static partial class DatabaseContextExtensions
    {
        public static FileResourceMimeTypeFileExtension GetFileResourceMimeTypeFileExtension(this IQueryable<FileResourceMimeTypeFileExtension> fileResourceMimeTypeFileExtensions, int fileResourceMimeTypeFileExtensionID)
        {
            var fileResourceMimeTypeFileExtension = fileResourceMimeTypeFileExtensions.SingleOrDefault(x => x.FileResourceMimeTypeFileExtensionID == fileResourceMimeTypeFileExtensionID);
            Check.RequireNotNullThrowNotFound(fileResourceMimeTypeFileExtension, "FileResourceMimeTypeFileExtension", fileResourceMimeTypeFileExtensionID);
            return fileResourceMimeTypeFileExtension;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFileResourceMimeTypeFileExtension(this IQueryable<FileResourceMimeTypeFileExtension> fileResourceMimeTypeFileExtensions, List<int> fileResourceMimeTypeFileExtensionIDList)
        {
            if(fileResourceMimeTypeFileExtensionIDList.Any())
            {
                var fileResourceMimeTypeFileExtensionsInSourceCollectionToDelete = fileResourceMimeTypeFileExtensions.Where(x => fileResourceMimeTypeFileExtensionIDList.Contains(x.FileResourceMimeTypeFileExtensionID));
                foreach (var fileResourceMimeTypeFileExtensionToDelete in fileResourceMimeTypeFileExtensionsInSourceCollectionToDelete.ToList())
                {
                    fileResourceMimeTypeFileExtensionToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFileResourceMimeTypeFileExtension(this IQueryable<FileResourceMimeTypeFileExtension> fileResourceMimeTypeFileExtensions, ICollection<FileResourceMimeTypeFileExtension> fileResourceMimeTypeFileExtensionsToDelete)
        {
            if(fileResourceMimeTypeFileExtensionsToDelete.Any())
            {
                var fileResourceMimeTypeFileExtensionIDList = fileResourceMimeTypeFileExtensionsToDelete.Select(x => x.FileResourceMimeTypeFileExtensionID).ToList();
                var fileResourceMimeTypeFileExtensionsToDeleteFromSourceList = fileResourceMimeTypeFileExtensions.Where(x => fileResourceMimeTypeFileExtensionIDList.Contains(x.FileResourceMimeTypeFileExtensionID)).ToList();

                foreach (var fileResourceMimeTypeFileExtensionToDelete in fileResourceMimeTypeFileExtensionsToDeleteFromSourceList)
                {
                    fileResourceMimeTypeFileExtensionToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFileResourceMimeTypeFileExtension(this IQueryable<FileResourceMimeTypeFileExtension> fileResourceMimeTypeFileExtensions, int fileResourceMimeTypeFileExtensionID)
        {
            DeleteFileResourceMimeTypeFileExtension(fileResourceMimeTypeFileExtensions, new List<int> { fileResourceMimeTypeFileExtensionID });
        }

        public static void DeleteFileResourceMimeTypeFileExtension(this IQueryable<FileResourceMimeTypeFileExtension> fileResourceMimeTypeFileExtensions, FileResourceMimeTypeFileExtension fileResourceMimeTypeFileExtensionToDelete)
        {
            DeleteFileResourceMimeTypeFileExtension(fileResourceMimeTypeFileExtensions, new List<FileResourceMimeTypeFileExtension> { fileResourceMimeTypeFileExtensionToDelete });
        }
    }
}