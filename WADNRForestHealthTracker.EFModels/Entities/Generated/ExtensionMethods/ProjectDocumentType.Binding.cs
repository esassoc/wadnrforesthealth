//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectDocumentType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WADNRForestHealthTracker.EFModels.Entities
{
    public abstract partial class ProjectDocumentType : IHavePrimaryKey
    {
        public static readonly ProjectDocumentTypeCostShareApplication CostShareApplication = ProjectDocumentTypeCostShareApplication.Instance;
        public static readonly ProjectDocumentTypeCostShareSheet CostShareSheet = ProjectDocumentTypeCostShareSheet.Instance;
        public static readonly ProjectDocumentTypeTreatmentSpecs TreatmentSpecs = ProjectDocumentTypeTreatmentSpecs.Instance;
        public static readonly ProjectDocumentTypeMap Map = ProjectDocumentTypeMap.Instance;
        public static readonly ProjectDocumentTypeApprovalLetter ApprovalLetter = ProjectDocumentTypeApprovalLetter.Instance;
        public static readonly ProjectDocumentTypeClaimForm ClaimForm = ProjectDocumentTypeClaimForm.Instance;
        public static readonly ProjectDocumentTypeOther Other = ProjectDocumentTypeOther.Instance;
        public static readonly ProjectDocumentTypeManagementPlan ManagementPlan = ProjectDocumentTypeManagementPlan.Instance;
        public static readonly ProjectDocumentTypeMonitoringReport MonitoringReport = ProjectDocumentTypeMonitoringReport.Instance;
        public static readonly ProjectDocumentTypeProjectScoringMatrix ProjectScoringMatrix = ProjectDocumentTypeProjectScoringMatrix.Instance;
        public static readonly ProjectDocumentTypeSiteVisitNotes SiteVisitNotes = ProjectDocumentTypeSiteVisitNotes.Instance;
        public static readonly ProjectDocumentTypeApprovalChecklist ApprovalChecklist = ProjectDocumentTypeApprovalChecklist.Instance;
        public static readonly ProjectDocumentTypeSelfCostStatement SelfCostStatement = ProjectDocumentTypeSelfCostStatement.Instance;

        public static readonly List<ProjectDocumentType> All;
        public static readonly ReadOnlyDictionary<int, ProjectDocumentType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ProjectDocumentType()
        {
            All = new List<ProjectDocumentType> { CostShareApplication, CostShareSheet, TreatmentSpecs, Map, ApprovalLetter, ClaimForm, Other, ManagementPlan, MonitoringReport, ProjectScoringMatrix, SiteVisitNotes, ApprovalChecklist, SelfCostStatement };
            AllLookupDictionary = new ReadOnlyDictionary<int, ProjectDocumentType>(All.ToDictionary(x => x.ProjectDocumentTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ProjectDocumentType(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName)
        {
            ProjectDocumentTypeID = projectDocumentTypeID;
            ProjectDocumentTypeName = projectDocumentTypeName;
            ProjectDocumentTypeDisplayName = projectDocumentTypeDisplayName;
        }

        [Key]
        public int ProjectDocumentTypeID { get; private set; }
        public string ProjectDocumentTypeName { get; private set; }
        public string ProjectDocumentTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectDocumentTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ProjectDocumentType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ProjectDocumentTypeID == ProjectDocumentTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectDocumentType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ProjectDocumentTypeID;
        }

        public static bool operator ==(ProjectDocumentType left, ProjectDocumentType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectDocumentType left, ProjectDocumentType right)
        {
            return !Equals(left, right);
        }

        public ProjectDocumentTypeEnum ToEnum => (ProjectDocumentTypeEnum)GetHashCode();

        public static ProjectDocumentType ToType(int enumValue)
        {
            return ToType((ProjectDocumentTypeEnum)enumValue);
        }

        public static ProjectDocumentType ToType(ProjectDocumentTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ProjectDocumentTypeEnum.ApprovalChecklist:
                    return ApprovalChecklist;
                case ProjectDocumentTypeEnum.ApprovalLetter:
                    return ApprovalLetter;
                case ProjectDocumentTypeEnum.ClaimForm:
                    return ClaimForm;
                case ProjectDocumentTypeEnum.CostShareApplication:
                    return CostShareApplication;
                case ProjectDocumentTypeEnum.CostShareSheet:
                    return CostShareSheet;
                case ProjectDocumentTypeEnum.ManagementPlan:
                    return ManagementPlan;
                case ProjectDocumentTypeEnum.Map:
                    return Map;
                case ProjectDocumentTypeEnum.MonitoringReport:
                    return MonitoringReport;
                case ProjectDocumentTypeEnum.Other:
                    return Other;
                case ProjectDocumentTypeEnum.ProjectScoringMatrix:
                    return ProjectScoringMatrix;
                case ProjectDocumentTypeEnum.SelfCostStatement:
                    return SelfCostStatement;
                case ProjectDocumentTypeEnum.SiteVisitNotes:
                    return SiteVisitNotes;
                case ProjectDocumentTypeEnum.TreatmentSpecs:
                    return TreatmentSpecs;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ProjectDocumentTypeEnum
    {
        CostShareApplication = 14,
        CostShareSheet = 15,
        TreatmentSpecs = 16,
        Map = 17,
        ApprovalLetter = 18,
        ClaimForm = 19,
        Other = 20,
        ManagementPlan = 21,
        MonitoringReport = 22,
        ProjectScoringMatrix = 23,
        SiteVisitNotes = 24,
        ApprovalChecklist = 25,
        SelfCostStatement = 26
    }

    public partial class ProjectDocumentTypeCostShareApplication : ProjectDocumentType
    {
        private ProjectDocumentTypeCostShareApplication(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeCostShareApplication Instance = new ProjectDocumentTypeCostShareApplication(14, @"CostShareApplication", @"Cost Share Application");
    }

    public partial class ProjectDocumentTypeCostShareSheet : ProjectDocumentType
    {
        private ProjectDocumentTypeCostShareSheet(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeCostShareSheet Instance = new ProjectDocumentTypeCostShareSheet(15, @"CostShareSheet", @"Cost Share Sheet");
    }

    public partial class ProjectDocumentTypeTreatmentSpecs : ProjectDocumentType
    {
        private ProjectDocumentTypeTreatmentSpecs(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeTreatmentSpecs Instance = new ProjectDocumentTypeTreatmentSpecs(16, @"TreatmentSpecs", @"Treatment Specs");
    }

    public partial class ProjectDocumentTypeMap : ProjectDocumentType
    {
        private ProjectDocumentTypeMap(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeMap Instance = new ProjectDocumentTypeMap(17, @"Map", @"Map");
    }

    public partial class ProjectDocumentTypeApprovalLetter : ProjectDocumentType
    {
        private ProjectDocumentTypeApprovalLetter(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeApprovalLetter Instance = new ProjectDocumentTypeApprovalLetter(18, @"ApprovalLetter", @"Approval Letter");
    }

    public partial class ProjectDocumentTypeClaimForm : ProjectDocumentType
    {
        private ProjectDocumentTypeClaimForm(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeClaimForm Instance = new ProjectDocumentTypeClaimForm(19, @"ClaimForm", @"Claim Form");
    }

    public partial class ProjectDocumentTypeOther : ProjectDocumentType
    {
        private ProjectDocumentTypeOther(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeOther Instance = new ProjectDocumentTypeOther(20, @"Other", @"Other");
    }

    public partial class ProjectDocumentTypeManagementPlan : ProjectDocumentType
    {
        private ProjectDocumentTypeManagementPlan(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeManagementPlan Instance = new ProjectDocumentTypeManagementPlan(21, @"ManagementPlan", @"Management Plan");
    }

    public partial class ProjectDocumentTypeMonitoringReport : ProjectDocumentType
    {
        private ProjectDocumentTypeMonitoringReport(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeMonitoringReport Instance = new ProjectDocumentTypeMonitoringReport(22, @"MonitoringReport", @"Monitoring Report");
    }

    public partial class ProjectDocumentTypeProjectScoringMatrix : ProjectDocumentType
    {
        private ProjectDocumentTypeProjectScoringMatrix(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeProjectScoringMatrix Instance = new ProjectDocumentTypeProjectScoringMatrix(23, @"ProjectScoringMatrix", @"Project Scoring Matrix");
    }

    public partial class ProjectDocumentTypeSiteVisitNotes : ProjectDocumentType
    {
        private ProjectDocumentTypeSiteVisitNotes(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeSiteVisitNotes Instance = new ProjectDocumentTypeSiteVisitNotes(24, @"SiteVisitNotes", @"Site Visit Notes");
    }

    public partial class ProjectDocumentTypeApprovalChecklist : ProjectDocumentType
    {
        private ProjectDocumentTypeApprovalChecklist(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeApprovalChecklist Instance = new ProjectDocumentTypeApprovalChecklist(25, @"ApprovalChecklist", @"Approval Checklist");
    }

    public partial class ProjectDocumentTypeSelfCostStatement : ProjectDocumentType
    {
        private ProjectDocumentTypeSelfCostStatement(int projectDocumentTypeID, string projectDocumentTypeName, string projectDocumentTypeDisplayName) : base(projectDocumentTypeID, projectDocumentTypeName, projectDocumentTypeDisplayName) {}
        public static readonly ProjectDocumentTypeSelfCostStatement Instance = new ProjectDocumentTypeSelfCostStatement(26, @"Self-CostStatement", @"Self-Cost Statement");
    }
}