//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Notification]
namespace WADNR.EFModels.Entities
{
    public partial class Notification
    {
        public int PrimaryKey => NotificationID;
        public NotificationType NotificationType => NotificationType.AllLookupDictionary[NotificationTypeID];

        public static class FieldLengths
        {

        }
    }
}