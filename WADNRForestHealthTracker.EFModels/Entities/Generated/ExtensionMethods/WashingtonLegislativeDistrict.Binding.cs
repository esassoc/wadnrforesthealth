//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WashingtonLegislativeDistrict]
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
    // Table [dbo].[WashingtonLegislativeDistrict] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[WashingtonLegislativeDistrict]")]
    public partial class WashingtonLegislativeDistrict : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected WashingtonLegislativeDistrict()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public WashingtonLegislativeDistrict(int washingtonLegislativeDistrictID, DbGeometry washingtonLegislativeDistrictLocation, int washingtonLegislativeDistrictNumber, string washingtonLegislativeDistrictName) : this()
        {
            this.WashingtonLegislativeDistrictID = washingtonLegislativeDistrictID;
            this.WashingtonLegislativeDistrictLocation = washingtonLegislativeDistrictLocation;
            this.WashingtonLegislativeDistrictNumber = washingtonLegislativeDistrictNumber;
            this.WashingtonLegislativeDistrictName = washingtonLegislativeDistrictName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public WashingtonLegislativeDistrict(DbGeometry washingtonLegislativeDistrictLocation, int washingtonLegislativeDistrictNumber, string washingtonLegislativeDistrictName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.WashingtonLegislativeDistrictID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.WashingtonLegislativeDistrictLocation = washingtonLegislativeDistrictLocation;
            this.WashingtonLegislativeDistrictNumber = washingtonLegislativeDistrictNumber;
            this.WashingtonLegislativeDistrictName = washingtonLegislativeDistrictName;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static WashingtonLegislativeDistrict CreateNewBlank()
        {
            return new WashingtonLegislativeDistrict(default(DbGeometry), default(int), default(string));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(WashingtonLegislativeDistrict).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.WashingtonLegislativeDistricts.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int WashingtonLegislativeDistrictID { get; set; }
        public DbGeometry WashingtonLegislativeDistrictLocation { get; set; }
        public int WashingtonLegislativeDistrictNumber { get; set; }
        public string WashingtonLegislativeDistrictName { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return WashingtonLegislativeDistrictID; } set { WashingtonLegislativeDistrictID = value; } }



        public static class FieldLengths
        {
            public const int WashingtonLegislativeDistrictName = 200;
        }
    }
}