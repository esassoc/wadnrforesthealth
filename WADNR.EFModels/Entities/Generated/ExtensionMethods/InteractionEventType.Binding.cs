//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEventType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WADNR.EFModels.Entities
{
    public abstract partial class InteractionEventType : IHavePrimaryKey
    {
        public static readonly InteractionEventTypeComplaint Complaint = InteractionEventTypeComplaint.Instance;
        public static readonly InteractionEventTypeFireSafetyPresentation FireSafetyPresentation = InteractionEventTypeFireSafetyPresentation.Instance;
        public static readonly InteractionEventTypeForestLandownerFieldDay ForestLandownerFieldDay = InteractionEventTypeForestLandownerFieldDay.Instance;
        public static readonly InteractionEventTypeOther Other = InteractionEventTypeOther.Instance;
        public static readonly InteractionEventTypeOutreach Outreach = InteractionEventTypeOutreach.Instance;
        public static readonly InteractionEventTypePhoneCall PhoneCall = InteractionEventTypePhoneCall.Instance;
        public static readonly InteractionEventTypeSiteVisit SiteVisit = InteractionEventTypeSiteVisit.Instance;
        public static readonly InteractionEventTypeTechnicalAssistance TechnicalAssistance = InteractionEventTypeTechnicalAssistance.Instance;
        public static readonly InteractionEventTypeWorkshop Workshop = InteractionEventTypeWorkshop.Instance;
        public static readonly InteractionEventTypeResearchMonitoring ResearchMonitoring = InteractionEventTypeResearchMonitoring.Instance;

        public static readonly List<InteractionEventType> All;
        public static readonly ReadOnlyDictionary<int, InteractionEventType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static InteractionEventType()
        {
            All = new List<InteractionEventType> { Complaint, FireSafetyPresentation, ForestLandownerFieldDay, Other, Outreach, PhoneCall, SiteVisit, TechnicalAssistance, Workshop, ResearchMonitoring };
            AllLookupDictionary = new ReadOnlyDictionary<int, InteractionEventType>(All.ToDictionary(x => x.InteractionEventTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected InteractionEventType(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName)
        {
            InteractionEventTypeID = interactionEventTypeID;
            InteractionEventTypeName = interactionEventTypeName;
            InteractionEventTypeDisplayName = interactionEventTypeDisplayName;
        }

        [Key]
        public int InteractionEventTypeID { get; private set; }
        public string InteractionEventTypeName { get; private set; }
        public string InteractionEventTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return InteractionEventTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(InteractionEventType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.InteractionEventTypeID == InteractionEventTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as InteractionEventType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return InteractionEventTypeID;
        }

        public static bool operator ==(InteractionEventType left, InteractionEventType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InteractionEventType left, InteractionEventType right)
        {
            return !Equals(left, right);
        }

        public InteractionEventTypeEnum ToEnum => (InteractionEventTypeEnum)GetHashCode();

        public static InteractionEventType ToType(int enumValue)
        {
            return ToType((InteractionEventTypeEnum)enumValue);
        }

        public static InteractionEventType ToType(InteractionEventTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case InteractionEventTypeEnum.Complaint:
                    return Complaint;
                case InteractionEventTypeEnum.FireSafetyPresentation:
                    return FireSafetyPresentation;
                case InteractionEventTypeEnum.ForestLandownerFieldDay:
                    return ForestLandownerFieldDay;
                case InteractionEventTypeEnum.Other:
                    return Other;
                case InteractionEventTypeEnum.Outreach:
                    return Outreach;
                case InteractionEventTypeEnum.PhoneCall:
                    return PhoneCall;
                case InteractionEventTypeEnum.ResearchMonitoring:
                    return ResearchMonitoring;
                case InteractionEventTypeEnum.SiteVisit:
                    return SiteVisit;
                case InteractionEventTypeEnum.TechnicalAssistance:
                    return TechnicalAssistance;
                case InteractionEventTypeEnum.Workshop:
                    return Workshop;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum InteractionEventTypeEnum
    {
        Complaint = 1,
        FireSafetyPresentation = 2,
        ForestLandownerFieldDay = 3,
        Other = 4,
        Outreach = 5,
        PhoneCall = 6,
        SiteVisit = 7,
        TechnicalAssistance = 8,
        Workshop = 9,
        ResearchMonitoring = 10
    }

    public partial class InteractionEventTypeComplaint : InteractionEventType
    {
        private InteractionEventTypeComplaint(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeComplaint Instance = new InteractionEventTypeComplaint(1, @"Complaint", @"Complaint");
    }

    public partial class InteractionEventTypeFireSafetyPresentation : InteractionEventType
    {
        private InteractionEventTypeFireSafetyPresentation(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeFireSafetyPresentation Instance = new InteractionEventTypeFireSafetyPresentation(2, @"FireSafetyPresentation", @"Fire Safety Presentation");
    }

    public partial class InteractionEventTypeForestLandownerFieldDay : InteractionEventType
    {
        private InteractionEventTypeForestLandownerFieldDay(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeForestLandownerFieldDay Instance = new InteractionEventTypeForestLandownerFieldDay(3, @"ForestLandownerFieldDay", @"Forest Landowner Field Day");
    }

    public partial class InteractionEventTypeOther : InteractionEventType
    {
        private InteractionEventTypeOther(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeOther Instance = new InteractionEventTypeOther(4, @"Other", @"Other");
    }

    public partial class InteractionEventTypeOutreach : InteractionEventType
    {
        private InteractionEventTypeOutreach(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeOutreach Instance = new InteractionEventTypeOutreach(5, @"Outreach", @"Education and Outreach");
    }

    public partial class InteractionEventTypePhoneCall : InteractionEventType
    {
        private InteractionEventTypePhoneCall(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypePhoneCall Instance = new InteractionEventTypePhoneCall(6, @"PhoneCall", @"Phone Call");
    }

    public partial class InteractionEventTypeSiteVisit : InteractionEventType
    {
        private InteractionEventTypeSiteVisit(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeSiteVisit Instance = new InteractionEventTypeSiteVisit(7, @"SiteVisit", @"Site Visit or Field Trip");
    }

    public partial class InteractionEventTypeTechnicalAssistance : InteractionEventType
    {
        private InteractionEventTypeTechnicalAssistance(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeTechnicalAssistance Instance = new InteractionEventTypeTechnicalAssistance(8, @"TechnicalAssistance", @"Technical Assistance");
    }

    public partial class InteractionEventTypeWorkshop : InteractionEventType
    {
        private InteractionEventTypeWorkshop(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeWorkshop Instance = new InteractionEventTypeWorkshop(9, @"Workshop", @"Workshop");
    }

    public partial class InteractionEventTypeResearchMonitoring : InteractionEventType
    {
        private InteractionEventTypeResearchMonitoring(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : base(interactionEventTypeID, interactionEventTypeName, interactionEventTypeDisplayName) {}
        public static readonly InteractionEventTypeResearchMonitoring Instance = new InteractionEventTypeResearchMonitoring(10, @"ResearchMonitoring", @"Research and Monitoring");
    }
}