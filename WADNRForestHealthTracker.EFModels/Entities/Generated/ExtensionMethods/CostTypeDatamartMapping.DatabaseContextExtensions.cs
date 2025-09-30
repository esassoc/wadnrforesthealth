//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CostTypeDatamartMapping]
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
        public static CostTypeDatamartMapping GetCostTypeDatamartMapping(this IQueryable<CostTypeDatamartMapping> costTypeDatamartMappings, int costTypeDatamartMappingID)
        {
            var costTypeDatamartMapping = costTypeDatamartMappings.SingleOrDefault(x => x.CostTypeDatamartMappingID == costTypeDatamartMappingID);
            Check.RequireNotNullThrowNotFound(costTypeDatamartMapping, "CostTypeDatamartMapping", costTypeDatamartMappingID);
            return costTypeDatamartMapping;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteCostTypeDatamartMapping(this IQueryable<CostTypeDatamartMapping> costTypeDatamartMappings, List<int> costTypeDatamartMappingIDList)
        {
            if(costTypeDatamartMappingIDList.Any())
            {
                var costTypeDatamartMappingsInSourceCollectionToDelete = costTypeDatamartMappings.Where(x => costTypeDatamartMappingIDList.Contains(x.CostTypeDatamartMappingID));
                foreach (var costTypeDatamartMappingToDelete in costTypeDatamartMappingsInSourceCollectionToDelete.ToList())
                {
                    costTypeDatamartMappingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteCostTypeDatamartMapping(this IQueryable<CostTypeDatamartMapping> costTypeDatamartMappings, ICollection<CostTypeDatamartMapping> costTypeDatamartMappingsToDelete)
        {
            if(costTypeDatamartMappingsToDelete.Any())
            {
                var costTypeDatamartMappingIDList = costTypeDatamartMappingsToDelete.Select(x => x.CostTypeDatamartMappingID).ToList();
                var costTypeDatamartMappingsToDeleteFromSourceList = costTypeDatamartMappings.Where(x => costTypeDatamartMappingIDList.Contains(x.CostTypeDatamartMappingID)).ToList();

                foreach (var costTypeDatamartMappingToDelete in costTypeDatamartMappingsToDeleteFromSourceList)
                {
                    costTypeDatamartMappingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteCostTypeDatamartMapping(this IQueryable<CostTypeDatamartMapping> costTypeDatamartMappings, int costTypeDatamartMappingID)
        {
            DeleteCostTypeDatamartMapping(costTypeDatamartMappings, new List<int> { costTypeDatamartMappingID });
        }

        public static void DeleteCostTypeDatamartMapping(this IQueryable<CostTypeDatamartMapping> costTypeDatamartMappings, CostTypeDatamartMapping costTypeDatamartMappingToDelete)
        {
            DeleteCostTypeDatamartMapping(costTypeDatamartMappings, new List<CostTypeDatamartMapping> { costTypeDatamartMappingToDelete });
        }
    }
}