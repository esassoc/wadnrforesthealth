//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocation]
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
        public static FundSourceAllocation GetFundSourceAllocation(this IQueryable<FundSourceAllocation> fundSourceAllocations, int fundSourceAllocationID)
        {
            var fundSourceAllocation = fundSourceAllocations.SingleOrDefault(x => x.FundSourceAllocationID == fundSourceAllocationID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocation, "FundSourceAllocation", fundSourceAllocationID);
            return fundSourceAllocation;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocation(this IQueryable<FundSourceAllocation> fundSourceAllocations, List<int> fundSourceAllocationIDList)
        {
            if(fundSourceAllocationIDList.Any())
            {
                var fundSourceAllocationsInSourceCollectionToDelete = fundSourceAllocations.Where(x => fundSourceAllocationIDList.Contains(x.FundSourceAllocationID));
                foreach (var fundSourceAllocationToDelete in fundSourceAllocationsInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocation(this IQueryable<FundSourceAllocation> fundSourceAllocations, ICollection<FundSourceAllocation> fundSourceAllocationsToDelete)
        {
            if(fundSourceAllocationsToDelete.Any())
            {
                var fundSourceAllocationIDList = fundSourceAllocationsToDelete.Select(x => x.FundSourceAllocationID).ToList();
                var fundSourceAllocationsToDeleteFromSourceList = fundSourceAllocations.Where(x => fundSourceAllocationIDList.Contains(x.FundSourceAllocationID)).ToList();

                foreach (var fundSourceAllocationToDelete in fundSourceAllocationsToDeleteFromSourceList)
                {
                    fundSourceAllocationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocation(this IQueryable<FundSourceAllocation> fundSourceAllocations, int fundSourceAllocationID)
        {
            DeleteFundSourceAllocation(fundSourceAllocations, new List<int> { fundSourceAllocationID });
        }

        public static void DeleteFundSourceAllocation(this IQueryable<FundSourceAllocation> fundSourceAllocations, FundSourceAllocation fundSourceAllocationToDelete)
        {
            DeleteFundSourceAllocation(fundSourceAllocations, new List<FundSourceAllocation> { fundSourceAllocationToDelete });
        }
    }
}