//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramNotificationSentProject]
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
        public static ProgramNotificationSentProject GetProgramNotificationSentProject(this IQueryable<ProgramNotificationSentProject> programNotificationSentProjects, int programNotificationSentProjectID)
        {
            var programNotificationSentProject = programNotificationSentProjects.SingleOrDefault(x => x.ProgramNotificationSentProjectID == programNotificationSentProjectID);
            Check.RequireNotNullThrowNotFound(programNotificationSentProject, "ProgramNotificationSentProject", programNotificationSentProjectID);
            return programNotificationSentProject;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProgramNotificationSentProject(this IQueryable<ProgramNotificationSentProject> programNotificationSentProjects, List<int> programNotificationSentProjectIDList)
        {
            if(programNotificationSentProjectIDList.Any())
            {
                var programNotificationSentProjectsInSourceCollectionToDelete = programNotificationSentProjects.Where(x => programNotificationSentProjectIDList.Contains(x.ProgramNotificationSentProjectID));
                foreach (var programNotificationSentProjectToDelete in programNotificationSentProjectsInSourceCollectionToDelete.ToList())
                {
                    programNotificationSentProjectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProgramNotificationSentProject(this IQueryable<ProgramNotificationSentProject> programNotificationSentProjects, ICollection<ProgramNotificationSentProject> programNotificationSentProjectsToDelete)
        {
            if(programNotificationSentProjectsToDelete.Any())
            {
                var programNotificationSentProjectIDList = programNotificationSentProjectsToDelete.Select(x => x.ProgramNotificationSentProjectID).ToList();
                var programNotificationSentProjectsToDeleteFromSourceList = programNotificationSentProjects.Where(x => programNotificationSentProjectIDList.Contains(x.ProgramNotificationSentProjectID)).ToList();

                foreach (var programNotificationSentProjectToDelete in programNotificationSentProjectsToDeleteFromSourceList)
                {
                    programNotificationSentProjectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProgramNotificationSentProject(this IQueryable<ProgramNotificationSentProject> programNotificationSentProjects, int programNotificationSentProjectID)
        {
            DeleteProgramNotificationSentProject(programNotificationSentProjects, new List<int> { programNotificationSentProjectID });
        }

        public static void DeleteProgramNotificationSentProject(this IQueryable<ProgramNotificationSentProject> programNotificationSentProjects, ProgramNotificationSentProject programNotificationSentProjectToDelete)
        {
            DeleteProgramNotificationSentProject(programNotificationSentProjects, new List<ProgramNotificationSentProject> { programNotificationSentProjectToDelete });
        }
    }
}