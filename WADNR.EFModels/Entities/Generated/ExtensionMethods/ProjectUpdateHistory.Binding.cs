//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateHistory]
namespace WADNR.EFModels.Entities
{
    public partial class ProjectUpdateHistory
    {
        public int PrimaryKey => ProjectUpdateHistoryID;
        public ProjectUpdateState ProjectUpdateState => ProjectUpdateState.AllLookupDictionary[ProjectUpdateStateID];

        public static class FieldLengths
        {

        }
    }
}