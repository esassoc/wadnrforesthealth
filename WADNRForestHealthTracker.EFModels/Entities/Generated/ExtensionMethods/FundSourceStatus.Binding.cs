//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceStatus]
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
    public abstract partial class FundSourceStatus : IHavePrimaryKey
    {
        public static readonly FundSourceStatusActive Active = FundSourceStatusActive.Instance;
        public static readonly FundSourceStatusPending Pending = FundSourceStatusPending.Instance;
        public static readonly FundSourceStatusPlanned Planned = FundSourceStatusPlanned.Instance;
        public static readonly FundSourceStatusCloseout Closeout = FundSourceStatusCloseout.Instance;

        public static readonly List<FundSourceStatus> All;
        public static readonly ReadOnlyDictionary<int, FundSourceStatus> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static FundSourceStatus()
        {
            All = new List<FundSourceStatus> { Active, Pending, Planned, Closeout };
            AllLookupDictionary = new ReadOnlyDictionary<int, FundSourceStatus>(All.ToDictionary(x => x.FundSourceStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected FundSourceStatus(int fundSourceStatusID, string fundSourceStatusName)
        {
            FundSourceStatusID = fundSourceStatusID;
            FundSourceStatusName = fundSourceStatusName;
        }

        [Key]
        public int FundSourceStatusID { get; private set; }
        public string FundSourceStatusName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(FundSourceStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.FundSourceStatusID == FundSourceStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as FundSourceStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return FundSourceStatusID;
        }

        public static bool operator ==(FundSourceStatus left, FundSourceStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FundSourceStatus left, FundSourceStatus right)
        {
            return !Equals(left, right);
        }

        public FundSourceStatusEnum ToEnum { get { return (FundSourceStatusEnum)GetHashCode(); } }

        public static FundSourceStatus ToType(int enumValue)
        {
            return ToType((FundSourceStatusEnum)enumValue);
        }

        public static FundSourceStatus ToType(FundSourceStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case FundSourceStatusEnum.Active:
                    return Active;
                case FundSourceStatusEnum.Closeout:
                    return Closeout;
                case FundSourceStatusEnum.Pending:
                    return Pending;
                case FundSourceStatusEnum.Planned:
                    return Planned;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum FundSourceStatusEnum
    {
        Active = 1,
        Pending = 2,
        Planned = 3,
        Closeout = 4
    }

    public partial class FundSourceStatusActive : FundSourceStatus
    {
        private FundSourceStatusActive(int fundSourceStatusID, string fundSourceStatusName) : base(fundSourceStatusID, fundSourceStatusName) {}
        public static readonly FundSourceStatusActive Instance = new FundSourceStatusActive(1, @"Active");
    }

    public partial class FundSourceStatusPending : FundSourceStatus
    {
        private FundSourceStatusPending(int fundSourceStatusID, string fundSourceStatusName) : base(fundSourceStatusID, fundSourceStatusName) {}
        public static readonly FundSourceStatusPending Instance = new FundSourceStatusPending(2, @"Pending");
    }

    public partial class FundSourceStatusPlanned : FundSourceStatus
    {
        private FundSourceStatusPlanned(int fundSourceStatusID, string fundSourceStatusName) : base(fundSourceStatusID, fundSourceStatusName) {}
        public static readonly FundSourceStatusPlanned Instance = new FundSourceStatusPlanned(3, @"Planned");
    }

    public partial class FundSourceStatusCloseout : FundSourceStatus
    {
        private FundSourceStatusCloseout(int fundSourceStatusID, string fundSourceStatusName) : base(fundSourceStatusID, fundSourceStatusName) {}
        public static readonly FundSourceStatusCloseout Instance = new FundSourceStatusCloseout(4, @"Closeout");
    }
}