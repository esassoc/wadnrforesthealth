//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationProgramManager]
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
        public static FundSourceAllocationProgramManager GetFundSourceAllocationProgramManager(this IQueryable<FundSourceAllocationProgramManager> fundSourceAllocationProgramManagers, int fundSourceAllocationProgramManagerID)
        {
            var fundSourceAllocationProgramManager = fundSourceAllocationProgramManagers.SingleOrDefault(x => x.FundSourceAllocationProgramManagerID == fundSourceAllocationProgramManagerID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationProgramManager, "FundSourceAllocationProgramManager", fundSourceAllocationProgramManagerID);
            return fundSourceAllocationProgramManager;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationProgramManager(this IQueryable<FundSourceAllocationProgramManager> fundSourceAllocationProgramManagers, List<int> fundSourceAllocationProgramManagerIDList)
        {
            if(fundSourceAllocationProgramManagerIDList.Any())
            {
                var fundSourceAllocationProgramManagersInSourceCollectionToDelete = fundSourceAllocationProgramManagers.Where(x => fundSourceAllocationProgramManagerIDList.Contains(x.FundSourceAllocationProgramManagerID));
                foreach (var fundSourceAllocationProgramManagerToDelete in fundSourceAllocationProgramManagersInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationProgramManagerToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationProgramManager(this IQueryable<FundSourceAllocationProgramManager> fundSourceAllocationProgramManagers, ICollection<FundSourceAllocationProgramManager> fundSourceAllocationProgramManagersToDelete)
        {
            if(fundSourceAllocationProgramManagersToDelete.Any())
            {
                var fundSourceAllocationProgramManagerIDList = fundSourceAllocationProgramManagersToDelete.Select(x => x.FundSourceAllocationProgramManagerID).ToList();
                var fundSourceAllocationProgramManagersToDeleteFromSourceList = fundSourceAllocationProgramManagers.Where(x => fundSourceAllocationProgramManagerIDList.Contains(x.FundSourceAllocationProgramManagerID)).ToList();

                foreach (var fundSourceAllocationProgramManagerToDelete in fundSourceAllocationProgramManagersToDeleteFromSourceList)
                {
                    fundSourceAllocationProgramManagerToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationProgramManager(this IQueryable<FundSourceAllocationProgramManager> fundSourceAllocationProgramManagers, int fundSourceAllocationProgramManagerID)
        {
            DeleteFundSourceAllocationProgramManager(fundSourceAllocationProgramManagers, new List<int> { fundSourceAllocationProgramManagerID });
        }

        public static void DeleteFundSourceAllocationProgramManager(this IQueryable<FundSourceAllocationProgramManager> fundSourceAllocationProgramManagers, FundSourceAllocationProgramManager fundSourceAllocationProgramManagerToDelete)
        {
            DeleteFundSourceAllocationProgramManager(fundSourceAllocationProgramManagers, new List<FundSourceAllocationProgramManager> { fundSourceAllocationProgramManagerToDelete });
        }
    }
}