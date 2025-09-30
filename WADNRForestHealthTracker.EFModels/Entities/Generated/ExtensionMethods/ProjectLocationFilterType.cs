//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectLocationFilterType]
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
    public abstract partial class ProjectLocationFilterType : IHavePrimaryKey
    {
        public static readonly ProjectLocationFilterTypeTaxonomyTrunk TaxonomyTrunk = ProjectLocationFilterTypeTaxonomyTrunk.Instance;
        public static readonly ProjectLocationFilterTypeTaxonomyBranch TaxonomyBranch = ProjectLocationFilterTypeTaxonomyBranch.Instance;
        public static readonly ProjectLocationFilterTypeProjectType ProjectType = ProjectLocationFilterTypeProjectType.Instance;
        public static readonly ProjectLocationFilterTypeClassification Classification = ProjectLocationFilterTypeClassification.Instance;
        public static readonly ProjectLocationFilterTypeProjectStage ProjectStage = ProjectLocationFilterTypeProjectStage.Instance;
        public static readonly ProjectLocationFilterTypeLeadImplementer LeadImplementer = ProjectLocationFilterTypeLeadImplementer.Instance;
        public static readonly ProjectLocationFilterTypeProgram Program = ProjectLocationFilterTypeProgram.Instance;

        public static readonly List<ProjectLocationFilterType> All;
        public static readonly ReadOnlyDictionary<int, ProjectLocationFilterType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ProjectLocationFilterType()
        {
            All = new List<ProjectLocationFilterType> { TaxonomyTrunk, TaxonomyBranch, ProjectType, Classification, ProjectStage, LeadImplementer, Program };
            AllLookupDictionary = new ReadOnlyDictionary<int, ProjectLocationFilterType>(All.ToDictionary(x => x.ProjectLocationFilterTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ProjectLocationFilterType(int projectLocationFilterTypeID, string projectLocationFilterTypeName, string projectLocationFilterTypeNameWithIdentifier, string projectLocationFilterTypeDisplayName, int sortOrder, int displayGroup)
        {
            ProjectLocationFilterTypeID = projectLocationFilterTypeID;
            ProjectLocationFilterTypeName = projectLocationFilterTypeName;
            ProjectLocationFilterTypeNameWithIdentifier = projectLocationFilterTypeNameWithIdentifier;
            ProjectLocationFilterTypeDisplayName = projectLocationFilterTypeDisplayName;
            SortOrder = sortOrder;
            DisplayGroup = displayGroup;
        }

        [Key]
        public int ProjectLocationFilterTypeID { get; private set; }
        public string ProjectLocationFilterTypeName { get; private set; }
        public string ProjectLocationFilterTypeNameWithIdentifier { get; private set; }
        public string ProjectLocationFilterTypeDisplayName { get; private set; }
        public int SortOrder { get; private set; }
        public int DisplayGroup { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectLocationFilterTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ProjectLocationFilterType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ProjectLocationFilterTypeID == ProjectLocationFilterTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectLocationFilterType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ProjectLocationFilterTypeID;
        }

        public static bool operator ==(ProjectLocationFilterType left, ProjectLocationFilterType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectLocationFilterType left, ProjectLocationFilterType right)
        {
            return !Equals(left, right);
        }

        public ProjectLocationFilterTypeEnum ToEnum { get { return (ProjectLocationFilterTypeEnum)GetHashCode(); } }

        public static ProjectLocationFilterType ToType(int enumValue)
        {
            return ToType((ProjectLocationFilterTypeEnum)enumValue);
        }

        public static ProjectLocationFilterType ToType(ProjectLocationFilterTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ProjectLocationFilterTypeEnum.Classification:
                    return Classification;
                case ProjectLocationFilterTypeEnum.LeadImplementer:
                    return LeadImplementer;
                case ProjectLocationFilterTypeEnum.Program:
                    return Program;
                case ProjectLocationFilterTypeEnum.ProjectStage:
                    return ProjectStage;
                case ProjectLocationFilterTypeEnum.ProjectType:
                    return ProjectType;
                case ProjectLocationFilterTypeEnum.TaxonomyBranch:
                    return TaxonomyBranch;
                case ProjectLocationFilterTypeEnum.TaxonomyTrunk:
                    return TaxonomyTrunk;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum ProjectLocationFilterTypeEnum
    {
        TaxonomyTrunk = 1,
        TaxonomyBranch = 2,
        ProjectType = 3,
        Classification = 4,
        ProjectStage = 5,
        LeadImplementer = 6,
        Program = 7
    }

    public partial class ProjectLocationFilterTypeTaxonomyTrunk : ProjectLocationFilterType
    {
        private ProjectLocationFilterTypeTaxonomyTrunk(int projectLocationFilterTypeID, string projectLocationFilterTypeName, string projectLocationFilterTypeNameWithIdentifier, string projectLocationFilterTypeDisplayName, int sortOrder, int displayGroup) : base(projectLocationFilterTypeID, projectLocationFilterTypeName, projectLocationFilterTypeNameWithIdentifier, projectLocationFilterTypeDisplayName, sortOrder, displayGroup) {}
        public static readonly ProjectLocationFilterTypeTaxonomyTrunk Instance = new ProjectLocationFilterTypeTaxonomyTrunk(1, @"TaxonomyTrunk", @"TaxonomyTrunkID", @"Taxonomy Trunk", 10, 1);
    }

    public partial class ProjectLocationFilterTypeTaxonomyBranch : ProjectLocationFilterType
    {
        private ProjectLocationFilterTypeTaxonomyBranch(int projectLocationFilterTypeID, string projectLocationFilterTypeName, string projectLocationFilterTypeNameWithIdentifier, string projectLocationFilterTypeDisplayName, int sortOrder, int displayGroup) : base(projectLocationFilterTypeID, projectLocationFilterTypeName, projectLocationFilterTypeNameWithIdentifier, projectLocationFilterTypeDisplayName, sortOrder, displayGroup) {}
        public static readonly ProjectLocationFilterTypeTaxonomyBranch Instance = new ProjectLocationFilterTypeTaxonomyBranch(2, @"TaxonomyBranch", @"TaxonomyBranchID", @"Taxonomy Branch", 20, 1);
    }

    public partial class ProjectLocationFilterTypeProjectType : ProjectLocationFilterType
    {
        private ProjectLocationFilterTypeProjectType(int projectLocationFilterTypeID, string projectLocationFilterTypeName, string projectLocationFilterTypeNameWithIdentifier, string projectLocationFilterTypeDisplayName, int sortOrder, int displayGroup) : base(projectLocationFilterTypeID, projectLocationFilterTypeName, projectLocationFilterTypeNameWithIdentifier, projectLocationFilterTypeDisplayName, sortOrder, displayGroup) {}
        public static readonly ProjectLocationFilterTypeProjectType Instance = new ProjectLocationFilterTypeProjectType(3, @"ProjectType", @"ProjectTypeID", @"Project Type", 30, 1);
    }

    public partial class ProjectLocationFilterTypeClassification : ProjectLocationFilterType
    {
        private ProjectLocationFilterTypeClassification(int projectLocationFilterTypeID, string projectLocationFilterTypeName, string projectLocationFilterTypeNameWithIdentifier, string projectLocationFilterTypeDisplayName, int sortOrder, int displayGroup) : base(projectLocationFilterTypeID, projectLocationFilterTypeName, projectLocationFilterTypeNameWithIdentifier, projectLocationFilterTypeDisplayName, sortOrder, displayGroup) {}
        public static readonly ProjectLocationFilterTypeClassification Instance = new ProjectLocationFilterTypeClassification(4, @"Classification", @"ClassificationID", @"Classification", 40, 3);
    }

    public partial class ProjectLocationFilterTypeProjectStage : ProjectLocationFilterType
    {
        private ProjectLocationFilterTypeProjectStage(int projectLocationFilterTypeID, string projectLocationFilterTypeName, string projectLocationFilterTypeNameWithIdentifier, string projectLocationFilterTypeDisplayName, int sortOrder, int displayGroup) : base(projectLocationFilterTypeID, projectLocationFilterTypeName, projectLocationFilterTypeNameWithIdentifier, projectLocationFilterTypeDisplayName, sortOrder, displayGroup) {}
        public static readonly ProjectLocationFilterTypeProjectStage Instance = new ProjectLocationFilterTypeProjectStage(5, @"ProjectStage", @"ProjectStageID", @"Project Stage", 50, 3);
    }

    public partial class ProjectLocationFilterTypeLeadImplementer : ProjectLocationFilterType
    {
        private ProjectLocationFilterTypeLeadImplementer(int projectLocationFilterTypeID, string projectLocationFilterTypeName, string projectLocationFilterTypeNameWithIdentifier, string projectLocationFilterTypeDisplayName, int sortOrder, int displayGroup) : base(projectLocationFilterTypeID, projectLocationFilterTypeName, projectLocationFilterTypeNameWithIdentifier, projectLocationFilterTypeDisplayName, sortOrder, displayGroup) {}
        public static readonly ProjectLocationFilterTypeLeadImplementer Instance = new ProjectLocationFilterTypeLeadImplementer(6, @"LeadImplementer", @"LeadImplementerID", @"Lead Implementer", 60, 4);
    }

    public partial class ProjectLocationFilterTypeProgram : ProjectLocationFilterType
    {
        private ProjectLocationFilterTypeProgram(int projectLocationFilterTypeID, string projectLocationFilterTypeName, string projectLocationFilterTypeNameWithIdentifier, string projectLocationFilterTypeDisplayName, int sortOrder, int displayGroup) : base(projectLocationFilterTypeID, projectLocationFilterTypeName, projectLocationFilterTypeNameWithIdentifier, projectLocationFilterTypeDisplayName, sortOrder, displayGroup) {}
        public static readonly ProjectLocationFilterTypeProgram Instance = new ProjectLocationFilterTypeProgram(7, @"Program", @"ProgramID", @"Program", 70, 4);
    }
}