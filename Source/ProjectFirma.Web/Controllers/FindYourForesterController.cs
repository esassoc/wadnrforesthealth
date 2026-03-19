using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LtInfo.Common.MvcResults;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.Security;
using ProjectFirma.Web.Views.FindYourForester;

namespace ProjectFirma.Web.Controllers
{
    public class FindYourForesterController : FirmaBaseController
    {
        [FindYourForesterViewFeature]
        public ViewResult Index()
        {
            var layerGeoJsons = new List<LayerGeoJson>();
            var layerVisibility = LayerInitialVisibility.Hide;
            Dictionary<int, string> foresterRoleDefinitionDictionary = new Dictionary<int, string>();
            foreach (var role in ForesterRole.All.OrderBy(x => x.SortOrder))
            {
                if (HttpRequestStorage.DatabaseEntities.ForesterWorkUnits.Any(x => x.ForesterRoleID == role.ForesterRoleID))
                {
                    var tempLayer = new LayerGeoJson(role.ForesterRoleDisplayName, FirmaWebConfiguration.WebMapServiceUrl,
                        FirmaWebConfiguration.GetFindYourForesterWmsLayerName(), "#59ACFF", 0.2m, layerVisibility, $"ForesterRoleID={role.ForesterRoleID}", true);

                    layerGeoJsons.Add(tempLayer);
                }

                var fieldDefinition = role.GetFieldDefinition();
                if (fieldDefinition != null)
                {
                    foresterRoleDefinitionDictionary.Add(role.ForesterRoleID, fieldDefinition.GetFieldDefinitionDescription().ToHtmlString());
                }
                
            }

            var mapInitJson = new MapInitJson("findYourForester", 10, layerGeoJsons, MapInitJson.GetExternalMapLayersForOtherMaps(), BoundingBox.MakeNewDefaultBoundingBox());
            var firmaPage = FirmaPage.GetFirmaPageByPageType(FirmaPageType.FindYourForester);

            var rootQuestions =
                HttpRequestStorage.DatabaseEntities.FindYourForesterQuestions.Where(x => !x.ParentQuestionID.HasValue).ToList();

            var viewData = new FindYourForesterViewData(CurrentPerson, mapInitJson, firmaPage, rootQuestions, foresterRoleDefinitionDictionary);
            return RazorView<FindYourForester, FindYourForesterViewData>(viewData);

        }



        [FindYourForesterManageFeature]
        public ViewResult Manage(int? foresterRoleID)
        {
            var bulkAssignForestersUrl = SitkaRoute<FindYourForesterController>.BuildUrlFromExpression(x => x.BulkAssignForesters(null));
            var layerGeoJsons = new List<LayerGeoJson>();
            var layerVisibility = foresterRoleID.HasValue ? LayerInitialVisibility.Hide : LayerInitialVisibility.Show;
            var initialForesterRoleIdToLoad = 1;
            ForesterRole.CommunityResilienceCoordinator.GetFieldDefinition();
            foreach (var role in ForesterRole.All.OrderBy(x => x.SortOrder))
            {
                if (HttpRequestStorage.DatabaseEntities.ForesterWorkUnits.Any(x => x.ForesterRoleID == role.ForesterRoleID))
                {
                    if (foresterRoleID.HasValue && foresterRoleID.Value == role.ForesterRoleID)
                    {
                        layerVisibility = LayerInitialVisibility.Show;
                    }
                    var tempLayer = new LayerGeoJson(role.ForesterRoleDisplayName, FirmaWebConfiguration.WebMapServiceUrl,
                        FirmaWebConfiguration.GetFindYourForesterWmsLayerName(), "#59ACFF", 0.2m, layerVisibility, $"ForesterRoleID={role.ForesterRoleID}", true, role.ForesterRoleID);
                    if (layerVisibility == LayerInitialVisibility.Show)
                    {
                        layerVisibility = LayerInitialVisibility.Hide;
                        initialForesterRoleIdToLoad = role.ForesterRoleID;
                    }
                    layerGeoJsons.Add(tempLayer);
                }
            }
            

            var mapInitJson = new MapInitJson("manageFindYourForester", 10, layerGeoJsons, MapInitJson.GetExternalMapLayersForOtherMaps(), BoundingBox.MakeNewDefaultBoundingBox());
            var firmaPage = FirmaPage.GetFirmaPageByPageType(FirmaPageType.ManageFindYourForester);
            var viewData = new ManageFindYourForesterViewData(CurrentPerson, mapInitJson, firmaPage, bulkAssignForestersUrl, foresterRoleID ?? initialForesterRoleIdToLoad);
            return RazorView<ManageFindYourForester, ManageFindYourForesterViewData>(viewData);
        }



        [FindYourForesterManageFeature]
        public GridJsonNetJObjectResult<ForesterWorkUnit> ManageFindYourForesterGridJsonData(ForesterRolePrimaryKey foresterRolePrimaryKey)
        {
            if (foresterRolePrimaryKey == null || foresterRolePrimaryKey.EntityObject == null)
            {
                return null;
            }
            var foresterRole = foresterRolePrimaryKey.EntityObject;
            var gridSpec = new ManageFindYourForesterGridSpec(CurrentPerson);
            var findYourForesterGridObjects = HttpRequestStorage.DatabaseEntities.ForesterWorkUnits.Where(x => x.ForesterRoleID == foresterRole.ForesterRoleID).ToList();
            var gridJsonNetJObjectResult = new GridJsonNetJObjectResult<ForesterWorkUnit>(findYourForesterGridObjects, gridSpec);
            return gridJsonNetJObjectResult;
        }

        [HttpGet]
        [FindYourForesterManageFeature]
        public ContentResult BulkAssignForesters()
        {
            return new ContentResult();
        }

        [HttpPost]
        [FindYourForesterManageFeature]
        public PartialViewResult BulkAssignForesters(BulkAssignForestersViewModel viewModel)
        {
            

            var people = HttpRequestStorage.DatabaseEntities.People.GetAllWadnrPeople().OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            var selectedForesterWorkUnits = new List<ForesterWorkUnit>();

            if (viewModel.ForesterWorkUnitIDList != null)
            {
                selectedForesterWorkUnits =
                    HttpRequestStorage.DatabaseEntities.ForesterWorkUnits.Where(x => viewModel.ForesterWorkUnitIDList.Contains(x.ForesterWorkUnitID)).ToList();
                viewModel.ForesterRoleID = selectedForesterWorkUnits.First().ForesterRoleID;

            }
            
            var viewData = new BulkAssignForestersViewData(CurrentPerson, people, selectedForesterWorkUnits);
            return RazorPartialView<BulkAssignForesters, BulkAssignForestersViewData, BulkAssignForestersViewModel>(viewData, viewModel);

        }

        /// <summary>
        /// Dummy get signature so that it can find the post action
        /// </summary>
        [HttpGet]
        [FindYourForesterManageFeature]
        public ContentResult UpdateBulkAssignedForesters()
        {
            return new ContentResult();
        }

        [HttpPost]
        [FindYourForesterManageFeature]
        [AutomaticallyCallEntityFrameworkSaveChangesWhenModelValid]
        public ActionResult UpdateBulkAssignedForesters(BulkAssignForestersViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BulkAssignForesters(viewModel);
            }

            viewModel.UpdateModel();

            if (viewModel.ForesterRoleID.HasValue)
            {
                return new ModalDialogFormJsonResult(SitkaRoute<FindYourForesterController>.BuildUrlFromExpression(tc => tc.Manage(viewModel.ForesterRoleID)));
            }
            else
            {
                return new ModalDialogFormJsonResult();
            }
        }


    }
}