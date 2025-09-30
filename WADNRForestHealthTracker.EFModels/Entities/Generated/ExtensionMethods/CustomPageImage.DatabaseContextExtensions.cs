//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomPageImage]
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
        public static CustomPageImage GetCustomPageImage(this IQueryable<CustomPageImage> customPageImages, int customPageImageID)
        {
            var customPageImage = customPageImages.SingleOrDefault(x => x.CustomPageImageID == customPageImageID);
            Check.RequireNotNullThrowNotFound(customPageImage, "CustomPageImage", customPageImageID);
            return customPageImage;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteCustomPageImage(this IQueryable<CustomPageImage> customPageImages, List<int> customPageImageIDList)
        {
            if(customPageImageIDList.Any())
            {
                var customPageImagesInSourceCollectionToDelete = customPageImages.Where(x => customPageImageIDList.Contains(x.CustomPageImageID));
                foreach (var customPageImageToDelete in customPageImagesInSourceCollectionToDelete.ToList())
                {
                    customPageImageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteCustomPageImage(this IQueryable<CustomPageImage> customPageImages, ICollection<CustomPageImage> customPageImagesToDelete)
        {
            if(customPageImagesToDelete.Any())
            {
                var customPageImageIDList = customPageImagesToDelete.Select(x => x.CustomPageImageID).ToList();
                var customPageImagesToDeleteFromSourceList = customPageImages.Where(x => customPageImageIDList.Contains(x.CustomPageImageID)).ToList();

                foreach (var customPageImageToDelete in customPageImagesToDeleteFromSourceList)
                {
                    customPageImageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteCustomPageImage(this IQueryable<CustomPageImage> customPageImages, int customPageImageID)
        {
            DeleteCustomPageImage(customPageImages, new List<int> { customPageImageID });
        }

        public static void DeleteCustomPageImage(this IQueryable<CustomPageImage> customPageImages, CustomPageImage customPageImageToDelete)
        {
            DeleteCustomPageImage(customPageImages, new List<CustomPageImage> { customPageImageToDelete });
        }
    }
}