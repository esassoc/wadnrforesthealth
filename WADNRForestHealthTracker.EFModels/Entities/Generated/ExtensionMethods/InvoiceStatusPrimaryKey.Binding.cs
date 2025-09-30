//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InvoiceStatus
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InvoiceStatusPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InvoiceStatus>
    {
        public InvoiceStatusPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InvoiceStatusPrimaryKey(InvoiceStatus invoiceStatus) : base(invoiceStatus){}

        public static implicit operator InvoiceStatusPrimaryKey(int primaryKeyValue)
        {
            return new InvoiceStatusPrimaryKey(primaryKeyValue);
        }

        public static implicit operator InvoiceStatusPrimaryKey(InvoiceStatus invoiceStatus)
        {
            return new InvoiceStatusPrimaryKey(invoiceStatus);
        }
    }
}