//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisExcludeIncludeColumn]
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
    // Table [dbo].[GisExcludeIncludeColumn] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[GisExcludeIncludeColumn]")]
    public partial class GisExcludeIncludeColumn : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected GisExcludeIncludeColumn()
        {
            this.GisExcludeIncludeColumnValues = new HashSet<GisExcludeIncludeColumnValue>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisExcludeIncludeColumn(int gisExcludeIncludeColumnID, int gisUploadSourceOrganizationID, string gisDefaultMappingColumnName, bool isWhitelist, bool? isBlacklist) : this()
        {
            this.GisExcludeIncludeColumnID = gisExcludeIncludeColumnID;
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganizationID;
            this.GisDefaultMappingColumnName = gisDefaultMappingColumnName;
            this.IsWhitelist = isWhitelist;
            this.IsBlacklist = isBlacklist;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public GisExcludeIncludeColumn(int gisUploadSourceOrganizationID, string gisDefaultMappingColumnName, bool isWhitelist) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisExcludeIncludeColumnID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganizationID;
            this.GisDefaultMappingColumnName = gisDefaultMappingColumnName;
            this.IsWhitelist = isWhitelist;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public GisExcludeIncludeColumn(GisUploadSourceOrganization gisUploadSourceOrganization, string gisDefaultMappingColumnName, bool isWhitelist) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.GisExcludeIncludeColumnID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.GisUploadSourceOrganizationID = gisUploadSourceOrganization.GisUploadSourceOrganizationID;
            this.GisUploadSourceOrganization = gisUploadSourceOrganization;
            gisUploadSourceOrganization.GisExcludeIncludeColumns.Add(this);
            this.GisDefaultMappingColumnName = gisDefaultMappingColumnName;
            this.IsWhitelist = isWhitelist;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static GisExcludeIncludeColumn CreateNewBlank(GisUploadSourceOrganization gisUploadSourceOrganization)
        {
            return new GisExcludeIncludeColumn(gisUploadSourceOrganization, default(string), default(bool));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return GisExcludeIncludeColumnValues.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(GisExcludeIncludeColumnValues.Any())
            {
                dependentObjects.Add(typeof(GisExcludeIncludeColumnValue).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(GisExcludeIncludeColumn).Name, typeof(GisExcludeIncludeColumnValue).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.GisExcludeIncludeColumns.Remove(this);
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

            foreach(var x in GisExcludeIncludeColumnValues.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int GisExcludeIncludeColumnID { get; set; }
        public int GisUploadSourceOrganizationID { get; set; }
        public string GisDefaultMappingColumnName { get; set; }
        public bool IsWhitelist { get; set; }
        public bool? IsBlacklist { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return GisExcludeIncludeColumnID; } set { GisExcludeIncludeColumnID = value; } }

        public virtual ICollection<GisExcludeIncludeColumnValue> GisExcludeIncludeColumnValues { get; set; }
        public virtual GisUploadSourceOrganization GisUploadSourceOrganization { get; set; }

        public static class FieldLengths
        {
            public const int GisDefaultMappingColumnName = 300;
        }
    }
}