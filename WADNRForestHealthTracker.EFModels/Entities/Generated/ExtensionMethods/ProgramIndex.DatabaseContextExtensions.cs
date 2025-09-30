//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramIndex]
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
        public static ProgramIndex GetProgramIndex(this IQueryable<ProgramIndex> programIndices, int programIndexID)
        {
            var programIndex = programIndices.SingleOrDefault(x => x.ProgramIndexID == programIndexID);
            Check.RequireNotNullThrowNotFound(programIndex, "ProgramIndex", programIndexID);
            return programIndex;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProgramIndex(this IQueryable<ProgramIndex> programIndices, List<int> programIndexIDList)
        {
            if(programIndexIDList.Any())
            {
                var programIndicesInSourceCollectionToDelete = programIndices.Where(x => programIndexIDList.Contains(x.ProgramIndexID));
                foreach (var programIndexToDelete in programIndicesInSourceCollectionToDelete.ToList())
                {
                    programIndexToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProgramIndex(this IQueryable<ProgramIndex> programIndices, ICollection<ProgramIndex> programIndicesToDelete)
        {
            if(programIndicesToDelete.Any())
            {
                var programIndexIDList = programIndicesToDelete.Select(x => x.ProgramIndexID).ToList();
                var programIndicesToDeleteFromSourceList = programIndices.Where(x => programIndexIDList.Contains(x.ProgramIndexID)).ToList();

                foreach (var programIndexToDelete in programIndicesToDeleteFromSourceList)
                {
                    programIndexToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProgramIndex(this IQueryable<ProgramIndex> programIndices, int programIndexID)
        {
            DeleteProgramIndex(programIndices, new List<int> { programIndexID });
        }

        public static void DeleteProgramIndex(this IQueryable<ProgramIndex> programIndices, ProgramIndex programIndexToDelete)
        {
            DeleteProgramIndex(programIndices, new List<ProgramIndex> { programIndexToDelete });
        }
    }
}