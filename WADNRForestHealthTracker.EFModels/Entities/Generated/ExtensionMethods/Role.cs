//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Role]
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
    public abstract partial class Role : IHavePrimaryKey
    {
        public static readonly RoleAdmin Admin = RoleAdmin.Instance;
        public static readonly RoleNormal Normal = RoleNormal.Instance;
        public static readonly RoleUnassigned Unassigned = RoleUnassigned.Instance;
        public static readonly RoleEsaAdmin EsaAdmin = RoleEsaAdmin.Instance;
        public static readonly RoleProjectSteward ProjectSteward = RoleProjectSteward.Instance;
        public static readonly RoleCanEditProgram CanEditProgram = RoleCanEditProgram.Instance;
        public static readonly RoleCanManagePageContent CanManagePageContent = RoleCanManagePageContent.Instance;
        public static readonly RoleCanViewLandownerInfo CanViewLandownerInfo = RoleCanViewLandownerInfo.Instance;
        public static readonly RoleCanManageFundSourcesAndAgreements CanManageFundSourcesAndAgreements = RoleCanManageFundSourcesAndAgreements.Instance;
        public static readonly RoleCanAddEditUsersContactsOrganizations CanAddEditUsersContactsOrganizations = RoleCanAddEditUsersContactsOrganizations.Instance;

        public static readonly List<Role> All;
        public static readonly ReadOnlyDictionary<int, Role> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static Role()
        {
            All = new List<Role> { Admin, Normal, Unassigned, EsaAdmin, ProjectSteward, CanEditProgram, CanManagePageContent, CanViewLandownerInfo, CanManageFundSourcesAndAgreements, CanAddEditUsersContactsOrganizations };
            AllLookupDictionary = new ReadOnlyDictionary<int, Role>(All.ToDictionary(x => x.RoleID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected Role(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole)
        {
            RoleID = roleID;
            RoleName = roleName;
            RoleDisplayName = roleDisplayName;
            RoleDescription = roleDescription;
            IsBaseRole = isBaseRole;
        }

        [Key]
        public int RoleID { get; private set; }
        public string RoleName { get; private set; }
        public string RoleDisplayName { get; private set; }
        public string RoleDescription { get; private set; }
        public bool IsBaseRole { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return RoleID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(Role other)
        {
            if (other == null)
            {
                return false;
            }
            return other.RoleID == RoleID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Role);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return RoleID;
        }

        public static bool operator ==(Role left, Role right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Role left, Role right)
        {
            return !Equals(left, right);
        }

        public RoleEnum ToEnum { get { return (RoleEnum)GetHashCode(); } }

        public static Role ToType(int enumValue)
        {
            return ToType((RoleEnum)enumValue);
        }

        public static Role ToType(RoleEnum enumValue)
        {
            switch (enumValue)
            {
                case RoleEnum.Admin:
                    return Admin;
                case RoleEnum.CanAddEditUsersContactsOrganizations:
                    return CanAddEditUsersContactsOrganizations;
                case RoleEnum.CanEditProgram:
                    return CanEditProgram;
                case RoleEnum.CanManageFundSourcesAndAgreements:
                    return CanManageFundSourcesAndAgreements;
                case RoleEnum.CanManagePageContent:
                    return CanManagePageContent;
                case RoleEnum.CanViewLandownerInfo:
                    return CanViewLandownerInfo;
                case RoleEnum.EsaAdmin:
                    return EsaAdmin;
                case RoleEnum.Normal:
                    return Normal;
                case RoleEnum.ProjectSteward:
                    return ProjectSteward;
                case RoleEnum.Unassigned:
                    return Unassigned;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum RoleEnum
    {
        Admin = 1,
        Normal = 2,
        Unassigned = 7,
        EsaAdmin = 8,
        ProjectSteward = 9,
        CanEditProgram = 10,
        CanManagePageContent = 11,
        CanViewLandownerInfo = 12,
        CanManageFundSourcesAndAgreements = 13,
        CanAddEditUsersContactsOrganizations = 14
    }

    public partial class RoleAdmin : Role
    {
        private RoleAdmin(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleAdmin Instance = new RoleAdmin(1, @"Admin", @"Administrator", @"", true);
    }

    public partial class RoleNormal : Role
    {
        private RoleNormal(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleNormal Instance = new RoleNormal(2, @"Normal", @"Normal User", @"Users with this role can propose new EIP projects, update existing EIP projects where their organization is the Lead Implementer, and view almost every page within the EIP Tracker.", true);
    }

    public partial class RoleUnassigned : Role
    {
        private RoleUnassigned(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleUnassigned Instance = new RoleUnassigned(7, @"Unassigned", @"Unassigned", @"", true);
    }

    public partial class RoleEsaAdmin : Role
    {
        private RoleEsaAdmin(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleEsaAdmin Instance = new RoleEsaAdmin(8, @"EsaAdmin", @"ESA Administrator", @"", true);
    }

    public partial class RoleProjectSteward : Role
    {
        private RoleProjectSteward(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleProjectSteward Instance = new RoleProjectSteward(9, @"ProjectSteward", @"Project Steward", @"Users with this role can approve Project Proposals, create new Projects, and approve Project Updates.", true);
    }

    public partial class RoleCanEditProgram : Role
    {
        private RoleCanEditProgram(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleCanEditProgram Instance = new RoleCanEditProgram(10, @"CanEditProgram", @"Can Edit Program", @"Users with this role can edit Projects that are from their Program", false);
    }

    public partial class RoleCanManagePageContent : Role
    {
        private RoleCanManagePageContent(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleCanManagePageContent Instance = new RoleCanManagePageContent(11, @"CanManagePageContent", @"Can Manage Page Content", @"Users with this role can edit content on custom pages", false);
    }

    public partial class RoleCanViewLandownerInfo : Role
    {
        private RoleCanViewLandownerInfo(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleCanViewLandownerInfo Instance = new RoleCanViewLandownerInfo(12, @"CanViewLandownerInfo", @"Can View Landowner Info", @"Users with this role can view landowner information", false);
    }

    public partial class RoleCanManageFundSourcesAndAgreements : Role
    {
        private RoleCanManageFundSourcesAndAgreements(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleCanManageFundSourcesAndAgreements Instance = new RoleCanManageFundSourcesAndAgreements(13, @"CanManageFundSourcesAndAgreements", @"Can Manage Fund Sources and Agreements", @"Users with this role can manage Fund Sources and Agreements", false);
    }

    public partial class RoleCanAddEditUsersContactsOrganizations : Role
    {
        private RoleCanAddEditUsersContactsOrganizations(int roleID, string roleName, string roleDisplayName, string roleDescription, bool isBaseRole) : base(roleID, roleName, roleDisplayName, roleDescription, isBaseRole) {}
        public static readonly RoleCanAddEditUsersContactsOrganizations Instance = new RoleCanAddEditUsersContactsOrganizations(14, @"CanAddEditUsersContactsOrganizations", @"Can Add/Edit Users, Contacts, Organizations", @"Users with this role can add and edit Users, Contacts and Organizations.", false);
    }
}