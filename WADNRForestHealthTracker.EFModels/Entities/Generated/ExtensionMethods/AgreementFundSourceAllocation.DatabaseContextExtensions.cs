//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementFundSourceAllocation]
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
        public static AgreementFundSourceAllocation GetAgreementFundSourceAllocation(this IQueryable<AgreementFundSourceAllocation> agreementFundSourceAllocations, int agreementFundSourceAllocationID)
        {
            var agreementFundSourceAllocation = agreementFundSourceAllocations.SingleOrDefault(x => x.AgreementFundSourceAllocationID == agreementFundSourceAllocationID);
            Check.RequireNotNullThrowNotFound(agreementFundSourceAllocation, "AgreementFundSourceAllocation", agreementFundSourceAllocationID);
            return agreementFundSourceAllocation;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteAgreementFundSourceAllocation(this IQueryable<AgreementFundSourceAllocation> agreementFundSourceAllocations, List<int> agreementFundSourceAllocationIDList)
        {
            if(agreementFundSourceAllocationIDList.Any())
            {
                var agreementFundSourceAllocationsInSourceCollectionToDelete = agreementFundSourceAllocations.Where(x => agreementFundSourceAllocationIDList.Contains(x.AgreementFundSourceAllocationID));
                foreach (var agreementFundSourceAllocationToDelete in agreementFundSourceAllocationsInSourceCollectionToDelete.ToList())
                {
                    agreementFundSourceAllocationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteAgreementFundSourceAllocation(this IQueryable<AgreementFundSourceAllocation> agreementFundSourceAllocations, ICollection<AgreementFundSourceAllocation> agreementFundSourceAllocationsToDelete)
        {
            if(agreementFundSourceAllocationsToDelete.Any())
            {
                var agreementFundSourceAllocationIDList = agreementFundSourceAllocationsToDelete.Select(x => x.AgreementFundSourceAllocationID).ToList();
                var agreementFundSourceAllocationsToDeleteFromSourceList = agreementFundSourceAllocations.Where(x => agreementFundSourceAllocationIDList.Contains(x.AgreementFundSourceAllocationID)).ToList();

                foreach (var agreementFundSourceAllocationToDelete in agreementFundSourceAllocationsToDeleteFromSourceList)
                {
                    agreementFundSourceAllocationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteAgreementFundSourceAllocation(this IQueryable<AgreementFundSourceAllocation> agreementFundSourceAllocations, int agreementFundSourceAllocationID)
        {
            DeleteAgreementFundSourceAllocation(agreementFundSourceAllocations, new List<int> { agreementFundSourceAllocationID });
        }

        public static void DeleteAgreementFundSourceAllocation(this IQueryable<AgreementFundSourceAllocation> agreementFundSourceAllocations, AgreementFundSourceAllocation agreementFundSourceAllocationToDelete)
        {
            DeleteAgreementFundSourceAllocation(agreementFundSourceAllocations, new List<AgreementFundSourceAllocation> { agreementFundSourceAllocationToDelete });
        }
    }
}