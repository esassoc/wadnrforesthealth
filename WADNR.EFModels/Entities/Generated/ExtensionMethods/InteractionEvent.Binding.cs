//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEvent]
namespace WADNR.EFModels.Entities
{
    public partial class InteractionEvent
    {
        public int PrimaryKey => InteractionEventID;
        public InteractionEventType InteractionEventType => InteractionEventType.AllLookupDictionary[InteractionEventTypeID];

        public static class FieldLengths
        {
            public const int InteractionEventTitle = 255;
            public const int InteractionEventDescription = 3000;
        }
    }
}