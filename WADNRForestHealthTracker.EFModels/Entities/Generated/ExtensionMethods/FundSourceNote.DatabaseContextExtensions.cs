//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceNote]
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public static partial class DatabaseContextExtensions
    {
        public static FundSourceNote GetFundSourceNote(this IQueryable<FundSourceNote> fundSourceNotes, int fundSourceNoteID)
        {
            var fundSourceNote = fundSourceNotes.SingleOrDefault(x => x.FundSourceNoteID == fundSourceNoteID);
            Check.RequireNotNullThrowNotFound(fundSourceNote, "FundSourceNote", fundSourceNoteID);
            return fundSourceNote;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceNote(this IQueryable<FundSourceNote> fundSourceNotes, List<int> fundSourceNoteIDList)
        {
            if(fundSourceNoteIDList.Any())
            {
                var fundSourceNotesInSourceCollectionToDelete = fundSourceNotes.Where(x => fundSourceNoteIDList.Contains(x.FundSourceNoteID));
                foreach (var fundSourceNoteToDelete in fundSourceNotesInSourceCollectionToDelete.ToList())
                {
                    fundSourceNoteToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceNote(this IQueryable<FundSourceNote> fundSourceNotes, ICollection<FundSourceNote> fundSourceNotesToDelete)
        {
            if(fundSourceNotesToDelete.Any())
            {
                var fundSourceNoteIDList = fundSourceNotesToDelete.Select(x => x.FundSourceNoteID).ToList();
                var fundSourceNotesToDeleteFromSourceList = fundSourceNotes.Where(x => fundSourceNoteIDList.Contains(x.FundSourceNoteID)).ToList();

                foreach (var fundSourceNoteToDelete in fundSourceNotesToDeleteFromSourceList)
                {
                    fundSourceNoteToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceNote(this IQueryable<FundSourceNote> fundSourceNotes, int fundSourceNoteID)
        {
            DeleteFundSourceNote(fundSourceNotes, new List<int> { fundSourceNoteID });
        }

        public static void DeleteFundSourceNote(this IQueryable<FundSourceNote> fundSourceNotes, FundSourceNote fundSourceNoteToDelete)
        {
            DeleteFundSourceNote(fundSourceNotes, new List<FundSourceNote> { fundSourceNoteToDelete });
        }
    }
}