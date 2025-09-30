//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PersonRole]
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
        public static PersonRole GetPersonRole(this IQueryable<PersonRole> personRoles, int personRoleID)
        {
            var personRole = personRoles.SingleOrDefault(x => x.PersonRoleID == personRoleID);
            Check.RequireNotNullThrowNotFound(personRole, "PersonRole", personRoleID);
            return personRole;
        }

        // Delete using an IDList (WADNR style)
        public static void DeletePersonRole(this IQueryable<PersonRole> personRoles, List<int> personRoleIDList)
        {
            if(personRoleIDList.Any())
            {
                var personRolesInSourceCollectionToDelete = personRoles.Where(x => personRoleIDList.Contains(x.PersonRoleID));
                foreach (var personRoleToDelete in personRolesInSourceCollectionToDelete.ToList())
                {
                    personRoleToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeletePersonRole(this IQueryable<PersonRole> personRoles, ICollection<PersonRole> personRolesToDelete)
        {
            if(personRolesToDelete.Any())
            {
                var personRoleIDList = personRolesToDelete.Select(x => x.PersonRoleID).ToList();
                var personRolesToDeleteFromSourceList = personRoles.Where(x => personRoleIDList.Contains(x.PersonRoleID)).ToList();

                foreach (var personRoleToDelete in personRolesToDeleteFromSourceList)
                {
                    personRoleToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeletePersonRole(this IQueryable<PersonRole> personRoles, int personRoleID)
        {
            DeletePersonRole(personRoles, new List<int> { personRoleID });
        }

        public static void DeletePersonRole(this IQueryable<PersonRole> personRoles, PersonRole personRoleToDelete)
        {
            DeletePersonRole(personRoles, new List<PersonRole> { personRoleToDelete });
        }
    }
}