//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Invoice]
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    // Table [dbo].[Invoice] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[Invoice]")]
    public partial class Invoice : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected Invoice()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public Invoice(int invoiceID, string invoiceIdentifyingName, DateTime invoiceDate, decimal? paymentAmount, int invoiceApprovalStatusID, string invoiceApprovalStatusComment, int invoiceMatchAmountTypeID, decimal? matchAmount, int invoiceStatusID, int? invoiceFileResourceID, int invoicePaymentRequestID, int? fundSourceID, int? programIndexID, int? projectCodeID, int? organizationCodeID, string invoiceNumber, string fund, string appn, string subObject) : this()
        {
            this.InvoiceID = invoiceID;
            this.InvoiceIdentifyingName = invoiceIdentifyingName;
            this.InvoiceDate = invoiceDate;
            this.PaymentAmount = paymentAmount;
            this.InvoiceApprovalStatusID = invoiceApprovalStatusID;
            this.InvoiceApprovalStatusComment = invoiceApprovalStatusComment;
            this.InvoiceMatchAmountTypeID = invoiceMatchAmountTypeID;
            this.MatchAmount = matchAmount;
            this.InvoiceStatusID = invoiceStatusID;
            this.InvoiceFileResourceID = invoiceFileResourceID;
            this.InvoicePaymentRequestID = invoicePaymentRequestID;
            this.FundSourceID = fundSourceID;
            this.ProgramIndexID = programIndexID;
            this.ProjectCodeID = projectCodeID;
            this.OrganizationCodeID = organizationCodeID;
            this.InvoiceNumber = invoiceNumber;
            this.Fund = fund;
            this.Appn = appn;
            this.SubObject = subObject;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public Invoice(DateTime invoiceDate, int invoiceApprovalStatusID, int invoiceMatchAmountTypeID, int invoiceStatusID, int invoicePaymentRequestID, string invoiceNumber) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InvoiceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.InvoiceDate = invoiceDate;
            this.InvoiceApprovalStatusID = invoiceApprovalStatusID;
            this.InvoiceMatchAmountTypeID = invoiceMatchAmountTypeID;
            this.InvoiceStatusID = invoiceStatusID;
            this.InvoicePaymentRequestID = invoicePaymentRequestID;
            this.InvoiceNumber = invoiceNumber;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public Invoice(DateTime invoiceDate, InvoiceApprovalStatus invoiceApprovalStatus, InvoiceMatchAmountType invoiceMatchAmountType, InvoiceStatus invoiceStatus, InvoicePaymentRequest invoicePaymentRequest, string invoiceNumber) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InvoiceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.InvoiceDate = invoiceDate;
            this.InvoiceApprovalStatusID = invoiceApprovalStatus.InvoiceApprovalStatusID;
            this.InvoiceMatchAmountTypeID = invoiceMatchAmountType.InvoiceMatchAmountTypeID;
            this.InvoiceStatusID = invoiceStatus.InvoiceStatusID;
            this.InvoicePaymentRequestID = invoicePaymentRequest.InvoicePaymentRequestID;
            this.InvoicePaymentRequest = invoicePaymentRequest;
            invoicePaymentRequest.Invoices.Add(this);
            this.InvoiceNumber = invoiceNumber;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static Invoice CreateNewBlank(InvoiceApprovalStatus invoiceApprovalStatus, InvoiceMatchAmountType invoiceMatchAmountType, InvoiceStatus invoiceStatus, InvoicePaymentRequest invoicePaymentRequest)
        {
            return new Invoice(default(DateTime), invoiceApprovalStatus, invoiceMatchAmountType, invoiceStatus, invoicePaymentRequest, default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return false;
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(Invoice).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.Invoices.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int InvoiceID { get; set; }
        public string InvoiceIdentifyingName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public int InvoiceApprovalStatusID { get; set; }
        public string InvoiceApprovalStatusComment { get; set; }
        public int InvoiceMatchAmountTypeID { get; set; }
        public decimal? MatchAmount { get; set; }
        public int InvoiceStatusID { get; set; }
        public int? InvoiceFileResourceID { get; set; }
        public int InvoicePaymentRequestID { get; set; }
        public int? FundSourceID { get; set; }
        public int? ProgramIndexID { get; set; }
        public int? ProjectCodeID { get; set; }
        public int? OrganizationCodeID { get; set; }
        public string InvoiceNumber { get; set; }
        public string Fund { get; set; }
        public string Appn { get; set; }
        public string SubObject { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return InvoiceID; } set { InvoiceID = value; } }

        public InvoiceApprovalStatus InvoiceApprovalStatus { get { return InvoiceApprovalStatus.AllLookupDictionary[InvoiceApprovalStatusID]; } }
        public InvoiceMatchAmountType InvoiceMatchAmountType { get { return InvoiceMatchAmountType.AllLookupDictionary[InvoiceMatchAmountTypeID]; } }
        public InvoiceStatus InvoiceStatus { get { return InvoiceStatus.AllLookupDictionary[InvoiceStatusID]; } }
        public virtual FileResource InvoiceFileResource { get; set; }
        public virtual InvoicePaymentRequest InvoicePaymentRequest { get; set; }
        public virtual FundSource FundSource { get; set; }
        public virtual ProgramIndex ProgramIndex { get; set; }
        public virtual ProjectCode ProjectCode { get; set; }
        public virtual OrganizationCode OrganizationCode { get; set; }

        public static class FieldLengths
        {
            public const int InvoiceIdentifyingName = 255;
            public const int InvoiceNumber = 50;
            public const int Fund = 255;
            public const int Appn = 255;
            public const int SubObject = 255;
        }
    }
}