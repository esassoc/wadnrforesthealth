//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TabularDataImportTableType]
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
    public abstract partial class TabularDataImportTableType : IHavePrimaryKey
    {
        public static readonly TabularDataImportTableTypeLoaNortheast LoaNortheast = TabularDataImportTableTypeLoaNortheast.Instance;
        public static readonly TabularDataImportTableTypeLoaSoutheast LoaSoutheast = TabularDataImportTableTypeLoaSoutheast.Instance;

        public static readonly List<TabularDataImportTableType> All;
        public static readonly ReadOnlyDictionary<int, TabularDataImportTableType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static TabularDataImportTableType()
        {
            All = new List<TabularDataImportTableType> { LoaNortheast, LoaSoutheast };
            AllLookupDictionary = new ReadOnlyDictionary<int, TabularDataImportTableType>(All.ToDictionary(x => x.TabularDataImportTableTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected TabularDataImportTableType(int tabularDataImportTableTypeID, string tabularDataImportTableTypeName)
        {
            TabularDataImportTableTypeID = tabularDataImportTableTypeID;
            TabularDataImportTableTypeName = tabularDataImportTableTypeName;
        }

        [Key]
        public int TabularDataImportTableTypeID { get; private set; }
        public string TabularDataImportTableTypeName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return TabularDataImportTableTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(TabularDataImportTableType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.TabularDataImportTableTypeID == TabularDataImportTableTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as TabularDataImportTableType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return TabularDataImportTableTypeID;
        }

        public static bool operator ==(TabularDataImportTableType left, TabularDataImportTableType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TabularDataImportTableType left, TabularDataImportTableType right)
        {
            return !Equals(left, right);
        }

        public TabularDataImportTableTypeEnum ToEnum { get { return (TabularDataImportTableTypeEnum)GetHashCode(); } }

        public static TabularDataImportTableType ToType(int enumValue)
        {
            return ToType((TabularDataImportTableTypeEnum)enumValue);
        }

        public static TabularDataImportTableType ToType(TabularDataImportTableTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case TabularDataImportTableTypeEnum.LoaNortheast:
                    return LoaNortheast;
                case TabularDataImportTableTypeEnum.LoaSoutheast:
                    return LoaSoutheast;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum TabularDataImportTableTypeEnum
    {
        LoaNortheast = 1,
        LoaSoutheast = 2
    }

    public partial class TabularDataImportTableTypeLoaNortheast : TabularDataImportTableType
    {
        private TabularDataImportTableTypeLoaNortheast(int tabularDataImportTableTypeID, string tabularDataImportTableTypeName) : base(tabularDataImportTableTypeID, tabularDataImportTableTypeName) {}
        public static readonly TabularDataImportTableTypeLoaNortheast Instance = new TabularDataImportTableTypeLoaNortheast(1, @"LoaNortheast");
    }

    public partial class TabularDataImportTableTypeLoaSoutheast : TabularDataImportTableType
    {
        private TabularDataImportTableTypeLoaSoutheast(int tabularDataImportTableTypeID, string tabularDataImportTableTypeName) : base(tabularDataImportTableTypeID, tabularDataImportTableTypeName) {}
        public static readonly TabularDataImportTableTypeLoaSoutheast Instance = new TabularDataImportTableTypeLoaSoutheast(2, @"LoaSoutheast");
    }
}