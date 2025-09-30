//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ExternalMapLayer]
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
        public static ExternalMapLayer GetExternalMapLayer(this IQueryable<ExternalMapLayer> externalMapLayers, int externalMapLayerID)
        {
            var externalMapLayer = externalMapLayers.SingleOrDefault(x => x.ExternalMapLayerID == externalMapLayerID);
            Check.RequireNotNullThrowNotFound(externalMapLayer, "ExternalMapLayer", externalMapLayerID);
            return externalMapLayer;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteExternalMapLayer(this IQueryable<ExternalMapLayer> externalMapLayers, List<int> externalMapLayerIDList)
        {
            if(externalMapLayerIDList.Any())
            {
                var externalMapLayersInSourceCollectionToDelete = externalMapLayers.Where(x => externalMapLayerIDList.Contains(x.ExternalMapLayerID));
                foreach (var externalMapLayerToDelete in externalMapLayersInSourceCollectionToDelete.ToList())
                {
                    externalMapLayerToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteExternalMapLayer(this IQueryable<ExternalMapLayer> externalMapLayers, ICollection<ExternalMapLayer> externalMapLayersToDelete)
        {
            if(externalMapLayersToDelete.Any())
            {
                var externalMapLayerIDList = externalMapLayersToDelete.Select(x => x.ExternalMapLayerID).ToList();
                var externalMapLayersToDeleteFromSourceList = externalMapLayers.Where(x => externalMapLayerIDList.Contains(x.ExternalMapLayerID)).ToList();

                foreach (var externalMapLayerToDelete in externalMapLayersToDeleteFromSourceList)
                {
                    externalMapLayerToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteExternalMapLayer(this IQueryable<ExternalMapLayer> externalMapLayers, int externalMapLayerID)
        {
            DeleteExternalMapLayer(externalMapLayers, new List<int> { externalMapLayerID });
        }

        public static void DeleteExternalMapLayer(this IQueryable<ExternalMapLayer> externalMapLayers, ExternalMapLayer externalMapLayerToDelete)
        {
            DeleteExternalMapLayer(externalMapLayers, new List<ExternalMapLayer> { externalMapLayerToDelete });
        }
    }
}