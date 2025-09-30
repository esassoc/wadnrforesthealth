//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationFileResource]
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
        public static FundSourceAllocationFileResource GetFundSourceAllocationFileResource(this IQueryable<FundSourceAllocationFileResource> fundSourceAllocationFileResources, int fundSourceAllocationFileResourceID)
        {
            var fundSourceAllocationFileResource = fundSourceAllocationFileResources.SingleOrDefault(x => x.FundSourceAllocationFileResourceID == fundSourceAllocationFileResourceID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationFileResource, "FundSourceAllocationFileResource", fundSourceAllocationFileResourceID);
            return fundSourceAllocationFileResource;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationFileResource(this IQueryable<FundSourceAllocationFileResource> fundSourceAllocationFileResources, List<int> fundSourceAllocationFileResourceIDList)
        {
            if(fundSourceAllocationFileResourceIDList.Any())
            {
                var fundSourceAllocationFileResourcesInSourceCollectionToDelete = fundSourceAllocationFileResources.Where(x => fundSourceAllocationFileResourceIDList.Contains(x.FundSourceAllocationFileResourceID));
                foreach (var fundSourceAllocationFileResourceToDelete in fundSourceAllocationFileResourcesInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationFileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationFileResource(this IQueryable<FundSourceAllocationFileResource> fundSourceAllocationFileResources, ICollection<FundSourceAllocationFileResource> fundSourceAllocationFileResourcesToDelete)
        {
            if(fundSourceAllocationFileResourcesToDelete.Any())
            {
                var fundSourceAllocationFileResourceIDList = fundSourceAllocationFileResourcesToDelete.Select(x => x.FundSourceAllocationFileResourceID).ToList();
                var fundSourceAllocationFileResourcesToDeleteFromSourceList = fundSourceAllocationFileResources.Where(x => fundSourceAllocationFileResourceIDList.Contains(x.FundSourceAllocationFileResourceID)).ToList();

                foreach (var fundSourceAllocationFileResourceToDelete in fundSourceAllocationFileResourcesToDeleteFromSourceList)
                {
                    fundSourceAllocationFileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationFileResource(this IQueryable<FundSourceAllocationFileResource> fundSourceAllocationFileResources, int fundSourceAllocationFileResourceID)
        {
            DeleteFundSourceAllocationFileResource(fundSourceAllocationFileResources, new List<int> { fundSourceAllocationFileResourceID });
        }

        public static void DeleteFundSourceAllocationFileResource(this IQueryable<FundSourceAllocationFileResource> fundSourceAllocationFileResources, FundSourceAllocationFileResource fundSourceAllocationFileResourceToDelete)
        {
            DeleteFundSourceAllocationFileResource(fundSourceAllocationFileResources, new List<FundSourceAllocationFileResource> { fundSourceAllocationFileResourceToDelete });
        }
    }
}