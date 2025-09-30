//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisCrossWalkDefault]
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
    // Table [dbo].[GisCrossWalkDefault] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisCrossWalkDefault]")]
    public partial class GisCrossWalkDefault : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisCrossWalkDefault()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisCrossWalkDefault(int gisCrossWalkDefaultID, int gisUploadSourceOrganizationID, int fieldDefinitionID, string gisCrossWalkSourceValue, string gisCrossWalkMappedValue) : this()
        {
            this.GisCrossWalkDefaultID = gisCrossWalkDefaultID;
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganizationID;
            this.FieldDefinitionID = fieldDefinitionID;
            this.GisCrossWalkSourceValue = gisCrossWalkSourceValue;
            this.GisCrossWalkMappedValue = gisCrossWalkMappedValue;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisCrossWalkDefault(int gisUploadSourceOrganizationID, int fieldDefinitionID, string gisCrossWalkSourceValue, string gisCrossWalkMappedValue) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisCrossWalkDefaultID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganizationID;
            this.FieldDefinitionID = fieldDefinitionID;
            this.GisCrossWalkSourceValue = gisCrossWalkSourceValue;
            this.GisCrossWalkMappedValue = gisCrossWalkMappedValue;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public GisCrossWalkDefault(GisUploadSourceOrganization gisUploadSourceOrganization, FieldDefinition fieldDefinition, string gisCrossWalkSourceValue, string gisCrossWalkMappedValue) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisCrossWalkDefaultID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganization.GisUploadSourceOrganizationID;
            this.GisUploadSourceOrganization = gisUploadSourceOrganization;
            gisUploadSourceOrganization.GisCrossWalkDefaults.Add(this);
            this.FieldDefinitionID = fieldDefinition.FieldDefinitionID;
            this.GisCrossWalkSourceValue = gisCrossWalkSourceValue;
            this.GisCrossWalkMappedValue = gisCrossWalkMappedValue;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisCrossWalkDefault CreateNewBlank(GisUploadSourceOrganization gisUploadSourceOrganization, FieldDefinition fieldDefinition)
        {
            return new GisCrossWalkDefault(gisUploadSourceOrganization, fieldDefinition, default(string), default(string));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisCrossWalkDefault).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisCrossWalkDefaults.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int GisCrossWalkDefaultID { get; set; }
        public int GisUploadSourceOrganizationID { get; set; }
        public int FieldDefinitionID { get; set; }
        public string GisCrossWalkSourceValue { get; set; }
        public string GisCrossWalkMappedValue { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return GisCrossWalkDefaultID; } set { GisCrossWalkDefaultID = value; } }

        public virtual GisUploadSourceOrganization GisUploadSourceOrganization { get; set; }
        public FieldDefinition FieldDefinition { get { return FieldDefinition.AllLookupDictionary[FieldDefinitionID]; } }

        public static class FieldLengths
        {
            public const int GisCrossWalkSourceValue = 300;
            public const int GisCrossWalkMappedValue = 300;
        }
    }
}