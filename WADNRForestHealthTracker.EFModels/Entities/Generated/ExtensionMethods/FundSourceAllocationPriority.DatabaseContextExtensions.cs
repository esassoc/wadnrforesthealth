//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationPriority]
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
        public static FundSourceAllocationPriority GetFundSourceAllocationPriority(this IQueryable<FundSourceAllocationPriority> fundSourceAllocationPriorities, int fundSourceAllocationPriorityID)
        {
            var fundSourceAllocationPriority = fundSourceAllocationPriorities.SingleOrDefault(x => x.FundSourceAllocationPriorityID == fundSourceAllocationPriorityID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationPriority, "FundSourceAllocationPriority", fundSourceAllocationPriorityID);
            return fundSourceAllocationPriority;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationPriority(this IQueryable<FundSourceAllocationPriority> fundSourceAllocationPriorities, List<int> fundSourceAllocationPriorityIDList)
        {
            if(fundSourceAllocationPriorityIDList.Any())
            {
                var fundSourceAllocationPrioritiesInSourceCollectionToDelete = fundSourceAllocationPriorities.Where(x => fundSourceAllocationPriorityIDList.Contains(x.FundSourceAllocationPriorityID));
                foreach (var fundSourceAllocationPriorityToDelete in fundSourceAllocationPrioritiesInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationPriorityToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationPriority(this IQueryable<FundSourceAllocationPriority> fundSourceAllocationPriorities, ICollection<FundSourceAllocationPriority> fundSourceAllocationPrioritiesToDelete)
        {
            if(fundSourceAllocationPrioritiesToDelete.Any())
            {
                var fundSourceAllocationPriorityIDList = fundSourceAllocationPrioritiesToDelete.Select(x => x.FundSourceAllocationPriorityID).ToList();
                var fundSourceAllocationPrioritiesToDeleteFromSourceList = fundSourceAllocationPriorities.Where(x => fundSourceAllocationPriorityIDList.Contains(x.FundSourceAllocationPriorityID)).ToList();

                foreach (var fundSourceAllocationPriorityToDelete in fundSourceAllocationPrioritiesToDeleteFromSourceList)
                {
                    fundSourceAllocationPriorityToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationPriority(this IQueryable<FundSourceAllocationPriority> fundSourceAllocationPriorities, int fundSourceAllocationPriorityID)
        {
            DeleteFundSourceAllocationPriority(fundSourceAllocationPriorities, new List<int> { fundSourceAllocationPriorityID });
        }

        public static void DeleteFundSourceAllocationPriority(this IQueryable<FundSourceAllocationPriority> fundSourceAllocationPriorities, FundSourceAllocationPriority fundSourceAllocationPriorityToDelete)
        {
            DeleteFundSourceAllocationPriority(fundSourceAllocationPriorities, new List<FundSourceAllocationPriority> { fundSourceAllocationPriorityToDelete });
        }
    }
}