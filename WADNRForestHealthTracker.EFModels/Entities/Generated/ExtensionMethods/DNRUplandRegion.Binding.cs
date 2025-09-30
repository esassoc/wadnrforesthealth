//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[DNRUplandRegion]
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
    // Table [dbo].[DNRUplandRegion] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[DNRUplandRegion]")]
    public partial class DNRUplandRegion : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected DNRUplandRegion()
        {
            this.Agreements = new HashSet<Agreement>();
            this.DNRUplandRegionContentImages = new HashSet<DNRUplandRegionContentImage>();
            this.FocusAreas = new HashSet<FocusArea>();
            this.FundSourceAllocations = new HashSet<FundSourceAllocation>();
            this.PersonStewardRegions = new HashSet<PersonStewardRegion>();
            this.ProjectRegions = new HashSet<ProjectRegion>();
            this.ProjectRegionUpdates = new HashSet<ProjectRegionUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public DNRUplandRegion(int dNRUplandRegionID, string dNRUplandRegionAbbrev, string dNRUplandRegionName, DbGeometry dNRUplandRegionLocation, string regionAddress, string regionCity, string regionState, string regionZip, string regionPhone, string regionEmail, int? dNRUplandRegionCoordinatorID, string regionContent) : this()
        {
            this.DNRUplandRegionID = dNRUplandRegionID;
            this.DNRUplandRegionAbbrev = dNRUplandRegionAbbrev;
            this.DNRUplandRegionName = dNRUplandRegionName;
            this.DNRUplandRegionLocation = dNRUplandRegionLocation;
            this.RegionAddress = regionAddress;
            this.RegionCity = regionCity;
            this.RegionState = regionState;
            this.RegionZip = regionZip;
            this.RegionPhone = regionPhone;
            this.RegionEmail = regionEmail;
            this.DNRUplandRegionCoordinatorID = dNRUplandRegionCoordinatorID;
            this.RegionContent = regionContent;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public DNRUplandRegion(string dNRUplandRegionName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.DNRUplandRegionID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.DNRUplandRegionName = dNRUplandRegionName;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static DNRUplandRegion CreateNewBlank()
        {
            return new DNRUplandRegion(default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return Agreements.Any() || DNRUplandRegionContentImages.Any() || FocusAreas.Any() || FundSourceAllocations.Any() || PersonStewardRegions.Any() || ProjectRegions.Any() || ProjectRegionUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(Agreements.Any())
            {
                dependentObjects.Add(typeof(Agreement).Name);
            }

            if(DNRUplandRegionContentImages.Any())
            {
                dependentObjects.Add(typeof(DNRUplandRegionContentImage).Name);
            }

            if(FocusAreas.Any())
            {
                dependentObjects.Add(typeof(FocusArea).Name);
            }

            if(FundSourceAllocations.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocation).Name);
            }

            if(PersonStewardRegions.Any())
            {
                dependentObjects.Add(typeof(PersonStewardRegion).Name);
            }

            if(ProjectRegions.Any())
            {
                dependentObjects.Add(typeof(ProjectRegion).Name);
            }

            if(ProjectRegionUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectRegionUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(DNRUplandRegion).Name, typeof(Agreement).Name, typeof(DNRUplandRegionContentImage).Name, typeof(FocusArea).Name, typeof(FundSourceAllocation).Name, typeof(PersonStewardRegion).Name, typeof(ProjectRegion).Name, typeof(ProjectRegionUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.DNRUplandRegions.Remove(this);
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

            foreach(var x in Agreements.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in DNRUplandRegionContentImages.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FocusAreas.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PersonStewardRegions.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectRegions.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectRegionUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int DNRUplandRegionID { get; set; }
        public string DNRUplandRegionAbbrev { get; set; }
        public string DNRUplandRegionName { get; set; }
        public DbGeometry DNRUplandRegionLocation { get; set; }
        public string RegionAddress { get; set; }
        public string RegionCity { get; set; }
        public string RegionState { get; set; }
        public string RegionZip { get; set; }
        public string RegionPhone { get; set; }
        public string RegionEmail { get; set; }
        public int? DNRUplandRegionCoordinatorID { get; set; }
        public string RegionContent { get; set; }
        [NotMapped]
        public HtmlString RegionContentHtmlString
        { 
            get { return RegionContent == null ? null : new HtmlString(RegionContent); }
            set { RegionContent = value?.ToString(); }
        }
        [NotMapped]
        public int PrimaryKey { get { return DNRUplandRegionID; } set { DNRUplandRegionID = value; } }

        public virtual ICollection<Agreement> Agreements { get; set; }
        public virtual ICollection<DNRUplandRegionContentImage> DNRUplandRegionContentImages { get; set; }
        public virtual ICollection<FocusArea> FocusAreas { get; set; }
        public virtual ICollection<FundSourceAllocation> FundSourceAllocations { get; set; }
        public virtual ICollection<PersonStewardRegion> PersonStewardRegions { get; set; }
        public virtual ICollection<ProjectRegion> ProjectRegions { get; set; }
        public virtual ICollection<ProjectRegionUpdate> ProjectRegionUpdates { get; set; }
        public virtual Person DNRUplandRegionCoordinator { get; set; }

        public static class FieldLengths
        {
            public const int DNRUplandRegionAbbrev = 10;
            public const int DNRUplandRegionName = 100;
            public const int RegionAddress = 255;
            public const int RegionCity = 30;
            public const int RegionState = 30;
            public const int RegionZip = 10;
            public const int RegionPhone = 30;
            public const int RegionEmail = 255;
        }
    }
}