//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationExpenditure]
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
        public static FundSourceAllocationExpenditure GetFundSourceAllocationExpenditure(this IQueryable<FundSourceAllocationExpenditure> fundSourceAllocationExpenditures, int fundSourceAllocationExpenditureID)
        {
            var fundSourceAllocationExpenditure = fundSourceAllocationExpenditures.SingleOrDefault(x => x.FundSourceAllocationExpenditureID == fundSourceAllocationExpenditureID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationExpenditure, "FundSourceAllocationExpenditure", fundSourceAllocationExpenditureID);
            return fundSourceAllocationExpenditure;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationExpenditure(this IQueryable<FundSourceAllocationExpenditure> fundSourceAllocationExpenditures, List<int> fundSourceAllocationExpenditureIDList)
        {
            if(fundSourceAllocationExpenditureIDList.Any())
            {
                var fundSourceAllocationExpendituresInSourceCollectionToDelete = fundSourceAllocationExpenditures.Where(x => fundSourceAllocationExpenditureIDList.Contains(x.FundSourceAllocationExpenditureID));
                foreach (var fundSourceAllocationExpenditureToDelete in fundSourceAllocationExpendituresInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationExpenditureToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationExpenditure(this IQueryable<FundSourceAllocationExpenditure> fundSourceAllocationExpenditures, ICollection<FundSourceAllocationExpenditure> fundSourceAllocationExpendituresToDelete)
        {
            if(fundSourceAllocationExpendituresToDelete.Any())
            {
                var fundSourceAllocationExpenditureIDList = fundSourceAllocationExpendituresToDelete.Select(x => x.FundSourceAllocationExpenditureID).ToList();
                var fundSourceAllocationExpendituresToDeleteFromSourceList = fundSourceAllocationExpenditures.Where(x => fundSourceAllocationExpenditureIDList.Contains(x.FundSourceAllocationExpenditureID)).ToList();

                foreach (var fundSourceAllocationExpenditureToDelete in fundSourceAllocationExpendituresToDeleteFromSourceList)
                {
                    fundSourceAllocationExpenditureToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationExpenditure(this IQueryable<FundSourceAllocationExpenditure> fundSourceAllocationExpenditures, int fundSourceAllocationExpenditureID)
        {
            DeleteFundSourceAllocationExpenditure(fundSourceAllocationExpenditures, new List<int> { fundSourceAllocationExpenditureID });
        }

        public static void DeleteFundSourceAllocationExpenditure(this IQueryable<FundSourceAllocationExpenditure> fundSourceAllocationExpenditures, FundSourceAllocationExpenditure fundSourceAllocationExpenditureToDelete)
        {
            DeleteFundSourceAllocationExpenditure(fundSourceAllocationExpenditures, new List<FundSourceAllocationExpenditure> { fundSourceAllocationExpenditureToDelete });
        }
    }
}