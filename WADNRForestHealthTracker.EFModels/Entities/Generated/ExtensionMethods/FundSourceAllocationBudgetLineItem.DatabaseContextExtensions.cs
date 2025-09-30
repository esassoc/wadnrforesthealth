//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationBudgetLineItem]
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
        public static FundSourceAllocationBudgetLineItem GetFundSourceAllocationBudgetLineItem(this IQueryable<FundSourceAllocationBudgetLineItem> fundSourceAllocationBudgetLineItems, int fundSourceAllocationBudgetLineItemID)
        {
            var fundSourceAllocationBudgetLineItem = fundSourceAllocationBudgetLineItems.SingleOrDefault(x => x.FundSourceAllocationBudgetLineItemID == fundSourceAllocationBudgetLineItemID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationBudgetLineItem, "FundSourceAllocationBudgetLineItem", fundSourceAllocationBudgetLineItemID);
            return fundSourceAllocationBudgetLineItem;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationBudgetLineItem(this IQueryable<FundSourceAllocationBudgetLineItem> fundSourceAllocationBudgetLineItems, List<int> fundSourceAllocationBudgetLineItemIDList)
        {
            if(fundSourceAllocationBudgetLineItemIDList.Any())
            {
                var fundSourceAllocationBudgetLineItemsInSourceCollectionToDelete = fundSourceAllocationBudgetLineItems.Where(x => fundSourceAllocationBudgetLineItemIDList.Contains(x.FundSourceAllocationBudgetLineItemID));
                foreach (var fundSourceAllocationBudgetLineItemToDelete in fundSourceAllocationBudgetLineItemsInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationBudgetLineItemToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationBudgetLineItem(this IQueryable<FundSourceAllocationBudgetLineItem> fundSourceAllocationBudgetLineItems, ICollection<FundSourceAllocationBudgetLineItem> fundSourceAllocationBudgetLineItemsToDelete)
        {
            if(fundSourceAllocationBudgetLineItemsToDelete.Any())
            {
                var fundSourceAllocationBudgetLineItemIDList = fundSourceAllocationBudgetLineItemsToDelete.Select(x => x.FundSourceAllocationBudgetLineItemID).ToList();
                var fundSourceAllocationBudgetLineItemsToDeleteFromSourceList = fundSourceAllocationBudgetLineItems.Where(x => fundSourceAllocationBudgetLineItemIDList.Contains(x.FundSourceAllocationBudgetLineItemID)).ToList();

                foreach (var fundSourceAllocationBudgetLineItemToDelete in fundSourceAllocationBudgetLineItemsToDeleteFromSourceList)
                {
                    fundSourceAllocationBudgetLineItemToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationBudgetLineItem(this IQueryable<FundSourceAllocationBudgetLineItem> fundSourceAllocationBudgetLineItems, int fundSourceAllocationBudgetLineItemID)
        {
            DeleteFundSourceAllocationBudgetLineItem(fundSourceAllocationBudgetLineItems, new List<int> { fundSourceAllocationBudgetLineItemID });
        }

        public static void DeleteFundSourceAllocationBudgetLineItem(this IQueryable<FundSourceAllocationBudgetLineItem> fundSourceAllocationBudgetLineItems, FundSourceAllocationBudgetLineItem fundSourceAllocationBudgetLineItemToDelete)
        {
            DeleteFundSourceAllocationBudgetLineItem(fundSourceAllocationBudgetLineItems, new List<FundSourceAllocationBudgetLineItem> { fundSourceAllocationBudgetLineItemToDelete });
        }
    }
}