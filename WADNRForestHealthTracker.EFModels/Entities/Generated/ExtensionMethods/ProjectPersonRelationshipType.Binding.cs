//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectPersonRelationshipType]
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
    public abstract partial class ProjectPersonRelationshipType : IHavePrimaryKey
    {
        public static readonly ProjectPersonRelationshipTypePrimaryContact PrimaryContact = ProjectPersonRelationshipTypePrimaryContact.Instance;
        public static readonly ProjectPersonRelationshipTypePrivateLandowner PrivateLandowner = ProjectPersonRelationshipTypePrivateLandowner.Instance;
        public static readonly ProjectPersonRelationshipTypeContractor Contractor = ProjectPersonRelationshipTypeContractor.Instance;
        public static readonly ProjectPersonRelationshipTypeServiceForestryRegionalCoordinator ServiceForestryRegionalCoordinator = ProjectPersonRelationshipTypeServiceForestryRegionalCoordinator.Instance;

        public static readonly List<ProjectPersonRelationshipType> All;
        public static readonly ReadOnlyDictionary<int, ProjectPersonRelationshipType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ProjectPersonRelationshipType()
        {
            All = new List<ProjectPersonRelationshipType> { PrimaryContact, PrivateLandowner, Contractor, ServiceForestryRegionalCoordinator };
            AllLookupDictionary = new ReadOnlyDictionary<int, ProjectPersonRelationshipType>(All.ToDictionary(x => x.ProjectPersonRelationshipTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ProjectPersonRelationshipType(int projectPersonRelationshipTypeID, string projectPersonRelationshipTypeName, string projectPersonRelationshipTypeDisplayName, int fieldDefinitionID, bool isRequired, bool isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, int sortOrder)
        {
            ProjectPersonRelationshipTypeID = projectPersonRelationshipTypeID;
            ProjectPersonRelationshipTypeName = projectPersonRelationshipTypeName;
            ProjectPersonRelationshipTypeDisplayName = projectPersonRelationshipTypeDisplayName;
            FieldDefinitionID = fieldDefinitionID;
            IsRequired = isRequired;
            IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo = isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo;
            SortOrder = sortOrder;
        }
        public FieldDefinition FieldDefinition { get { return FieldDefinition.AllLookupDictionary[FieldDefinitionID]; } }
        [Key]
        public int ProjectPersonRelationshipTypeID { get; private set; }
        public string ProjectPersonRelationshipTypeName { get; private set; }
        public string ProjectPersonRelationshipTypeDisplayName { get; private set; }
        public int FieldDefinitionID { get; private set; }
        public bool IsRequired { get; private set; }
        public bool IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo { get; private set; }
        public int SortOrder { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectPersonRelationshipTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ProjectPersonRelationshipType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ProjectPersonRelationshipTypeID == ProjectPersonRelationshipTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectPersonRelationshipType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ProjectPersonRelationshipTypeID;
        }

        public static bool operator ==(ProjectPersonRelationshipType left, ProjectPersonRelationshipType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectPersonRelationshipType left, ProjectPersonRelationshipType right)
        {
            return !Equals(left, right);
        }

        public ProjectPersonRelationshipTypeEnum ToEnum { get { return (ProjectPersonRelationshipTypeEnum)GetHashCode(); } }

        public static ProjectPersonRelationshipType ToType(int enumValue)
        {
            return ToType((ProjectPersonRelationshipTypeEnum)enumValue);
        }

        public static ProjectPersonRelationshipType ToType(ProjectPersonRelationshipTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ProjectPersonRelationshipTypeEnum.Contractor:
                    return Contractor;
                case ProjectPersonRelationshipTypeEnum.PrimaryContact:
                    return PrimaryContact;
                case ProjectPersonRelationshipTypeEnum.PrivateLandowner:
                    return PrivateLandowner;
                case ProjectPersonRelationshipTypeEnum.ServiceForestryRegionalCoordinator:
                    return ServiceForestryRegionalCoordinator;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum ProjectPersonRelationshipTypeEnum
    {
        PrimaryContact = 1,
        PrivateLandowner = 2,
        Contractor = 3,
        ServiceForestryRegionalCoordinator = 4
    }

    public partial class ProjectPersonRelationshipTypePrimaryContact : ProjectPersonRelationshipType
    {
        private ProjectPersonRelationshipTypePrimaryContact(int projectPersonRelationshipTypeID, string projectPersonRelationshipTypeName, string projectPersonRelationshipTypeDisplayName, int fieldDefinitionID, bool isRequired, bool isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, int sortOrder) : base(projectPersonRelationshipTypeID, projectPersonRelationshipTypeName, projectPersonRelationshipTypeDisplayName, fieldDefinitionID, isRequired, isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, sortOrder) {}
        public static readonly ProjectPersonRelationshipTypePrimaryContact Instance = new ProjectPersonRelationshipTypePrimaryContact(1, @"PrimaryContact", @"Primary Contact", 275, false, false, 10);
    }

    public partial class ProjectPersonRelationshipTypePrivateLandowner : ProjectPersonRelationshipType
    {
        private ProjectPersonRelationshipTypePrivateLandowner(int projectPersonRelationshipTypeID, string projectPersonRelationshipTypeName, string projectPersonRelationshipTypeDisplayName, int fieldDefinitionID, bool isRequired, bool isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, int sortOrder) : base(projectPersonRelationshipTypeID, projectPersonRelationshipTypeName, projectPersonRelationshipTypeDisplayName, fieldDefinitionID, isRequired, isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, sortOrder) {}
        public static readonly ProjectPersonRelationshipTypePrivateLandowner Instance = new ProjectPersonRelationshipTypePrivateLandowner(2, @"PrivateLandowner", @"Private Landowner", 273, false, true, 30);
    }

    public partial class ProjectPersonRelationshipTypeContractor : ProjectPersonRelationshipType
    {
        private ProjectPersonRelationshipTypeContractor(int projectPersonRelationshipTypeID, string projectPersonRelationshipTypeName, string projectPersonRelationshipTypeDisplayName, int fieldDefinitionID, bool isRequired, bool isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, int sortOrder) : base(projectPersonRelationshipTypeID, projectPersonRelationshipTypeName, projectPersonRelationshipTypeDisplayName, fieldDefinitionID, isRequired, isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, sortOrder) {}
        public static readonly ProjectPersonRelationshipTypeContractor Instance = new ProjectPersonRelationshipTypeContractor(3, @"Contractor", @"Contractor", 272, false, false, 20);
    }

    public partial class ProjectPersonRelationshipTypeServiceForestryRegionalCoordinator : ProjectPersonRelationshipType
    {
        private ProjectPersonRelationshipTypeServiceForestryRegionalCoordinator(int projectPersonRelationshipTypeID, string projectPersonRelationshipTypeName, string projectPersonRelationshipTypeDisplayName, int fieldDefinitionID, bool isRequired, bool isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, int sortOrder) : base(projectPersonRelationshipTypeID, projectPersonRelationshipTypeName, projectPersonRelationshipTypeDisplayName, fieldDefinitionID, isRequired, isRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo, sortOrder) {}
        public static readonly ProjectPersonRelationshipTypeServiceForestryRegionalCoordinator Instance = new ProjectPersonRelationshipTypeServiceForestryRegionalCoordinator(4, @"ServiceForestryRegionalCoordinator", @"Service Forestry Regional Coordinator", 507, false, false, 40);
    }
}