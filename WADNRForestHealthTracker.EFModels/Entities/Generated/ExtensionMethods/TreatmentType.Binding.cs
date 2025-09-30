//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentType]
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
    public abstract partial class TreatmentType : IHavePrimaryKey
    {
        public static readonly TreatmentTypeCommercial Commercial = TreatmentTypeCommercial.Instance;
        public static readonly TreatmentTypePrescribedFire PrescribedFire = TreatmentTypePrescribedFire.Instance;
        public static readonly TreatmentTypeNonCommercial NonCommercial = TreatmentTypeNonCommercial.Instance;
        public static readonly TreatmentTypeOther Other = TreatmentTypeOther.Instance;

        public static readonly List<TreatmentType> All;
        public static readonly ReadOnlyDictionary<int, TreatmentType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static TreatmentType()
        {
            All = new List<TreatmentType> { Commercial, PrescribedFire, NonCommercial, Other };
            AllLookupDictionary = new ReadOnlyDictionary<int, TreatmentType>(All.ToDictionary(x => x.TreatmentTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected TreatmentType(int treatmentTypeID, string treatmentTypeName, string treatmentTypeDisplayName)
        {
            TreatmentTypeID = treatmentTypeID;
            TreatmentTypeName = treatmentTypeName;
            TreatmentTypeDisplayName = treatmentTypeDisplayName;
        }

        [Key]
        public int TreatmentTypeID { get; private set; }
        public string TreatmentTypeName { get; private set; }
        public string TreatmentTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return TreatmentTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(TreatmentType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.TreatmentTypeID == TreatmentTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as TreatmentType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return TreatmentTypeID;
        }

        public static bool operator ==(TreatmentType left, TreatmentType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TreatmentType left, TreatmentType right)
        {
            return !Equals(left, right);
        }

        public TreatmentTypeEnum ToEnum { get { return (TreatmentTypeEnum)GetHashCode(); } }

        public static TreatmentType ToType(int enumValue)
        {
            return ToType((TreatmentTypeEnum)enumValue);
        }

        public static TreatmentType ToType(TreatmentTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case TreatmentTypeEnum.Commercial:
                    return Commercial;
                case TreatmentTypeEnum.NonCommercial:
                    return NonCommercial;
                case TreatmentTypeEnum.Other:
                    return Other;
                case TreatmentTypeEnum.PrescribedFire:
                    return PrescribedFire;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum TreatmentTypeEnum
    {
        Commercial = 1,
        PrescribedFire = 2,
        NonCommercial = 3,
        Other = 4
    }

    public partial class TreatmentTypeCommercial : TreatmentType
    {
        private TreatmentTypeCommercial(int treatmentTypeID, string treatmentTypeName, string treatmentTypeDisplayName) : base(treatmentTypeID, treatmentTypeName, treatmentTypeDisplayName) {}
        public static readonly TreatmentTypeCommercial Instance = new TreatmentTypeCommercial(1, @"Commercial", @"Commercial");
    }

    public partial class TreatmentTypePrescribedFire : TreatmentType
    {
        private TreatmentTypePrescribedFire(int treatmentTypeID, string treatmentTypeName, string treatmentTypeDisplayName) : base(treatmentTypeID, treatmentTypeName, treatmentTypeDisplayName) {}
        public static readonly TreatmentTypePrescribedFire Instance = new TreatmentTypePrescribedFire(2, @"PrescribedFire", @"Prescribed Fire");
    }

    public partial class TreatmentTypeNonCommercial : TreatmentType
    {
        private TreatmentTypeNonCommercial(int treatmentTypeID, string treatmentTypeName, string treatmentTypeDisplayName) : base(treatmentTypeID, treatmentTypeName, treatmentTypeDisplayName) {}
        public static readonly TreatmentTypeNonCommercial Instance = new TreatmentTypeNonCommercial(3, @"NonCommercial", @"Non-Commercial");
    }

    public partial class TreatmentTypeOther : TreatmentType
    {
        private TreatmentTypeOther(int treatmentTypeID, string treatmentTypeName, string treatmentTypeDisplayName) : base(treatmentTypeID, treatmentTypeName, treatmentTypeDisplayName) {}
        public static readonly TreatmentTypeOther Instance = new TreatmentTypeOther(4, @"Other", @"Other");
    }
}