//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FileResource]
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
    // Table [dbo].[FileResource] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FileResource]")]
    public partial class FileResource : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FileResource()
        {
            this.AgreementsWhereYouAreTheAgreementFileResource = new HashSet<Agreement>();
            this.ClassificationsWhereYouAreTheKeyImageFileResource = new HashSet<Classification>();
            this.CustomPageImages = new HashSet<CustomPageImage>();
            this.DNRUplandRegionContentImages = new HashSet<DNRUplandRegionContentImage>();
            this.FieldDefinitionDataImages = new HashSet<FieldDefinitionDataImage>();
            this.FirmaHomePageImages = new HashSet<FirmaHomePageImage>();
            this.FirmaPageImages = new HashSet<FirmaPageImage>();
            this.FundSourceAllocationFileResources = new HashSet<FundSourceAllocationFileResource>();
            this.FundSourceFileResources = new HashSet<FundSourceFileResource>();
            this.InteractionEventFileResources = new HashSet<InteractionEventFileResource>();
            this.InvoicesWhereYouAreTheInvoiceFileResource = new HashSet<Invoice>();
            this.OrganizationsWhereYouAreTheLogoFileResource = new HashSet<Organization>();
            this.PriorityLandscapeFileResources = new HashSet<PriorityLandscapeFileResource>();
            this.ProgramsWhereYouAreTheProgramExampleGeospatialUploadFileResource = new HashSet<Program>();
            this.ProgramsWhereYouAreTheProgramFileResource = new HashSet<Program>();
            this.ProjectDocuments = new HashSet<ProjectDocument>();
            this.ProjectDocumentUpdates = new HashSet<ProjectDocumentUpdate>();
            this.ProjectImages = new HashSet<ProjectImage>();
            this.ProjectImageUpdates = new HashSet<ProjectImageUpdate>();
            this.ReportTemplates = new HashSet<ReportTemplate>();
            this.SystemAttributesWhereYouAreTheBannerLogoFileResource = new HashSet<SystemAttribute>();
            this.SystemAttributesWhereYouAreTheSquareLogoFileResource = new HashSet<SystemAttribute>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FileResource(int fileResourceID, int fileResourceMimeTypeID, string originalBaseFilename, string originalFileExtension, Guid fileResourceGUID, byte[] fileResourceData, int createPersonID, DateTime createDate) : this()
        {
            this.FileResourceID = fileResourceID;
            this.FileResourceMimeTypeID = fileResourceMimeTypeID;
            this.OriginalBaseFilename = originalBaseFilename;
            this.OriginalFileExtension = originalFileExtension;
            this.FileResourceGUID = fileResourceGUID;
            this.FileResourceData = fileResourceData;
            this.CreatePersonID = createPersonID;
            this.CreateDate = createDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FileResource(int fileResourceMimeTypeID, string originalBaseFilename, string originalFileExtension, Guid fileResourceGUID, byte[] fileResourceData, int createPersonID, DateTime createDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FileResourceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FileResourceMimeTypeID = fileResourceMimeTypeID;
            this.OriginalBaseFilename = originalBaseFilename;
            this.OriginalFileExtension = originalFileExtension;
            this.FileResourceGUID = fileResourceGUID;
            this.FileResourceData = fileResourceData;
            this.CreatePersonID = createPersonID;
            this.CreateDate = createDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public FileResource(FileResourceMimeType fileResourceMimeType, string originalBaseFilename, string originalFileExtension, Guid fileResourceGUID, byte[] fileResourceData, Person createPerson, DateTime createDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FileResourceID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.FileResourceMimeTypeID = fileResourceMimeType.FileResourceMimeTypeID;
            this.OriginalBaseFilename = originalBaseFilename;
            this.OriginalFileExtension = originalFileExtension;
            this.FileResourceGUID = fileResourceGUID;
            this.FileResourceData = fileResourceData;
            this.CreatePersonID = createPerson.PersonID;
            this.CreatePerson = createPerson;
            createPerson.FileResourcesWhereYouAreTheCreatePerson.Add(this);
            this.CreateDate = createDate;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FileResource CreateNewBlank(FileResourceMimeType fileResourceMimeType, Person createPerson)
        {
            return new FileResource(fileResourceMimeType, default(string), default(string), default(Guid), default(byte[]), createPerson, default(DateTime));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return AgreementsWhereYouAreTheAgreementFileResource.Any() || ClassificationsWhereYouAreTheKeyImageFileResource.Any() || CustomPageImages.Any() || DNRUplandRegionContentImages.Any() || FieldDefinitionDataImages.Any() || FirmaHomePageImages.Any() || FirmaPageImages.Any() || FundSourceAllocationFileResources.Any() || FundSourceFileResources.Any() || (InteractionEventFileResource != null) || InvoicesWhereYouAreTheInvoiceFileResource.Any() || OrganizationsWhereYouAreTheLogoFileResource.Any() || (PriorityLandscapeFileResource != null) || ProgramsWhereYouAreTheProgramExampleGeospatialUploadFileResource.Any() || ProgramsWhereYouAreTheProgramFileResource.Any() || ProjectDocuments.Any() || ProjectDocumentUpdates.Any() || ProjectImages.Any() || ProjectImageUpdates.Any() || ReportTemplates.Any() || SystemAttributesWhereYouAreTheBannerLogoFileResource.Any() || SystemAttributesWhereYouAreTheSquareLogoFileResource.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(AgreementsWhereYouAreTheAgreementFileResource.Any())
            {
                dependentObjects.Add(typeof(Agreement).Name);
            }

            if(ClassificationsWhereYouAreTheKeyImageFileResource.Any())
            {
                dependentObjects.Add(typeof(Classification).Name);
            }

            if(CustomPageImages.Any())
            {
                dependentObjects.Add(typeof(CustomPageImage).Name);
            }

            if(DNRUplandRegionContentImages.Any())
            {
                dependentObjects.Add(typeof(DNRUplandRegionContentImage).Name);
            }

            if(FieldDefinitionDataImages.Any())
            {
                dependentObjects.Add(typeof(FieldDefinitionDataImage).Name);
            }

            if(FirmaHomePageImages.Any())
            {
                dependentObjects.Add(typeof(FirmaHomePageImage).Name);
            }

            if(FirmaPageImages.Any())
            {
                dependentObjects.Add(typeof(FirmaPageImage).Name);
            }

            if(FundSourceAllocationFileResources.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationFileResource).Name);
            }

            if(FundSourceFileResources.Any())
            {
                dependentObjects.Add(typeof(FundSourceFileResource).Name);
            }

            if((InteractionEventFileResource != null))
            {
                dependentObjects.Add(typeof(InteractionEventFileResource).Name);
            }

            if(InvoicesWhereYouAreTheInvoiceFileResource.Any())
            {
                dependentObjects.Add(typeof(Invoice).Name);
            }

            if(OrganizationsWhereYouAreTheLogoFileResource.Any())
            {
                dependentObjects.Add(typeof(Organization).Name);
            }

            if((PriorityLandscapeFileResource != null))
            {
                dependentObjects.Add(typeof(PriorityLandscapeFileResource).Name);
            }

            if(ProgramsWhereYouAreTheProgramExampleGeospatialUploadFileResource.Any())
            {
                dependentObjects.Add(typeof(Program).Name);
            }

            if(ProgramsWhereYouAreTheProgramFileResource.Any())
            {
                dependentObjects.Add(typeof(Program).Name);
            }

            if(ProjectDocuments.Any())
            {
                dependentObjects.Add(typeof(ProjectDocument).Name);
            }

            if(ProjectDocumentUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectDocumentUpdate).Name);
            }

            if(ProjectImages.Any())
            {
                dependentObjects.Add(typeof(ProjectImage).Name);
            }

            if(ProjectImageUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectImageUpdate).Name);
            }

            if(ReportTemplates.Any())
            {
                dependentObjects.Add(typeof(ReportTemplate).Name);
            }

            if(SystemAttributesWhereYouAreTheBannerLogoFileResource.Any())
            {
                dependentObjects.Add(typeof(SystemAttribute).Name);
            }

            if(SystemAttributesWhereYouAreTheSquareLogoFileResource.Any())
            {
                dependentObjects.Add(typeof(SystemAttribute).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FileResource).Name, typeof(Agreement).Name, typeof(Classification).Name, typeof(CustomPageImage).Name, typeof(DNRUplandRegionContentImage).Name, typeof(FieldDefinitionDataImage).Name, typeof(FirmaHomePageImage).Name, typeof(FirmaPageImage).Name, typeof(FundSourceAllocationFileResource).Name, typeof(FundSourceFileResource).Name, typeof(InteractionEventFileResource).Name, typeof(Invoice).Name, typeof(Organization).Name, typeof(PriorityLandscapeFileResource).Name, typeof(Program).Name, typeof(ProjectDocument).Name, typeof(ProjectDocumentUpdate).Name, typeof(ProjectImage).Name, typeof(ProjectImageUpdate).Name, typeof(ReportTemplate).Name, typeof(SystemAttribute).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FileResources.Remove(this);
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

            foreach(var x in AgreementsWhereYouAreTheAgreementFileResource.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ClassificationsWhereYouAreTheKeyImageFileResource.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in CustomPageImages.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in DNRUplandRegionContentImages.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FieldDefinitionDataImages.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FirmaHomePageImages.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FirmaPageImages.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationFileResources.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceFileResources.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in InteractionEventFileResources.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in InvoicesWhereYouAreTheInvoiceFileResource.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in OrganizationsWhereYouAreTheLogoFileResource.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PriorityLandscapeFileResources.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramsWhereYouAreTheProgramExampleGeospatialUploadFileResource.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramsWhereYouAreTheProgramFileResource.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectDocuments.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectDocumentUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectImages.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectImageUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ReportTemplates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in SystemAttributesWhereYouAreTheBannerLogoFileResource.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in SystemAttributesWhereYouAreTheSquareLogoFileResource.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int FileResourceID { get; set; }
        public int FileResourceMimeTypeID { get; set; }
        public string OriginalBaseFilename { get; set; }
        public string OriginalFileExtension { get; set; }
        public Guid FileResourceGUID { get; set; }
        public byte[] FileResourceData { get; set; }
        public int CreatePersonID { get; set; }
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FileResourceID; } set { FileResourceID = value; } }

        public virtual ICollection<Agreement> AgreementsWhereYouAreTheAgreementFileResource { get; set; }
        public virtual ICollection<Classification> ClassificationsWhereYouAreTheKeyImageFileResource { get; set; }
        public virtual ICollection<CustomPageImage> CustomPageImages { get; set; }
        public virtual ICollection<DNRUplandRegionContentImage> DNRUplandRegionContentImages { get; set; }
        public virtual ICollection<FieldDefinitionDataImage> FieldDefinitionDataImages { get; set; }
        public virtual ICollection<FirmaHomePageImage> FirmaHomePageImages { get; set; }
        public virtual ICollection<FirmaPageImage> FirmaPageImages { get; set; }
        public virtual ICollection<FundSourceAllocationFileResource> FundSourceAllocationFileResources { get; set; }
        public virtual ICollection<FundSourceFileResource> FundSourceFileResources { get; set; }
        protected virtual ICollection<InteractionEventFileResource> InteractionEventFileResources { get; set; }
        [NotMapped]
        public InteractionEventFileResource InteractionEventFileResource { get { return InteractionEventFileResources.SingleOrDefault(); } set { InteractionEventFileResources = new List<InteractionEventFileResource>{value};} }
        public virtual ICollection<Invoice> InvoicesWhereYouAreTheInvoiceFileResource { get; set; }
        public virtual ICollection<Organization> OrganizationsWhereYouAreTheLogoFileResource { get; set; }
        protected virtual ICollection<PriorityLandscapeFileResource> PriorityLandscapeFileResources { get; set; }
        [NotMapped]
        public PriorityLandscapeFileResource PriorityLandscapeFileResource { get { return PriorityLandscapeFileResources.SingleOrDefault(); } set { PriorityLandscapeFileResources = new List<PriorityLandscapeFileResource>{value};} }
        public virtual ICollection<Program> ProgramsWhereYouAreTheProgramExampleGeospatialUploadFileResource { get; set; }
        public virtual ICollection<Program> ProgramsWhereYouAreTheProgramFileResource { get; set; }
        public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; }
        public virtual ICollection<ProjectDocumentUpdate> ProjectDocumentUpdates { get; set; }
        public virtual ICollection<ProjectImage> ProjectImages { get; set; }
        public virtual ICollection<ProjectImageUpdate> ProjectImageUpdates { get; set; }
        public virtual ICollection<ReportTemplate> ReportTemplates { get; set; }
        public virtual ICollection<SystemAttribute> SystemAttributesWhereYouAreTheBannerLogoFileResource { get; set; }
        public virtual ICollection<SystemAttribute> SystemAttributesWhereYouAreTheSquareLogoFileResource { get; set; }
        public FileResourceMimeType FileResourceMimeType { get { return FileResourceMimeType.AllLookupDictionary[FileResourceMimeTypeID]; } }
        public virtual Person CreatePerson { get; set; }

        public static class FieldLengths
        {
            public const int OriginalBaseFilename = 255;
            public const int OriginalFileExtension = 255;
        }
    }
}