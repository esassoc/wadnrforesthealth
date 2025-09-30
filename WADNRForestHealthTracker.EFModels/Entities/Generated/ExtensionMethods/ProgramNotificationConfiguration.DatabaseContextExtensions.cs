//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramNotificationConfiguration]
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
        public static ProgramNotificationConfiguration GetProgramNotificationConfiguration(this IQueryable<ProgramNotificationConfiguration> programNotificationConfigurations, int programNotificationConfigurationID)
        {
            var programNotificationConfiguration = programNotificationConfigurations.SingleOrDefault(x => x.ProgramNotificationConfigurationID == programNotificationConfigurationID);
            Check.RequireNotNullThrowNotFound(programNotificationConfiguration, "ProgramNotificationConfiguration", programNotificationConfigurationID);
            return programNotificationConfiguration;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProgramNotificationConfiguration(this IQueryable<ProgramNotificationConfiguration> programNotificationConfigurations, List<int> programNotificationConfigurationIDList)
        {
            if(programNotificationConfigurationIDList.Any())
            {
                var programNotificationConfigurationsInSourceCollectionToDelete = programNotificationConfigurations.Where(x => programNotificationConfigurationIDList.Contains(x.ProgramNotificationConfigurationID));
                foreach (var programNotificationConfigurationToDelete in programNotificationConfigurationsInSourceCollectionToDelete.ToList())
                {
                    programNotificationConfigurationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProgramNotificationConfiguration(this IQueryable<ProgramNotificationConfiguration> programNotificationConfigurations, ICollection<ProgramNotificationConfiguration> programNotificationConfigurationsToDelete)
        {
            if(programNotificationConfigurationsToDelete.Any())
            {
                var programNotificationConfigurationIDList = programNotificationConfigurationsToDelete.Select(x => x.ProgramNotificationConfigurationID).ToList();
                var programNotificationConfigurationsToDeleteFromSourceList = programNotificationConfigurations.Where(x => programNotificationConfigurationIDList.Contains(x.ProgramNotificationConfigurationID)).ToList();

                foreach (var programNotificationConfigurationToDelete in programNotificationConfigurationsToDeleteFromSourceList)
                {
                    programNotificationConfigurationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProgramNotificationConfiguration(this IQueryable<ProgramNotificationConfiguration> programNotificationConfigurations, int programNotificationConfigurationID)
        {
            DeleteProgramNotificationConfiguration(programNotificationConfigurations, new List<int> { programNotificationConfigurationID });
        }

        public static void DeleteProgramNotificationConfiguration(this IQueryable<ProgramNotificationConfiguration> programNotificationConfigurations, ProgramNotificationConfiguration programNotificationConfigurationToDelete)
        {
            DeleteProgramNotificationConfiguration(programNotificationConfigurations, new List<ProgramNotificationConfiguration> { programNotificationConfigurationToDelete });
        }
    }
}