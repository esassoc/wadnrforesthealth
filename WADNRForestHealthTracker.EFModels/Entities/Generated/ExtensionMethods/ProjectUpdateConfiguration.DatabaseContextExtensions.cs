//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateConfiguration]
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
        public static ProjectUpdateConfiguration GetProjectUpdateConfiguration(this IQueryable<ProjectUpdateConfiguration> projectUpdateConfigurations, int projectUpdateConfigurationID)
        {
            var projectUpdateConfiguration = projectUpdateConfigurations.SingleOrDefault(x => x.ProjectUpdateConfigurationID == projectUpdateConfigurationID);
            Check.RequireNotNullThrowNotFound(projectUpdateConfiguration, "ProjectUpdateConfiguration", projectUpdateConfigurationID);
            return projectUpdateConfiguration;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectUpdateConfiguration(this IQueryable<ProjectUpdateConfiguration> projectUpdateConfigurations, List<int> projectUpdateConfigurationIDList)
        {
            if(projectUpdateConfigurationIDList.Any())
            {
                var projectUpdateConfigurationsInSourceCollectionToDelete = projectUpdateConfigurations.Where(x => projectUpdateConfigurationIDList.Contains(x.ProjectUpdateConfigurationID));
                foreach (var projectUpdateConfigurationToDelete in projectUpdateConfigurationsInSourceCollectionToDelete.ToList())
                {
                    projectUpdateConfigurationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectUpdateConfiguration(this IQueryable<ProjectUpdateConfiguration> projectUpdateConfigurations, ICollection<ProjectUpdateConfiguration> projectUpdateConfigurationsToDelete)
        {
            if(projectUpdateConfigurationsToDelete.Any())
            {
                var projectUpdateConfigurationIDList = projectUpdateConfigurationsToDelete.Select(x => x.ProjectUpdateConfigurationID).ToList();
                var projectUpdateConfigurationsToDeleteFromSourceList = projectUpdateConfigurations.Where(x => projectUpdateConfigurationIDList.Contains(x.ProjectUpdateConfigurationID)).ToList();

                foreach (var projectUpdateConfigurationToDelete in projectUpdateConfigurationsToDeleteFromSourceList)
                {
                    projectUpdateConfigurationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectUpdateConfiguration(this IQueryable<ProjectUpdateConfiguration> projectUpdateConfigurations, int projectUpdateConfigurationID)
        {
            DeleteProjectUpdateConfiguration(projectUpdateConfigurations, new List<int> { projectUpdateConfigurationID });
        }

        public static void DeleteProjectUpdateConfiguration(this IQueryable<ProjectUpdateConfiguration> projectUpdateConfigurations, ProjectUpdateConfiguration projectUpdateConfigurationToDelete)
        {
            DeleteProjectUpdateConfiguration(projectUpdateConfigurations, new List<ProjectUpdateConfiguration> { projectUpdateConfigurationToDelete });
        }
    }
}