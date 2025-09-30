//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramNotificationType]
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
    public abstract partial class ProgramNotificationType : IHavePrimaryKey
    {
        public static readonly ProgramNotificationTypeCompletedProjectsMaintenanceReminder CompletedProjectsMaintenanceReminder = ProgramNotificationTypeCompletedProjectsMaintenanceReminder.Instance;

        public static readonly List<ProgramNotificationType> All;
        public static readonly ReadOnlyDictionary<int, ProgramNotificationType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ProgramNotificationType()
        {
            All = new List<ProgramNotificationType> { CompletedProjectsMaintenanceReminder };
            AllLookupDictionary = new ReadOnlyDictionary<int, ProgramNotificationType>(All.ToDictionary(x => x.ProgramNotificationTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ProgramNotificationType(int programNotificationTypeID, string programNotificationTypeName, string programNotificationTypeDisplayName)
        {
            ProgramNotificationTypeID = programNotificationTypeID;
            ProgramNotificationTypeName = programNotificationTypeName;
            ProgramNotificationTypeDisplayName = programNotificationTypeDisplayName;
        }

        [Key]
        public int ProgramNotificationTypeID { get; private set; }
        public string ProgramNotificationTypeName { get; private set; }
        public string ProgramNotificationTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ProgramNotificationTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ProgramNotificationType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ProgramNotificationTypeID == ProgramNotificationTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ProgramNotificationType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ProgramNotificationTypeID;
        }

        public static bool operator ==(ProgramNotificationType left, ProgramNotificationType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProgramNotificationType left, ProgramNotificationType right)
        {
            return !Equals(left, right);
        }

        public ProgramNotificationTypeEnum ToEnum { get { return (ProgramNotificationTypeEnum)GetHashCode(); } }

        public static ProgramNotificationType ToType(int enumValue)
        {
            return ToType((ProgramNotificationTypeEnum)enumValue);
        }

        public static ProgramNotificationType ToType(ProgramNotificationTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ProgramNotificationTypeEnum.CompletedProjectsMaintenanceReminder:
                    return CompletedProjectsMaintenanceReminder;
                default:
                    throw new ArgumentException(string.Format("Unable to map Enum: {0}", enumValue));
            }
        }
    }

    public enum ProgramNotificationTypeEnum
    {
        CompletedProjectsMaintenanceReminder = 1
    }

    public partial class ProgramNotificationTypeCompletedProjectsMaintenanceReminder : ProgramNotificationType
    {
        private ProgramNotificationTypeCompletedProjectsMaintenanceReminder(int programNotificationTypeID, string programNotificationTypeName, string programNotificationTypeDisplayName) : base(programNotificationTypeID, programNotificationTypeName, programNotificationTypeDisplayName) {}
        public static readonly ProgramNotificationTypeCompletedProjectsMaintenanceReminder Instance = new ProgramNotificationTypeCompletedProjectsMaintenanceReminder(1, @"CompletedProjectsMaintenanceReminder", @"Completed Projects Maintenance Reminder");
    }
}