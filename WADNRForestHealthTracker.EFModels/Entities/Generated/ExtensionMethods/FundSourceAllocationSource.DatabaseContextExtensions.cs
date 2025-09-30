//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationSource]
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
        public static FundSourceAllocationSource GetFundSourceAllocationSource(this IQueryable<FundSourceAllocationSource> fundSourceAllocationSources, int fundSourceAllocationSourceID)
        {
            var fundSourceAllocationSource = fundSourceAllocationSources.SingleOrDefault(x => x.FundSourceAllocationSourceID == fundSourceAllocationSourceID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationSource, "FundSourceAllocationSource", fundSourceAllocationSourceID);
            return fundSourceAllocationSource;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationSource(this IQueryable<FundSourceAllocationSource> fundSourceAllocationSources, List<int> fundSourceAllocationSourceIDList)
        {
            if(fundSourceAllocationSourceIDList.Any())
            {
                var fundSourceAllocationSourcesInSourceCollectionToDelete = fundSourceAllocationSources.Where(x => fundSourceAllocationSourceIDList.Contains(x.FundSourceAllocationSourceID));
                foreach (var fundSourceAllocationSourceToDelete in fundSourceAllocationSourcesInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationSourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationSource(this IQueryable<FundSourceAllocationSource> fundSourceAllocationSources, ICollection<FundSourceAllocationSource> fundSourceAllocationSourcesToDelete)
        {
            if(fundSourceAllocationSourcesToDelete.Any())
            {
                var fundSourceAllocationSourceIDList = fundSourceAllocationSourcesToDelete.Select(x => x.FundSourceAllocationSourceID).ToList();
                var fundSourceAllocationSourcesToDeleteFromSourceList = fundSourceAllocationSources.Where(x => fundSourceAllocationSourceIDList.Contains(x.FundSourceAllocationSourceID)).ToList();

                foreach (var fundSourceAllocationSourceToDelete in fundSourceAllocationSourcesToDeleteFromSourceList)
                {
                    fundSourceAllocationSourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationSource(this IQueryable<FundSourceAllocationSource> fundSourceAllocationSources, int fundSourceAllocationSourceID)
        {
            DeleteFundSourceAllocationSource(fundSourceAllocationSources, new List<int> { fundSourceAllocationSourceID });
        }

        public static void DeleteFundSourceAllocationSource(this IQueryable<FundSourceAllocationSource> fundSourceAllocationSources, FundSourceAllocationSource fundSourceAllocationSourceToDelete)
        {
            DeleteFundSourceAllocationSource(fundSourceAllocationSources, new List<FundSourceAllocationSource> { fundSourceAllocationSourceToDelete });
        }
    }
}