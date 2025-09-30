//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentUpdate]
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
    // Table [dbo].[TreatmentUpdate] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[TreatmentUpdate]")]
    public partial class TreatmentUpdate : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected TreatmentUpdate()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public TreatmentUpdate(int treatmentUpdateID, int projectUpdateBatchID, DateTime? treatmentStartDate, DateTime? treatmentEndDate, decimal treatmentFootprintAcres, string treatmentNotes, int treatmentTypeID, decimal? treatmentTreatedAcres, string treatmentTypeImportedText, int? createGisUploadAttemptID, int? updateGisUploadAttemptID, int treatmentDetailedActivityTypeID, string treatmentDetailedActivityTypeImportedText, int? programID, bool? importedFromGis, int? projectLocationUpdateID, int? treatmentCodeID, decimal? costPerAcre) : this()
        {
            this.TreatmentUpdateID = treatmentUpdateID;
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.TreatmentStartDate = treatmentStartDate;
            this.TreatmentEndDate = treatmentEndDate;
            this.TreatmentFootprintAcres = treatmentFootprintAcres;
            this.TreatmentNotes = treatmentNotes;
            this.TreatmentTypeID = treatmentTypeID;
            this.TreatmentTreatedAcres = treatmentTreatedAcres;
            this.TreatmentTypeImportedText = treatmentTypeImportedText;
            this.CreateGisUploadAttemptID = createGisUploadAttemptID;
            this.UpdateGisUploadAttemptID = updateGisUploadAttemptID;
            this.TreatmentDetailedActivityTypeID = treatmentDetailedActivityTypeID;
            this.TreatmentDetailedActivityTypeImportedText = treatmentDetailedActivityTypeImportedText;
            this.ProgramID = programID;
            this.ImportedFromGis = importedFromGis;
            this.ProjectLocationUpdateID = projectLocationUpdateID;
            this.TreatmentCodeID = treatmentCodeID;
            this.CostPerAcre = costPerAcre;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public TreatmentUpdate(int projectUpdateBatchID, decimal treatmentFootprintAcres, int treatmentTypeID, int treatmentDetailedActivityTypeID) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.TreatmentUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProjectUpdateBatchID = projectUpdateBatchID;
            this.TreatmentFootprintAcres = treatmentFootprintAcres;
            this.TreatmentTypeID = treatmentTypeID;
            this.TreatmentDetailedActivityTypeID = treatmentDetailedActivityTypeID;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public TreatmentUpdate(ProjectUpdateBatch projectUpdateBatch, decimal treatmentFootprintAcres, TreatmentType treatmentType, TreatmentDetailedActivityType treatmentDetailedActivityType) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.TreatmentUpdateID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProjectUpdateBatchID = projectUpdateBatch.ProjectUpdateBatchID;
            this.ProjectUpdateBatch = projectUpdateBatch;
            projectUpdateBatch.TreatmentUpdates.Add(this);
            this.TreatmentFootprintAcres = treatmentFootprintAcres;
            this.TreatmentTypeID = treatmentType.TreatmentTypeID;
            this.TreatmentDetailedActivityTypeID = treatmentDetailedActivityType.TreatmentDetailedActivityTypeID;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static TreatmentUpdate CreateNewBlank(ProjectUpdateBatch projectUpdateBatch, TreatmentType treatmentType, TreatmentDetailedActivityType treatmentDetailedActivityType)
        {
            return new TreatmentUpdate(projectUpdateBatch, default(decimal), treatmentType, treatmentDetailedActivityType);
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(TreatmentUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.TreatmentUpdates.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int TreatmentUpdateID { get; set; }
        public int ProjectUpdateBatchID { get; set; }
        public DateTime? TreatmentStartDate { get; set; }
        public DateTime? TreatmentEndDate { get; set; }
        public decimal TreatmentFootprintAcres { get; set; }
        public string TreatmentNotes { get; set; }
        public int TreatmentTypeID { get; set; }
        public decimal? TreatmentTreatedAcres { get; set; }
        public string TreatmentTypeImportedText { get; set; }
        public int? CreateGisUploadAttemptID { get; set; }
        public int? UpdateGisUploadAttemptID { get; set; }
        public int TreatmentDetailedActivityTypeID { get; set; }
        public string TreatmentDetailedActivityTypeImportedText { get; set; }
        public int? ProgramID { get; set; }
        public bool? ImportedFromGis { get; set; }
        public int? ProjectLocationUpdateID { get; set; }
        public int? TreatmentCodeID { get; set; }
        public decimal? CostPerAcre { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return TreatmentUpdateID; } set { TreatmentUpdateID = value; } }

        public virtual ProjectUpdateBatch ProjectUpdateBatch { get; set; }
        public TreatmentType TreatmentType { get { return TreatmentType.AllLookupDictionary[TreatmentTypeID]; } }
        public virtual GisUploadAttempt CreateGisUploadAttempt { get; set; }
        public virtual GisUploadAttempt UpdateGisUploadAttempt { get; set; }
        public TreatmentDetailedActivityType TreatmentDetailedActivityType { get { return TreatmentDetailedActivityType.AllLookupDictionary[TreatmentDetailedActivityTypeID]; } }
        public virtual Program Program { get; set; }
        public virtual ProjectLocationUpdate ProjectLocationUpdate { get; set; }
        public virtual TreatmentCode TreatmentCode { get; set; }

        public static class FieldLengths
        {
            public const int TreatmentNotes = 2000;
            public const int TreatmentTypeImportedText = 200;
            public const int TreatmentDetailedActivityTypeImportedText = 200;
        }
    }
}