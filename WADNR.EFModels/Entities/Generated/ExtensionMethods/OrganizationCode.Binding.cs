//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OrganizationCode]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WADNR.EFModels.Entities
{
    public abstract partial class OrganizationCode : IHavePrimaryKey
    {
        public static readonly OrganizationCodeForestResilienceDivision ForestResilienceDivision = OrganizationCodeForestResilienceDivision.Instance;
        public static readonly OrganizationCodeNEregion NEregion = OrganizationCodeNEregion.Instance;
        public static readonly OrganizationCodeSEregion SEregion = OrganizationCodeSEregion.Instance;
        public static readonly OrganizationCodeNWregion NWregion = OrganizationCodeNWregion.Instance;
        public static readonly OrganizationCodeSPSregion SPSregion = OrganizationCodeSPSregion.Instance;
        public static readonly OrganizationCodeOLYregion OLYregion = OrganizationCodeOLYregion.Instance;
        public static readonly OrganizationCodePCregion PCregion = OrganizationCodePCregion.Instance;

        public static readonly List<OrganizationCode> All;
        public static readonly ReadOnlyDictionary<int, OrganizationCode> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static OrganizationCode()
        {
            All = new List<OrganizationCode> { ForestResilienceDivision, NEregion, SEregion, NWregion, SPSregion, OLYregion, PCregion };
            AllLookupDictionary = new ReadOnlyDictionary<int, OrganizationCode>(All.ToDictionary(x => x.OrganizationCodeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected OrganizationCode(int organizationCodeID, string organizationCodeName, string organizationCodeValue)
        {
            OrganizationCodeID = organizationCodeID;
            OrganizationCodeName = organizationCodeName;
            OrganizationCodeValue = organizationCodeValue;
        }

        [Key]
        public int OrganizationCodeID { get; private set; }
        public string OrganizationCodeName { get; private set; }
        public string OrganizationCodeValue { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return OrganizationCodeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(OrganizationCode other)
        {
            if (other == null)
            {
                return false;
            }
            return other.OrganizationCodeID == OrganizationCodeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as OrganizationCode);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return OrganizationCodeID;
        }

        public static bool operator ==(OrganizationCode left, OrganizationCode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OrganizationCode left, OrganizationCode right)
        {
            return !Equals(left, right);
        }

        public OrganizationCodeEnum ToEnum => (OrganizationCodeEnum)GetHashCode();

        public static OrganizationCode ToType(int enumValue)
        {
            return ToType((OrganizationCodeEnum)enumValue);
        }

        public static OrganizationCode ToType(OrganizationCodeEnum enumValue)
        {
            switch (enumValue)
            {
                case OrganizationCodeEnum.ForestResilienceDivision:
                    return ForestResilienceDivision;
                case OrganizationCodeEnum.NEregion:
                    return NEregion;
                case OrganizationCodeEnum.NWregion:
                    return NWregion;
                case OrganizationCodeEnum.OLYregion:
                    return OLYregion;
                case OrganizationCodeEnum.PCregion:
                    return PCregion;
                case OrganizationCodeEnum.SEregion:
                    return SEregion;
                case OrganizationCodeEnum.SPSregion:
                    return SPSregion;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum OrganizationCodeEnum
    {
        ForestResilienceDivision = 1,
        NEregion = 2,
        SEregion = 3,
        NWregion = 4,
        SPSregion = 5,
        OLYregion = 6,
        PCregion = 7
    }

    public partial class OrganizationCodeForestResilienceDivision : OrganizationCode
    {
        private OrganizationCodeForestResilienceDivision(int organizationCodeID, string organizationCodeName, string organizationCodeValue) : base(organizationCodeID, organizationCodeName, organizationCodeValue) {}
        public static readonly OrganizationCodeForestResilienceDivision Instance = new OrganizationCodeForestResilienceDivision(1, @"Forest Resilience Division", @"5900");
    }

    public partial class OrganizationCodeNEregion : OrganizationCode
    {
        private OrganizationCodeNEregion(int organizationCodeID, string organizationCodeName, string organizationCodeValue) : base(organizationCodeID, organizationCodeName, organizationCodeValue) {}
        public static readonly OrganizationCodeNEregion Instance = new OrganizationCodeNEregion(2, @"NE region", @"2300");
    }

    public partial class OrganizationCodeSEregion : OrganizationCode
    {
        private OrganizationCodeSEregion(int organizationCodeID, string organizationCodeName, string organizationCodeValue) : base(organizationCodeID, organizationCodeName, organizationCodeValue) {}
        public static readonly OrganizationCodeSEregion Instance = new OrganizationCodeSEregion(3, @"SE region", @"0100");
    }

    public partial class OrganizationCodeNWregion : OrganizationCode
    {
        private OrganizationCodeNWregion(int organizationCodeID, string organizationCodeName, string organizationCodeValue) : base(organizationCodeID, organizationCodeName, organizationCodeValue) {}
        public static readonly OrganizationCodeNWregion Instance = new OrganizationCodeNWregion(4, @"NW region", @"1900");
    }

    public partial class OrganizationCodeSPSregion : OrganizationCode
    {
        private OrganizationCodeSPSregion(int organizationCodeID, string organizationCodeName, string organizationCodeValue) : base(organizationCodeID, organizationCodeName, organizationCodeValue) {}
        public static readonly OrganizationCodeSPSregion Instance = new OrganizationCodeSPSregion(5, @"SPS region", @"0900");
    }

    public partial class OrganizationCodeOLYregion : OrganizationCode
    {
        private OrganizationCodeOLYregion(int organizationCodeID, string organizationCodeName, string organizationCodeValue) : base(organizationCodeID, organizationCodeName, organizationCodeValue) {}
        public static readonly OrganizationCodeOLYregion Instance = new OrganizationCodeOLYregion(6, @"OLY region", @"0200");
    }

    public partial class OrganizationCodePCregion : OrganizationCode
    {
        private OrganizationCodePCregion(int organizationCodeID, string organizationCodeName, string organizationCodeValue) : base(organizationCodeID, organizationCodeName, organizationCodeValue) {}
        public static readonly OrganizationCodePCregion Instance = new OrganizationCodePCregion(7, @"PC region", @"0400");
    }
}