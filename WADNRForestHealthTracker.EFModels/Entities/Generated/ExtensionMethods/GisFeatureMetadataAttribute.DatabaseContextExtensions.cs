//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisFeatureMetadataAttribute]
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
        public static GisFeatureMetadataAttribute GetGisFeatureMetadataAttribute(this IQueryable<GisFeatureMetadataAttribute> gisFeatureMetadataAttributes, int gisFeatureMetadataAttributeID)
        {
            var gisFeatureMetadataAttribute = gisFeatureMetadataAttributes.SingleOrDefault(x => x.GisFeatureMetadataAttributeID == gisFeatureMetadataAttributeID);
            Check.RequireNotNullThrowNotFound(gisFeatureMetadataAttribute, "GisFeatureMetadataAttribute", gisFeatureMetadataAttributeID);
            return gisFeatureMetadataAttribute;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisFeatureMetadataAttribute(this IQueryable<GisFeatureMetadataAttribute> gisFeatureMetadataAttributes, List<int> gisFeatureMetadataAttributeIDList)
        {
            if(gisFeatureMetadataAttributeIDList.Any())
            {
                var gisFeatureMetadataAttributesInSourceCollectionToDelete = gisFeatureMetadataAttributes.Where(x => gisFeatureMetadataAttributeIDList.Contains(x.GisFeatureMetadataAttributeID));
                foreach (var gisFeatureMetadataAttributeToDelete in gisFeatureMetadataAttributesInSourceCollectionToDelete.ToList())
                {
                    gisFeatureMetadataAttributeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisFeatureMetadataAttribute(this IQueryable<GisFeatureMetadataAttribute> gisFeatureMetadataAttributes, ICollection<GisFeatureMetadataAttribute> gisFeatureMetadataAttributesToDelete)
        {
            if(gisFeatureMetadataAttributesToDelete.Any())
            {
                var gisFeatureMetadataAttributeIDList = gisFeatureMetadataAttributesToDelete.Select(x => x.GisFeatureMetadataAttributeID).ToList();
                var gisFeatureMetadataAttributesToDeleteFromSourceList = gisFeatureMetadataAttributes.Where(x => gisFeatureMetadataAttributeIDList.Contains(x.GisFeatureMetadataAttributeID)).ToList();

                foreach (var gisFeatureMetadataAttributeToDelete in gisFeatureMetadataAttributesToDeleteFromSourceList)
                {
                    gisFeatureMetadataAttributeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisFeatureMetadataAttribute(this IQueryable<GisFeatureMetadataAttribute> gisFeatureMetadataAttributes, int gisFeatureMetadataAttributeID)
        {
            DeleteGisFeatureMetadataAttribute(gisFeatureMetadataAttributes, new List<int> { gisFeatureMetadataAttributeID });
        }

        public static void DeleteGisFeatureMetadataAttribute(this IQueryable<GisFeatureMetadataAttribute> gisFeatureMetadataAttributes, GisFeatureMetadataAttribute gisFeatureMetadataAttributeToDelete)
        {
            DeleteGisFeatureMetadataAttribute(gisFeatureMetadataAttributes, new List<GisFeatureMetadataAttribute> { gisFeatureMetadataAttributeToDelete });
        }
    }
}