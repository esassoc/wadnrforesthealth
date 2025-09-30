//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Invoice]
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
        public static Invoice GetInvoice(this IQueryable<Invoice> invoices, int invoiceID)
        {
            var invoice = invoices.SingleOrDefault(x => x.InvoiceID == invoiceID);
            Check.RequireNotNullThrowNotFound(invoice, "Invoice", invoiceID);
            return invoice;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteInvoice(this IQueryable<Invoice> invoices, List<int> invoiceIDList)
        {
            if(invoiceIDList.Any())
            {
                var invoicesInSourceCollectionToDelete = invoices.Where(x => invoiceIDList.Contains(x.InvoiceID));
                foreach (var invoiceToDelete in invoicesInSourceCollectionToDelete.ToList())
                {
                    invoiceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteInvoice(this IQueryable<Invoice> invoices, ICollection<Invoice> invoicesToDelete)
        {
            if(invoicesToDelete.Any())
            {
                var invoiceIDList = invoicesToDelete.Select(x => x.InvoiceID).ToList();
                var invoicesToDeleteFromSourceList = invoices.Where(x => invoiceIDList.Contains(x.InvoiceID)).ToList();

                foreach (var invoiceToDelete in invoicesToDeleteFromSourceList)
                {
                    invoiceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteInvoice(this IQueryable<Invoice> invoices, int invoiceID)
        {
            DeleteInvoice(invoices, new List<int> { invoiceID });
        }

        public static void DeleteInvoice(this IQueryable<Invoice> invoices, Invoice invoiceToDelete)
        {
            DeleteInvoice(invoices, new List<Invoice> { invoiceToDelete });
        }
    }
}