//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisFeature]
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
        public static GisFeature GetGisFeature(this IQueryable<GisFeature> gisFeatures, int gisFeatureID)
        {
            var gisFeature = gisFeatures.SingleOrDefault(x => x.GisFeatureID == gisFeatureID);
            Check.RequireNotNullThrowNotFound(gisFeature, "GisFeature", gisFeatureID);
            return gisFeature;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisFeature(this IQueryable<GisFeature> gisFeatures, List<int> gisFeatureIDList)
        {
            if(gisFeatureIDList.Any())
            {
                var gisFeaturesInSourceCollectionToDelete = gisFeatures.Where(x => gisFeatureIDList.Contains(x.GisFeatureID));
                foreach (var gisFeatureToDelete in gisFeaturesInSourceCollectionToDelete.ToList())
                {
                    gisFeatureToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisFeature(this IQueryable<GisFeature> gisFeatures, ICollection<GisFeature> gisFeaturesToDelete)
        {
            if(gisFeaturesToDelete.Any())
            {
                var gisFeatureIDList = gisFeaturesToDelete.Select(x => x.GisFeatureID).ToList();
                var gisFeaturesToDeleteFromSourceList = gisFeatures.Where(x => gisFeatureIDList.Contains(x.GisFeatureID)).ToList();

                foreach (var gisFeatureToDelete in gisFeaturesToDeleteFromSourceList)
                {
                    gisFeatureToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisFeature(this IQueryable<GisFeature> gisFeatures, int gisFeatureID)
        {
            DeleteGisFeature(gisFeatures, new List<int> { gisFeatureID });
        }

        public static void DeleteGisFeature(this IQueryable<GisFeature> gisFeatures, GisFeature gisFeatureToDelete)
        {
            DeleteGisFeature(gisFeatures, new List<GisFeature> { gisFeatureToDelete });
        }
    }
}