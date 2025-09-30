//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ForesterRole]
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
    public abstract partial class ForesterRole : IHavePrimaryKey
    {
        public static readonly ForesterRoleServiceForester ServiceForester = ForesterRoleServiceForester.Instance;
        public static readonly ForesterRoleServiceForestrySpecialist ServiceForestrySpecialist = ForesterRoleServiceForestrySpecialist.Instance;
        public static readonly ForesterRoleForestPracticesForester ForestPracticesForester = ForesterRoleForestPracticesForester.Instance;
        public static readonly ForesterRoleStewardshipFishAndWildlifeBiologist StewardshipFishAndWildlifeBiologist = ForesterRoleStewardshipFishAndWildlifeBiologist.Instance;
        public static readonly ForesterRoleUrbanForestryTechnician UrbanForestryTechnician = ForesterRoleUrbanForestryTechnician.Instance;
        public static readonly ForesterRoleCommunityResilienceCoordinator CommunityResilienceCoordinator = ForesterRoleCommunityResilienceCoordinator.Instance;
        public static readonly ForesterRoleRegulationAssistanceForester RegulationAssistanceForester = ForesterRoleRegulationAssistanceForester.Instance;
        public static readonly ForesterRoleFamilyForestFishPassageProgram FamilyForestFishPassageProgram = ForesterRoleFamilyForestFishPassageProgram.Instance;
        public static readonly ForesterRoleForestryRiparianEasementProgram ForestryRiparianEasementProgram = ForesterRoleForestryRiparianEasementProgram.Instance;
        public static readonly ForesterRoleRiversAndHabitatOpenSpaceProgramManager RiversAndHabitatOpenSpaceProgramManager = ForesterRoleRiversAndHabitatOpenSpaceProgramManager.Instance;
        public static readonly ForesterRoleServiceForestryProgramManager ServiceForestryProgramManager = ForesterRoleServiceForestryProgramManager.Instance;
        public static readonly ForesterRoleUcfStatewideSpecialist UcfStatewideSpecialist = ForesterRoleUcfStatewideSpecialist.Instance;
        public static readonly ForesterRoleSmallForestLandownerOfficeProgramManager SmallForestLandownerOfficeProgramManager = ForesterRoleSmallForestLandownerOfficeProgramManager.Instance;
        public static readonly ForesterRoleForestRegulationFishAndWildlifeBiologist ForestRegulationFishAndWildlifeBiologist = ForesterRoleForestRegulationFishAndWildlifeBiologist.Instance;

        public static readonly List<ForesterRole> All;
        public static readonly ReadOnlyDictionary<int, ForesterRole> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ForesterRole()
        {
            All = new List<ForesterRole> { ServiceForester, ServiceForestrySpecialist, ForestPracticesForester, StewardshipFishAndWildlifeBiologist, UrbanForestryTechnician, CommunityResilienceCoordinator, RegulationAssistanceForester, FamilyForestFishPassageProgram, ForestryRiparianEasementProgram, RiversAndHabitatOpenSpaceProgramManager, ServiceForestryProgramManager, UcfStatewideSpecialist, SmallForestLandownerOfficeProgramManager, ForestRegulationFishAndWildlifeBiologist };
            AllLookupDictionary = new ReadOnlyDictionary<int, ForesterRole>(All.ToDictionary(x => x.ForesterRoleID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ForesterRole(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder)
        {
            ForesterRoleID = foresterRoleID;
            ForesterRoleDisplayName = foresterRoleDisplayName;
            ForesterRoleName = foresterRoleName;
            SortOrder = sortOrder;
        }

        [Key]
        public int ForesterRoleID { get; private set; }
        public string ForesterRoleDisplayName { get; private set; }
        public string ForesterRoleName { get; private set; }
        public int SortOrder { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ForesterRoleID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ForesterRole other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ForesterRoleID == ForesterRoleID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ForesterRole);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ForesterRoleID;
        }

        public static bool operator ==(ForesterRole left, ForesterRole right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ForesterRole left, ForesterRole right)
        {
            return !Equals(left, right);
        }

        public ForesterRoleEnum ToEnum { get { return (ForesterRoleEnum)GetHashCode(); } }

        public static ForesterRole ToType(int enumValue)
        {
            return ToType((ForesterRoleEnum)enumValue);
        }

        public static ForesterRole ToType(ForesterRoleEnum enumValue)
        {
            switch (enumValue)
            {
                case ForesterRoleEnum.CommunityResilienceCoordinator:
                    return CommunityResilienceCoordinator;
                case ForesterRoleEnum.FamilyForestFishPassageProgram:
                    return FamilyForestFishPassageProgram;
                case ForesterRoleEnum.ForestPracticesForester:
                    return ForestPracticesForester;
                case ForesterRoleEnum.ForestRegulationFishAndWildlifeBiologist:
                    return ForestRegulationFishAndWildlifeBiologist;
                case ForesterRoleEnum.ForestryRiparianEasementProgram:
                    return ForestryRiparianEasementProgram;
                case ForesterRoleEnum.RegulationAssistanceForester:
                    return RegulationAssistanceForester;
                case ForesterRoleEnum.RiversAndHabitatOpenSpaceProgramManager:
                    return RiversAndHabitatOpenSpaceProgramManager;
                case ForesterRoleEnum.ServiceForester:
                    return ServiceForester;
                case ForesterRoleEnum.ServiceForestryProgramManager:
                    return ServiceForestryProgramManager;
                case ForesterRoleEnum.ServiceForestrySpecialist:
                    return ServiceForestrySpecialist;
                case ForesterRoleEnum.SmallForestLandownerOfficeProgramManager:
                    return SmallForestLandownerOfficeProgramManager;
                case ForesterRoleEnum.StewardshipFishAndWildlifeBiologist:
                    return StewardshipFishAndWildlifeBiologist;
                case ForesterRoleEnum.UcfStatewideSpecialist:
                    return UcfStatewideSpecialist;
                case ForesterRoleEnum.UrbanForestryTechnician:
                    return UrbanForestryTechnician;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum ForesterRoleEnum
    {
        ServiceForester = 1,
        ServiceForestrySpecialist = 2,
        ForestPracticesForester = 3,
        StewardshipFishAndWildlifeBiologist = 4,
        UrbanForestryTechnician = 5,
        CommunityResilienceCoordinator = 6,
        RegulationAssistanceForester = 7,
        FamilyForestFishPassageProgram = 8,
        ForestryRiparianEasementProgram = 9,
        RiversAndHabitatOpenSpaceProgramManager = 10,
        ServiceForestryProgramManager = 11,
        UcfStatewideSpecialist = 12,
        SmallForestLandownerOfficeProgramManager = 13,
        ForestRegulationFishAndWildlifeBiologist = 14
    }

    public partial class ForesterRoleServiceForester : ForesterRole
    {
        private ForesterRoleServiceForester(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleServiceForester Instance = new ForesterRoleServiceForester(1, @"Service Forester", @"ServiceForester", 10);
    }

    public partial class ForesterRoleServiceForestrySpecialist : ForesterRole
    {
        private ForesterRoleServiceForestrySpecialist(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleServiceForestrySpecialist Instance = new ForesterRoleServiceForestrySpecialist(2, @"Service Forestry Specialist", @"ServiceForestrySpecialist", 160);
    }

    public partial class ForesterRoleForestPracticesForester : ForesterRole
    {
        private ForesterRoleForestPracticesForester(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleForestPracticesForester Instance = new ForesterRoleForestPracticesForester(3, @"Forest Practices Forester", @"ForestPracticesForester", 80);
    }

    public partial class ForesterRoleStewardshipFishAndWildlifeBiologist : ForesterRole
    {
        private ForesterRoleStewardshipFishAndWildlifeBiologist(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleStewardshipFishAndWildlifeBiologist Instance = new ForesterRoleStewardshipFishAndWildlifeBiologist(4, @"Stewardship Fish & Wildlife Biologist", @"StewardshipFishAndWildlifeBiologist", 20);
    }

    public partial class ForesterRoleUrbanForestryTechnician : ForesterRole
    {
        private ForesterRoleUrbanForestryTechnician(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleUrbanForestryTechnician Instance = new ForesterRoleUrbanForestryTechnician(5, @"Urban Forestry Technician", @"UrbanForestryTechnician", 120);
    }

    public partial class ForesterRoleCommunityResilienceCoordinator : ForesterRole
    {
        private ForesterRoleCommunityResilienceCoordinator(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleCommunityResilienceCoordinator Instance = new ForesterRoleCommunityResilienceCoordinator(6, @"Community Resilience Coordinator", @"CommunityResilienceCoordinator", 70);
    }

    public partial class ForesterRoleRegulationAssistanceForester : ForesterRole
    {
        private ForesterRoleRegulationAssistanceForester(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleRegulationAssistanceForester Instance = new ForesterRoleRegulationAssistanceForester(7, @"Regulation Assistance Forester", @"RegulationAssistanceForester", 30);
    }

    public partial class ForesterRoleFamilyForestFishPassageProgram : ForesterRole
    {
        private ForesterRoleFamilyForestFishPassageProgram(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleFamilyForestFishPassageProgram Instance = new ForesterRoleFamilyForestFishPassageProgram(8, @"Family Forest Fish Passage Program", @"FamilyForestFishPassageProgram", 90);
    }

    public partial class ForesterRoleForestryRiparianEasementProgram : ForesterRole
    {
        private ForesterRoleForestryRiparianEasementProgram(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleForestryRiparianEasementProgram Instance = new ForesterRoleForestryRiparianEasementProgram(9, @"Forestry Riparian Easement Program", @"ForestryRiparianEasementProgram", 100);
    }

    public partial class ForesterRoleRiversAndHabitatOpenSpaceProgramManager : ForesterRole
    {
        private ForesterRoleRiversAndHabitatOpenSpaceProgramManager(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleRiversAndHabitatOpenSpaceProgramManager Instance = new ForesterRoleRiversAndHabitatOpenSpaceProgramManager(10, @"Rivers and Habitat Open Space Program Manager", @"RiversAndHabitatOpenSpaceProgramManager", 110);
    }

    public partial class ForesterRoleServiceForestryProgramManager : ForesterRole
    {
        private ForesterRoleServiceForestryProgramManager(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleServiceForestryProgramManager Instance = new ForesterRoleServiceForestryProgramManager(11, @"Service Forestry Program Manager", @"ServiceForestryProgramManager", 130);
    }

    public partial class ForesterRoleUcfStatewideSpecialist : ForesterRole
    {
        private ForesterRoleUcfStatewideSpecialist(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleUcfStatewideSpecialist Instance = new ForesterRoleUcfStatewideSpecialist(12, @"UCF Statewide Specialist", @"UcfStatewideSpecialist", 150);
    }

    public partial class ForesterRoleSmallForestLandownerOfficeProgramManager : ForesterRole
    {
        private ForesterRoleSmallForestLandownerOfficeProgramManager(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleSmallForestLandownerOfficeProgramManager Instance = new ForesterRoleSmallForestLandownerOfficeProgramManager(13, @"Small Forest Landowner Office Program Manager", @"SmallForestLandownerOfficeProgramManager", 140);
    }

    public partial class ForesterRoleForestRegulationFishAndWildlifeBiologist : ForesterRole
    {
        private ForesterRoleForestRegulationFishAndWildlifeBiologist(int foresterRoleID, string foresterRoleDisplayName, string foresterRoleName, int sortOrder) : base(foresterRoleID, foresterRoleDisplayName, foresterRoleName, sortOrder) {}
        public static readonly ForesterRoleForestRegulationFishAndWildlifeBiologist Instance = new ForesterRoleForestRegulationFishAndWildlifeBiologist(14, @"Forest Regulation Fish & Wildlife Biologist", @"ForestRegulationFishAndWildlifeBiologist", 60);
    }
}