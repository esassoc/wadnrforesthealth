//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundingSource]
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
    public abstract partial class FundingSource : IHavePrimaryKey
    {
        public static readonly FundingSourceFederal Federal = FundingSourceFederal.Instance;
        public static readonly FundingSourceState State = FundingSourceState.Instance;
        public static readonly FundingSourcePrivate Private = FundingSourcePrivate.Instance;
        public static readonly FundingSourceOther Other = FundingSourceOther.Instance;

        public static readonly List<FundingSource> All;
        public static readonly ReadOnlyDictionary<int, FundingSource> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static FundingSource()
        {
            All = new List<FundingSource> { Federal, State, Private, Other };
            AllLookupDictionary = new ReadOnlyDictionary<int, FundingSource>(All.ToDictionary(x => x.FundingSourceID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected FundingSource(int fundingSourceID, string fundingSourceName, string fundingSourceDisplayName)
        {
            FundingSourceID = fundingSourceID;
            FundingSourceName = fundingSourceName;
            FundingSourceDisplayName = fundingSourceDisplayName;
        }

        [Key]
        public int FundingSourceID { get; private set; }
        public string FundingSourceName { get; private set; }
        public string FundingSourceDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return FundingSourceID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(FundingSource other)
        {
            if (other == null)
            {
                return false;
            }
            return other.FundingSourceID == FundingSourceID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as FundingSource);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return FundingSourceID;
        }

        public static bool operator ==(FundingSource left, FundingSource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FundingSource left, FundingSource right)
        {
            return !Equals(left, right);
        }

        public FundingSourceEnum ToEnum { get { return (FundingSourceEnum)GetHashCode(); } }

        public static FundingSource ToType(int enumValue)
        {
            return ToType((FundingSourceEnum)enumValue);
        }

        public static FundingSource ToType(FundingSourceEnum enumValue)
        {
            switch (enumValue)
            {
                case FundingSourceEnum.Federal:
                    return Federal;
                case FundingSourceEnum.Other:
                    return Other;
                case FundingSourceEnum.Private:
                    return Private;
                case FundingSourceEnum.State:
                    return State;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum FundingSourceEnum
    {
        Federal = 1,
        State = 2,
        Private = 3,
        Other = 4
    }

    public partial class FundingSourceFederal : FundingSource
    {
        private FundingSourceFederal(int fundingSourceID, string fundingSourceName, string fundingSourceDisplayName) : base(fundingSourceID, fundingSourceName, fundingSourceDisplayName) {}
        public static readonly FundingSourceFederal Instance = new FundingSourceFederal(1, @"Federal", @"Federal");
    }

    public partial class FundingSourceState : FundingSource
    {
        private FundingSourceState(int fundingSourceID, string fundingSourceName, string fundingSourceDisplayName) : base(fundingSourceID, fundingSourceName, fundingSourceDisplayName) {}
        public static readonly FundingSourceState Instance = new FundingSourceState(2, @"State", @"State");
    }

    public partial class FundingSourcePrivate : FundingSource
    {
        private FundingSourcePrivate(int fundingSourceID, string fundingSourceName, string fundingSourceDisplayName) : base(fundingSourceID, fundingSourceName, fundingSourceDisplayName) {}
        public static readonly FundingSourcePrivate Instance = new FundingSourcePrivate(3, @"Private", @"Private");
    }

    public partial class FundingSourceOther : FundingSource
    {
        private FundingSourceOther(int fundingSourceID, string fundingSourceName, string fundingSourceDisplayName) : base(fundingSourceID, fundingSourceName, fundingSourceDisplayName) {}
        public static readonly FundingSourceOther Instance = new FundingSourceOther(4, @"Other", @"Other");
    }
}