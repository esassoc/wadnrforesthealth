//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[DNRUplandRegionContentImage]
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
        public static DNRUplandRegionContentImage GetDNRUplandRegionContentImage(this IQueryable<DNRUplandRegionContentImage> dNRUplandRegionContentImages, int dNRUplandRegionContentImageID)
        {
            var dNRUplandRegionContentImage = dNRUplandRegionContentImages.SingleOrDefault(x => x.DNRUplandRegionContentImageID == dNRUplandRegionContentImageID);
            Check.RequireNotNullThrowNotFound(dNRUplandRegionContentImage, "DNRUplandRegionContentImage", dNRUplandRegionContentImageID);
            return dNRUplandRegionContentImage;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteDNRUplandRegionContentImage(this IQueryable<DNRUplandRegionContentImage> dNRUplandRegionContentImages, List<int> dNRUplandRegionContentImageIDList)
        {
            if(dNRUplandRegionContentImageIDList.Any())
            {
                var dNRUplandRegionContentImagesInSourceCollectionToDelete = dNRUplandRegionContentImages.Where(x => dNRUplandRegionContentImageIDList.Contains(x.DNRUplandRegionContentImageID));
                foreach (var dNRUplandRegionContentImageToDelete in dNRUplandRegionContentImagesInSourceCollectionToDelete.ToList())
                {
                    dNRUplandRegionContentImageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteDNRUplandRegionContentImage(this IQueryable<DNRUplandRegionContentImage> dNRUplandRegionContentImages, ICollection<DNRUplandRegionContentImage> dNRUplandRegionContentImagesToDelete)
        {
            if(dNRUplandRegionContentImagesToDelete.Any())
            {
                var dNRUplandRegionContentImageIDList = dNRUplandRegionContentImagesToDelete.Select(x => x.DNRUplandRegionContentImageID).ToList();
                var dNRUplandRegionContentImagesToDeleteFromSourceList = dNRUplandRegionContentImages.Where(x => dNRUplandRegionContentImageIDList.Contains(x.DNRUplandRegionContentImageID)).ToList();

                foreach (var dNRUplandRegionContentImageToDelete in dNRUplandRegionContentImagesToDeleteFromSourceList)
                {
                    dNRUplandRegionContentImageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteDNRUplandRegionContentImage(this IQueryable<DNRUplandRegionContentImage> dNRUplandRegionContentImages, int dNRUplandRegionContentImageID)
        {
            DeleteDNRUplandRegionContentImage(dNRUplandRegionContentImages, new List<int> { dNRUplandRegionContentImageID });
        }

        public static void DeleteDNRUplandRegionContentImage(this IQueryable<DNRUplandRegionContentImage> dNRUplandRegionContentImages, DNRUplandRegionContentImage dNRUplandRegionContentImageToDelete)
        {
            DeleteDNRUplandRegionContentImage(dNRUplandRegionContentImages, new List<DNRUplandRegionContentImage> { dNRUplandRegionContentImageToDelete });
        }
    }
}