using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class ProjectDocumentUpsertRequest
{
    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int? ProjectDocumentTypeID { get; set; }
}
