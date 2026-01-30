using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class ProjectNoteUpsertRequest
{
    public int ProjectID { get; set; }

    [Required]
    [MaxLength(8000)]
    public string Note { get; set; } = string.Empty;
}
