using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class PersonUpsertRequestDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? PersonAddress { get; set; }

    public int? OrganizationID { get; set; }

    public int? VendorID { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
