//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportRequestLog]
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
        public static SupportRequestLog GetSupportRequestLog(this IQueryable<SupportRequestLog> supportRequestLogs, int supportRequestLogID)
        {
            var supportRequestLog = supportRequestLogs.SingleOrDefault(x => x.SupportRequestLogID == supportRequestLogID);
            Check.RequireNotNullThrowNotFound(supportRequestLog, "SupportRequestLog", supportRequestLogID);
            return supportRequestLog;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteSupportRequestLog(this IQueryable<SupportRequestLog> supportRequestLogs, List<int> supportRequestLogIDList)
        {
            if(supportRequestLogIDList.Any())
            {
                var supportRequestLogsInSourceCollectionToDelete = supportRequestLogs.Where(x => supportRequestLogIDList.Contains(x.SupportRequestLogID));
                foreach (var supportRequestLogToDelete in supportRequestLogsInSourceCollectionToDelete.ToList())
                {
                    supportRequestLogToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteSupportRequestLog(this IQueryable<SupportRequestLog> supportRequestLogs, ICollection<SupportRequestLog> supportRequestLogsToDelete)
        {
            if(supportRequestLogsToDelete.Any())
            {
                var supportRequestLogIDList = supportRequestLogsToDelete.Select(x => x.SupportRequestLogID).ToList();
                var supportRequestLogsToDeleteFromSourceList = supportRequestLogs.Where(x => supportRequestLogIDList.Contains(x.SupportRequestLogID)).ToList();

                foreach (var supportRequestLogToDelete in supportRequestLogsToDeleteFromSourceList)
                {
                    supportRequestLogToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteSupportRequestLog(this IQueryable<SupportRequestLog> supportRequestLogs, int supportRequestLogID)
        {
            DeleteSupportRequestLog(supportRequestLogs, new List<int> { supportRequestLogID });
        }

        public static void DeleteSupportRequestLog(this IQueryable<SupportRequestLog> supportRequestLogs, SupportRequestLog supportRequestLogToDelete)
        {
            DeleteSupportRequestLog(supportRequestLogs, new List<SupportRequestLog> { supportRequestLogToDelete });
        }
    }
}