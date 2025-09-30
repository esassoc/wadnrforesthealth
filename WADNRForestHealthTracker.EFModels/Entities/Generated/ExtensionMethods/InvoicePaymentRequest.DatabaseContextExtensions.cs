//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InvoicePaymentRequest]
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
        public static InvoicePaymentRequest GetInvoicePaymentRequest(this IQueryable<InvoicePaymentRequest> invoicePaymentRequests, int invoicePaymentRequestID)
        {
            var invoicePaymentRequest = invoicePaymentRequests.SingleOrDefault(x => x.InvoicePaymentRequestID == invoicePaymentRequestID);
            Check.RequireNotNullThrowNotFound(invoicePaymentRequest, "InvoicePaymentRequest", invoicePaymentRequestID);
            return invoicePaymentRequest;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteInvoicePaymentRequest(this IQueryable<InvoicePaymentRequest> invoicePaymentRequests, List<int> invoicePaymentRequestIDList)
        {
            if(invoicePaymentRequestIDList.Any())
            {
                var invoicePaymentRequestsInSourceCollectionToDelete = invoicePaymentRequests.Where(x => invoicePaymentRequestIDList.Contains(x.InvoicePaymentRequestID));
                foreach (var invoicePaymentRequestToDelete in invoicePaymentRequestsInSourceCollectionToDelete.ToList())
                {
                    invoicePaymentRequestToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteInvoicePaymentRequest(this IQueryable<InvoicePaymentRequest> invoicePaymentRequests, ICollection<InvoicePaymentRequest> invoicePaymentRequestsToDelete)
        {
            if(invoicePaymentRequestsToDelete.Any())
            {
                var invoicePaymentRequestIDList = invoicePaymentRequestsToDelete.Select(x => x.InvoicePaymentRequestID).ToList();
                var invoicePaymentRequestsToDeleteFromSourceList = invoicePaymentRequests.Where(x => invoicePaymentRequestIDList.Contains(x.InvoicePaymentRequestID)).ToList();

                foreach (var invoicePaymentRequestToDelete in invoicePaymentRequestsToDeleteFromSourceList)
                {
                    invoicePaymentRequestToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteInvoicePaymentRequest(this IQueryable<InvoicePaymentRequest> invoicePaymentRequests, int invoicePaymentRequestID)
        {
            DeleteInvoicePaymentRequest(invoicePaymentRequests, new List<int> { invoicePaymentRequestID });
        }

        public static void DeleteInvoicePaymentRequest(this IQueryable<InvoicePaymentRequest> invoicePaymentRequests, InvoicePaymentRequest invoicePaymentRequestToDelete)
        {
            DeleteInvoicePaymentRequest(invoicePaymentRequests, new List<InvoicePaymentRequest> { invoicePaymentRequestToDelete });
        }
    }
}