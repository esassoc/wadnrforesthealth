namespace WADNRForestHealthTracker.Models.DataTransferObjects
{
    public class FieldDefinitionDetail
    {
        public int FieldDefinitionID { get; set; }
        public string FieldDefinitionName { get; set; }
        public string FieldDefinitionDisplayName { get; set; }
        public PersonDetail LastUpdatePerson { get; set; } //todo: we just need PersonDetail generated
    }
}
