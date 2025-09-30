//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[RecurrenceInterval]
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
    public abstract partial class RecurrenceInterval : IHavePrimaryKey
    {
        public static readonly RecurrenceIntervalOneYear OneYear = RecurrenceIntervalOneYear.Instance;
        public static readonly RecurrenceIntervalFiveYears FiveYears = RecurrenceIntervalFiveYears.Instance;
        public static readonly RecurrenceIntervalTenYears TenYears = RecurrenceIntervalTenYears.Instance;
        public static readonly RecurrenceIntervalFifteenYears FifteenYears = RecurrenceIntervalFifteenYears.Instance;

        public static readonly List<RecurrenceInterval> All;
        public static readonly ReadOnlyDictionary<int, RecurrenceInterval> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static RecurrenceInterval()
        {
            All = new List<RecurrenceInterval> { OneYear, FiveYears, TenYears, FifteenYears };
            AllLookupDictionary = new ReadOnlyDictionary<int, RecurrenceInterval>(All.ToDictionary(x => x.RecurrenceIntervalID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected RecurrenceInterval(int recurrenceIntervalID, int recurrenceIntervalInYears, string recurrenceIntervalDisplayName, string recurrenceIntervalName)
        {
            RecurrenceIntervalID = recurrenceIntervalID;
            RecurrenceIntervalInYears = recurrenceIntervalInYears;
            RecurrenceIntervalDisplayName = recurrenceIntervalDisplayName;
            RecurrenceIntervalName = recurrenceIntervalName;
        }

        [Key]
        public int RecurrenceIntervalID { get; private set; }
        public int RecurrenceIntervalInYears { get; private set; }
        public string RecurrenceIntervalDisplayName { get; private set; }
        public string RecurrenceIntervalName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return RecurrenceIntervalID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(RecurrenceInterval other)
        {
            if (other == null)
            {
                return false;
            }
            return other.RecurrenceIntervalID == RecurrenceIntervalID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as RecurrenceInterval);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return RecurrenceIntervalID;
        }

        public static bool operator ==(RecurrenceInterval left, RecurrenceInterval right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RecurrenceInterval left, RecurrenceInterval right)
        {
            return !Equals(left, right);
        }

        public RecurrenceIntervalEnum ToEnum { get { return (RecurrenceIntervalEnum)GetHashCode(); } }

        public static RecurrenceInterval ToType(int enumValue)
        {
            return ToType((RecurrenceIntervalEnum)enumValue);
        }

        public static RecurrenceInterval ToType(RecurrenceIntervalEnum enumValue)
        {
            switch (enumValue)
            {
                case RecurrenceIntervalEnum.FifteenYears:
                    return FifteenYears;
                case RecurrenceIntervalEnum.FiveYears:
                    return FiveYears;
                case RecurrenceIntervalEnum.OneYear:
                    return OneYear;
                case RecurrenceIntervalEnum.TenYears:
                    return TenYears;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum RecurrenceIntervalEnum
    {
        OneYear = 1,
        FiveYears = 2,
        TenYears = 3,
        FifteenYears = 4
    }

    public partial class RecurrenceIntervalOneYear : RecurrenceInterval
    {
        private RecurrenceIntervalOneYear(int recurrenceIntervalID, int recurrenceIntervalInYears, string recurrenceIntervalDisplayName, string recurrenceIntervalName) : base(recurrenceIntervalID, recurrenceIntervalInYears, recurrenceIntervalDisplayName, recurrenceIntervalName) {}
        public static readonly RecurrenceIntervalOneYear Instance = new RecurrenceIntervalOneYear(1, 1, @"1 Year", @"OneYear");
    }

    public partial class RecurrenceIntervalFiveYears : RecurrenceInterval
    {
        private RecurrenceIntervalFiveYears(int recurrenceIntervalID, int recurrenceIntervalInYears, string recurrenceIntervalDisplayName, string recurrenceIntervalName) : base(recurrenceIntervalID, recurrenceIntervalInYears, recurrenceIntervalDisplayName, recurrenceIntervalName) {}
        public static readonly RecurrenceIntervalFiveYears Instance = new RecurrenceIntervalFiveYears(2, 5, @"5 Years", @"FiveYears");
    }

    public partial class RecurrenceIntervalTenYears : RecurrenceInterval
    {
        private RecurrenceIntervalTenYears(int recurrenceIntervalID, int recurrenceIntervalInYears, string recurrenceIntervalDisplayName, string recurrenceIntervalName) : base(recurrenceIntervalID, recurrenceIntervalInYears, recurrenceIntervalDisplayName, recurrenceIntervalName) {}
        public static readonly RecurrenceIntervalTenYears Instance = new RecurrenceIntervalTenYears(3, 10, @"10 Years", @"TenYears");
    }

    public partial class RecurrenceIntervalFifteenYears : RecurrenceInterval
    {
        private RecurrenceIntervalFifteenYears(int recurrenceIntervalID, int recurrenceIntervalInYears, string recurrenceIntervalDisplayName, string recurrenceIntervalName) : base(recurrenceIntervalID, recurrenceIntervalInYears, recurrenceIntervalDisplayName, recurrenceIntervalName) {}
        public static readonly RecurrenceIntervalFifteenYears Instance = new RecurrenceIntervalFifteenYears(4, 15, @"15 Years", @"FifteenYears");
    }
}