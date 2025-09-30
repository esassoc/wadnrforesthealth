//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectProgram]
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
        public static ProjectProgram GetProjectProgram(this IQueryable<ProjectProgram> projectPrograms, int projectProgramID)
        {
            var projectProgram = projectPrograms.SingleOrDefault(x => x.ProjectProgramID == projectProgramID);
            Check.RequireNotNullThrowNotFound(projectProgram, "ProjectProgram", projectProgramID);
            return projectProgram;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectProgram(this IQueryable<ProjectProgram> projectPrograms, List<int> projectProgramIDList)
        {
            if(projectProgramIDList.Any())
            {
                var projectProgramsInSourceCollectionToDelete = projectPrograms.Where(x => projectProgramIDList.Contains(x.ProjectProgramID));
                foreach (var projectProgramToDelete in projectProgramsInSourceCollectionToDelete.ToList())
                {
                    projectProgramToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectProgram(this IQueryable<ProjectProgram> projectPrograms, ICollection<ProjectProgram> projectProgramsToDelete)
        {
            if(projectProgramsToDelete.Any())
            {
                var projectProgramIDList = projectProgramsToDelete.Select(x => x.ProjectProgramID).ToList();
                var projectProgramsToDeleteFromSourceList = projectPrograms.Where(x => projectProgramIDList.Contains(x.ProjectProgramID)).ToList();

                foreach (var projectProgramToDelete in projectProgramsToDeleteFromSourceList)
                {
                    projectProgramToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectProgram(this IQueryable<ProjectProgram> projectPrograms, int projectProgramID)
        {
            DeleteProjectProgram(projectPrograms, new List<int> { projectProgramID });
        }

        public static void DeleteProjectProgram(this IQueryable<ProjectProgram> projectPrograms, ProjectProgram projectProgramToDelete)
        {
            DeleteProjectProgram(projectPrograms, new List<ProjectProgram> { projectProgramToDelete });
        }
    }
}