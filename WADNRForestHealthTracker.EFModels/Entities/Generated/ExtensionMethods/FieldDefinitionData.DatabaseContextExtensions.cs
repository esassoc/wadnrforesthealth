//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FieldDefinitionData]
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
        public static FieldDefinitionData GetFieldDefinitionData(this IQueryable<FieldDefinitionData> fieldDefinitionDatas, int fieldDefinitionDataID)
        {
            var fieldDefinitionData = fieldDefinitionDatas.SingleOrDefault(x => x.FieldDefinitionDataID == fieldDefinitionDataID);
            Check.RequireNotNullThrowNotFound(fieldDefinitionData, "FieldDefinitionData", fieldDefinitionDataID);
            return fieldDefinitionData;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFieldDefinitionData(this IQueryable<FieldDefinitionData> fieldDefinitionDatas, List<int> fieldDefinitionDataIDList)
        {
            if(fieldDefinitionDataIDList.Any())
            {
                var fieldDefinitionDatasInSourceCollectionToDelete = fieldDefinitionDatas.Where(x => fieldDefinitionDataIDList.Contains(x.FieldDefinitionDataID));
                foreach (var fieldDefinitionDataToDelete in fieldDefinitionDatasInSourceCollectionToDelete.ToList())
                {
                    fieldDefinitionDataToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFieldDefinitionData(this IQueryable<FieldDefinitionData> fieldDefinitionDatas, ICollection<FieldDefinitionData> fieldDefinitionDatasToDelete)
        {
            if(fieldDefinitionDatasToDelete.Any())
            {
                var fieldDefinitionDataIDList = fieldDefinitionDatasToDelete.Select(x => x.FieldDefinitionDataID).ToList();
                var fieldDefinitionDatasToDeleteFromSourceList = fieldDefinitionDatas.Where(x => fieldDefinitionDataIDList.Contains(x.FieldDefinitionDataID)).ToList();

                foreach (var fieldDefinitionDataToDelete in fieldDefinitionDatasToDeleteFromSourceList)
                {
                    fieldDefinitionDataToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFieldDefinitionData(this IQueryable<FieldDefinitionData> fieldDefinitionDatas, int fieldDefinitionDataID)
        {
            DeleteFieldDefinitionData(fieldDefinitionDatas, new List<int> { fieldDefinitionDataID });
        }

        public static void DeleteFieldDefinitionData(this IQueryable<FieldDefinitionData> fieldDefinitionDatas, FieldDefinitionData fieldDefinitionDataToDelete)
        {
            DeleteFieldDefinitionData(fieldDefinitionDatas, new List<FieldDefinitionData> { fieldDefinitionDataToDelete });
        }
    }
}