namespace WADNR.Models.DataTransferObjects.FileResource
{
    public class FileResourceDetail
    {
        public int FileResourceID { get; set; }
        public Guid FileResourceGUID { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileResourceMIMETypeDisplayName { get; set; } = string.Empty;
        public DateTimeOffset CreateDate { get; set; }
    }
}
