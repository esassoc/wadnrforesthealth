//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InvoiceApprovalStatus]
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
    public abstract partial class InvoiceApprovalStatus : IHavePrimaryKey
    {
        public static readonly InvoiceApprovalStatusNotSet NotSet = InvoiceApprovalStatusNotSet.Instance;
        public static readonly InvoiceApprovalStatusApproved Approved = InvoiceApprovalStatusApproved.Instance;
        public static readonly InvoiceApprovalStatusDenied Denied = InvoiceApprovalStatusDenied.Instance;

        public static readonly List<InvoiceApprovalStatus> All;
        public static readonly ReadOnlyDictionary<int, InvoiceApprovalStatus> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static InvoiceApprovalStatus()
        {
            All = new List<InvoiceApprovalStatus> { NotSet, Approved, Denied };
            AllLookupDictionary = new ReadOnlyDictionary<int, InvoiceApprovalStatus>(All.ToDictionary(x => x.InvoiceApprovalStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected InvoiceApprovalStatus(int invoiceApprovalStatusID, string invoiceApprovalStatusName)
        {
            InvoiceApprovalStatusID = invoiceApprovalStatusID;
            InvoiceApprovalStatusName = invoiceApprovalStatusName;
        }

        [Key]
        public int InvoiceApprovalStatusID { get; private set; }
        public string InvoiceApprovalStatusName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return InvoiceApprovalStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(InvoiceApprovalStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.InvoiceApprovalStatusID == InvoiceApprovalStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as InvoiceApprovalStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return InvoiceApprovalStatusID;
        }

        public static bool operator ==(InvoiceApprovalStatus left, InvoiceApprovalStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InvoiceApprovalStatus left, InvoiceApprovalStatus right)
        {
            return !Equals(left, right);
        }

        public InvoiceApprovalStatusEnum ToEnum { get { return (InvoiceApprovalStatusEnum)GetHashCode(); } }

        public static InvoiceApprovalStatus ToType(int enumValue)
        {
            return ToType((InvoiceApprovalStatusEnum)enumValue);
        }

        public static InvoiceApprovalStatus ToType(InvoiceApprovalStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case InvoiceApprovalStatusEnum.Approved:
                    return Approved;
                case InvoiceApprovalStatusEnum.Denied:
                    return Denied;
                case InvoiceApprovalStatusEnum.NotSet:
                    return NotSet;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum InvoiceApprovalStatusEnum
    {
        NotSet = 1,
        Approved = 2,
        Denied = 3
    }

    public partial class InvoiceApprovalStatusNotSet : InvoiceApprovalStatus
    {
        private InvoiceApprovalStatusNotSet(int invoiceApprovalStatusID, string invoiceApprovalStatusName) : base(invoiceApprovalStatusID, invoiceApprovalStatusName) {}
        public static readonly InvoiceApprovalStatusNotSet Instance = new InvoiceApprovalStatusNotSet(1, @"Not Set");
    }

    public partial class InvoiceApprovalStatusApproved : InvoiceApprovalStatus
    {
        private InvoiceApprovalStatusApproved(int invoiceApprovalStatusID, string invoiceApprovalStatusName) : base(invoiceApprovalStatusID, invoiceApprovalStatusName) {}
        public static readonly InvoiceApprovalStatusApproved Instance = new InvoiceApprovalStatusApproved(2, @"Approved");
    }

    public partial class InvoiceApprovalStatusDenied : InvoiceApprovalStatus
    {
        private InvoiceApprovalStatusDenied(int invoiceApprovalStatusID, string invoiceApprovalStatusName) : base(invoiceApprovalStatusID, invoiceApprovalStatusName) {}
        public static readonly InvoiceApprovalStatusDenied Instance = new InvoiceApprovalStatusDenied(3, @"Denied");
    }
}