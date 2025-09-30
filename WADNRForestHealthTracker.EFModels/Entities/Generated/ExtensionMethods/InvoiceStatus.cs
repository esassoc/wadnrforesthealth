//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InvoiceStatus]
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public abstract partial class InvoiceStatus : IHavePrimaryKey
    {
        public static readonly InvoiceStatusPending Pending = InvoiceStatusPending.Instance;
        public static readonly InvoiceStatusPaid Paid = InvoiceStatusPaid.Instance;
        public static readonly InvoiceStatusCanceled Canceled = InvoiceStatusCanceled.Instance;

        public static readonly List<InvoiceStatus> All;
        public static readonly ReadOnlyDictionary<int, InvoiceStatus> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static InvoiceStatus()
        {
            All = new List<InvoiceStatus> { Pending, Paid, Canceled };
            AllLookupDictionary = new ReadOnlyDictionary<int, InvoiceStatus>(All.ToDictionary(x => x.InvoiceStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected InvoiceStatus(int invoiceStatusID, string invoiceStatusName, string invoiceStatusDisplayName)
        {
            InvoiceStatusID = invoiceStatusID;
            InvoiceStatusName = invoiceStatusName;
            InvoiceStatusDisplayName = invoiceStatusDisplayName;
        }

        [Key]
        public int InvoiceStatusID { get; private set; }
        public string InvoiceStatusName { get; private set; }
        public string InvoiceStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return InvoiceStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(InvoiceStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.InvoiceStatusID == InvoiceStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as InvoiceStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return InvoiceStatusID;
        }

        public static bool operator ==(InvoiceStatus left, InvoiceStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InvoiceStatus left, InvoiceStatus right)
        {
            return !Equals(left, right);
        }

        public InvoiceStatusEnum ToEnum { get { return (InvoiceStatusEnum)GetHashCode(); } }

        public static InvoiceStatus ToType(int enumValue)
        {
            return ToType((InvoiceStatusEnum)enumValue);
        }

        public static InvoiceStatus ToType(InvoiceStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case InvoiceStatusEnum.Canceled:
                    return Canceled;
                case InvoiceStatusEnum.Paid:
                    return Paid;
                case InvoiceStatusEnum.Pending:
                    return Pending;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum InvoiceStatusEnum
    {
        Pending = 1,
        Paid = 2,
        Canceled = 3
    }

    public partial class InvoiceStatusPending : InvoiceStatus
    {
        private InvoiceStatusPending(int invoiceStatusID, string invoiceStatusName, string invoiceStatusDisplayName) : base(invoiceStatusID, invoiceStatusName, invoiceStatusDisplayName) {}
        public static readonly InvoiceStatusPending Instance = new InvoiceStatusPending(1, @"Pending", @"Pending");
    }

    public partial class InvoiceStatusPaid : InvoiceStatus
    {
        private InvoiceStatusPaid(int invoiceStatusID, string invoiceStatusName, string invoiceStatusDisplayName) : base(invoiceStatusID, invoiceStatusName, invoiceStatusDisplayName) {}
        public static readonly InvoiceStatusPaid Instance = new InvoiceStatusPaid(2, @"Paid", @"Paid");
    }

    public partial class InvoiceStatusCanceled : InvoiceStatus
    {
        private InvoiceStatusCanceled(int invoiceStatusID, string invoiceStatusName, string invoiceStatusDisplayName) : base(invoiceStatusID, invoiceStatusName, invoiceStatusDisplayName) {}
        public static readonly InvoiceStatusCanceled Instance = new InvoiceStatusCanceled(3, @"Canceled", @"Canceled");
    }
}