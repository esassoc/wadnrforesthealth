//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Tag]
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
        public static Tag GetTag(this IQueryable<Tag> tags, int tagID)
        {
            var tag = tags.SingleOrDefault(x => x.TagID == tagID);
            Check.RequireNotNullThrowNotFound(tag, "Tag", tagID);
            return tag;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTag(this IQueryable<Tag> tags, List<int> tagIDList)
        {
            if(tagIDList.Any())
            {
                var tagsInSourceCollectionToDelete = tags.Where(x => tagIDList.Contains(x.TagID));
                foreach (var tagToDelete in tagsInSourceCollectionToDelete.ToList())
                {
                    tagToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTag(this IQueryable<Tag> tags, ICollection<Tag> tagsToDelete)
        {
            if(tagsToDelete.Any())
            {
                var tagIDList = tagsToDelete.Select(x => x.TagID).ToList();
                var tagsToDeleteFromSourceList = tags.Where(x => tagIDList.Contains(x.TagID)).ToList();

                foreach (var tagToDelete in tagsToDeleteFromSourceList)
                {
                    tagToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTag(this IQueryable<Tag> tags, int tagID)
        {
            DeleteTag(tags, new List<int> { tagID });
        }

        public static void DeleteTag(this IQueryable<Tag> tags, Tag tagToDelete)
        {
            DeleteTag(tags, new List<Tag> { tagToDelete });
        }
    }
}