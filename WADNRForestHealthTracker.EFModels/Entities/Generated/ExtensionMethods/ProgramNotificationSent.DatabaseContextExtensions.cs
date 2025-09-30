//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramNotificationSent]
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
        public static ProgramNotificationSent GetProgramNotificationSent(this IQueryable<ProgramNotificationSent> programNotificationSents, int programNotificationSentID)
        {
            var programNotificationSent = programNotificationSents.SingleOrDefault(x => x.ProgramNotificationSentID == programNotificationSentID);
            Check.RequireNotNullThrowNotFound(programNotificationSent, "ProgramNotificationSent", programNotificationSentID);
            return programNotificationSent;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProgramNotificationSent(this IQueryable<ProgramNotificationSent> programNotificationSents, List<int> programNotificationSentIDList)
        {
            if(programNotificationSentIDList.Any())
            {
                var programNotificationSentsInSourceCollectionToDelete = programNotificationSents.Where(x => programNotificationSentIDList.Contains(x.ProgramNotificationSentID));
                foreach (var programNotificationSentToDelete in programNotificationSentsInSourceCollectionToDelete.ToList())
                {
                    programNotificationSentToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProgramNotificationSent(this IQueryable<ProgramNotificationSent> programNotificationSents, ICollection<ProgramNotificationSent> programNotificationSentsToDelete)
        {
            if(programNotificationSentsToDelete.Any())
            {
                var programNotificationSentIDList = programNotificationSentsToDelete.Select(x => x.ProgramNotificationSentID).ToList();
                var programNotificationSentsToDeleteFromSourceList = programNotificationSents.Where(x => programNotificationSentIDList.Contains(x.ProgramNotificationSentID)).ToList();

                foreach (var programNotificationSentToDelete in programNotificationSentsToDeleteFromSourceList)
                {
                    programNotificationSentToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProgramNotificationSent(this IQueryable<ProgramNotificationSent> programNotificationSents, int programNotificationSentID)
        {
            DeleteProgramNotificationSent(programNotificationSents, new List<int> { programNotificationSentID });
        }

        public static void DeleteProgramNotificationSent(this IQueryable<ProgramNotificationSent> programNotificationSents, ProgramNotificationSent programNotificationSentToDelete)
        {
            DeleteProgramNotificationSent(programNotificationSents, new List<ProgramNotificationSent> { programNotificationSentToDelete });
        }
    }
}