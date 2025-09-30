//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TaxonomyBranch]
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
        public static TaxonomyBranch GetTaxonomyBranch(this IQueryable<TaxonomyBranch> taxonomyBranches, int taxonomyBranchID)
        {
            var taxonomyBranch = taxonomyBranches.SingleOrDefault(x => x.TaxonomyBranchID == taxonomyBranchID);
            Check.RequireNotNullThrowNotFound(taxonomyBranch, "TaxonomyBranch", taxonomyBranchID);
            return taxonomyBranch;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTaxonomyBranch(this IQueryable<TaxonomyBranch> taxonomyBranches, List<int> taxonomyBranchIDList)
        {
            if(taxonomyBranchIDList.Any())
            {
                var taxonomyBranchesInSourceCollectionToDelete = taxonomyBranches.Where(x => taxonomyBranchIDList.Contains(x.TaxonomyBranchID));
                foreach (var taxonomyBranchToDelete in taxonomyBranchesInSourceCollectionToDelete.ToList())
                {
                    taxonomyBranchToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTaxonomyBranch(this IQueryable<TaxonomyBranch> taxonomyBranches, ICollection<TaxonomyBranch> taxonomyBranchesToDelete)
        {
            if(taxonomyBranchesToDelete.Any())
            {
                var taxonomyBranchIDList = taxonomyBranchesToDelete.Select(x => x.TaxonomyBranchID).ToList();
                var taxonomyBranchesToDeleteFromSourceList = taxonomyBranches.Where(x => taxonomyBranchIDList.Contains(x.TaxonomyBranchID)).ToList();

                foreach (var taxonomyBranchToDelete in taxonomyBranchesToDeleteFromSourceList)
                {
                    taxonomyBranchToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTaxonomyBranch(this IQueryable<TaxonomyBranch> taxonomyBranches, int taxonomyBranchID)
        {
            DeleteTaxonomyBranch(taxonomyBranches, new List<int> { taxonomyBranchID });
        }

        public static void DeleteTaxonomyBranch(this IQueryable<TaxonomyBranch> taxonomyBranches, TaxonomyBranch taxonomyBranchToDelete)
        {
            DeleteTaxonomyBranch(taxonomyBranches, new List<TaxonomyBranch> { taxonomyBranchToDelete });
        }
    }
}