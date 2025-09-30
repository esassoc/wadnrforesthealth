//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[County]
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
    // Table [dbo].[County] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[County]")]
    public partial class County : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected County()
        {
            this.ProjectCounties = new HashSet<ProjectCounty>();
            this.ProjectCountyUpdates = new HashSet<ProjectCountyUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public County(string countyName, int stateProvinceID, DbGeometry countyFeature, int countyID) : this()
        {
            this.CountyName = countyName;
            this.StateProvinceID = stateProvinceID;
            this.CountyFeature = countyFeature;
            this.CountyID = countyID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public County(string countyName, int stateProvinceID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.CountyID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.CountyName = countyName;
            this.StateProvinceID = stateProvinceID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public County(string countyName, StateProvince stateProvince) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.CountyID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.CountyName = countyName;
            this.StateProvinceID = stateProvince.StateProvinceID;
            this.StateProvince = stateProvince;
            stateProvince.Counties.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static County CreateNewBlank(StateProvince stateProvince)
        {
            return new County(default(string), stateProvince);
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return ProjectCounties.Any() || ProjectCountyUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(ProjectCounties.Any())
            {
                dependentObjects.Add(typeof(ProjectCounty).Name);
            }

            if(ProjectCountyUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectCountyUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(County).Name, typeof(ProjectCounty).Name, typeof(ProjectCountyUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.Counties.Remove(this);
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

            foreach(var x in ProjectCounties.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectCountyUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        public string CountyName { get; set; }
        public int StateProvinceID { get; set; }
        public DbGeometry CountyFeature { get; set; }
        [Key]
        public int CountyID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return CountyID; } set { CountyID = value; } }

        public virtual ICollection<ProjectCounty> ProjectCounties { get; set; }
        public virtual ICollection<ProjectCountyUpdate> ProjectCountyUpdates { get; set; }
        public virtual StateProvince StateProvince { get; set; }

        public static class FieldLengths
        {
            public const int CountyName = 100;
        }
    }
}