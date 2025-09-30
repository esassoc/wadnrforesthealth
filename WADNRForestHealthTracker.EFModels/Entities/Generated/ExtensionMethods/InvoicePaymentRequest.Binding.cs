//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InvoicePaymentRequest]
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
    // Table [dbo].[InvoicePaymentRequest] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[InvoicePaymentRequest]")]
    public partial class InvoicePaymentRequest : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected InvoicePaymentRequest()
        {
            this.Invoices = new HashSet<Invoice>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public InvoicePaymentRequest(int invoicePaymentRequestID, int projectID, int? vendorID, int? preparedByPersonID, string purchaseAuthority, bool purchaseAuthorityIsLandownerCostShareAgreement, string duns, DateTime invoicePaymentRequestDate, string notes) : this()
        {
            this.InvoicePaymentRequestID = invoicePaymentRequestID;
            this.ProjectID = projectID;
            this.VendorID = vendorID;
            this.PreparedByPersonID = preparedByPersonID;
            this.PurchaseAuthority = purchaseAuthority;
            this.PurchaseAuthorityIsLandownerCostShareAgreement = purchaseAuthorityIsLandownerCostShareAgreement;
            this.Duns = duns;
            this.InvoicePaymentRequestDate = invoicePaymentRequestDate;
            this.Notes = notes;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public InvoicePaymentRequest(int projectID, bool purchaseAuthorityIsLandownerCostShareAgreement, DateTime invoicePaymentRequestDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InvoicePaymentRequestID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectID = projectID;
            this.PurchaseAuthorityIsLandownerCostShareAgreement = purchaseAuthorityIsLandownerCostShareAgreement;
            this.InvoicePaymentRequestDate = invoicePaymentRequestDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public InvoicePaymentRequest(Project project, bool purchaseAuthorityIsLandownerCostShareAgreement, DateTime invoicePaymentRequestDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InvoicePaymentRequestID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectID = project.ProjectID;
            this.Project = project;
            project.InvoicePaymentRequests.Add(this);
            this.PurchaseAuthorityIsLandownerCostShareAgreement = purchaseAuthorityIsLandownerCostShareAgreement;
            this.InvoicePaymentRequestDate = invoicePaymentRequestDate;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static InvoicePaymentRequest CreateNewBlank(Project project)
        {
            return new InvoicePaymentRequest(project, default(bool), default(DateTime));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return Invoices.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(Invoices.Any())
            {
                dependentObjects.Add(typeof(Invoice).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(InvoicePaymentRequest).Name, typeof(Invoice).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.InvoicePaymentRequests.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            DeleteChildren(dbContext);
            Delete(dbContext);
        }
        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public void DeleteChildren(DatabaseEntities dbContext)
        {

            foreach(var x in Invoices.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int InvoicePaymentRequestID { get; set; }
        public int ProjectID { get; set; }
        public int? VendorID { get; set; }
        public int? PreparedByPersonID { get; set; }
        public string PurchaseAuthority { get; set; }
        public bool PurchaseAuthorityIsLandownerCostShareAgreement { get; set; }
        public string Duns { get; set; }
        public DateTime InvoicePaymentRequestDate { get; set; }
        public string Notes { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return InvoicePaymentRequestID; } set { InvoicePaymentRequestID = value; } }

        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual Project Project { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual Person PreparedByPerson { get; set; }

        public static class FieldLengths
        {
            public const int PurchaseAuthority = 255;
            public const int Duns = 20;
        }
    }
}