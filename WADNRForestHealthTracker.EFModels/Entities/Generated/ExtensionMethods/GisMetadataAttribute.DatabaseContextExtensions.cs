//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisMetadataAttribute]
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
        public static GisMetadataAttribute GetGisMetadataAttribute(this IQueryable<GisMetadataAttribute> gisMetadataAttributes, int gisMetadataAttributeID)
        {
            var gisMetadataAttribute = gisMetadataAttributes.SingleOrDefault(x => x.GisMetadataAttributeID == gisMetadataAttributeID);
            Check.RequireNotNullThrowNotFound(gisMetadataAttribute, "GisMetadataAttribute", gisMetadataAttributeID);
            return gisMetadataAttribute;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisMetadataAttribute(this IQueryable<GisMetadataAttribute> gisMetadataAttributes, List<int> gisMetadataAttributeIDList)
        {
            if(gisMetadataAttributeIDList.Any())
            {
                var gisMetadataAttributesInSourceCollectionToDelete = gisMetadataAttributes.Where(x => gisMetadataAttributeIDList.Contains(x.GisMetadataAttributeID));
                foreach (var gisMetadataAttributeToDelete in gisMetadataAttributesInSourceCollectionToDelete.ToList())
                {
                    gisMetadataAttributeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisMetadataAttribute(this IQueryable<GisMetadataAttribute> gisMetadataAttributes, ICollection<GisMetadataAttribute> gisMetadataAttributesToDelete)
        {
            if(gisMetadataAttributesToDelete.Any())
            {
                var gisMetadataAttributeIDList = gisMetadataAttributesToDelete.Select(x => x.GisMetadataAttributeID).ToList();
                var gisMetadataAttributesToDeleteFromSourceList = gisMetadataAttributes.Where(x => gisMetadataAttributeIDList.Contains(x.GisMetadataAttributeID)).ToList();

                foreach (var gisMetadataAttributeToDelete in gisMetadataAttributesToDeleteFromSourceList)
                {
                    gisMetadataAttributeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisMetadataAttribute(this IQueryable<GisMetadataAttribute> gisMetadataAttributes, int gisMetadataAttributeID)
        {
            DeleteGisMetadataAttribute(gisMetadataAttributes, new List<int> { gisMetadataAttributeID });
        }

        public static void DeleteGisMetadataAttribute(this IQueryable<GisMetadataAttribute> gisMetadataAttributes, GisMetadataAttribute gisMetadataAttributeToDelete)
        {
            DeleteGisMetadataAttribute(gisMetadataAttributes, new List<GisMetadataAttribute> { gisMetadataAttributeToDelete });
        }
    }
}