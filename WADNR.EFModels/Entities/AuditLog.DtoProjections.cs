using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AuditLogProjections
{
    public static readonly Expression<Func<AuditLog, ProjectAuditLogGridRow>> AsProjectGridRow = x => new ProjectAuditLogGridRow
    {
        AuditLogID = x.AuditLogID,
        AuditLogEventTypeID = x.AuditLogEventTypeID,
        AuditLogDate = x.AuditLogDate,
        PersonName = x.Person.FirstName + " " + x.Person.LastName,
        AuditLogEventTypeName = null, // Resolved client-side from AuditLogEventType.AllLookupDictionary
        TableName = x.TableName,
        ColumnName = x.ColumnName,
        OriginalValue = x.OriginalValue,
        NewValue = x.NewValue,
        AuditDescription = x.AuditDescription
    };
}
