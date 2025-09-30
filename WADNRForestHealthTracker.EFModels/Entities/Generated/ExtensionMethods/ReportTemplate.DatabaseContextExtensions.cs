//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ReportTemplate]
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
        public static ReportTemplate GetReportTemplate(this IQueryable<ReportTemplate> reportTemplates, int reportTemplateID)
        {
            var reportTemplate = reportTemplates.SingleOrDefault(x => x.ReportTemplateID == reportTemplateID);
            Check.RequireNotNullThrowNotFound(reportTemplate, "ReportTemplate", reportTemplateID);
            return reportTemplate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteReportTemplate(this IQueryable<ReportTemplate> reportTemplates, List<int> reportTemplateIDList)
        {
            if(reportTemplateIDList.Any())
            {
                var reportTemplatesInSourceCollectionToDelete = reportTemplates.Where(x => reportTemplateIDList.Contains(x.ReportTemplateID));
                foreach (var reportTemplateToDelete in reportTemplatesInSourceCollectionToDelete.ToList())
                {
                    reportTemplateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteReportTemplate(this IQueryable<ReportTemplate> reportTemplates, ICollection<ReportTemplate> reportTemplatesToDelete)
        {
            if(reportTemplatesToDelete.Any())
            {
                var reportTemplateIDList = reportTemplatesToDelete.Select(x => x.ReportTemplateID).ToList();
                var reportTemplatesToDeleteFromSourceList = reportTemplates.Where(x => reportTemplateIDList.Contains(x.ReportTemplateID)).ToList();

                foreach (var reportTemplateToDelete in reportTemplatesToDeleteFromSourceList)
                {
                    reportTemplateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteReportTemplate(this IQueryable<ReportTemplate> reportTemplates, int reportTemplateID)
        {
            DeleteReportTemplate(reportTemplates, new List<int> { reportTemplateID });
        }

        public static void DeleteReportTemplate(this IQueryable<ReportTemplate> reportTemplates, ReportTemplate reportTemplateToDelete)
        {
            DeleteReportTemplate(reportTemplates, new List<ReportTemplate> { reportTemplateToDelete });
        }
    }
}