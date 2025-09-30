//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ArcOnlineFinanceApiRawJsonImport]
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
        public static ArcOnlineFinanceApiRawJsonImport GetArcOnlineFinanceApiRawJsonImport(this IQueryable<ArcOnlineFinanceApiRawJsonImport> arcOnlineFinanceApiRawJsonImports, int arcOnlineFinanceApiRawJsonImportID)
        {
            var arcOnlineFinanceApiRawJsonImport = arcOnlineFinanceApiRawJsonImports.SingleOrDefault(x => x.ArcOnlineFinanceApiRawJsonImportID == arcOnlineFinanceApiRawJsonImportID);
            Check.RequireNotNullThrowNotFound(arcOnlineFinanceApiRawJsonImport, "ArcOnlineFinanceApiRawJsonImport", arcOnlineFinanceApiRawJsonImportID);
            return arcOnlineFinanceApiRawJsonImport;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteArcOnlineFinanceApiRawJsonImport(this IQueryable<ArcOnlineFinanceApiRawJsonImport> arcOnlineFinanceApiRawJsonImports, List<int> arcOnlineFinanceApiRawJsonImportIDList)
        {
            if(arcOnlineFinanceApiRawJsonImportIDList.Any())
            {
                var arcOnlineFinanceApiRawJsonImportsInSourceCollectionToDelete = arcOnlineFinanceApiRawJsonImports.Where(x => arcOnlineFinanceApiRawJsonImportIDList.Contains(x.ArcOnlineFinanceApiRawJsonImportID));
                foreach (var arcOnlineFinanceApiRawJsonImportToDelete in arcOnlineFinanceApiRawJsonImportsInSourceCollectionToDelete.ToList())
                {
                    arcOnlineFinanceApiRawJsonImportToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteArcOnlineFinanceApiRawJsonImport(this IQueryable<ArcOnlineFinanceApiRawJsonImport> arcOnlineFinanceApiRawJsonImports, ICollection<ArcOnlineFinanceApiRawJsonImport> arcOnlineFinanceApiRawJsonImportsToDelete)
        {
            if(arcOnlineFinanceApiRawJsonImportsToDelete.Any())
            {
                var arcOnlineFinanceApiRawJsonImportIDList = arcOnlineFinanceApiRawJsonImportsToDelete.Select(x => x.ArcOnlineFinanceApiRawJsonImportID).ToList();
                var arcOnlineFinanceApiRawJsonImportsToDeleteFromSourceList = arcOnlineFinanceApiRawJsonImports.Where(x => arcOnlineFinanceApiRawJsonImportIDList.Contains(x.ArcOnlineFinanceApiRawJsonImportID)).ToList();

                foreach (var arcOnlineFinanceApiRawJsonImportToDelete in arcOnlineFinanceApiRawJsonImportsToDeleteFromSourceList)
                {
                    arcOnlineFinanceApiRawJsonImportToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteArcOnlineFinanceApiRawJsonImport(this IQueryable<ArcOnlineFinanceApiRawJsonImport> arcOnlineFinanceApiRawJsonImports, int arcOnlineFinanceApiRawJsonImportID)
        {
            DeleteArcOnlineFinanceApiRawJsonImport(arcOnlineFinanceApiRawJsonImports, new List<int> { arcOnlineFinanceApiRawJsonImportID });
        }

        public static void DeleteArcOnlineFinanceApiRawJsonImport(this IQueryable<ArcOnlineFinanceApiRawJsonImport> arcOnlineFinanceApiRawJsonImports, ArcOnlineFinanceApiRawJsonImport arcOnlineFinanceApiRawJsonImportToDelete)
        {
            DeleteArcOnlineFinanceApiRawJsonImport(arcOnlineFinanceApiRawJsonImports, new List<ArcOnlineFinanceApiRawJsonImport> { arcOnlineFinanceApiRawJsonImportToDelete });
        }
    }
}