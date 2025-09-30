//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementPerson]
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
    // Table [dbo].[AgreementPerson] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[AgreementPerson]")]
    public partial class AgreementPerson : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected AgreementPerson()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public AgreementPerson(int agreementPersonID, int agreementID, int personID, int agreementPersonRoleID) : this()
        {
            this.AgreementPersonID = agreementPersonID;
            this.AgreementID = agreementID;
            this.PersonID = personID;
            this.AgreementPersonRoleID = agreementPersonRoleID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public AgreementPerson(int agreementID, int personID, int agreementPersonRoleID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.AgreementPersonID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.AgreementID = agreementID;
            this.PersonID = personID;
            this.AgreementPersonRoleID = agreementPersonRoleID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public AgreementPerson(Agreement agreement, Person person, AgreementPersonRole agreementPersonRole) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.AgreementPersonID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.AgreementID = agreement.AgreementID;
            this.Agreement = agreement;
            agreement.AgreementPeople.Add(this);
            this.PersonID = person.PersonID;
            this.Person = person;
            person.AgreementPeople.Add(this);
            this.AgreementPersonRoleID = agreementPersonRole.AgreementPersonRoleID;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static AgreementPerson CreateNewBlank(Agreement agreement, Person person, AgreementPersonRole agreementPersonRole)
        {
            return new AgreementPerson(agreement, person, agreementPersonRole);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(AgreementPerson).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.AgreementPeople.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int AgreementPersonID { get; set; }
        public int AgreementID { get; set; }
        public int PersonID { get; set; }
        public int AgreementPersonRoleID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return AgreementPersonID; } set { AgreementPersonID = value; } }

        public virtual Agreement Agreement { get; set; }
        public virtual Person Person { get; set; }
        public AgreementPersonRole AgreementPersonRole { get { return AgreementPersonRole.AllLookupDictionary[AgreementPersonRoleID]; } }

        public static class FieldLengths
        {

        }
    }
}