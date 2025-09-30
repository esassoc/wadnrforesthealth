//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FocusArea]
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
    // Table [dbo].[FocusArea] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FocusArea]")]
    public partial class FocusArea : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FocusArea()
        {
            this.FocusAreaLocationStagings = new HashSet<FocusAreaLocationStaging>();
            this.Projects = new HashSet<Project>();
            this.ProjectUpdates = new HashSet<ProjectUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FocusArea(int focusAreaID, string focusAreaName, int focusAreaStatusID, DbGeometry focusAreaLocation, int dNRUplandRegionID, decimal? plannedFootprintAcres) : this()
        {
            this.FocusAreaID = focusAreaID;
            this.FocusAreaName = focusAreaName;
            this.FocusAreaStatusID = focusAreaStatusID;
            this.FocusAreaLocation = focusAreaLocation;
            this.DNRUplandRegionID = dNRUplandRegionID;
            this.PlannedFootprintAcres = plannedFootprintAcres;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FocusArea(string focusAreaName, int focusAreaStatusID, int dNRUplandRegionID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FocusAreaID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FocusAreaName = focusAreaName;
            this.FocusAreaStatusID = focusAreaStatusID;
            this.DNRUplandRegionID = dNRUplandRegionID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FocusArea(string focusAreaName, FocusAreaStatus focusAreaStatus, DNRUplandRegion dNRUplandRegion) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FocusAreaID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FocusAreaName = focusAreaName;
            this.FocusAreaStatusID = focusAreaStatus.FocusAreaStatusID;
            this.DNRUplandRegionID = dNRUplandRegion.DNRUplandRegionID;
            this.DNRUplandRegion = dNRUplandRegion;
            dNRUplandRegion.FocusAreas.Add(this);
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FocusArea CreateNewBlank(FocusAreaStatus focusAreaStatus, DNRUplandRegion dNRUplandRegion)
        {
            return new FocusArea(default(string), focusAreaStatus, dNRUplandRegion);
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return FocusAreaLocationStagings.Any() || Projects.Any() || ProjectUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(FocusAreaLocationStagings.Any())
            {
                dependentObjects.Add(typeof(FocusAreaLocationStaging).Name);
            }

            if(Projects.Any())
            {
                dependentObjects.Add(typeof(Project).Name);
            }

            if(ProjectUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FocusArea).Name, typeof(FocusAreaLocationStaging).Name, typeof(Project).Name, typeof(ProjectUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FocusAreas.Remove(this);
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

            foreach(var x in FocusAreaLocationStagings.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in Projects.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int FocusAreaID { get; set; }
        public string FocusAreaName { get; set; }
        public int FocusAreaStatusID { get; set; }
        public DbGeometry FocusAreaLocation { get; set; }
        public int DNRUplandRegionID { get; set; }
        public decimal? PlannedFootprintAcres { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FocusAreaID; } set { FocusAreaID = value; } }

        public virtual ICollection<FocusAreaLocationStaging> FocusAreaLocationStagings { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<ProjectUpdate> ProjectUpdates { get; set; }
        public FocusAreaStatus FocusAreaStatus { get { return FocusAreaStatus.AllLookupDictionary[FocusAreaStatusID]; } }
        public virtual DNRUplandRegion DNRUplandRegion { get; set; }

        public static class FieldLengths
        {
            public const int FocusAreaName = 200;
        }
    }
}