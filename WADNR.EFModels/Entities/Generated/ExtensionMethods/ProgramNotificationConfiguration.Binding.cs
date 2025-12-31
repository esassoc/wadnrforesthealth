//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramNotificationConfiguration]
namespace WADNR.EFModels.Entities
{
    public partial class ProgramNotificationConfiguration
    {
        public int PrimaryKey => ProgramNotificationConfigurationID;
        public ProgramNotificationType ProgramNotificationType => ProgramNotificationType.AllLookupDictionary[ProgramNotificationTypeID];
        public RecurrenceInterval RecurrenceInterval => RecurrenceInterval.AllLookupDictionary[RecurrenceIntervalID];

        public static class FieldLengths
        {

        }
    }
}