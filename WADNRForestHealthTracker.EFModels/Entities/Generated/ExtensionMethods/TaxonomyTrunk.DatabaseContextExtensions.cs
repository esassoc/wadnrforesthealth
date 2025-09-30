//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TaxonomyTrunk]
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
        public static TaxonomyTrunk GetTaxonomyTrunk(this IQueryable<TaxonomyTrunk> taxonomyTrunks, int taxonomyTrunkID)
        {
            var taxonomyTrunk = taxonomyTrunks.SingleOrDefault(x => x.TaxonomyTrunkID == taxonomyTrunkID);
            Check.RequireNotNullThrowNotFound(taxonomyTrunk, "TaxonomyTrunk", taxonomyTrunkID);
            return taxonomyTrunk;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTaxonomyTrunk(this IQueryable<TaxonomyTrunk> taxonomyTrunks, List<int> taxonomyTrunkIDList)
        {
            if(taxonomyTrunkIDList.Any())
            {
                var taxonomyTrunksInSourceCollectionToDelete = taxonomyTrunks.Where(x => taxonomyTrunkIDList.Contains(x.TaxonomyTrunkID));
                foreach (var taxonomyTrunkToDelete in taxonomyTrunksInSourceCollectionToDelete.ToList())
                {
                    taxonomyTrunkToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTaxonomyTrunk(this IQueryable<TaxonomyTrunk> taxonomyTrunks, ICollection<TaxonomyTrunk> taxonomyTrunksToDelete)
        {
            if(taxonomyTrunksToDelete.Any())
            {
                var taxonomyTrunkIDList = taxonomyTrunksToDelete.Select(x => x.TaxonomyTrunkID).ToList();
                var taxonomyTrunksToDeleteFromSourceList = taxonomyTrunks.Where(x => taxonomyTrunkIDList.Contains(x.TaxonomyTrunkID)).ToList();

                foreach (var taxonomyTrunkToDelete in taxonomyTrunksToDeleteFromSourceList)
                {
                    taxonomyTrunkToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTaxonomyTrunk(this IQueryable<TaxonomyTrunk> taxonomyTrunks, int taxonomyTrunkID)
        {
            DeleteTaxonomyTrunk(taxonomyTrunks, new List<int> { taxonomyTrunkID });
        }

        public static void DeleteTaxonomyTrunk(this IQueryable<TaxonomyTrunk> taxonomyTrunks, TaxonomyTrunk taxonomyTrunkToDelete)
        {
            DeleteTaxonomyTrunk(taxonomyTrunks, new List<TaxonomyTrunk> { taxonomyTrunkToDelete });
        }
    }
}