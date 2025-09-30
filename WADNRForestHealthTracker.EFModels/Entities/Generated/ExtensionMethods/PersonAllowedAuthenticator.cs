//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[PersonAllowedAuthenticator]
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
    // Table [dbo].[PersonAllowedAuthenticator] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[PersonAllowedAuthenticator]")]
    public partial class PersonAllowedAuthenticator : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected PersonAllowedAuthenticator()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public PersonAllowedAuthenticator(int personAllowedAuthenticatorID, int personID, int authenticatorID) : this()
        {
            this.PersonAllowedAuthenticatorID = personAllowedAuthenticatorID;
            this.PersonID = personID;
            this.AuthenticatorID = authenticatorID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public PersonAllowedAuthenticator(int personID, int authenticatorID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.PersonAllowedAuthenticatorID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.PersonID = personID;
            this.AuthenticatorID = authenticatorID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public PersonAllowedAuthenticator(Person person, Authenticator authenticator) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.PersonAllowedAuthenticatorID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.PersonID = person.PersonID;
            this.Person = person;
            person.PersonAllowedAuthenticators.Add(this);
            this.AuthenticatorID = authenticator.AuthenticatorID;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static PersonAllowedAuthenticator CreateNewBlank(Person person, Authenticator authenticator)
        {
            return new PersonAllowedAuthenticator(person, authenticator);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(PersonAllowedAuthenticator).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.PersonAllowedAuthenticators.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int PersonAllowedAuthenticatorID { get; set; }
        public int PersonID { get; set; }
        public int AuthenticatorID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return PersonAllowedAuthenticatorID; } set { PersonAllowedAuthenticatorID = value; } }

        public virtual Person Person { get; set; }
        public Authenticator Authenticator { get { return Authenticator.AllLookupDictionary[AuthenticatorID]; } }

        public static class FieldLengths
        {

        }
    }
}