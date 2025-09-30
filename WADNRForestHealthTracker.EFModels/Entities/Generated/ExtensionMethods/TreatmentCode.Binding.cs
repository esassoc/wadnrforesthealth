//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentCode]
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
    // Table [dbo].[TreatmentCode] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[TreatmentCode]")]
    public partial class TreatmentCode : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected TreatmentCode()
        {
            this.Treatments = new HashSet<Treatment>();
            this.TreatmentUpdates = new HashSet<TreatmentUpdate>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public TreatmentCode(int treatmentCodeID, string treatmentCodeName, string treatmentCodeDisplayName) : this()
        {
            this.TreatmentCodeID = treatmentCodeID;
            this.TreatmentCodeName = treatmentCodeName;
            this.TreatmentCodeDisplayName = treatmentCodeDisplayName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public TreatmentCode(string treatmentCodeName, string treatmentCodeDisplayName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.TreatmentCodeID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.TreatmentCodeName = treatmentCodeName;
            this.TreatmentCodeDisplayName = treatmentCodeDisplayName;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static TreatmentCode CreateNewBlank()
        {
            return new TreatmentCode(default(string), default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return Treatments.Any() || TreatmentUpdates.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(Treatments.Any())
            {
                dependentObjects.Add(typeof(Treatment).Name);
            }

            if(TreatmentUpdates.Any())
            {
                dependentObjects.Add(typeof(TreatmentUpdate).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(TreatmentCode).Name, typeof(Treatment).Name, typeof(TreatmentUpdate).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.TreatmentCodes.Remove(this);
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

            foreach(var x in Treatments.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TreatmentUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int TreatmentCodeID { get; set; }
        public string TreatmentCodeName { get; set; }
        public string TreatmentCodeDisplayName { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return TreatmentCodeID; } set { TreatmentCodeID = value; } }

        public virtual ICollection<Treatment> Treatments { get; set; }
        public virtual ICollection<TreatmentUpdate> TreatmentUpdates { get; set; }

        public static class FieldLengths
        {
            public const int TreatmentCodeName = 100;
            public const int TreatmentCodeDisplayName = 100;
        }
    }
}