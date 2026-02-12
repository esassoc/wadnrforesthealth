using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using ApprovalUtilities.Utilities;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using GeoJSON.Net.Feature;
using LtInfo.Common;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.GdalOgr;
using LtInfo.Common.Models;
using LtInfo.Common.Mvc;
using LtInfo.Common.MvcResults;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.Security;
using ProjectFirma.Web.Views.Program;
using ProjectFirma.Web.Views.Shared;
using ProjectFirma.Web.Views.Shared.ProjectImportBlockList;
using Detail = ProjectFirma.Web.Views.Program.Detail;
using DetailViewData = ProjectFirma.Web.Views.Program.DetailViewData;
using EditViewData = ProjectFirma.Web.Views.Program.EditViewData;
using EditViewModel = ProjectFirma.Web.Views.Program.EditViewModel;
using Index = ProjectFirma.Web.Views.Program.Index;
using IndexViewData = ProjectFirma.Web.Views.Program.IndexViewData;
using RelationshipType = ProjectFirma.Web.Models.RelationshipType;

namespace ProjectFirma.Web.Controllers
{
    public class ProgramController : FirmaBaseController
    {


        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult DeleteProgramExampleDocument(FileResourcePrimaryKey fileResourcePrimaryKey)
        {
            var viewModel = new ConfirmDialogFormViewModel(fileResourcePrimaryKey.PrimaryKeyValue);
            return ViewDeleteProgramExampleDocument(fileResourcePrimaryKey.EntityObject, viewModel);
        }

        private PartialViewResult ViewDeleteProgramExampleDocument(FileResource fileResource, ConfirmDialogFormViewModel viewModel)
        {
            var confirmMessage = $"Are you sure you want to delete this Program Geospatial Example Document '{fileResource.OriginalCompleteFileName}'?";
            var viewData = new ConfirmDialogFormViewData(confirmMessage, true);
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult DeleteProgramExampleDocument(FileResourcePrimaryKey fileResourcePrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var document = fileResourcePrimaryKey.EntityObject;
            if (!ModelState.IsValid)
            {
                return ViewDeleteProgramExampleDocument(document, viewModel);
            }

            document.ProgramsWhereYouAreTheProgramExampleGeospatialUploadFileResource.ForEach(x => x.ProgramExampleGeospatialUploadFileResource = null);
            HttpRequestStorage.DatabaseEntities.SaveChanges();
            document.Delete(HttpRequestStorage.DatabaseEntities);
            var message = $"Program Geospatial Example Document '{document.OriginalCompleteFileName}' successfully deleted.";
            SetMessageForDisplay(message);
            return new ModalDialogFormJsonResult();
        }


        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult DeleteProgramDocument(FileResourcePrimaryKey fileResourcePrimaryKey)
        {
            var viewModel = new ConfirmDialogFormViewModel(fileResourcePrimaryKey.PrimaryKeyValue);
            return ViewProgramDocument(fileResourcePrimaryKey.EntityObject, viewModel);
        }

        private PartialViewResult ViewProgramDocument(FileResource fileResource, ConfirmDialogFormViewModel viewModel)
        {
            var confirmMessage = $"Are you sure you want to delete this Program Document '{fileResource.OriginalCompleteFileName}'?";
            var viewData = new ConfirmDialogFormViewData(confirmMessage, true);
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult DeleteProgramDocument(FileResourcePrimaryKey fileResourcePrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var document = fileResourcePrimaryKey.EntityObject;
            if (!ModelState.IsValid)
            {
                return ViewProgramDocument(document, viewModel);
            }

            document.ProgramsWhereYouAreTheProgramFileResource.ForEach(x => x.ProgramFileResource = null);
            HttpRequestStorage.DatabaseEntities.SaveChanges();
            document.Delete(HttpRequestStorage.DatabaseEntities);
            var message = $"Program Document '{document.OriginalCompleteFileName}' successfully deleted.";
            SetMessageForDisplay(message);
            return new ModalDialogFormJsonResult();
        }


        [ProgramViewFeature]
        public ViewResult Detail(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var viewData = new DetailViewData(CurrentPerson, program);
            return RazorView<Detail, DetailViewData>(viewData);
        }

        [ProgramViewFeature]
        public ViewResult Index()
        {
            var firmaPage = FirmaPage.GetFirmaPageByPageType(FirmaPageType.ProgramsList);
            var viewData = new IndexViewData(CurrentPerson, firmaPage);
            return RazorView<Index, IndexViewData>(viewData);
        }


        [ProgramViewFeature]
        public GridJsonNetJObjectResult<Program> IndexGridJsonData()
        {
            var gridSpec = new ProgramGridSpec(CurrentPerson, null);
            var programs = HttpRequestStorage.DatabaseEntities.Programs.ToList().OrderBy(x => x.ProgramName).ToList();
            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<Program>(programs, gridSpec);
            return gridJsonNetJObjectResult;
        }

        [ProgramViewFeature]
        public GridJsonNetJObjectResult<Program> ProgramGridJsonData(OrganizationPrimaryKey organizationPrimaryKey)
        {
            var organization = organizationPrimaryKey.EntityObject;
            var gridSpec = new ProgramGridSpec(CurrentPerson, organization);
            var programs = organization.Programs.ToList();
            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<Program>(programs, gridSpec);
            return gridJsonNetJObjectResult;
        }

        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult NewProgram(OrganizationPrimaryKey organizationPrimaryKey)
        {
            var organization = organizationPrimaryKey.EntityObject;
            var viewModel = new Views.Program.EditViewModel { IsActive = true, OrganizationID = organization.OrganizationID };
            return ViewEdit(viewModel, null, organization);
        }


        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult NewProgram(OrganizationPrimaryKey organizationPrimaryKey, Views.Program.EditViewModel viewModel)
        {
            var organization = organizationPrimaryKey.EntityObject;
            if (!ModelState.IsValid)
            {
                return ViewEdit(viewModel, null, organization);
            }

            var program = new Program(organization, true, DateTime.Now, CurrentPerson, false);
            viewModel.UpdateModel(program, CurrentPerson, true);

            var leadImplementerRelationshipType = HttpRequestStorage.DatabaseEntities.RelationshipTypes.Single(x => x.RelationshipTypeID == RelationshipType.LeadImplementerID);
            var gisUploadSourceOrganization = new GisUploadSourceOrganization(program.ProgramName, false,
                ProjectStage.Implementation, false, program.Organization,
                leadImplementerRelationshipType, false, true, true, program, false, false, false);
            gisUploadSourceOrganization.ImportIsFlattened = false;

            HttpRequestStorage.DatabaseEntities.Programs.Add(program);
            HttpRequestStorage.DatabaseEntities.GisUploadSourceOrganizations.Add(gisUploadSourceOrganization);

            HttpRequestStorage.DatabaseEntities.SaveChanges();

            if (viewModel.ProgramFileResourceData != null)
            {

                program.ProgramFileResource = FileResource.CreateNewFromHttpPostedFileAndSave(viewModel.ProgramFileResourceData, CurrentPerson);
            }

            if (viewModel.ProgramExampleFileResourceData != null)
            {

                program.ProgramExampleGeospatialUploadFileResource = FileResource.CreateNewFromHttpPostedFileAndSave(viewModel.ProgramExampleFileResourceData, CurrentPerson);
            }

            SetMessageForDisplay($"{FieldDefinition.Program.GetFieldDefinitionLabel()} {program.DisplayName} successfully created.");
            return new ModalDialogFormJsonResult();
        }


        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult New()
        {
            var viewModel = new Views.Program.EditViewModel { IsActive = true };
            return ViewEdit(viewModel, null, null);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult New(Views.Program.EditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return ViewEdit(viewModel, null, null);
            }

            var organizationID = viewModel.OrganizationID.GetValueOrDefault();

            var organization = HttpRequestStorage.DatabaseEntities.Organizations.Single(x =>
                x.OrganizationID == organizationID);

            var program = new Program(organization, true, DateTime.Now, CurrentPerson, false);
            viewModel.UpdateModel(program, CurrentPerson, true);
            var leadImplementerRelationshipType = HttpRequestStorage.DatabaseEntities.RelationshipTypes.Single(x => x.RelationshipTypeID == RelationshipType.LeadImplementerID);
            var gisUploadSourceOrganization = new GisUploadSourceOrganization(program.ProgramName, false,
                ProjectStage.Implementation, false, program.Organization,
                leadImplementerRelationshipType, false, true, true, program, false, false, false);
            gisUploadSourceOrganization.ImportIsFlattened = false;
            HttpRequestStorage.DatabaseEntities.Programs.Add(program);
            HttpRequestStorage.DatabaseEntities.GisUploadSourceOrganizations.Add(gisUploadSourceOrganization);
            HttpRequestStorage.DatabaseEntities.SaveChanges();
            if (viewModel.ProgramFileResourceData != null)
            {

                program.ProgramFileResource = FileResource.CreateNewFromHttpPostedFileAndSave(viewModel.ProgramFileResourceData, CurrentPerson);
            }

            if (viewModel.ProgramExampleFileResourceData != null)
            {

                program.ProgramExampleGeospatialUploadFileResource = FileResource.CreateNewFromHttpPostedFileAndSave(viewModel.ProgramExampleFileResourceData, CurrentPerson);
            }
            SetMessageForDisplay($"{FieldDefinition.Program.GetFieldDefinitionLabel()} {program.DisplayName} successfully created.");
            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewEdit(Views.Program.EditViewModel viewModel, Person currentPrimaryContactPerson, Organization organization)
        {
            var organizationsAsSelectListItems = HttpRequestStorage.DatabaseEntities.Organizations
                .Where(x => !string.Equals(x.OrganizationName, Organization.OrganizationUnknown))
                .OrderBy(x => x.OrganizationName)
                .ToSelectListWithEmptyFirstRow(x => x.OrganizationID.ToString(CultureInfo.InvariantCulture),
                    x => x.OrganizationName);
            var activePeople = HttpRequestStorage.DatabaseEntities.People.GetActiveWadnrPeople().Where(x => x.IsFullUser()).ToList();
            if (currentPrimaryContactPerson != null && !activePeople.Contains(currentPrimaryContactPerson))
            {
                activePeople.Add(currentPrimaryContactPerson);
            }
            var people = activePeople.OrderBy(x => x.FullNameLastFirst).ToSelectListWithEmptyFirstRow(x => x.PersonID.ToString(CultureInfo.InvariantCulture),
                x => x.FullNameFirstLastAndOrg);
            bool isSitkaAdmin = new SitkaAdminFeature().HasPermissionByPerson(CurrentPerson);
            var viewData = new Views.Program.EditViewData(organizationsAsSelectListItems, people, isSitkaAdmin, organization);
            return RazorPartialView<Views.Program.Edit, Views.Program.EditViewData, Views.Program.EditViewModel>(viewData, viewModel);
        }


        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult Edit(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var viewModel = new EditViewModel(program);
            return ViewEdit(viewModel, program.ProgramPrimaryContactPerson, null);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult Edit(ProgramPrimaryKey programPrimaryKey, EditViewModel viewModel)
        {
            var program = programPrimaryKey.EntityObject;
            if (!ModelState.IsValid)
            {
                return ViewEdit(viewModel, program.ProgramPrimaryContactPerson, null);
            }
            viewModel.UpdateModel(program, CurrentPerson, false);
            if (viewModel.ProgramFileResourceData != null)
            {
                var currentAgreementFileResource = program.ProgramFileResource;
                program.ProgramFileResource = null;
                // Delete old Agreement file, if present
                if (currentAgreementFileResource != null)
                {
                    HttpRequestStorage.DatabaseEntities.SaveChanges();
                    HttpRequestStorage.DatabaseEntities.FileResources.DeleteFileResource(currentAgreementFileResource);
                }
                program.ProgramFileResource = FileResource.CreateNewFromHttpPostedFileAndSave(viewModel.ProgramFileResourceData, CurrentPerson);
            }

            if (viewModel.ProgramExampleFileResourceData != null)
            {
                var currentAgreementFileResource = program.ProgramExampleGeospatialUploadFileResource;
                program.ProgramExampleGeospatialUploadFileResource = null;
                // Delete old Agreement file, if present
                if (currentAgreementFileResource != null)
                {
                    HttpRequestStorage.DatabaseEntities.SaveChanges();
                    HttpRequestStorage.DatabaseEntities.FileResources.DeleteFileResource(currentAgreementFileResource);
                }
                program.ProgramExampleGeospatialUploadFileResource = FileResource.CreateNewFromHttpPostedFileAndSave(viewModel.ProgramExampleFileResourceData, CurrentPerson);
            }
            return new ModalDialogFormJsonResult();
        }

        [HttpGet]
        [ProgramEditMappingsFeature]
        public PartialViewResult EditImportBasics(GisUploadSourceOrganizationPrimaryKey gisUploadSourceOrganizationPrimaryKey)
        {
            var gisUploadSourceOrganization = gisUploadSourceOrganizationPrimaryKey.EntityObject;
            var viewModel = new EditImportBasicsViewModel(gisUploadSourceOrganization);
            return ViewEditImportBasics(viewModel);
        }

        [HttpPost]
        [ProgramEditMappingsFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult EditImportBasics(GisUploadSourceOrganizationPrimaryKey gisUploadSourceOrganizationPrimaryKey, EditImportBasicsViewModel viewModel)
        {
            var gisUploadSourceOrganization = gisUploadSourceOrganizationPrimaryKey.EntityObject;
            if (!ModelState.IsValid)
            {
                return ViewEditImportBasics(viewModel);
            }
            viewModel.UpdateModel(gisUploadSourceOrganization);

            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewEditImportBasics(EditImportBasicsViewModel viewModel)
        {
            var organizationsAsSelectListItems = HttpRequestStorage.DatabaseEntities.Organizations
                .Where(x => !string.Equals(x.OrganizationName, Organization.OrganizationUnknown))
                .OrderBy(x => x.OrganizationName)
                .ToSelectListWithEmptyFirstRow(x => x.OrganizationID.ToString(CultureInfo.InvariantCulture),
                    x => x.DisplayName);

            var projectStages = ProjectStage.All.OrderBy(x => x.SortOrder).ToSelectListWithEmptyFirstRow(x => x.ProjectStageID.ToString(CultureInfo.InvariantCulture),
                x => x.ProjectStageDisplayName);
            var viewData = new EditImportBasicsViewData(organizationsAsSelectListItems, projectStages);
            return RazorPartialView<EditImportBasics, EditImportBasicsViewData, EditImportBasicsViewModel>(viewData, viewModel);
        }


        [HttpGet]
        [ProgramEditMappingsFeature]
        public PartialViewResult EditDefaultMappings(GisUploadSourceOrganizationPrimaryKey gisUploadSourceOrganizationPrimaryKey)
        {
            var gisUploadSourceOrganization = gisUploadSourceOrganizationPrimaryKey.EntityObject;
            var viewModel = new EditDefaultMappingsViewModel(gisUploadSourceOrganization);
            return ViewEditDefaultMappings(viewModel, gisUploadSourceOrganization);
        }

        [HttpPost]
        [ProgramEditMappingsFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult EditDefaultMappings(GisUploadSourceOrganizationPrimaryKey gisUploadSourceOrganizationPrimaryKey, EditDefaultMappingsViewModel viewModel)
        {
            var gisUploadSourceOrganization = gisUploadSourceOrganizationPrimaryKey.EntityObject;
            if (!ModelState.IsValid)
            {
                return ViewEditDefaultMappings(viewModel, gisUploadSourceOrganization);
            }
            viewModel.UpdateModel(gisUploadSourceOrganization);

            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewEditDefaultMappings(EditDefaultMappingsViewModel viewModel, GisUploadSourceOrganization gisUploadSourceOrganization)
        {
            var organizationsAsSelectListItems = HttpRequestStorage.DatabaseEntities.Organizations
                .Where(x => !string.Equals(x.OrganizationName, Organization.OrganizationUnknown))
                .OrderBy(x => x.OrganizationName)
                .ToSelectListWithEmptyFirstRow(x => x.OrganizationID.ToString(CultureInfo.InvariantCulture),
                    x => x.DisplayName);

            var projectStages = ProjectStage.All.OrderBy(x => x.SortOrder).ToSelectListWithEmptyFirstRow(x => x.ProjectStageID.ToString(CultureInfo.InvariantCulture),
                x => x.ProjectStageDisplayName);
            var viewData = new EditDefaultMappingsViewData(organizationsAsSelectListItems, projectStages, gisUploadSourceOrganization);
            return RazorPartialView<EditDefaultMappings, EditDefaultMappingsViewData, EditDefaultMappingsViewModel>(viewData, viewModel);
        }


        [HttpGet]
        [ProgramEditMappingsFeature]
        public PartialViewResult EditCrosswalkValues(GisUploadSourceOrganizationPrimaryKey gisUploadSourceOrganizationPrimaryKey)
        {
            var gisUploadSourceOrganization = gisUploadSourceOrganizationPrimaryKey.EntityObject;
            var viewModel = new EditCrosswalkValuesViewModel(gisUploadSourceOrganization);
            return ViewEditCrosswalkValues(viewModel, gisUploadSourceOrganization);
        }

        [HttpPost]
        [ProgramEditMappingsFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult EditCrosswalkValues(GisUploadSourceOrganizationPrimaryKey gisUploadSourceOrganizationPrimaryKey, EditCrosswalkValuesViewModel viewModel)
        {
            var gisUploadSourceOrganization = gisUploadSourceOrganizationPrimaryKey.EntityObject;
            if (!ModelState.IsValid)
            {
                return ViewEditCrosswalkValues(viewModel, gisUploadSourceOrganization);
            }
            viewModel.UpdateModel(gisUploadSourceOrganization, CurrentPerson);

            return new ModalDialogFormJsonResult();
        }

        private PartialViewResult ViewEditCrosswalkValues(EditCrosswalkValuesViewModel viewModel, GisUploadSourceOrganization gisUploadSourceOrganization)
        {
            var leadImplementerSelectListItems = HttpRequestStorage.DatabaseEntities.Organizations.OrderBy(x => x.OrganizationName)
                .ToSelectList(x => x.OrganizationID.ToString(CultureInfo.InvariantCulture),
                    x => x.OrganizationName);

            var projectStageSelectListItems = ProjectStage.All.ToSelectList(x => x.ProjectStageID.ToString(CultureInfo.InvariantCulture),
                x => x.ProjectStageName);

            var treatmentTypeSelectListItems = TreatmentType.All.ToSelectList(
                x => x.TreatmentTypeID.ToString(CultureInfo.InvariantCulture),
                x => x.TreatmentTypeDisplayName);

            var treatmentDetailedActivityTypeSelectListItems = TreatmentDetailedActivityType.All.ToSelectList(
                x => x.TreatmentDetailedActivityTypeID.ToString(CultureInfo.InvariantCulture),
                x => x.TreatmentDetailedActivityTypeDisplayName);

            var viewData = new EditCrosswalkValuesViewData(leadImplementerSelectListItems, projectStageSelectListItems, treatmentTypeSelectListItems, treatmentDetailedActivityTypeSelectListItems, gisUploadSourceOrganization.ImportIsFlattened ?? false);
            return RazorPartialView<EditCrosswalkValues, EditCrosswalkValuesViewData, EditCrosswalkValuesViewModel>(viewData, viewModel);
        }



        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult DeleteProgram(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var viewModel = new ConfirmDialogFormViewModel(program.ProgramID);
            return ViewDeleteProgram(program, viewModel);
        }

        private PartialViewResult ViewDeleteProgram(Program program, ConfirmDialogFormViewModel viewModel)
        {
            string optionalProjectCountString = program.ProjectPrograms.Any() ? $"will delete {program.ProjectPrograms.Count} Projects and" : string.Empty;
            string projectNumberAndDurationWarning = $"<b>This {optionalProjectCountString} may take several minutes to complete.</b>";
            // For now, only show duration warning when ProjectCount > 0. Request by DAL.
            projectNumberAndDurationWarning = program.ProjectPrograms.Any() ? projectNumberAndDurationWarning : string.Empty;
            var confirmMessage = $"Are you sure you want to delete Program \"{program.ProgramNameDisplay}\" with Parent Organization {program.Organization.DisplayName}? <br/><br/>{projectNumberAndDurationWarning}";
            var viewData = new ConfirmDialogFormViewData(confirmMessage, true);
            return RazorPartialView<ConfirmDialogForm, ConfirmDialogFormViewData, ConfirmDialogFormViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult DeleteProgram(ProgramPrimaryKey programPrimaryKey, ConfirmDialogFormViewModel viewModel)
        {
            var program = programPrimaryKey.EntityObject;

            string programName = program.ProgramNameDisplay;
            string parentOrganizationName = program.Organization.DisplayName;
            string projectsDeletedCountString = program.ProjectPrograms.Any() ? $"{program.ProjectPrograms.Count} Projects deleted." : string.Empty;

            if (!ModelState.IsValid)
            {
                return ViewDeleteProgram(program, viewModel);
            }
            var message = $"Program \"{programName}\" with Parent Organization {parentOrganizationName} successfully deleted. {projectsDeletedCountString}";
            program.DeleteFull(HttpRequestStorage.DatabaseEntities);
            SetMessageForDisplay(message);

            return new ModalDialogFormJsonResult();
        }



        private PartialViewResult ViewEditProgramPeople(EditProgramPeopleViewModel viewModel)
        {
            var activePeople = HttpRequestStorage.DatabaseEntities.People.Include(y => y.PersonRoles).ToList().Where(x => x.IsFullUser() && x.PersonRoles.Any(pr => pr.RoleID == Role.CanEditProgram.RoleID)).ToList();

            var people = activePeople.OrderBy(x => x.FullNameLastFirst).Select(x => new PersonSimple(x)).ToList();

            var viewData = new EditProgramPeopleViewData(people);
            return RazorPartialView<EditProgramPeople, EditProgramPeopleViewData, EditProgramPeopleViewModel>(viewData, viewModel);
        }


        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult EditProgramPeople(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var viewModel = new EditProgramPeopleViewModel(program);
            return ViewEditProgramPeople(viewModel);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult EditProgramPeople(ProgramPrimaryKey programPrimaryKey, EditProgramPeopleViewModel viewModel)
        {

            var program = programPrimaryKey.EntityObject;
            if (!ModelState.IsValid)
            {
                return ViewEditProgramPeople(viewModel);
            }
            viewModel.UpdateModel(program, CurrentPerson);

            return new ModalDialogFormJsonResult();
        }


        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult EditProgramNotificationConfiguration(ProgramNotificationConfigurationPrimaryKey programNotificationConfigurationPrimaryKey)
        {
            var programNotificationConfiguration = programNotificationConfigurationPrimaryKey.EntityObject;
            EditProgramNotificationConfigurationViewModel viewModel = new EditProgramNotificationConfigurationViewModel(programNotificationConfiguration);
            return ViewEditProgramNotificationConfiguration(viewModel);
        }

        private PartialViewResult ViewEditProgramNotificationConfiguration(EditProgramNotificationConfigurationViewModel viewModel)
        {
            var recurrenceIntervals = RecurrenceInterval.All.ToSelectListWithEmptyFirstRow(x => x.RecurrenceIntervalID.ToString(CultureInfo.InvariantCulture),
                    x => x.RecurrenceIntervalDisplayName);

            var programNotificationTypes = ProgramNotificationType.All.ToSelectListWithEmptyFirstRow(x => x.ProgramNotificationTypeID.ToString(CultureInfo.InvariantCulture),
                x => x.ProgramNotificationTypeDisplayName);

            var viewData = new EditProgramNotificationConfigurationViewData(CurrentPerson, recurrenceIntervals, programNotificationTypes);
            return RazorPartialView<EditProgramNotificationConfiguration, EditProgramNotificationConfigurationViewData, EditProgramNotificationConfigurationViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult EditProgramNotificationConfiguration(ProgramNotificationConfigurationPrimaryKey programNotificationConfigurationPrimaryKey, EditProgramNotificationConfigurationViewModel viewModel)
        {
            var programNotificationConfiguration = programNotificationConfigurationPrimaryKey.EntityObject;
            Check.Require(programNotificationConfiguration.ProgramNotificationConfigurationID == viewModel.ProgramNotificationConfigurationID, "URL ProgramNotificationConfigurationID does not match Form ProgramNotificationConfigurationID. Should not happen.");
            if (!ModelState.IsValid)
            {
                return ViewEditProgramNotificationConfiguration(viewModel);
            }

            viewModel.UpdateModel(programNotificationConfiguration);
            SetMessageForDisplay("Program Notification Configuration saved successfully.");

            return new ModalDialogFormJsonResult();
        }

        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult NewProgramNotificationConfiguration(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var viewModel = new EditProgramNotificationConfigurationViewModel(program);
            return ViewEditProgramNotificationConfiguration(viewModel);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult NewProgramNotificationConfiguration(ProgramPrimaryKey programPrimaryKey, EditProgramNotificationConfigurationViewModel viewModel)
        {
            var program = programPrimaryKey.EntityObject;
            Check.Require(program.ProgramID == viewModel.ProgramID, "URL ProgramID does not match Form ProgramID. Should not happen.");
            if (!ModelState.IsValid)
            {
                return ViewEditProgramNotificationConfiguration(viewModel);
            }

            var recurrenceInterval = RecurrenceInterval.All.Single(x => x.RecurrenceIntervalID == viewModel.RecurrenceIntervalID);
            var programNotificationType = ProgramNotificationType.All.Single(x => x.ProgramNotificationTypeID == viewModel.ProgramNotificationTypeID);
            var programNotificationConfiguration = ProgramNotificationConfiguration.CreateNewBlank(program, programNotificationType, recurrenceInterval);
            viewModel.UpdateModel(programNotificationConfiguration);
            SetMessageForDisplay("Program Notification Configuration created successfully.");
            return new ModalDialogFormJsonResult();
        }

        [ProgramViewFeature]
        public GridJsonNetJObjectResult<ProgramNotificationConfiguration> ProgramNotificationGridJsonData(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var gridSpec = new ProgramNotificationGridSpec(CurrentPerson, program);
            var programNotifications = program.ProgramNotificationConfigurations.ToList();
            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<ProgramNotificationConfiguration>(programNotifications, gridSpec);
            return gridJsonNetJObjectResult;
        }

        [ProgramViewFeature]
        public GridJsonNetJObjectResult<Project> ProgramProjectListGridJson(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var programProjectDictionary = HttpRequestStorage.DatabaseEntities.ProjectPrograms.Include(x => x.Program).ToList()
                .GroupBy(x => x.ProjectID).ToDictionary(x => x.Key, x => x.ToList().Select(y => y.Program).ToList());

            var gridSpec = new ProjectListGridSpec(CurrentPerson, program, programProjectDictionary);
            var allProjectIDsUnderProgram = HttpRequestStorage.DatabaseEntities.ProjectPrograms.Where(p => p.ProgramID == program.ProgramID);
            var projects = HttpRequestStorage.DatabaseEntities.Projects
                .Join(allProjectIDsUnderProgram,
                    p => p.ProjectID,
                    ap => ap.ProjectID,
                    (p, ap) => p);

            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<Project>(projects.ToList(), gridSpec);
            return gridJsonNetJObjectResult;
        }

        [ProgramViewFeature]
        public GridJsonNetJObjectResult<ProjectImportBlockList> ProgramProjectBlockListGridJson(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var projectImportBlockList = HttpRequestStorage.DatabaseEntities.ProjectImportBlockLists
                .Where(x => x.ProgramID == program.ProgramID).ToList();

            var gridSpec = new ProjectBlockListGridSpec(CurrentPerson, program);

            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<ProjectImportBlockList>(projectImportBlockList, gridSpec);
            return gridJsonNetJObjectResult;
        }

        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult NewBlockListEntry(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var viewModel = new EditProjectImportBlockListViewModel()
            {
                ProgramID = program.ProgramID,
            };
            var viewData = new EditProjectImportBlockListViewData(CurrentPerson, null);
            return RazorPartialView<EditProjectImportBlockList, EditProjectImportBlockListViewData, EditProjectImportBlockListViewModel>(viewData, viewModel);
        }

        [HttpGet]
        [ProgramManageFeature]
        public PartialViewResult NewBlockListEntryFromProject(ProgramPrimaryKey programPrimaryKey, ProjectPrimaryKey projectPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            var project = projectPrimaryKey.EntityObject;
            var viewModel = new EditProjectImportBlockListViewModel()
            {
                ProgramID = program.ProgramID,
                ProjectID = project.ProjectID,
                ProjectName = project.ProjectName,
                ProjectGisIdentifier = project.ProjectGisIdentifier
            };
            var viewData = new EditProjectImportBlockListViewData(CurrentPerson, project);
            return RazorPartialView<EditProjectImportBlockList, EditProjectImportBlockListViewData, EditProjectImportBlockListViewModel>(viewData, viewModel);
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult NewBlockListEntryFromProject(ProgramPrimaryKey programPrimaryKey, ProjectPrimaryKey projectPrimaryKey, EditProjectImportBlockListViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.ProjectName) && string.IsNullOrEmpty(viewModel.ProjectGisIdentifier))
            {
                var validationMessage = "You must provide Project Name and/or Project GIS Identifier.";
                this.ModelState.AddModelError("Required", validationMessage);
            }

            if (!ModelState.IsValid)
            {
                return NewBlockListEntryFromProject(programPrimaryKey, projectPrimaryKey);
            }

            SaveBlockListEntry(viewModel);

            return new ModalDialogFormJsonResult();
        }

        [HttpPost]
        [ProgramManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult NewBlockListEntry(ProgramPrimaryKey programPrimaryKey, EditProjectImportBlockListViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.ProjectName) && string.IsNullOrEmpty(viewModel.ProjectGisIdentifier))
            {
                var validationMessage = "You must provide Project Name and/or Project GIS Identifier.";
                this.ModelState.AddModelError("Required", validationMessage);
            }

            if (!ModelState.IsValid)
            {
                return NewBlockListEntry(programPrimaryKey);
            }

            SaveBlockListEntry(viewModel);

            return new ModalDialogFormJsonResult();
        }

        private void SaveBlockListEntry(EditProjectImportBlockListViewModel viewModel)
        {
            var blockListEntry = new ProjectImportBlockList(viewModel.ProgramID)
            {
                ProjectImportBlockListID = viewModel.ProjectImportBlockListID,
                ProjectID = viewModel.ProjectID,
                ProjectName = viewModel.ProjectName,
                ProjectGisIdentifier = viewModel.ProjectGisIdentifier,
                Notes = viewModel.Notes,
            };
            HttpRequestStorage.DatabaseEntities.ProjectImportBlockLists.Add(blockListEntry);
            HttpRequestStorage.DatabaseEntities.SaveChanges();
        }


        [HttpGet]
        [ProgramEditorFeature]
        public ActionResult DownloadProjectsAsGdb(ProgramPrimaryKey programPrimaryKey)
        {
            var program = programPrimaryKey.EntityObject;
            

            Check.Assert(program != null);

            var gdbFileName = $"ProjectsInProgram-{program.DisplayName}-{DateTime.Now.ToStringDateYearMonthDay()}.gdb";
            var pathForGdbFileSave = $"C:\\Sitka\\WADNRForestHealth\\ProgramShapeFiles\\{gdbFileName}";
            var pathForZip = $"{pathForGdbFileSave}.zip";

            if (System.IO.File.Exists(pathForZip))
            {
                return File(pathForZip, "application/zip", $"{gdbFileName}.zip");
            }

            var allProjectIDsUnderProgram = HttpRequestStorage.DatabaseEntities.ProjectPrograms.Where(p => p.ProgramID == program.ProgramID);
            var projects = HttpRequestStorage.DatabaseEntities.Projects
                .Join(allProjectIDsUnderProgram,
                    p => p.ProjectID,
                    ap => ap.ProjectID,
                    (p, ap) => p).ToList();
            
            var projectFeatureCollection = new FeatureCollection();

            projects = projects.Where(x => x.ProjectLocationPoint != null).ToList();
            projectFeatureCollection.Features.AddRange(projects.Select(x =>
            {
                var feature = x.MakePointFeatureWithRelevantProperties(x.ProjectLocationPoint, true, true, FieldDefinition.Organization.GetFieldDefinitionLabel(), FieldDefinition.Organization.GetFieldDefinitionLabelPluralized());
                feature.Properties["FeatureColor"] = "#99b3ff";
                return feature;
            }).ToList());


            var disposableJsonTempFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".json");

            using (StreamWriter writer = new StreamWriter(disposableJsonTempFile.FileInfo.FullName))
            {
                writer.Write(projectFeatureCollection.ToGeoJsonString());
            }

            var ogr2OgrCommandLineRunner = new Ogr2OgrCommandLineRunner(FirmaWebConfiguration.Ogr2OgrExecutable,
                Ogr2OgrCommandLineRunner.DefaultCoordinateSystemId,
                FirmaWebConfiguration.HttpRuntimeExecutionTimeout.TotalMilliseconds);

            var args = Ogr2OgrCommandLineRunner.BuildCommandLineArgumentsForGeoJsonToFileGdb(disposableJsonTempFile.FileInfo.FullName,
                4326, pathForGdbFileSave, "ProjectLocationSimple", false,
                "POINT");

            ProcessUtilityResult processUtilityResult = ogr2OgrCommandLineRunner.ExecuteOgr2OgrCommand(args);

            
            ZipFile.CreateFromDirectory(pathForGdbFileSave, pathForZip, CompressionLevel.Optimal, false);

            //var geomType = Ogr2OgrGeomTypes.GetOgr2OgrGeomType(ogr2OgrGeomTypeID);
            //var workStatementElementVersion = WorkStatementElementVersions.GetWorkStatementElementVersion(workStatementElementVersionID);
            //new DownloadWorkStatementElementShapefilesFeature().DemandPermission(TaurusSession, workStatementElementVersion);
            //var viewModel = new DownloadWorkStatementElementShapefilesViewModel(workStatementElementVersionID, TaurusSession.TaurusSessionGuid, true, workStatementElementVersion.WorkStatementElementID);
            //var pathForZip = geomType == Ogr2OgrGeomType.Poly ? viewModel.PolyShapefilePathZip() :
            //    geomType == Ogr2OgrGeomType.Line ? viewModel.LineShapefilePathZip() : viewModel.PointShapefilePathZip();
            //var name = geomType == Ogr2OgrGeomType.Poly ? "wse_polygons.zip" : geomType == Ogr2OgrGeomType.Line ? "wse_lines.zip" : "wse_points.zip";
            //return File(pathForZip, "application/zip", name);

            return File(pathForZip, "application/zip", $"{gdbFileName}.zip");
        }


        


    }
}