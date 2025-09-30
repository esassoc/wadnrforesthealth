//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Country]
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
    // Table [dbo].[Country] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[Country]")]
    public partial class Country : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected Country()
        {
            this.StateProvinces = new HashSet<StateProvince>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public Country(int countryID, string countryName, string countryAbbrev) : this()
        {
            this.CountryID = countryID;
            this.CountryName = countryName;
            this.CountryAbbrev = countryAbbrev;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public Country(string countryName, string countryAbbrev) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.CountryID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.CountryName = countryName;
            this.CountryAbbrev = countryAbbrev;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static Country CreateNewBlank()
        {
            return new Country(default(string), default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return StateProvinces.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(StateProvinces.Any())
            {
                dependentObjects.Add(typeof(StateProvince).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(Country).Name, typeof(StateProvince).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.Countries.Remove(this);
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

            foreach(var x in StateProvinces.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string CountryAbbrev { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return CountryID; } set { CountryID = value; } }

        public virtual ICollection<StateProvince> StateProvinces { get; set; }

        public static class FieldLengths
        {
            public const int CountryName = 255;
            public const int CountryAbbrev = 2;
        }
    }
}