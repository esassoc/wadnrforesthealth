//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PersonStewardTaxonomyBranch]
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
        public static PersonStewardTaxonomyBranch GetPersonStewardTaxonomyBranch(this IQueryable<PersonStewardTaxonomyBranch> personStewardTaxonomyBranches, int personStewardTaxonomyBranchID)
        {
            var personStewardTaxonomyBranch = personStewardTaxonomyBranches.SingleOrDefault(x => x.PersonStewardTaxonomyBranchID == personStewardTaxonomyBranchID);
            Check.RequireNotNullThrowNotFound(personStewardTaxonomyBranch, "PersonStewardTaxonomyBranch", personStewardTaxonomyBranchID);
            return personStewardTaxonomyBranch;
        }

        // Delete using an IDList (WADNR style)
        public static void DeletePersonStewardTaxonomyBranch(this IQueryable<PersonStewardTaxonomyBranch> personStewardTaxonomyBranches, List<int> personStewardTaxonomyBranchIDList)
        {
            if(personStewardTaxonomyBranchIDList.Any())
            {
                var personStewardTaxonomyBranchesInSourceCollectionToDelete = personStewardTaxonomyBranches.Where(x => personStewardTaxonomyBranchIDList.Contains(x.PersonStewardTaxonomyBranchID));
                foreach (var personStewardTaxonomyBranchToDelete in personStewardTaxonomyBranchesInSourceCollectionToDelete.ToList())
                {
                    personStewardTaxonomyBranchToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeletePersonStewardTaxonomyBranch(this IQueryable<PersonStewardTaxonomyBranch> personStewardTaxonomyBranches, ICollection<PersonStewardTaxonomyBranch> personStewardTaxonomyBranchesToDelete)
        {
            if(personStewardTaxonomyBranchesToDelete.Any())
            {
                var personStewardTaxonomyBranchIDList = personStewardTaxonomyBranchesToDelete.Select(x => x.PersonStewardTaxonomyBranchID).ToList();
                var personStewardTaxonomyBranchesToDeleteFromSourceList = personStewardTaxonomyBranches.Where(x => personStewardTaxonomyBranchIDList.Contains(x.PersonStewardTaxonomyBranchID)).ToList();

                foreach (var personStewardTaxonomyBranchToDelete in personStewardTaxonomyBranchesToDeleteFromSourceList)
                {
                    personStewardTaxonomyBranchToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeletePersonStewardTaxonomyBranch(this IQueryable<PersonStewardTaxonomyBranch> personStewardTaxonomyBranches, int personStewardTaxonomyBranchID)
        {
            DeletePersonStewardTaxonomyBranch(personStewardTaxonomyBranches, new List<int> { personStewardTaxonomyBranchID });
        }

        public static void DeletePersonStewardTaxonomyBranch(this IQueryable<PersonStewardTaxonomyBranch> personStewardTaxonomyBranches, PersonStewardTaxonomyBranch personStewardTaxonomyBranchToDelete)
        {
            DeletePersonStewardTaxonomyBranch(personStewardTaxonomyBranches, new List<PersonStewardTaxonomyBranch> { personStewardTaxonomyBranchToDelete });
        }
    }
}