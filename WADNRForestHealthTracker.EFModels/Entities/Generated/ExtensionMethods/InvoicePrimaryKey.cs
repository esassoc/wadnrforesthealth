//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.Invoice
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InvoicePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<Invoice>
    {
        public InvoicePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InvoicePrimaryKey(Invoice invoice) : base(invoice){}

        public static implicit operator InvoicePrimaryKey(int primaryKeyValue)
        {
            return new InvoicePrimaryKey(primaryKeyValue);
        }

        public static implicit operator InvoicePrimaryKey(Invoice invoice)
        {
            return new InvoicePrimaryKey(invoice);
        }
    }
}