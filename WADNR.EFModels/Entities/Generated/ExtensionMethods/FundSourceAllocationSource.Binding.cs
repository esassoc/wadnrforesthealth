//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationSource]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WADNR.EFModels.Entities
{
    public abstract partial class FundSourceAllocationSource : IHavePrimaryKey
    {
        public static readonly FundSourceAllocationSourceState State = FundSourceAllocationSourceState.Instance;
        public static readonly FundSourceAllocationSourceStateGFS StateGFS = FundSourceAllocationSourceStateGFS.Instance;
        public static readonly FundSourceAllocationSourceStateCapital StateCapital = FundSourceAllocationSourceStateCapital.Instance;
        public static readonly FundSourceAllocationSourceStateOther StateOther = FundSourceAllocationSourceStateOther.Instance;
        public static readonly FundSourceAllocationSourceFederalWSFM FederalWSFM = FundSourceAllocationSourceFederalWSFM.Instance;
        public static readonly FundSourceAllocationSourceFederaNFPWUINonFedWUI FederaNFPWUINonFedWUI = FundSourceAllocationSourceFederaNFPWUINonFedWUI.Instance;
        public static readonly FundSourceAllocationSourceFederalCWDG FederalCWDG = FundSourceAllocationSourceFederalCWDG.Instance;
        public static readonly FundSourceAllocationSourceFederalLSR FederalLSR = FundSourceAllocationSourceFederalLSR.Instance;
        public static readonly FundSourceAllocationSourceFederalBipartisanInfrastructureLaw FederalBipartisanInfrastructureLaw = FundSourceAllocationSourceFederalBipartisanInfrastructureLaw.Instance;
        public static readonly FundSourceAllocationSourceFederalInflationReductionAct FederalInflationReductionAct = FundSourceAllocationSourceFederalInflationReductionAct.Instance;
        public static readonly FundSourceAllocationSourceFederalConsolidatedPaymentFundSource FederalConsolidatedPaymentFundSource = FundSourceAllocationSourceFederalConsolidatedPaymentFundSource.Instance;
        public static readonly FundSourceAllocationSourceFederalCooperativeAgreements FederalCooperativeAgreements = FundSourceAllocationSourceFederalCooperativeAgreements.Instance;
        public static readonly FundSourceAllocationSourceFederalDisasterRelief FederalDisasterRelief = FundSourceAllocationSourceFederalDisasterRelief.Instance;
        public static readonly FundSourceAllocationSourceFederalForestHealthProtection FederalForestHealthProtection = FundSourceAllocationSourceFederalForestHealthProtection.Instance;
        public static readonly FundSourceAllocationSourceFederalForestLegacy FederalForestLegacy = FundSourceAllocationSourceFederalForestLegacy.Instance;
        public static readonly FundSourceAllocationSourceFederalWesternBarkBeetle FederalWesternBarkBeetle = FundSourceAllocationSourceFederalWesternBarkBeetle.Instance;
        public static readonly FundSourceAllocationSourceFederalFEMA FederalFEMA = FundSourceAllocationSourceFederalFEMA.Instance;
        public static readonly FundSourceAllocationSourceFederalBLM FederalBLM = FundSourceAllocationSourceFederalBLM.Instance;
        public static readonly FundSourceAllocationSourceFederalOther FederalOther = FundSourceAllocationSourceFederalOther.Instance;
        public static readonly FundSourceAllocationSourcePrivate Private = FundSourceAllocationSourcePrivate.Instance;
        public static readonly FundSourceAllocationSourceOther Other = FundSourceAllocationSourceOther.Instance;

        public static readonly List<FundSourceAllocationSource> All;
        public static readonly ReadOnlyDictionary<int, FundSourceAllocationSource> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static FundSourceAllocationSource()
        {
            All = new List<FundSourceAllocationSource> { State, StateGFS, StateCapital, StateOther, FederalWSFM, FederaNFPWUINonFedWUI, FederalCWDG, FederalLSR, FederalBipartisanInfrastructureLaw, FederalInflationReductionAct, FederalConsolidatedPaymentFundSource, FederalCooperativeAgreements, FederalDisasterRelief, FederalForestHealthProtection, FederalForestLegacy, FederalWesternBarkBeetle, FederalFEMA, FederalBLM, FederalOther, Private, Other };
            AllLookupDictionary = new ReadOnlyDictionary<int, FundSourceAllocationSource>(All.ToDictionary(x => x.FundSourceAllocationSourceID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected FundSourceAllocationSource(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder)
        {
            FundSourceAllocationSourceID = fundSourceAllocationSourceID;
            FundSourceAllocationSourceName = fundSourceAllocationSourceName;
            FundSourceAllocationSourceDisplayName = fundSourceAllocationSourceDisplayName;
            SortOrder = sortOrder;
        }

        [Key]
        public int FundSourceAllocationSourceID { get; private set; }
        public string FundSourceAllocationSourceName { get; private set; }
        public string FundSourceAllocationSourceDisplayName { get; private set; }
        public int SortOrder { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationSourceID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(FundSourceAllocationSource other)
        {
            if (other == null)
            {
                return false;
            }
            return other.FundSourceAllocationSourceID == FundSourceAllocationSourceID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as FundSourceAllocationSource);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return FundSourceAllocationSourceID;
        }

        public static bool operator ==(FundSourceAllocationSource left, FundSourceAllocationSource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FundSourceAllocationSource left, FundSourceAllocationSource right)
        {
            return !Equals(left, right);
        }

        public FundSourceAllocationSourceEnum ToEnum => (FundSourceAllocationSourceEnum)GetHashCode();

        public static FundSourceAllocationSource ToType(int enumValue)
        {
            return ToType((FundSourceAllocationSourceEnum)enumValue);
        }

        public static FundSourceAllocationSource ToType(FundSourceAllocationSourceEnum enumValue)
        {
            switch (enumValue)
            {
                case FundSourceAllocationSourceEnum.FederalBipartisanInfrastructureLaw:
                    return FederalBipartisanInfrastructureLaw;
                case FundSourceAllocationSourceEnum.FederalBLM:
                    return FederalBLM;
                case FundSourceAllocationSourceEnum.FederalConsolidatedPaymentFundSource:
                    return FederalConsolidatedPaymentFundSource;
                case FundSourceAllocationSourceEnum.FederalCooperativeAgreements:
                    return FederalCooperativeAgreements;
                case FundSourceAllocationSourceEnum.FederalCWDG:
                    return FederalCWDG;
                case FundSourceAllocationSourceEnum.FederalDisasterRelief:
                    return FederalDisasterRelief;
                case FundSourceAllocationSourceEnum.FederalFEMA:
                    return FederalFEMA;
                case FundSourceAllocationSourceEnum.FederalForestHealthProtection:
                    return FederalForestHealthProtection;
                case FundSourceAllocationSourceEnum.FederalForestLegacy:
                    return FederalForestLegacy;
                case FundSourceAllocationSourceEnum.FederalInflationReductionAct:
                    return FederalInflationReductionAct;
                case FundSourceAllocationSourceEnum.FederalLSR:
                    return FederalLSR;
                case FundSourceAllocationSourceEnum.FederalOther:
                    return FederalOther;
                case FundSourceAllocationSourceEnum.FederalWesternBarkBeetle:
                    return FederalWesternBarkBeetle;
                case FundSourceAllocationSourceEnum.FederalWSFM:
                    return FederalWSFM;
                case FundSourceAllocationSourceEnum.FederaNFPWUINonFedWUI:
                    return FederaNFPWUINonFedWUI;
                case FundSourceAllocationSourceEnum.Other:
                    return Other;
                case FundSourceAllocationSourceEnum.Private:
                    return Private;
                case FundSourceAllocationSourceEnum.State:
                    return State;
                case FundSourceAllocationSourceEnum.StateCapital:
                    return StateCapital;
                case FundSourceAllocationSourceEnum.StateGFS:
                    return StateGFS;
                case FundSourceAllocationSourceEnum.StateOther:
                    return StateOther;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum FundSourceAllocationSourceEnum
    {
        State = 1,
        StateGFS = 2,
        StateCapital = 3,
        StateOther = 4,
        FederalWSFM = 5,
        FederaNFPWUINonFedWUI = 6,
        FederalCWDG = 7,
        FederalLSR = 8,
        FederalBipartisanInfrastructureLaw = 9,
        FederalInflationReductionAct = 10,
        FederalConsolidatedPaymentFundSource = 11,
        FederalCooperativeAgreements = 12,
        FederalDisasterRelief = 13,
        FederalForestHealthProtection = 14,
        FederalForestLegacy = 15,
        FederalWesternBarkBeetle = 16,
        FederalFEMA = 17,
        FederalBLM = 18,
        FederalOther = 19,
        Private = 20,
        Other = 21
    }

    public partial class FundSourceAllocationSourceState : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceState(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceState Instance = new FundSourceAllocationSourceState(1, @"State", @"State", 10);
    }

    public partial class FundSourceAllocationSourceStateGFS : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceStateGFS(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceStateGFS Instance = new FundSourceAllocationSourceStateGFS(2, @"StateGFS", @"State - GFS", 20);
    }

    public partial class FundSourceAllocationSourceStateCapital : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceStateCapital(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceStateCapital Instance = new FundSourceAllocationSourceStateCapital(3, @"StateCapital", @"State - Capital", 30);
    }

    public partial class FundSourceAllocationSourceStateOther : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceStateOther(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceStateOther Instance = new FundSourceAllocationSourceStateOther(4, @"StateOther", @"State - Other", 40);
    }

    public partial class FundSourceAllocationSourceFederalWSFM : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalWSFM(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalWSFM Instance = new FundSourceAllocationSourceFederalWSFM(5, @"FederalWSFM", @"Federal - WSFM", 50);
    }

    public partial class FundSourceAllocationSourceFederaNFPWUINonFedWUI : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederaNFPWUINonFedWUI(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederaNFPWUINonFedWUI Instance = new FundSourceAllocationSourceFederaNFPWUINonFedWUI(6, @"FederaNFPWUINonFedWUI", @"Federal - NFP WUI (Non-fed WUI)", 60);
    }

    public partial class FundSourceAllocationSourceFederalCWDG : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalCWDG(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalCWDG Instance = new FundSourceAllocationSourceFederalCWDG(7, @"FederalCWDG", @"Federal - CWDG", 70);
    }

    public partial class FundSourceAllocationSourceFederalLSR : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalLSR(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalLSR Instance = new FundSourceAllocationSourceFederalLSR(8, @"FederalLSR", @"Federal - LSR", 80);
    }

    public partial class FundSourceAllocationSourceFederalBipartisanInfrastructureLaw : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalBipartisanInfrastructureLaw(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalBipartisanInfrastructureLaw Instance = new FundSourceAllocationSourceFederalBipartisanInfrastructureLaw(9, @"FederalBipartisanInfrastructureLaw", @"Federal - Bipartisan Infrastructure Law", 90);
    }

    public partial class FundSourceAllocationSourceFederalInflationReductionAct : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalInflationReductionAct(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalInflationReductionAct Instance = new FundSourceAllocationSourceFederalInflationReductionAct(10, @"FederalInflationReductionAct", @"Federal - Inflation Reduction Act", 100);
    }

    public partial class FundSourceAllocationSourceFederalConsolidatedPaymentFundSource : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalConsolidatedPaymentFundSource(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalConsolidatedPaymentFundSource Instance = new FundSourceAllocationSourceFederalConsolidatedPaymentFundSource(11, @"FederalConsolidatedPaymentFundSource", @"Federal - Consolidated Payment FundSource", 110);
    }

    public partial class FundSourceAllocationSourceFederalCooperativeAgreements : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalCooperativeAgreements(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalCooperativeAgreements Instance = new FundSourceAllocationSourceFederalCooperativeAgreements(12, @"FederalCooperativeAgreements", @"Federal - Cooperative Agreements", 120);
    }

    public partial class FundSourceAllocationSourceFederalDisasterRelief : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalDisasterRelief(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalDisasterRelief Instance = new FundSourceAllocationSourceFederalDisasterRelief(13, @"FederalDisasterRelief", @"Federal - Disaster Relief", 130);
    }

    public partial class FundSourceAllocationSourceFederalForestHealthProtection : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalForestHealthProtection(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalForestHealthProtection Instance = new FundSourceAllocationSourceFederalForestHealthProtection(14, @"FederalForestHealthProtection", @"Federal - Forest Health Protection", 140);
    }

    public partial class FundSourceAllocationSourceFederalForestLegacy : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalForestLegacy(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalForestLegacy Instance = new FundSourceAllocationSourceFederalForestLegacy(15, @"FederalForestLegacy", @"Federal - Forest Legacy", 150);
    }

    public partial class FundSourceAllocationSourceFederalWesternBarkBeetle : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalWesternBarkBeetle(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalWesternBarkBeetle Instance = new FundSourceAllocationSourceFederalWesternBarkBeetle(16, @"FederalWesternBarkBeetle", @"Federal - Western Bark Beetle", 160);
    }

    public partial class FundSourceAllocationSourceFederalFEMA : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalFEMA(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalFEMA Instance = new FundSourceAllocationSourceFederalFEMA(17, @"FederalFEMA", @"Federal - FEMA", 170);
    }

    public partial class FundSourceAllocationSourceFederalBLM : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalBLM(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalBLM Instance = new FundSourceAllocationSourceFederalBLM(18, @"FederalBLM", @"Federal - BLM", 180);
    }

    public partial class FundSourceAllocationSourceFederalOther : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceFederalOther(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceFederalOther Instance = new FundSourceAllocationSourceFederalOther(19, @"FederalOther", @"Federal - Other", 190);
    }

    public partial class FundSourceAllocationSourcePrivate : FundSourceAllocationSource
    {
        private FundSourceAllocationSourcePrivate(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourcePrivate Instance = new FundSourceAllocationSourcePrivate(20, @"Private", @"Private", 200);
    }

    public partial class FundSourceAllocationSourceOther : FundSourceAllocationSource
    {
        private FundSourceAllocationSourceOther(int fundSourceAllocationSourceID, string fundSourceAllocationSourceName, string fundSourceAllocationSourceDisplayName, int sortOrder) : base(fundSourceAllocationSourceID, fundSourceAllocationSourceName, fundSourceAllocationSourceDisplayName, sortOrder) {}
        public static readonly FundSourceAllocationSourceOther Instance = new FundSourceAllocationSourceOther(21, @"Other", @"Other", 210);
    }
}