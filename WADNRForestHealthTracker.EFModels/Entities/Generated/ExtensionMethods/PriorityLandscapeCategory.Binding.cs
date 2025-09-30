//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PriorityLandscapeCategory]
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
    public abstract partial class PriorityLandscapeCategory : IHavePrimaryKey
    {
        public static readonly PriorityLandscapeCategoryEast East = PriorityLandscapeCategoryEast.Instance;
        public static readonly PriorityLandscapeCategoryWest West = PriorityLandscapeCategoryWest.Instance;

        public static readonly List<PriorityLandscapeCategory> All;
        public static readonly ReadOnlyDictionary<int, PriorityLandscapeCategory> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static PriorityLandscapeCategory()
        {
            All = new List<PriorityLandscapeCategory> { East, West };
            AllLookupDictionary = new ReadOnlyDictionary<int, PriorityLandscapeCategory>(All.ToDictionary(x => x.PriorityLandscapeCategoryID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected PriorityLandscapeCategory(int priorityLandscapeCategoryID, string priorityLandscapeCategoryName, string priorityLandscapeCategoryDisplayName, string priorityLandscapeCategoryMapLayerColor)
        {
            PriorityLandscapeCategoryID = priorityLandscapeCategoryID;
            PriorityLandscapeCategoryName = priorityLandscapeCategoryName;
            PriorityLandscapeCategoryDisplayName = priorityLandscapeCategoryDisplayName;
            PriorityLandscapeCategoryMapLayerColor = priorityLandscapeCategoryMapLayerColor;
        }

        [Key]
        public int PriorityLandscapeCategoryID { get; private set; }
        public string PriorityLandscapeCategoryName { get; private set; }
        public string PriorityLandscapeCategoryDisplayName { get; private set; }
        public string PriorityLandscapeCategoryMapLayerColor { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return PriorityLandscapeCategoryID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(PriorityLandscapeCategory other)
        {
            if (other == null)
            {
                return false;
            }
            return other.PriorityLandscapeCategoryID == PriorityLandscapeCategoryID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as PriorityLandscapeCategory);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return PriorityLandscapeCategoryID;
        }

        public static bool operator ==(PriorityLandscapeCategory left, PriorityLandscapeCategory right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PriorityLandscapeCategory left, PriorityLandscapeCategory right)
        {
            return !Equals(left, right);
        }

        public PriorityLandscapeCategoryEnum ToEnum { get { return (PriorityLandscapeCategoryEnum)GetHashCode(); } }

        public static PriorityLandscapeCategory ToType(int enumValue)
        {
            return ToType((PriorityLandscapeCategoryEnum)enumValue);
        }

        public static PriorityLandscapeCategory ToType(PriorityLandscapeCategoryEnum enumValue)
        {
            switch (enumValue)
            {
                case PriorityLandscapeCategoryEnum.East:
                    return East;
                case PriorityLandscapeCategoryEnum.West:
                    return West;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum PriorityLandscapeCategoryEnum
    {
        East = 1,
        West = 2
    }

    public partial class PriorityLandscapeCategoryEast : PriorityLandscapeCategory
    {
        private PriorityLandscapeCategoryEast(int priorityLandscapeCategoryID, string priorityLandscapeCategoryName, string priorityLandscapeCategoryDisplayName, string priorityLandscapeCategoryMapLayerColor) : base(priorityLandscapeCategoryID, priorityLandscapeCategoryName, priorityLandscapeCategoryDisplayName, priorityLandscapeCategoryMapLayerColor) {}
        public static readonly PriorityLandscapeCategoryEast Instance = new PriorityLandscapeCategoryEast(1, @"East", @"20-Year Forest Health Strategic Plan: Eastern WA Priority Landscapes", @"#59ACFF");
    }

    public partial class PriorityLandscapeCategoryWest : PriorityLandscapeCategory
    {
        private PriorityLandscapeCategoryWest(int priorityLandscapeCategoryID, string priorityLandscapeCategoryName, string priorityLandscapeCategoryDisplayName, string priorityLandscapeCategoryMapLayerColor) : base(priorityLandscapeCategoryID, priorityLandscapeCategoryName, priorityLandscapeCategoryDisplayName, priorityLandscapeCategoryMapLayerColor) {}
        public static readonly PriorityLandscapeCategoryWest Instance = new PriorityLandscapeCategoryWest(2, @"West", @"Western WA Forest Health Priority Landscapes", @"#0267e3");
    }
}