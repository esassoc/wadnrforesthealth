namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for the Documents & Notes step of the Project Update workflow.
/// </summary>
public class ProjectUpdateDocumentsNotesStep
{
    public int ProjectUpdateBatchID { get; set; }
    public List<ProjectDocumentUpdateItem> Documents { get; set; } = new();
    public List<ProjectNoteUpdateItem> Notes { get; set; } = new();
}

/// <summary>
/// A document in an Update batch.
/// </summary>
public class ProjectDocumentUpdateItem
{
    public int ProjectDocumentUpdateID { get; set; }
    public int ProjectUpdateBatchID { get; set; }
    public int FileResourceID { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public string? DocumentDescription { get; set; }
    public int? ProjectDocumentTypeID { get; set; }
    public string? FileResourceUrl { get; set; }
}

/// <summary>
/// A note in an Update batch.
/// </summary>
public class ProjectNoteUpdateItem
{
    public int ProjectNoteUpdateID { get; set; }
    public int ProjectUpdateBatchID { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTimeOffset CreateDate { get; set; }
    public string? CreatedByPersonName { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    public string? UpdatedByPersonName { get; set; }
}

/// <summary>
/// Request for saving the Documents & Notes step of the Project Update workflow.
/// </summary>
public class ProjectUpdateDocumentsNotesStepRequest
{
    public List<ProjectDocumentUpdateItemRequest> Documents { get; set; } = new();
    public List<ProjectNoteUpdateItemRequest> Notes { get; set; } = new();
}

/// <summary>
/// Request item for a single document in the Update Documents step.
/// </summary>
public class ProjectDocumentUpdateItemRequest
{
    public int? ProjectDocumentUpdateID { get; set; }
    public int FileResourceID { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public string? DocumentDescription { get; set; }
}

/// <summary>
/// Request item for a single note in the Update Notes step.
/// </summary>
public class ProjectNoteUpdateItemRequest
{
    public int? ProjectNoteUpdateID { get; set; }
    public string Note { get; set; } = string.Empty;
}
