//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[SystemAttribute]
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
        public static SystemAttribute GetSystemAttribute(this IQueryable<SystemAttribute> systemAttributes, int systemAttributeID)
        {
            var systemAttribute = systemAttributes.SingleOrDefault(x => x.SystemAttributeID == systemAttributeID);
            Check.RequireNotNullThrowNotFound(systemAttribute, "SystemAttribute", systemAttributeID);
            return systemAttribute;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteSystemAttribute(this IQueryable<SystemAttribute> systemAttributes, List<int> systemAttributeIDList)
        {
            if(systemAttributeIDList.Any())
            {
                var systemAttributesInSourceCollectionToDelete = systemAttributes.Where(x => systemAttributeIDList.Contains(x.SystemAttributeID));
                foreach (var systemAttributeToDelete in systemAttributesInSourceCollectionToDelete.ToList())
                {
                    systemAttributeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteSystemAttribute(this IQueryable<SystemAttribute> systemAttributes, ICollection<SystemAttribute> systemAttributesToDelete)
        {
            if(systemAttributesToDelete.Any())
            {
                var systemAttributeIDList = systemAttributesToDelete.Select(x => x.SystemAttributeID).ToList();
                var systemAttributesToDeleteFromSourceList = systemAttributes.Where(x => systemAttributeIDList.Contains(x.SystemAttributeID)).ToList();

                foreach (var systemAttributeToDelete in systemAttributesToDeleteFromSourceList)
                {
                    systemAttributeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteSystemAttribute(this IQueryable<SystemAttribute> systemAttributes, int systemAttributeID)
        {
            DeleteSystemAttribute(systemAttributes, new List<int> { systemAttributeID });
        }

        public static void DeleteSystemAttribute(this IQueryable<SystemAttribute> systemAttributes, SystemAttribute systemAttributeToDelete)
        {
            DeleteSystemAttribute(systemAttributes, new List<SystemAttribute> { systemAttributeToDelete });
        }
    }
}