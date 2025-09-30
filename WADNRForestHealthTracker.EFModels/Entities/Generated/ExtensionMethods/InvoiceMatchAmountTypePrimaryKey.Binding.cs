//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InvoiceMatchAmountType
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InvoiceMatchAmountTypePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InvoiceMatchAmountType>
    {
        public InvoiceMatchAmountTypePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InvoiceMatchAmountTypePrimaryKey(InvoiceMatchAmountType invoiceMatchAmountType) : base(invoiceMatchAmountType){}

        public static implicit operator InvoiceMatchAmountTypePrimaryKey(int primaryKeyValue)
        {
            return new InvoiceMatchAmountTypePrimaryKey(primaryKeyValue);
        }

        public static implicit operator InvoiceMatchAmountTypePrimaryKey(InvoiceMatchAmountType invoiceMatchAmountType)
        {
            return new InvoiceMatchAmountTypePrimaryKey(invoiceMatchAmountType);
        }
    }
}