//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementPerson]
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
        public static AgreementPerson GetAgreementPerson(this IQueryable<AgreementPerson> agreementPeople, int agreementPersonID)
        {
            var agreementPerson = agreementPeople.SingleOrDefault(x => x.AgreementPersonID == agreementPersonID);
            Check.RequireNotNullThrowNotFound(agreementPerson, "AgreementPerson", agreementPersonID);
            return agreementPerson;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteAgreementPerson(this IQueryable<AgreementPerson> agreementPeople, List<int> agreementPersonIDList)
        {
            if(agreementPersonIDList.Any())
            {
                var agreementPeopleInSourceCollectionToDelete = agreementPeople.Where(x => agreementPersonIDList.Contains(x.AgreementPersonID));
                foreach (var agreementPersonToDelete in agreementPeopleInSourceCollectionToDelete.ToList())
                {
                    agreementPersonToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteAgreementPerson(this IQueryable<AgreementPerson> agreementPeople, ICollection<AgreementPerson> agreementPeopleToDelete)
        {
            if(agreementPeopleToDelete.Any())
            {
                var agreementPersonIDList = agreementPeopleToDelete.Select(x => x.AgreementPersonID).ToList();
                var agreementPeopleToDeleteFromSourceList = agreementPeople.Where(x => agreementPersonIDList.Contains(x.AgreementPersonID)).ToList();

                foreach (var agreementPersonToDelete in agreementPeopleToDeleteFromSourceList)
                {
                    agreementPersonToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteAgreementPerson(this IQueryable<AgreementPerson> agreementPeople, int agreementPersonID)
        {
            DeleteAgreementPerson(agreementPeople, new List<int> { agreementPersonID });
        }

        public static void DeleteAgreementPerson(this IQueryable<AgreementPerson> agreementPeople, AgreementPerson agreementPersonToDelete)
        {
            DeleteAgreementPerson(agreementPeople, new List<AgreementPerson> { agreementPersonToDelete });
        }
    }
}