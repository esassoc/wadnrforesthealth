/*-----------------------------------------------------------------------
<copyright file="ProjectUpdateBatch.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
Copyright (c) Tahoe Regional Planning Agency and Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using GeoJSON.Net.Feature;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Views.ProjectUpdate;
using LtInfo.Common;
using LtInfo.Common.DbSpatial;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;

namespace ProjectFirma.Web.Models
{
    public partial class ProjectUpdateBatch
    {
        public bool IsApproved => ProjectUpdateState == ProjectUpdateState.Approved;

        public bool IsReturned => ProjectUpdateState == ProjectUpdateState.Returned;

        public bool IsSubmitted => ProjectUpdateState == ProjectUpdateState.Submitted;

        public bool IsCreated => ProjectUpdateState == ProjectUpdateState.Created;

        public bool IsNew => ProjectUpdateState == ProjectUpdateState.Created &&
                             !ModelObjectHelpers.IsRealPrimaryKeyValue(ProjectUpdateBatchID);

        public ProjectUpdateHistory LatestProjectUpdateHistorySubmitted => ProjectUpdateHistories.GetLatestProjectUpdateHistory(ProjectUpdateState.Submitted);

        public DateTime? LatestSubmittalDate => LatestProjectUpdateHistorySubmitted?.TransitionDate;

        public ProjectUpdateHistory LatestProjectUpdateHistoryReturned => ProjectUpdateHistories.GetLatestProjectUpdateHistory(ProjectUpdateState.Returned);

        public bool IsReadyToSubmit => InEditableState && IsPassingAllValidationRules() && ModelObjectHelpers.IsRealPrimaryKeyValue(ProjectUpdateBatchID);

        public bool IsReadyToApprove => IsPassingAllValidationRules();

        private bool IsPassingAllValidationRules()
        {
            return AreProjectBasicsValid && 
                   IsProjectLocationSimpleValid() &&
                   IsProjectPriorityLandscapeValid() &&
                   IsProjectRegionValid();
        }

        public bool InEditableState => Project.IsActiveProject() && (IsCreated || IsReturned);

        public static ProjectUpdateBatch GetLatestNotApprovedProjectUpdateBatchOrCreateNew(Project project, Person currentPerson)
        {
            
            ProjectUpdateBatch projectUpdateBatch;
            if (project.GetLatestNotApprovedUpdateBatch() != null)
            {
                projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            }
            else
            {
                projectUpdateBatch = CreateNewProjectUpdateBatchForProject(project, currentPerson);
            }

            return projectUpdateBatch;
        }

        public static ProjectUpdateBatch GetLatestNotApprovedProjectUpdateBatchOrCreateNewAndSaveToDatabase(Project project, Person currentPerson)
        {
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchOrCreateNew(project, currentPerson);
            if (!ModelObjectHelpers.IsRealPrimaryKeyValue(projectUpdateBatch.ProjectUpdateBatchID))
            {
                HttpRequestStorage.DatabaseEntities.SaveChanges();
            }
            return projectUpdateBatch;
        }

        public static ProjectUpdateBatch CreateNewProjectUpdateBatchForProject(Project project, Person currentPerson)
        {
            var projectUpdateBatch = CreateProjectUpdateBatchAndLogTransition(project, currentPerson);

            // basics & map
            ProjectUpdate.CreateFromProject(projectUpdateBatch);

            // expenditures exempt explanation
            projectUpdateBatch.SyncExpendituresYearsExemptionExplanation();

            // Expected Funding
            ProjectFundSourceAllocationRequestUpdate.CreateFromProject(projectUpdateBatch);

            // project locations - detailed
            ProjectLocationUpdate.CreateFromProject(projectUpdateBatch);

            // project priority landscape
            ProjectPriorityLandscapeUpdate.CreateFromProject(projectUpdateBatch);

            // project region
            ProjectRegionUpdate.CreateFromProject(projectUpdateBatch);

            // project county
            ProjectCountyUpdate.CreateFromProject(projectUpdateBatch);

            // treatments - must happen after locations
            TreatmentUpdate.CreateFromProject(projectUpdateBatch);

            // photos
            ProjectImageUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.IsPhotosUpdated = false;

            // external links
            ProjectExternalLinkUpdate.CreateFromProject(projectUpdateBatch);

            // notes
            ProjectNoteUpdate.CreateFromProject(projectUpdateBatch);

            // organizations
            ProjectOrganizationUpdate.CreateFromProject(projectUpdateBatch);

            ProjectPersonUpdate.CreateFromProject(projectUpdateBatch);

            // Documents
            ProjectDocumentUpdate.CreateFromProject(projectUpdateBatch);

            return projectUpdateBatch;
        }

        /// <summary>
        /// Only public for unit testing
        /// </summary>
        public static ProjectUpdateBatch CreateProjectUpdateBatchAndLogTransition(Project project, Person currentPerson)
        {
            var projectUpdateBatch = new ProjectUpdateBatch(project, DateTime.Now, currentPerson, ProjectUpdateState.Created, false);

            //move optional fields to the project update batch
            projectUpdateBatch.NoRegionsExplanation = project.NoRegionsExplanation;
            projectUpdateBatch.NoPriorityLandscapesExplanation = project.NoPriorityLandscapesExplanation;


            // create a project update history record
            CreateNewTransitionRecord(projectUpdateBatch, ProjectUpdateState.Created, currentPerson, DateTime.Now);
            return projectUpdateBatch;
        }

        public void SyncExpendituresYearsExemptionExplanation()
        {
            NoExpendituresToReportExplanation = Project.NoExpendituresToReportExplanation;
        }

        public void TickleLastUpdateDate(Person currentPerson)
        {
            TickleLastUpdateDate(DateTime.Now, currentPerson);
        }

        private void TickleLastUpdateDate(DateTime transitionDate, Person currentPerson)
        {
            LastUpdateDate = transitionDate;
            LastUpdatePerson = currentPerson;
        }

        private static void RefreshFromDatabase(IEnumerable updates)
        {
            ((IObjectContextAdapter)HttpRequestStorage.DatabaseEntities).ObjectContext.Refresh(RefreshMode.StoreWins, updates);
        }

        public void DeleteProjectImageUpdates()
        {
            DeleteProjectImageUpdates(ProjectImageUpdates);
            RefreshFromDatabase(ProjectImageUpdates);
        }

        public static void DeleteProjectImageUpdates(ICollection<ProjectImageUpdate> projectImageUpdates)
        {
            var projectImageFileResourceIDsToDelete = projectImageUpdates.Where(x => x.FileResourceID.HasValue).Select(x => x.FileResourceID.Value).ToList();
            HttpRequestStorage.DatabaseEntities.ProjectImageUpdates.DeleteProjectImageUpdate(projectImageUpdates);
            HttpRequestStorage.DatabaseEntities.FileResources.DeleteFileResource(projectImageFileResourceIDsToDelete);
        }

        public void DeleteProjectExternalLinkUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectExternalLinkUpdates.DeleteProjectExternalLinkUpdate(ProjectExternalLinkUpdates);
            RefreshFromDatabase(ProjectExternalLinkUpdates);
        }

        public void DeleteProjectNoteUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectNoteUpdates.DeleteProjectNoteUpdate(ProjectNoteUpdates);
            RefreshFromDatabase(ProjectNoteUpdates);
        }
        
        public void DeleteProjectDocumentUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectDocumentUpdates.DeleteProjectDocumentUpdate(ProjectDocumentUpdates);
            RefreshFromDatabase(ProjectDocumentUpdates);
        }

        public void DeleteProjectFundSourceAllocationRequestUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectFundSourceAllocationRequestUpdates.DeleteProjectFundSourceAllocationRequestUpdate(ProjectFundSourceAllocationRequestUpdates);
            RefreshFromDatabase(ProjectFundSourceAllocationRequestUpdates);
        }

        public void DeleteProjectFundingSourceUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectFundingSourceUpdates.DeleteProjectFundingSourceUpdate(ProjectFundingSourceUpdates);
            RefreshFromDatabase(ProjectFundingSourceUpdates);
        }

        public void DeleteProjectLocationUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectLocationUpdates.DeleteProjectLocationUpdate(ProjectLocationUpdates);
            RefreshFromDatabase(ProjectLocationUpdates);
        }

        public void DeleteProjectLocationStagingUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectLocationStagingUpdates.DeleteProjectLocationStagingUpdate(ProjectLocationStagingUpdates);
            RefreshFromDatabase(ProjectLocationStagingUpdates);
        }

        public void DeleteProjectPriorityLandscapeUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectPriorityLandscapeUpdates.DeleteProjectPriorityLandscapeUpdate(ProjectPriorityLandscapeUpdates);
            RefreshFromDatabase(ProjectPriorityLandscapeUpdates);
        }

        public void DeleteProjectRegionUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectRegionUpdates.DeleteProjectRegionUpdate(ProjectRegionUpdates);
            RefreshFromDatabase(ProjectRegionUpdates);
        }
        
        public void DeleteProjectOrganizationUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectOrganizationUpdates.DeleteProjectOrganizationUpdate(ProjectOrganizationUpdates);
            RefreshFromDatabase(ProjectOrganizationUpdates);
        }

        public void DeleteProjectUpdatePrograms()
        {
            HttpRequestStorage.DatabaseEntities.ProjectUpdatePrograms.DeleteProjectUpdateProgram(ProjectUpdatePrograms);
            RefreshFromDatabase(ProjectUpdatePrograms);
        }

        public void DeleteProjectContactUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectPersonUpdates.DeleteProjectPersonUpdate(ProjectPersonUpdates);
            RefreshFromDatabase(ProjectPersonUpdates);
        }

        public void DeleteTreatmentUpdates()
        {
            HttpRequestStorage.DatabaseEntities.TreatmentUpdates.DeleteTreatmentUpdate(TreatmentUpdates);
            RefreshFromDatabase(TreatmentUpdates);
        }

        public void DeleteAll()
        {
            DeleteFull(HttpRequestStorage.DatabaseEntities);
        }

        public BasicsValidationResult ValidateProjectBasics()
        {
            return new BasicsValidationResult(ProjectUpdate);
        }

        private bool? _areProjectBasicsValid;
        public bool AreProjectBasicsValid
        {
            get
            {
                if (!_areProjectBasicsValid.HasValue)
                {
                    _areProjectBasicsValid = ValidateProjectBasics().IsValid;
                }
                return _areProjectBasicsValid.Value;
            }
            private set => _areProjectBasicsValid = value;
        }


        public ExpectedFundingValidationResult ValidateExpectedFunding(List<ProjectFundSourceAllocationRequestSimple> newprojectFundSourceAllocationRequests)
        {
            return new ExpectedFundingValidationResult();
        }


        public OrganizationsValidationResult ValidateOrganizations()
        {
            return new OrganizationsValidationResult(ProjectOrganizationUpdates.Select(x => new ProjectOrganizationSimple(x))
                .ToList());
        }

        public bool AreOrganizationsValid()
        {
            return ValidateOrganizations().IsValid;
        }

        public ContactsValidationResult ValidateContacts()
        {
            return new ContactsValidationResult(ProjectPersonUpdates.Select(x => new ProjectPersonSimple(x))
                .ToList());
        }

        public bool AreContactsValid()
        {
            return ValidateContacts().IsValid;
        }

        public LocationSimpleValidationResult ValidateProjectLocationSimple()
        {           
            var incomplete = ProjectUpdate.ProjectLocationPoint == null &&
                             string.IsNullOrWhiteSpace(ProjectUpdate.ProjectLocationNotes);

            var locationSimpleValidationResult = new LocationSimpleValidationResult(incomplete);

            return locationSimpleValidationResult;
        }
        
        public bool IsProjectLocationSimpleValid()
        {
            return ValidateProjectLocationSimple().IsValid;
        }

        public DNRUplandRegionsValidationResult ValidateProjectRegion()
        {
            var incomplete = !ProjectRegionUpdates.Any() && string.IsNullOrWhiteSpace(NoRegionsExplanation);
            var regionValidationResult = new DNRUplandRegionsValidationResult(incomplete);
            return regionValidationResult;
        }

        public CountiesValidationResult ValidateProjectCounty()
        {
            var incomplete = !ProjectCountyUpdates.Any() && string.IsNullOrWhiteSpace(NoCountiesExplanation);
            var countyValidationResult = new CountiesValidationResult(incomplete);
            return countyValidationResult;
        }

        public bool IsProjectRegionValid()
        {
            return ValidateProjectRegion().IsValid;
        }

        public bool IsProjectCountyValid()
        {
            return ValidateProjectCounty().IsValid;
        }

        public void DeleteProjectCountyUpdates()
        {
            HttpRequestStorage.DatabaseEntities.ProjectCountyUpdates.DeleteProjectCountyUpdate(ProjectCountyUpdates);
            RefreshFromDatabase(ProjectCountyUpdates);
        }

        public PriorityLandscapesValidationResult ValidateProjectPriorityLandscape()
        {
            var incomplete = !ProjectPriorityLandscapeUpdates.Any() && string.IsNullOrWhiteSpace(NoPriorityLandscapesExplanation);
            var priorityLandscapeValidationResult = new PriorityLandscapesValidationResult(incomplete);
            return priorityLandscapeValidationResult;
        }

        public bool IsProjectPriorityLandscapeValid()
        {
            return ValidateProjectPriorityLandscape().IsValid;
        }

        public void SubmitToReviewer(Person currentPerson, DateTime transitionDate)
        {
            Check.Require(IsReadyToSubmit, $"You cannot submit a {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} update that is not ready to be submitted!");
            CreateNewTransitionRecord(this, ProjectUpdateState.Submitted, currentPerson, transitionDate);
        }

        public void Return(Person currentPerson, DateTime transitionDate)
        {
            Check.Require(IsSubmitted, $"You cannot return a {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} update that has not been submitted!");
            CreateNewTransitionRecord(this, ProjectUpdateState.Returned, currentPerson, transitionDate);
        }

        public void Approve(
            // TODO: Neutered per #1136; most likely will bring back when BOR project starts
            //IList<ProjectBudget> projectBudgets, 
            Person currentPerson, DateTime transitionDate,
            IList<ProjectFundingSource> projectFundingSources,
            IList<ProjectExternalLink> projectExternalLinks, IList<ProjectNote> projectNotes,
            IList<ProjectImage> projectImages, IList<ProjectLocation> projectLocations,
            IList<ProjectPriorityLandscape> projectPriorityLandscapes, 
            IList<ProjectRegion> projectRegions, 
            IList<ProjectCounty> projectCounties,
            IList<ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequests,
            IList<ProjectOrganization> allProjectOrganizations,
            IList<ProjectDocument> allProjectDocuments,
            IList<ProjectPerson> allProjectPersons,
            IList<ProjectProgram> allProjectPrograms,
            IList<Treatment> allTreatments)
        {
            Check.Require(IsSubmitted, $"You cannot approve a {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} update that has not been submitted!");
            CommitChangesToProject(projectFundingSources,
                // TODO: Neutered per #1136; most likely will bring back when BOR project starts
                //                projectBudgets,
                projectExternalLinks,
                projectNotes,
                projectImages,
                projectLocations,
                projectPriorityLandscapes,
                projectRegions,
                projectCounties,
                projectFundSourceAllocationRequests,
                allProjectOrganizations,
                allProjectDocuments,
                allProjectPersons,
                allProjectPrograms,
                allTreatments);
            CreateNewTransitionRecord(this, ProjectUpdateState.Approved, currentPerson, transitionDate);
            PushTransitionRecordsToAuditLog();
        }

        private void PushTransitionRecordsToAuditLog()
        {
            foreach (var projectUpdateHistory in ProjectUpdateHistories.ToList())
            {
                var auditLog = new AuditLog(projectUpdateHistory.UpdatePerson,
                    projectUpdateHistory.TransitionDate,
                    AuditLogEventType.Added,
                    $"{Models.FieldDefinition.Project.GetFieldDefinitionLabel()} Update",
                    projectUpdateHistory.ProjectUpdateHistoryID,
                    $"{Models.FieldDefinition.Project.GetFieldDefinitionLabel()} Update record",
                    projectUpdateHistory.ProjectUpdateState.ProjectUpdateStateDisplayName) {ProjectID = ProjectID};
            }
        }

        private void CommitChangesToProject( 
                // TODO: Neutered per #1136; most likely will bring back when BOR project starts
//            IList<ProjectBudget> projectBudgets,
                IList<ProjectFundingSource> projectFundingSources,
                IList<ProjectExternalLink> projectExternalLinks, IList<ProjectNote> projectNotes,
                IList<ProjectImage> projectImages, IList<ProjectLocation> projectLocations,
                IList<ProjectPriorityLandscape> projectPriorityLandscapes,
                IList<ProjectRegion> projectRegions,
                IList<ProjectCounty> projectCounties,
                IList<ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequests,
                IList<ProjectOrganization> allProjectOrganizations,
                IList<ProjectDocument> allProjectDocuments,
                IList<ProjectPerson> allProjectPeople,
                IList<ProjectProgram> allProjectPrograms,
                IList<Treatment> allTreatments)
        {
            // basics
            ProjectUpdate.CommitChangesToProject(Project);

            //Programs
            ProjectUpdateProgram.CommitChangesToProject(this, allProjectPrograms);

            // expected funding
            ProjectFundSourceAllocationRequestUpdate.CommitChangesToProject(this, projectFundSourceAllocationRequests);

            ProjectFundingSourceUpdate.CommitChangesToProject(this, projectFundingSources);

            // TODO: Neutered per #1136; most likely will bring back when BOR project starts
            //  project budgets
            //ProjectBudgetUpdate.CommitChangesToProject(this, projectBudgets);

            // project location simple
            ProjectUpdate.CommitSimpleLocationToProject(Project);

            // project location detailed
            ProjectLocationUpdate.CommitChangesToProject(this, projectLocations);

            // project priorityLandscape
            ProjectPriorityLandscapeUpdate.CommitChangesToProject(this, projectPriorityLandscapes);

            // project region
            ProjectRegionUpdate.CommitChangesToProject(this, projectRegions);

            // project county
            ProjectCountyUpdate.CommitChangesToProject(this, projectCounties);

            // photos
            ProjectImageUpdate.CommitChangesToProject(this, projectImages);
            IsPhotosUpdated = false;

            // external links
            ProjectExternalLinkUpdate.CommitChangesToProject(this, projectExternalLinks);

            // notes
            ProjectNoteUpdate.CommitChangesToProject(this, projectNotes);

            // Organizations
            ProjectOrganizationUpdate.CommitChangesToProject(this, allProjectOrganizations);

            // Organizations
            ProjectPersonUpdate.CommitChangesToProject(this, allProjectPeople);

            // Documents
            ProjectDocumentUpdate.CommitChangesToProject(this, allProjectDocuments);

            // Treatments
            TreatmentUpdate.CommitChangesToProject(this, allTreatments);
        }

        public void RejectSubmission(Person currentPerson, DateTime transitionDate)
        {
            Check.Require(IsSubmitted, "You cannot reject a batch that has not been submitted!");
            CreateNewTransitionRecord(this, ProjectUpdateState.Returned, currentPerson, transitionDate);
        }

        /// <summary>
        /// Note, the saving is done by the controller
        /// </summary>
        private static void CreateNewTransitionRecord(ProjectUpdateBatch projectUpdateBatch, ProjectUpdateState projectUpdateState, Person currentPerson, DateTime transitionDate)
        {
            var projectUpdateHistory = new ProjectUpdateHistory(projectUpdateBatch, projectUpdateState, currentPerson, transitionDate);
            HttpRequestStorage.DatabaseEntities.ProjectUpdateHistories.Add(projectUpdateHistory);
            projectUpdateBatch.ProjectUpdateStateID = projectUpdateState.ProjectUpdateStateID;
            projectUpdateBatch.TickleLastUpdateDate(transitionDate, currentPerson);
        }

        public bool AreAccomplishmentsRelevant()
        {
            var projectStage = ProjectUpdate == null ? Project.ProjectStage : ProjectUpdate.ProjectStage;
            return projectStage != ProjectStage.Planned;
        }

        public List<ProjectSectionSimple> GetApplicableWizardSections(bool ignoreStatus)
        {
            return ProjectWorkflowSectionGrouping.All.SelectMany(x => x.GetProjectUpdateSections(this, null, ignoreStatus)).OrderBy(x => x.ProjectWorkflowSectionGrouping.SortOrder).ThenBy(x => x.SortOrder).ToList();
        }

        public FeatureCollection AllDetailedLocationsToGeoJsonFeatureCollection()
        {
            return ProjectLocationUpdates.ToGeoJsonFeatureCollection();
        }

        public bool HasProjectLocationDetail => AllDetailedLocationsToGeoJsonFeatureCollection().Features.Any();

        public void AutoAssignProjectPriorityLandscapesAndDnrUplandRegionsAndCounties()
        {
            var detailedProjectLocations = ProjectLocationUpdates.Select(x => x.ProjectLocationGeometry).ToList();
            var projectHasDetailedLocations = HasProjectLocationDetail;
            var detailedProjectLocationsAggregated = HasProjectLocationDetail ? detailedProjectLocations.Aggregate((x, y) => x.Union(y)) : null;
            var detailedProjectLocationsAggregatedMadeValid = HasProjectLocationDetail ? (detailedProjectLocationsAggregated.IsValid ? detailedProjectLocationsAggregated : detailedProjectLocationsAggregated.ToSqlGeometry().MakeValid().ToDbGeometryWithCoordinateSystem()) : null;

            var projectLocationPoint = this.ProjectUpdate.ProjectLocationPoint;
            var projectLocationPointExists = this.ProjectUpdate.HasProjectLocationPoint;

            var updatedProjectPriorityLandscapes = HttpRequestStorage.DatabaseEntities.PriorityLandscapes
                .Where(x => (projectHasDetailedLocations && x.PriorityLandscapeLocation.Intersects(detailedProjectLocationsAggregatedMadeValid)) || (projectLocationPointExists && x.PriorityLandscapeLocation.Intersects(projectLocationPoint)))
                .ToList()
                .Select(x => new ProjectPriorityLandscapeUpdate(ProjectUpdateBatchID, x.PriorityLandscapeID))
                .ToList();

            if (!updatedProjectPriorityLandscapes.Any())
            {
                NoPriorityLandscapesExplanation =
                    "Neither the simple location nor the detailed location on this project intersects with any Priority Landscape.";
            }
            else
            {
                NoPriorityLandscapesExplanation = null;
            }


            ProjectPriorityLandscapeUpdates.Merge(updatedProjectPriorityLandscapes, HttpRequestStorage.DatabaseEntities.ProjectPriorityLandscapeUpdates.Local, (x, y) => x.ProjectUpdateBatchID == y.ProjectUpdateBatchID && x.PriorityLandscapeID == y.PriorityLandscapeID);

            var updatedProjectRegions = HttpRequestStorage.DatabaseEntities.DNRUplandRegions
                .Where(x => (projectHasDetailedLocations && x.DNRUplandRegionLocation.Intersects(detailedProjectLocationsAggregatedMadeValid)) || (projectLocationPointExists && x.DNRUplandRegionLocation.Intersects(projectLocationPoint)))
                .ToList()
                .Select(x => new ProjectRegionUpdate(ProjectUpdateBatchID, x.DNRUplandRegionID))
                .ToList();

            if (!updatedProjectRegions.Any())
            {
                NoRegionsExplanation =
                    "Neither the simple location nor the detailed location on this project intersects with any DNR Upland Region.";
            }
            else
            {
                NoRegionsExplanation = null;
            }

            ProjectRegionUpdates.Merge(updatedProjectRegions, HttpRequestStorage.DatabaseEntities.ProjectRegionUpdates.Local, (x, y) => x.ProjectUpdateBatchID == y.ProjectUpdateBatchID && x.DNRUplandRegionID == y.DNRUplandRegionID);

            var updatedProjectCounties = HttpRequestStorage.DatabaseEntities.Counties
                .Where(x => (projectHasDetailedLocations && x.CountyFeature.Intersects(detailedProjectLocationsAggregatedMadeValid)) || (projectLocationPointExists && x.CountyFeature.Intersects(projectLocationPoint)))
                .ToList()
                .Select(x => new ProjectCountyUpdate(ProjectUpdateBatchID, x.CountyID))
                .ToList();

            if (!updatedProjectCounties.Any())
            {
                NoCountiesExplanation =
                    "Neither the simple location nor the detailed location on this project intersects with any county.";
            }
            else
            {
                NoCountiesExplanation = null;
            }

            ProjectCountyUpdates.Merge(updatedProjectCounties, HttpRequestStorage.DatabaseEntities.ProjectCountyUpdates.Local, (x, y) => x.ProjectUpdateBatchID == y.ProjectUpdateBatchID && x.CountyID == y.CountyID);
        }
    }
}
