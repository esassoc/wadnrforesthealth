//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[RelationshipType]
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
        public static RelationshipType GetRelationshipType(this IQueryable<RelationshipType> relationshipTypes, int relationshipTypeID)
        {
            var relationshipType = relationshipTypes.SingleOrDefault(x => x.RelationshipTypeID == relationshipTypeID);
            Check.RequireNotNullThrowNotFound(relationshipType, "RelationshipType", relationshipTypeID);
            return relationshipType;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteRelationshipType(this IQueryable<RelationshipType> relationshipTypes, List<int> relationshipTypeIDList)
        {
            if(relationshipTypeIDList.Any())
            {
                var relationshipTypesInSourceCollectionToDelete = relationshipTypes.Where(x => relationshipTypeIDList.Contains(x.RelationshipTypeID));
                foreach (var relationshipTypeToDelete in relationshipTypesInSourceCollectionToDelete.ToList())
                {
                    relationshipTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteRelationshipType(this IQueryable<RelationshipType> relationshipTypes, ICollection<RelationshipType> relationshipTypesToDelete)
        {
            if(relationshipTypesToDelete.Any())
            {
                var relationshipTypeIDList = relationshipTypesToDelete.Select(x => x.RelationshipTypeID).ToList();
                var relationshipTypesToDeleteFromSourceList = relationshipTypes.Where(x => relationshipTypeIDList.Contains(x.RelationshipTypeID)).ToList();

                foreach (var relationshipTypeToDelete in relationshipTypesToDeleteFromSourceList)
                {
                    relationshipTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteRelationshipType(this IQueryable<RelationshipType> relationshipTypes, int relationshipTypeID)
        {
            DeleteRelationshipType(relationshipTypes, new List<int> { relationshipTypeID });
        }

        public static void DeleteRelationshipType(this IQueryable<RelationshipType> relationshipTypes, RelationshipType relationshipTypeToDelete)
        {
            DeleteRelationshipType(relationshipTypes, new List<RelationshipType> { relationshipTypeToDelete });
        }
    }
}