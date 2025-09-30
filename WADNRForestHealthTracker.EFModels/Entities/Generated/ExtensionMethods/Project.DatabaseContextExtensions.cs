//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Project]
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
        public static Project GetProject(this IQueryable<Project> projects, int projectID)
        {
            var project = projects.SingleOrDefault(x => x.ProjectID == projectID);
            Check.RequireNotNullThrowNotFound(project, "Project", projectID);
            return project;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProject(this IQueryable<Project> projects, List<int> projectIDList)
        {
            if(projectIDList.Any())
            {
                var projectsInSourceCollectionToDelete = projects.Where(x => projectIDList.Contains(x.ProjectID));
                foreach (var projectToDelete in projectsInSourceCollectionToDelete.ToList())
                {
                    projectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProject(this IQueryable<Project> projects, ICollection<Project> projectsToDelete)
        {
            if(projectsToDelete.Any())
            {
                var projectIDList = projectsToDelete.Select(x => x.ProjectID).ToList();
                var projectsToDeleteFromSourceList = projects.Where(x => projectIDList.Contains(x.ProjectID)).ToList();

                foreach (var projectToDelete in projectsToDeleteFromSourceList)
                {
                    projectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProject(this IQueryable<Project> projects, int projectID)
        {
            DeleteProject(projects, new List<int> { projectID });
        }

        public static void DeleteProject(this IQueryable<Project> projects, Project projectToDelete)
        {
            DeleteProject(projects, new List<Project> { projectToDelete });
        }
    }
}