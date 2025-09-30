//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEvent]
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
    // Table [dbo].[InteractionEvent] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[InteractionEvent]")]
    public partial class InteractionEvent : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected InteractionEvent()
        {
            this.InteractionEventContacts = new HashSet<InteractionEventContact>();
            this.InteractionEventFileResources = new HashSet<InteractionEventFileResource>();
            this.InteractionEventProjects = new HashSet<InteractionEventProject>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public InteractionEvent(int interactionEventID, int interactionEventTypeID, int? staffPersonID, string interactionEventTitle, string interactionEventDescription, DateTime interactionEventDate, DbGeometry interactionEventLocationSimple) : this()
        {
            this.InteractionEventID = interactionEventID;
            this.InteractionEventTypeID = interactionEventTypeID;
            this.StaffPersonID = staffPersonID;
            this.InteractionEventTitle = interactionEventTitle;
            this.InteractionEventDescription = interactionEventDescription;
            this.InteractionEventDate = interactionEventDate;
            this.InteractionEventLocationSimple = interactionEventLocationSimple;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public InteractionEvent(int interactionEventTypeID, string interactionEventTitle, DateTime interactionEventDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InteractionEventID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.InteractionEventTypeID = interactionEventTypeID;
            this.InteractionEventTitle = interactionEventTitle;
            this.InteractionEventDate = interactionEventDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public InteractionEvent(InteractionEventType interactionEventType, string interactionEventTitle, DateTime interactionEventDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InteractionEventID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.InteractionEventTypeID = interactionEventType.InteractionEventTypeID;
            this.InteractionEventType = interactionEventType;
            interactionEventType.InteractionEvents.Add(this);
            this.InteractionEventTitle = interactionEventTitle;
            this.InteractionEventDate = interactionEventDate;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static InteractionEvent CreateNewBlank(InteractionEventType interactionEventType)
        {
            return new InteractionEvent(interactionEventType, default(string), default(DateTime));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return InteractionEventContacts.Any() || InteractionEventFileResources.Any() || InteractionEventProjects.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(InteractionEventContacts.Any())
            {
                dependentObjects.Add(typeof(InteractionEventContact).Name);
            }

            if(InteractionEventFileResources.Any())
            {
                dependentObjects.Add(typeof(InteractionEventFileResource).Name);
            }

            if(InteractionEventProjects.Any())
            {
                dependentObjects.Add(typeof(InteractionEventProject).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(InteractionEvent).Name, typeof(InteractionEventContact).Name, typeof(InteractionEventFileResource).Name, typeof(InteractionEventProject).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.InteractionEvents.Remove(this);
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

            foreach(var x in InteractionEventContacts.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in InteractionEventFileResources.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in InteractionEventProjects.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int InteractionEventID { get; set; }
        public int InteractionEventTypeID { get; set; }
        public int? StaffPersonID { get; set; }
        public string InteractionEventTitle { get; set; }
        public string InteractionEventDescription { get; set; }
        public DateTime InteractionEventDate { get; set; }
        public DbGeometry InteractionEventLocationSimple { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return InteractionEventID; } set { InteractionEventID = value; } }

        public virtual ICollection<InteractionEventContact> InteractionEventContacts { get; set; }
        public virtual ICollection<InteractionEventFileResource> InteractionEventFileResources { get; set; }
        public virtual ICollection<InteractionEventProject> InteractionEventProjects { get; set; }
        public virtual InteractionEventType InteractionEventType { get; set; }
        public virtual Person StaffPerson { get; set; }

        public static class FieldLengths
        {
            public const int InteractionEventTitle = 255;
            public const int InteractionEventDescription = 3000;
        }
    }
}