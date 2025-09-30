//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InvoicePaymentRequest
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InvoicePaymentRequestPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InvoicePaymentRequest>
    {
        public InvoicePaymentRequestPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InvoicePaymentRequestPrimaryKey(InvoicePaymentRequest invoicePaymentRequest) : base(invoicePaymentRequest){}

        public static implicit operator InvoicePaymentRequestPrimaryKey(int primaryKeyValue)
        {
            return new InvoicePaymentRequestPrimaryKey(primaryKeyValue);
        }

        public static implicit operator InvoicePaymentRequestPrimaryKey(InvoicePaymentRequest invoicePaymentRequest)
        {
            return new InvoicePaymentRequestPrimaryKey(invoicePaymentRequest);
        }
    }
}