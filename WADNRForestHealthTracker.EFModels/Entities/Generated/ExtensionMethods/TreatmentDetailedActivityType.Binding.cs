//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentDetailedActivityType]
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
    public abstract partial class TreatmentDetailedActivityType : IHavePrimaryKey
    {
        public static readonly TreatmentDetailedActivityTypeChipping Chipping = TreatmentDetailedActivityTypeChipping.Instance;
        public static readonly TreatmentDetailedActivityTypePruning Pruning = TreatmentDetailedActivityTypePruning.Instance;
        public static readonly TreatmentDetailedActivityTypeThinning Thinning = TreatmentDetailedActivityTypeThinning.Instance;
        public static readonly TreatmentDetailedActivityTypeMastication Mastication = TreatmentDetailedActivityTypeMastication.Instance;
        public static readonly TreatmentDetailedActivityTypeGrazing Grazing = TreatmentDetailedActivityTypeGrazing.Instance;
        public static readonly TreatmentDetailedActivityTypeLopAndScatter LopAndScatter = TreatmentDetailedActivityTypeLopAndScatter.Instance;
        public static readonly TreatmentDetailedActivityTypeBiomassRemoval BiomassRemoval = TreatmentDetailedActivityTypeBiomassRemoval.Instance;
        public static readonly TreatmentDetailedActivityTypeHandPile HandPile = TreatmentDetailedActivityTypeHandPile.Instance;
        public static readonly TreatmentDetailedActivityTypeBroadcastBurn BroadcastBurn = TreatmentDetailedActivityTypeBroadcastBurn.Instance;
        public static readonly TreatmentDetailedActivityTypeHandPileBurn HandPileBurn = TreatmentDetailedActivityTypeHandPileBurn.Instance;
        public static readonly TreatmentDetailedActivityTypeMachinePileBurn MachinePileBurn = TreatmentDetailedActivityTypeMachinePileBurn.Instance;
        public static readonly TreatmentDetailedActivityTypeSlash Slash = TreatmentDetailedActivityTypeSlash.Instance;
        public static readonly TreatmentDetailedActivityTypeOther Other = TreatmentDetailedActivityTypeOther.Instance;
        public static readonly TreatmentDetailedActivityTypeJackpotBurn JackpotBurn = TreatmentDetailedActivityTypeJackpotBurn.Instance;
        public static readonly TreatmentDetailedActivityTypeMachinePile MachinePile = TreatmentDetailedActivityTypeMachinePile.Instance;
        public static readonly TreatmentDetailedActivityTypeFuelBreak FuelBreak = TreatmentDetailedActivityTypeFuelBreak.Instance;
        public static readonly TreatmentDetailedActivityTypePlanting Planting = TreatmentDetailedActivityTypePlanting.Instance;
        public static readonly TreatmentDetailedActivityTypeBrushControl BrushControl = TreatmentDetailedActivityTypeBrushControl.Instance;
        public static readonly TreatmentDetailedActivityTypeMowing Mowing = TreatmentDetailedActivityTypeMowing.Instance;
        public static readonly TreatmentDetailedActivityTypeRegen Regen = TreatmentDetailedActivityTypeRegen.Instance;
        public static readonly TreatmentDetailedActivityTypePileBurn PileBurn = TreatmentDetailedActivityTypePileBurn.Instance;

        public static readonly List<TreatmentDetailedActivityType> All;
        public static readonly ReadOnlyDictionary<int, TreatmentDetailedActivityType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static TreatmentDetailedActivityType()
        {
            All = new List<TreatmentDetailedActivityType> { Chipping, Pruning, Thinning, Mastication, Grazing, LopAndScatter, BiomassRemoval, HandPile, BroadcastBurn, HandPileBurn, MachinePileBurn, Slash, Other, JackpotBurn, MachinePile, FuelBreak, Planting, BrushControl, Mowing, Regen, PileBurn };
            AllLookupDictionary = new ReadOnlyDictionary<int, TreatmentDetailedActivityType>(All.ToDictionary(x => x.TreatmentDetailedActivityTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected TreatmentDetailedActivityType(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName)
        {
            TreatmentDetailedActivityTypeID = treatmentDetailedActivityTypeID;
            TreatmentDetailedActivityTypeName = treatmentDetailedActivityTypeName;
            TreatmentDetailedActivityTypeDisplayName = treatmentDetailedActivityTypeDisplayName;
        }

        [Key]
        public int TreatmentDetailedActivityTypeID { get; private set; }
        public string TreatmentDetailedActivityTypeName { get; private set; }
        public string TreatmentDetailedActivityTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return TreatmentDetailedActivityTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(TreatmentDetailedActivityType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.TreatmentDetailedActivityTypeID == TreatmentDetailedActivityTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as TreatmentDetailedActivityType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return TreatmentDetailedActivityTypeID;
        }

        public static bool operator ==(TreatmentDetailedActivityType left, TreatmentDetailedActivityType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TreatmentDetailedActivityType left, TreatmentDetailedActivityType right)
        {
            return !Equals(left, right);
        }

        public TreatmentDetailedActivityTypeEnum ToEnum { get { return (TreatmentDetailedActivityTypeEnum)GetHashCode(); } }

        public static TreatmentDetailedActivityType ToType(int enumValue)
        {
            return ToType((TreatmentDetailedActivityTypeEnum)enumValue);
        }

        public static TreatmentDetailedActivityType ToType(TreatmentDetailedActivityTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case TreatmentDetailedActivityTypeEnum.BiomassRemoval:
                    return BiomassRemoval;
                case TreatmentDetailedActivityTypeEnum.BroadcastBurn:
                    return BroadcastBurn;
                case TreatmentDetailedActivityTypeEnum.BrushControl:
                    return BrushControl;
                case TreatmentDetailedActivityTypeEnum.Chipping:
                    return Chipping;
                case TreatmentDetailedActivityTypeEnum.FuelBreak:
                    return FuelBreak;
                case TreatmentDetailedActivityTypeEnum.Grazing:
                    return Grazing;
                case TreatmentDetailedActivityTypeEnum.HandPile:
                    return HandPile;
                case TreatmentDetailedActivityTypeEnum.HandPileBurn:
                    return HandPileBurn;
                case TreatmentDetailedActivityTypeEnum.JackpotBurn:
                    return JackpotBurn;
                case TreatmentDetailedActivityTypeEnum.LopAndScatter:
                    return LopAndScatter;
                case TreatmentDetailedActivityTypeEnum.MachinePile:
                    return MachinePile;
                case TreatmentDetailedActivityTypeEnum.MachinePileBurn:
                    return MachinePileBurn;
                case TreatmentDetailedActivityTypeEnum.Mastication:
                    return Mastication;
                case TreatmentDetailedActivityTypeEnum.Mowing:
                    return Mowing;
                case TreatmentDetailedActivityTypeEnum.Other:
                    return Other;
                case TreatmentDetailedActivityTypeEnum.PileBurn:
                    return PileBurn;
                case TreatmentDetailedActivityTypeEnum.Planting:
                    return Planting;
                case TreatmentDetailedActivityTypeEnum.Pruning:
                    return Pruning;
                case TreatmentDetailedActivityTypeEnum.Regen:
                    return Regen;
                case TreatmentDetailedActivityTypeEnum.Slash:
                    return Slash;
                case TreatmentDetailedActivityTypeEnum.Thinning:
                    return Thinning;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum TreatmentDetailedActivityTypeEnum
    {
        Chipping = 1,
        Pruning = 2,
        Thinning = 3,
        Mastication = 4,
        Grazing = 5,
        LopAndScatter = 6,
        BiomassRemoval = 7,
        HandPile = 8,
        BroadcastBurn = 9,
        HandPileBurn = 10,
        MachinePileBurn = 11,
        Slash = 12,
        Other = 13,
        JackpotBurn = 14,
        MachinePile = 15,
        FuelBreak = 16,
        Planting = 17,
        BrushControl = 18,
        Mowing = 19,
        Regen = 20,
        PileBurn = 21
    }

    public partial class TreatmentDetailedActivityTypeChipping : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeChipping(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeChipping Instance = new TreatmentDetailedActivityTypeChipping(1, @"Chipping", @"Chipping");
    }

    public partial class TreatmentDetailedActivityTypePruning : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypePruning(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypePruning Instance = new TreatmentDetailedActivityTypePruning(2, @"Pruning", @"Pruning");
    }

    public partial class TreatmentDetailedActivityTypeThinning : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeThinning(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeThinning Instance = new TreatmentDetailedActivityTypeThinning(3, @"Thinning", @"Thinning");
    }

    public partial class TreatmentDetailedActivityTypeMastication : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeMastication(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeMastication Instance = new TreatmentDetailedActivityTypeMastication(4, @"Mastication", @"Mastication");
    }

    public partial class TreatmentDetailedActivityTypeGrazing : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeGrazing(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeGrazing Instance = new TreatmentDetailedActivityTypeGrazing(5, @"Grazing", @"Grazing");
    }

    public partial class TreatmentDetailedActivityTypeLopAndScatter : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeLopAndScatter(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeLopAndScatter Instance = new TreatmentDetailedActivityTypeLopAndScatter(6, @"LopAndScatter", @"Lop and Scatter");
    }

    public partial class TreatmentDetailedActivityTypeBiomassRemoval : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeBiomassRemoval(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeBiomassRemoval Instance = new TreatmentDetailedActivityTypeBiomassRemoval(7, @"BiomassRemoval", @"Biomass Removal");
    }

    public partial class TreatmentDetailedActivityTypeHandPile : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeHandPile(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeHandPile Instance = new TreatmentDetailedActivityTypeHandPile(8, @"HandPile", @"Hand Pile");
    }

    public partial class TreatmentDetailedActivityTypeBroadcastBurn : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeBroadcastBurn(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeBroadcastBurn Instance = new TreatmentDetailedActivityTypeBroadcastBurn(9, @"BroadcastBurn", @"Broadcast Burn");
    }

    public partial class TreatmentDetailedActivityTypeHandPileBurn : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeHandPileBurn(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeHandPileBurn Instance = new TreatmentDetailedActivityTypeHandPileBurn(10, @"HandPileBurn", @"Hand Pile Burn");
    }

    public partial class TreatmentDetailedActivityTypeMachinePileBurn : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeMachinePileBurn(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeMachinePileBurn Instance = new TreatmentDetailedActivityTypeMachinePileBurn(11, @"MachinePileBurn", @"Machine Pile Burn");
    }

    public partial class TreatmentDetailedActivityTypeSlash : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeSlash(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeSlash Instance = new TreatmentDetailedActivityTypeSlash(12, @"Slash", @"Slash");
    }

    public partial class TreatmentDetailedActivityTypeOther : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeOther(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeOther Instance = new TreatmentDetailedActivityTypeOther(13, @"Other", @"Other");
    }

    public partial class TreatmentDetailedActivityTypeJackpotBurn : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeJackpotBurn(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeJackpotBurn Instance = new TreatmentDetailedActivityTypeJackpotBurn(14, @"JackpotBurn", @"Jackpot Burn");
    }

    public partial class TreatmentDetailedActivityTypeMachinePile : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeMachinePile(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeMachinePile Instance = new TreatmentDetailedActivityTypeMachinePile(15, @"MachinePile", @"Machine Pile");
    }

    public partial class TreatmentDetailedActivityTypeFuelBreak : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeFuelBreak(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeFuelBreak Instance = new TreatmentDetailedActivityTypeFuelBreak(16, @"FuelBreak", @"Fuel Break");
    }

    public partial class TreatmentDetailedActivityTypePlanting : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypePlanting(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypePlanting Instance = new TreatmentDetailedActivityTypePlanting(17, @"Planting", @"Planting");
    }

    public partial class TreatmentDetailedActivityTypeBrushControl : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeBrushControl(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeBrushControl Instance = new TreatmentDetailedActivityTypeBrushControl(18, @"BrushControl", @"Brush Control");
    }

    public partial class TreatmentDetailedActivityTypeMowing : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeMowing(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeMowing Instance = new TreatmentDetailedActivityTypeMowing(19, @"Mowing", @"Mowing");
    }

    public partial class TreatmentDetailedActivityTypeRegen : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypeRegen(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypeRegen Instance = new TreatmentDetailedActivityTypeRegen(20, @"Regen", @"Regen");
    }

    public partial class TreatmentDetailedActivityTypePileBurn : TreatmentDetailedActivityType
    {
        private TreatmentDetailedActivityTypePileBurn(int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeName, string treatmentDetailedActivityTypeDisplayName) : base(treatmentDetailedActivityTypeID, treatmentDetailedActivityTypeName, treatmentDetailedActivityTypeDisplayName) {}
        public static readonly TreatmentDetailedActivityTypePileBurn Instance = new TreatmentDetailedActivityTypePileBurn(21, @"PileBurn", @"Pile Burn");
    }
}