//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PersonRole]
namespace WADNR.EFModels.Entities
{
    public partial class PersonRole
    {
        public int PrimaryKey => PersonRoleID;
        public Role Role => Role.AllLookupDictionary[RoleID];

        public static class FieldLengths
        {

        }
    }
}