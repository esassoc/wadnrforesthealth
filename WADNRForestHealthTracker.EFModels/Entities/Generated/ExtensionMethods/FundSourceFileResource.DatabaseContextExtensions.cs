//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceFileResource]
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
        public static FundSourceFileResource GetFundSourceFileResource(this IQueryable<FundSourceFileResource> fundSourceFileResources, int fundSourceFileResourceID)
        {
            var fundSourceFileResource = fundSourceFileResources.SingleOrDefault(x => x.FundSourceFileResourceID == fundSourceFileResourceID);
            Check.RequireNotNullThrowNotFound(fundSourceFileResource, "FundSourceFileResource", fundSourceFileResourceID);
            return fundSourceFileResource;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceFileResource(this IQueryable<FundSourceFileResource> fundSourceFileResources, List<int> fundSourceFileResourceIDList)
        {
            if(fundSourceFileResourceIDList.Any())
            {
                var fundSourceFileResourcesInSourceCollectionToDelete = fundSourceFileResources.Where(x => fundSourceFileResourceIDList.Contains(x.FundSourceFileResourceID));
                foreach (var fundSourceFileResourceToDelete in fundSourceFileResourcesInSourceCollectionToDelete.ToList())
                {
                    fundSourceFileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceFileResource(this IQueryable<FundSourceFileResource> fundSourceFileResources, ICollection<FundSourceFileResource> fundSourceFileResourcesToDelete)
        {
            if(fundSourceFileResourcesToDelete.Any())
            {
                var fundSourceFileResourceIDList = fundSourceFileResourcesToDelete.Select(x => x.FundSourceFileResourceID).ToList();
                var fundSourceFileResourcesToDeleteFromSourceList = fundSourceFileResources.Where(x => fundSourceFileResourceIDList.Contains(x.FundSourceFileResourceID)).ToList();

                foreach (var fundSourceFileResourceToDelete in fundSourceFileResourcesToDeleteFromSourceList)
                {
                    fundSourceFileResourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceFileResource(this IQueryable<FundSourceFileResource> fundSourceFileResources, int fundSourceFileResourceID)
        {
            DeleteFundSourceFileResource(fundSourceFileResources, new List<int> { fundSourceFileResourceID });
        }

        public static void DeleteFundSourceFileResource(this IQueryable<FundSourceFileResource> fundSourceFileResources, FundSourceFileResource fundSourceFileResourceToDelete)
        {
            DeleteFundSourceFileResource(fundSourceFileResources, new List<FundSourceFileResource> { fundSourceFileResourceToDelete });
        }
    }
}