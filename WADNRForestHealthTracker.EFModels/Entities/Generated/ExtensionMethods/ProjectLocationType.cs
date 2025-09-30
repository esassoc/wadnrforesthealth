//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectLocationType]
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
    public abstract partial class ProjectLocationType : IHavePrimaryKey
    {
        public static readonly ProjectLocationTypeProjectArea ProjectArea = ProjectLocationTypeProjectArea.Instance;
        public static readonly ProjectLocationTypeTreatmentArea TreatmentArea = ProjectLocationTypeTreatmentArea.Instance;
        public static readonly ProjectLocationTypeResearchPlot ResearchPlot = ProjectLocationTypeResearchPlot.Instance;
        public static readonly ProjectLocationTypeTestSite TestSite = ProjectLocationTypeTestSite.Instance;
        public static readonly ProjectLocationTypeOther Other = ProjectLocationTypeOther.Instance;

        public static readonly List<ProjectLocationType> All;
        public static readonly ReadOnlyDictionary<int, ProjectLocationType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ProjectLocationType()
        {
            All = new List<ProjectLocationType> { ProjectArea, TreatmentArea, ResearchPlot, TestSite, Other };
            AllLookupDictionary = new ReadOnlyDictionary<int, ProjectLocationType>(All.ToDictionary(x => x.ProjectLocationTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ProjectLocationType(int projectLocationTypeID, string projectLocationTypeName, string projectLocationTypeDisplayName, string projectLocationTypeMapLayerColor)
        {
            ProjectLocationTypeID = projectLocationTypeID;
            ProjectLocationTypeName = projectLocationTypeName;
            ProjectLocationTypeDisplayName = projectLocationTypeDisplayName;
            ProjectLocationTypeMapLayerColor = projectLocationTypeMapLayerColor;
        }

        [Key]
        public int ProjectLocationTypeID { get; private set; }
        public string ProjectLocationTypeName { get; private set; }
        public string ProjectLocationTypeDisplayName { get; private set; }
        public string ProjectLocationTypeMapLayerColor { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectLocationTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ProjectLocationType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ProjectLocationTypeID == ProjectLocationTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectLocationType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ProjectLocationTypeID;
        }

        public static bool operator ==(ProjectLocationType left, ProjectLocationType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectLocationType left, ProjectLocationType right)
        {
            return !Equals(left, right);
        }

        public ProjectLocationTypeEnum ToEnum { get { return (ProjectLocationTypeEnum)GetHashCode(); } }

        public static ProjectLocationType ToType(int enumValue)
        {
            return ToType((ProjectLocationTypeEnum)enumValue);
        }

        public static ProjectLocationType ToType(ProjectLocationTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ProjectLocationTypeEnum.Other:
                    return Other;
                case ProjectLocationTypeEnum.ProjectArea:
                    return ProjectArea;
                case ProjectLocationTypeEnum.ResearchPlot:
                    return ResearchPlot;
                case ProjectLocationTypeEnum.TestSite:
                    return TestSite;
                case ProjectLocationTypeEnum.TreatmentArea:
                    return TreatmentArea;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum ProjectLocationTypeEnum
    {
        ProjectArea = 1,
        TreatmentArea = 2,
        ResearchPlot = 3,
        TestSite = 4,
        Other = 5
    }

    public partial class ProjectLocationTypeProjectArea : ProjectLocationType
    {
        private ProjectLocationTypeProjectArea(int projectLocationTypeID, string projectLocationTypeName, string projectLocationTypeDisplayName, string projectLocationTypeMapLayerColor) : base(projectLocationTypeID, projectLocationTypeName, projectLocationTypeDisplayName, projectLocationTypeMapLayerColor) {}
        public static readonly ProjectLocationTypeProjectArea Instance = new ProjectLocationTypeProjectArea(1, @"ProjectArea", @"Project Area", @"#2c96c3");
    }

    public partial class ProjectLocationTypeTreatmentArea : ProjectLocationType
    {
        private ProjectLocationTypeTreatmentArea(int projectLocationTypeID, string projectLocationTypeName, string projectLocationTypeDisplayName, string projectLocationTypeMapLayerColor) : base(projectLocationTypeID, projectLocationTypeName, projectLocationTypeDisplayName, projectLocationTypeMapLayerColor) {}
        public static readonly ProjectLocationTypeTreatmentArea Instance = new ProjectLocationTypeTreatmentArea(2, @"TreatmentArea", @"Treatment Area", @"#2b7ac3");
    }

    public partial class ProjectLocationTypeResearchPlot : ProjectLocationType
    {
        private ProjectLocationTypeResearchPlot(int projectLocationTypeID, string projectLocationTypeName, string projectLocationTypeDisplayName, string projectLocationTypeMapLayerColor) : base(projectLocationTypeID, projectLocationTypeName, projectLocationTypeDisplayName, projectLocationTypeMapLayerColor) {}
        public static readonly ProjectLocationTypeResearchPlot Instance = new ProjectLocationTypeResearchPlot(3, @"ResearchPlot", @"Research Plot", @"#2a44c3");
    }

    public partial class ProjectLocationTypeTestSite : ProjectLocationType
    {
        private ProjectLocationTypeTestSite(int projectLocationTypeID, string projectLocationTypeName, string projectLocationTypeDisplayName, string projectLocationTypeMapLayerColor) : base(projectLocationTypeID, projectLocationTypeName, projectLocationTypeDisplayName, projectLocationTypeMapLayerColor) {}
        public static readonly ProjectLocationTypeTestSite Instance = new ProjectLocationTypeTestSite(4, @"TestSite", @"Test Site", @"#3e29c3");
    }

    public partial class ProjectLocationTypeOther : ProjectLocationType
    {
        private ProjectLocationTypeOther(int projectLocationTypeID, string projectLocationTypeName, string projectLocationTypeDisplayName, string projectLocationTypeMapLayerColor) : base(projectLocationTypeID, projectLocationTypeName, projectLocationTypeDisplayName, projectLocationTypeMapLayerColor) {}
        public static readonly ProjectLocationTypeOther Instance = new ProjectLocationTypeOther(5, @"Other", @"Other", @"#6928c3");
    }
}