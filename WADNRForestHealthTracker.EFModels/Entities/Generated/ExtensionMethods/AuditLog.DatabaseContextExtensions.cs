//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AuditLog]
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
        public static AuditLog GetAuditLog(this IQueryable<AuditLog> auditLogs, int auditLogID)
        {
            var auditLog = auditLogs.SingleOrDefault(x => x.AuditLogID == auditLogID);
            Check.RequireNotNullThrowNotFound(auditLog, "AuditLog", auditLogID);
            return auditLog;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteAuditLog(this IQueryable<AuditLog> auditLogs, List<int> auditLogIDList)
        {
            if(auditLogIDList.Any())
            {
                var auditLogsInSourceCollectionToDelete = auditLogs.Where(x => auditLogIDList.Contains(x.AuditLogID));
                foreach (var auditLogToDelete in auditLogsInSourceCollectionToDelete.ToList())
                {
                    auditLogToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteAuditLog(this IQueryable<AuditLog> auditLogs, ICollection<AuditLog> auditLogsToDelete)
        {
            if(auditLogsToDelete.Any())
            {
                var auditLogIDList = auditLogsToDelete.Select(x => x.AuditLogID).ToList();
                var auditLogsToDeleteFromSourceList = auditLogs.Where(x => auditLogIDList.Contains(x.AuditLogID)).ToList();

                foreach (var auditLogToDelete in auditLogsToDeleteFromSourceList)
                {
                    auditLogToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteAuditLog(this IQueryable<AuditLog> auditLogs, int auditLogID)
        {
            DeleteAuditLog(auditLogs, new List<int> { auditLogID });
        }

        public static void DeleteAuditLog(this IQueryable<AuditLog> auditLogs, AuditLog auditLogToDelete)
        {
            DeleteAuditLog(auditLogs, new List<AuditLog> { auditLogToDelete });
        }
    }
}