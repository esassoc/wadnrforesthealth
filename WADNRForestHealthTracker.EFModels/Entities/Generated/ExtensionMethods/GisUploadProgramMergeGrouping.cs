//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadProgramMergeGrouping]
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
    // Table [dbo].[GisUploadProgramMergeGrouping] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisUploadProgramMergeGrouping]")]
    public partial class GisUploadProgramMergeGrouping : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisUploadProgramMergeGrouping()
        {
            this.GisUploadSourceOrganizations = new HashSet<GisUploadSourceOrganization>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisUploadProgramMergeGrouping(int gisUploadProgramMergeGroupingID, string gisUploadProgramMergeGroupingName) : this()
        {
            this.GisUploadProgramMergeGroupingID = gisUploadProgramMergeGroupingID;
            this.GisUploadProgramMergeGroupingName = gisUploadProgramMergeGroupingName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisUploadProgramMergeGrouping(string gisUploadProgramMergeGroupingName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisUploadProgramMergeGroupingID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisUploadProgramMergeGroupingName = gisUploadProgramMergeGroupingName;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisUploadProgramMergeGrouping CreateNewBlank()
        {
            return new GisUploadProgramMergeGrouping(default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return GisUploadSourceOrganizations.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(GisUploadSourceOrganizations.Any())
            {
                dependentObjects.Add(typeof(GisUploadSourceOrganization).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisUploadProgramMergeGrouping).Name, typeof(GisUploadSourceOrganization).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisUploadProgramMergeGroupings.Remove(this);
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

            foreach(var x in GisUploadSourceOrganizations.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int GisUploadProgramMergeGroupingID { get; set; }
        public string GisUploadProgramMergeGroupingName { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return GisUploadProgramMergeGroupingID; } set { GisUploadProgramMergeGroupingID = value; } }

        public virtual ICollection<GisUploadSourceOrganization> GisUploadSourceOrganizations { get; set; }

        public static class FieldLengths
        {
            public const int GisUploadProgramMergeGroupingName = 100;
        }
    }
}