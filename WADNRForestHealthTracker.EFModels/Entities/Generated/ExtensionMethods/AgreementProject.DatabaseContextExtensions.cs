//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementProject]
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
        public static AgreementProject GetAgreementProject(this IQueryable<AgreementProject> agreementProjects, int agreementProjectID)
        {
            var agreementProject = agreementProjects.SingleOrDefault(x => x.AgreementProjectID == agreementProjectID);
            Check.RequireNotNullThrowNotFound(agreementProject, "AgreementProject", agreementProjectID);
            return agreementProject;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteAgreementProject(this IQueryable<AgreementProject> agreementProjects, List<int> agreementProjectIDList)
        {
            if(agreementProjectIDList.Any())
            {
                var agreementProjectsInSourceCollectionToDelete = agreementProjects.Where(x => agreementProjectIDList.Contains(x.AgreementProjectID));
                foreach (var agreementProjectToDelete in agreementProjectsInSourceCollectionToDelete.ToList())
                {
                    agreementProjectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteAgreementProject(this IQueryable<AgreementProject> agreementProjects, ICollection<AgreementProject> agreementProjectsToDelete)
        {
            if(agreementProjectsToDelete.Any())
            {
                var agreementProjectIDList = agreementProjectsToDelete.Select(x => x.AgreementProjectID).ToList();
                var agreementProjectsToDeleteFromSourceList = agreementProjects.Where(x => agreementProjectIDList.Contains(x.AgreementProjectID)).ToList();

                foreach (var agreementProjectToDelete in agreementProjectsToDeleteFromSourceList)
                {
                    agreementProjectToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteAgreementProject(this IQueryable<AgreementProject> agreementProjects, int agreementProjectID)
        {
            DeleteAgreementProject(agreementProjects, new List<int> { agreementProjectID });
        }

        public static void DeleteAgreementProject(this IQueryable<AgreementProject> agreementProjects, AgreementProject agreementProjectToDelete)
        {
            DeleteAgreementProject(agreementProjects, new List<AgreementProject> { agreementProjectToDelete });
        }
    }
}