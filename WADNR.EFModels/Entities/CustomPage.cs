namespace WADNR.EFModels.Entities;

public partial class CustomPage
{
    public bool HasPageContent => !string.IsNullOrWhiteSpace(CustomPageContent);
}
