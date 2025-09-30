//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[LoaStage]
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
        public static LoaStage GetLoaStage(this IQueryable<LoaStage> loaStages, int loaStageID)
        {
            var loaStage = loaStages.SingleOrDefault(x => x.LoaStageID == loaStageID);
            Check.RequireNotNullThrowNotFound(loaStage, "LoaStage", loaStageID);
            return loaStage;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteLoaStage(this IQueryable<LoaStage> loaStages, List<int> loaStageIDList)
        {
            if(loaStageIDList.Any())
            {
                var loaStagesInSourceCollectionToDelete = loaStages.Where(x => loaStageIDList.Contains(x.LoaStageID));
                foreach (var loaStageToDelete in loaStagesInSourceCollectionToDelete.ToList())
                {
                    loaStageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteLoaStage(this IQueryable<LoaStage> loaStages, ICollection<LoaStage> loaStagesToDelete)
        {
            if(loaStagesToDelete.Any())
            {
                var loaStageIDList = loaStagesToDelete.Select(x => x.LoaStageID).ToList();
                var loaStagesToDeleteFromSourceList = loaStages.Where(x => loaStageIDList.Contains(x.LoaStageID)).ToList();

                foreach (var loaStageToDelete in loaStagesToDeleteFromSourceList)
                {
                    loaStageToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteLoaStage(this IQueryable<LoaStage> loaStages, int loaStageID)
        {
            DeleteLoaStage(loaStages, new List<int> { loaStageID });
        }

        public static void DeleteLoaStage(this IQueryable<LoaStage> loaStages, LoaStage loaStageToDelete)
        {
            DeleteLoaStage(loaStages, new List<LoaStage> { loaStageToDelete });
        }
    }
}