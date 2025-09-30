//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisCrossWalkDefault]
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
        public static GisCrossWalkDefault GetGisCrossWalkDefault(this IQueryable<GisCrossWalkDefault> gisCrossWalkDefaults, int gisCrossWalkDefaultID)
        {
            var gisCrossWalkDefault = gisCrossWalkDefaults.SingleOrDefault(x => x.GisCrossWalkDefaultID == gisCrossWalkDefaultID);
            Check.RequireNotNullThrowNotFound(gisCrossWalkDefault, "GisCrossWalkDefault", gisCrossWalkDefaultID);
            return gisCrossWalkDefault;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisCrossWalkDefault(this IQueryable<GisCrossWalkDefault> gisCrossWalkDefaults, List<int> gisCrossWalkDefaultIDList)
        {
            if(gisCrossWalkDefaultIDList.Any())
            {
                var gisCrossWalkDefaultsInSourceCollectionToDelete = gisCrossWalkDefaults.Where(x => gisCrossWalkDefaultIDList.Contains(x.GisCrossWalkDefaultID));
                foreach (var gisCrossWalkDefaultToDelete in gisCrossWalkDefaultsInSourceCollectionToDelete.ToList())
                {
                    gisCrossWalkDefaultToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisCrossWalkDefault(this IQueryable<GisCrossWalkDefault> gisCrossWalkDefaults, ICollection<GisCrossWalkDefault> gisCrossWalkDefaultsToDelete)
        {
            if(gisCrossWalkDefaultsToDelete.Any())
            {
                var gisCrossWalkDefaultIDList = gisCrossWalkDefaultsToDelete.Select(x => x.GisCrossWalkDefaultID).ToList();
                var gisCrossWalkDefaultsToDeleteFromSourceList = gisCrossWalkDefaults.Where(x => gisCrossWalkDefaultIDList.Contains(x.GisCrossWalkDefaultID)).ToList();

                foreach (var gisCrossWalkDefaultToDelete in gisCrossWalkDefaultsToDeleteFromSourceList)
                {
                    gisCrossWalkDefaultToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisCrossWalkDefault(this IQueryable<GisCrossWalkDefault> gisCrossWalkDefaults, int gisCrossWalkDefaultID)
        {
            DeleteGisCrossWalkDefault(gisCrossWalkDefaults, new List<int> { gisCrossWalkDefaultID });
        }

        public static void DeleteGisCrossWalkDefault(this IQueryable<GisCrossWalkDefault> gisCrossWalkDefaults, GisCrossWalkDefault gisCrossWalkDefaultToDelete)
        {
            DeleteGisCrossWalkDefault(gisCrossWalkDefaults, new List<GisCrossWalkDefault> { gisCrossWalkDefaultToDelete });
        }
    }
}