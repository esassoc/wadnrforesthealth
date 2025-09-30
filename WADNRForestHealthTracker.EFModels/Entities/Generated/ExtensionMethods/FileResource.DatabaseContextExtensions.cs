//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResource]
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
        public static FileResource GetFileResource(this IQueryable<FileResource> fileResources, int fileResourceID)
        {
            var fileResource = fileResources.SingleOrDefault(x => x.FileResourceID == fileResourceID);
            Check.RequireNotNullThrowNotFound(fileResource, "FileResource", fileResourceID);
            return fileResource;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFileResource(this IQueryable<FileResource> fileResources, List<int> fileResourceIDList)
        {
            if(fileResourceIDList.Any())
            {
                var fileResourcesInSourceCollectionToDelete = fileResources.Where(x => fileResourceIDList.Contains(x.FileResourceID));
                foreach (var fileResourceToDelete in fileResourcesInSourceCollectionToDelete.ToList())
                {
                    fileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFileResource(this IQueryable<FileResource> fileResources, ICollection<FileResource> fileResourcesToDelete)
        {
            if(fileResourcesToDelete.Any())
            {
                var fileResourceIDList = fileResourcesToDelete.Select(x => x.FileResourceID).ToList();
                var fileResourcesToDeleteFromSourceList = fileResources.Where(x => fileResourceIDList.Contains(x.FileResourceID)).ToList();

                foreach (var fileResourceToDelete in fileResourcesToDeleteFromSourceList)
                {
                    fileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFileResource(this IQueryable<FileResource> fileResources, int fileResourceID)
        {
            DeleteFileResource(fileResources, new List<int> { fileResourceID });
        }

        public static void DeleteFileResource(this IQueryable<FileResource> fileResources, FileResource fileResourceToDelete)
        {
            DeleteFileResource(fileResources, new List<FileResource> { fileResourceToDelete });
        }
    }
}