//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[NotificationProject]
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
        public static NotificationProject GetNotificationProject(this IQueryable<NotificationProject> notificationProjects, int notificationProjectID)
        {
            var notificationProject = notificationProjects.SingleOrDefault(x => x.NotificationProjectID == notificationProjectID);
            Check.RequireNotNullThrowNotFound(notificationProject, "NotificationProject", notificationProjectID);
            return notificationProject;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteNotificationProject(this IQueryable<NotificationProject> notificationProjects, List<int> notificationProjectIDList)
        {
            if(notificationProjectIDList.Any())
            {
                var notificationProjectsInSourceCollectionToDelete = notificationProjects.Where(x => notificationProjectIDList.Contains(x.NotificationProjectID));
                foreach (var notificationProjectToDelete in notificationProjectsInSourceCollectionToDelete.ToList())
                {
                    notificationProjectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteNotificationProject(this IQueryable<NotificationProject> notificationProjects, ICollection<NotificationProject> notificationProjectsToDelete)
        {
            if(notificationProjectsToDelete.Any())
            {
                var notificationProjectIDList = notificationProjectsToDelete.Select(x => x.NotificationProjectID).ToList();
                var notificationProjectsToDeleteFromSourceList = notificationProjects.Where(x => notificationProjectIDList.Contains(x.NotificationProjectID)).ToList();

                foreach (var notificationProjectToDelete in notificationProjectsToDeleteFromSourceList)
                {
                    notificationProjectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteNotificationProject(this IQueryable<NotificationProject> notificationProjects, int notificationProjectID)
        {
            DeleteNotificationProject(notificationProjects, new List<int> { notificationProjectID });
        }

        public static void DeleteNotificationProject(this IQueryable<NotificationProject> notificationProjects, NotificationProject notificationProjectToDelete)
        {
            DeleteNotificationProject(notificationProjects, new List<NotificationProject> { notificationProjectToDelete });
        }
    }
}