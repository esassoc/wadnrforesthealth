//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentCode]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WADNRForestHealthTracker.EFModels.Entities
{
    public abstract partial class TreatmentCode : IHavePrimaryKey
    {
        public static readonly TreatmentCodeBR1 BR1 = TreatmentCodeBR1.Instance;
        public static readonly TreatmentCodeBR2 BR2 = TreatmentCodeBR2.Instance;
        public static readonly TreatmentCodePL1New PL1New = TreatmentCodePL1New.Instance;
        public static readonly TreatmentCodePL1Revised PL1Revised = TreatmentCodePL1Revised.Instance;
        public static readonly TreatmentCodePL2New PL2New = TreatmentCodePL2New.Instance;
        public static readonly TreatmentCodePL2Revised PL2Revised = TreatmentCodePL2Revised.Instance;
        public static readonly TreatmentCodePL3New PL3New = TreatmentCodePL3New.Instance;
        public static readonly TreatmentCodePL3Revised PL3Revised = TreatmentCodePL3Revised.Instance;
        public static readonly TreatmentCodePL4New PL4New = TreatmentCodePL4New.Instance;
        public static readonly TreatmentCodePL4Revised PL4Revised = TreatmentCodePL4Revised.Instance;
        public static readonly TreatmentCodePL5New PL5New = TreatmentCodePL5New.Instance;
        public static readonly TreatmentCodePL5Revised PL5Revised = TreatmentCodePL5Revised.Instance;
        public static readonly TreatmentCodePR1 PR1 = TreatmentCodePR1.Instance;
        public static readonly TreatmentCodePR2 PR2 = TreatmentCodePR2.Instance;
        public static readonly TreatmentCodeRX1 RX1 = TreatmentCodeRX1.Instance;
        public static readonly TreatmentCodeSL1 SL1 = TreatmentCodeSL1.Instance;
        public static readonly TreatmentCodeSL2 SL2 = TreatmentCodeSL2.Instance;
        public static readonly TreatmentCodeSL3 SL3 = TreatmentCodeSL3.Instance;
        public static readonly TreatmentCodeSL4 SL4 = TreatmentCodeSL4.Instance;
        public static readonly TreatmentCodeTH1 TH1 = TreatmentCodeTH1.Instance;
        public static readonly TreatmentCodeTH2 TH2 = TreatmentCodeTH2.Instance;
        public static readonly TreatmentCodeTH3 TH3 = TreatmentCodeTH3.Instance;
        public static readonly TreatmentCodeTH4 TH4 = TreatmentCodeTH4.Instance;

        public static readonly List<TreatmentCode> All;
        public static readonly ReadOnlyDictionary<int, TreatmentCode> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static TreatmentCode()
        {
            All = new List<TreatmentCode> { BR1, BR2, PL1New, PL1Revised, PL2New, PL2Revised, PL3New, PL3Revised, PL4New, PL4Revised, PL5New, PL5Revised, PR1, PR2, RX1, SL1, SL2, SL3, SL4, TH1, TH2, TH3, TH4 };
            AllLookupDictionary = new ReadOnlyDictionary<int, TreatmentCode>(All.ToDictionary(x => x.TreatmentCodeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected TreatmentCode(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName)
        {
            TreatmentCodeID = treatmentCodeID;
            TreatmentCodeName = treatmentCodeName;
            TreatmentCodeDisplayName = treatmentCodeDisplayName;
        }

        [Key]
        public int TreatmentCodeID { get; private set; }
        public string TreatmentCodeName { get; private set; }
        public string TreatmentCodeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return TreatmentCodeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(TreatmentCode other)
        {
            if (other == null)
            {
                return false;
            }
            return other.TreatmentCodeID == TreatmentCodeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as TreatmentCode);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return TreatmentCodeID;
        }

        public static bool operator ==(TreatmentCode left, TreatmentCode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TreatmentCode left, TreatmentCode right)
        {
            return !Equals(left, right);
        }

        public TreatmentCodeEnum ToEnum => (TreatmentCodeEnum)GetHashCode();

        public static TreatmentCode ToType(int enumValue)
        {
            return ToType((TreatmentCodeEnum)enumValue);
        }

        public static TreatmentCode ToType(TreatmentCodeEnum enumValue)
        {
            switch (enumValue)
            {
                case TreatmentCodeEnum.BR1:
                    return BR1;
                case TreatmentCodeEnum.BR2:
                    return BR2;
                case TreatmentCodeEnum.PL1New:
                    return PL1New;
                case TreatmentCodeEnum.PL1Revised:
                    return PL1Revised;
                case TreatmentCodeEnum.PL2New:
                    return PL2New;
                case TreatmentCodeEnum.PL2Revised:
                    return PL2Revised;
                case TreatmentCodeEnum.PL3New:
                    return PL3New;
                case TreatmentCodeEnum.PL3Revised:
                    return PL3Revised;
                case TreatmentCodeEnum.PL4New:
                    return PL4New;
                case TreatmentCodeEnum.PL4Revised:
                    return PL4Revised;
                case TreatmentCodeEnum.PL5New:
                    return PL5New;
                case TreatmentCodeEnum.PL5Revised:
                    return PL5Revised;
                case TreatmentCodeEnum.PR1:
                    return PR1;
                case TreatmentCodeEnum.PR2:
                    return PR2;
                case TreatmentCodeEnum.RX1:
                    return RX1;
                case TreatmentCodeEnum.SL1:
                    return SL1;
                case TreatmentCodeEnum.SL2:
                    return SL2;
                case TreatmentCodeEnum.SL3:
                    return SL3;
                case TreatmentCodeEnum.SL4:
                    return SL4;
                case TreatmentCodeEnum.TH1:
                    return TH1;
                case TreatmentCodeEnum.TH2:
                    return TH2;
                case TreatmentCodeEnum.TH3:
                    return TH3;
                case TreatmentCodeEnum.TH4:
                    return TH4;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum TreatmentCodeEnum
    {
        BR1 = 1,
        BR2 = 2,
        PL1New = 3,
        PL1Revised = 4,
        PL2New = 5,
        PL2Revised = 6,
        PL3New = 7,
        PL3Revised = 8,
        PL4New = 9,
        PL4Revised = 10,
        PL5New = 11,
        PL5Revised = 12,
        PR1 = 13,
        PR2 = 14,
        RX1 = 15,
        SL1 = 16,
        SL2 = 17,
        SL3 = 18,
        SL4 = 19,
        TH1 = 20,
        TH2 = 21,
        TH3 = 22,
        TH4 = 23
    }

    public partial class TreatmentCodeBR1 : TreatmentCode
    {
        private TreatmentCodeBR1(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeBR1 Instance = new TreatmentCodeBR1(1, @"BR-1", @"BR-1: Brush Control");
    }

    public partial class TreatmentCodeBR2 : TreatmentCode
    {
        private TreatmentCodeBR2(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeBR2 Instance = new TreatmentCodeBR2(2, @"BR-2", @"BR-2: Brush Control");
    }

    public partial class TreatmentCodePL1New : TreatmentCode
    {
        private TreatmentCodePL1New(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL1New Instance = new TreatmentCodePL1New(3, @"PL-1-New", @"PL-1: New Plan 20-100 acres");
    }

    public partial class TreatmentCodePL1Revised : TreatmentCode
    {
        private TreatmentCodePL1Revised(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL1Revised Instance = new TreatmentCodePL1Revised(4, @"PL-1-Revised", @"PL-1: Revised Plan 20-100 acres");
    }

    public partial class TreatmentCodePL2New : TreatmentCode
    {
        private TreatmentCodePL2New(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL2New Instance = new TreatmentCodePL2New(5, @"PL-2-New", @"PL-2: New Plan 101-250 acres");
    }

    public partial class TreatmentCodePL2Revised : TreatmentCode
    {
        private TreatmentCodePL2Revised(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL2Revised Instance = new TreatmentCodePL2Revised(6, @"PL-2-Revised", @"PL-2: Revised Plan 101-250 acres");
    }

    public partial class TreatmentCodePL3New : TreatmentCode
    {
        private TreatmentCodePL3New(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL3New Instance = new TreatmentCodePL3New(7, @"PL-3-New", @"PL-3: New Plan 251-500 acres");
    }

    public partial class TreatmentCodePL3Revised : TreatmentCode
    {
        private TreatmentCodePL3Revised(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL3Revised Instance = new TreatmentCodePL3Revised(8, @"PL-3-Revised", @"PL-3: Revised Plan 251-500 acres");
    }

    public partial class TreatmentCodePL4New : TreatmentCode
    {
        private TreatmentCodePL4New(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL4New Instance = new TreatmentCodePL4New(9, @"PL-4-New", @"PL-4: New Plan 501-1000 acres");
    }

    public partial class TreatmentCodePL4Revised : TreatmentCode
    {
        private TreatmentCodePL4Revised(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL4Revised Instance = new TreatmentCodePL4Revised(10, @"PL-4-Revised", @"PL-4: Revised Plan 501-1000 acres");
    }

    public partial class TreatmentCodePL5New : TreatmentCode
    {
        private TreatmentCodePL5New(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL5New Instance = new TreatmentCodePL5New(11, @"PL-5-New", @"PL-5: New Plan 1001+ acres");
    }

    public partial class TreatmentCodePL5Revised : TreatmentCode
    {
        private TreatmentCodePL5Revised(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePL5Revised Instance = new TreatmentCodePL5Revised(12, @"PL-5-Revised", @"PL-5: Revised Plan 1001+ acres");
    }

    public partial class TreatmentCodePR1 : TreatmentCode
    {
        private TreatmentCodePR1(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePR1 Instance = new TreatmentCodePR1(13, @"PR-1", @"PR-1: Pruning");
    }

    public partial class TreatmentCodePR2 : TreatmentCode
    {
        private TreatmentCodePR2(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodePR2 Instance = new TreatmentCodePR2(14, @"PR-2", @"PR-2: Pruning");
    }

    public partial class TreatmentCodeRX1 : TreatmentCode
    {
        private TreatmentCodeRX1(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeRX1 Instance = new TreatmentCodeRX1(15, @"RX-1", @"RX-1: Prescribed Broadcast Burning");
    }

    public partial class TreatmentCodeSL1 : TreatmentCode
    {
        private TreatmentCodeSL1(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeSL1 Instance = new TreatmentCodeSL1(16, @"SL-1", @"SL-1: Slash Disposal");
    }

    public partial class TreatmentCodeSL2 : TreatmentCode
    {
        private TreatmentCodeSL2(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeSL2 Instance = new TreatmentCodeSL2(17, @"SL-2", @"SL-2: Slash Disposal");
    }

    public partial class TreatmentCodeSL3 : TreatmentCode
    {
        private TreatmentCodeSL3(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeSL3 Instance = new TreatmentCodeSL3(18, @"SL-3", @"SL-3: Slash Disposal");
    }

    public partial class TreatmentCodeSL4 : TreatmentCode
    {
        private TreatmentCodeSL4(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeSL4 Instance = new TreatmentCodeSL4(19, @"SL-4", @"SL-4: Slash Disposal");
    }

    public partial class TreatmentCodeTH1 : TreatmentCode
    {
        private TreatmentCodeTH1(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeTH1 Instance = new TreatmentCodeTH1(20, @"TH-1", @"TH-1: Thinning");
    }

    public partial class TreatmentCodeTH2 : TreatmentCode
    {
        private TreatmentCodeTH2(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeTH2 Instance = new TreatmentCodeTH2(21, @"TH-2", @"TH-2: Thinning");
    }

    public partial class TreatmentCodeTH3 : TreatmentCode
    {
        private TreatmentCodeTH3(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeTH3 Instance = new TreatmentCodeTH3(22, @"TH-3", @"TH-3: Thinning");
    }

    public partial class TreatmentCodeTH4 : TreatmentCode
    {
        private TreatmentCodeTH4(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : base(treatmentCodeID, treatmentCodeName, treatmentCodeDisplayName) {}
        public static readonly TreatmentCodeTH4 Instance = new TreatmentCodeTH4(23, @"TH-4", @"TH-4: Thinning");
    }
}