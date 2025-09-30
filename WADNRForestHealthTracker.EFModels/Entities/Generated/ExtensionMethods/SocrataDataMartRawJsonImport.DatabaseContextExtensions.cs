//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SocrataDataMartRawJsonImport]
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
        public static SocrataDataMartRawJsonImport GetSocrataDataMartRawJsonImport(this IQueryable<SocrataDataMartRawJsonImport> socrataDataMartRawJsonImports, int socrataDataMartRawJsonImportID)
        {
            var socrataDataMartRawJsonImport = socrataDataMartRawJsonImports.SingleOrDefault(x => x.SocrataDataMartRawJsonImportID == socrataDataMartRawJsonImportID);
            Check.RequireNotNullThrowNotFound(socrataDataMartRawJsonImport, "SocrataDataMartRawJsonImport", socrataDataMartRawJsonImportID);
            return socrataDataMartRawJsonImport;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteSocrataDataMartRawJsonImport(this IQueryable<SocrataDataMartRawJsonImport> socrataDataMartRawJsonImports, List<int> socrataDataMartRawJsonImportIDList)
        {
            if(socrataDataMartRawJsonImportIDList.Any())
            {
                var socrataDataMartRawJsonImportsInSourceCollectionToDelete = socrataDataMartRawJsonImports.Where(x => socrataDataMartRawJsonImportIDList.Contains(x.SocrataDataMartRawJsonImportID));
                foreach (var socrataDataMartRawJsonImportToDelete in socrataDataMartRawJsonImportsInSourceCollectionToDelete.ToList())
                {
                    socrataDataMartRawJsonImportToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteSocrataDataMartRawJsonImport(this IQueryable<SocrataDataMartRawJsonImport> socrataDataMartRawJsonImports, ICollection<SocrataDataMartRawJsonImport> socrataDataMartRawJsonImportsToDelete)
        {
            if(socrataDataMartRawJsonImportsToDelete.Any())
            {
                var socrataDataMartRawJsonImportIDList = socrataDataMartRawJsonImportsToDelete.Select(x => x.SocrataDataMartRawJsonImportID).ToList();
                var socrataDataMartRawJsonImportsToDeleteFromSourceList = socrataDataMartRawJsonImports.Where(x => socrataDataMartRawJsonImportIDList.Contains(x.SocrataDataMartRawJsonImportID)).ToList();

                foreach (var socrataDataMartRawJsonImportToDelete in socrataDataMartRawJsonImportsToDeleteFromSourceList)
                {
                    socrataDataMartRawJsonImportToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteSocrataDataMartRawJsonImport(this IQueryable<SocrataDataMartRawJsonImport> socrataDataMartRawJsonImports, int socrataDataMartRawJsonImportID)
        {
            DeleteSocrataDataMartRawJsonImport(socrataDataMartRawJsonImports, new List<int> { socrataDataMartRawJsonImportID });
        }

        public static void DeleteSocrataDataMartRawJsonImport(this IQueryable<SocrataDataMartRawJsonImport> socrataDataMartRawJsonImports, SocrataDataMartRawJsonImport socrataDataMartRawJsonImportToDelete)
        {
            DeleteSocrataDataMartRawJsonImport(socrataDataMartRawJsonImports, new List<SocrataDataMartRawJsonImport> { socrataDataMartRawJsonImportToDelete });
        }
    }
}