//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[JsonImportStatusType]
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
    public abstract partial class JsonImportStatusType : IHavePrimaryKey
    {
        public static readonly JsonImportStatusTypeNotYetProcessed NotYetProcessed = JsonImportStatusTypeNotYetProcessed.Instance;
        public static readonly JsonImportStatusTypeProcessingFailed ProcessingFailed = JsonImportStatusTypeProcessingFailed.Instance;
        public static readonly JsonImportStatusTypeProcessingSuceeded ProcessingSuceeded = JsonImportStatusTypeProcessingSuceeded.Instance;
        public static readonly JsonImportStatusTypeProcessingIndeterminate ProcessingIndeterminate = JsonImportStatusTypeProcessingIndeterminate.Instance;

        public static readonly List<JsonImportStatusType> All;
        public static readonly ReadOnlyDictionary<int, JsonImportStatusType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static JsonImportStatusType()
        {
            All = new List<JsonImportStatusType> { NotYetProcessed, ProcessingFailed, ProcessingSuceeded, ProcessingIndeterminate };
            AllLookupDictionary = new ReadOnlyDictionary<int, JsonImportStatusType>(All.ToDictionary(x => x.JsonImportStatusTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected JsonImportStatusType(int jsonImportStatusTypeID, string jsonImportStatusTypeName)
        {
            JsonImportStatusTypeID = jsonImportStatusTypeID;
            JsonImportStatusTypeName = jsonImportStatusTypeName;
        }

        [Key]
        public int JsonImportStatusTypeID { get; private set; }
        public string JsonImportStatusTypeName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return JsonImportStatusTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(JsonImportStatusType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.JsonImportStatusTypeID == JsonImportStatusTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as JsonImportStatusType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return JsonImportStatusTypeID;
        }

        public static bool operator ==(JsonImportStatusType left, JsonImportStatusType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(JsonImportStatusType left, JsonImportStatusType right)
        {
            return !Equals(left, right);
        }

        public JsonImportStatusTypeEnum ToEnum { get { return (JsonImportStatusTypeEnum)GetHashCode(); } }

        public static JsonImportStatusType ToType(int enumValue)
        {
            return ToType((JsonImportStatusTypeEnum)enumValue);
        }

        public static JsonImportStatusType ToType(JsonImportStatusTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case JsonImportStatusTypeEnum.NotYetProcessed:
                    return NotYetProcessed;
                case JsonImportStatusTypeEnum.ProcessingFailed:
                    return ProcessingFailed;
                case JsonImportStatusTypeEnum.ProcessingIndeterminate:
                    return ProcessingIndeterminate;
                case JsonImportStatusTypeEnum.ProcessingSuceeded:
                    return ProcessingSuceeded;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum JsonImportStatusTypeEnum
    {
        NotYetProcessed = 1,
        ProcessingFailed = 2,
        ProcessingSuceeded = 3,
        ProcessingIndeterminate = 4
    }

    public partial class JsonImportStatusTypeNotYetProcessed : JsonImportStatusType
    {
        private JsonImportStatusTypeNotYetProcessed(int jsonImportStatusTypeID, string jsonImportStatusTypeName) : base(jsonImportStatusTypeID, jsonImportStatusTypeName) {}
        public static readonly JsonImportStatusTypeNotYetProcessed Instance = new JsonImportStatusTypeNotYetProcessed(1, @"NotYetProcessed");
    }

    public partial class JsonImportStatusTypeProcessingFailed : JsonImportStatusType
    {
        private JsonImportStatusTypeProcessingFailed(int jsonImportStatusTypeID, string jsonImportStatusTypeName) : base(jsonImportStatusTypeID, jsonImportStatusTypeName) {}
        public static readonly JsonImportStatusTypeProcessingFailed Instance = new JsonImportStatusTypeProcessingFailed(2, @"ProcessingFailed");
    }

    public partial class JsonImportStatusTypeProcessingSuceeded : JsonImportStatusType
    {
        private JsonImportStatusTypeProcessingSuceeded(int jsonImportStatusTypeID, string jsonImportStatusTypeName) : base(jsonImportStatusTypeID, jsonImportStatusTypeName) {}
        public static readonly JsonImportStatusTypeProcessingSuceeded Instance = new JsonImportStatusTypeProcessingSuceeded(3, @"ProcessingSuceeded");
    }

    public partial class JsonImportStatusTypeProcessingIndeterminate : JsonImportStatusType
    {
        private JsonImportStatusTypeProcessingIndeterminate(int jsonImportStatusTypeID, string jsonImportStatusTypeName) : base(jsonImportStatusTypeID, jsonImportStatusTypeName) {}
        public static readonly JsonImportStatusTypeProcessingIndeterminate Instance = new JsonImportStatusTypeProcessingIndeterminate(4, @"ProcessingIndeterminate");
    }
}