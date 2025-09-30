//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadAttemptGisMetadataAttribute]
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
        public static GisUploadAttemptGisMetadataAttribute GetGisUploadAttemptGisMetadataAttribute(this IQueryable<GisUploadAttemptGisMetadataAttribute> gisUploadAttemptGisMetadataAttributes, int gisUploadAttemptGisMetadataAttributeID)
        {
            var gisUploadAttemptGisMetadataAttribute = gisUploadAttemptGisMetadataAttributes.SingleOrDefault(x => x.GisUploadAttemptGisMetadataAttributeID == gisUploadAttemptGisMetadataAttributeID);
            Check.RequireNotNullThrowNotFound(gisUploadAttemptGisMetadataAttribute, "GisUploadAttemptGisMetadataAttribute", gisUploadAttemptGisMetadataAttributeID);
            return gisUploadAttemptGisMetadataAttribute;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisUploadAttemptGisMetadataAttribute(this IQueryable<GisUploadAttemptGisMetadataAttribute> gisUploadAttemptGisMetadataAttributes, List<int> gisUploadAttemptGisMetadataAttributeIDList)
        {
            if(gisUploadAttemptGisMetadataAttributeIDList.Any())
            {
                var gisUploadAttemptGisMetadataAttributesInSourceCollectionToDelete = gisUploadAttemptGisMetadataAttributes.Where(x => gisUploadAttemptGisMetadataAttributeIDList.Contains(x.GisUploadAttemptGisMetadataAttributeID));
                foreach (var gisUploadAttemptGisMetadataAttributeToDelete in gisUploadAttemptGisMetadataAttributesInSourceCollectionToDelete.ToList())
                {
                    gisUploadAttemptGisMetadataAttributeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisUploadAttemptGisMetadataAttribute(this IQueryable<GisUploadAttemptGisMetadataAttribute> gisUploadAttemptGisMetadataAttributes, ICollection<GisUploadAttemptGisMetadataAttribute> gisUploadAttemptGisMetadataAttributesToDelete)
        {
            if(gisUploadAttemptGisMetadataAttributesToDelete.Any())
            {
                var gisUploadAttemptGisMetadataAttributeIDList = gisUploadAttemptGisMetadataAttributesToDelete.Select(x => x.GisUploadAttemptGisMetadataAttributeID).ToList();
                var gisUploadAttemptGisMetadataAttributesToDeleteFromSourceList = gisUploadAttemptGisMetadataAttributes.Where(x => gisUploadAttemptGisMetadataAttributeIDList.Contains(x.GisUploadAttemptGisMetadataAttributeID)).ToList();

                foreach (var gisUploadAttemptGisMetadataAttributeToDelete in gisUploadAttemptGisMetadataAttributesToDeleteFromSourceList)
                {
                    gisUploadAttemptGisMetadataAttributeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisUploadAttemptGisMetadataAttribute(this IQueryable<GisUploadAttemptGisMetadataAttribute> gisUploadAttemptGisMetadataAttributes, int gisUploadAttemptGisMetadataAttributeID)
        {
            DeleteGisUploadAttemptGisMetadataAttribute(gisUploadAttemptGisMetadataAttributes, new List<int> { gisUploadAttemptGisMetadataAttributeID });
        }

        public static void DeleteGisUploadAttemptGisMetadataAttribute(this IQueryable<GisUploadAttemptGisMetadataAttribute> gisUploadAttemptGisMetadataAttributes, GisUploadAttemptGisMetadataAttribute gisUploadAttemptGisMetadataAttributeToDelete)
        {
            DeleteGisUploadAttemptGisMetadataAttribute(gisUploadAttemptGisMetadataAttributes, new List<GisUploadAttemptGisMetadataAttribute> { gisUploadAttemptGisMetadataAttributeToDelete });
        }
    }
}