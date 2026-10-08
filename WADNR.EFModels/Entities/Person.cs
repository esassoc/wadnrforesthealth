namespace WADNR.EFModels.Entities;

public partial class Person
{
    public const int AnonymousPersonID = -999;

    /// <summary>The "System User" person that scheduled jobs (e.g. GIS imports) are attributed to.</summary>
    public const int SystemPersonID = 5424;
}