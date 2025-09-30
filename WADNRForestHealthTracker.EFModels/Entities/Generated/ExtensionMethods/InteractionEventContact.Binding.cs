//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[InteractionEventContact]
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
    // Table [dbo].[InteractionEventContact] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[InteractionEventContact]")]
    public partial class InteractionEventContact : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected InteractionEventContact()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public InteractionEventContact(int interactionEventContactID, int interactionEventID, int personID) : this()
        {
            this.InteractionEventContactID = interactionEventContactID;
            this.InteractionEventID = interactionEventID;
            this.PersonID = personID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public InteractionEventContact(int interactionEventID, int personID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InteractionEventContactID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.InteractionEventID = interactionEventID;
            this.PersonID = personID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public InteractionEventContact(InteractionEvent interactionEvent, Person person) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.InteractionEventContactID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.InteractionEventID = interactionEvent.InteractionEventID;
            this.InteractionEvent = interactionEvent;
            interactionEvent.InteractionEventContacts.Add(this);
            this.PersonID = person.PersonID;
            this.Person = person;
            person.InteractionEventContacts.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static InteractionEventContact CreateNewBlank(InteractionEvent interactionEvent, Person person)
        {
            return new InteractionEventContact(interactionEvent, person);
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return false;
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(InteractionEventContact).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.InteractionEventContacts.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int InteractionEventContactID { get; set; }
        public int InteractionEventID { get; set; }
        public int PersonID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return InteractionEventContactID; } set { InteractionEventContactID = value; } }

        public virtual InteractionEvent InteractionEvent { get; set; }
        public virtual Person Person { get; set; }

        public static class FieldLengths
        {

        }
    }
}