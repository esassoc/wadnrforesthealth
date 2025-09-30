//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectStage]
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
    public abstract partial class ProjectStage : IHavePrimaryKey
    {
        public static readonly ProjectStagePlanned Planned = ProjectStagePlanned.Instance;
        public static readonly ProjectStageImplementation Implementation = ProjectStageImplementation.Instance;
        public static readonly ProjectStageCompleted Completed = ProjectStageCompleted.Instance;
        public static readonly ProjectStageCancelled Cancelled = ProjectStageCancelled.Instance;

        public static readonly List<ProjectStage> All;
        public static readonly ReadOnlyDictionary<int, ProjectStage> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ProjectStage()
        {
            All = new List<ProjectStage> { Planned, Implementation, Completed, Cancelled };
            AllLookupDictionary = new ReadOnlyDictionary<int, ProjectStage>(All.ToDictionary(x => x.ProjectStageID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ProjectStage(int projectStageID, string projectStageName, string projectStageDisplayName, int sortOrder, string projectStageColor)
        {
            ProjectStageID = projectStageID;
            ProjectStageName = projectStageName;
            ProjectStageDisplayName = projectStageDisplayName;
            SortOrder = sortOrder;
            ProjectStageColor = projectStageColor;
        }

        [Key]
        public int ProjectStageID { get; private set; }
        public string ProjectStageName { get; private set; }
        public string ProjectStageDisplayName { get; private set; }
        public int SortOrder { get; private set; }
        public string ProjectStageColor { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectStageID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ProjectStage other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ProjectStageID == ProjectStageID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectStage);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ProjectStageID;
        }

        public static bool operator ==(ProjectStage left, ProjectStage right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectStage left, ProjectStage right)
        {
            return !Equals(left, right);
        }

        public ProjectStageEnum ToEnum { get { return (ProjectStageEnum)GetHashCode(); } }

        public static ProjectStage ToType(int enumValue)
        {
            return ToType((ProjectStageEnum)enumValue);
        }

        public static ProjectStage ToType(ProjectStageEnum enumValue)
        {
            switch (enumValue)
            {
                case ProjectStageEnum.Cancelled:
                    return Cancelled;
                case ProjectStageEnum.Completed:
                    return Completed;
                case ProjectStageEnum.Implementation:
                    return Implementation;
                case ProjectStageEnum.Planned:
                    return Planned;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum ProjectStageEnum
    {
        Planned = 2,
        Implementation = 3,
        Completed = 4,
        Cancelled = 5
    }

    public partial class ProjectStagePlanned : ProjectStage
    {
        private ProjectStagePlanned(int projectStageID, string projectStageName, string projectStageDisplayName, int sortOrder, string projectStageColor) : base(projectStageID, projectStageName, projectStageDisplayName, sortOrder, projectStageColor) {}
        public static readonly ProjectStagePlanned Instance = new ProjectStagePlanned(2, @"Planned", @"Planned", 20, @"#80B2FF");
    }

    public partial class ProjectStageImplementation : ProjectStage
    {
        private ProjectStageImplementation(int projectStageID, string projectStageName, string projectStageDisplayName, int sortOrder, string projectStageColor) : base(projectStageID, projectStageName, projectStageDisplayName, sortOrder, projectStageColor) {}
        public static readonly ProjectStageImplementation Instance = new ProjectStageImplementation(3, @"Implementation", @"Implementation", 30, @"#1975FF");
    }

    public partial class ProjectStageCompleted : ProjectStage
    {
        private ProjectStageCompleted(int projectStageID, string projectStageName, string projectStageDisplayName, int sortOrder, string projectStageColor) : base(projectStageID, projectStageName, projectStageDisplayName, sortOrder, projectStageColor) {}
        public static readonly ProjectStageCompleted Instance = new ProjectStageCompleted(4, @"Completed", @"Completed", 50, @"#000066");
    }

    public partial class ProjectStageCancelled : ProjectStage
    {
        private ProjectStageCancelled(int projectStageID, string projectStageName, string projectStageDisplayName, int sortOrder, string projectStageColor) : base(projectStageID, projectStageName, projectStageDisplayName, sortOrder, projectStageColor) {}
        public static readonly ProjectStageCancelled Instance = new ProjectStageCancelled(5, @"Cancelled", @"Cancelled", 25, @"#D6D6D6");
    }
}