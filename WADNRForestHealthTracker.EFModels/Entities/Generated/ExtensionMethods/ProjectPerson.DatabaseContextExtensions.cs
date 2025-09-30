//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPerson]
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
        public static ProjectPerson GetProjectPerson(this IQueryable<ProjectPerson> projectPeople, int projectPersonID)
        {
            var projectPerson = projectPeople.SingleOrDefault(x => x.ProjectPersonID == projectPersonID);
            Check.RequireNotNullThrowNotFound(projectPerson, "ProjectPerson", projectPersonID);
            return projectPerson;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectPerson(this IQueryable<ProjectPerson> projectPeople, List<int> projectPersonIDList)
        {
            if(projectPersonIDList.Any())
            {
                var projectPeopleInSourceCollectionToDelete = projectPeople.Where(x => projectPersonIDList.Contains(x.ProjectPersonID));
                foreach (var projectPersonToDelete in projectPeopleInSourceCollectionToDelete.ToList())
                {
                    projectPersonToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectPerson(this IQueryable<ProjectPerson> projectPeople, ICollection<ProjectPerson> projectPeopleToDelete)
        {
            if(projectPeopleToDelete.Any())
            {
                var projectPersonIDList = projectPeopleToDelete.Select(x => x.ProjectPersonID).ToList();
                var projectPeopleToDeleteFromSourceList = projectPeople.Where(x => projectPersonIDList.Contains(x.ProjectPersonID)).ToList();

                foreach (var projectPersonToDelete in projectPeopleToDeleteFromSourceList)
                {
                    projectPersonToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectPerson(this IQueryable<ProjectPerson> projectPeople, int projectPersonID)
        {
            DeleteProjectPerson(projectPeople, new List<int> { projectPersonID });
        }

        public static void DeleteProjectPerson(this IQueryable<ProjectPerson> projectPeople, ProjectPerson projectPersonToDelete)
        {
            DeleteProjectPerson(projectPeople, new List<ProjectPerson> { projectPersonToDelete });
        }
    }
}