//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementPerson]
namespace WADNR.EFModels.Entities
{
    public partial class AgreementPerson
    {
        public int PrimaryKey => AgreementPersonID;
        public AgreementPersonRole AgreementPersonRole => AgreementPersonRole.AllLookupDictionary[AgreementPersonRoleID];

        public static class FieldLengths
        {

        }
    }
}