//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ArcOnlineFinanceApiRawJsonImportTableType]
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
    public abstract partial class ArcOnlineFinanceApiRawJsonImportTableType : IHavePrimaryKey
    {
        public static readonly ArcOnlineFinanceApiRawJsonImportTableTypeVendor Vendor = ArcOnlineFinanceApiRawJsonImportTableTypeVendor.Instance;
        public static readonly ArcOnlineFinanceApiRawJsonImportTableTypeProgramIndex ProgramIndex = ArcOnlineFinanceApiRawJsonImportTableTypeProgramIndex.Instance;
        public static readonly ArcOnlineFinanceApiRawJsonImportTableTypeProjectCode ProjectCode = ArcOnlineFinanceApiRawJsonImportTableTypeProjectCode.Instance;
        public static readonly ArcOnlineFinanceApiRawJsonImportTableTypeFundSourceExpenditure FundSourceExpenditure = ArcOnlineFinanceApiRawJsonImportTableTypeFundSourceExpenditure.Instance;

        public static readonly List<ArcOnlineFinanceApiRawJsonImportTableType> All;
        public static readonly ReadOnlyDictionary<int, ArcOnlineFinanceApiRawJsonImportTableType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ArcOnlineFinanceApiRawJsonImportTableType()
        {
            All = new List<ArcOnlineFinanceApiRawJsonImportTableType> { Vendor, ProgramIndex, ProjectCode, FundSourceExpenditure };
            AllLookupDictionary = new ReadOnlyDictionary<int, ArcOnlineFinanceApiRawJsonImportTableType>(All.ToDictionary(x => x.ArcOnlineFinanceApiRawJsonImportTableTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ArcOnlineFinanceApiRawJsonImportTableType(int arcOnlineFinanceApiRawJsonImportTableTypeID, string arcOnlineFinanceApiRawJsonImportTableTypeName)
        {
            ArcOnlineFinanceApiRawJsonImportTableTypeID = arcOnlineFinanceApiRawJsonImportTableTypeID;
            ArcOnlineFinanceApiRawJsonImportTableTypeName = arcOnlineFinanceApiRawJsonImportTableTypeName;
        }

        [Key]
        public int ArcOnlineFinanceApiRawJsonImportTableTypeID { get; private set; }
        public string ArcOnlineFinanceApiRawJsonImportTableTypeName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ArcOnlineFinanceApiRawJsonImportTableTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ArcOnlineFinanceApiRawJsonImportTableType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ArcOnlineFinanceApiRawJsonImportTableTypeID == ArcOnlineFinanceApiRawJsonImportTableTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ArcOnlineFinanceApiRawJsonImportTableType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ArcOnlineFinanceApiRawJsonImportTableTypeID;
        }

        public static bool operator ==(ArcOnlineFinanceApiRawJsonImportTableType left, ArcOnlineFinanceApiRawJsonImportTableType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ArcOnlineFinanceApiRawJsonImportTableType left, ArcOnlineFinanceApiRawJsonImportTableType right)
        {
            return !Equals(left, right);
        }

        public ArcOnlineFinanceApiRawJsonImportTableTypeEnum ToEnum { get { return (ArcOnlineFinanceApiRawJsonImportTableTypeEnum)GetHashCode(); } }

        public static ArcOnlineFinanceApiRawJsonImportTableType ToType(int enumValue)
        {
            return ToType((ArcOnlineFinanceApiRawJsonImportTableTypeEnum)enumValue);
        }

        public static ArcOnlineFinanceApiRawJsonImportTableType ToType(ArcOnlineFinanceApiRawJsonImportTableTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ArcOnlineFinanceApiRawJsonImportTableTypeEnum.FundSourceExpenditure:
                    return FundSourceExpenditure;
                case ArcOnlineFinanceApiRawJsonImportTableTypeEnum.ProgramIndex:
                    return ProgramIndex;
                case ArcOnlineFinanceApiRawJsonImportTableTypeEnum.ProjectCode:
                    return ProjectCode;
                case ArcOnlineFinanceApiRawJsonImportTableTypeEnum.Vendor:
                    return Vendor;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum ArcOnlineFinanceApiRawJsonImportTableTypeEnum
    {
        Vendor = 1,
        ProgramIndex = 2,
        ProjectCode = 3,
        FundSourceExpenditure = 4
    }

    public partial class ArcOnlineFinanceApiRawJsonImportTableTypeVendor : ArcOnlineFinanceApiRawJsonImportTableType
    {
        private ArcOnlineFinanceApiRawJsonImportTableTypeVendor(int arcOnlineFinanceApiRawJsonImportTableTypeID, string arcOnlineFinanceApiRawJsonImportTableTypeName) : base(arcOnlineFinanceApiRawJsonImportTableTypeID, arcOnlineFinanceApiRawJsonImportTableTypeName) {}
        public static readonly ArcOnlineFinanceApiRawJsonImportTableTypeVendor Instance = new ArcOnlineFinanceApiRawJsonImportTableTypeVendor(1, @"Vendor");
    }

    public partial class ArcOnlineFinanceApiRawJsonImportTableTypeProgramIndex : ArcOnlineFinanceApiRawJsonImportTableType
    {
        private ArcOnlineFinanceApiRawJsonImportTableTypeProgramIndex(int arcOnlineFinanceApiRawJsonImportTableTypeID, string arcOnlineFinanceApiRawJsonImportTableTypeName) : base(arcOnlineFinanceApiRawJsonImportTableTypeID, arcOnlineFinanceApiRawJsonImportTableTypeName) {}
        public static readonly ArcOnlineFinanceApiRawJsonImportTableTypeProgramIndex Instance = new ArcOnlineFinanceApiRawJsonImportTableTypeProgramIndex(2, @"ProgramIndex");
    }

    public partial class ArcOnlineFinanceApiRawJsonImportTableTypeProjectCode : ArcOnlineFinanceApiRawJsonImportTableType
    {
        private ArcOnlineFinanceApiRawJsonImportTableTypeProjectCode(int arcOnlineFinanceApiRawJsonImportTableTypeID, string arcOnlineFinanceApiRawJsonImportTableTypeName) : base(arcOnlineFinanceApiRawJsonImportTableTypeID, arcOnlineFinanceApiRawJsonImportTableTypeName) {}
        public static readonly ArcOnlineFinanceApiRawJsonImportTableTypeProjectCode Instance = new ArcOnlineFinanceApiRawJsonImportTableTypeProjectCode(3, @"ProjectCode");
    }

    public partial class ArcOnlineFinanceApiRawJsonImportTableTypeFundSourceExpenditure : ArcOnlineFinanceApiRawJsonImportTableType
    {
        private ArcOnlineFinanceApiRawJsonImportTableTypeFundSourceExpenditure(int arcOnlineFinanceApiRawJsonImportTableTypeID, string arcOnlineFinanceApiRawJsonImportTableTypeName) : base(arcOnlineFinanceApiRawJsonImportTableTypeID, arcOnlineFinanceApiRawJsonImportTableTypeName) {}
        public static readonly ArcOnlineFinanceApiRawJsonImportTableTypeFundSourceExpenditure Instance = new ArcOnlineFinanceApiRawJsonImportTableTypeFundSourceExpenditure(4, @"FundSourceExpenditure");
    }
}