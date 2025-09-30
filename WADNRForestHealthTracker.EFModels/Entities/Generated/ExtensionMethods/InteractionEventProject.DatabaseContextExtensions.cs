//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEventProject]
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
        public static InteractionEventProject GetInteractionEventProject(this IQueryable<InteractionEventProject> interactionEventProjects, int interactionEventProjectID)
        {
            var interactionEventProject = interactionEventProjects.SingleOrDefault(x => x.InteractionEventProjectID == interactionEventProjectID);
            Check.RequireNotNullThrowNotFound(interactionEventProject, "InteractionEventProject", interactionEventProjectID);
            return interactionEventProject;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteInteractionEventProject(this IQueryable<InteractionEventProject> interactionEventProjects, List<int> interactionEventProjectIDList)
        {
            if(interactionEventProjectIDList.Any())
            {
                var interactionEventProjectsInSourceCollectionToDelete = interactionEventProjects.Where(x => interactionEventProjectIDList.Contains(x.InteractionEventProjectID));
                foreach (var interactionEventProjectToDelete in interactionEventProjectsInSourceCollectionToDelete.ToList())
                {
                    interactionEventProjectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteInteractionEventProject(this IQueryable<InteractionEventProject> interactionEventProjects, ICollection<InteractionEventProject> interactionEventProjectsToDelete)
        {
            if(interactionEventProjectsToDelete.Any())
            {
                var interactionEventProjectIDList = interactionEventProjectsToDelete.Select(x => x.InteractionEventProjectID).ToList();
                var interactionEventProjectsToDeleteFromSourceList = interactionEventProjects.Where(x => interactionEventProjectIDList.Contains(x.InteractionEventProjectID)).ToList();

                foreach (var interactionEventProjectToDelete in interactionEventProjectsToDeleteFromSourceList)
                {
                    interactionEventProjectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteInteractionEventProject(this IQueryable<InteractionEventProject> interactionEventProjects, int interactionEventProjectID)
        {
            DeleteInteractionEventProject(interactionEventProjects, new List<int> { interactionEventProjectID });
        }

        public static void DeleteInteractionEventProject(this IQueryable<InteractionEventProject> interactionEventProjects, InteractionEventProject interactionEventProjectToDelete)
        {
            DeleteInteractionEventProject(interactionEventProjects, new List<InteractionEventProject> { interactionEventProjectToDelete });
        }
    }
}