//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TabularDataImport]
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
        public static TabularDataImport GetTabularDataImport(this IQueryable<TabularDataImport> tabularDataImports, int tabularDataImportID)
        {
            var tabularDataImport = tabularDataImports.SingleOrDefault(x => x.TabularDataImportID == tabularDataImportID);
            Check.RequireNotNullThrowNotFound(tabularDataImport, "TabularDataImport", tabularDataImportID);
            return tabularDataImport;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTabularDataImport(this IQueryable<TabularDataImport> tabularDataImports, List<int> tabularDataImportIDList)
        {
            if(tabularDataImportIDList.Any())
            {
                var tabularDataImportsInSourceCollectionToDelete = tabularDataImports.Where(x => tabularDataImportIDList.Contains(x.TabularDataImportID));
                foreach (var tabularDataImportToDelete in tabularDataImportsInSourceCollectionToDelete.ToList())
                {
                    tabularDataImportToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTabularDataImport(this IQueryable<TabularDataImport> tabularDataImports, ICollection<TabularDataImport> tabularDataImportsToDelete)
        {
            if(tabularDataImportsToDelete.Any())
            {
                var tabularDataImportIDList = tabularDataImportsToDelete.Select(x => x.TabularDataImportID).ToList();
                var tabularDataImportsToDeleteFromSourceList = tabularDataImports.Where(x => tabularDataImportIDList.Contains(x.TabularDataImportID)).ToList();

                foreach (var tabularDataImportToDelete in tabularDataImportsToDeleteFromSourceList)
                {
                    tabularDataImportToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTabularDataImport(this IQueryable<TabularDataImport> tabularDataImports, int tabularDataImportID)
        {
            DeleteTabularDataImport(tabularDataImports, new List<int> { tabularDataImportID });
        }

        public static void DeleteTabularDataImport(this IQueryable<TabularDataImport> tabularDataImports, TabularDataImport tabularDataImportToDelete)
        {
            DeleteTabularDataImport(tabularDataImports, new List<TabularDataImport> { tabularDataImportToDelete });
        }
    }
}