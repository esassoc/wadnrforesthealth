//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PersonAllowedAuthenticator]
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
        public static PersonAllowedAuthenticator GetPersonAllowedAuthenticator(this IQueryable<PersonAllowedAuthenticator> personAllowedAuthenticators, int personAllowedAuthenticatorID)
        {
            var personAllowedAuthenticator = personAllowedAuthenticators.SingleOrDefault(x => x.PersonAllowedAuthenticatorID == personAllowedAuthenticatorID);
            Check.RequireNotNullThrowNotFound(personAllowedAuthenticator, "PersonAllowedAuthenticator", personAllowedAuthenticatorID);
            return personAllowedAuthenticator;
        }

        // Delete using an IDList (WADNR style)
        public static void DeletePersonAllowedAuthenticator(this IQueryable<PersonAllowedAuthenticator> personAllowedAuthenticators, List<int> personAllowedAuthenticatorIDList)
        {
            if(personAllowedAuthenticatorIDList.Any())
            {
                var personAllowedAuthenticatorsInSourceCollectionToDelete = personAllowedAuthenticators.Where(x => personAllowedAuthenticatorIDList.Contains(x.PersonAllowedAuthenticatorID));
                foreach (var personAllowedAuthenticatorToDelete in personAllowedAuthenticatorsInSourceCollectionToDelete.ToList())
                {
                    personAllowedAuthenticatorToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeletePersonAllowedAuthenticator(this IQueryable<PersonAllowedAuthenticator> personAllowedAuthenticators, ICollection<PersonAllowedAuthenticator> personAllowedAuthenticatorsToDelete)
        {
            if(personAllowedAuthenticatorsToDelete.Any())
            {
                var personAllowedAuthenticatorIDList = personAllowedAuthenticatorsToDelete.Select(x => x.PersonAllowedAuthenticatorID).ToList();
                var personAllowedAuthenticatorsToDeleteFromSourceList = personAllowedAuthenticators.Where(x => personAllowedAuthenticatorIDList.Contains(x.PersonAllowedAuthenticatorID)).ToList();

                foreach (var personAllowedAuthenticatorToDelete in personAllowedAuthenticatorsToDeleteFromSourceList)
                {
                    personAllowedAuthenticatorToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeletePersonAllowedAuthenticator(this IQueryable<PersonAllowedAuthenticator> personAllowedAuthenticators, int personAllowedAuthenticatorID)
        {
            DeletePersonAllowedAuthenticator(personAllowedAuthenticators, new List<int> { personAllowedAuthenticatorID });
        }

        public static void DeletePersonAllowedAuthenticator(this IQueryable<PersonAllowedAuthenticator> personAllowedAuthenticators, PersonAllowedAuthenticator personAllowedAuthenticatorToDelete)
        {
            DeletePersonAllowedAuthenticator(personAllowedAuthenticators, new List<PersonAllowedAuthenticator> { personAllowedAuthenticatorToDelete });
        }
    }
}