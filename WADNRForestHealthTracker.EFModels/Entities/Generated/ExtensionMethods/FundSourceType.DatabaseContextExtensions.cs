//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceType]
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
        public static FundSourceType GetFundSourceType(this IQueryable<FundSourceType> fundSourceTypes, int fundSourceTypeID)
        {
            var fundSourceType = fundSourceTypes.SingleOrDefault(x => x.FundSourceTypeID == fundSourceTypeID);
            Check.RequireNotNullThrowNotFound(fundSourceType, "FundSourceType", fundSourceTypeID);
            return fundSourceType;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceType(this IQueryable<FundSourceType> fundSourceTypes, List<int> fundSourceTypeIDList)
        {
            if(fundSourceTypeIDList.Any())
            {
                var fundSourceTypesInSourceCollectionToDelete = fundSourceTypes.Where(x => fundSourceTypeIDList.Contains(x.FundSourceTypeID));
                foreach (var fundSourceTypeToDelete in fundSourceTypesInSourceCollectionToDelete.ToList())
                {
                    fundSourceTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceType(this IQueryable<FundSourceType> fundSourceTypes, ICollection<FundSourceType> fundSourceTypesToDelete)
        {
            if(fundSourceTypesToDelete.Any())
            {
                var fundSourceTypeIDList = fundSourceTypesToDelete.Select(x => x.FundSourceTypeID).ToList();
                var fundSourceTypesToDeleteFromSourceList = fundSourceTypes.Where(x => fundSourceTypeIDList.Contains(x.FundSourceTypeID)).ToList();

                foreach (var fundSourceTypeToDelete in fundSourceTypesToDeleteFromSourceList)
                {
                    fundSourceTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceType(this IQueryable<FundSourceType> fundSourceTypes, int fundSourceTypeID)
        {
            DeleteFundSourceType(fundSourceTypes, new List<int> { fundSourceTypeID });
        }

        public static void DeleteFundSourceType(this IQueryable<FundSourceType> fundSourceTypes, FundSourceType fundSourceTypeToDelete)
        {
            DeleteFundSourceType(fundSourceTypes, new List<FundSourceType> { fundSourceTypeToDelete });
        }
    }
}