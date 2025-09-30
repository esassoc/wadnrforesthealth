//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationNote]
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
        public static FundSourceAllocationNote GetFundSourceAllocationNote(this IQueryable<FundSourceAllocationNote> fundSourceAllocationNotes, int fundSourceAllocationNoteID)
        {
            var fundSourceAllocationNote = fundSourceAllocationNotes.SingleOrDefault(x => x.FundSourceAllocationNoteID == fundSourceAllocationNoteID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationNote, "FundSourceAllocationNote", fundSourceAllocationNoteID);
            return fundSourceAllocationNote;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationNote(this IQueryable<FundSourceAllocationNote> fundSourceAllocationNotes, List<int> fundSourceAllocationNoteIDList)
        {
            if(fundSourceAllocationNoteIDList.Any())
            {
                var fundSourceAllocationNotesInSourceCollectionToDelete = fundSourceAllocationNotes.Where(x => fundSourceAllocationNoteIDList.Contains(x.FundSourceAllocationNoteID));
                foreach (var fundSourceAllocationNoteToDelete in fundSourceAllocationNotesInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationNoteToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationNote(this IQueryable<FundSourceAllocationNote> fundSourceAllocationNotes, ICollection<FundSourceAllocationNote> fundSourceAllocationNotesToDelete)
        {
            if(fundSourceAllocationNotesToDelete.Any())
            {
                var fundSourceAllocationNoteIDList = fundSourceAllocationNotesToDelete.Select(x => x.FundSourceAllocationNoteID).ToList();
                var fundSourceAllocationNotesToDeleteFromSourceList = fundSourceAllocationNotes.Where(x => fundSourceAllocationNoteIDList.Contains(x.FundSourceAllocationNoteID)).ToList();

                foreach (var fundSourceAllocationNoteToDelete in fundSourceAllocationNotesToDeleteFromSourceList)
                {
                    fundSourceAllocationNoteToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationNote(this IQueryable<FundSourceAllocationNote> fundSourceAllocationNotes, int fundSourceAllocationNoteID)
        {
            DeleteFundSourceAllocationNote(fundSourceAllocationNotes, new List<int> { fundSourceAllocationNoteID });
        }

        public static void DeleteFundSourceAllocationNote(this IQueryable<FundSourceAllocationNote> fundSourceAllocationNotes, FundSourceAllocationNote fundSourceAllocationNoteToDelete)
        {
            DeleteFundSourceAllocationNote(fundSourceAllocationNotes, new List<FundSourceAllocationNote> { fundSourceAllocationNoteToDelete });
        }
    }
}