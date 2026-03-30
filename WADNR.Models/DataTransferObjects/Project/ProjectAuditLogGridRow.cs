namespace WADNR.Models.DataTransferObjects;

public class ProjectAuditLogGridRow
{
    public int AuditLogID { get; set; }
    public int AuditLogEventTypeID { get; set; }
    public DateTimeOffset AuditLogDate { get; set; }
    public int PersonID { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string AuditLogEventTypeName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string? OriginalValue { get; set; }
    public string? NewValue { get; set; }
    public string? AuditDescription { get; set; }
    public string Section { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
