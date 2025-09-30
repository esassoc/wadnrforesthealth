//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationNoteInternal]
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
        public static FundSourceAllocationNoteInternal GetFundSourceAllocationNoteInternal(this IQueryable<FundSourceAllocationNoteInternal> fundSourceAllocationNoteInternals, int fundSourceAllocationNoteInternalID)
        {
            var fundSourceAllocationNoteInternal = fundSourceAllocationNoteInternals.SingleOrDefault(x => x.FundSourceAllocationNoteInternalID == fundSourceAllocationNoteInternalID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationNoteInternal, "FundSourceAllocationNoteInternal", fundSourceAllocationNoteInternalID);
            return fundSourceAllocationNoteInternal;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationNoteInternal(this IQueryable<FundSourceAllocationNoteInternal> fundSourceAllocationNoteInternals, List<int> fundSourceAllocationNoteInternalIDList)
        {
            if(fundSourceAllocationNoteInternalIDList.Any())
            {
                var fundSourceAllocationNoteInternalsInSourceCollectionToDelete = fundSourceAllocationNoteInternals.Where(x => fundSourceAllocationNoteInternalIDList.Contains(x.FundSourceAllocationNoteInternalID));
                foreach (var fundSourceAllocationNoteInternalToDelete in fundSourceAllocationNoteInternalsInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationNoteInternalToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationNoteInternal(this IQueryable<FundSourceAllocationNoteInternal> fundSourceAllocationNoteInternals, ICollection<FundSourceAllocationNoteInternal> fundSourceAllocationNoteInternalsToDelete)
        {
            if(fundSourceAllocationNoteInternalsToDelete.Any())
            {
                var fundSourceAllocationNoteInternalIDList = fundSourceAllocationNoteInternalsToDelete.Select(x => x.FundSourceAllocationNoteInternalID).ToList();
                var fundSourceAllocationNoteInternalsToDeleteFromSourceList = fundSourceAllocationNoteInternals.Where(x => fundSourceAllocationNoteInternalIDList.Contains(x.FundSourceAllocationNoteInternalID)).ToList();

                foreach (var fundSourceAllocationNoteInternalToDelete in fundSourceAllocationNoteInternalsToDeleteFromSourceList)
                {
                    fundSourceAllocationNoteInternalToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationNoteInternal(this IQueryable<FundSourceAllocationNoteInternal> fundSourceAllocationNoteInternals, int fundSourceAllocationNoteInternalID)
        {
            DeleteFundSourceAllocationNoteInternal(fundSourceAllocationNoteInternals, new List<int> { fundSourceAllocationNoteInternalID });
        }

        public static void DeleteFundSourceAllocationNoteInternal(this IQueryable<FundSourceAllocationNoteInternal> fundSourceAllocationNoteInternals, FundSourceAllocationNoteInternal fundSourceAllocationNoteInternalToDelete)
        {
            DeleteFundSourceAllocationNoteInternal(fundSourceAllocationNoteInternals, new List<FundSourceAllocationNoteInternal> { fundSourceAllocationNoteInternalToDelete });
        }
    }
}