/*-----------------------------------------------------------------------
<copyright file="ProjectUpdateController.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using LtInfo.Common;
using LtInfo.Common.DbSpatial;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using LtInfo.Common.MvcResults;
using MoreLinq;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.ScheduledJobs;
using ProjectFirma.Web.Security;
using ProjectFirma.Web.Security.Shared;
using ProjectFirma.Web.Views.Map;
using ProjectFirma.Web.Views.Project;
using ProjectFirma.Web.Views.ProjectCounty;
using ProjectFirma.Web.Views.ProjectExternalLink;
using ProjectFirma.Web.Views.ProjectFunding;
using ProjectFirma.Web.Views.ProjectPriorityLandscape;
using ProjectFirma.Web.Views.ProjectRegion;
using ProjectFirma.Web.Views.ProjectUpdate;
using ProjectFirma.Web.Views.Shared;
using ProjectFirma.Web.Views.Shared.ProjectControls;
using ProjectFirma.Web.Views.Shared.ProjectLocationControls;
using ProjectFirma.Web.Views.Shared.ProjectOrganization;
using ProjectFirma.Web.Views.Shared.ProjectPerson;
using ProjectFirma.Web.Views.Shared.ProjectUpdateDiffControls;
using ProjectFirma.Web.Views.Shared.TextControls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Basics = ProjectFirma.Web.Views.ProjectUpdate.Basics;
using BasicsViewData = ProjectFirma.Web.Views.ProjectUpdate.BasicsViewData;
using BasicsViewModel = ProjectFirma.Web.Views.ProjectUpdate.BasicsViewModel;
using ExpectedFunding = ProjectFirma.Web.Views.ProjectUpdate.ExpectedFunding;
using ExpectedFundingViewData = ProjectFirma.Web.Views.ProjectUpdate.ExpectedFundingViewData;
using ExpectedFundingViewModel = ProjectFirma.Web.Views.ProjectUpdate.ExpectedFundingViewModel;
using LocationDetailed = ProjectFirma.Web.Views.ProjectUpdate.LocationDetailed;
using LocationDetailedViewData = ProjectFirma.Web.Views.ProjectUpdate.LocationDetailedViewData;
using LocationDetailedViewModel = ProjectFirma.Web.Views.ProjectUpdate.LocationDetailedViewModel;
using LocationSimple = ProjectFirma.Web.Views.ProjectUpdate.LocationSimple;
using LocationSimpleViewData = ProjectFirma.Web.Views.ProjectUpdate.LocationSimpleViewData;
using LocationSimpleViewModel = ProjectFirma.Web.Views.ProjectUpdate.LocationSimpleViewModel;
using Photos = ProjectFirma.Web.Views.ProjectUpdate.Photos;

namespace ProjectFirma.Web.Controllers
{
    public class ProjectUpdateController : FirmaBaseController
    {
        public const string ProjectUpdateBatchDiffLogPartialViewPath = "~/Views/ProjectUpdate/ProjectUpdateBatchDiffLog.cshtml";
        public const string ProjectBasicsPartialViewPath = "~/Views/Shared/ProjectControls/ProjectBasics.cshtml";
        public const string ProjectExpectedFundingPartialViewPath = "~/Views/Shared/ProjectUpdateDiffControls/ProjectFundSourceAllocationRequestsDetail.cshtml";
        public const string ImageGalleryPartialViewPath = "~/Views/Shared/ImageGallery.cshtml";
        public const string ExternalLinksPartialViewPath = "~/Views/Shared/TextControls/EntityExternalLinks.cshtml";
        public const string EntityNotesPartialViewPath = "~/Views/Shared/TextControls/EntityNotes.cshtml";
        public const string ProjectOrganizationsPartialViewPath = "~/Views/Shared/ProjectOrganization/ProjectOrganizationsDetail.cshtml";
        public const string ProjectPeoplePartialViewPath = "~/Views/Shared/ProjectPerson/ProjectPeopleDetail.cshtml";
        //public const string ProjectDocumentsPartialViewPath = "~/Views/Shared/ProjectDocument/ProjectDocumentsDetail.cshtml";

        [LoggedInAndNotUnassignedRoleUnclassifiedFeature]
        public ViewResult AllMyProjects()
        {
            const ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum filterTypeEnum = ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.AllMyProjects;
            var gridDataUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(tc => tc.IndexGridJsonData(filterTypeEnum));
            return ViewIndex(gridDataUrl, filterTypeEnum);
        }

        [LoggedInAndNotUnassignedRoleUnclassifiedFeature]
        public ViewResult MySubmittedProjects()
        {
            const ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum filterTypeEnum = ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.MySubmittedProjects;
            var gridDataUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(tc => tc.IndexGridJsonData(filterTypeEnum));
            return ViewIndex(gridDataUrl, filterTypeEnum);
        }

        [LoggedInAndNotUnassignedRoleUnclassifiedFeature]
        public ViewResult MyProjectsRequiringAnUpdate()
        {
            const ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum filterTypeEnum = ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.MyProjectsRequiringAnUpdate;
            var gridDataUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(tc => tc.IndexGridJsonData(filterTypeEnum));
            return ViewIndex(gridDataUrl, filterTypeEnum);
        }

        [ProjectUpdateAdminFeature]
        public ViewResult AllProjects()
        {
            const ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum filterTypeEnum = ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.AllProjects;
            var gridDataUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(tc => tc.IndexGridJsonData(filterTypeEnum));
            return ViewIndex(gridDataUrl, filterTypeEnum);
        }

        [ProjectUpdateAdminFeature]
        public ViewResult SubmittedProjects()
        {
            const ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum filterTypeEnum = ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.SubmittedProjects;
            var gridDataUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(tc => tc.IndexGridJsonData(filterTypeEnum));
            return ViewIndex(gridDataUrl, filterTypeEnum);
        }

        private ViewResult ViewIndex(string gridDataUrl, ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum projectUpdateStatusFilterTypeEnum)
        {
            var firmaPage = FirmaPage.GetFirmaPageByPageType(FirmaPageType.MyProjects);
            var viewData = new MyProjectsViewData(CurrentPerson, firmaPage, projectUpdateStatusFilterTypeEnum, gridDataUrl);
            return RazorView<MyProjects, MyProjectsViewData>(viewData);
        }

        [LoggedInAndNotUnassignedRoleUnclassifiedFeature]
        public GridJsonNetJObjectResult<Project> IndexGridJsonData(ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum projectUpdateStatusFilterType)
        {
            var gridSpec = new ProjectUpdateStatusGridSpec(projectUpdateStatusFilterType, CurrentPerson.IsApprover());
            var projects = HttpRequestStorage.DatabaseEntities.Projects.ToList().GetActiveProjectsVisibleToUser(CurrentPerson).Where(p => p.IsUpdatableViaProjectUpdateProcess);

            switch (projectUpdateStatusFilterType)
            {
                case ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.AllMyProjects:
                    projects = projects.Where(p => p.IsMyProject(CurrentPerson));
                    break;
                case ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.MyProjectsRequiringAnUpdate:
                    projects = projects.Where(p => p.IsMyProject(CurrentPerson) && p.IsUpdateMandatory && p.GetLatestUpdateState() != ProjectUpdateState.Submitted);
                    break;
                case ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.MySubmittedProjects:
                    projects = projects.Where(p => p.IsMyProject(CurrentPerson) && (!p.IsUpdateMandatory || p.GetLatestUpdateState() == ProjectUpdateState.Submitted));
                    break;
                case ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.SubmittedProjects:
                    projects = projects.Where(p =>
                    {
                        var latestUpdateState = p.GetLatestUpdateState();
                        return latestUpdateState != null && latestUpdateState == ProjectUpdateState.Submitted;
                    });
                    break;
                case ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.AllProjects:
                    break;
            }
            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<Project>(projects.OrderBy(x => x.DisplayName).ToList(), gridSpec);
            return gridJsonNetJObjectResult;
        }

        [HttpGet]
        [ProjectUpdateAdminFeature]
        public PartialViewResult EditProjectUpdateConfiguration()
        {
            EditProjectUpdateConfigurationViewModel viewModel = new EditProjectUpdateConfigurationViewModel(MultiTenantHelpers.GetProjectUpdateConfiguration());
            return ViewEditProjectUpdateConfiguration(viewModel);
        }

        private PartialViewResult ViewEditProjectUpdateConfiguration(EditProjectUpdateConfigurationViewModel viewModel)
        {
            var viewData = new EditProjectUpdateConfigurationViewData(CurrentPerson);
            return RazorPartialView<EditProjectUpdateConfiguration, EditProjectUpdateConfigurationViewData, EditProjectUpdateConfigurationViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProjectUpdateAdminFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult EditProjectUpdateConfiguration(EditProjectUpdateConfigurationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return ViewEditProjectUpdateConfiguration(viewModel);
            }

            viewModel.UpdateModel(MultiTenantHelpers.GetProjectUpdateConfiguration());
            SetMessageForDisplay("Notifications configured successfully.");

            return new ModalDialogFormJsonResult();
        }


        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult StartProjectUpdate(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var viewModel = new ConfirmDialogFormViewModel(projectPrimaryKey.PrimaryKeyValue);
            var viewData = new ConfirmDialogFormViewData($"To make changes to the project you must start a {FieldDefinition.Project.GetFieldDefinitionLabel()} update. <br/>The reviewer will then receive your update and either approve or return your {FieldDefinition.Project.GetFieldDefinitionLabel()} update request.", true);
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult StartProjectUpdate(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            ProjectUpdateBatch.GetLatestNotApprovedProjectUpdateBatchOrCreateNewAndSaveToDatabase(project, CurrentPerson);
            return new ModalDialogFormJsonResult(new SitkaRoute<ProjectUpdateController>(x => x.Basics(project)).BuildUrlFromExpression()); 
        }


        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult Basics(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            var viewModel = new BasicsViewModel(projectUpdate, projectUpdateBatch.BasicsComment);
            return ViewBasics(projectUpdate, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Basics(ProjectPrimaryKey projectPrimaryKey, BasicsViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            if (!ModelState.IsValid)
            {
                return ViewBasics(projectUpdate, viewModel);
            }
            if (!ModelObjectHelpers.IsRealPrimaryKeyValue(projectUpdate.ProjectUpdateID))
            {
                HttpRequestStorage.DatabaseEntities.ProjectUpdates.Add(projectUpdate);
            }
            viewModel.UpdateModel(projectUpdate, CurrentPerson);
            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.BasicsComment = viewModel.Comments;
            }
            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch,
                ProjectUpdateSection.Basics.ProjectUpdateSectionDisplayName);
        }

        private ViewResult ViewBasics(ProjectUpdate projectUpdate, BasicsViewModel viewModel)
        {
            var basicsValidationResult = projectUpdate.ProjectUpdateBatch.ValidateProjectBasics();
            var updateStatus = GetUpdateStatus(projectUpdate.ProjectUpdateBatch); // note, the way the diff for the basics section is built, it will actually "commit" the updated values to the project, so it needs to be done last, or we need to change the current approach

            var projectStages = projectUpdate.ProjectUpdateBatch.Project.ProjectStage.GetProjectStagesThatProjectCanUpdateTo();
            var focusAreas = HttpRequestStorage.DatabaseEntities.FocusAreas.ToList();
            var allPrograms = HttpRequestStorage.DatabaseEntities.Programs.ToList();
            var leadImplementerRelationshipType = HttpRequestStorage.DatabaseEntities.RelationshipTypes
                .Include(relationshipType => relationshipType.OrganizationTypeRelationshipTypes).SingleOrDefault(x => x.RelationshipTypeID == RelationshipType.LeadImplementerID);
            Check.EnsureNotNull(leadImplementerRelationshipType, "Lead Implementer Relationship Type cannot be found");
            var organizationTypesAvailableForLeadImplementer = leadImplementerRelationshipType.OrganizationTypeRelationshipTypes.Select(x => x.OrganizationTypeID).ToList();
            var organizations = HttpRequestStorage.DatabaseEntities.Organizations.Where(x => organizationTypesAvailableForLeadImplementer.Contains(x.OrganizationTypeID)).ToList();
            var firmaPageType = FirmaPageType.ToType(FirmaPageTypeEnum.ProjectUpdateInstructions);
            var firmaPage = FirmaPage.GetFirmaPageByPageType(firmaPageType);
            var viewData = new BasicsViewData(CurrentPerson, projectUpdate, projectStages, updateStatus, basicsValidationResult, focusAreas, allPrograms, Models.Project.ImportedFieldWarningMessage, firmaPage, organizations);
            return RazorView<Basics, BasicsViewData, BasicsViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshBasics(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshBasics(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshBasics(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            if (projectUpdate != null)
            {
                projectUpdate.LoadUpdateFromProject(project);
                projectUpdateBatch.DeleteProjectUpdatePrograms();
                projectUpdate.LoadProgramsFromProject(project);
                projectUpdateBatch.DeleteProjectOrganizationUpdates();
                // refresh data
                ProjectOrganizationUpdate.CreateFromProject(projectUpdateBatch);
                projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            }
            return new ModalDialogFormJsonResult(SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.Basics(projectPrimaryKey)));
        }

        private PartialViewResult ViewRefreshBasics(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the {FieldDefinition.Project.GetFieldDefinitionLabel()} basics data? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        private static ProjectUpdateBatch GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(Project project)
        {
            return GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project,
                $"We should have a {FieldDefinition.Project.GetFieldDefinitionLabel()} update batch when refreshing; didn't find one for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
        }

        private static ProjectUpdateBatch GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(Project project, string message)
        {
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            Check.RequireNotNull(projectUpdateBatch, message);
            return projectUpdateBatch;
        }


        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult ExpectedFunding(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }

            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            var projectFundSourceAllocationRequestUpdates = projectUpdateBatch.ProjectFundSourceAllocationRequestUpdates.ToList();
            var viewModel = new ExpectedFundingViewModel(projectFundSourceAllocationRequestUpdates,
                                                         projectUpdateBatch.ExpectedFundingComment, 
                                                         projectUpdate.EstimatedTotalCost,
                                                         projectUpdate.ProjectFundingSourceNotes,
                                                         projectUpdateBatch.ProjectFundingSourceUpdates.ToList());
            return ViewExpectedFunding(projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult ExpectedFunding(ProjectPrimaryKey projectPrimaryKey, ExpectedFundingViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            if (!ModelState.IsValid)
            {
                return ViewExpectedFunding(projectUpdateBatch, viewModel);
            }

            HttpRequestStorage.DatabaseEntities.ProjectFundSourceAllocationRequestUpdates.Load();
            var currentProjectFundSourceAllocationRequestUpdates = projectUpdateBatch.ProjectFundSourceAllocationRequestUpdates.ToList();
            var allProjectFundSourceAllocationExpectedFunding = HttpRequestStorage.DatabaseEntities.ProjectFundSourceAllocationRequestUpdates.Local;

            HttpRequestStorage.DatabaseEntities.ProjectFundingSourceUpdates.Load();
            var projectFundingSourceUpdates = projectUpdateBatch.ProjectFundingSourceUpdates.ToList();
            var allProjectFundingSourceUpdates = HttpRequestStorage.DatabaseEntities.ProjectFundingSourceUpdates.Local;

            viewModel.UpdateModel(projectUpdateBatch, currentProjectFundSourceAllocationRequestUpdates, allProjectFundSourceAllocationExpectedFunding, projectUpdateBatch.ProjectUpdate, projectFundingSourceUpdates, allProjectFundingSourceUpdates);

            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.ExpectedFundingComment = viewModel.Comments;
            }

            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch,
                ProjectUpdateSection.ExpectedFunding.ProjectUpdateSectionDisplayName);
        }

        private ViewResult ViewExpectedFunding(ProjectUpdateBatch projectUpdateBatch, ExpectedFundingViewModel viewModel)
        {
            var allFundSourceAllocations = HttpRequestStorage.DatabaseEntities.FundSourceAllocations.ToList().Select(x => new FundSourceAllocationSimple(x)).OrderBy(p => p.DisplayName).ToList();
            var expectedFundingValidationResult = projectUpdateBatch.ValidateExpectedFunding(viewModel.ProjectFundSourceAllocationRequests);
            var estimatedTotalCost = projectUpdateBatch.ProjectUpdate.EstimatedTotalCost.GetValueOrDefault();

            var viewDataForAngularEditor = new ExpectedFundingViewData.ViewDataForAngularClass(projectUpdateBatch,
                allFundSourceAllocations,
                estimatedTotalCost 
                );
            var isLoa = projectUpdateBatch.Project.ProjectPrograms.Any(x => x.ProgramID == ProjectController.LoaProgramID);
            var projectFundingDetailViewData = new ProjectFundingDetailViewData(CurrentPerson, new List<IFundSourceAllocationRequestAmount>(projectUpdateBatch.ProjectFundSourceAllocationRequestUpdates), isLoa);

            var viewData = new ExpectedFundingViewData(CurrentPerson, projectUpdateBatch, viewDataForAngularEditor, projectFundingDetailViewData, GetUpdateStatus(projectUpdateBatch), expectedFundingValidationResult);
            return RazorView<ExpectedFunding, ExpectedFundingViewData, ExpectedFundingViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshExpectedFunding(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshExpectedFunding(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshExpectedFunding(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            projectUpdateBatch.DeleteProjectFundSourceAllocationRequestUpdates();
            projectUpdateBatch.DeleteProjectFundingSourceUpdates();
            // refresh data
            ProjectFundSourceAllocationRequestUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.ProjectUpdate.EstimatedTotalCost = project.EstimatedTotalCost;
            projectUpdateBatch.ProjectUpdate.ProjectFundingSourceNotes = project.ProjectFundingSourceNotes;
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshExpectedFunding(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the expected funding for this {FieldDefinition.Project.GetFieldDefinitionLabel()}? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()}. Any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult Photos(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var viewData = new PhotosViewData(CurrentPerson, projectUpdateBatch, updateStatus);
            return RazorView<Photos, PhotosViewData>(viewData);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshPhotos(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshPhotos(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshPhotos(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            projectUpdateBatch.DeleteProjectImageUpdates();
            // finally create a new project update record, refreshing with the current project data at this point in time
            ProjectImageUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.IsPhotosUpdated = false;
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshPhotos(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the photos for this {FieldDefinition.Project.GetFieldDefinitionLabel()}? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult LocationSimple(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            var viewModel = new LocationSimpleViewModel(projectUpdate.ProjectLocationPoint,
                projectUpdate.ProjectLocationSimpleType.ToEnum,
                projectUpdate.ProjectLocationNotes,
                projectUpdateBatch.LocationSimpleComment);
            return ViewLocationSimple(project, projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult LocationSimple(ProjectPrimaryKey projectPrimaryKey, LocationSimpleViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            if (!ModelState.IsValid)
            {
                return ViewLocationSimple(project, projectUpdateBatch, viewModel);
            }
            viewModel.UpdateModelBatch(projectUpdateBatch);
            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.LocationSimpleComment = viewModel.Comments;
            }

            projectUpdateBatch.AutoAssignProjectPriorityLandscapesAndDnrUplandRegionsAndCounties();

            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch,
                ProjectUpdateSection.LocationSimple.ProjectUpdateSectionDisplayName);
        }

        private ViewResult ViewLocationSimple(Project project, ProjectUpdateBatch projectUpdateBatch, LocationSimpleViewModel viewModel)
        {          
            var projectUpdate = projectUpdateBatch.ProjectUpdate;

            var mapInitJsonForEdit = new MapInitJson($"project_{project.ProjectID}_EditMap",
                10,
                MapInitJson.GetAllGeospatialAreaMapLayers(LayerInitialVisibility.Hide),
                MapInitJson.GetExternalMapLayersForOtherMaps(),
                BoundingBox.MakeNewDefaultBoundingBox(),
                false) {DisablePopups = true};
            var locationSimpleValidationResult = projectUpdateBatch.ValidateProjectLocationSimple();

            var projectLocationSummaryMapInitJson = new ProjectLocationSummaryMapInitJson(projectUpdate,
                $"project_{project.ProjectID}_EditMap", true);

            var mapPostUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(c => c.LocationSimple(project, null));
            var mapFormID = GenerateEditProjectLocationFormID(project);
            var editProjectLocationViewData = new ProjectLocationSimpleViewData(CurrentPerson, mapInitJsonForEdit, FirmaWebConfiguration.GetWmsLayerNames(), null, mapPostUrl, mapFormID, FirmaWebConfiguration.WebMapServiceUrl);
            var projectLocationSummaryViewData = new ProjectLocationSummaryViewData(projectUpdate, projectLocationSummaryMapInitJson, new List<PriorityLandscape>(), new List<DNRUplandRegion>(), string.Empty, string.Empty, new List<County>(), String.Empty);
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var viewData = new LocationSimpleViewData(CurrentPerson, projectUpdate, editProjectLocationViewData, projectLocationSummaryViewData, locationSimpleValidationResult, updateStatus);
            return RazorView<LocationSimple, LocationSimpleViewData, LocationSimpleViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshProjectLocationSimple(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshProjectLocationSimple(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshProjectLocationSimple(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            if (projectUpdate == null)
            {
                return new ModalDialogFormJsonResult();
            }
            projectUpdate.LoadSimpleLocationFromProject(project);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshProjectLocationSimple(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the {FieldDefinition.ProjectLocation.GetFieldDefinitionLabel()} data? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult LocationDetailed(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var viewModel = new LocationDetailedViewModel(projectUpdateBatch.LocationDetailedComment, projectUpdateBatch.ProjectLocationUpdates);
            return ViewLocationDetailed(projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult LocationDetailed(ProjectPrimaryKey projectPrimaryKey, LocationDetailedViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            if (!ModelState.IsValid)
            {
                return ViewLocationDetailed(projectUpdateBatch, viewModel);
            }

            // MCS: This check needs to be done separate from the other validation checks; if it fails we need to recreate the ViewModel
            // This is necessary to add back the ProjectLocationUpdates that should not have been deleted
            var deletionErrors = ValidateProjectLocationUpdateDeletions(projectUpdateBatch, viewModel);
            deletionErrors.ForEach(x => ModelState.AddModelError("Validation", x));
            if (!ModelState.IsValid)
            {
                return ViewLocationDetailed(projectUpdateBatch, new LocationDetailedViewModel(projectUpdateBatch.LocationDetailedComment, projectUpdateBatch.ProjectLocationUpdates));
            }

            SaveProjectLocationUpdates(viewModel, projectUpdateBatch);

            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.LocationDetailedComment = viewModel.Comments;
            }
            projectUpdateBatch.AutoAssignProjectPriorityLandscapesAndDnrUplandRegionsAndCounties();
            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch, ProjectUpdateSection.LocationDetailed.ProjectUpdateSectionDisplayName);
        }

        private List<string> ValidateProjectLocationUpdateDeletions(ProjectUpdateBatch projectUpdateBatch, LocationDetailedViewModel viewModel)
        {
            var result = new List<string>();

            var existingProjectLocationUpdates = projectUpdateBatch.ProjectLocationUpdates;
            // ProjectLocationJson.ProjectLocationID is actually a ProjectLocationUpdateID
            var updatedProjectLocationUpdateIDs = viewModel.ProjectLocationJsons.Select(x => x.ProjectLocationID)
                .Union(viewModel.ArcGisProjectLocationJsons.Select(y => y.ProjectLocationID))
                .ToList();
            var deletedProjectLocationUpdates = existingProjectLocationUpdates.Where(x => !updatedProjectLocationUpdateIDs.Contains(x.ProjectLocationUpdateID)).ToList();
            foreach (var dplu in deletedProjectLocationUpdates)
            {
                if (dplu.TreatmentUpdates.Any())
                {
                    result.Add($"Project Location Update {dplu.ProjectLocationUpdateName} cannot be deleted because it has Treatment Updates.");
                }
            }

            return result;
        }

        private ViewResult ViewLocationDetailed(ProjectUpdateBatch projectUpdateBatch, LocationDetailedViewModel viewModel)
        {
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            var project = projectUpdateBatch.Project;

            var mapDivID = $"project_{project.ProjectID}_EditDetailedMap";
            var detailedLocationGeoJsonFeatureCollection = projectUpdateBatch.ProjectLocationUpdates.Where(pl => !pl.ArcGisObjectID.HasValue).ToGeoJsonFeatureCollection(); ;
            var editableLayerGeoJson = new LayerGeoJson($"{FieldDefinition.ProjectLocation.GetFieldDefinitionLabel()} Detail", detailedLocationGeoJsonFeatureCollection, "red", 1, LayerInitialVisibility.Show);

            var arcGisLocationGeoJsonFeatureCollection = projectUpdateBatch.ProjectLocationUpdates.Where(pl => pl.ArcGisObjectID.HasValue).ToGeoJsonFeatureCollection();
            var arcGisLayerGeoJson = new LayerGeoJson($"{FieldDefinition.ProjectLocation.GetFieldDefinitionLabel()} Detail", arcGisLocationGeoJsonFeatureCollection, "red", 1, LayerInitialVisibility.Show);

            var boundingBox = ProjectLocationSummaryMapInitJson.GetProjectBoundingBox(projectUpdate);
            var layers = MapInitJson.GetAllGeospatialAreaMapLayers(LayerInitialVisibility.Show);
            layers.AddRange(MapInitJson.GetProjectLocationSimpleMapLayer(projectUpdate));
            var mapInitJson = new MapInitJson(mapDivID, 10, layers, MapInitJson.GetExternalMapLayersForOtherMaps(), boundingBox) {AllowFullScreen = false, DisablePopups = true};
            var mapFormID = ProjectLocationController.GenerateEditProjectLocationFormID(projectUpdateBatch.ProjectID);
            var uploadGisFileUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(c => c.ImportGdbFile(project.ProjectID));
            var saveFeatureCollectionUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.LocationDetailed(project.ProjectID, null));

            var hasSimpleLocationPoint = projectUpdate.ProjectLocationPoint != null;

            var projectLocationDetailViewData = new ProjectLocationDetailViewData(projectUpdateBatch.ProjectID,
                mapInitJson,
                editableLayerGeoJson,
                arcGisLayerGeoJson,
                uploadGisFileUrl,
                mapFormID,
                saveFeatureCollectionUrl,
                ProjectLocationUpdate.FieldLengths.ProjectLocationUpdateNotes,
                hasSimpleLocationPoint);
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var viewData = new LocationDetailedViewData(CurrentPerson, projectUpdateBatch, projectLocationDetailViewData, uploadGisFileUrl, updateStatus);
            return RazorView<LocationDetailed, LocationDetailedViewData, LocationDetailedViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshProjectLocationDetailed(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshProjectLocationDetailed(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshProjectLocationDetailed(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);

            // Validate refresh will not orphan any Treatment records
            var isValid = LocationDetailedViewData.CanRevertLocationDetailed(projectUpdateBatch);
            if (!isValid)
            {
                SetErrorForDisplay("Reverting project locations would delete Treatment Areas associated with Treatments.");
                return new ModalDialogFormJsonResult();
            }

            // We need to consider that TreatmentUpdates could have foreign keys to the ProjectLocationUpdates being refreshed
            projectUpdateBatch.DeleteProjectLocationStagingUpdates();

            // Remove project location updates 
            //var projectLocationUpdatesToDelete = projectUpdateBatch.ProjectLocationUpdates.ToList();

            RevertProjectLocationUpdates(project, projectUpdateBatch);

            //recreate project location updates from project locations
            //ProjectLocationUpdate.CreateFromProject(projectUpdateBatch);


            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        public static bool RevertProjectLocationUpdates(Project project, ProjectUpdateBatch projectUpdateBatch)
        {

            try
            {
                HttpRequestStorage.DatabaseEntities.ProjectLocationUpdates.Load();
                var allProjectLocationUpdates = HttpRequestStorage.DatabaseEntities.ProjectLocationUpdates.Local;
                //var allInDb = HttpRequestStorage.DatabaseEntities.ProjectLocationUpdates.ToList();//.DeleteProjectLocationUpdate(projectLocationUpdatesToDelete);

                var projectLocationUpdatesFromProject = project.ProjectLocations.Select(pl => new ProjectLocationUpdate(projectUpdateBatch,
                    pl.ProjectLocationGeometry,
                    pl.ProjectLocationNotes,
                    pl.ProjectLocationTypeID,
                    pl.ProjectLocationName,
                    pl.ArcGisObjectID,
                    pl.ArcGisGlobalID
                )).ToList();
                projectUpdateBatch.ProjectLocationUpdates.Merge(projectLocationUpdatesFromProject, allProjectLocationUpdates,
                    (x, y) => x.ProjectUpdateBatchID == y.ProjectUpdateBatchID && x.ProjectLocationUpdateName == y.ProjectLocationUpdateName,
                    (x, y) =>
                    {
                        x.ProjectLocationGeometry = y.ProjectLocationGeometry;
                        x.ProjectLocationTypeID = y.ProjectLocationTypeID;
                        x.ProjectLocationNotes = y.ProjectLocationNotes;
                        x.ArcGisGlobalID = y.ArcGisGlobalID;
                        x.ArcGisObjectID = y.ArcGisObjectID;
                });
    
            }
            catch (Exception )
            {
                return false;
            }
            
            return true;
        }

        private PartialViewResult ViewRefreshProjectLocationDetailed(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the {FieldDefinition.ProjectLocation.GetFieldDefinitionLabel()} data? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult ImportGdbFile(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var viewModel = new ImportGdbFileViewModel();
            return ViewImportGdbFile(project, viewModel);
        }

        private PartialViewResult ViewImportGdbFile(ProjectPrimaryKey projectPrimaryKey, ImportGdbFileViewModel viewModel)
        {
            var projectID = projectPrimaryKey.EntityObject.ProjectID;
            var mapFormID = ProjectLocationController.GenerateEditProjectLocationFormID(projectID);
            var newGisUploadUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.ImportGdbFile(projectID, null));
            var approveGisUploadUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.ApproveGisUpload(projectID, null));
            var viewData = new ImportGdbFileViewData(mapFormID, newGisUploadUrl, approveGisUploadUrl);
            return RazorPartialView<ImportGdbFile, ImportGdbFileViewData, ImportGdbFileViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult ImportGdbFile(ProjectPrimaryKey projectPrimaryKey, ImportGdbFileViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            if (!ModelState.IsValid)
            {
                return ViewImportGdbFile(project, viewModel);
            }

            var httpPostedFileBase = viewModel.FileResourceData;
            var fileEnding = ".gdb.zip";
            using (var disposableTempFile = DisposableTempFile.MakeDisposableTempFileEndingIn(fileEnding))
            {
                var gdbFile = disposableTempFile.FileInfo;
                httpPostedFileBase.SaveAs(gdbFile.FullName);
                HttpRequestStorage.DatabaseEntities.ProjectLocationStagingUpdates.DeleteProjectLocationStagingUpdate(projectUpdateBatch.ProjectLocationStagingUpdates);
                projectUpdateBatch.ProjectLocationStagingUpdates.Clear();
                ProjectLocationStagingUpdate.CreateProjectLocationStagingUpdateListFromGdb(gdbFile, projectUpdateBatch, CurrentPerson);
            }
            return ApproveGisUpload(project);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult ApproveGisUpload(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ProjectLocationDetailViewModel(project.ProjectLocations);
            return ViewApproveGisUpload(projectUpdateBatch, viewModel);
        }

        private PartialViewResult ViewApproveGisUpload(ProjectUpdateBatch projectUpdateBatch, ProjectLocationDetailViewModel viewModel)
        {
            var projectLocationStagingUpdates = projectUpdateBatch.ProjectLocationStagingUpdates.ToList();
            var layerGeoJsons =
                projectLocationStagingUpdates.Select(
                    (projectLocationStaging, i) =>
                        new LayerGeoJson(projectLocationStaging.FeatureClassName,
                            projectLocationStaging.ToGeoJsonFeatureCollection(),
                            FirmaHelpers.DefaultColorRange[i],
                            1,
                            LayerInitialVisibility.Show)).ToList();

            var boundingBox = BoundingBox.MakeBoundingBoxFromLayerGeoJsonList(layerGeoJsons);

            var mapInitJson = new MapInitJson($"project_{projectUpdateBatch.ProjectID}_PreviewMap", 10, layerGeoJsons, MapInitJson.GetExternalMapLayersForOtherMaps(), boundingBox, false) {AllowFullScreen = false, DisablePopups = true};
            var mapFormID = ProjectLocationController.GenerateEditProjectLocationFormID(projectUpdateBatch.ProjectID);
            var approveGisUploadUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.ApproveGisUpload(projectUpdateBatch.Project, null));

            var viewData = new ApproveGisUploadViewData(new List<IProjectLocationStaging>(projectLocationStagingUpdates), mapInitJson, mapFormID, approveGisUploadUrl);
            return RazorPartialView<ApproveGisUpload, ApproveGisUploadViewData, ProjectLocationDetailViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult ApproveGisUpload(ProjectPrimaryKey projectPrimaryKey, ProjectLocationDetailViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            if (!ModelState.IsValid)
            {
                return ViewApproveGisUpload(projectUpdateBatch, viewModel);
            }
            SaveProjectLocationUpdates(viewModel, projectUpdateBatch);
            var haveSqlGeometrys = new List<IHaveSqlGeometry>(projectUpdateBatch.ProjectLocationUpdates.ToList());
            DbSpatialHelper.Reduce(haveSqlGeometrys);
            return new ModalDialogFormJsonResult();
        }

        private static void SaveProjectLocationUpdates(ProjectLocationDetailViewModel viewModel, ProjectUpdateBatch projectUpdateBatch)
        {
            // MCS: This code is based on ProjectLocationController.SaveProjectDetailedLocations(); it cannot simply use a Merge because of the way the code branches based on ArcGisObjectID

            //update the editable ProjectLocations
            if (viewModel.ProjectLocationJsons != null)
            {
                var projectLocationUpdatesFromViewModel = new List<ProjectLocationUpdate>();
                foreach (var projectLocationJson in viewModel.ProjectLocationJsons.Where(x => !x.ArcGisObjectID.HasValue))
                {
                    var projectLocationGeometry = DbGeometry.FromText(projectLocationJson.ProjectLocationGeometryWellKnownText, FirmaWebConfiguration.GeoSpatialReferenceID);
                    var projectLocationUpdateInDB = projectUpdateBatch.ProjectLocationUpdates.SingleOrDefault(x => x.ProjectLocationUpdateID == projectLocationJson.ProjectLocationID);  // projectLocationJson.ProjectLocationID is actually a ProjectLocationUpdateID
                    if (projectLocationUpdateInDB == null)  // creating a new ProjectLocationUpdate
                    {
                        var projectLocationType = ProjectLocationType.All.FirstOrDefault(x => x.ProjectLocationTypeID == projectLocationJson.ProjectLocationTypeID);
                        var projectLocationUpdate = new ProjectLocationUpdate(
                            projectUpdateBatch,
                            projectLocationGeometry,
                            projectLocationJson.ProjectLocationNotes,
                            projectLocationType,
                            projectLocationJson.ProjectLocationName,
                            null,
                            null
                        );
                        projectLocationUpdatesFromViewModel.Add(projectLocationUpdate);
                    }
                    else  // updating existing ProjectLocationUpdate after applying updates from incoming ProjectLocationJson
                    {
                        projectLocationUpdateInDB.ProjectLocationGeometry = projectLocationGeometry;
                        projectLocationUpdateInDB.ProjectLocationTypeID = projectLocationJson.ProjectLocationTypeID;
                        projectLocationUpdateInDB.ProjectLocationUpdateName = projectLocationJson.ProjectLocationName;
                        projectLocationUpdateInDB.ProjectLocationNotes = projectLocationJson.ProjectLocationNotes;
                        projectLocationUpdatesFromViewModel.Add(projectLocationUpdateInDB);
                    }
                }

                HttpRequestStorage.DatabaseEntities.ProjectLocations.Load();
                var allProjectLocationUpdates = HttpRequestStorage.DatabaseEntities.ProjectLocationUpdates.Local;
                projectUpdateBatch.ProjectLocationUpdates
                    .Merge(projectLocationUpdatesFromViewModel, allProjectLocationUpdates, (x, y) => x.ProjectLocationUpdateID == y.ProjectLocationUpdateID);
            }

            // update ProjectLocationNotes for ones from ArcGIS
            foreach (var matched in viewModel.ArcGisProjectLocationJsons.Where(x => x.ArcGisObjectID.HasValue)
                .Join(projectUpdateBatch.ProjectLocationUpdates, plj => plj.ProjectLocationID, plu => plu.ProjectLocationUpdateID,
                    (lhs, rhs) => new { ProjectLocationJson = lhs, ProjectLocationUpdate = rhs }))
            {
                matched.ProjectLocationUpdate.ProjectLocationNotes = matched.ProjectLocationJson.ProjectLocationNotes;
            }
        }

        #region Region functions
        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult Regions(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }

            var regionIDs = projectUpdateBatch.ProjectRegionUpdates.Select(x => x.DNRUplandRegionID).ToList();
            var noRegionsExplanation = projectUpdateBatch.NoRegionsExplanation;
            var viewModel = new DNRUplandRegionsViewModel(regionIDs, noRegionsExplanation);
            return ViewRegions(project, projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Regions(ProjectPrimaryKey projectPrimaryKey, DNRUplandRegionsViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }

            if (!ModelState.IsValid)
            {
                return ViewRegions(project, projectUpdateBatch, viewModel);
            }
            var currentProjectUpdateRegions = projectUpdateBatch.ProjectRegionUpdates.ToList();
            var allProjectUpdateRegions = HttpRequestStorage.DatabaseEntities.ProjectRegionUpdates.Local;
            viewModel.UpdateModelBatch(projectUpdateBatch, currentProjectUpdateRegions, allProjectUpdateRegions);

            projectUpdateBatch.NoRegionsExplanation = viewModel.NoDNRUplandRegionsExplanation;
            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.NoRegionsExplanation = viewModel.NoDNRUplandRegionsExplanation;
            }
            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch, ProjectUpdateSection.DNRUplandRegions.ProjectUpdateSectionDisplayName);
        }

        private ViewResult ViewRegions(Project project, ProjectUpdateBatch projectUpdateBatch, DNRUplandRegionsViewModel viewModel)
        {
            var projectUpdate = projectUpdateBatch.ProjectUpdate;

            var boundingBox = ProjectLocationSummaryMapInitJson.GetProjectBoundingBox(projectUpdate);
            var layers = MapInitJson.GetRegionMapLayers(LayerInitialVisibility.Show);
            layers.AddRange(MapInitJson.GetProjectLocationSimpleAndDetailedMapLayers(projectUpdate));
            var mapInitJson = new MapInitJson("projectRegionMap", 0, layers, MapInitJson.GetExternalMapLayersForOtherMaps(), boundingBox) { AllowFullScreen = false, DisablePopups = true};
           
            var regionValidationResult = projectUpdateBatch.ValidateProjectRegion();
            var regions = projectUpdate.GetProjectRegions().ToList();
            var projectLocationSummaryMapInitJson = new ProjectLocationSummaryMapInitJson(projectUpdate, $"project_{project.ProjectID}_EditMap", false);
            var regionIDs = viewModel.DNRUplandRegionIDs ?? new List<int>();
            var regionsInViewModel = HttpRequestStorage.DatabaseEntities.DNRUplandRegions.Where(x => regionIDs.Contains(x.DNRUplandRegionID)).ToList();
            var editProjectRegionsPostUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(c => c.Regions(project, null));
            var editProjectRegionsFormId = GenerateEditProjectLocationFormID(project);

            var editProjectLocationViewData = new EditProjectRegionsViewData(CurrentPerson, mapInitJson, regionsInViewModel, editProjectRegionsPostUrl, editProjectRegionsFormId, projectUpdate.HasProjectLocationPoint, projectUpdate.HasProjectLocationDetail);
            var noRegionsExplanation = projectUpdateBatch.NoRegionsExplanation;

            var projectLocationSummaryViewData = new ProjectLocationSummaryViewData(projectUpdate, projectLocationSummaryMapInitJson, new List<PriorityLandscape>(), regions, noRegionsExplanation, string.Empty, new List<County>(), string.Empty);
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var viewData = new DNRUplandRegionsViewData(CurrentPerson, projectUpdate, editProjectLocationViewData, projectLocationSummaryViewData, regionValidationResult, updateStatus);
            return RazorView<Views.ProjectUpdate.DNRUplandRegions, DNRUplandRegionsViewData, DNRUplandRegionsViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshProjectRegion(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshProjectRegion(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshProjectRegion(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            if (projectUpdate == null)
            {
                return new ModalDialogFormJsonResult();
            }
            projectUpdateBatch.DeleteProjectRegionUpdates();

            // refresh the data
            ProjectRegionUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.NoRegionsExplanation = project.NoRegionsExplanation;
            //ProjectGeospatialAreaTypeNoteUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshProjectRegion(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the {FieldDefinition.DNRUplandRegion.FieldDefinitionDisplayName} data? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }
        #endregion


        #region County functions
        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult Counties(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }

            var countiesIDs = projectUpdateBatch.ProjectCountyUpdates.Select(x => x.CountyID).ToList();
            var noCountiesExplanation = projectUpdateBatch.NoCountiesExplanation;
            var viewModel = new CountiesViewModel(countiesIDs, noCountiesExplanation);
            return ViewCounties(project, projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Counties(ProjectPrimaryKey projectPrimaryKey, CountiesViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }

            if (!ModelState.IsValid)
            {
                return ViewCounties(project, projectUpdateBatch, viewModel);
            }
            var currentProjectUpdateCounties = projectUpdateBatch.ProjectCountyUpdates.ToList();
            var allProjectUpdateCounties = HttpRequestStorage.DatabaseEntities.ProjectCountyUpdates.Local;
            viewModel.UpdateModelBatch(projectUpdateBatch, currentProjectUpdateCounties, allProjectUpdateCounties);

            projectUpdateBatch.NoCountiesExplanation = viewModel.NoCountiesExplanation;
            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.NoCountiesExplanation = viewModel.NoCountiesExplanation;
            }
            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch, ProjectUpdateSection.Counties.ProjectUpdateSectionDisplayName);
        }

        private ViewResult ViewCounties(Project project, ProjectUpdateBatch projectUpdateBatch, CountiesViewModel viewModel)
        {
            var projectUpdate = projectUpdateBatch.ProjectUpdate;

            var boundingBox = ProjectLocationSummaryMapInitJson.GetProjectBoundingBox(projectUpdate);
            var layers = MapInitJson.GetCountyMapLayers(LayerInitialVisibility.Show);
            layers.AddRange(MapInitJson.GetProjectLocationSimpleAndDetailedMapLayers(projectUpdate));
            var mapInitJson = new MapInitJson("projectCountyMap", 0, layers, MapInitJson.GetExternalMapLayersForOtherMaps(), boundingBox) { AllowFullScreen = false, DisablePopups = true };

            var countyValidationResult = projectUpdateBatch.ValidateProjectCounty();
            var counties = HttpRequestStorage.DatabaseEntities.Counties.ToList();
            var projectLocationSummaryMapInitJson = new ProjectLocationSummaryMapInitJson(projectUpdate, $"project_{project.ProjectID}_EditMap", false);
            var countyIDs = viewModel.CountyIDs ?? new List<int>();
            var countiesInViewModel = HttpRequestStorage.DatabaseEntities.Counties.Where(x => countyIDs.Contains(x.CountyID)).ToList();
            var editProjectCountiesPostUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(c => c.Counties(project, null));
            var editProjectCountiesFormId = GenerateEditProjectLocationFormID(project);

            var editProjectLocationViewData = new EditProjectCountiesViewData(CurrentPerson, mapInitJson, countiesInViewModel, editProjectCountiesPostUrl, editProjectCountiesFormId, projectUpdate.HasProjectLocationPoint, projectUpdate.HasProjectLocationDetail);
            var noCountiesExplanation = projectUpdateBatch.NoCountiesExplanation;

            var projectLocationSummaryViewData = new ProjectLocationSummaryViewData(projectUpdate, projectLocationSummaryMapInitJson, new List<PriorityLandscape>(), new List<DNRUplandRegion>(), string.Empty, string.Empty, counties, noCountiesExplanation);
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var viewData = new CountiesViewData(CurrentPerson, projectUpdate, editProjectLocationViewData, projectLocationSummaryViewData, countyValidationResult, updateStatus);
            return RazorView<Counties, CountiesViewData, CountiesViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshProjectCounty(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshProjectCounty(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshProjectCounty(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            if (projectUpdate == null)
            {
                return new ModalDialogFormJsonResult();
            }
            projectUpdateBatch.DeleteProjectCountyUpdates();

            // refresh the data
            ProjectCountyUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.NoCountiesExplanation = project.NoCountiesExplanation;
            //ProjectGeospatialAreaTypeNoteUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshProjectCounty(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the {FieldDefinition.County.FieldDefinitionDisplayName} data? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }
        #endregion







        #region PriorityLandscape functions
        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult PriorityLandscapes(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }

            var priorityLandscapeIDs = projectUpdateBatch.ProjectPriorityLandscapeUpdates.Select(x => x.PriorityLandscapeID).ToList();
            var noPriorityLandscapesExplanation = projectUpdateBatch.NoPriorityLandscapesExplanation;
            var viewModel = new PriorityLandscapesViewModel(priorityLandscapeIDs, noPriorityLandscapesExplanation);
            return ViewPriorityLandscapes(project, projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult PriorityLandscapes(ProjectPrimaryKey projectPrimaryKey, PriorityLandscapesViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }

            if (!ModelState.IsValid)
            {
                return ViewPriorityLandscapes(project, projectUpdateBatch, viewModel);
            }
            var currentProjectUpdatePriorityLandscapes = projectUpdateBatch.ProjectPriorityLandscapeUpdates.ToList();
            var allProjectUpdatePriorityLandscapes = HttpRequestStorage.DatabaseEntities.ProjectPriorityLandscapeUpdates.Local;
            viewModel.UpdateModelBatch(projectUpdateBatch, currentProjectUpdatePriorityLandscapes, allProjectUpdatePriorityLandscapes);

            projectUpdateBatch.NoPriorityLandscapesExplanation = viewModel.NoPriorityLandscapesExplanation;
            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.NoPriorityLandscapesExplanation = viewModel.NoPriorityLandscapesExplanation;
            }
            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch, ProjectUpdateSection.PriorityLandscapes.ProjectUpdateSectionDisplayName);
        }

        private ViewResult ViewPriorityLandscapes(Project project, ProjectUpdateBatch projectUpdateBatch, PriorityLandscapesViewModel viewModel)
        {
            var projectUpdate = projectUpdateBatch.ProjectUpdate;

            var boundingBox = ProjectLocationSummaryMapInitJson.GetProjectBoundingBox(projectUpdate);
            var layers = MapInitJson.GetPriorityLandscapeMapLayers(LayerInitialVisibility.Show);
            layers.AddRange(MapInitJson.GetProjectLocationSimpleAndDetailedMapLayers(projectUpdate));
            var mapInitJson = new MapInitJson("projectPriorityLandscapeMap", 0, layers, MapInitJson.GetExternalMapLayersForOtherMaps(), boundingBox) { AllowFullScreen = false, DisablePopups = true};
           
            var priorityLandscapeValidationResult = projectUpdateBatch.ValidateProjectPriorityLandscape();
            var priorityLandscapes = projectUpdate.GetProjectPriorityLandscapes().ToList();
            var projectLocationSummaryMapInitJson = new ProjectLocationSummaryMapInitJson(projectUpdate, $"project_{project.ProjectID}_EditMap", false);
            var priorityLandscapeIDs = viewModel.PriorityLandscapeIDs ?? new List<int>();
            var priorityLandscapesInViewModel = HttpRequestStorage.DatabaseEntities.PriorityLandscapes.Where(x => priorityLandscapeIDs.Contains(x.PriorityLandscapeID)).ToList();
            var editProjectPriorityLandscapesPostUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(c => c.PriorityLandscapes(project, null));
            var editProjectPriorityLandscapesFormId = GenerateEditProjectLocationFormID(project);

            var editProjectLocationViewData = new EditProjectPriorityLandscapesViewData(CurrentPerson, mapInitJson, priorityLandscapesInViewModel, editProjectPriorityLandscapesPostUrl, editProjectPriorityLandscapesFormId, projectUpdate.HasProjectLocationPoint, projectUpdate.HasProjectLocationDetail);
            var noPriorityLandscapesExplanation = projectUpdateBatch.NoPriorityLandscapesExplanation;

            var projectLocationSummaryViewData = new ProjectLocationSummaryViewData(projectUpdate, projectLocationSummaryMapInitJson, priorityLandscapes, new List<DNRUplandRegion>(), string.Empty, noPriorityLandscapesExplanation, new List<County>(), string.Empty);
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var viewData = new PriorityLandscapesViewData(CurrentPerson, projectUpdate, editProjectLocationViewData, projectLocationSummaryViewData, priorityLandscapeValidationResult, updateStatus);
            return RazorView<Views.ProjectUpdate.PriorityLandscapes, PriorityLandscapesViewData, PriorityLandscapesViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshProjectPriorityLandscape(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshProjectPriorityLandscape(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshProjectPriorityLandscape(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            if (projectUpdate == null)
            {
                return new ModalDialogFormJsonResult();
            }
            projectUpdateBatch.DeleteProjectPriorityLandscapeUpdates();

            // refresh the data
            ProjectPriorityLandscapeUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.NoPriorityLandscapesExplanation = project.NoPriorityLandscapesExplanation;
            //ProjectGeospatialAreaTypeNoteUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshProjectPriorityLandscape(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the {FieldDefinition.PriorityLandscape.GetFieldDefinitionLabel()} data? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }
        #endregion


        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult DocumentsAndNotes(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var diffUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.DiffNotesAndDocuments(projectPrimaryKey));
            var viewData = new DocumentsAndNotesViewData(CurrentPerson, projectUpdateBatch, updateStatus, diffUrl);
            return RazorView<DocumentsAndNotes, DocumentsAndNotesViewData>(viewData);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshNotesAndDocuments(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshNotesAndDocuments(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshNotesAndDocuments(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            projectUpdateBatch.DeleteProjectNoteUpdates();
            projectUpdateBatch.DeleteProjectDocumentUpdates();
            // finally create a new project update record, refreshing with the current project data at this point in time
            ProjectNoteUpdate.CreateFromProject(projectUpdateBatch);
            ProjectDocumentUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshNotesAndDocuments(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the notes for this {FieldDefinition.Project.GetFieldDefinitionLabel()}? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult ExternalLinks(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var viewModel =
                new EditProjectExternalLinksViewModel(
                    projectUpdateBatch.ProjectExternalLinkUpdates.Select(
                        x => new ProjectExternalLinkSimple(x.ProjectExternalLinkUpdateID, x.ProjectUpdateBatchID, x.ExternalLinkLabel, x.ExternalLinkUrl)).ToList());
            return ViewExternalLinks(projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult ExternalLinks(ProjectPrimaryKey projectPrimaryKey, EditProjectExternalLinksViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            if (!ModelState.IsValid)
            {
                return ViewExternalLinks(projectUpdateBatch, viewModel);
            }
            var currentProjectExternalLinkUpdates = projectUpdateBatch.ProjectExternalLinkUpdates.ToList();
            HttpRequestStorage.DatabaseEntities.ProjectExternalLinkUpdates.Load();
            var allProjectExternalLinkUpdates = HttpRequestStorage.DatabaseEntities.ProjectExternalLinkUpdates.Local;
            viewModel.UpdateModel(currentProjectExternalLinkUpdates, allProjectExternalLinkUpdates);

            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch,
                ProjectUpdateSection.ExternalLinks.ProjectUpdateSectionDisplayName);
        }

        private ViewResult ViewExternalLinks(ProjectUpdateBatch projectUpdateBatch, EditProjectExternalLinksViewModel viewModel)
        {
            var refreshUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.RefreshExternalLinks(projectUpdateBatch.Project));
            var diffUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.DiffExternalLinks(projectUpdateBatch.Project));
            var entityExternalLinksViewData = new EntityExternalLinksViewData(ExternalLink.CreateFromEntityExternalLink(new List<IEntityExternalLink>(projectUpdateBatch.ProjectExternalLinkUpdates)));
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var viewDataForAngularClass = new ExternalLinksViewData.ViewDataForAngularClass(projectUpdateBatch.ProjectUpdateBatchID);
            var viewData = new ExternalLinksViewData(CurrentPerson,
                projectUpdateBatch,
                updateStatus,
                viewDataForAngularClass,
                entityExternalLinksViewData,
                refreshUrl,
                diffUrl);
            return RazorView<ExternalLinks, ExternalLinksViewData, EditProjectExternalLinksViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshExternalLinks(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshExternalLinks(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshExternalLinks(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            projectUpdateBatch.DeleteProjectExternalLinkUpdates();
            // finally create a new project update record, refreshing with the current project data at this point in time
            ProjectExternalLinkUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshExternalLinks(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the notes for this {FieldDefinition.Project.GetFieldDefinitionLabel()}? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateAdminFeatureWithProjectContext]
        public PartialViewResult Approve(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update to approve for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}!");
            Check.Require(projectUpdateBatch.IsSubmitted, $"The {FieldDefinition.Project.GetFieldDefinitionLabel()} is not in a state to be ready to be approved!");
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewApprove(projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateAdminFeatureWithProjectContext]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Approve(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update to approve for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}!");
            Check.Require(projectUpdateBatch.IsSubmitted, $"The {FieldDefinition.Project.GetFieldDefinitionLabel()} is not in a state to be ready to be approved!");
            WriteHtmlDiffLogs(projectPrimaryKey, projectUpdateBatch);

            HttpRequestStorage.DatabaseEntities.ProjectFundSourceAllocationRequests.Load();
            var allprojectFundSourceAllocationRequests = HttpRequestStorage.DatabaseEntities.ProjectFundSourceAllocationRequests.Local;
            HttpRequestStorage.DatabaseEntities.ProjectFundingSources.Load();
            var allprojectFundingSources = HttpRequestStorage.DatabaseEntities.ProjectFundingSources.Local;
            // TODO: Neutered per #1136; most likely will bring back when BOR project starts
            //HttpRequestStorage.DatabaseEntities.ProjectBudgets.Load();
            //var allProjectBudgets = HttpRequestStorage.DatabaseEntities.ProjectBudgets.Local;
            HttpRequestStorage.DatabaseEntities.ProjectExternalLinks.Load();
            var allProjectExternalLinks = HttpRequestStorage.DatabaseEntities.ProjectExternalLinks.Local;
            HttpRequestStorage.DatabaseEntities.ProjectNotes.Load();
            var allProjectNotes = HttpRequestStorage.DatabaseEntities.ProjectNotes.Local;
            HttpRequestStorage.DatabaseEntities.ProjectImages.Load();
            var allProjectImages = HttpRequestStorage.DatabaseEntities.ProjectImages.Local;
            HttpRequestStorage.DatabaseEntities.ProjectLocations.Load();
            var allProjectLocations = HttpRequestStorage.DatabaseEntities.ProjectLocations.Local;
            HttpRequestStorage.DatabaseEntities.ProjectPriorityLandscapes.Load();
            var allProjectPriorityLandscapes = HttpRequestStorage.DatabaseEntities.ProjectPriorityLandscapes.Local;
            HttpRequestStorage.DatabaseEntities.ProjectRegions.Load();
            var allProjectRegions = HttpRequestStorage.DatabaseEntities.ProjectRegions.Local;
            HttpRequestStorage.DatabaseEntities.ProjectCounties.Load();
            var allProjectCounties = HttpRequestStorage.DatabaseEntities.ProjectCounties.Local;
            HttpRequestStorage.DatabaseEntities.ProjectOrganizations.Load();
            var allProjectOrganizations = HttpRequestStorage.DatabaseEntities.ProjectOrganizations.Local;
            HttpRequestStorage.DatabaseEntities.ProjectPeople.Load();
            var allProjectPeople = HttpRequestStorage.DatabaseEntities.ProjectPeople.Local;
            HttpRequestStorage.DatabaseEntities.ProjectDocuments.Load();
            var allProjectDocuments = HttpRequestStorage.DatabaseEntities.ProjectDocuments.Local;
            HttpRequestStorage.DatabaseEntities.ProjectPrograms.Load();
            var allProjectPrograms = HttpRequestStorage.DatabaseEntities.ProjectPrograms.Local;
            HttpRequestStorage.DatabaseEntities.Treatments.Load();
            var allTreatments = HttpRequestStorage.DatabaseEntities.Treatments.Local;

            projectUpdateBatch.Approve(CurrentPerson,
                DateTime.Now,
                allprojectFundingSources,
                // TODO: Neutered per #1136; most likely will bring back when BOR project starts
                //                allProjectBudgets,
                allProjectExternalLinks,
                allProjectNotes,
                allProjectImages,
                allProjectLocations,
                allProjectPriorityLandscapes,
                allProjectRegions,
                allProjectCounties,
                allprojectFundSourceAllocationRequests,
                allProjectOrganizations,
                allProjectDocuments,
                allProjectPeople,
                allProjectPrograms,
                allTreatments);

            HttpRequestStorage.DatabaseEntities.SaveChanges();

            var peopleToCc = HttpRequestStorage.DatabaseEntities.People.GetPeopleWhoReceiveNotifications().Union(project.GetProjectStewards()).Distinct().OrderBy(ht => ht.FullNameLastFirst).ToList();

            NotificationProject.SendApprovalMessage(peopleToCc, projectUpdateBatch);

            SetMessageForDisplay($"The update for {FieldDefinition.Project.GetFieldDefinitionLabel()} {projectUpdateBatch.Project.DisplayName} was approved!");
            return new ModalDialogFormJsonResult(SitkaRoute<ProjectController>.BuildUrlFromExpression(x => x.Detail(project)));
        }

        private PartialViewResult ViewApprove(ProjectUpdateBatch projectUpdate, ConfirmDialogFormViewModel viewModel)
        {
            var viewData = new ConfirmDialogFormViewData($"Are you sure you want to approve the updates to {FieldDefinition.Project.GetFieldDefinitionLabel()} {projectUpdate.Project.DisplayName}?");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        private void WriteHtmlDiffLogs(ProjectPrimaryKey projectPrimaryKey, ProjectUpdateBatch projectUpdateBatch)
        {
            var basicsDiffContainer = DiffBasicsImpl(projectPrimaryKey);
            if (basicsDiffContainer.HasChanged)
            {
                var basicsDiffHelper = new HtmlDiff.HtmlDiff(basicsDiffContainer.OriginalHtml, basicsDiffContainer.UpdatedHtml);                
                projectUpdateBatch.BasicsDiffLogHtmlString = new HtmlString(basicsDiffHelper.Build());
            }            

            // TODO: Neutered per #1136; most likely will bring back when BOR project starts
            //var budgetsDiffContainer = DiffBudgetsImpl(projectPrimaryKey);
            //if (budgetsDiffContainer.HasChanged)
            //{
            //    var budgetsDiffHelper = new HtmlDiff.HtmlDiff(budgetsDiffContainer.OriginalHtml, budgetsDiffContainer.UpdatedHtml);
            //    projectUpdateBatch.BudgetsDiffLogHtmlString = new HtmlString(budgetsDiffHelper.Build());
            //}

            var externalLinksDiffContainer = DiffExternalLinksImpl(projectPrimaryKey);
            if (externalLinksDiffContainer.HasChanged)
            {
                var externalLinksDiffHelper = new HtmlDiff.HtmlDiff(externalLinksDiffContainer.OriginalHtml, externalLinksDiffContainer.UpdatedHtml);
                projectUpdateBatch.ExternalLinksDiffLogHtmlString = new HtmlString(externalLinksDiffHelper.Build());
            }

            var notesDiffContainer = DiffNotesAndDocumentsImpl(projectPrimaryKey);
            if (notesDiffContainer.HasChanged)
            {
                var notesDiffHelper = new HtmlDiff.HtmlDiff(notesDiffContainer.OriginalHtml, notesDiffContainer.UpdatedHtml);
                projectUpdateBatch.NotesDiffLogHtmlString = new HtmlString(notesDiffHelper.Build());
            }
        }


        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult Submit(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update to submit for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewSubmit(projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Submit(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update to submit for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            projectUpdateBatch.SubmitToReviewer(CurrentPerson, DateTime.Now);
            var peopleToCc = HttpRequestStorage.DatabaseEntities.People.GetPeopleWhoReceiveNotifications().Union(project.GetProjectStewards()).Distinct().OrderBy(ht => ht.FullNameLastFirst).ToList();
            NotificationProject.SendSubmittedMessage(peopleToCc, projectUpdateBatch);
            SetMessageForDisplay($"The update for {FieldDefinition.Project.GetFieldDefinitionLabel()} '{projectUpdateBatch.Project.DisplayName}' was submitted.");
            return new ModalDialogFormJsonResult(project.GetDetailUrl());
        }

        private PartialViewResult ViewSubmit(ProjectUpdateBatch projectUpdate, ConfirmDialogFormViewModel viewModel)
        {
            //TODO: Change "for review" to specific reviewer as determined by tenant review 
            var viewData = new ConfirmDialogFormViewData($"Are you sure you want to submit {FieldDefinition.Project.GetFieldDefinitionLabel()} {projectUpdate.Project.DisplayName} for review?");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [LoggedInAndNotUnassignedRoleUnclassifiedFeature]
        public PartialViewResult SubmitAll()
        {
            var viewModel = new ConfirmDialogFormViewModel(CurrentPerson.PersonID);
            return ViewSubmitAll(viewModel);
        }

        [HttpPost]
        [LoggedInAndNotUnassignedRoleUnclassifiedFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult SubmitAll(ConfirmDialogFormViewModel viewModel)
        {
            var projectUpdateBatches =
                HttpRequestStorage.DatabaseEntities.ProjectUpdateBatches.ToList()
                    .Where(pub => pub.IsReadyToSubmit && pub.Project.ProjectStage.RequiresReportedExpenditures() && pub.Project.IsMyProject(CurrentPerson))
                    .ToList();
            var peopleToCc = HttpRequestStorage.DatabaseEntities.People.GetPeopleWhoReceiveNotifications();
            projectUpdateBatches.ForEach(pub =>
            {
                pub.SubmitToReviewer(CurrentPerson, DateTime.Now);
                var peopleToNotify = peopleToCc.Union(pub.Project.GetProjectStewards()).Distinct().OrderBy(ht => ht.FullNameLastFirst).ToList();
                NotificationProject.SendSubmittedMessage(peopleToNotify, pub);
            });
            SetMessageForDisplay($"The update(s) for {FieldDefinition.Project.GetFieldDefinitionLabelPluralized()} {string.Join(", ", projectUpdateBatches.Select(x => x.Project.DisplayName))} have been submitted.");
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewSubmitAll(ConfirmDialogFormViewModel viewModel)
        {
            //TODO: Change "for review" to specific reviewer as determined by tentant review 
            var viewData = new ConfirmDialogFormViewData($"Are you sure you want to submit all your {FieldDefinition.Project.GetFieldDefinitionLabelPluralized()} that are ready to be submitted for review?");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateAdminFeatureWithProjectContext]
        public PartialViewResult Return(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update to return for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            Check.Require(projectUpdateBatch.IsSubmitted, $"You cannot return a {FieldDefinition.Project.GetFieldDefinitionLabel()} Update that has not been submitted!");
            var viewModel = new ReturnDialogFormViewModel();
            return ViewReturn(projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateAdminFeatureWithProjectContext]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Return(ProjectPrimaryKey projectPrimaryKey, ReturnDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update to return for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            Check.Require(projectUpdateBatch.IsSubmitted, $"You cannot return a {FieldDefinition.Project.GetFieldDefinitionLabel()} Update that has not been submitted!");
            viewModel.UpdateModel(projectUpdateBatch);
            projectUpdateBatch.Return(CurrentPerson, DateTime.Now);
            var peopleToCc = HttpRequestStorage.DatabaseEntities.People.GetPeopleWhoReceiveNotifications().Union(project.GetProjectStewards()).Distinct().OrderBy(ht => ht.FullNameLastFirst).ToList();
            NotificationProject.SendReturnedMessage(peopleToCc, projectUpdateBatch);
            SetMessageForDisplay($"The update submitted for {FieldDefinition.Project.GetFieldDefinitionLabel()} {projectUpdateBatch.Project.DisplayName} has been returned.");
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewReturn(ProjectUpdateBatch projectUpdate, ReturnDialogFormViewModel viewModel)
        {
            var viewData = new ReturnDialogFormViewData($"Are you sure you want to return {FieldDefinition.Project.GetFieldDefinitionLabel()} {projectUpdate.Project.DisplayName}?");
            return RazorPartialView<ReturnDialogForm, ReturnDialogFormViewData, ReturnDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult DeleteProjectUpdate(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update to delete for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewDeleteProjectUpdate(viewModel, projectUpdateBatch);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult DeleteProjectUpdate(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update to delete for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            projectUpdateBatch.DeleteFull(HttpRequestStorage.DatabaseEntities);
            SetMessageForDisplay($"{FieldDefinition.Project.GetFieldDefinitionLabel()} Update successfully deleted.");
            return new ModalDialogFormJsonResult(SitkaRoute<ProjectController>.BuildUrlFromExpression(x => x.Detail(project)));
        }

        private PartialViewResult ViewDeleteProjectUpdate(ConfirmDialogFormViewModel viewModel, ProjectUpdateBatch projectUpdateBatch)
        {
            var viewData = new ConfirmDialogFormViewData($"Are you sure you want to delete this update for {FieldDefinition.Project.GetFieldDefinitionLabel()} {projectUpdateBatch.Project.DisplayName}?");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult History(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var viewData = new HistoryViewData(projectUpdateBatch);
            return RazorPartialView<History, HistoryViewData>(viewData);
        }

        private static string GenerateEditProjectLocationFormID(Project project)
        {
            return $"editMapForProposal{project.ProjectID}";
        }

        [ProjectUpdateAdminFeature]
        public ViewResult Manage()
        {
            var customNotificationUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.CreateCustomNotification(null));
            var projectsRequiringUpdateGridSpec = new ProjectUpdateStatusGridSpec(ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.AllProjects, CurrentPerson.IsApprover())
            {
                ObjectNameSingular = $"{FieldDefinition.Project.GetFieldDefinitionLabel()}",
                ObjectNamePlural = $"{FieldDefinition.Project.GetFieldDefinitionLabelPluralized()}",
                SaveFiltersInCookie = true
            };
            var contactsReceivingReminderGridSpec = new PeopleReceivingReminderGridSpec(true, CurrentPerson) {ObjectNameSingular = "Person", ObjectNamePlural = "People", SaveFiltersInCookie = true};
            var firmaPage = FirmaPage.GetFirmaPageByPageType(FirmaPageType.ManageUpdateNotifications);

            var projectsWithNoContactCount = GetProjectsWithNoContact(CurrentPerson).Count;

            var viewData = new ManageViewData(CurrentPerson,
                firmaPage,
                customNotificationUrl,
                projectsRequiringUpdateGridSpec,
                SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.ProjectsRequiringUpdateGridJsonData()),
                contactsReceivingReminderGridSpec,
                SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.PeopleReceivingReminderGridJsonData(true)),
                projectsWithNoContactCount,
                MultiTenantHelpers.GetProjectUpdateConfiguration());
            return RazorView<Manage, ManageViewData>(viewData);
        }

        private static List<Project> GetProjectsWithNoContact(Person currentPerson)
        {
            var projectsRequiringUpdate = HttpRequestStorage.DatabaseEntities.Projects.ToList().GetActiveProjectsVisibleToUser(currentPerson).Where(x => x.IsUpdatableViaProjectUpdateProcess).ToList();
            return projectsRequiringUpdate.Where(x => x.GetPrimaryContact() == null).ToList();
        }

        [ProjectUpdateAdminFeature]
        public GridJsonNetJObjectResult<Project> ProjectsRequiringUpdateGridJsonData()
        {
            var gridSpec = new ProjectUpdateStatusGridSpec(ProjectUpdateStatusGridSpec.ProjectUpdateStatusFilterTypeEnum.AllProjects, CurrentPerson.IsApprover());
            var projects =
                HttpRequestStorage.DatabaseEntities.Projects.ToList().GetActiveProjectsVisibleToUser(CurrentPerson).Where(x => x.IsUpdatableViaProjectUpdateProcess && x.IsEditableToThisPerson(CurrentPerson)).ToList();
            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<Project>(projects, gridSpec);
            return gridJsonNetJObjectResult;
        }

        [LoggedInAndNotUnassignedRoleUnclassifiedFeature]
        public GridJsonNetJObjectResult<Person> PeopleReceivingReminderGridJsonData(bool showCheckbox)
        {
            var gridSpec = new PeopleReceivingReminderGridSpec(showCheckbox, CurrentPerson);
            var projects = HttpRequestStorage.DatabaseEntities.Projects.ToList().GetActiveProjectsVisibleToUser(CurrentPerson).Where(x => x.IsUpdatableViaProjectUpdateProcess && x.IsEditableToThisPerson(CurrentPerson)).ToList();
            var people = projects.GetPrimaryContactPeople();
            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<Person>(people, gridSpec);
            return gridJsonNetJObjectResult;
        }

        /// <summary>
        /// Dummy get signature so that it can find the post action
        /// </summary>
        [HttpGet]
        [ProjectUpdateAdminFeature]
        public ContentResult CreateCustomNotification()
        {
            return new ContentResult();
        }

        [HttpPost]
        [ProjectUpdateAdminFeature]
        public PartialViewResult CreateCustomNotification(CustomNotificationViewModel viewModel)
        {
            var peopleToNotify = HttpRequestStorage.DatabaseEntities.People.Where(x => viewModel.PersonIDList.Contains(x.PersonID)).ToList();
            var sendPreviewEmailUrl = SitkaRoute<ProjectUpdateController>.BuildUrlFromExpression(x => x.SendPreviewOfCustomNotification(null));
            var viewData = new CustomNotificationViewData(CurrentPerson, peopleToNotify, sendPreviewEmailUrl);
            return RazorPartialView<CustomNotification, CustomNotificationViewData, CustomNotificationViewModel>(viewData, viewModel);
        }

        /// <summary>
        /// Dummy get signature so that it can find the post action
        /// </summary>
        [HttpGet]
        [ProjectUpdateAdminFeature]
        public ContentResult SendCustomNotification()
        {
            return new ContentResult();
        }

        [HttpPost]
        [ProjectUpdateAdminFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult SendCustomNotification(CustomNotificationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return CreateCustomNotification(viewModel);
            }

            var peopleToNotify = HttpRequestStorage.DatabaseEntities.People.Where(x => viewModel.PersonIDList.Contains(x.PersonID)).ToList();
            var emailsToSendTo = peopleToNotify.Select(x => x.Email).ToList();
            var emailsToReplyTo = new List<string> { FirmaWebConfiguration.DoNotReplyEmail };

            var message = new MailMessage {Subject = viewModel.Subject, AlternateViews = {AlternateView.CreateAlternateViewFromString(viewModel.NotificationContent ?? string.Empty, null, "text/html")}};

            Notification.SendMessageAndLogNotification(message, emailsToSendTo, emailsToReplyTo, new List<string>(), peopleToNotify, DateTime.Now, new List<Project>(), NotificationType.Custom);

            SetMessageForDisplay($"Custom notification sent to: {string.Join("; ", emailsToSendTo)}");

            return new ModalDialogFormJsonResult();
        }

        /// <summary>
        /// Dummy get signature so that it can find the post action
        /// </summary>
        [HttpGet]
        [ProjectUpdateAdminFeature]
        public ContentResult SendPreviewOfCustomNotification()
        {
            return new ContentResult();
        }

        [HttpPost]
        [ProjectUpdateAdminFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult SendPreviewOfCustomNotification(CustomNotificationViewModel viewModel)
        {
            var emailsToSendTo = new List<string> {CurrentPerson.Email};
            var message = new MailMessage { Subject = viewModel.Subject, AlternateViews = { AlternateView.CreateAlternateViewFromString(viewModel.NotificationContent, null, "text/html") } };
            Notification.SendMessage(message, emailsToSendTo, new List<string>(), new List<string>());
            return CreateCustomNotification(viewModel);
        }
        
        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult DiffBasics(ProjectPrimaryKey projectPrimaryKey)
        {
            var htmlDiffContainer = DiffBasicsImpl(projectPrimaryKey);
            var htmlDiff = new HtmlDiff.HtmlDiff(htmlDiffContainer.OriginalHtml, htmlDiffContainer.UpdatedHtml);
            return ViewHtmlDiff(htmlDiff.Build(), string.Empty);
        }

        private HtmlDiffContainer DiffBasicsImpl(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            var projectUpdate = projectUpdateBatch.ProjectUpdate;
            var originalHtml = GeneratePartialViewForProjectBasics(project);
            var updatedHtml = GeneratePartialViewForProjectBasics(projectUpdateBatch);

            return new HtmlDiffContainer(originalHtml, updatedHtml);
        }

        private string GeneratePartialViewForProjectBasics(Project project)
        {
            var viewData = new ProjectBasicsViewData(project);
            var partialViewAsString = RenderPartialViewToString(ProjectBasicsPartialViewPath, viewData);
            return partialViewAsString;
        }
        
        private string GeneratePartialViewForProjectBasics(ProjectUpdateBatch projectUpdateBatch)
        {
            var viewData = new ProjectBasicsViewData(projectUpdateBatch);
            var partialViewAsString = RenderPartialViewToString(ProjectBasicsPartialViewPath, viewData);
            return partialViewAsString;
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult DiffExpectedFunding(ProjectPrimaryKey projectPrimaryKey)
        {
            var htmlDiffContainer = DiffExpectedFundingImpl(projectPrimaryKey);
            var htmlDiff = new HtmlDiff.HtmlDiff(htmlDiffContainer.OriginalHtml, htmlDiffContainer.UpdatedHtml);
            return ViewHtmlDiff(htmlDiff.Build(), string.Empty);
        }

        private HtmlDiffContainer DiffExpectedFundingImpl(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            var estimatedTotalCostOriginal = project.EstimatedTotalCost;
            var projectFundingSourceNotesOriginal = project.ProjectFundingSourceNotes;
            var fundingSourcesOriginal = project.ProjectFundingSources.Select(x => x.FundingSource).OrderBy(y => y.FundingSourceID).ToList();
            
            var estimatedTotalCostModified = projectUpdateBatch.ProjectUpdate.EstimatedTotalCost;
            var projectFundingSourceNotesModified = projectUpdateBatch.ProjectUpdate.ProjectFundingSourceNotes;
            var fundingSourcesModified = projectUpdateBatch.ProjectFundingSourceUpdates.Select(x => x.FundingSource).OrderBy(y => y.FundingSourceID).ToList();

            var fundSourceAllocationRequestsOriginal = new List<IFundSourceAllocationRequestAmount>(project.ProjectFundSourceAllocationRequests.ToList());
            var projectFundSourceAllocationRequestsUpdated = new List<IFundSourceAllocationRequestAmount>(projectUpdateBatch.ProjectFundSourceAllocationRequestUpdates.ToList());
            
            var originalHtml = GeneratePartialViewForOriginalFundingRequests(fundSourceAllocationRequestsOriginal, projectFundSourceAllocationRequestsUpdated, fundingSourcesOriginal, estimatedTotalCostOriginal, projectFundingSourceNotesOriginal, project.IsInLandownerAssistanceProgram);
            var updatedHtml = GeneratePartialViewForModifiedFundingRequests(fundSourceAllocationRequestsOriginal, projectFundSourceAllocationRequestsUpdated, fundingSourcesModified, estimatedTotalCostModified, projectFundingSourceNotesModified, projectUpdateBatch.Project.IsInLandownerAssistanceProgram);
            return new HtmlDiffContainer(originalHtml, updatedHtml);
        }

        private string GeneratePartialViewForOriginalFundingRequests(List<IFundSourceAllocationRequestAmount> projectFundSourceAllocationRequestsOriginal,
            List<IFundSourceAllocationRequestAmount> projectFundSourceAllocationRequestsUpdated, List<FundingSource> fundingSources, decimal? estimatedTotalCost, string projectFundingSourceNotes, bool isProjectAnLoaProject)
        {
            var fundSourceAllocationsInOriginal = projectFundSourceAllocationRequestsOriginal.Select(x => x.FundSourceAllocation.FundSourceAllocationID).ToList();
            var fundSourceAllocationsInUpdated = projectFundSourceAllocationRequestsUpdated.Select(x => x.FundSourceAllocation.FundSourceAllocationID).ToList();
            var fundSourceAllocationsOnlyInOriginal = fundSourceAllocationsInOriginal.Where(x => !fundSourceAllocationsInUpdated.Contains(x)).ToList();
            var fundSourceAllocationRequestAmounts = projectFundSourceAllocationRequestsOriginal.Select(x => new FundSourceAllocationRequestAmount(x)).ToList();
            fundSourceAllocationRequestAmounts.AddRange(projectFundSourceAllocationRequestsUpdated.Where(x => !fundSourceAllocationsInOriginal.Contains(x.FundSourceAllocation.FundSourceAllocationID)).Select(x =>
                new FundSourceAllocationRequestAmount(x.FundSourceAllocation, x.TotalAmount,x.MatchAmount, x.PayAmount, HtmlDiffContainer.DisplayCssClassAddedElement)));
            fundSourceAllocationRequestAmounts.Where(x => fundSourceAllocationsOnlyInOriginal.Contains(x.FundSourceAllocation.FundSourceAllocationID)).ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassDeletedElement);
            return GeneratePartialViewForExpectedFundingAsString(fundSourceAllocationRequestAmounts, fundingSources, estimatedTotalCost, projectFundingSourceNotes, isProjectAnLoaProject);
        }

        private string GeneratePartialViewForModifiedFundingRequests(List<IFundSourceAllocationRequestAmount> projectFundSourceAllocationRequestsOriginal,
            List<IFundSourceAllocationRequestAmount> projectFundSourceAllocationRequestsUpdated, List<FundingSource> fundingSources, decimal? estimatedTotalCost, string projectFundingSourceNotes, bool isProjectAnLoaProject)
        {
            var fundSourceAllocationsInOriginal = projectFundSourceAllocationRequestsOriginal.Select(x => x.FundSourceAllocation.FundSourceAllocationID).ToList();
            var fundSourceAllocationsInUpdated = projectFundSourceAllocationRequestsUpdated.Select(x => x.FundSourceAllocation.FundSourceAllocationID).ToList();
            var fundSourceAllocationsOnlyInUpdated = fundSourceAllocationsInUpdated.Where(x => !fundSourceAllocationsInOriginal.Contains(x)).ToList();
            var fundSourceAllocationRequestAmounts = projectFundSourceAllocationRequestsUpdated.Select(x => new FundSourceAllocationRequestAmount(x)).ToList();
            fundSourceAllocationRequestAmounts.AddRange(projectFundSourceAllocationRequestsOriginal.Where(x => !fundSourceAllocationsInUpdated.Contains(x.FundSourceAllocation.FundSourceAllocationID)).Select(x =>
                new FundSourceAllocationRequestAmount(x.FundSourceAllocation, x.TotalAmount,x.MatchAmount, x.PayAmount, HtmlDiffContainer.DisplayCssClassDeletedElement)));
            fundSourceAllocationRequestAmounts.Where(x => fundSourceAllocationsOnlyInUpdated.Contains(x.FundSourceAllocation.FundSourceAllocationID)).ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassAddedElement);
            return GeneratePartialViewForExpectedFundingAsString(fundSourceAllocationRequestAmounts, fundingSources, estimatedTotalCost, projectFundingSourceNotes, isProjectAnLoaProject);
        }

        private string GeneratePartialViewForExpectedFundingAsString(List<FundSourceAllocationRequestAmount> fundSourceAllocationRequestAmounts, List<FundingSource> fundingSources, decimal? estimatedTotalCost, string projectFundingSourceNotes, bool isProjectAnLoaProject)
        {
            var viewData = new ProjectFundSourceAllocationRequestsDetailViewData(fundSourceAllocationRequestAmounts, fundingSources, estimatedTotalCost, projectFundingSourceNotes, isProjectAnLoaProject);
            var partialViewAsString = RenderPartialViewToString(ProjectExpectedFundingPartialViewPath, viewData);
            return partialViewAsString;
        }

        //[HttpGet]
        //[ProjectUpdateCreateEditSubmitFeature]
        //public PartialViewResult DiffBudgets(ProjectPrimaryKey projectPrimaryKey)
        //{
        //    var htmlDiffContainer = DiffBudgetsImpl(projectPrimaryKey);
        //    var htmlDiff = new HtmlDiff.HtmlDiff(htmlDiffContainer.OriginalHtml, htmlDiffContainer.UpdatedHtml);
        //    return ViewHtmlDiff(htmlDiff.Build(), string.Empty);
        //}

        //private HtmlDiffContainer DiffBudgetsImpl(ProjectPrimaryKey projectPrimaryKey)
        //{
        //    var project = projectPrimaryKey.EntityObject;
        //    var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");

        //    var projectBudgetsOriginal = project.ProjectBudgets.ToList();
        //    var calendarYearsOriginal = projectBudgetsOriginal.CalculateCalendarYearRangeForBudgets(project);
        //    var projectBudgetsUpdated = projectUpdateBatch.ProjectBudgetUpdates.ToList();
        //    var calendarYearsUpdated = projectBudgetsUpdated.CalculateCalendarYearRangeForBudgets(projectUpdateBatch.ProjectUpdate);

        //    var originalHtml = GeneratePartialViewForOriginalBudgets(new List<IProjectBudgetAmount>(projectBudgetsOriginal),
        //        calendarYearsOriginal,
        //        new List<IProjectBudgetAmount>(projectBudgetsUpdated),
        //        calendarYearsUpdated);

        //    var updatedHtml = GeneratePartialViewForModifiedBudgets(new List<IProjectBudgetAmount>(projectBudgetsOriginal),
        //        calendarYearsOriginal,
        //        new List<IProjectBudgetAmount>(projectBudgetsUpdated),
        //        calendarYearsUpdated);
        //    var htmlDiffContainer = new HtmlDiffContainer(originalHtml, updatedHtml);
        //    return htmlDiffContainer;
        //}

        //private string GeneratePartialViewForBudgetsAsString(List<ProjectBudgetAmount2> projectBudgetAmounts, List<CalendarYearString> calendarYearStrings)
        //{
        //    var viewData = new Views.Shared.ProjectUpdateDiffControls.ProjectBudgetSummaryViewData(projectBudgetAmounts, calendarYearStrings);
        //    var partialViewAsString = RenderPartialViewToString(TransporationBudgetsPartialViewPath, viewData);
        //    return partialViewAsString;
        //}

        //private string GeneratePartialViewForOriginalBudgets(List<IProjectBudgetAmount> projectBudgetAmountsOriginal,
        //    List<int> calendarYearsOriginal,
        //    List<IProjectBudgetAmount> projectBudgetAmountsUpdated,
        //    List<int> calendarYearsUpdated)
        //{
        //    var fundSourceAllocationsInOriginal = projectBudgetAmountsOriginal.Select(x => x.FundSourceAllocationID).Distinct().ToList();
        //    var fundSourceAllocationsInUpdated = projectBudgetAmountsUpdated.Select(x => x.FundSourceAllocationID).Distinct().ToList();
        //    var fundSourceAllocationsOnlyInOriginal = fundSourceAllocationsInOriginal.Where(x => !fundSourceAllocationsInUpdated.Contains(x)).ToList();

        //    var projectBudgetAmountsInOriginal = ProjectBudgetAmount2.CreateFromProjectBudgets(projectBudgetAmountsOriginal,
        //        calendarYearsOriginal);
        //    // we need to zero out calendar year values only in original
        //    foreach (var projectBudgetAmount2 in projectBudgetAmountsInOriginal)
        //    {
        //        ZeroOutBudget(projectBudgetAmount2, calendarYearsOriginal.Except(calendarYearsUpdated).ToList());
        //    }

        //    var projectBudgetAmountsInUpdated = ProjectBudgetAmount2.CreateFromProjectBudgets(projectBudgetAmountsUpdated, calendarYearsUpdated);

        //    // find the ones that are only in the modified set and add them and mark them as "added"
        //    projectBudgetAmountsInOriginal.AddRange(
        //        projectBudgetAmountsInUpdated.Where(x => !fundSourceAllocationsInOriginal.Contains(x.FundSourceAllocationID))
        //            .Select(x => ProjectBudgetAmount2.Clone(x, HtmlDiffContainer.DisplayCssClassAddedElement))
        //            .ToList());
        //    // find the ones only in original and mark them as "deleted"
        //    projectBudgetAmountsInOriginal.Where(x => fundSourceAllocationsOnlyInOriginal.Contains(x.FundSourceAllocationID))
        //        .ForEach(x =>
        //        {
        //            ZeroOutBudget(x, calendarYearsOriginal);
        //            x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassDeletedElement;
        //        });

        //    var calendarYearStrings = GetCalendarYearStringsForDiffForOriginal(calendarYearsOriginal, calendarYearsUpdated);
        //    return GeneratePartialViewForBudgetsAsString(projectBudgetAmountsInOriginal, calendarYearStrings);
        //}

        //private static void ZeroOutBudget(ProjectBudgetAmount2 projectBudgetAmount2, List<int> calendarYearsToZeroOut)
        //{
        //    foreach (var projectCostTypeCalendarYearBudget in projectBudgetAmount2.ProjectCostTypeCalendarYearBudgets)
        //    {
        //        foreach (var calendarYear in calendarYearsToZeroOut)
        //        {
        //            projectCostTypeCalendarYearBudget.CalendarYearBudget[calendarYear] = 0;
        //        }                
        //    }
        //}

        //private string GeneratePartialViewForModifiedBudgets(List<IProjectBudgetAmount> projectBudgetAmountsOriginal,
        //    List<int> calendarYearsOriginal,
        //    List<IProjectBudgetAmount> projectBudgetAmountsUpdated,
        //    List<int> calendarYearsUpdated)
        //{
        //    var fundSourceAllocationsInOriginal = projectBudgetAmountsOriginal.Select(x => x.FundSourceAllocationID).Distinct().ToList();
        //    var fundSourceAllocationsInUpdated = projectBudgetAmountsUpdated.Select(x => x.FundSourceAllocationID).Distinct().ToList();
        //    var fundSourceAllocationsOnlyInUpdated = fundSourceAllocationsInUpdated.Where(x => !fundSourceAllocationsInOriginal.Contains(x)).ToList();

        //    var projectBudgetAmountsInOriginal = ProjectBudgetAmount2.CreateFromProjectBudgets(projectBudgetAmountsOriginal, calendarYearsOriginal);
        //    var projectBudgetAmountsInUpdated = ProjectBudgetAmount2.CreateFromProjectBudgets(projectBudgetAmountsUpdated, calendarYearsUpdated);

        //    // find the ones that are only in the original set and add them and mark them as "deleted"
        //    projectBudgetAmountsInUpdated.AddRange(
        //        projectBudgetAmountsInOriginal.Where(x => !fundSourceAllocationsInUpdated.Contains(x.FundSourceAllocationID))
        //            .Select(x => ProjectBudgetAmount2.Clone(x, HtmlDiffContainer.DisplayCssClassDeletedElement))
        //            .ToList());
        //    // find the ones only in modified and mark them as "added"
        //    projectBudgetAmountsInUpdated.Where(x => fundSourceAllocationsOnlyInUpdated.Contains(x.FundSourceAllocationID)).ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassAddedElement);

        //    var calendarYearStrings = GetCalendarYearStringsForDiffForUpdated(calendarYearsOriginal, calendarYearsUpdated);
        //    return GeneratePartialViewForBudgetsAsString(projectBudgetAmountsInUpdated, calendarYearStrings);
        //}

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult DiffPhotos(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            var htmlDiffContainer = DiffPhotosImpl(projectUpdateBatch);
            var htmlDiff = new HtmlDiff.HtmlDiff(htmlDiffContainer.OriginalHtml, htmlDiffContainer.UpdatedHtml);
            return ViewHtmlDiff(htmlDiff.Build(), string.Empty);
        }

        private HtmlDiffContainer DiffPhotosImpl(ProjectUpdateBatch projectUpdateBatch)
        {
            var project = projectUpdateBatch.Project;

            var originalImages = new List<IFileResourcePhoto>(project.ProjectImages);
            var updatedImages = new List<IFileResourcePhoto>(projectUpdateBatch.ProjectImageUpdates);

            var dummyProject = Project.CreateNewBlank(
                ProjectType.CreateNewBlank(TaxonomyBranch.CreateNewBlank(TaxonomyTrunk.CreateNewBlank())),
                ProjectStage.Completed,
                ProjectLocationSimpleType.None,
                ProjectApprovalStatus.Approved
            );

            dummyProject.FocusArea = FocusArea.CreateNewBlank(FocusAreaStatus.Completed, DNRUplandRegion.CreateNewBlank());

            var dummyProjectUpdateBatch = ProjectUpdateBatch.CreateNewBlank(dummyProject, CurrentPerson, ProjectUpdateState.Created);

            foreach (var updatedImage in projectUpdateBatch.ProjectImageUpdates)
            {
                var matchingOriginalImage = originalImages.SingleOrDefault(x => updatedImage.ProjectImageID.HasValue && updatedImage.ProjectImageID == x.EntityImageIDAsNullable);
                if (matchingOriginalImage == null)
                {
                    var placeHolderImage = new ProjectImage(updatedImage.FileResource,
                        dummyProject,
                        updatedImage.Caption,
                        updatedImage.Credit,
                        updatedImage.IsKeyPhoto,
                        updatedImage.ExcludeFromFactSheet)
                    {
                        AdditionalCssClasses = new List<string>() {"added-photo"},
                        ProjectImageTimingID = updatedImage.ProjectImageTiming?.ProjectImageTimingID
                    };
                    updatedImage.AdditionalCssClasses = new List<string>() { "added-photo" };
                    originalImages.Add(placeHolderImage);
                }
            }

            foreach (var originalImage in project.ProjectImages)
            {
                var matchingUpdatedImage = updatedImages.SingleOrDefault(x => originalImage.ProjectImageID == x.EntityImageIDAsNullable);
                if (matchingUpdatedImage == null)
                {
                    var placeHolderImage = new ProjectImageUpdate(dummyProjectUpdateBatch.ProjectUpdateBatchID,
                        originalImage.Caption,
                        originalImage.Credit,
                        originalImage.IsKeyPhoto,
                        originalImage.ExcludeFromFactSheet)
                    {
                        ProjectImageTimingID = originalImage.ProjectImageTiming?.ProjectImageTimingID,
                        FileResource = originalImage.FileResource,
                        ProjectImageID = originalImage.ProjectImageID,
                        AdditionalCssClasses = new List<string>() { "deleted-photo" }
                    };
                    originalImage.AdditionalCssClasses = new List<string>() { "deleted-photo" };
                    updatedImages.Add(placeHolderImage);
                }
                else
                {
                    originalImage.OrderBy = matchingUpdatedImage.CaptionOnFullView;
                }
            }

            var original = GeneratePartialViewForPhotos(originalImages);
            var updated = GeneratePartialViewForPhotos(updatedImages);

            var htmlDiff = new HtmlDiffContainer(original, updated);
            return htmlDiff;
        }

        private string GeneratePartialViewForPhotos(IEnumerable<IFileResourcePhoto> images)
        {
            var viewData = new ImageGalleryViewData(CurrentPerson, "ProjectImageDiff", images, false, string.Empty, string.Empty, false, x => x.CaptionOnFullView, "Photo");
            var partialViewAsString = RenderPartialViewToString(ImageGalleryPartialViewPath, viewData);
            return partialViewAsString;
        }

        private static bool IsLocationSimpleUpdated(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");

            if (project.ProjectLocationSimpleTypeID != projectUpdateBatch.ProjectUpdate.ProjectLocationSimpleTypeID)
                return true;

            if (project.ProjectLocationNotes != projectUpdateBatch.ProjectUpdate.ProjectLocationNotes)
                return true;

            switch (project.ProjectLocationSimpleType.ToEnum)
            {
                case ProjectLocationSimpleTypeEnum.PointOnMap:
                    if (project.ProjectLocationPoint == null || projectUpdateBatch.ProjectUpdate.ProjectLocationPoint == null)
                    {
                        SitkaLogger.Instance.LogDetailedErrorMessage($"{FieldDefinition.Project.GetFieldDefinitionLabel()} {project.ProjectID} appears to have inconsistent simple location configuration.");
                        return true;
                    }
                    return project.ProjectLocationPoint.ToSqlGeometry().STEquals(projectUpdateBatch.ProjectUpdate.ProjectLocationPoint.ToSqlGeometry()).IsFalse;
            }

            return false;
        }

        private static bool IsLocationDetailedUpdated(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");

            var originalLocationDetailed = project.GetProjectLocationDetails().ToList();
            var updatedLocationDetailed = projectUpdateBatch.ProjectLocationUpdates;

            if (!originalLocationDetailed.Any() && !updatedLocationDetailed.Any())
                return false;

            if (originalLocationDetailed.Count != updatedLocationDetailed.Count)
                return true;

            var originalLocationAsListOfStrings = originalLocationDetailed.Select(x => x.ProjectLocationGeometry.ToString() + x.ProjectLocationNotes).ToList();
            var updatedLocationAsListOfStrings = updatedLocationDetailed.Select(x => x.ProjectLocationGeometry.ToString() + x.ProjectLocationUpdateNotes).ToList();

            var enumerable = originalLocationAsListOfStrings.Except(updatedLocationAsListOfStrings);
            return enumerable.Any();
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult DiffExternalLinks(ProjectPrimaryKey projectPrimaryKey)
        {
            var htmlDiffContainer = DiffExternalLinksImpl(projectPrimaryKey);
            var htmlDiff = new HtmlDiff.HtmlDiff(htmlDiffContainer.OriginalHtml, htmlDiffContainer.UpdatedHtml);
            return ViewHtmlDiff(htmlDiff.Build(), string.Empty);
        }

        private HtmlDiffContainer DiffExternalLinksImpl(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            var entityExternalLinksOriginal = new List<IEntityExternalLink>(project.ProjectExternalLinks);
            var entityExternalLinksUpdated = new List<IEntityExternalLink>(projectUpdateBatch.ProjectExternalLinkUpdates);

            var originalHtml = GeneratePartialViewForExternalLinks(entityExternalLinksOriginal);
            var updatedHtml = GeneratePartialViewForExternalLinks(entityExternalLinksUpdated);

            return new HtmlDiffContainer(originalHtml, updatedHtml);
        }

        private string GeneratePartialViewForExternalLinks(List<IEntityExternalLink> entityExternalLinksOriginal )
        {

            var externalLinksOriginal = ExternalLink.CreateFromEntityExternalLink(entityExternalLinksOriginal);

            return GeneratePartialViewForExternalLinksImpl(externalLinksOriginal);
        }


        private string GeneratePartialViewForExternalLinksImpl(List<ExternalLink> entityExternalLinks)
        {
            var viewData = new EntityExternalLinksViewData(entityExternalLinks);
            var partialViewToString = RenderPartialViewToString(ExternalLinksPartialViewPath, viewData);
            return partialViewToString;
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult DiffNotesAndDocuments(ProjectPrimaryKey projectPrimaryKey)
        {
            var htmlDiffContainer = DiffNotesAndDocumentsImpl(projectPrimaryKey);
            var htmlDiff = new HtmlDiff.HtmlDiff(htmlDiffContainer.OriginalHtml, htmlDiffContainer.UpdatedHtml);
            return ViewHtmlDiff(htmlDiff.Build(), string.Empty);
        }

        private HtmlDiffContainer DiffNotesAndDocumentsImpl(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project,$"There is no current {FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");
            var entityNotesOriginal = new List<IEntityNote>(project.ProjectNotes);
            var entityNotesUpdated = new List<IEntityNote>(projectUpdateBatch.ProjectNoteUpdates);

            var originalHtmlNotes = GeneratePartialViewForOriginalNotes(entityNotesOriginal, entityNotesUpdated);
            var updatedHtmlNotes = GeneratePartialViewForModifiedNotes(entityNotesOriginal, entityNotesUpdated);
            // TODO: Commented out until such time as it is appropriate to take this feature live
            //var entityDocumentsOriginal = new List<IEntityDocument>(project.ProjectDocuments);
            //var entityDocumentsUpdated = new List<IEntityDocument>(projectUpdateBatch.ProjectDocumentUpdates);
            //var originalHtmlDocuments = GeneratePartialViewForOriginalDocuments(entityDocumentsOriginal, entityDocumentsUpdated);
            //var updatedHtmlDocuments = GeneratePartialViewForModifiedDocuments(entityDocumentsOriginal, entityDocumentsUpdated);

            var originalHtml = originalHtmlNotes;//+ originalHtmlDocuments;
            var updatedHtml = updatedHtmlNotes; //updatedHtmlDocuments;

            return new HtmlDiffContainer(originalHtml, updatedHtml);
        }

        private string GeneratePartialViewForOriginalNotes(List<IEntityNote> entityNotesOriginal, List<IEntityNote> entityNotesUpdated)
        {
            var urlsInOriginal = entityNotesOriginal.Select(x => x.Note).Distinct().ToList();
            var urlsInModified = entityNotesUpdated.Select(x => x.Note).Distinct().ToList();
            var urlsOnlyInOriginal = urlsInOriginal.Where(x => !urlsInModified.Contains(x)).ToList();

            var externalLinksOriginal = EntityNote.CreateFromEntityNote(entityNotesOriginal);
            var externalLinksUpdated = EntityNote.CreateFromEntityNote(entityNotesUpdated);
            // find the ones that are only in the modified set and add them and mark them as "added"
            externalLinksOriginal.AddRange(
                externalLinksUpdated.Where(x => !urlsInOriginal.Contains(x.Note))
                    .Select(x => new EntityNote(x.Created, x.CreatedBy, x.LastUpdated, x.LastUpdatedBy, x.DeleteUrl, x.EditUrl, x.Note, HtmlDiffContainer.DisplayCssClassAddedElement))
                    .ToList());
            // find the ones only in original and mark them as "deleted"
            externalLinksOriginal.Where(x => urlsOnlyInOriginal.Contains(x.Note)).ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassDeletedElement);
            return GeneratePartialViewForNotes(externalLinksOriginal);
        }


        private string GeneratePartialViewForModifiedNotes(List<IEntityNote> entityNotesOriginal, List<IEntityNote> entityNotesUpdated)
        {
            var urlsInOriginal = entityNotesOriginal.Select(x => x.Note).Distinct().ToList();
            var urlsInUpdated = entityNotesUpdated.Select(x => x.Note).Distinct().ToList();
            var urlsOnlyInUpdated = urlsInUpdated.Where(x => !urlsInOriginal.Contains(x)).ToList();

            var externalLinksOriginal = EntityNote.CreateFromEntityNote(entityNotesOriginal);
            var externalLinksUpdated = EntityNote.CreateFromEntityNote(entityNotesUpdated);
            // find the ones that are only in the original set and add them and mark them as "deleted"
            externalLinksUpdated.AddRange(
                externalLinksOriginal.Where(x => !urlsInUpdated.Contains(x.Note))
                    .Select(x => new EntityNote(x.Created, x.CreatedBy, x.LastUpdated, x.LastUpdatedBy, x.DeleteUrl, x.EditUrl, x.Note, HtmlDiffContainer.DisplayCssClassDeletedElement))
                    .ToList());
            externalLinksUpdated.Where(x => urlsOnlyInUpdated.Contains(x.Note)).ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassAddedElement);
            return GeneratePartialViewForNotes(externalLinksUpdated);
        }

        private string GeneratePartialViewForNotes(List<EntityNote> entityNotes)
        {
            var viewData = new EntityNotesViewData(entityNotes, null, $"{FieldDefinition.Project.GetFieldDefinitionLabel()}", false);
            var partialViewToString = RenderPartialViewToString(EntityNotesPartialViewPath, viewData);
            return partialViewToString;
        }


        // TODO: Commented out until such time as it is appropriate to take this feature live
        //private string GeneratePartialViewForOriginalDocuments(List<IEntityDocument> entityDocumentsOriginal, List<IEntityDocument> entityDocumentsUpdated)
        //{
        //    var fileResourceIDsInOriginal = entityDocumentsOriginal.Select(x=>x.FileResource.FileResourceID).ToList();
        //    var fileResourceIDsInModified = entityDocumentsUpdated.Select(x => x.FileResource.FileResourceID).ToList();
        //    var urlsOnlyInOriginal = fileResourceIDsInOriginal.Where(x => !fileResourceIDsInModified.Contains(x)).ToList();

        //    var externalLinksOriginal = EntityDocument.CreateFromEntityDocument(entityDocumentsOriginal);
        //    var externalLinksUpdated = EntityDocument.CreateFromEntityDocument(entityDocumentsUpdated);
        //    // find the ones that are only in the modified set and add them and mark them as "added"
        //    externalLinksOriginal.AddRange(
        //        externalLinksUpdated.Where(x => !fileResourceIDsInOriginal.Contains(x.FileResource.FileResourceID))
        //            .Select(x => new EntityDocument(x.DeleteUrl,x.EditUrl,x.FileResource,HtmlDiffContainer.DisplayCssClassAddedElement, x.DisplayName, x.Description))
        //            .ToList());
        //    // find the ones only in original and mark them as "deleted"
        //    externalLinksOriginal.Where(x => urlsOnlyInOriginal.Contains(x.FileResource.FileResourceID)).ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassDeletedElement);
        //    return GeneratePartialViewForDocuments(externalLinksOriginal.OrderBy(x => x.FileResource.FileResourceID).ToList());
        ////}

        //private string GeneratePartialViewForModifiedDocuments(List<IEntityDocument> entityDocumentsOriginal, List<IEntityDocument> entityDocumentsUpdated)
        //{
        //     var fileResouceIDsInOriginal = entityDocumentsOriginal.Select(x => x.FileResource.FileResourceID).ToList();
        //    var fileResourceIDsInModified = entityDocumentsUpdated.Select(x => x.FileResource.FileResourceID).ToList();
        //    var urlsOnlyInUpdated = fileResourceIDsInModified.Where(x => !fileResouceIDsInOriginal.Contains(x)).ToList();

        //    var externalLinksOriginal = EntityDocument.CreateFromEntityDocument(entityDocumentsOriginal);
        //    var externalLinksUpdated = EntityDocument.CreateFromEntityDocument(entityDocumentsUpdated);
        //    // find the ones that are only in the original set and add them and mark them as "deleted"
        //    externalLinksUpdated.AddRange(
        //        externalLinksOriginal.Where(x => !fileResourceIDsInModified.Contains(x.FileResource.FileResourceID))
        //            .Select(x => new EntityDocument(x.DeleteUrl, x.EditUrl, x.FileResource, HtmlDiffContainer.DisplayCssClassDeletedElement, x.DisplayName, x.Description))
        //            .ToList());
        //    externalLinksUpdated.Where(x => urlsOnlyInUpdated.Contains(x.FileResource.FileResourceID)).ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassAddedElement);
        //    return GeneratePartialViewForDocuments(externalLinksUpdated.OrderBy(x=>x.FileResource.FileResourceID).ToList());
        //}
        //private string GeneratePartialViewForDocuments(List<EntityDocument> entityDocuments)
        //{
        //    var viewData = new ProjectDocumentsDetailViewData(entityDocuments, null, $"{FieldDefinition.Project.GetFieldDefinitionLabel()}", false, false);
        //    var partialViewToString = RenderPartialViewToString(ProjectDocumentsPartialViewPath, viewData);
        //    return partialViewToString;
        //}


        [HttpGet]
        [LoggedInAndNotUnassignedRoleUnclassifiedFeature]
        public PartialViewResult ProjectUpdateBatchDiff(ProjectUpdateBatchPrimaryKey projectUpdateBatchPrimaryKey)
        {
            var projectUpdateBatch = projectUpdateBatchPrimaryKey.EntityObject;
            var viewData = new ProjectUpdateBatchDiffLogViewData(CurrentPerson, projectUpdateBatch);
            var partialViewToString = RenderPartialViewToString(ProjectUpdateBatchDiffLogPartialViewPath, viewData);
            return ViewHtmlDiff(partialViewToString,$"{FieldDefinition.Project.GetFieldDefinitionLabel()} Update from {projectUpdateBatch.LastUpdateDate.ToLongDateString()}");
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult Organizations(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();

            var viewModel = new OrganizationsViewModel(projectUpdateBatch);

            return ViewOrganizations(projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Organizations(ProjectPrimaryKey projectPrimaryKey, OrganizationsViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();

            if (!ModelState.IsValid)
            {
                return ViewOrganizations(projectUpdateBatch, viewModel);
            }

            

            HttpRequestStorage.DatabaseEntities.ProjectOrganizationUpdates.Load();
            var projectOrganizationUpdates = projectUpdateBatch.ProjectOrganizationUpdates.ToList();
            var allProjectOrganizationUpdates = HttpRequestStorage.DatabaseEntities.ProjectOrganizationUpdates.Local;

            viewModel.UpdateModel(projectUpdateBatch,projectOrganizationUpdates, allProjectOrganizationUpdates);
            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.OrganizationsComment = viewModel.Comments;
            }

            SetMessageForDisplay($"{FieldDefinition.Project.GetFieldDefinitionLabel()} {FieldDefinition.Organization.GetFieldDefinitionLabelPluralized()} successfully saved.");

            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch,
                ProjectUpdateSection.Organizations.ProjectUpdateSectionDisplayName);
        }

        private ActionResult ViewOrganizations(ProjectUpdateBatch projectUpdateBatch, OrganizationsViewModel viewModel)
        {
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var organizationsValidationResult = projectUpdateBatch.ValidateOrganizations();

            var allOrganizations = HttpRequestStorage.DatabaseEntities.Organizations.GetActiveOrganizations();
            var allPeople = HttpRequestStorage.DatabaseEntities.People.ToList().Where(x=>x.IsFullUser()).OrderBy(p => p.FullNameFirstLastAndOrg).ToList();
            if (!allPeople.Contains(CurrentPerson))
            {
                allPeople.Add(CurrentPerson);
            }
            var allRelationshipTypes = HttpRequestStorage.DatabaseEntities.RelationshipTypes.ToList();
            var defaultPrimaryContact = projectUpdateBatch.Project?.GetPrimaryContact() ?? CurrentPerson.Organization.PrimaryContactPerson;
            
            var editOrganizationsViewData = new EditOrganizationsViewData(allOrganizations, allRelationshipTypes, defaultPrimaryContact, projectUpdateBatch.Project?.IsLeadImplementerOrganizationImported() ?? false, false);

            var projectOrganizationsDetailViewData = new ProjectOrganizationsDetailViewData(projectUpdateBatch.ProjectOrganizationUpdates.Select(x => new ProjectOrganizationRelationship(x.ProjectUpdateBatch.Project, x.Organization, x.RelationshipType)).ToList(), projectUpdateBatch.ProjectUpdate.GetPrimaryContact());
            var viewData = new OrganizationsViewData(CurrentPerson, projectUpdateBatch, updateStatus, editOrganizationsViewData, organizationsValidationResult,projectOrganizationsDetailViewData);

            return RazorView<Organizations, OrganizationsViewData, OrganizationsViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult Contacts(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();

            var viewModel = new ContactsViewModel(projectUpdateBatch);

            return ViewContacts(projectUpdateBatch, viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Contacts(ProjectPrimaryKey projectPrimaryKey, ContactsViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();

            if (!ModelState.IsValid)
            {
                return ViewContacts(projectUpdateBatch, viewModel);
            }

            

            HttpRequestStorage.DatabaseEntities.ProjectPersonUpdates.Load();
            var projectPersonUpdates = projectUpdateBatch.ProjectPersonUpdates.ToList();
            var allProjectPersonUpdates = HttpRequestStorage.DatabaseEntities.ProjectPersonUpdates.Local;

            viewModel.UpdateModel(projectUpdateBatch,projectPersonUpdates, allProjectPersonUpdates);
            if (projectUpdateBatch.IsSubmitted)
            {
                projectUpdateBatch.ContactsComment = viewModel.Comments;
            }

            SetMessageForDisplay($"{FieldDefinition.Project.GetFieldDefinitionLabel()} {FieldDefinition.Contact.GetFieldDefinitionLabelPluralized()} successfully saved.");

            return TickleLastUpdateDateAndGoToNextSection(viewModel, projectUpdateBatch,
                ProjectUpdateSection.Contacts.ProjectUpdateSectionDisplayName);
        }

        private ActionResult ViewContacts(ProjectUpdateBatch projectUpdateBatch, ContactsViewModel viewModel)
        {
            var updateStatus = GetUpdateStatus(projectUpdateBatch);
            var organizationsValidationResult = projectUpdateBatch.ValidateContacts();

            var allPeople = HttpRequestStorage.DatabaseEntities.People.ToList();

            var editPersonsViewData =
                new EditPeopleViewData(allPeople, ProjectPersonRelationshipType.All, CurrentPerson, projectUpdateBatch.Project.IsPrivateLandownerImported(), false);

            var projectPersonsDetailViewData = new ProjectPeopleDetailViewData(projectUpdateBatch.ProjectPersonUpdates.Select(x => new ProjectPersonRelationship(x.ProjectUpdateBatch.Project, x.Person, x.ProjectPersonRelationshipType)).ToList(), CurrentPerson);
            var viewData = new ContactsViewData(CurrentPerson, projectUpdateBatch, updateStatus, editPersonsViewData, organizationsValidationResult,projectPersonsDetailViewData);

            return RazorView<Contacts, ContactsViewData, ContactsViewModel>(viewData, viewModel);
        }

        private UpdateStatus GetUpdateStatus(ProjectUpdateBatch projectUpdateBatch)
        {
//            var isBudgetsUpdated = DiffBudgetsImpl(projectUpdateBatch.ProjectID).HasChanged;
            // ReSharper disable once ConvertToConstant.Local
            var isBudgetsUpdated = false;
            var isLocationSimpleUpdated = IsLocationSimpleUpdated(projectUpdateBatch.ProjectID);
            var isLocationDetailUpdated = IsLocationDetailedUpdated(projectUpdateBatch.ProjectID);
            var isRegionsUpdated = IsRegionUpdated(projectUpdateBatch);
            var isCountiesUpdated = IsCountyUpdated(projectUpdateBatch);

            var isPriorityLandscapesUpdated = IsPriorityLandscapeUpdated(projectUpdateBatch);
            var isExternalLinksUpdated = DiffExternalLinksImpl(projectUpdateBatch.ProjectID).HasChanged;
            var isNotesUpdated = DiffNotesAndDocumentsImpl(projectUpdateBatch.ProjectID).HasChanged;
            var isTreatmentsUpdated = false;  // MCS the ability to diff treatments will be added in a different story

            var isExpectedFundingUpdated = DiffExpectedFundingImpl(projectUpdateBatch.ProjectID).HasChanged;

            var isOrganizationsUpdated = DiffOrganizationsImpl(projectUpdateBatch.ProjectID).HasChanged;
            var isContactsUpdated = DiffContactsImpl(projectUpdateBatch.ProjectID).HasChanged;

            //Must be called last, since basics actually changes the Project object which can break the other Diff functions
            var isBasicsUpdated = DiffBasicsImpl(projectUpdateBatch.ProjectID).HasChanged;

            return new UpdateStatus(isBasicsUpdated,
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                isBudgetsUpdated,
                projectUpdateBatch.IsPhotosUpdated,
                isLocationSimpleUpdated,
                isLocationDetailUpdated,
                isPriorityLandscapesUpdated,
                isRegionsUpdated,
                isCountiesUpdated,
                isExternalLinksUpdated,
                isNotesUpdated,
                isExpectedFundingUpdated,
                isOrganizationsUpdated,
                isContactsUpdated,
                isTreatmentsUpdated
                );
        }

        private static bool IsRegionUpdated(ProjectUpdateBatch projectUpdateBatch)
        {
            var project = projectUpdateBatch.Project;
            var originalRegionIDs = project.ProjectRegions.Select(x => x.DNRUplandRegionID).ToList();
            var updatedRegionIDs = projectUpdateBatch.ProjectRegionUpdates.Select(x => x.DNRUplandRegionID).ToList();

            if (!originalRegionIDs.Any() && !updatedRegionIDs.Any())
                return false;

            if (originalRegionIDs.Count != updatedRegionIDs.Count)
                return true;

            var enumerable = originalRegionIDs.Except(updatedRegionIDs);
            return enumerable.Any();
        }

        private static bool IsCountyUpdated(ProjectUpdateBatch projectUpdateBatch)
        {
            var project = projectUpdateBatch.Project;
            var originalCountyIDs = project.ProjectCounties.Select(x => x.CountyID).ToList();
            var updatedCountyIDs = projectUpdateBatch.ProjectCountyUpdates.Select(x => x.CountyID).ToList();

            if (!originalCountyIDs.Any() && !updatedCountyIDs.Any())
                return false;

            if (originalCountyIDs.Count != updatedCountyIDs.Count)
                return true;

            var enumerable = originalCountyIDs.Except(updatedCountyIDs);
            return enumerable.Any();
        }
        private static bool IsPriorityLandscapeUpdated(ProjectUpdateBatch projectUpdateBatch)
        {
            var project = projectUpdateBatch.Project;
            var originalPriorityLandscapeIDs = project.ProjectPriorityLandscapes.Select(x => x.PriorityLandscapeID).ToList();
            var updatedPriorityLandscapeIDs = projectUpdateBatch.ProjectPriorityLandscapeUpdates.Select(x => x.PriorityLandscapeID).ToList();

            if (!originalPriorityLandscapeIDs.Any() && !updatedPriorityLandscapeIDs.Any())
                return false;

            if (originalPriorityLandscapeIDs.Count != updatedPriorityLandscapeIDs.Count)
                return true;

            var enumerable = originalPriorityLandscapeIDs.Except(updatedPriorityLandscapeIDs);
            return enumerable.Any();
        }


        private PartialViewResult ViewHtmlDiff(string htmlDiff, string diffTitle)
        {
            var viewData = new HtmlDiffSummaryViewData(htmlDiff, diffTitle);
            return RazorPartialView<HtmlDiffSummary, HtmlDiffSummaryViewData>(viewData);
        }

        private static List<CalendarYearString> GetCalendarYearStringsForDiffForOriginal(List<int> calendarYearsOriginal, List<int> calendarYearsUpdated)
        {
            var calendarYearStrings = new List<CalendarYearString>();
            // get the calendar years that are not in the original and add them and mark as "added"
            calendarYearsUpdated.Where(x => !calendarYearsOriginal.Contains(x)).ForEach(x => calendarYearStrings.Add(new CalendarYearString(x, AddedDeletedOrRealElement.AddedElement)));
            // now go through all the original calendar years and mark them as either "deleted" or an update, with "deleted" meaning not being in the modified set
            calendarYearStrings.AddRange(calendarYearsOriginal.Select(x => new CalendarYearString(x, !calendarYearsUpdated.Contains(x) ? AddedDeletedOrRealElement.DeletedElement : AddedDeletedOrRealElement.RealElement)));
            return calendarYearStrings;
        }

        private static List<CalendarYearString> GetCalendarYearStringsForDiffForUpdated(List<int> calendarYearsOriginal, List<int> calendarYearsUpdated)
        {
            var calendarYearStrings = new List<CalendarYearString>();
            // get the calendar years that are not in the modified and add them and mark as "deleted"
            calendarYearsOriginal.Where(x => !calendarYearsUpdated.Contains(x)).ForEach(x => calendarYearStrings.Add(new CalendarYearString(x, AddedDeletedOrRealElement.DeletedElement)));
            // now go through all the modified calendar years and mark them as either "added" or an update, with "added" meaning not being in the original set
            calendarYearStrings.AddRange(calendarYearsUpdated.Select(i => new CalendarYearString(i, !calendarYearsOriginal.Contains(i) ? AddedDeletedOrRealElement.AddedElement : AddedDeletedOrRealElement.RealElement)));
            return calendarYearStrings;
        }

        private ActionResult TickleLastUpdateDateAndGoToNextSection(FormViewModel viewModel, ProjectUpdateBatch projectUpdateBatch, string currentSectionName)
        {
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            var applicableWizardSections = projectUpdateBatch.GetApplicableWizardSections(true);
            var currentSection = applicableWizardSections.Single(x => x.SectionDisplayName.Equals(currentSectionName, StringComparison.InvariantCultureIgnoreCase));
            var nextProjectUpdateSection = applicableWizardSections.Where(x => x.SortOrder > currentSection.SortOrder).OrderBy(x => x.SortOrder).FirstOrDefault();
            var nextSection = viewModel.AutoAdvance && nextProjectUpdateSection != null ? nextProjectUpdateSection.SectionUrl : currentSection.SectionUrl;
            return Redirect(nextSection);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult RefreshOrganizations(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshOrganizations(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshOrganizations(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            projectUpdateBatch.DeleteProjectOrganizationUpdates();
            // refresh data
            ProjectOrganizationUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshOrganizations(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the {FieldDefinition.Organization.GetFieldDefinitionLabelPluralized()} for this {FieldDefinition.Project.GetFieldDefinitionLabel()}? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()}. Any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }


        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult RefreshContacts(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshContacts(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshContacts(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            projectUpdateBatch.DeleteProjectContactUpdates();
            // refresh data
            ProjectPersonUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshContacts(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the {FieldDefinition.Contact.GetFieldDefinitionLabelPluralized()} for this {FieldDefinition.Project.GetFieldDefinitionLabel()}? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()}. Any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult DiffOrganizations(ProjectPrimaryKey projectPrimaryKey)
        {
            var htmlDiffContainer = DiffOrganizationsImpl(projectPrimaryKey);
            var htmlDiff = new HtmlDiff.HtmlDiff(htmlDiffContainer.OriginalHtml, htmlDiffContainer.UpdatedHtml);
            return ViewHtmlDiff(htmlDiff.Build(), string.Empty);
        }

        private HtmlDiffContainer DiffOrganizationsImpl(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");

            var projectOrganizationsOriginal = new List<IProjectOrganization>(project.ProjectOrganizations.ToList());
            var projectOrganizationsUpdated = new List<IProjectOrganization>(projectUpdateBatch.ProjectOrganizationUpdates.ToList());

            var updatedHtml = GeneratePartialViewForModifiedOrganizations(projectOrganizationsOriginal, projectOrganizationsUpdated, projectUpdateBatch.ProjectUpdate);
            var originalHtml = GeneratePartialViewForOriginalOrganizations(projectOrganizationsOriginal, projectOrganizationsUpdated, projectUpdateBatch.Project);

            return new HtmlDiffContainer(originalHtml, updatedHtml);
        }

        private string GeneratePartialViewForModifiedOrganizations(
            List<IProjectOrganization> projectOrganizationsOriginal,
            List<IProjectOrganization> projectOrganizationsUpdated, ProjectUpdate projectUpdate)
        {
            var organizationsInOriginal = projectOrganizationsOriginal;
            var organizationsInUpdated = projectOrganizationsUpdated;
            var comparer = new ProjectOrganizationEqualityComparer();

            var organizationsOnlyInOriginal = organizationsInOriginal.Where(x => !organizationsInUpdated.Contains(x, comparer)).ToList();
            var projectOrganizations = projectOrganizationsOriginal.Select(x => new ProjectOrganization(x)).ToList();


            projectOrganizations.AddRange(projectOrganizationsUpdated.Where(x => !organizationsInOriginal.Contains(x, comparer)).Select(x =>
                new ProjectOrganization(x.Organization, x.RelationshipType, HtmlDiffContainer.DisplayCssClassAddedElement)));
            projectOrganizations
                .Where(x => organizationsOnlyInOriginal.Contains(x, comparer))
                .ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassDeletedElement);

            return GeneratePartialViewForOrganizationsAsString(projectOrganizations, projectUpdate.GetPrimaryContact());
        }

        private string GeneratePartialViewForOriginalOrganizations(
            List<IProjectOrganization> projectOrganizationsOriginal,
            List<IProjectOrganization> projectOrganizationsUpdated, Project project)
        {
            var organizationsInOriginal = projectOrganizationsOriginal;
            var organizationsInUpdated = projectOrganizationsUpdated;
            var comparer = new ProjectOrganizationEqualityComparer();

            var organizationsOnlyInUpdated = organizationsInUpdated.Where(x => !organizationsInOriginal.Contains(x, comparer)).ToList();
            var projectOrganizations = projectOrganizationsUpdated.Select(x => new ProjectOrganization(x)).ToList();

            projectOrganizations.AddRange(projectOrganizationsOriginal.Where(x => !organizationsInUpdated.Contains(x, comparer)).Select(x =>
                new ProjectOrganization(x.Organization, x.RelationshipType, HtmlDiffContainer.DisplayCssClassDeletedElement)));
            projectOrganizations
                .Where(x => organizationsOnlyInUpdated.Contains(x, comparer))
                .ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassAddedElement);

            return GeneratePartialViewForOrganizationsAsString(projectOrganizations, project.GetPrimaryContact());
        }

        private string GeneratePartialViewForOrganizationsAsString(IEnumerable<ProjectOrganization> projectOrganizations, Person primaryContactPerson)
        {
            var viewData = new ProjectOrganizationsDetailViewData(projectOrganizations.Select(x => new ProjectOrganizationRelationship(x.Project, x.Organization, x.RelationshipType, x.DisplayCssClass)).ToList(), primaryContactPerson);
            var partialViewAsString = RenderPartialViewToString(ProjectOrganizationsPartialViewPath, viewData);
            return partialViewAsString;
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult DiffContacts(ProjectPrimaryKey projectPrimaryKey)
        {
            var htmlDiffContainer = DiffContactsImpl(projectPrimaryKey);
            var htmlDiff = new HtmlDiff.HtmlDiff(htmlDiffContainer.OriginalHtml, htmlDiffContainer.UpdatedHtml);
            return ViewHtmlDiff(htmlDiff.Build(), string.Empty);
        }

        private HtmlDiffContainer DiffContactsImpl(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project, $"There is no current {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} Update for {Models.FieldDefinition.Project.GetFieldDefinitionLabel()} {project.DisplayName}");

            var projectPeopleOriginal = new List<IProjectPerson>(project.ProjectPeople.ToList());
            var projectPeopleUpdated = new List<IProjectPerson>(projectUpdateBatch.ProjectPersonUpdates.ToList());

            var updatedHtml = GeneratePartialViewForModifiedContacts(projectPeopleOriginal, projectPeopleUpdated, projectUpdateBatch.ProjectUpdate);
            var originalHtml = GeneratePartialViewForOriginalContacts(projectPeopleOriginal, projectPeopleUpdated, projectUpdateBatch.Project);

            return new HtmlDiffContainer(originalHtml, updatedHtml);
        }

        private string GeneratePartialViewForModifiedContacts(
            List<IProjectPerson> projectPeopleOriginal,
            List<IProjectPerson> projectPeopleUpdated, ProjectUpdate projectUpdate)
        {
            var peopleInOriginal = projectPeopleOriginal;
            var peopleInUpdated = projectPeopleUpdated;
            var comparer = new ProjectPersonEqualityComparer();

            var peopleOnlyInOriginal = peopleInOriginal.Where(x => !peopleInUpdated.Contains(x, comparer)).ToList();
            var projectPeople = projectPeopleOriginal.Select(x => new ProjectPerson(x)).ToList();


            projectPeople.AddRange(projectPeopleUpdated.Where(x => !peopleInOriginal.Contains(x, comparer)).Select(x =>
                new ProjectPerson(x.Person, x.ProjectPersonRelationshipType, HtmlDiffContainer.DisplayCssClassAddedElement)));
            projectPeople
                .Where(x => peopleOnlyInOriginal.Contains(x, comparer))
                .ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassDeletedElement);

            return GeneratePartialViewForPeopleAsString(projectPeople, projectUpdate.GetPrimaryContact());
        }

        private string GeneratePartialViewForOriginalContacts(
            List<IProjectPerson> projectPeopleOriginal,
            List<IProjectPerson> projectPeopleUpdated, Project project)
        {
            var peopleInOriginal = projectPeopleOriginal;
            var peopleInUpdated = projectPeopleUpdated;
            var comparer = new ProjectPersonEqualityComparer();

            var peopleOnlyInUpdated = peopleInUpdated.Where(x => !peopleInOriginal.Contains(x, comparer)).ToList();
            var projectPeople = projectPeopleUpdated.Select(x => new ProjectPerson(x)).ToList();

            projectPeople.AddRange(projectPeopleOriginal.Where(x => !peopleInUpdated.Contains(x, comparer)).Select(x =>
                new ProjectPerson(x.Person, x.ProjectPersonRelationshipType, HtmlDiffContainer.DisplayCssClassDeletedElement)));
            projectPeople
                .Where(x => peopleOnlyInUpdated.Contains(x, comparer))
                .ForEach(x => x.DisplayCssClass = HtmlDiffContainer.DisplayCssClassAddedElement);

            return GeneratePartialViewForPeopleAsString(projectPeople, project.GetPrimaryContact());
        }

        private string GeneratePartialViewForPeopleAsString(IEnumerable<ProjectPerson> projectPeople, Person primaryContactPerson)
        {
            var viewData = new ProjectPeopleDetailViewData(projectPeople.Select(x => new ProjectPersonRelationship(x.Project, x.Person, x.ProjectPersonRelationshipType, x.DisplayCssClass)).ToList(), CurrentPerson);
            var partialViewAsString = RenderPartialViewToString(ProjectPeoplePartialViewPath, viewData);
            return partialViewAsString;
        }

        public class ProjectOrganizationEqualityComparer : EqualityComparerByProperty<IProjectOrganization>
        {
            public ProjectOrganizationEqualityComparer() : base(x => new {x.Organization.OrganizationID, x.RelationshipType.RelationshipTypeID})
            {
            }
        }

        public class ProjectPersonEqualityComparer : EqualityComparerByProperty<IProjectPerson>
        {
            public ProjectPersonEqualityComparer() : base(x => new {x.Person.PersonID, x.ProjectPersonRelationshipType.ProjectPersonRelationshipTypeID})
            {
            }
        }

        // BootstrapHtmlHelper's alert modal dialog method isn't great at dealing with near-arbitrary HTML like we expect these "Intro Content" strings to be, so we're using the From Url version instead, which seems to work better.

        [AnonymousUnclassifiedFeature]
        public ContentResult KickOffIntroPreview()
        {
            return new ContentResult
            {
                Content = EmailContentPreview(MultiTenantHelpers.GetProjectUpdateConfiguration().ProjectUpdateKickOffIntroContent)
            };
        }

        [AnonymousUnclassifiedFeature]
        public ContentResult ReminderIntroPreview()
        {
            return new ContentResult
            {
                Content = EmailContentPreview(MultiTenantHelpers.GetProjectUpdateConfiguration().ProjectUpdateReminderIntroContent) 
            };
        }

        [AnonymousUnclassifiedFeature]
        public ContentResult CloseOutIntroPreview()
        {
            return new ContentResult
            {
                Content = EmailContentPreview(MultiTenantHelpers.GetProjectUpdateConfiguration().ProjectUpdateCloseOutIntroContent)
            };
        }

        private static string EmailContentPreview(string introContent)
        {
            var tenantAttribute = HttpRequestStorage.DatabaseEntities.SystemAttributes.Single();

            var emailContentPreview = new ProjectUpdateNotificationHelper(
                tenantAttribute.PrimaryContactPerson.Email, introContent, "",
                tenantAttribute.SquareLogoFileResource,
                MultiTenantHelpers.GetToolDisplayName()).GetEmailContentPreview();

            return emailContentPreview;
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public ActionResult Treatments(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;

            var projectUpdateBatch = project.GetLatestNotApprovedUpdateBatch();
            if (projectUpdateBatch == null)
            {
                return RedirectToAction(new SitkaRoute<ProjectController>(x => x.Detail(project)));
            }
            var updateStatus = GetUpdateStatus(projectUpdateBatch);

            return ViewTreatmentUpdates(projectUpdateBatch, updateStatus);
        }

        private ActionResult ViewTreatmentUpdates(ProjectUpdateBatch projectUpdateBatch, UpdateStatus updateStatus)
        {
            var treatmentUpdateGridSpec = new TreatmentUpdateGridSpec(CurrentPerson, projectUpdateBatch);
            var treatmentGridDataUrl = SitkaRoute<TreatmentController>.BuildUrlFromExpression(tc => tc.TreatmentUpdateProjectDetailGridJsonData(projectUpdateBatch));
            var viewData = new TreatmentsViewData(CurrentPerson, projectUpdateBatch, treatmentUpdateGridSpec, treatmentGridDataUrl, updateStatus);

            return RazorView<Treatments, TreatmentsViewData>(viewData);
        }

        [HttpGet]
        [ProjectUpdateCreateEditSubmitFeature]
        public PartialViewResult RefreshTreatments(ProjectPrimaryKey projectPrimaryKey)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            var viewModel = new ConfirmDialogFormViewModel(projectUpdateBatch.ProjectUpdateBatchID);
            return ViewRefreshTreatments(viewModel);
        }

        [HttpPost]
        [ProjectUpdateCreateEditSubmitFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult RefreshTreatments(ProjectPrimaryKey projectPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var project = projectPrimaryKey.EntityObject;
            var projectUpdateBatch = GetLatestNotApprovedProjectUpdateBatchAndThrowIfNoneFound(project);
            projectUpdateBatch.DeleteTreatmentUpdates();
            // finally create a new project update record, refreshing with the current project data at this point in time
            TreatmentUpdate.CreateFromProject(projectUpdateBatch);
            projectUpdateBatch.TickleLastUpdateDate(CurrentPerson);
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewRefreshTreatments(ConfirmDialogFormViewModel viewModel)
        {
            var viewData =
                new ConfirmDialogFormViewData(
                    $"Are you sure you want to refresh the treatments for this {FieldDefinition.Project.GetFieldDefinitionLabel()}? This will pull the most recently approved information for the {FieldDefinition.Project.GetFieldDefinitionLabel()} and any updates made in this section will be lost.");
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }
    }
}
