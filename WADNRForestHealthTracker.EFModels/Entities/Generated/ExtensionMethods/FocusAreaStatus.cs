//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FocusAreaStatus]
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
    public abstract partial class FocusAreaStatus : IHavePrimaryKey
    {
        public static readonly FocusAreaStatusPlanned Planned = FocusAreaStatusPlanned.Instance;
        public static readonly FocusAreaStatusInProgress InProgress = FocusAreaStatusInProgress.Instance;
        public static readonly FocusAreaStatusCompleted Completed = FocusAreaStatusCompleted.Instance;

        public static readonly List<FocusAreaStatus> All;
        public static readonly ReadOnlyDictionary<int, FocusAreaStatus> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static FocusAreaStatus()
        {
            All = new List<FocusAreaStatus> { Planned, InProgress, Completed };
            AllLookupDictionary = new ReadOnlyDictionary<int, FocusAreaStatus>(All.ToDictionary(x => x.FocusAreaStatusID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected FocusAreaStatus(int focusAreaStatusID, string focusAreaStatusName, string focusAreaStatusDisplayName)
        {
            FocusAreaStatusID = focusAreaStatusID;
            FocusAreaStatusName = focusAreaStatusName;
            FocusAreaStatusDisplayName = focusAreaStatusDisplayName;
        }

        [Key]
        public int FocusAreaStatusID { get; private set; }
        public string FocusAreaStatusName { get; private set; }
        public string FocusAreaStatusDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return FocusAreaStatusID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(FocusAreaStatus other)
        {
            if (other == null)
            {
                return false;
            }
            return other.FocusAreaStatusID == FocusAreaStatusID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as FocusAreaStatus);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return FocusAreaStatusID;
        }

        public static bool operator ==(FocusAreaStatus left, FocusAreaStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FocusAreaStatus left, FocusAreaStatus right)
        {
            return !Equals(left, right);
        }

        public FocusAreaStatusEnum ToEnum { get { return (FocusAreaStatusEnum)GetHashCode(); } }

        public static FocusAreaStatus ToType(int enumValue)
        {
            return ToType((FocusAreaStatusEnum)enumValue);
        }

        public static FocusAreaStatus ToType(FocusAreaStatusEnum enumValue)
        {
            switch (enumValue)
            {
                case FocusAreaStatusEnum.Completed:
                    return Completed;
                case FocusAreaStatusEnum.InProgress:
                    return InProgress;
                case FocusAreaStatusEnum.Planned:
                    return Planned;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum FocusAreaStatusEnum
    {
        Planned = 1,
        InProgress = 2,
        Completed = 3
    }

    public partial class FocusAreaStatusPlanned : FocusAreaStatus
    {
        private FocusAreaStatusPlanned(int focusAreaStatusID, string focusAreaStatusName, string focusAreaStatusDisplayName) : base(focusAreaStatusID, focusAreaStatusName, focusAreaStatusDisplayName) {}
        public static readonly FocusAreaStatusPlanned Instance = new FocusAreaStatusPlanned(1, @"Planned", @"Planned");
    }

    public partial class FocusAreaStatusInProgress : FocusAreaStatus
    {
        private FocusAreaStatusInProgress(int focusAreaStatusID, string focusAreaStatusName, string focusAreaStatusDisplayName) : base(focusAreaStatusID, focusAreaStatusName, focusAreaStatusDisplayName) {}
        public static readonly FocusAreaStatusInProgress Instance = new FocusAreaStatusInProgress(2, @"In Progress", @"In Progress");
    }

    public partial class FocusAreaStatusCompleted : FocusAreaStatus
    {
        private FocusAreaStatusCompleted(int focusAreaStatusID, string focusAreaStatusName, string focusAreaStatusDisplayName) : base(focusAreaStatusID, focusAreaStatusName, focusAreaStatusDisplayName) {}
        public static readonly FocusAreaStatusCompleted Instance = new FocusAreaStatusCompleted(3, @"Completed", @"Completed");
    }
}