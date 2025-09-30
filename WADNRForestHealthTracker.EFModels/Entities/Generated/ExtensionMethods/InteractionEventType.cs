//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEventType]
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
    // Table [dbo].[InteractionEventType] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[InteractionEventType]")]
    public partial class InteractionEventType : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected InteractionEventType()
        {
            this.InteractionEvents = new HashSet<InteractionEvent>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public InteractionEventType(int interactionEventTypeID, string interactionEventTypeName, string interactionEventTypeDisplayName) : this()
        {
            this.InteractionEventTypeID = interactionEventTypeID;
            this.InteractionEventTypeName = interactionEventTypeName;
            this.InteractionEventTypeDisplayName = interactionEventTypeDisplayName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public InteractionEventType(string interactionEventTypeName, string interactionEventTypeDisplayName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InteractionEventTypeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.InteractionEventTypeName = interactionEventTypeName;
            this.InteractionEventTypeDisplayName = interactionEventTypeDisplayName;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static InteractionEventType CreateNewBlank()
        {
            return new InteractionEventType(default(string), default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return InteractionEvents.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(InteractionEvents.Any())
            {
                dependentObjects.Add(typeof(InteractionEvent).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(InteractionEventType).Name, typeof(InteractionEvent).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.InteractionEventTypes.Remove(this);
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

            foreach(var x in InteractionEvents.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int InteractionEventTypeID { get; set; }
        public string InteractionEventTypeName { get; set; }
        public string InteractionEventTypeDisplayName { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return InteractionEventTypeID; } set { InteractionEventTypeID = value; } }

        public virtual ICollection<InteractionEvent> InteractionEvents { get; set; }

        public static class FieldLengths
        {
            public const int InteractionEventTypeName = 200;
            public const int InteractionEventTypeDisplayName = 255;
        }
    }
}