//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementType]
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
        public static AgreementType GetAgreementType(this IQueryable<AgreementType> agreementTypes, int agreementTypeID)
        {
            var agreementType = agreementTypes.SingleOrDefault(x => x.AgreementTypeID == agreementTypeID);
            Check.RequireNotNullThrowNotFound(agreementType, "AgreementType", agreementTypeID);
            return agreementType;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteAgreementType(this IQueryable<AgreementType> agreementTypes, List<int> agreementTypeIDList)
        {
            if(agreementTypeIDList.Any())
            {
                var agreementTypesInSourceCollectionToDelete = agreementTypes.Where(x => agreementTypeIDList.Contains(x.AgreementTypeID));
                foreach (var agreementTypeToDelete in agreementTypesInSourceCollectionToDelete.ToList())
                {
                    agreementTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteAgreementType(this IQueryable<AgreementType> agreementTypes, ICollection<AgreementType> agreementTypesToDelete)
        {
            if(agreementTypesToDelete.Any())
            {
                var agreementTypeIDList = agreementTypesToDelete.Select(x => x.AgreementTypeID).ToList();
                var agreementTypesToDeleteFromSourceList = agreementTypes.Where(x => agreementTypeIDList.Contains(x.AgreementTypeID)).ToList();

                foreach (var agreementTypeToDelete in agreementTypesToDeleteFromSourceList)
                {
                    agreementTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteAgreementType(this IQueryable<AgreementType> agreementTypes, int agreementTypeID)
        {
            DeleteAgreementType(agreementTypes, new List<int> { agreementTypeID });
        }

        public static void DeleteAgreementType(this IQueryable<AgreementType> agreementTypes, AgreementType agreementTypeToDelete)
        {
            DeleteAgreementType(agreementTypes, new List<AgreementType> { agreementTypeToDelete });
        }
    }
}