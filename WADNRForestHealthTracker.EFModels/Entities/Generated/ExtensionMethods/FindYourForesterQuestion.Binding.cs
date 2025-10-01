//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FindYourForesterQuestion]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class FindYourForesterQuestion
    {
        public int PrimaryKey => FindYourForesterQuestionID;
        public ForesterRole? ForesterRole => ForesterRoleID.HasValue ? ForesterRole.AllLookupDictionary[ForesterRoleID.Value] : null;

        public static class FieldLengths
        {
            public const int QuestionText = 500;
        }
    }
}