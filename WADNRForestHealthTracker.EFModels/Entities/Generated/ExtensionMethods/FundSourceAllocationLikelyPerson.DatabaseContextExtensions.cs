//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationLikelyPerson]
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
        public static FundSourceAllocationLikelyPerson GetFundSourceAllocationLikelyPerson(this IQueryable<FundSourceAllocationLikelyPerson> fundSourceAllocationLikelyPeople, int fundSourceAllocationLikelyPersonID)
        {
            var fundSourceAllocationLikelyPerson = fundSourceAllocationLikelyPeople.SingleOrDefault(x => x.FundSourceAllocationLikelyPersonID == fundSourceAllocationLikelyPersonID);
            Check.RequireNotNullThrowNotFound(fundSourceAllocationLikelyPerson, "FundSourceAllocationLikelyPerson", fundSourceAllocationLikelyPersonID);
            return fundSourceAllocationLikelyPerson;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFundSourceAllocationLikelyPerson(this IQueryable<FundSourceAllocationLikelyPerson> fundSourceAllocationLikelyPeople, List<int> fundSourceAllocationLikelyPersonIDList)
        {
            if(fundSourceAllocationLikelyPersonIDList.Any())
            {
                var fundSourceAllocationLikelyPeopleInSourceCollectionToDelete = fundSourceAllocationLikelyPeople.Where(x => fundSourceAllocationLikelyPersonIDList.Contains(x.FundSourceAllocationLikelyPersonID));
                foreach (var fundSourceAllocationLikelyPersonToDelete in fundSourceAllocationLikelyPeopleInSourceCollectionToDelete.ToList())
                {
                    fundSourceAllocationLikelyPersonToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFundSourceAllocationLikelyPerson(this IQueryable<FundSourceAllocationLikelyPerson> fundSourceAllocationLikelyPeople, ICollection<FundSourceAllocationLikelyPerson> fundSourceAllocationLikelyPeopleToDelete)
        {
            if(fundSourceAllocationLikelyPeopleToDelete.Any())
            {
                var fundSourceAllocationLikelyPersonIDList = fundSourceAllocationLikelyPeopleToDelete.Select(x => x.FundSourceAllocationLikelyPersonID).ToList();
                var fundSourceAllocationLikelyPeopleToDeleteFromSourceList = fundSourceAllocationLikelyPeople.Where(x => fundSourceAllocationLikelyPersonIDList.Contains(x.FundSourceAllocationLikelyPersonID)).ToList();

                foreach (var fundSourceAllocationLikelyPersonToDelete in fundSourceAllocationLikelyPeopleToDeleteFromSourceList)
                {
                    fundSourceAllocationLikelyPersonToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFundSourceAllocationLikelyPerson(this IQueryable<FundSourceAllocationLikelyPerson> fundSourceAllocationLikelyPeople, int fundSourceAllocationLikelyPersonID)
        {
            DeleteFundSourceAllocationLikelyPerson(fundSourceAllocationLikelyPeople, new List<int> { fundSourceAllocationLikelyPersonID });
        }

        public static void DeleteFundSourceAllocationLikelyPerson(this IQueryable<FundSourceAllocationLikelyPerson> fundSourceAllocationLikelyPeople, FundSourceAllocationLikelyPerson fundSourceAllocationLikelyPersonToDelete)
        {
            DeleteFundSourceAllocationLikelyPerson(fundSourceAllocationLikelyPeople, new List<FundSourceAllocationLikelyPerson> { fundSourceAllocationLikelyPersonToDelete });
        }
    }
}