//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSource]
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
        public static FundSource GetFundSource(this IQueryable<FundSource> fundSources, int fundSourceID)
        {
            var fundSource = fundSources.SingleOrDefault(x => x.FundSourceID == fundSourceID);
            Check.RequireNotNullThrowNotFound(fundSource, "FundSource", fundSourceID);
            return fundSource;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSource(this IQueryable<FundSource> fundSources, List<int> fundSourceIDList)
        {
            if(fundSourceIDList.Any())
            {
                var fundSourcesInSourceCollectionToDelete = fundSources.Where(x => fundSourceIDList.Contains(x.FundSourceID));
                foreach (var fundSourceToDelete in fundSourcesInSourceCollectionToDelete.ToList())
                {
                    fundSourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSource(this IQueryable<FundSource> fundSources, ICollection<FundSource> fundSourcesToDelete)
        {
            if(fundSourcesToDelete.Any())
            {
                var fundSourceIDList = fundSourcesToDelete.Select(x => x.FundSourceID).ToList();
                var fundSourcesToDeleteFromSourceList = fundSources.Where(x => fundSourceIDList.Contains(x.FundSourceID)).ToList();

                foreach (var fundSourceToDelete in fundSourcesToDeleteFromSourceList)
                {
                    fundSourceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSource(this IQueryable<FundSource> fundSources, int fundSourceID)
        {
            DeleteFundSource(fundSources, new List<int> { fundSourceID });
        }

        public static void DeleteFundSource(this IQueryable<FundSource> fundSources, FundSource fundSourceToDelete)
        {
            DeleteFundSource(fundSources, new List<FundSource> { fundSourceToDelete });
        }
    }
}