//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ExternalMapLayer]
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
    // Table [dbo].[ExternalMapLayer] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ExternalMapLayer]")]
    public partial class ExternalMapLayer : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ExternalMapLayer()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ExternalMapLayer(int externalMapLayerID, string displayName, string layerUrl, string layerDescription, string featureNameField, bool displayOnPriorityLandscape, bool displayOnProjectMap, bool displayOnAllOthers, bool isActive, bool isTiledMapService) : this()
        {
            this.ExternalMapLayerID = externalMapLayerID;
            this.DisplayName = displayName;
            this.LayerUrl = layerUrl;
            this.LayerDescription = layerDescription;
            this.FeatureNameField = featureNameField;
            this.DisplayOnPriorityLandscape = displayOnPriorityLandscape;
            this.DisplayOnProjectMap = displayOnProjectMap;
            this.DisplayOnAllOthers = displayOnAllOthers;
            this.IsActive = isActive;
            this.IsTiledMapService = isTiledMapService;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ExternalMapLayer(string displayName, string layerUrl, bool displayOnPriorityLandscape, bool displayOnProjectMap, bool displayOnAllOthers, bool isActive, bool isTiledMapService) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ExternalMapLayerID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.DisplayName = displayName;
            this.LayerUrl = layerUrl;
            this.DisplayOnPriorityLandscape = displayOnPriorityLandscape;
            this.DisplayOnProjectMap = displayOnProjectMap;
            this.DisplayOnAllOthers = displayOnAllOthers;
            this.IsActive = isActive;
            this.IsTiledMapService = isTiledMapService;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ExternalMapLayer CreateNewBlank()
        {
            return new ExternalMapLayer(default(string), default(string), default(bool), default(bool), default(bool), default(bool), default(bool));
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
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ExternalMapLayer).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ExternalMapLayers.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int ExternalMapLayerID { get; set; }
        public string DisplayName { get; set; }
        public string LayerUrl { get; set; }
        public string LayerDescription { get; set; }
        public string FeatureNameField { get; set; }
        public bool DisplayOnPriorityLandscape { get; set; }
        public bool DisplayOnProjectMap { get; set; }
        public bool DisplayOnAllOthers { get; set; }
        public bool IsActive { get; set; }
        public bool IsTiledMapService { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ExternalMapLayerID; } set { ExternalMapLayerID = value; } }



        public static class FieldLengths
        {
            public const int DisplayName = 75;
            public const int LayerUrl = 500;
            public const int LayerDescription = 200;
            public const int FeatureNameField = 100;
        }
    }
}