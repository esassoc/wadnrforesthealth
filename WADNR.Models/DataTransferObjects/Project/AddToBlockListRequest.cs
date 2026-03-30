using System.ComponentModel.DataAnnotations;

namespace WADNR.Models.DataTransferObjects;

public class AddToBlockListRequest : IValidatableObject
{
    [StringLength(140)]
    public string? ProjectGisIdentifier { get; set; }

    [StringLength(140)]
    public string? ProjectName { get; set; }

    public int? ProjectID { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(ProjectGisIdentifier) && string.IsNullOrWhiteSpace(ProjectName))
        {
            yield return new ValidationResult(
                "You must provide Project Name and/or Project GIS Identifier.",
                new[] { nameof(ProjectGisIdentifier), nameof(ProjectName) });
        }
    }
}
