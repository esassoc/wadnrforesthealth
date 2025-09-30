//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Vendor]
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
    // Table [dbo].[Vendor] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[Vendor]")]
    public partial class Vendor : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected Vendor()
        {
            this.InvoicePaymentRequests = new HashSet<InvoicePaymentRequest>();
            this.Organizations = new HashSet<Organization>();
            this.People = new HashSet<Person>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public Vendor(int vendorID, string vendorName, string statewideVendorNumber, string statewideVendorNumberSuffix, string vendorType, string billingAgency, string billingSubAgency, string billingFund, string billingFundBreakout, string vendorAddressLine1, string vendorAddressLine2, string vendorAddressLine3, string vendorCity, string vendorState, string vendorZip, string remarks, string vendorPhone, string vendorStatus, string taxpayerIdNumber, string email) : this()
        {
            this.VendorID = vendorID;
            this.VendorName = vendorName;
            this.StatewideVendorNumber = statewideVendorNumber;
            this.StatewideVendorNumberSuffix = statewideVendorNumberSuffix;
            this.VendorType = vendorType;
            this.BillingAgency = billingAgency;
            this.BillingSubAgency = billingSubAgency;
            this.BillingFund = billingFund;
            this.BillingFundBreakout = billingFundBreakout;
            this.VendorAddressLine1 = vendorAddressLine1;
            this.VendorAddressLine2 = vendorAddressLine2;
            this.VendorAddressLine3 = vendorAddressLine3;
            this.VendorCity = vendorCity;
            this.VendorState = vendorState;
            this.VendorZip = vendorZip;
            this.Remarks = remarks;
            this.VendorPhone = vendorPhone;
            this.VendorStatus = vendorStatus;
            this.TaxpayerIdNumber = taxpayerIdNumber;
            this.Email = email;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public Vendor(string vendorName, string statewideVendorNumber, string statewideVendorNumberSuffix) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.VendorID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.VendorName = vendorName;
            this.StatewideVendorNumber = statewideVendorNumber;
            this.StatewideVendorNumberSuffix = statewideVendorNumberSuffix;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static Vendor CreateNewBlank()
        {
            return new Vendor(default(string), default(string), default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return InvoicePaymentRequests.Any() || Organizations.Any() || People.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(InvoicePaymentRequests.Any())
            {
                dependentObjects.Add(typeof(InvoicePaymentRequest).Name);
            }

            if(Organizations.Any())
            {
                dependentObjects.Add(typeof(Organization).Name);
            }

            if(People.Any())
            {
                dependentObjects.Add(typeof(Person).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(Vendor).Name, typeof(InvoicePaymentRequest).Name, typeof(Organization).Name, typeof(Person).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.Vendors.Remove(this);
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

            foreach(var x in InvoicePaymentRequests.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in Organizations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in People.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string StatewideVendorNumber { get; set; }
        public string StatewideVendorNumberSuffix { get; set; }
        public string VendorType { get; set; }
        public string BillingAgency { get; set; }
        public string BillingSubAgency { get; set; }
        public string BillingFund { get; set; }
        public string BillingFundBreakout { get; set; }
        public string VendorAddressLine1 { get; set; }
        public string VendorAddressLine2 { get; set; }
        public string VendorAddressLine3 { get; set; }
        public string VendorCity { get; set; }
        public string VendorState { get; set; }
        public string VendorZip { get; set; }
        public string Remarks { get; set; }
        public string VendorPhone { get; set; }
        public string VendorStatus { get; set; }
        public string TaxpayerIdNumber { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return VendorID; } set { VendorID = value; } }

        public virtual ICollection<InvoicePaymentRequest> InvoicePaymentRequests { get; set; }
        public virtual ICollection<Organization> Organizations { get; set; }
        public virtual ICollection<Person> People { get; set; }

        public static class FieldLengths
        {
            public const int VendorName = 100;
            public const int StatewideVendorNumber = 20;
            public const int StatewideVendorNumberSuffix = 10;
            public const int VendorType = 3;
            public const int BillingAgency = 200;
            public const int BillingSubAgency = 200;
            public const int BillingFund = 200;
            public const int BillingFundBreakout = 200;
            public const int VendorAddressLine1 = 200;
            public const int VendorAddressLine2 = 200;
            public const int VendorAddressLine3 = 200;
            public const int VendorCity = 200;
            public const int VendorState = 200;
            public const int VendorZip = 200;
            public const int Remarks = 200;
            public const int VendorPhone = 200;
            public const int VendorStatus = 200;
            public const int TaxpayerIdNumber = 200;
            public const int Email = 200;
        }
    }
}