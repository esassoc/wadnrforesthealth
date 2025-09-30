//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PriorityLandscape]
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
        public static PriorityLandscape GetPriorityLandscape(this IQueryable<PriorityLandscape> priorityLandscapes, int priorityLandscapeID)
        {
            var priorityLandscape = priorityLandscapes.SingleOrDefault(x => x.PriorityLandscapeID == priorityLandscapeID);
            Check.RequireNotNullThrowNotFound(priorityLandscape, "PriorityLandscape", priorityLandscapeID);
            return priorityLandscape;
        }

        // Delete using an IDList (WADNR style)
        public static void DeletePriorityLandscape(this IQueryable<PriorityLandscape> priorityLandscapes, List<int> priorityLandscapeIDList)
        {
            if(priorityLandscapeIDList.Any())
            {
                var priorityLandscapesInSourceCollectionToDelete = priorityLandscapes.Where(x => priorityLandscapeIDList.Contains(x.PriorityLandscapeID));
                foreach (var priorityLandscapeToDelete in priorityLandscapesInSourceCollectionToDelete.ToList())
                {
                    priorityLandscapeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeletePriorityLandscape(this IQueryable<PriorityLandscape> priorityLandscapes, ICollection<PriorityLandscape> priorityLandscapesToDelete)
        {
            if(priorityLandscapesToDelete.Any())
            {
                var priorityLandscapeIDList = priorityLandscapesToDelete.Select(x => x.PriorityLandscapeID).ToList();
                var priorityLandscapesToDeleteFromSourceList = priorityLandscapes.Where(x => priorityLandscapeIDList.Contains(x.PriorityLandscapeID)).ToList();

                foreach (var priorityLandscapeToDelete in priorityLandscapesToDeleteFromSourceList)
                {
                    priorityLandscapeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeletePriorityLandscape(this IQueryable<PriorityLandscape> priorityLandscapes, int priorityLandscapeID)
        {
            DeletePriorityLandscape(priorityLandscapes, new List<int> { priorityLandscapeID });
        }

        public static void DeletePriorityLandscape(this IQueryable<PriorityLandscape> priorityLandscapes, PriorityLandscape priorityLandscapeToDelete)
        {
            DeletePriorityLandscape(priorityLandscapes, new List<PriorityLandscape> { priorityLandscapeToDelete });
        }
    }
}