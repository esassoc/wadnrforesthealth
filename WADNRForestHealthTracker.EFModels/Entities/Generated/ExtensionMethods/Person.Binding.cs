//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Person]
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
    // Table [dbo].[Person] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[Person]")]
    public partial class Person : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected Person()
        {
            this.AgreementPeople = new HashSet<AgreementPerson>();
            this.AuditLogs = new HashSet<AuditLog>();
            this.DNRUplandRegionsWhereYouAreTheDNRUplandRegionCoordinator = new HashSet<DNRUplandRegion>();
            this.FileResourcesWhereYouAreTheCreatePerson = new HashSet<FileResource>();
            this.ForesterWorkUnits = new HashSet<ForesterWorkUnit>();
            this.FundSourceAllocationsWhereYouAreTheFundSourceManager = new HashSet<FundSourceAllocation>();
            this.FundSourceAllocationChangeLogsWhereYouAreTheChangePerson = new HashSet<FundSourceAllocationChangeLog>();
            this.FundSourceAllocationLikelyPeople = new HashSet<FundSourceAllocationLikelyPerson>();
            this.FundSourceAllocationNotesWhereYouAreTheCreatedByPerson = new HashSet<FundSourceAllocationNote>();
            this.FundSourceAllocationNotesWhereYouAreTheLastUpdatedByPerson = new HashSet<FundSourceAllocationNote>();
            this.FundSourceAllocationNoteInternalsWhereYouAreTheCreatedByPerson = new HashSet<FundSourceAllocationNoteInternal>();
            this.FundSourceAllocationNoteInternalsWhereYouAreTheLastUpdatedByPerson = new HashSet<FundSourceAllocationNoteInternal>();
            this.FundSourceAllocationProgramManagers = new HashSet<FundSourceAllocationProgramManager>();
            this.FundSourceNotesWhereYouAreTheCreatedByPerson = new HashSet<FundSourceNote>();
            this.FundSourceNotesWhereYouAreTheLastUpdatedByPerson = new HashSet<FundSourceNote>();
            this.FundSourceNoteInternalsWhereYouAreTheCreatedByPerson = new HashSet<FundSourceNoteInternal>();
            this.FundSourceNoteInternalsWhereYouAreTheLastUpdatedByPerson = new HashSet<FundSourceNoteInternal>();
            this.GisUploadAttemptsWhereYouAreTheGisUploadAttemptCreatePerson = new HashSet<GisUploadAttempt>();
            this.InteractionEventsWhereYouAreTheStaffPerson = new HashSet<InteractionEvent>();
            this.InteractionEventContacts = new HashSet<InteractionEventContact>();
            this.InvoicePaymentRequestsWhereYouAreThePreparedByPerson = new HashSet<InvoicePaymentRequest>();
            this.Notifications = new HashSet<Notification>();
            this.OrganizationsWhereYouAreThePrimaryContactPerson = new HashSet<Organization>();
            this.PeopleWhereYouAreTheAddedByPerson = new HashSet<Person>();
            this.PersonAllowedAuthenticators = new HashSet<PersonAllowedAuthenticator>();
            this.PersonRoles = new HashSet<PersonRole>();
            this.PersonStewardOrganizations = new HashSet<PersonStewardOrganization>();
            this.PersonStewardRegions = new HashSet<PersonStewardRegion>();
            this.PersonStewardTaxonomyBranches = new HashSet<PersonStewardTaxonomyBranch>();
            this.ProgramsWhereYouAreTheProgramCreatePerson = new HashSet<Program>();
            this.ProgramsWhereYouAreTheProgramLastUpdatedByPerson = new HashSet<Program>();
            this.ProgramsWhereYouAreTheProgramPrimaryContactPerson = new HashSet<Program>();
            this.ProgramNotificationSentsWhereYouAreTheSentToPerson = new HashSet<ProgramNotificationSent>();
            this.ProgramPeople = new HashSet<ProgramPerson>();
            this.ProjectsWhereYouAreTheProposingPerson = new HashSet<Project>();
            this.ProjectsWhereYouAreTheReviewedByPerson = new HashSet<Project>();
            this.ProjectInternalNotesWhereYouAreTheCreatePerson = new HashSet<ProjectInternalNote>();
            this.ProjectInternalNotesWhereYouAreTheUpdatePerson = new HashSet<ProjectInternalNote>();
            this.ProjectLocationStagings = new HashSet<ProjectLocationStaging>();
            this.ProjectLocationStagingUpdates = new HashSet<ProjectLocationStagingUpdate>();
            this.ProjectNotesWhereYouAreTheCreatePerson = new HashSet<ProjectNote>();
            this.ProjectNotesWhereYouAreTheUpdatePerson = new HashSet<ProjectNote>();
            this.ProjectNoteUpdatesWhereYouAreTheCreatePerson = new HashSet<ProjectNoteUpdate>();
            this.ProjectNoteUpdatesWhereYouAreTheUpdatePerson = new HashSet<ProjectNoteUpdate>();
            this.ProjectPeople = new HashSet<ProjectPerson>();
            this.ProjectPersonUpdates = new HashSet<ProjectPersonUpdate>();
            this.ProjectUpdateBatchesWhereYouAreTheLastUpdatePerson = new HashSet<ProjectUpdateBatch>();
            this.ProjectUpdateHistoriesWhereYouAreTheUpdatePerson = new HashSet<ProjectUpdateHistory>();
            this.SupportRequestLogsWhereYouAreTheRequestPerson = new HashSet<SupportRequestLog>();
            this.SystemAttributesWhereYouAreThePrimaryContactPerson = new HashSet<SystemAttribute>();
            this.TabularDataImportsWhereYouAreTheLastProcessedPerson = new HashSet<TabularDataImport>();
            this.TabularDataImportsWhereYouAreTheUploadPerson = new HashSet<TabularDataImport>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public Person(int personID, string firstName, string lastName, string email, string phone, DateTime createDate, DateTime? updateDate, DateTime? lastActivityDate, bool isActive, int? organizationID, bool receiveSupportEmails, Guid? webServiceAccessToken, string middleName, string notes, string personAddress, int? addedByPersonID, int? vendorID, bool? isProgramManager, bool? createdAsPartOfBulkImport) : this()
        {
            this.PersonID = personID;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Phone = phone;
            this.CreateDate = createDate;
            this.UpdateDate = updateDate;
            this.LastActivityDate = lastActivityDate;
            this.IsActive = isActive;
            this.OrganizationID = organizationID;
            this.ReceiveSupportEmails = receiveSupportEmails;
            this.WebServiceAccessToken = webServiceAccessToken;
            this.MiddleName = middleName;
            this.Notes = notes;
            this.PersonAddress = personAddress;
            this.AddedByPersonID = addedByPersonID;
            this.VendorID = vendorID;
            this.IsProgramManager = isProgramManager;
            this.CreatedAsPartOfBulkImport = createdAsPartOfBulkImport;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public Person(string firstName, DateTime createDate, bool isActive, bool receiveSupportEmails) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.PersonID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.FirstName = firstName;
            this.CreateDate = createDate;
            this.IsActive = isActive;
            this.ReceiveSupportEmails = receiveSupportEmails;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static Person CreateNewBlank()
        {
            return new Person(default(string), default(DateTime), default(bool), default(bool));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return AgreementPeople.Any() || AuditLogs.Any() || DNRUplandRegionsWhereYouAreTheDNRUplandRegionCoordinator.Any() || FileResourcesWhereYouAreTheCreatePerson.Any() || ForesterWorkUnits.Any() || FundSourceAllocationsWhereYouAreTheFundSourceManager.Any() || FundSourceAllocationChangeLogsWhereYouAreTheChangePerson.Any() || FundSourceAllocationLikelyPeople.Any() || FundSourceAllocationNotesWhereYouAreTheCreatedByPerson.Any() || FundSourceAllocationNotesWhereYouAreTheLastUpdatedByPerson.Any() || FundSourceAllocationNoteInternalsWhereYouAreTheCreatedByPerson.Any() || FundSourceAllocationNoteInternalsWhereYouAreTheLastUpdatedByPerson.Any() || FundSourceAllocationProgramManagers.Any() || FundSourceNotesWhereYouAreTheCreatedByPerson.Any() || FundSourceNotesWhereYouAreTheLastUpdatedByPerson.Any() || FundSourceNoteInternalsWhereYouAreTheCreatedByPerson.Any() || FundSourceNoteInternalsWhereYouAreTheLastUpdatedByPerson.Any() || GisUploadAttemptsWhereYouAreTheGisUploadAttemptCreatePerson.Any() || InteractionEventsWhereYouAreTheStaffPerson.Any() || InteractionEventContacts.Any() || InvoicePaymentRequestsWhereYouAreThePreparedByPerson.Any() || Notifications.Any() || OrganizationsWhereYouAreThePrimaryContactPerson.Any() || PeopleWhereYouAreTheAddedByPerson.Any() || PersonAllowedAuthenticators.Any() || PersonRoles.Any() || PersonStewardOrganizations.Any() || PersonStewardRegions.Any() || PersonStewardTaxonomyBranches.Any() || ProgramsWhereYouAreTheProgramCreatePerson.Any() || ProgramsWhereYouAreTheProgramLastUpdatedByPerson.Any() || ProgramsWhereYouAreTheProgramPrimaryContactPerson.Any() || ProgramNotificationSentsWhereYouAreTheSentToPerson.Any() || ProgramPeople.Any() || ProjectsWhereYouAreTheProposingPerson.Any() || ProjectsWhereYouAreTheReviewedByPerson.Any() || ProjectInternalNotesWhereYouAreTheCreatePerson.Any() || ProjectInternalNotesWhereYouAreTheUpdatePerson.Any() || ProjectLocationStagings.Any() || ProjectLocationStagingUpdates.Any() || ProjectNotesWhereYouAreTheCreatePerson.Any() || ProjectNotesWhereYouAreTheUpdatePerson.Any() || ProjectNoteUpdatesWhereYouAreTheCreatePerson.Any() || ProjectNoteUpdatesWhereYouAreTheUpdatePerson.Any() || ProjectPeople.Any() || ProjectPersonUpdates.Any() || ProjectUpdateBatchesWhereYouAreTheLastUpdatePerson.Any() || ProjectUpdateHistoriesWhereYouAreTheUpdatePerson.Any() || SupportRequestLogsWhereYouAreTheRequestPerson.Any() || SystemAttributesWhereYouAreThePrimaryContactPerson.Any() || TabularDataImportsWhereYouAreTheLastProcessedPerson.Any() || TabularDataImportsWhereYouAreTheUploadPerson.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(AgreementPeople.Any())
            {
                dependentObjects.Add(typeof(AgreementPerson).Name);
            }

            if(AuditLogs.Any())
            {
                dependentObjects.Add(typeof(AuditLog).Name);
            }

            if(DNRUplandRegionsWhereYouAreTheDNRUplandRegionCoordinator.Any())
            {
                dependentObjects.Add(typeof(DNRUplandRegion).Name);
            }

            if(FileResourcesWhereYouAreTheCreatePerson.Any())
            {
                dependentObjects.Add(typeof(FileResource).Name);
            }

            if(ForesterWorkUnits.Any())
            {
                dependentObjects.Add(typeof(ForesterWorkUnit).Name);
            }

            if(FundSourceAllocationsWhereYouAreTheFundSourceManager.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocation).Name);
            }

            if(FundSourceAllocationChangeLogsWhereYouAreTheChangePerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationChangeLog).Name);
            }

            if(FundSourceAllocationLikelyPeople.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationLikelyPerson).Name);
            }

            if(FundSourceAllocationNotesWhereYouAreTheCreatedByPerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationNote).Name);
            }

            if(FundSourceAllocationNotesWhereYouAreTheLastUpdatedByPerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationNote).Name);
            }

            if(FundSourceAllocationNoteInternalsWhereYouAreTheCreatedByPerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationNoteInternal).Name);
            }

            if(FundSourceAllocationNoteInternalsWhereYouAreTheLastUpdatedByPerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationNoteInternal).Name);
            }

            if(FundSourceAllocationProgramManagers.Any())
            {
                dependentObjects.Add(typeof(FundSourceAllocationProgramManager).Name);
            }

            if(FundSourceNotesWhereYouAreTheCreatedByPerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceNote).Name);
            }

            if(FundSourceNotesWhereYouAreTheLastUpdatedByPerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceNote).Name);
            }

            if(FundSourceNoteInternalsWhereYouAreTheCreatedByPerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceNoteInternal).Name);
            }

            if(FundSourceNoteInternalsWhereYouAreTheLastUpdatedByPerson.Any())
            {
                dependentObjects.Add(typeof(FundSourceNoteInternal).Name);
            }

            if(GisUploadAttemptsWhereYouAreTheGisUploadAttemptCreatePerson.Any())
            {
                dependentObjects.Add(typeof(GisUploadAttempt).Name);
            }

            if(InteractionEventsWhereYouAreTheStaffPerson.Any())
            {
                dependentObjects.Add(typeof(InteractionEvent).Name);
            }

            if(InteractionEventContacts.Any())
            {
                dependentObjects.Add(typeof(InteractionEventContact).Name);
            }

            if(InvoicePaymentRequestsWhereYouAreThePreparedByPerson.Any())
            {
                dependentObjects.Add(typeof(InvoicePaymentRequest).Name);
            }

            if(Notifications.Any())
            {
                dependentObjects.Add(typeof(Notification).Name);
            }

            if(OrganizationsWhereYouAreThePrimaryContactPerson.Any())
            {
                dependentObjects.Add(typeof(Organization).Name);
            }

            if(PeopleWhereYouAreTheAddedByPerson.Any())
            {
                dependentObjects.Add(typeof(Person).Name);
            }

            if(PersonAllowedAuthenticators.Any())
            {
                dependentObjects.Add(typeof(PersonAllowedAuthenticator).Name);
            }

            if(PersonRoles.Any())
            {
                dependentObjects.Add(typeof(PersonRole).Name);
            }

            if(PersonStewardOrganizations.Any())
            {
                dependentObjects.Add(typeof(PersonStewardOrganization).Name);
            }

            if(PersonStewardRegions.Any())
            {
                dependentObjects.Add(typeof(PersonStewardRegion).Name);
            }

            if(PersonStewardTaxonomyBranches.Any())
            {
                dependentObjects.Add(typeof(PersonStewardTaxonomyBranch).Name);
            }

            if(ProgramsWhereYouAreTheProgramCreatePerson.Any())
            {
                dependentObjects.Add(typeof(Program).Name);
            }

            if(ProgramsWhereYouAreTheProgramLastUpdatedByPerson.Any())
            {
                dependentObjects.Add(typeof(Program).Name);
            }

            if(ProgramsWhereYouAreTheProgramPrimaryContactPerson.Any())
            {
                dependentObjects.Add(typeof(Program).Name);
            }

            if(ProgramNotificationSentsWhereYouAreTheSentToPerson.Any())
            {
                dependentObjects.Add(typeof(ProgramNotificationSent).Name);
            }

            if(ProgramPeople.Any())
            {
                dependentObjects.Add(typeof(ProgramPerson).Name);
            }

            if(ProjectsWhereYouAreTheProposingPerson.Any())
            {
                dependentObjects.Add(typeof(Project).Name);
            }

            if(ProjectsWhereYouAreTheReviewedByPerson.Any())
            {
                dependentObjects.Add(typeof(Project).Name);
            }

            if(ProjectInternalNotesWhereYouAreTheCreatePerson.Any())
            {
                dependentObjects.Add(typeof(ProjectInternalNote).Name);
            }

            if(ProjectInternalNotesWhereYouAreTheUpdatePerson.Any())
            {
                dependentObjects.Add(typeof(ProjectInternalNote).Name);
            }

            if(ProjectLocationStagings.Any())
            {
                dependentObjects.Add(typeof(ProjectLocationStaging).Name);
            }

            if(ProjectLocationStagingUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectLocationStagingUpdate).Name);
            }

            if(ProjectNotesWhereYouAreTheCreatePerson.Any())
            {
                dependentObjects.Add(typeof(ProjectNote).Name);
            }

            if(ProjectNotesWhereYouAreTheUpdatePerson.Any())
            {
                dependentObjects.Add(typeof(ProjectNote).Name);
            }

            if(ProjectNoteUpdatesWhereYouAreTheCreatePerson.Any())
            {
                dependentObjects.Add(typeof(ProjectNoteUpdate).Name);
            }

            if(ProjectNoteUpdatesWhereYouAreTheUpdatePerson.Any())
            {
                dependentObjects.Add(typeof(ProjectNoteUpdate).Name);
            }

            if(ProjectPeople.Any())
            {
                dependentObjects.Add(typeof(ProjectPerson).Name);
            }

            if(ProjectPersonUpdates.Any())
            {
                dependentObjects.Add(typeof(ProjectPersonUpdate).Name);
            }

            if(ProjectUpdateBatchesWhereYouAreTheLastUpdatePerson.Any())
            {
                dependentObjects.Add(typeof(ProjectUpdateBatch).Name);
            }

            if(ProjectUpdateHistoriesWhereYouAreTheUpdatePerson.Any())
            {
                dependentObjects.Add(typeof(ProjectUpdateHistory).Name);
            }

            if(SupportRequestLogsWhereYouAreTheRequestPerson.Any())
            {
                dependentObjects.Add(typeof(SupportRequestLog).Name);
            }

            if(SystemAttributesWhereYouAreThePrimaryContactPerson.Any())
            {
                dependentObjects.Add(typeof(SystemAttribute).Name);
            }

            if(TabularDataImportsWhereYouAreTheLastProcessedPerson.Any())
            {
                dependentObjects.Add(typeof(TabularDataImport).Name);
            }

            if(TabularDataImportsWhereYouAreTheUploadPerson.Any())
            {
                dependentObjects.Add(typeof(TabularDataImport).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(Person).Name, typeof(AgreementPerson).Name, typeof(AuditLog).Name, typeof(DNRUplandRegion).Name, typeof(FileResource).Name, typeof(ForesterWorkUnit).Name, typeof(FundSourceAllocation).Name, typeof(FundSourceAllocationChangeLog).Name, typeof(FundSourceAllocationLikelyPerson).Name, typeof(FundSourceAllocationNote).Name, typeof(FundSourceAllocationNoteInternal).Name, typeof(FundSourceAllocationProgramManager).Name, typeof(FundSourceNote).Name, typeof(FundSourceNoteInternal).Name, typeof(GisUploadAttempt).Name, typeof(InteractionEvent).Name, typeof(InteractionEventContact).Name, typeof(InvoicePaymentRequest).Name, typeof(Notification).Name, typeof(Organization).Name, typeof(PersonAllowedAuthenticator).Name, typeof(PersonRole).Name, typeof(PersonStewardOrganization).Name, typeof(PersonStewardRegion).Name, typeof(PersonStewardTaxonomyBranch).Name, typeof(Program).Name, typeof(ProgramNotificationSent).Name, typeof(ProgramPerson).Name, typeof(Project).Name, typeof(ProjectInternalNote).Name, typeof(ProjectLocationStaging).Name, typeof(ProjectLocationStagingUpdate).Name, typeof(ProjectNote).Name, typeof(ProjectNoteUpdate).Name, typeof(ProjectPerson).Name, typeof(ProjectPersonUpdate).Name, typeof(ProjectUpdateBatch).Name, typeof(ProjectUpdateHistory).Name, typeof(SupportRequestLog).Name, typeof(SystemAttribute).Name, typeof(TabularDataImport).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.People.Remove(this);
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

            foreach(var x in AgreementPeople.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in AuditLogs.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in DNRUplandRegionsWhereYouAreTheDNRUplandRegionCoordinator.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FileResourcesWhereYouAreTheCreatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ForesterWorkUnits.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationsWhereYouAreTheFundSourceManager.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationChangeLogsWhereYouAreTheChangePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationLikelyPeople.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationNotesWhereYouAreTheCreatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationNotesWhereYouAreTheLastUpdatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationNoteInternalsWhereYouAreTheCreatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationNoteInternalsWhereYouAreTheLastUpdatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceAllocationProgramManagers.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceNotesWhereYouAreTheCreatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceNotesWhereYouAreTheLastUpdatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceNoteInternalsWhereYouAreTheCreatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in FundSourceNoteInternalsWhereYouAreTheLastUpdatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in GisUploadAttemptsWhereYouAreTheGisUploadAttemptCreatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in InteractionEventsWhereYouAreTheStaffPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in InteractionEventContacts.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in InvoicePaymentRequestsWhereYouAreThePreparedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in Notifications.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in OrganizationsWhereYouAreThePrimaryContactPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PeopleWhereYouAreTheAddedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PersonAllowedAuthenticators.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PersonRoles.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PersonStewardOrganizations.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PersonStewardRegions.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in PersonStewardTaxonomyBranches.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramsWhereYouAreTheProgramCreatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramsWhereYouAreTheProgramLastUpdatedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramsWhereYouAreTheProgramPrimaryContactPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramNotificationSentsWhereYouAreTheSentToPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProgramPeople.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectsWhereYouAreTheProposingPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectsWhereYouAreTheReviewedByPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectInternalNotesWhereYouAreTheCreatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectInternalNotesWhereYouAreTheUpdatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectLocationStagings.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectLocationStagingUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectNotesWhereYouAreTheCreatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectNotesWhereYouAreTheUpdatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectNoteUpdatesWhereYouAreTheCreatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectNoteUpdatesWhereYouAreTheUpdatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectPeople.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectPersonUpdates.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectUpdateBatchesWhereYouAreTheLastUpdatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in ProjectUpdateHistoriesWhereYouAreTheUpdatePerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in SupportRequestLogsWhereYouAreTheRequestPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in SystemAttributesWhereYouAreThePrimaryContactPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TabularDataImportsWhereYouAreTheLastProcessedPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }

            foreach(var x in TabularDataImportsWhereYouAreTheUploadPerson.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public bool IsActive { get; set; }
        public int? OrganizationID { get; set; }
        public bool ReceiveSupportEmails { get; set; }
        public Guid? WebServiceAccessToken { get; set; }
        public string MiddleName { get; set; }
        public string Notes { get; set; }
        public string PersonAddress { get; set; }
        public int? AddedByPersonID { get; set; }
        public int? VendorID { get; set; }
        public bool? IsProgramManager { get; set; }
        public bool? CreatedAsPartOfBulkImport { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return PersonID; } set { PersonID = value; } }

        public virtual ICollection<AgreementPerson> AgreementPeople { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        public virtual ICollection<DNRUplandRegion> DNRUplandRegionsWhereYouAreTheDNRUplandRegionCoordinator { get; set; }
        public virtual ICollection<FileResource> FileResourcesWhereYouAreTheCreatePerson { get; set; }
        public virtual ICollection<ForesterWorkUnit> ForesterWorkUnits { get; set; }
        public virtual ICollection<FundSourceAllocation> FundSourceAllocationsWhereYouAreTheFundSourceManager { get; set; }
        public virtual ICollection<FundSourceAllocationChangeLog> FundSourceAllocationChangeLogsWhereYouAreTheChangePerson { get; set; }
        public virtual ICollection<FundSourceAllocationLikelyPerson> FundSourceAllocationLikelyPeople { get; set; }
        public virtual ICollection<FundSourceAllocationNote> FundSourceAllocationNotesWhereYouAreTheCreatedByPerson { get; set; }
        public virtual ICollection<FundSourceAllocationNote> FundSourceAllocationNotesWhereYouAreTheLastUpdatedByPerson { get; set; }
        public virtual ICollection<FundSourceAllocationNoteInternal> FundSourceAllocationNoteInternalsWhereYouAreTheCreatedByPerson { get; set; }
        public virtual ICollection<FundSourceAllocationNoteInternal> FundSourceAllocationNoteInternalsWhereYouAreTheLastUpdatedByPerson { get; set; }
        public virtual ICollection<FundSourceAllocationProgramManager> FundSourceAllocationProgramManagers { get; set; }
        public virtual ICollection<FundSourceNote> FundSourceNotesWhereYouAreTheCreatedByPerson { get; set; }
        public virtual ICollection<FundSourceNote> FundSourceNotesWhereYouAreTheLastUpdatedByPerson { get; set; }
        public virtual ICollection<FundSourceNoteInternal> FundSourceNoteInternalsWhereYouAreTheCreatedByPerson { get; set; }
        public virtual ICollection<FundSourceNoteInternal> FundSourceNoteInternalsWhereYouAreTheLastUpdatedByPerson { get; set; }
        public virtual ICollection<GisUploadAttempt> GisUploadAttemptsWhereYouAreTheGisUploadAttemptCreatePerson { get; set; }
        public virtual ICollection<InteractionEvent> InteractionEventsWhereYouAreTheStaffPerson { get; set; }
        public virtual ICollection<InteractionEventContact> InteractionEventContacts { get; set; }
        public virtual ICollection<InvoicePaymentRequest> InvoicePaymentRequestsWhereYouAreThePreparedByPerson { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Organization> OrganizationsWhereYouAreThePrimaryContactPerson { get; set; }
        public virtual ICollection<Person> PeopleWhereYouAreTheAddedByPerson { get; set; }
        public virtual Person AddedByPerson { get; set; }
        public virtual ICollection<PersonAllowedAuthenticator> PersonAllowedAuthenticators { get; set; }
        public virtual ICollection<PersonRole> PersonRoles { get; set; }
        public virtual ICollection<PersonStewardOrganization> PersonStewardOrganizations { get; set; }
        public virtual ICollection<PersonStewardRegion> PersonStewardRegions { get; set; }
        public virtual ICollection<PersonStewardTaxonomyBranch> PersonStewardTaxonomyBranches { get; set; }
        public virtual ICollection<Program> ProgramsWhereYouAreTheProgramCreatePerson { get; set; }
        public virtual ICollection<Program> ProgramsWhereYouAreTheProgramLastUpdatedByPerson { get; set; }
        public virtual ICollection<Program> ProgramsWhereYouAreTheProgramPrimaryContactPerson { get; set; }
        public virtual ICollection<ProgramNotificationSent> ProgramNotificationSentsWhereYouAreTheSentToPerson { get; set; }
        public virtual ICollection<ProgramPerson> ProgramPeople { get; set; }
        public virtual ICollection<Project> ProjectsWhereYouAreTheProposingPerson { get; set; }
        public virtual ICollection<Project> ProjectsWhereYouAreTheReviewedByPerson { get; set; }
        public virtual ICollection<ProjectInternalNote> ProjectInternalNotesWhereYouAreTheCreatePerson { get; set; }
        public virtual ICollection<ProjectInternalNote> ProjectInternalNotesWhereYouAreTheUpdatePerson { get; set; }
        public virtual ICollection<ProjectLocationStaging> ProjectLocationStagings { get; set; }
        public virtual ICollection<ProjectLocationStagingUpdate> ProjectLocationStagingUpdates { get; set; }
        public virtual ICollection<ProjectNote> ProjectNotesWhereYouAreTheCreatePerson { get; set; }
        public virtual ICollection<ProjectNote> ProjectNotesWhereYouAreTheUpdatePerson { get; set; }
        public virtual ICollection<ProjectNoteUpdate> ProjectNoteUpdatesWhereYouAreTheCreatePerson { get; set; }
        public virtual ICollection<ProjectNoteUpdate> ProjectNoteUpdatesWhereYouAreTheUpdatePerson { get; set; }
        public virtual ICollection<ProjectPerson> ProjectPeople { get; set; }
        public virtual ICollection<ProjectPersonUpdate> ProjectPersonUpdates { get; set; }
        public virtual ICollection<ProjectUpdateBatch> ProjectUpdateBatchesWhereYouAreTheLastUpdatePerson { get; set; }
        public virtual ICollection<ProjectUpdateHistory> ProjectUpdateHistoriesWhereYouAreTheUpdatePerson { get; set; }
        public virtual ICollection<SupportRequestLog> SupportRequestLogsWhereYouAreTheRequestPerson { get; set; }
        public virtual ICollection<SystemAttribute> SystemAttributesWhereYouAreThePrimaryContactPerson { get; set; }
        public virtual ICollection<TabularDataImport> TabularDataImportsWhereYouAreTheLastProcessedPerson { get; set; }
        public virtual ICollection<TabularDataImport> TabularDataImportsWhereYouAreTheUploadPerson { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Vendor Vendor { get; set; }

        public static class FieldLengths
        {
            public const int FirstName = 100;
            public const int LastName = 100;
            public const int Email = 255;
            public const int Phone = 30;
            public const int MiddleName = 100;
            public const int Notes = 500;
            public const int PersonAddress = 255;
        }
    }
}