//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateBatch]
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
        public static ProjectUpdateBatch GetProjectUpdateBatch(this IQueryable<ProjectUpdateBatch> projectUpdateBatches, int projectUpdateBatchID)
        {
            var projectUpdateBatch = projectUpdateBatches.SingleOrDefault(x => x.ProjectUpdateBatchID == projectUpdateBatchID);
            Check.RequireNotNullThrowNotFound(projectUpdateBatch, "ProjectUpdateBatch", projectUpdateBatchID);
            return projectUpdateBatch;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProjectUpdateBatch(this IQueryable<ProjectUpdateBatch> projectUpdateBatches, List<int> projectUpdateBatchIDList)
        {
            if(projectUpdateBatchIDList.Any())
            {
                var projectUpdateBatchesInSourceCollectionToDelete = projectUpdateBatches.Where(x => projectUpdateBatchIDList.Contains(x.ProjectUpdateBatchID));
                foreach (var projectUpdateBatchToDelete in projectUpdateBatchesInSourceCollectionToDelete.ToList())
                {
                    projectUpdateBatchToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProjectUpdateBatch(this IQueryable<ProjectUpdateBatch> projectUpdateBatches, ICollection<ProjectUpdateBatch> projectUpdateBatchesToDelete)
        {
            if(projectUpdateBatchesToDelete.Any())
            {
                var projectUpdateBatchIDList = projectUpdateBatchesToDelete.Select(x => x.ProjectUpdateBatchID).ToList();
                var projectUpdateBatchesToDeleteFromSourceList = projectUpdateBatches.Where(x => projectUpdateBatchIDList.Contains(x.ProjectUpdateBatchID)).ToList();

                foreach (var projectUpdateBatchToDelete in projectUpdateBatchesToDeleteFromSourceList)
                {
                    projectUpdateBatchToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProjectUpdateBatch(this IQueryable<ProjectUpdateBatch> projectUpdateBatches, int projectUpdateBatchID)
        {
            DeleteProjectUpdateBatch(projectUpdateBatches, new List<int> { projectUpdateBatchID });
        }

        public static void DeleteProjectUpdateBatch(this IQueryable<ProjectUpdateBatch> projectUpdateBatches, ProjectUpdateBatch projectUpdateBatchToDelete)
        {
            DeleteProjectUpdateBatch(projectUpdateBatches, new List<ProjectUpdateBatch> { projectUpdateBatchToDelete });
        }
    }
}