//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FederalFundCode]
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
        public static FederalFundCode GetFederalFundCode(this IQueryable<FederalFundCode> federalFundCodes, int federalFundCodeID)
        {
            var federalFundCode = federalFundCodes.SingleOrDefault(x => x.FederalFundCodeID == federalFundCodeID);
            Check.RequireNotNullThrowNotFound(federalFundCode, "FederalFundCode", federalFundCodeID);
            return federalFundCode;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFederalFundCode(this IQueryable<FederalFundCode> federalFundCodes, List<int> federalFundCodeIDList)
        {
            if(federalFundCodeIDList.Any())
            {
                var federalFundCodesInSourceCollectionToDelete = federalFundCodes.Where(x => federalFundCodeIDList.Contains(x.FederalFundCodeID));
                foreach (var federalFundCodeToDelete in federalFundCodesInSourceCollectionToDelete.ToList())
                {
                    federalFundCodeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFederalFundCode(this IQueryable<FederalFundCode> federalFundCodes, ICollection<FederalFundCode> federalFundCodesToDelete)
        {
            if(federalFundCodesToDelete.Any())
            {
                var federalFundCodeIDList = federalFundCodesToDelete.Select(x => x.FederalFundCodeID).ToList();
                var federalFundCodesToDeleteFromSourceList = federalFundCodes.Where(x => federalFundCodeIDList.Contains(x.FederalFundCodeID)).ToList();

                foreach (var federalFundCodeToDelete in federalFundCodesToDeleteFromSourceList)
                {
                    federalFundCodeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFederalFundCode(this IQueryable<FederalFundCode> federalFundCodes, int federalFundCodeID)
        {
            DeleteFederalFundCode(federalFundCodes, new List<int> { federalFundCodeID });
        }

        public static void DeleteFederalFundCode(this IQueryable<FederalFundCode> federalFundCodes, FederalFundCode federalFundCodeToDelete)
        {
            DeleteFederalFundCode(federalFundCodes, new List<FederalFundCode> { federalFundCodeToDelete });
        }
    }
}