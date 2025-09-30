//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementStatus]
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
        public static AgreementStatus GetAgreementStatus(this IQueryable<AgreementStatus> agreementStatuses, int agreementStatusID)
        {
            var agreementStatus = agreementStatuses.SingleOrDefault(x => x.AgreementStatusID == agreementStatusID);
            Check.RequireNotNullThrowNotFound(agreementStatus, "AgreementStatus", agreementStatusID);
            return agreementStatus;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteAgreementStatus(this IQueryable<AgreementStatus> agreementStatuses, List<int> agreementStatusIDList)
        {
            if(agreementStatusIDList.Any())
            {
                var agreementStatusesInSourceCollectionToDelete = agreementStatuses.Where(x => agreementStatusIDList.Contains(x.AgreementStatusID));
                foreach (var agreementStatusToDelete in agreementStatusesInSourceCollectionToDelete.ToList())
                {
                    agreementStatusToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteAgreementStatus(this IQueryable<AgreementStatus> agreementStatuses, ICollection<AgreementStatus> agreementStatusesToDelete)
        {
            if(agreementStatusesToDelete.Any())
            {
                var agreementStatusIDList = agreementStatusesToDelete.Select(x => x.AgreementStatusID).ToList();
                var agreementStatusesToDeleteFromSourceList = agreementStatuses.Where(x => agreementStatusIDList.Contains(x.AgreementStatusID)).ToList();

                foreach (var agreementStatusToDelete in agreementStatusesToDeleteFromSourceList)
                {
                    agreementStatusToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteAgreementStatus(this IQueryable<AgreementStatus> agreementStatuses, int agreementStatusID)
        {
            DeleteAgreementStatus(agreementStatuses, new List<int> { agreementStatusID });
        }

        public static void DeleteAgreementStatus(this IQueryable<AgreementStatus> agreementStatuses, AgreementStatus agreementStatusToDelete)
        {
            DeleteAgreementStatus(agreementStatuses, new List<AgreementStatus> { agreementStatusToDelete });
        }
    }
}