namespace WADNRForestHealthTracker.Models.DataTransferObjects
{
    public class OrganizationUpdateRequest
    {
        public Guid? OrganizationGuid { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string? OrganizationAbbreviation { get; set; }
        public int SectorID { get; set; }
        public int? PrimaryContactPersonID { get; set; }
        public bool IsActive { get; set; }
        public string? OrganizationUrl { get; set; }
        public int? LogoFileResourceInfoID { get; set; }
        public bool IsUserAccountOrganization { get; set; }
    }
}
