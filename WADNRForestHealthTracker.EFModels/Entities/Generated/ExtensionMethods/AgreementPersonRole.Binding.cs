//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementPersonRole]
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
    public abstract partial class AgreementPersonRole : IHavePrimaryKey
    {
        public static readonly AgreementPersonRoleContractManager ContractManager = AgreementPersonRoleContractManager.Instance;
        public static readonly AgreementPersonRoleProjectManager ProjectManager = AgreementPersonRoleProjectManager.Instance;
        public static readonly AgreementPersonRoleProjectCoordinator ProjectCoordinator = AgreementPersonRoleProjectCoordinator.Instance;
        public static readonly AgreementPersonRoleSigner Signer = AgreementPersonRoleSigner.Instance;
        public static readonly AgreementPersonRoleTechnicalContact TechnicalContact = AgreementPersonRoleTechnicalContact.Instance;

        public static readonly List<AgreementPersonRole> All;
        public static readonly ReadOnlyDictionary<int, AgreementPersonRole> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static AgreementPersonRole()
        {
            All = new List<AgreementPersonRole> { ContractManager, ProjectManager, ProjectCoordinator, Signer, TechnicalContact };
            AllLookupDictionary = new ReadOnlyDictionary<int, AgreementPersonRole>(All.ToDictionary(x => x.AgreementPersonRoleID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected AgreementPersonRole(int agreementPersonRoleID, string agreementPersonRoleName, string agreementPersonRoleDisplayName)
        {
            AgreementPersonRoleID = agreementPersonRoleID;
            AgreementPersonRoleName = agreementPersonRoleName;
            AgreementPersonRoleDisplayName = agreementPersonRoleDisplayName;
        }

        [Key]
        public int AgreementPersonRoleID { get; private set; }
        public string AgreementPersonRoleName { get; private set; }
        public string AgreementPersonRoleDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return AgreementPersonRoleID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(AgreementPersonRole other)
        {
            if (other == null)
            {
                return false;
            }
            return other.AgreementPersonRoleID == AgreementPersonRoleID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as AgreementPersonRole);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return AgreementPersonRoleID;
        }

        public static bool operator ==(AgreementPersonRole left, AgreementPersonRole right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AgreementPersonRole left, AgreementPersonRole right)
        {
            return !Equals(left, right);
        }

        public AgreementPersonRoleEnum ToEnum { get { return (AgreementPersonRoleEnum)GetHashCode(); } }

        public static AgreementPersonRole ToType(int enumValue)
        {
            return ToType((AgreementPersonRoleEnum)enumValue);
        }

        public static AgreementPersonRole ToType(AgreementPersonRoleEnum enumValue)
        {
            switch (enumValue)
            {
                case AgreementPersonRoleEnum.ContractManager:
                    return ContractManager;
                case AgreementPersonRoleEnum.ProjectCoordinator:
                    return ProjectCoordinator;
                case AgreementPersonRoleEnum.ProjectManager:
                    return ProjectManager;
                case AgreementPersonRoleEnum.Signer:
                    return Signer;
                case AgreementPersonRoleEnum.TechnicalContact:
                    return TechnicalContact;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum AgreementPersonRoleEnum
    {
        ContractManager = 1,
        ProjectManager = 2,
        ProjectCoordinator = 3,
        Signer = 4,
        TechnicalContact = 5
    }

    public partial class AgreementPersonRoleContractManager : AgreementPersonRole
    {
        private AgreementPersonRoleContractManager(int agreementPersonRoleID, string agreementPersonRoleName, string agreementPersonRoleDisplayName) : base(agreementPersonRoleID, agreementPersonRoleName, agreementPersonRoleDisplayName) {}
        public static readonly AgreementPersonRoleContractManager Instance = new AgreementPersonRoleContractManager(1, @"ContractManager", @"Contract Manager");
    }

    public partial class AgreementPersonRoleProjectManager : AgreementPersonRole
    {
        private AgreementPersonRoleProjectManager(int agreementPersonRoleID, string agreementPersonRoleName, string agreementPersonRoleDisplayName) : base(agreementPersonRoleID, agreementPersonRoleName, agreementPersonRoleDisplayName) {}
        public static readonly AgreementPersonRoleProjectManager Instance = new AgreementPersonRoleProjectManager(2, @"ProjectManager", @"Project Manager");
    }

    public partial class AgreementPersonRoleProjectCoordinator : AgreementPersonRole
    {
        private AgreementPersonRoleProjectCoordinator(int agreementPersonRoleID, string agreementPersonRoleName, string agreementPersonRoleDisplayName) : base(agreementPersonRoleID, agreementPersonRoleName, agreementPersonRoleDisplayName) {}
        public static readonly AgreementPersonRoleProjectCoordinator Instance = new AgreementPersonRoleProjectCoordinator(3, @"ProjectCoordinator", @"Project Coordinator");
    }

    public partial class AgreementPersonRoleSigner : AgreementPersonRole
    {
        private AgreementPersonRoleSigner(int agreementPersonRoleID, string agreementPersonRoleName, string agreementPersonRoleDisplayName) : base(agreementPersonRoleID, agreementPersonRoleName, agreementPersonRoleDisplayName) {}
        public static readonly AgreementPersonRoleSigner Instance = new AgreementPersonRoleSigner(4, @"Signer", @"Signer");
    }

    public partial class AgreementPersonRoleTechnicalContact : AgreementPersonRole
    {
        private AgreementPersonRoleTechnicalContact(int agreementPersonRoleID, string agreementPersonRoleName, string agreementPersonRoleDisplayName) : base(agreementPersonRoleID, agreementPersonRoleName, agreementPersonRoleDisplayName) {}
        public static readonly AgreementPersonRoleTechnicalContact Instance = new AgreementPersonRoleTechnicalContact(5, @"TechnicalContact", @"Technical Contact");
    }
}