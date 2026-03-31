//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SupportRequestLog]
namespace WADNR.EFModels.Entities
{
    public partial class SupportRequestLog
    {
        public int PrimaryKey => SupportRequestLogID;
        public SupportRequestType SupportRequestType => SupportRequestType.AllLookupDictionary[SupportRequestTypeID];

        public static class FieldLengths
        {
            public const int RequestPersonName = 200;
            public const int RequestPersonEmail = 256;
            public const int RequestDescription = 2000;
            public const int RequestPersonOrganization = 500;
            public const int RequestPersonPhone = 50;
        }
    }
}