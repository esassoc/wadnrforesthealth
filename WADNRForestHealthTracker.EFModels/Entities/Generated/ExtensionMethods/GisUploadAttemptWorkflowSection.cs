//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadAttemptWorkflowSection]
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
    public abstract partial class GisUploadAttemptWorkflowSection : IHavePrimaryKey
    {
        public static readonly GisUploadAttemptWorkflowSectionUploadGisFile UploadGisFile = GisUploadAttemptWorkflowSectionUploadGisFile.Instance;
        public static readonly GisUploadAttemptWorkflowSectionValidateFeatures ValidateFeatures = GisUploadAttemptWorkflowSectionValidateFeatures.Instance;
        public static readonly GisUploadAttemptWorkflowSectionValidateMetadata ValidateMetadata = GisUploadAttemptWorkflowSectionValidateMetadata.Instance;
        public static readonly GisUploadAttemptWorkflowSectionReviewMapping ReviewMapping = GisUploadAttemptWorkflowSectionReviewMapping.Instance;
        public static readonly GisUploadAttemptWorkflowSectionRviewStagedImport RviewStagedImport = GisUploadAttemptWorkflowSectionRviewStagedImport.Instance;

        public static readonly List<GisUploadAttemptWorkflowSection> All;
        public static readonly ReadOnlyDictionary<int, GisUploadAttemptWorkflowSection> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static GisUploadAttemptWorkflowSection()
        {
            All = new List<GisUploadAttemptWorkflowSection> { UploadGisFile, ValidateFeatures, ValidateMetadata, ReviewMapping, RviewStagedImport };
            AllLookupDictionary = new ReadOnlyDictionary<int, GisUploadAttemptWorkflowSection>(All.ToDictionary(x => x.GisUploadAttemptWorkflowSectionID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected GisUploadAttemptWorkflowSection(int gisUploadAttemptWorkflowSectionID, string gisUploadAttemptWorkflowSectionName, string gisUploadAttemptWorkflowSectionDisplayName, int sortOrder, bool hasCompletionStatus, int gisUploadAttemptWorkflowSectionGroupingID)
        {
            GisUploadAttemptWorkflowSectionID = gisUploadAttemptWorkflowSectionID;
            GisUploadAttemptWorkflowSectionName = gisUploadAttemptWorkflowSectionName;
            GisUploadAttemptWorkflowSectionDisplayName = gisUploadAttemptWorkflowSectionDisplayName;
            SortOrder = sortOrder;
            HasCompletionStatus = hasCompletionStatus;
            GisUploadAttemptWorkflowSectionGroupingID = gisUploadAttemptWorkflowSectionGroupingID;
        }
        public GisUploadAttemptWorkflowSectionGrouping GisUploadAttemptWorkflowSectionGrouping { get { return GisUploadAttemptWorkflowSectionGrouping.AllLookupDictionary[GisUploadAttemptWorkflowSectionGroupingID]; } }
        [Key]
        public int GisUploadAttemptWorkflowSectionID { get; private set; }
        public string GisUploadAttemptWorkflowSectionName { get; private set; }
        public string GisUploadAttemptWorkflowSectionDisplayName { get; private set; }
        public int SortOrder { get; private set; }
        public bool HasCompletionStatus { get; private set; }
        public int GisUploadAttemptWorkflowSectionGroupingID { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return GisUploadAttemptWorkflowSectionID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(GisUploadAttemptWorkflowSection other)
        {
            if (other == null)
            {
                return false;
            }
            return other.GisUploadAttemptWorkflowSectionID == GisUploadAttemptWorkflowSectionID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as GisUploadAttemptWorkflowSection);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return GisUploadAttemptWorkflowSectionID;
        }

        public static bool operator ==(GisUploadAttemptWorkflowSection left, GisUploadAttemptWorkflowSection right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GisUploadAttemptWorkflowSection left, GisUploadAttemptWorkflowSection right)
        {
            return !Equals(left, right);
        }

        public GisUploadAttemptWorkflowSectionEnum ToEnum { get { return (GisUploadAttemptWorkflowSectionEnum)GetHashCode(); } }

        public static GisUploadAttemptWorkflowSection ToType(int enumValue)
        {
            return ToType((GisUploadAttemptWorkflowSectionEnum)enumValue);
        }

        public static GisUploadAttemptWorkflowSection ToType(GisUploadAttemptWorkflowSectionEnum enumValue)
        {
            switch (enumValue)
            {
                case GisUploadAttemptWorkflowSectionEnum.ReviewMapping:
                    return ReviewMapping;
                case GisUploadAttemptWorkflowSectionEnum.RviewStagedImport:
                    return RviewStagedImport;
                case GisUploadAttemptWorkflowSectionEnum.UploadGisFile:
                    return UploadGisFile;
                case GisUploadAttemptWorkflowSectionEnum.ValidateFeatures:
                    return ValidateFeatures;
                case GisUploadAttemptWorkflowSectionEnum.ValidateMetadata:
                    return ValidateMetadata;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum GisUploadAttemptWorkflowSectionEnum
    {
        UploadGisFile = 2,
        ValidateFeatures = 3,
        ValidateMetadata = 4,
        ReviewMapping = 6,
        RviewStagedImport = 7
    }

    public partial class GisUploadAttemptWorkflowSectionUploadGisFile : GisUploadAttemptWorkflowSection
    {
        private GisUploadAttemptWorkflowSectionUploadGisFile(int gisUploadAttemptWorkflowSectionID, string gisUploadAttemptWorkflowSectionName, string gisUploadAttemptWorkflowSectionDisplayName, int sortOrder, bool hasCompletionStatus, int gisUploadAttemptWorkflowSectionGroupingID) : base(gisUploadAttemptWorkflowSectionID, gisUploadAttemptWorkflowSectionName, gisUploadAttemptWorkflowSectionDisplayName, sortOrder, hasCompletionStatus, gisUploadAttemptWorkflowSectionGroupingID) {}
        public static readonly GisUploadAttemptWorkflowSectionUploadGisFile Instance = new GisUploadAttemptWorkflowSectionUploadGisFile(2, @"UploadGisFile", @"Upload GIS File", 20, true, 1);
    }

    public partial class GisUploadAttemptWorkflowSectionValidateFeatures : GisUploadAttemptWorkflowSection
    {
        private GisUploadAttemptWorkflowSectionValidateFeatures(int gisUploadAttemptWorkflowSectionID, string gisUploadAttemptWorkflowSectionName, string gisUploadAttemptWorkflowSectionDisplayName, int sortOrder, bool hasCompletionStatus, int gisUploadAttemptWorkflowSectionGroupingID) : base(gisUploadAttemptWorkflowSectionID, gisUploadAttemptWorkflowSectionName, gisUploadAttemptWorkflowSectionDisplayName, sortOrder, hasCompletionStatus, gisUploadAttemptWorkflowSectionGroupingID) {}
        public static readonly GisUploadAttemptWorkflowSectionValidateFeatures Instance = new GisUploadAttemptWorkflowSectionValidateFeatures(3, @"ValidateFeatures", @"Validate Features", 30, true, 1);
    }

    public partial class GisUploadAttemptWorkflowSectionValidateMetadata : GisUploadAttemptWorkflowSection
    {
        private GisUploadAttemptWorkflowSectionValidateMetadata(int gisUploadAttemptWorkflowSectionID, string gisUploadAttemptWorkflowSectionName, string gisUploadAttemptWorkflowSectionDisplayName, int sortOrder, bool hasCompletionStatus, int gisUploadAttemptWorkflowSectionGroupingID) : base(gisUploadAttemptWorkflowSectionID, gisUploadAttemptWorkflowSectionName, gisUploadAttemptWorkflowSectionDisplayName, sortOrder, hasCompletionStatus, gisUploadAttemptWorkflowSectionGroupingID) {}
        public static readonly GisUploadAttemptWorkflowSectionValidateMetadata Instance = new GisUploadAttemptWorkflowSectionValidateMetadata(4, @"ValidateMetadata", @"Validate Metadata", 40, true, 2);
    }

    public partial class GisUploadAttemptWorkflowSectionReviewMapping : GisUploadAttemptWorkflowSection
    {
        private GisUploadAttemptWorkflowSectionReviewMapping(int gisUploadAttemptWorkflowSectionID, string gisUploadAttemptWorkflowSectionName, string gisUploadAttemptWorkflowSectionDisplayName, int sortOrder, bool hasCompletionStatus, int gisUploadAttemptWorkflowSectionGroupingID) : base(gisUploadAttemptWorkflowSectionID, gisUploadAttemptWorkflowSectionName, gisUploadAttemptWorkflowSectionDisplayName, sortOrder, hasCompletionStatus, gisUploadAttemptWorkflowSectionGroupingID) {}
        public static readonly GisUploadAttemptWorkflowSectionReviewMapping Instance = new GisUploadAttemptWorkflowSectionReviewMapping(6, @"ReviewMapping", @"Review Mapping", 60, true, 2);
    }

    public partial class GisUploadAttemptWorkflowSectionRviewStagedImport : GisUploadAttemptWorkflowSection
    {
        private GisUploadAttemptWorkflowSectionRviewStagedImport(int gisUploadAttemptWorkflowSectionID, string gisUploadAttemptWorkflowSectionName, string gisUploadAttemptWorkflowSectionDisplayName, int sortOrder, bool hasCompletionStatus, int gisUploadAttemptWorkflowSectionGroupingID) : base(gisUploadAttemptWorkflowSectionID, gisUploadAttemptWorkflowSectionName, gisUploadAttemptWorkflowSectionDisplayName, sortOrder, hasCompletionStatus, gisUploadAttemptWorkflowSectionGroupingID) {}
        public static readonly GisUploadAttemptWorkflowSectionRviewStagedImport Instance = new GisUploadAttemptWorkflowSectionRviewStagedImport(7, @"RviewStagedImport", @"Review Staged Import", 70, true, 2);
    }
}