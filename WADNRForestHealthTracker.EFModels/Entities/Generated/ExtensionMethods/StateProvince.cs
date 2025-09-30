//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[StateProvince]
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
    // Table [dbo].[StateProvince] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[StateProvince]")]
    public partial class StateProvince : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected StateProvince()
        {
            this.Counties = new HashSet<County>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public StateProvince(int stateProvinceID, string stateProvinceAbbreviation, string stateProvinceName, bool isBpaRelevant, decimal? southWestLatitude, decimal? southWestLongitude, decimal? northEastLatitude, decimal? northEastLongitude, int? mapObjectID, DbGeometry stateProvinceFeature, int countryID) : this()
        {
            this.StateProvinceID = stateProvinceID;
            this.StateProvinceAbbreviation = stateProvinceAbbreviation;
            this.StateProvinceName = stateProvinceName;
            this.IsBpaRelevant = isBpaRelevant;
            this.SouthWestLatitude = southWestLatitude;
            this.SouthWestLongitude = southWestLongitude;
            this.NorthEastLatitude = northEastLatitude;
            this.NorthEastLongitude = northEastLongitude;
            this.MapObjectID = mapObjectID;
            this.StateProvinceFeature = stateProvinceFeature;
            this.CountryID = countryID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public StateProvince(string stateProvinceAbbreviation, string stateProvinceName, bool isBpaRelevant, int countryID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.StateProvinceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.StateProvinceAbbreviation = stateProvinceAbbreviation;
            this.StateProvinceName = stateProvinceName;
            this.IsBpaRelevant = isBpaRelevant;
            this.CountryID = countryID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public StateProvince(string stateProvinceAbbreviation, string stateProvinceName, bool isBpaRelevant, Country country) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.StateProvinceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.StateProvinceAbbreviation = stateProvinceAbbreviation;
            this.StateProvinceName = stateProvinceName;
            this.IsBpaRelevant = isBpaRelevant;
            this.CountryID = country.CountryID;
            this.Country = country;
            country.StateProvinces.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static StateProvince CreateNewBlank(Country country)
        {
            return new StateProvince(default(string), default(string), default(bool), country);
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return Counties.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(Counties.Any())
            {
                dependentObjects.Add(typeof(County).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(StateProvince).Name, typeof(County).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.StateProvinces.Remove(this);
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

            foreach(var x in Counties.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int StateProvinceID { get; set; }
        public string StateProvinceAbbreviation { get; set; }
        public string StateProvinceName { get; set; }
        public bool IsBpaRelevant { get; set; }
        public decimal? SouthWestLatitude { get; set; }
        public decimal? SouthWestLongitude { get; set; }
        public decimal? NorthEastLatitude { get; set; }
        public decimal? NorthEastLongitude { get; set; }
        public int? MapObjectID { get; set; }
        public DbGeometry StateProvinceFeature { get; set; }
        public int CountryID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return StateProvinceID; } set { StateProvinceID = value; } }

        public virtual ICollection<County> Counties { get; set; }
        public virtual Country Country { get; set; }

        public static class FieldLengths
        {
            public const int StateProvinceAbbreviation = 2;
            public const int StateProvinceName = 50;
        }
    }
}