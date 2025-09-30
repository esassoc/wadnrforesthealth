//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentArea]
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
    // Table [dbo].[TreatmentArea] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[TreatmentArea]")]
    public partial class TreatmentArea : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected TreatmentArea()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public TreatmentArea(int treatmentAreaID, DbGeometry treatmentAreaFeature, int? temporaryTreatmentCacheID) : this()
        {
            this.TreatmentAreaID = treatmentAreaID;
            this.TreatmentAreaFeature = treatmentAreaFeature;
            this.TemporaryTreatmentCacheID = temporaryTreatmentCacheID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public TreatmentArea(DbGeometry treatmentAreaFeature) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.TreatmentAreaID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.TreatmentAreaFeature = treatmentAreaFeature;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static TreatmentArea CreateNewBlank()
        {
            return new TreatmentArea(default(DbGeometry));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(TreatmentArea).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.TreatmentAreas.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int TreatmentAreaID { get; set; }
        public DbGeometry TreatmentAreaFeature { get; set; }
        public int? TemporaryTreatmentCacheID { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return TreatmentAreaID; } set { TreatmentAreaID = value; } }



        public static class FieldLengths
        {

        }
    }
}