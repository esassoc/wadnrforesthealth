//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceNoteInternal]
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
        public static FundSourceNoteInternal GetFundSourceNoteInternal(this IQueryable<FundSourceNoteInternal> fundSourceNoteInternals, int fundSourceNoteInternalID)
        {
            var fundSourceNoteInternal = fundSourceNoteInternals.SingleOrDefault(x => x.FundSourceNoteInternalID == fundSourceNoteInternalID);
            Check.RequireNotNullThrowNotFound(fundSourceNoteInternal, "FundSourceNoteInternal", fundSourceNoteInternalID);
            return fundSourceNoteInternal;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceNoteInternal(this IQueryable<FundSourceNoteInternal> fundSourceNoteInternals, List<int> fundSourceNoteInternalIDList)
        {
            if(fundSourceNoteInternalIDList.Any())
            {
                var fundSourceNoteInternalsInSourceCollectionToDelete = fundSourceNoteInternals.Where(x => fundSourceNoteInternalIDList.Contains(x.FundSourceNoteInternalID));
                foreach (var fundSourceNoteInternalToDelete in fundSourceNoteInternalsInSourceCollectionToDelete.ToList())
                {
                    fundSourceNoteInternalToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceNoteInternal(this IQueryable<FundSourceNoteInternal> fundSourceNoteInternals, ICollection<FundSourceNoteInternal> fundSourceNoteInternalsToDelete)
        {
            if(fundSourceNoteInternalsToDelete.Any())
            {
                var fundSourceNoteInternalIDList = fundSourceNoteInternalsToDelete.Select(x => x.FundSourceNoteInternalID).ToList();
                var fundSourceNoteInternalsToDeleteFromSourceList = fundSourceNoteInternals.Where(x => fundSourceNoteInternalIDList.Contains(x.FundSourceNoteInternalID)).ToList();

                foreach (var fundSourceNoteInternalToDelete in fundSourceNoteInternalsToDeleteFromSourceList)
                {
                    fundSourceNoteInternalToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceNoteInternal(this IQueryable<FundSourceNoteInternal> fundSourceNoteInternals, int fundSourceNoteInternalID)
        {
            DeleteFundSourceNoteInternal(fundSourceNoteInternals, new List<int> { fundSourceNoteInternalID });
        }

        public static void DeleteFundSourceNoteInternal(this IQueryable<FundSourceNoteInternal> fundSourceNoteInternals, FundSourceNoteInternal fundSourceNoteInternalToDelete)
        {
            DeleteFundSourceNoteInternal(fundSourceNoteInternals, new List<FundSourceNoteInternal> { fundSourceNoteInternalToDelete });
        }
    }
}