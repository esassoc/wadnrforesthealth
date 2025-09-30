//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationExpenditureJsonStage]
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
        public static FundSourceAllocationExpenditureJsonStage GetFundSourceAllocationExpenditureJsonStage(this IQueryable<FundSourceAllocationExpenditureJsonStage> fundSourceAllocationExpenditureJsonStages, int fundSourceAllocationExpenditureJsonStageID)
        {
            var fundSourceAllocationExpenditureJsonStage = fundSourceAllocationExpenditureJsonStages.SingleOrDefault(x => x.FundSourceAllocationExpenditureJsonStageID == fundSourceAllocationExpenditureJsonStageID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationExpenditureJsonStage, "FundSourceAllocationExpenditureJsonStage", fundSourceAllocationExpenditureJsonStageID);
            return fundSourceAllocationExpenditureJsonStage;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationExpenditureJsonStage(this IQueryable<FundSourceAllocationExpenditureJsonStage> fundSourceAllocationExpenditureJsonStages, List<int> fundSourceAllocationExpenditureJsonStageIDList)
        {
            if(fundSourceAllocationExpenditureJsonStageIDList.Any())
            {
                var fundSourceAllocationExpenditureJsonStagesInSourceCollectionToDelete = fundSourceAllocationExpenditureJsonStages.Where(x => fundSourceAllocationExpenditureJsonStageIDList.Contains(x.FundSourceAllocationExpenditureJsonStageID));
                foreach (var fundSourceAllocationExpenditureJsonStageToDelete in fundSourceAllocationExpenditureJsonStagesInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationExpenditureJsonStageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationExpenditureJsonStage(this IQueryable<FundSourceAllocationExpenditureJsonStage> fundSourceAllocationExpenditureJsonStages, ICollection<FundSourceAllocationExpenditureJsonStage> fundSourceAllocationExpenditureJsonStagesToDelete)
        {
            if(fundSourceAllocationExpenditureJsonStagesToDelete.Any())
            {
                var fundSourceAllocationExpenditureJsonStageIDList = fundSourceAllocationExpenditureJsonStagesToDelete.Select(x => x.FundSourceAllocationExpenditureJsonStageID).ToList();
                var fundSourceAllocationExpenditureJsonStagesToDeleteFromSourceList = fundSourceAllocationExpenditureJsonStages.Where(x => fundSourceAllocationExpenditureJsonStageIDList.Contains(x.FundSourceAllocationExpenditureJsonStageID)).ToList();

                foreach (var fundSourceAllocationExpenditureJsonStageToDelete in fundSourceAllocationExpenditureJsonStagesToDeleteFromSourceList)
                {
                    fundSourceAllocationExpenditureJsonStageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationExpenditureJsonStage(this IQueryable<FundSourceAllocationExpenditureJsonStage> fundSourceAllocationExpenditureJsonStages, int fundSourceAllocationExpenditureJsonStageID)
        {
            DeleteFundSourceAllocationExpenditureJsonStage(fundSourceAllocationExpenditureJsonStages, new List<int> { fundSourceAllocationExpenditureJsonStageID });
        }

        public static void DeleteFundSourceAllocationExpenditureJsonStage(this IQueryable<FundSourceAllocationExpenditureJsonStage> fundSourceAllocationExpenditureJsonStages, FundSourceAllocationExpenditureJsonStage fundSourceAllocationExpenditureJsonStageToDelete)
        {
            DeleteFundSourceAllocationExpenditureJsonStage(fundSourceAllocationExpenditureJsonStages, new List<FundSourceAllocationExpenditureJsonStage> { fundSourceAllocationExpenditureJsonStageToDelete });
        }
    }
}