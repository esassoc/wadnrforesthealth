//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SystemAttribute]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class SystemAttribute
    {
        public int PrimaryKey => SystemAttributeID;
        public ProjectStewardshipAreaType? ProjectStewardshipAreaType => ProjectStewardshipAreaTypeID.HasValue ? ProjectStewardshipAreaType.AllLookupDictionary[ProjectStewardshipAreaTypeID.Value] : null;

        public static class FieldLengths
        {
            public const int RecaptchaPublicKey = 100;
            public const int RecaptchaPrivateKey = 100;
            public const int SocrataAppToken = 200;
        }
    }
}