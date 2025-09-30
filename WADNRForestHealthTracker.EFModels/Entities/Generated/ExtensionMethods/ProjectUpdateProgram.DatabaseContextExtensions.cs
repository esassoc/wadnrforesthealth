//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateProgram]
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
        public static ProjectUpdateProgram GetProjectUpdateProgram(this IQueryable<ProjectUpdateProgram> projectUpdatePrograms, int projectUpdateProgramID)
        {
            var projectUpdateProgram = projectUpdatePrograms.SingleOrDefault(x => x.ProjectUpdateProgramID == projectUpdateProgramID);
            Check.RequireNotNullThrowNotFound(projectUpdateProgram, "ProjectUpdateProgram", projectUpdateProgramID);
            return projectUpdateProgram;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectUpdateProgram(this IQueryable<ProjectUpdateProgram> projectUpdatePrograms, List<int> projectUpdateProgramIDList)
        {
            if(projectUpdateProgramIDList.Any())
            {
                var projectUpdateProgramsInSourceCollectionToDelete = projectUpdatePrograms.Where(x => projectUpdateProgramIDList.Contains(x.ProjectUpdateProgramID));
                foreach (var projectUpdateProgramToDelete in projectUpdateProgramsInSourceCollectionToDelete.ToList())
                {
                    projectUpdateProgramToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectUpdateProgram(this IQueryable<ProjectUpdateProgram> projectUpdatePrograms, ICollection<ProjectUpdateProgram> projectUpdateProgramsToDelete)
        {
            if(projectUpdateProgramsToDelete.Any())
            {
                var projectUpdateProgramIDList = projectUpdateProgramsToDelete.Select(x => x.ProjectUpdateProgramID).ToList();
                var projectUpdateProgramsToDeleteFromSourceList = projectUpdatePrograms.Where(x => projectUpdateProgramIDList.Contains(x.ProjectUpdateProgramID)).ToList();

                foreach (var projectUpdateProgramToDelete in projectUpdateProgramsToDeleteFromSourceList)
                {
                    projectUpdateProgramToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectUpdateProgram(this IQueryable<ProjectUpdateProgram> projectUpdatePrograms, int projectUpdateProgramID)
        {
            DeleteProjectUpdateProgram(projectUpdatePrograms, new List<int> { projectUpdateProgramID });
        }

        public static void DeleteProjectUpdateProgram(this IQueryable<ProjectUpdateProgram> projectUpdatePrograms, ProjectUpdateProgram projectUpdateProgramToDelete)
        {
            DeleteProjectUpdateProgram(projectUpdatePrograms, new List<ProjectUpdateProgram> { projectUpdateProgramToDelete });
        }
    }
}