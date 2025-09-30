//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisDefaultMapping]
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
    // Table [dbo].[GisDefaultMapping] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisDefaultMapping]")]
    public partial class GisDefaultMapping : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisDefaultMapping()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisDefaultMapping(int gisDefaultMappingID, int gisUploadSourceOrganizationID, int fieldDefinitionID, string gisDefaultMappingColumnName) : this()
        {
            this.GisDefaultMappingID = gisDefaultMappingID;
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganizationID;
            this.FieldDefinitionID = fieldDefinitionID;
            this.GisDefaultMappingColumnName = gisDefaultMappingColumnName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisDefaultMapping(int gisUploadSourceOrganizationID, int fieldDefinitionID, string gisDefaultMappingColumnName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisDefaultMappingID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganizationID;
            this.FieldDefinitionID = fieldDefinitionID;
            this.GisDefaultMappingColumnName = gisDefaultMappingColumnName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public GisDefaultMapping(GisUploadSourceOrganization gisUploadSourceOrganization, FieldDefinition fieldDefinition, string gisDefaultMappingColumnName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisDefaultMappingID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganization.GisUploadSourceOrganizationID;
            this.GisUploadSourceOrganization = gisUploadSourceOrganization;
            gisUploadSourceOrganization.GisDefaultMappings.Add(this);
            this.FieldDefinitionID = fieldDefinition.FieldDefinitionID;
            this.GisDefaultMappingColumnName = gisDefaultMappingColumnName;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisDefaultMapping CreateNewBlank(GisUploadSourceOrganization gisUploadSourceOrganization, FieldDefinition fieldDefinition)
        {
            return new GisDefaultMapping(gisUploadSourceOrganization, fieldDefinition, default(string));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisDefaultMapping).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisDefaultMappings.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int GisDefaultMappingID { get; set; }
        public int GisUploadSourceOrganizationID { get; set; }
        public int FieldDefinitionID { get; set; }
        public string GisDefaultMappingColumnName { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return GisDefaultMappingID; } set { GisDefaultMappingID = value; } }

        public virtual GisUploadSourceOrganization GisUploadSourceOrganization { get; set; }
        public FieldDefinition FieldDefinition { get { return FieldDefinition.AllLookupDictionary[FieldDefinitionID]; } }

        public static class FieldLengths
        {
            public const int GisDefaultMappingColumnName = 300;
        }
    }
}