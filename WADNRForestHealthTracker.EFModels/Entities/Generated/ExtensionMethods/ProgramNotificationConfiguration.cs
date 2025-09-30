//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramNotificationConfiguration]
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    // Table [dbo].[ProgramNotificationConfiguration] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProgramNotificationConfiguration]")]
    public partial class ProgramNotificationConfiguration : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProgramNotificationConfiguration()
        {
            this.ProgramNotificationSents = new HashSet<ProgramNotificationSent>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProgramNotificationConfiguration(int programNotificationConfigurationID, int programID, int programNotificationTypeID, int recurrenceIntervalID, string notificationEmailText) : this()
        {
            this.ProgramNotificationConfigurationID = programNotificationConfigurationID;
            this.ProgramID = programID;
            this.ProgramNotificationTypeID = programNotificationTypeID;
            this.RecurrenceIntervalID = recurrenceIntervalID;
            this.NotificationEmailText = notificationEmailText;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProgramNotificationConfiguration(int programID, int programNotificationTypeID, int recurrenceIntervalID, string notificationEmailText) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramNotificationConfigurationID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProgramID = programID;
            this.ProgramNotificationTypeID = programNotificationTypeID;
            this.RecurrenceIntervalID = recurrenceIntervalID;
            this.NotificationEmailText = notificationEmailText;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProgramNotificationConfiguration(Program program, ProgramNotificationType programNotificationType, RecurrenceInterval recurrenceInterval, string notificationEmailText) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramNotificationConfigurationID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProgramID = program.ProgramID;
            this.Program = program;
            program.ProgramNotificationConfigurations.Add(this);
            this.ProgramNotificationTypeID = programNotificationType.ProgramNotificationTypeID;
            this.RecurrenceIntervalID = recurrenceInterval.RecurrenceIntervalID;
            this.NotificationEmailText = notificationEmailText;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProgramNotificationConfiguration CreateNewBlank(Program program, ProgramNotificationType programNotificationType, RecurrenceInterval recurrenceInterval)
        {
            return new ProgramNotificationConfiguration(program, programNotificationType, recurrenceInterval, default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return ProgramNotificationSents.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(ProgramNotificationSents.Any())
            {
                dependentObjects.Add(typeof(ProgramNotificationSent).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProgramNotificationConfiguration).Name, typeof(ProgramNotificationSent).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProgramNotificationConfigurations.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            DeleteChildren(dbContext);
            Delete(dbContext);
        }
        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public void DeleteChildren(DatabaseEntities dbContext)
        {

            foreach(var x in ProgramNotificationSents.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProgramNotificationConfigurationID { get; set; }
        public int ProgramID { get; set; }
        public int ProgramNotificationTypeID { get; set; }
        public int RecurrenceIntervalID { get; set; }
        public string NotificationEmailText { get; set; }
        [NotMapped]
        public HtmlString NotificationEmailTextHtmlString
        { 
            get { return NotificationEmailText == null ? null : new HtmlString(NotificationEmailText); }
            set { NotificationEmailText = value?.ToString(); }
        }
        [NotMapped]
        public int PrimaryKey { get { return ProgramNotificationConfigurationID; } set { ProgramNotificationConfigurationID = value; } }

        public virtual ICollection<ProgramNotificationSent> ProgramNotificationSents { get; set; }
        public virtual Program Program { get; set; }
        public ProgramNotificationType ProgramNotificationType { get { return ProgramNotificationType.AllLookupDictionary[ProgramNotificationTypeID]; } }
        public RecurrenceInterval RecurrenceInterval { get { return RecurrenceInterval.AllLookupDictionary[RecurrenceIntervalID]; } }

        public static class FieldLengths
        {

        }
    }
}