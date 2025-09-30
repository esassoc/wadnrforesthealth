//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.InvoiceApprovalStatus
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class InvoiceApprovalStatusPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<InvoiceApprovalStatus>
    {
        public InvoiceApprovalStatusPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public InvoiceApprovalStatusPrimaryKey(InvoiceApprovalStatus invoiceApprovalStatus) : base(invoiceApprovalStatus){}

        public static implicit operator InvoiceApprovalStatusPrimaryKey(int primaryKeyValue)
        {
            return new InvoiceApprovalStatusPrimaryKey(primaryKeyValue);
        }

        public static implicit operator InvoiceApprovalStatusPrimaryKey(InvoiceApprovalStatus invoiceApprovalStatus)
        {
            return new InvoiceApprovalStatusPrimaryKey(invoiceApprovalStatus);
        }
    }
}