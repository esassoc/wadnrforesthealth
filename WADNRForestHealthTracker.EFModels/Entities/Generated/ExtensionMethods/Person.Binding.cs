//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Person]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class Person
    {
        public int PrimaryKey => PersonID;


        public static class FieldLengths
        {
            public const int FirstName = 100;
            public const int LastName = 100;
            public const int Email = 255;
            public const int Phone = 30;
            public const int MiddleName = 100;
            public const int Notes = 500;
            public const int PersonAddress = 255;
        }
    }
}