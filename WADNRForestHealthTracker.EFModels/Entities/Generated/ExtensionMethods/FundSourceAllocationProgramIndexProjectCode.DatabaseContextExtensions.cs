//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationProgramIndexProjectCode]
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
        public static FundSourceAllocationProgramIndexProjectCode GetFundSourceAllocationProgramIndexProjectCode(this IQueryable<FundSourceAllocationProgramIndexProjectCode> fundSourceAllocationProgramIndexProjectCodes, int fundSourceAllocationProgramIndexProjectCodeID)
        {
            var fundSourceAllocationProgramIndexProjectCode = fundSourceAllocationProgramIndexProjectCodes.SingleOrDefault(x => x.FundSourceAllocationProgramIndexProjectCodeID == fundSourceAllocationProgramIndexProjectCodeID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationProgramIndexProjectCode, "FundSourceAllocationProgramIndexProjectCode", fundSourceAllocationProgramIndexProjectCodeID);
            return fundSourceAllocationProgramIndexProjectCode;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationProgramIndexProjectCode(this IQueryable<FundSourceAllocationProgramIndexProjectCode> fundSourceAllocationProgramIndexProjectCodes, List<int> fundSourceAllocationProgramIndexProjectCodeIDList)
        {
            if(fundSourceAllocationProgramIndexProjectCodeIDList.Any())
            {
                var fundSourceAllocationProgramIndexProjectCodesInSourceCollectionToDelete = fundSourceAllocationProgramIndexProjectCodes.Where(x => fundSourceAllocationProgramIndexProjectCodeIDList.Contains(x.FundSourceAllocationProgramIndexProjectCodeID));
                foreach (var fundSourceAllocationProgramIndexProjectCodeToDelete in fundSourceAllocationProgramIndexProjectCodesInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationProgramIndexProjectCodeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationProgramIndexProjectCode(this IQueryable<FundSourceAllocationProgramIndexProjectCode> fundSourceAllocationProgramIndexProjectCodes, ICollection<FundSourceAllocationProgramIndexProjectCode> fundSourceAllocationProgramIndexProjectCodesToDelete)
        {
            if(fundSourceAllocationProgramIndexProjectCodesToDelete.Any())
            {
                var fundSourceAllocationProgramIndexProjectCodeIDList = fundSourceAllocationProgramIndexProjectCodesToDelete.Select(x => x.FundSourceAllocationProgramIndexProjectCodeID).ToList();
                var fundSourceAllocationProgramIndexProjectCodesToDeleteFromSourceList = fundSourceAllocationProgramIndexProjectCodes.Where(x => fundSourceAllocationProgramIndexProjectCodeIDList.Contains(x.FundSourceAllocationProgramIndexProjectCodeID)).ToList();

                foreach (var fundSourceAllocationProgramIndexProjectCodeToDelete in fundSourceAllocationProgramIndexProjectCodesToDeleteFromSourceList)
                {
                    fundSourceAllocationProgramIndexProjectCodeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationProgramIndexProjectCode(this IQueryable<FundSourceAllocationProgramIndexProjectCode> fundSourceAllocationProgramIndexProjectCodes, int fundSourceAllocationProgramIndexProjectCodeID)
        {
            DeleteFundSourceAllocationProgramIndexProjectCode(fundSourceAllocationProgramIndexProjectCodes, new List<int> { fundSourceAllocationProgramIndexProjectCodeID });
        }

        public static void DeleteFundSourceAllocationProgramIndexProjectCode(this IQueryable<FundSourceAllocationProgramIndexProjectCode> fundSourceAllocationProgramIndexProjectCodes, FundSourceAllocationProgramIndexProjectCode fundSourceAllocationProgramIndexProjectCodeToDelete)
        {
            DeleteFundSourceAllocationProgramIndexProjectCode(fundSourceAllocationProgramIndexProjectCodes, new List<FundSourceAllocationProgramIndexProjectCode> { fundSourceAllocationProgramIndexProjectCodeToDelete });
        }
    }
}