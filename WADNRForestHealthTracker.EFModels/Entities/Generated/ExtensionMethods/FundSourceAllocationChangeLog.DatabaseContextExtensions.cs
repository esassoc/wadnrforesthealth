//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationChangeLog]
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
        public static FundSourceAllocationChangeLog GetFundSourceAllocationChangeLog(this IQueryable<FundSourceAllocationChangeLog> fundSourceAllocationChangeLogs, int fundSourceAllocationChangeLogID)
        {
            var fundSourceAllocationChangeLog = fundSourceAllocationChangeLogs.SingleOrDefault(x => x.FundSourceAllocationChangeLogID == fundSourceAllocationChangeLogID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationChangeLog, "FundSourceAllocationChangeLog", fundSourceAllocationChangeLogID);
            return fundSourceAllocationChangeLog;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationChangeLog(this IQueryable<FundSourceAllocationChangeLog> fundSourceAllocationChangeLogs, List<int> fundSourceAllocationChangeLogIDList)
        {
            if(fundSourceAllocationChangeLogIDList.Any())
            {
                var fundSourceAllocationChangeLogsInSourceCollectionToDelete = fundSourceAllocationChangeLogs.Where(x => fundSourceAllocationChangeLogIDList.Contains(x.FundSourceAllocationChangeLogID));
                foreach (var fundSourceAllocationChangeLogToDelete in fundSourceAllocationChangeLogsInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationChangeLogToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationChangeLog(this IQueryable<FundSourceAllocationChangeLog> fundSourceAllocationChangeLogs, ICollection<FundSourceAllocationChangeLog> fundSourceAllocationChangeLogsToDelete)
        {
            if(fundSourceAllocationChangeLogsToDelete.Any())
            {
                var fundSourceAllocationChangeLogIDList = fundSourceAllocationChangeLogsToDelete.Select(x => x.FundSourceAllocationChangeLogID).ToList();
                var fundSourceAllocationChangeLogsToDeleteFromSourceList = fundSourceAllocationChangeLogs.Where(x => fundSourceAllocationChangeLogIDList.Contains(x.FundSourceAllocationChangeLogID)).ToList();

                foreach (var fundSourceAllocationChangeLogToDelete in fundSourceAllocationChangeLogsToDeleteFromSourceList)
                {
                    fundSourceAllocationChangeLogToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationChangeLog(this IQueryable<FundSourceAllocationChangeLog> fundSourceAllocationChangeLogs, int fundSourceAllocationChangeLogID)
        {
            DeleteFundSourceAllocationChangeLog(fundSourceAllocationChangeLogs, new List<int> { fundSourceAllocationChangeLogID });
        }

        public static void DeleteFundSourceAllocationChangeLog(this IQueryable<FundSourceAllocationChangeLog> fundSourceAllocationChangeLogs, FundSourceAllocationChangeLog fundSourceAllocationChangeLogToDelete)
        {
            DeleteFundSourceAllocationChangeLog(fundSourceAllocationChangeLogs, new List<FundSourceAllocationChangeLog> { fundSourceAllocationChangeLogToDelete });
        }
    }
}