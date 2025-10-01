//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FirmaPage]
namespace WADNRForestHealthTracker.EFModels.Entities
{
    public partial class FirmaPage
    {
        public int PrimaryKey => FirmaPageID;
        public FirmaPageType FirmaPageType => FirmaPageType.AllLookupDictionary[FirmaPageTypeID];

        public static class FieldLengths
        {

        }
    }
}