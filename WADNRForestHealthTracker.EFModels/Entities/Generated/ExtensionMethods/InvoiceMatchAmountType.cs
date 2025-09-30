//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InvoiceMatchAmountType]
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
    public abstract partial class InvoiceMatchAmountType : IHavePrimaryKey
    {
        public static readonly InvoiceMatchAmountTypeDollarAmount DollarAmount = InvoiceMatchAmountTypeDollarAmount.Instance;
        public static readonly InvoiceMatchAmountTypeN_A N_A = InvoiceMatchAmountTypeN_A.Instance;
        public static readonly InvoiceMatchAmountTypeDNR DNR = InvoiceMatchAmountTypeDNR.Instance;

        public static readonly List<InvoiceMatchAmountType> All;
        public static readonly ReadOnlyDictionary<int, InvoiceMatchAmountType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static InvoiceMatchAmountType()
        {
            All = new List<InvoiceMatchAmountType> { DollarAmount, N_A, DNR };
            AllLookupDictionary = new ReadOnlyDictionary<int, InvoiceMatchAmountType>(All.ToDictionary(x => x.InvoiceMatchAmountTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected InvoiceMatchAmountType(int invoiceMatchAmountTypeID, string invoiceMatchAmountTypeName, string invoiceMatchAmountTypeDisplayName)
        {
            InvoiceMatchAmountTypeID = invoiceMatchAmountTypeID;
            InvoiceMatchAmountTypeName = invoiceMatchAmountTypeName;
            InvoiceMatchAmountTypeDisplayName = invoiceMatchAmountTypeDisplayName;
        }

        [Key]
        public int InvoiceMatchAmountTypeID { get; private set; }
        public string InvoiceMatchAmountTypeName { get; private set; }
        public string InvoiceMatchAmountTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return InvoiceMatchAmountTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(InvoiceMatchAmountType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.InvoiceMatchAmountTypeID == InvoiceMatchAmountTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as InvoiceMatchAmountType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return InvoiceMatchAmountTypeID;
        }

        public static bool operator ==(InvoiceMatchAmountType left, InvoiceMatchAmountType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InvoiceMatchAmountType left, InvoiceMatchAmountType right)
        {
            return !Equals(left, right);
        }

        public InvoiceMatchAmountTypeEnum ToEnum { get { return (InvoiceMatchAmountTypeEnum)GetHashCode(); } }

        public static InvoiceMatchAmountType ToType(int enumValue)
        {
            return ToType((InvoiceMatchAmountTypeEnum)enumValue);
        }

        public static InvoiceMatchAmountType ToType(InvoiceMatchAmountTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case InvoiceMatchAmountTypeEnum.DNR:
                    return DNR;
                case InvoiceMatchAmountTypeEnum.DollarAmount:
                    return DollarAmount;
                case InvoiceMatchAmountTypeEnum.N_A:
                    return N_A;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum InvoiceMatchAmountTypeEnum
    {
        DollarAmount = 1,
        N_A = 2,
        DNR = 3
    }

    public partial class InvoiceMatchAmountTypeDollarAmount : InvoiceMatchAmountType
    {
        private InvoiceMatchAmountTypeDollarAmount(int invoiceMatchAmountTypeID, string invoiceMatchAmountTypeName, string invoiceMatchAmountTypeDisplayName) : base(invoiceMatchAmountTypeID, invoiceMatchAmountTypeName, invoiceMatchAmountTypeDisplayName) {}
        public static readonly InvoiceMatchAmountTypeDollarAmount Instance = new InvoiceMatchAmountTypeDollarAmount(1, @"DollarAmount", @"Dollar Amount (enter amount in input below)");
    }

    public partial class InvoiceMatchAmountTypeN_A : InvoiceMatchAmountType
    {
        private InvoiceMatchAmountTypeN_A(int invoiceMatchAmountTypeID, string invoiceMatchAmountTypeName, string invoiceMatchAmountTypeDisplayName) : base(invoiceMatchAmountTypeID, invoiceMatchAmountTypeName, invoiceMatchAmountTypeDisplayName) {}
        public static readonly InvoiceMatchAmountTypeN_A Instance = new InvoiceMatchAmountTypeN_A(2, @"N/A", @"N/A");
    }

    public partial class InvoiceMatchAmountTypeDNR : InvoiceMatchAmountType
    {
        private InvoiceMatchAmountTypeDNR(int invoiceMatchAmountTypeID, string invoiceMatchAmountTypeName, string invoiceMatchAmountTypeDisplayName) : base(invoiceMatchAmountTypeID, invoiceMatchAmountTypeName, invoiceMatchAmountTypeDisplayName) {}
        public static readonly InvoiceMatchAmountTypeDNR Instance = new InvoiceMatchAmountTypeDNR(3, @"DNR", @"DNR");
    }
}