//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProjectUpdateSection]
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
    public abstract partial class ProjectUpdateSection : IHavePrimaryKey
    {
        public static readonly ProjectUpdateSectionBasics Basics = ProjectUpdateSectionBasics.Instance;
        public static readonly ProjectUpdateSectionLocationSimple LocationSimple = ProjectUpdateSectionLocationSimple.Instance;
        public static readonly ProjectUpdateSectionLocationDetailed LocationDetailed = ProjectUpdateSectionLocationDetailed.Instance;
        public static readonly ProjectUpdateSectionExpectedFunding ExpectedFunding = ProjectUpdateSectionExpectedFunding.Instance;
        public static readonly ProjectUpdateSectionPhotos Photos = ProjectUpdateSectionPhotos.Instance;
        public static readonly ProjectUpdateSectionExternalLinks ExternalLinks = ProjectUpdateSectionExternalLinks.Instance;
        public static readonly ProjectUpdateSectionNotesAndDocuments NotesAndDocuments = ProjectUpdateSectionNotesAndDocuments.Instance;
        public static readonly ProjectUpdateSectionOrganizations Organizations = ProjectUpdateSectionOrganizations.Instance;
        public static readonly ProjectUpdateSectionContacts Contacts = ProjectUpdateSectionContacts.Instance;
        public static readonly ProjectUpdateSectionDNRUplandRegions DNRUplandRegions = ProjectUpdateSectionDNRUplandRegions.Instance;
        public static readonly ProjectUpdateSectionPriorityLandscapes PriorityLandscapes = ProjectUpdateSectionPriorityLandscapes.Instance;
        public static readonly ProjectUpdateSectionTreatments Treatments = ProjectUpdateSectionTreatments.Instance;
        public static readonly ProjectUpdateSectionCounties Counties = ProjectUpdateSectionCounties.Instance;

        public static readonly List<ProjectUpdateSection> All;
        public static readonly ReadOnlyDictionary<int, ProjectUpdateSection> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ProjectUpdateSection()
        {
            All = new List<ProjectUpdateSection> { Basics, LocationSimple, LocationDetailed, ExpectedFunding, Photos, ExternalLinks, NotesAndDocuments, Organizations, Contacts, DNRUplandRegions, PriorityLandscapes, Treatments, Counties };
            AllLookupDictionary = new ReadOnlyDictionary<int, ProjectUpdateSection>(All.ToDictionary(x => x.ProjectUpdateSectionID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ProjectUpdateSection(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID)
        {
            ProjectUpdateSectionID = projectUpdateSectionID;
            ProjectUpdateSectionName = projectUpdateSectionName;
            ProjectUpdateSectionDisplayName = projectUpdateSectionDisplayName;
            SortOrder = sortOrder;
            HasCompletionStatus = hasCompletionStatus;
            ProjectWorkflowSectionGroupingID = projectWorkflowSectionGroupingID;
        }
        public ProjectWorkflowSectionGrouping ProjectWorkflowSectionGrouping { get { return ProjectWorkflowSectionGrouping.AllLookupDictionary[ProjectWorkflowSectionGroupingID]; } }
        [Key]
        public int ProjectUpdateSectionID { get; private set; }
        public string ProjectUpdateSectionName { get; private set; }
        public string ProjectUpdateSectionDisplayName { get; private set; }
        public int SortOrder { get; private set; }
        public bool HasCompletionStatus { get; private set; }
        public int ProjectWorkflowSectionGroupingID { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ProjectUpdateSectionID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ProjectUpdateSection other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ProjectUpdateSectionID == ProjectUpdateSectionID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectUpdateSection);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ProjectUpdateSectionID;
        }

        public static bool operator ==(ProjectUpdateSection left, ProjectUpdateSection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectUpdateSection left, ProjectUpdateSection right)
        {
            return !Equals(left, right);
        }

        public ProjectUpdateSectionEnum ToEnum { get { return (ProjectUpdateSectionEnum)GetHashCode(); } }

        public static ProjectUpdateSection ToType(int enumValue)
        {
            return ToType((ProjectUpdateSectionEnum)enumValue);
        }

        public static ProjectUpdateSection ToType(ProjectUpdateSectionEnum enumValue)
        {
            switch (enumValue)
            {
                case ProjectUpdateSectionEnum.Basics:
                    return Basics;
                case ProjectUpdateSectionEnum.Contacts:
                    return Contacts;
                case ProjectUpdateSectionEnum.Counties:
                    return Counties;
                case ProjectUpdateSectionEnum.DNRUplandRegions:
                    return DNRUplandRegions;
                case ProjectUpdateSectionEnum.ExpectedFunding:
                    return ExpectedFunding;
                case ProjectUpdateSectionEnum.ExternalLinks:
                    return ExternalLinks;
                case ProjectUpdateSectionEnum.LocationDetailed:
                    return LocationDetailed;
                case ProjectUpdateSectionEnum.LocationSimple:
                    return LocationSimple;
                case ProjectUpdateSectionEnum.NotesAndDocuments:
                    return NotesAndDocuments;
                case ProjectUpdateSectionEnum.Organizations:
                    return Organizations;
                case ProjectUpdateSectionEnum.Photos:
                    return Photos;
                case ProjectUpdateSectionEnum.PriorityLandscapes:
                    return PriorityLandscapes;
                case ProjectUpdateSectionEnum.Treatments:
                    return Treatments;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum ProjectUpdateSectionEnum
    {
        Basics = 2,
        LocationSimple = 3,
        LocationDetailed = 4,
        ExpectedFunding = 7,
        Photos = 9,
        ExternalLinks = 10,
        NotesAndDocuments = 11,
        Organizations = 12,
        Contacts = 13,
        DNRUplandRegions = 14,
        PriorityLandscapes = 15,
        Treatments = 17,
        Counties = 18
    }

    public partial class ProjectUpdateSectionBasics : ProjectUpdateSection
    {
        private ProjectUpdateSectionBasics(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionBasics Instance = new ProjectUpdateSectionBasics(2, @"Basics", @"Basics", 10, true, 6);
    }

    public partial class ProjectUpdateSectionLocationSimple : ProjectUpdateSection
    {
        private ProjectUpdateSectionLocationSimple(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionLocationSimple Instance = new ProjectUpdateSectionLocationSimple(3, @"LocationSimple", @"Location - Simple", 20, true, 6);
    }

    public partial class ProjectUpdateSectionLocationDetailed : ProjectUpdateSection
    {
        private ProjectUpdateSectionLocationDetailed(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionLocationDetailed Instance = new ProjectUpdateSectionLocationDetailed(4, @"LocationDetailed", @"Location - Detailed", 30, false, 2);
    }

    public partial class ProjectUpdateSectionExpectedFunding : ProjectUpdateSection
    {
        private ProjectUpdateSectionExpectedFunding(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionExpectedFunding Instance = new ProjectUpdateSectionExpectedFunding(7, @"ExpectedFunding", @"Expected Funding", 100, false, 5);
    }

    public partial class ProjectUpdateSectionPhotos : ProjectUpdateSection
    {
        private ProjectUpdateSectionPhotos(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionPhotos Instance = new ProjectUpdateSectionPhotos(9, @"Photos", @"Photos", 110, false, 5);
    }

    public partial class ProjectUpdateSectionExternalLinks : ProjectUpdateSection
    {
        private ProjectUpdateSectionExternalLinks(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionExternalLinks Instance = new ProjectUpdateSectionExternalLinks(10, @"ExternalLinks", @"External Links", 130, false, 5);
    }

    public partial class ProjectUpdateSectionNotesAndDocuments : ProjectUpdateSection
    {
        private ProjectUpdateSectionNotesAndDocuments(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionNotesAndDocuments Instance = new ProjectUpdateSectionNotesAndDocuments(11, @"NotesAndDocuments", @"Documents and Notes", 120, false, 5);
    }

    public partial class ProjectUpdateSectionOrganizations : ProjectUpdateSection
    {
        private ProjectUpdateSectionOrganizations(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionOrganizations Instance = new ProjectUpdateSectionOrganizations(12, @"Organizations", @"Organizations", 90, true, 5);
    }

    public partial class ProjectUpdateSectionContacts : ProjectUpdateSection
    {
        private ProjectUpdateSectionContacts(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionContacts Instance = new ProjectUpdateSectionContacts(13, @"Contacts", @"Contacts", 80, true, 5);
    }

    public partial class ProjectUpdateSectionDNRUplandRegions : ProjectUpdateSection
    {
        private ProjectUpdateSectionDNRUplandRegions(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionDNRUplandRegions Instance = new ProjectUpdateSectionDNRUplandRegions(14, @"DNRUplandRegions", @"DNR Upland Regions", 50, true, 2);
    }

    public partial class ProjectUpdateSectionPriorityLandscapes : ProjectUpdateSection
    {
        private ProjectUpdateSectionPriorityLandscapes(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionPriorityLandscapes Instance = new ProjectUpdateSectionPriorityLandscapes(15, @"PriorityLandscapes", @"Priority Landscapes", 40, true, 2);
    }

    public partial class ProjectUpdateSectionTreatments : ProjectUpdateSection
    {
        private ProjectUpdateSectionTreatments(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionTreatments Instance = new ProjectUpdateSectionTreatments(17, @"Treatments", @"Treatments", 70, false, 2);
    }

    public partial class ProjectUpdateSectionCounties : ProjectUpdateSection
    {
        private ProjectUpdateSectionCounties(int projectUpdateSectionID, string projectUpdateSectionName, string projectUpdateSectionDisplayName, int sortOrder, bool hasCompletionStatus, int projectWorkflowSectionGroupingID) : base(projectUpdateSectionID, projectUpdateSectionName, projectUpdateSectionDisplayName, sortOrder, hasCompletionStatus, projectWorkflowSectionGroupingID) {}
        public static readonly ProjectUpdateSectionCounties Instance = new ProjectUpdateSectionCounties(18, @"Counties", @"Counties", 60, true, 2);
    }
}