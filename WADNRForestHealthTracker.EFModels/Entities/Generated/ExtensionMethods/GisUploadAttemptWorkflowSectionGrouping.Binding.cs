//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadAttemptWorkflowSectionGrouping]
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
    public abstract partial class GisUploadAttemptWorkflowSectionGrouping : IHavePrimaryKey
    {
        public static readonly GisUploadAttemptWorkflowSectionGroupingGeospatialValidation GeospatialValidation = GisUploadAttemptWorkflowSectionGroupingGeospatialValidation.Instance;
        public static readonly GisUploadAttemptWorkflowSectionGroupingMetadataMapping MetadataMapping = GisUploadAttemptWorkflowSectionGroupingMetadataMapping.Instance;

        public static readonly List<GisUploadAttemptWorkflowSectionGrouping> All;
        public static readonly ReadOnlyDictionary<int, GisUploadAttemptWorkflowSectionGrouping> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static GisUploadAttemptWorkflowSectionGrouping()
        {
            All = new List<GisUploadAttemptWorkflowSectionGrouping> { GeospatialValidation, MetadataMapping };
            AllLookupDictionary = new ReadOnlyDictionary<int, GisUploadAttemptWorkflowSectionGrouping>(All.ToDictionary(x => x.GisUploadAttemptWorkflowSectionGroupingID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected GisUploadAttemptWorkflowSectionGrouping(int gisUploadAttemptWorkflowSectionGroupingID, string gisUploadAttemptWorkflowSectionGroupingName, string gisUploadAttemptWorkflowSectionGroupingDisplayName, int sortOrder)
        {
            GisUploadAttemptWorkflowSectionGroupingID = gisUploadAttemptWorkflowSectionGroupingID;
            GisUploadAttemptWorkflowSectionGroupingName = gisUploadAttemptWorkflowSectionGroupingName;
            GisUploadAttemptWorkflowSectionGroupingDisplayName = gisUploadAttemptWorkflowSectionGroupingDisplayName;
            SortOrder = sortOrder;
        }
        public List<GisUploadAttemptWorkflowSection> GisUploadAttemptWorkflowSections { get { return GisUploadAttemptWorkflowSection.All.Where(x => x.GisUploadAttemptWorkflowSectionGroupingID == GisUploadAttemptWorkflowSectionGroupingID).ToList(); } }
        [Key]
        public int GisUploadAttemptWorkflowSectionGroupingID { get; private set; }
        public string GisUploadAttemptWorkflowSectionGroupingName { get; private set; }
        public string GisUploadAttemptWorkflowSectionGroupingDisplayName { get; private set; }
        public int SortOrder { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return GisUploadAttemptWorkflowSectionGroupingID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(GisUploadAttemptWorkflowSectionGrouping other)
        {
            if (other == null)
            {
                return false;
            }
            return other.GisUploadAttemptWorkflowSectionGroupingID == GisUploadAttemptWorkflowSectionGroupingID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as GisUploadAttemptWorkflowSectionGrouping);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return GisUploadAttemptWorkflowSectionGroupingID;
        }

        public static bool operator ==(GisUploadAttemptWorkflowSectionGrouping left, GisUploadAttemptWorkflowSectionGrouping right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GisUploadAttemptWorkflowSectionGrouping left, GisUploadAttemptWorkflowSectionGrouping right)
        {
            return !Equals(left, right);
        }

        public GisUploadAttemptWorkflowSectionGroupingEnum ToEnum { get { return (GisUploadAttemptWorkflowSectionGroupingEnum)GetHashCode(); } }

        public static GisUploadAttemptWorkflowSectionGrouping ToType(int enumValue)
        {
            return ToType((GisUploadAttemptWorkflowSectionGroupingEnum)enumValue);
        }

        public static GisUploadAttemptWorkflowSectionGrouping ToType(GisUploadAttemptWorkflowSectionGroupingEnum enumValue)
        {
            switch (enumValue)
            {
                case GisUploadAttemptWorkflowSectionGroupingEnum.GeospatialValidation:
                    return GeospatialValidation;
                case GisUploadAttemptWorkflowSectionGroupingEnum.MetadataMapping:
                    return MetadataMapping;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum GisUploadAttemptWorkflowSectionGroupingEnum
    {
        GeospatialValidation = 1,
        MetadataMapping = 2
    }

    public partial class GisUploadAttemptWorkflowSectionGroupingGeospatialValidation : GisUploadAttemptWorkflowSectionGrouping
    {
        private GisUploadAttemptWorkflowSectionGroupingGeospatialValidation(int gisUploadAttemptWorkflowSectionGroupingID, string gisUploadAttemptWorkflowSectionGroupingName, string gisUploadAttemptWorkflowSectionGroupingDisplayName, int sortOrder) : base(gisUploadAttemptWorkflowSectionGroupingID, gisUploadAttemptWorkflowSectionGroupingName, gisUploadAttemptWorkflowSectionGroupingDisplayName, sortOrder) {}
        public static readonly GisUploadAttemptWorkflowSectionGroupingGeospatialValidation Instance = new GisUploadAttemptWorkflowSectionGroupingGeospatialValidation(1, @"GeospatialValidation", @"Geospatial Validation", 10);
    }

    public partial class GisUploadAttemptWorkflowSectionGroupingMetadataMapping : GisUploadAttemptWorkflowSectionGrouping
    {
        private GisUploadAttemptWorkflowSectionGroupingMetadataMapping(int gisUploadAttemptWorkflowSectionGroupingID, string gisUploadAttemptWorkflowSectionGroupingName, string gisUploadAttemptWorkflowSectionGroupingDisplayName, int sortOrder) : base(gisUploadAttemptWorkflowSectionGroupingID, gisUploadAttemptWorkflowSectionGroupingName, gisUploadAttemptWorkflowSectionGroupingDisplayName, sortOrder) {}
        public static readonly GisUploadAttemptWorkflowSectionGroupingMetadataMapping Instance = new GisUploadAttemptWorkflowSectionGroupingMetadataMapping(2, @"MetadataMapping", @"Metadata Mapping", 20);
    }
}