using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class SupportRequestCreateDto
{
    [Required]
    public int SupportRequestTypeID { get; set; }

    [Required]
    [StringLength(2000)]
    public string RequestDescription { get; set; } = null!;

    [StringLength(500)]
    public string? RequestPersonOrganization { get; set; }

    [StringLength(50)]
    public string? RequestPersonPhone { get; set; }

    [StringLength(500)]
    public string? CurrentPageUrl { get; set; }
}
