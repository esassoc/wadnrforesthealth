//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Agreement]
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
        public static Agreement GetAgreement(this IQueryable<Agreement> agreements, int agreementID)
        {
            var agreement = agreements.SingleOrDefault(x => x.AgreementID == agreementID);
            Check.RequireNotNullThrowNotFound(agreement, "Agreement", agreementID);
            return agreement;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteAgreement(this IQueryable<Agreement> agreements, List<int> agreementIDList)
        {
            if(agreementIDList.Any())
            {
                var agreementsInSourceCollectionToDelete = agreements.Where(x => agreementIDList.Contains(x.AgreementID));
                foreach (var agreementToDelete in agreementsInSourceCollectionToDelete.ToList())
                {
                    agreementToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteAgreement(this IQueryable<Agreement> agreements, ICollection<Agreement> agreementsToDelete)
        {
            if(agreementsToDelete.Any())
            {
                var agreementIDList = agreementsToDelete.Select(x => x.AgreementID).ToList();
                var agreementsToDeleteFromSourceList = agreements.Where(x => agreementIDList.Contains(x.AgreementID)).ToList();

                foreach (var agreementToDelete in agreementsToDeleteFromSourceList)
                {
                    agreementToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteAgreement(this IQueryable<Agreement> agreements, int agreementID)
        {
            DeleteAgreement(agreements, new List<int> { agreementID });
        }

        public static void DeleteAgreement(this IQueryable<Agreement> agreements, Agreement agreementToDelete)
        {
            DeleteAgreement(agreements, new List<Agreement> { agreementToDelete });
        }
    }
}