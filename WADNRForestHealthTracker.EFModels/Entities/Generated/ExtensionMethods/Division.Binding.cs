//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Division]
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
    public abstract partial class Division : IHavePrimaryKey
    {
        public static readonly DivisionForestHealth ForestHealth = DivisionForestHealth.Instance;
        public static readonly DivisionWildfire Wildfire = DivisionWildfire.Instance;

        public static readonly List<Division> All;
        public static readonly ReadOnlyDictionary<int, Division> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static Division()
        {
            All = new List<Division> { ForestHealth, Wildfire };
            AllLookupDictionary = new ReadOnlyDictionary<int, Division>(All.ToDictionary(x => x.DivisionID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected Division(int divisionID, string divisionName, string divisionDisplayName)
        {
            DivisionID = divisionID;
            DivisionName = divisionName;
            DivisionDisplayName = divisionDisplayName;
        }

        [Key]
        public int DivisionID { get; private set; }
        public string DivisionName { get; private set; }
        public string DivisionDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return DivisionID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(Division other)
        {
            if (other == null)
            {
                return false;
            }
            return other.DivisionID == DivisionID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Division);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return DivisionID;
        }

        public static bool operator ==(Division left, Division right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Division left, Division right)
        {
            return !Equals(left, right);
        }

        public DivisionEnum ToEnum { get { return (DivisionEnum)GetHashCode(); } }

        public static Division ToType(int enumValue)
        {
            return ToType((DivisionEnum)enumValue);
        }

        public static Division ToType(DivisionEnum enumValue)
        {
            switch (enumValue)
            {
                case DivisionEnum.ForestHealth:
                    return ForestHealth;
                case DivisionEnum.Wildfire:
                    return Wildfire;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum DivisionEnum
    {
        ForestHealth = 1,
        Wildfire = 2
    }

    public partial class DivisionForestHealth : Division
    {
        private DivisionForestHealth(int divisionID, string divisionName, string divisionDisplayName) : base(divisionID, divisionName, divisionDisplayName) {}
        public static readonly DivisionForestHealth Instance = new DivisionForestHealth(1, @"ForestHealth", @"DNR Headquarters - Forest Health");
    }

    public partial class DivisionWildfire : Division
    {
        private DivisionWildfire(int divisionID, string divisionName, string divisionDisplayName) : base(divisionID, divisionName, divisionDisplayName) {}
        public static readonly DivisionWildfire Instance = new DivisionWildfire(2, @"Wildfire", @"DNR Headquarters - Wildfire");
    }
}