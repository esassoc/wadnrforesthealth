/*-----------------------------------------------------------------------
<copyright file="ResultsController.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using LtInfo.Common.Mvc;
using LtInfo.Common.MvcResults;
using Newtonsoft.Json.Linq;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;
using ProjectFirma.Web.Security;
using ProjectFirma.Web.Views.FindYourForester;
using ProjectFirma.Web.Views.Map;
using ProjectFirma.Web.Views.Results;
using ProjectFirma.Web.Views.Shared.ProjectLocationControls;
using ProjectFirma.Web.Views.Shared.SortOrder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Linq;

namespace ProjectFirma.Web.Controllers
{
    public class ResultsController : FirmaBaseController
    {
        [ProjectLocationsViewFeature]
        public ViewResult ProjectMap()
        {
            List<int> filterValues;
            ProjectLocationFilterType projectLocationFilterType;
            ProjectColorByType colorByValue;

            var currentPersonCanViewProposals = CurrentPerson.CanViewProposals;
            if (!String.IsNullOrEmpty(Request.QueryString[ProjectMapCustomization.FilterByQueryStringParameter]))
            {
                projectLocationFilterType = ProjectLocationFilterType.ToType(Request
                    .QueryString[ProjectMapCustomization.FilterByQueryStringParameter]
                    .ParseAsEnum<ProjectLocationFilterTypeEnum>());
            }
            else
            {
                projectLocationFilterType = ProjectMapCustomization.DefaultLocationFilterType;
            }

            if (!String.IsNullOrEmpty(Request.QueryString[ProjectMapCustomization.FilterValuesQueryStringParameter]))
            {
                var filterValuesAsString = Request.QueryString[ProjectMapCustomization.FilterValuesQueryStringParameter]
                    .Split(',');
                filterValues = filterValuesAsString.Select(Int32.Parse).ToList();
            }
            else
            {
                filterValues = ProjectMapCustomization.GetDefaultLocationFilterValues(currentPersonCanViewProposals);
            }

            if (!String.IsNullOrEmpty(Request.QueryString[ProjectMapCustomization.ColorByQueryStringParameter]))
            {
                colorByValue = ProjectColorByType.ToType(Request
                    .QueryString[ProjectMapCustomization.ColorByQueryStringParameter]
                    .ParseAsEnum<ProjectColorByTypeEnum>());
            }
            else
            {
                colorByValue = ProjectMapCustomization.DefaultColorByType;
            }

            var firmaPage = FirmaPage.GetFirmaPageByPageType(FirmaPageType.ProjectMap);

            var projectsToShow = ProjectMapCustomization.ProjectsForMap(CurrentPerson);

            var initialCustomization =
                new ProjectMapCustomization(projectLocationFilterType, filterValues, colorByValue);
            var projectLocationsLayerGeoJson =
                new LayerGeoJson($"{FieldDefinition.ProjectLocation.GetFieldDefinitionLabel()}",
                    Project.MappedPointsToGeoJsonFeatureCollection(projectsToShow, true, true), "red", 1,
                    LayerInitialVisibility.Show);
            var projectLocationsMapInitJson = new ProjectLocationsMapInitJson(projectLocationsLayerGeoJson, initialCustomization, "ProjectLocationsMap");

            projectLocationsMapInitJson.Layers.AddRange(HttpRequestStorage.DatabaseEntities.Organizations.GetBoundaryLayerGeoJson());

            projectLocationsMapInitJson.Layers.Add(MapInitJson.GetWashingtonLegislativeDistrictLayer());
            var interactionEventLayer = HttpRequestStorage.DatabaseEntities.InteractionEvents.GetInteractionEventsLayerGeoJson();
            projectLocationsMapInitJson.Layers.Add(interactionEventLayer);
            //projectLocationsMapInitJson.Layers.Add(MapInitJson.GetWashingtonCountyLayer());
            
            

            var projectLocationsMapViewData = new ProjectLocationsMapViewData(projectLocationsMapInitJson.MapDivID, 
                                                                              colorByValue.DisplayName, 
                                                                              MultiTenantHelpers.GetTopLevelTaxonomyTiers(), 
                                                                              currentPersonCanViewProposals);

            
            var projectLocationFilterTypesAndValues = CreateProjectLocationFilterTypesAndValuesDictionary(currentPersonCanViewProposals);
            var projectLocationsUrl = SitkaRoute<ResultsController>.BuildAbsoluteUrlHttpsFromExpression(x => x.ProjectMap());

            var filteredProjectsWithLocationAreasUrl =
                SitkaRoute<ResultsController>.BuildUrlFromExpression(x => x.FilteredProjectsWithLocationAreas(null));

            var projectMapLocationJsons = new List<ProjectMapLocationJson>();
            var filteredProjectList = projectsToShow.Where(x1 => x1.HasProjectLocationPoint).Where(x => x.ProjectStage.ShouldShowOnMap()).ToList();
            projectMapLocationJsons = filteredProjectList.ToList().Select(p => new ProjectMapLocationJson(p)).ToList();

            var viewData = new ProjectMapViewData(CurrentPerson,
                                                  firmaPage,
                                                  projectLocationsMapInitJson,
                                                  projectLocationsMapViewData,
                                                  projectLocationFilterTypesAndValues,
                                                  projectLocationsUrl, 
                                                  filteredProjectsWithLocationAreasUrl, 
                                                  projectMapLocationJsons);
            return RazorView<ProjectMap, ProjectMapViewData>(viewData);
        }

        private static Dictionary<ProjectLocationFilterTypeSimple, IEnumerable<SelectListItem>> CreateProjectLocationFilterTypesAndValuesDictionary(bool showProposals)
        {
            var projectLocationFilterTypesAndValues =
                new Dictionary<ProjectLocationFilterTypeSimple, IEnumerable<SelectListItem>>();

            if (MultiTenantHelpers.IsTaxonomyLevelTrunk())
            {
                var taxonomyTrunksAsSelectListItems = HttpRequestStorage.DatabaseEntities.TaxonomyTrunks.AsEnumerable().ToSelectList(
                                                    x => x.TaxonomyTrunkID.ToString(CultureInfo.InvariantCulture), x => x.DisplayName);
                var taxonomyTrunkFilterSimple = new ProjectLocationFilterTypeSimple(ProjectLocationFilterType.TaxonomyTrunk);
                projectLocationFilterTypesAndValues.Add(taxonomyTrunkFilterSimple, taxonomyTrunksAsSelectListItems);
            }

            if (!MultiTenantHelpers.IsTaxonomyLevelLeaf())
            {
                var taxonomyBranchesAsSelectListItems = HttpRequestStorage.DatabaseEntities.TaxonomyBranches.AsEnumerable().ToSelectList(
                                                        x => x.TaxonomyBranchID.ToString(CultureInfo.InvariantCulture), x => x.DisplayName);
                var taxonomyBranchFilterSimple = new ProjectLocationFilterTypeSimple(ProjectLocationFilterType.TaxonomyBranch);
                projectLocationFilterTypesAndValues.Add(taxonomyBranchFilterSimple, taxonomyBranchesAsSelectListItems);
            }

            var projectTypesAsSelectListItems = HttpRequestStorage.DatabaseEntities.ProjectTypes.AsEnumerable().ToSelectList(
                                                x => x.ProjectTypeID.ToString(CultureInfo.InvariantCulture), x => x.DisplayName);
            var projectTypeFilterSimple = new ProjectLocationFilterTypeSimple(ProjectLocationFilterType.ProjectType);
            projectLocationFilterTypesAndValues.Add(projectTypeFilterSimple, projectTypesAsSelectListItems);

            var classificationsAsSelectList = MultiTenantHelpers.GetClassificationSystems().SelectMany(x => x.Classifications).ToSelectList(x => x.ClassificationID.ToString(CultureInfo.InvariantCulture), x => MultiTenantHelpers.GetClassificationSystems().Count > 1 ? $"{x.ClassificationSystem.ClassificationSystemName} - {x.DisplayName}" : x.DisplayName);
            var classificationFilterSimple = new ProjectLocationFilterTypeSimple(ProjectLocationFilterType.Classification, string.Join(" & ", MultiTenantHelpers.GetClassificationSystems().Select(x => x.ClassificationSystemName).ToList()));
            projectLocationFilterTypesAndValues.Add(classificationFilterSimple, classificationsAsSelectList);

            var projectStagesAsSelectListItems = ProjectMapCustomization.GetProjectStagesForMap().ToSelectList(x => x.ProjectStageID.ToString(CultureInfo.InvariantCulture), x => x.ProjectStageDisplayName);
            var projectStageFilterSimple = new ProjectLocationFilterTypeSimple(ProjectLocationFilterType.ProjectStage);
            projectLocationFilterTypesAndValues.Add(projectStageFilterSimple, projectStagesAsSelectListItems);

            var programsAsSelectListItems = HttpRequestStorage.DatabaseEntities.Programs.AsEnumerable().ToSelectList(x => x.ProgramID.ToString(CultureInfo.InvariantCulture), y => y.DisplayName);
            var programFilterSimple = new ProjectLocationFilterTypeSimple(ProjectLocationFilterType.Program);
            projectLocationFilterTypesAndValues.Add(programFilterSimple, programsAsSelectListItems);

            var leadImplementorsAsSelectListItems = HttpRequestStorage.DatabaseEntities.Organizations.Where(o => o.ProjectOrganizations.Any(po => po.RelationshipTypeID == RelationshipType.LeadImplementerID)).AsEnumerable().ToSelectList(x => x.OrganizationID.ToString(CultureInfo.InvariantCulture), y => y.DisplayName);
            var leadImplementerFilterSimple = new ProjectLocationFilterTypeSimple(ProjectLocationFilterType.LeadImplementer);
            projectLocationFilterTypesAndValues.Add(leadImplementerFilterSimple, leadImplementorsAsSelectListItems);

            return projectLocationFilterTypesAndValues;
        }

        [ProjectLocationsViewFeature]
        [HttpGet]
        public ContentResult FilteredProjectsWithLocationAreas()
        {
            return new ContentResult();
        }

        [ProjectLocationsViewFeature]
        [HttpPost]
        public JsonNetJArrayResult FilteredProjectsWithLocationAreas(ProjectMapCustomization projectMapCustomization)
        {
            if (projectMapCustomization.FilterPropertyValues == null ||
                !projectMapCustomization.FilterPropertyValues.Any())
            {
                return new JsonNetJArrayResult(new List<object>());
            }
            var projectLocationFilterTypeFromFilterPropertyName = projectMapCustomization
                .GetProjectLocationFilterTypeFromFilterPropertyName();
            var filterFunction =
                projectLocationFilterTypeFromFilterPropertyName.GetFilterFunction(projectMapCustomization
                    .FilterPropertyValues);
            var allProjectsForMap = ProjectMapCustomization.ProjectsForMap(CurrentPerson);
            var filteredProjects = allProjectsForMap.Where(filterFunction.Compile())
                .ToList();

            var filteredProjectsWithLocationAreas = filteredProjects.Where(x => !x.HasProjectLocationPoint).ToList();

            var taxonomyLevel = MultiTenantHelpers.GetTaxonomyLevel();
            var taxonomyTiersAsFancyTreeNodes = taxonomyLevel
                .GetTaxonomyTiers().SortByOrderThenName().Select(x => x.ToFancyTreeNode(CurrentPerson))
                .ToList();
            var projectsIDsThatDoNotHaveSimpleLocation = filteredProjectsWithLocationAreas
                .Select(project => project.FancyTreeNodeKey.ToString()).ToList();
            switch (taxonomyLevel.ToEnum)
            {
                case TaxonomyLevelEnum.Leaf:
                    PruneProjectsFromTaxonomyLeaves(taxonomyTiersAsFancyTreeNodes, projectsIDsThatDoNotHaveSimpleLocation);
                    break;
                case TaxonomyLevelEnum.Branch:
                    PruneTaxonomyBranchesWithNoProjects(taxonomyTiersAsFancyTreeNodes, projectsIDsThatDoNotHaveSimpleLocation);
                    break;
                case TaxonomyLevelEnum.Trunk:
                    foreach (var taxonomyTrunkNode in taxonomyTiersAsFancyTreeNodes)
                    {
                        var taxonomyBranchNodes = taxonomyTrunkNode.Children.ToList();
                        PruneTaxonomyBranchesWithNoProjects(taxonomyBranchNodes, projectsIDsThatDoNotHaveSimpleLocation);
                        taxonomyTrunkNode.Children = taxonomyBranchNodes;
                    }
                    taxonomyTiersAsFancyTreeNodes.RemoveAll(x => !x.Children.Any());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new JsonNetJArrayResult(taxonomyTiersAsFancyTreeNodes);
        }

        private static void PruneTaxonomyBranchesWithNoProjects(List<FancyTreeNode> taxonomyBranchNodes,
            List<string> projectsIDsThatDoNotHaveSimpleLocation)
        {
            foreach (var taxonomyBranchNode in taxonomyBranchNodes)
            {
                var projectTypeNodes = taxonomyBranchNode.Children.ToList();
                PruneProjectsFromTaxonomyLeaves(projectTypeNodes, projectsIDsThatDoNotHaveSimpleLocation);
                taxonomyBranchNode.Children = projectTypeNodes;
            }
            taxonomyBranchNodes.RemoveAll(x => !x.Children.Any());

        }

        private static void PruneProjectsFromTaxonomyLeaves(List<FancyTreeNode> projectTypeNodes,
            List<string> projectsIDsThatDoNotHaveSimpleLocation)
        {
            foreach (var projectTypeNode in projectTypeNodes)
            {
                projectTypeNode.Children.RemoveAll(x => !projectsIDsThatDoNotHaveSimpleLocation.Contains(x.Key));
            }

            projectTypeNodes.RemoveAll(x => !x.Children.Any());
        }


        [HttpPost]
        [ProjectLocationsViewFeature]
        public JsonNetJObjectResult GeocodeAddress(string address)
        {

            var url = "https://wamaspublic.watech.wa.gov/resources/web/api/Wamas/v1/Singleline?address=" + address;

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;

                JObject joResponse = JObject.Parse(responseBody);

                //var lat = msg.GeocodedAddress.Location.GcYCoord;
                //var long = msg.GeocodedAddress.Location.GcXCoord;
                // Extract latitude and longitude from the geocode response
                var latitude = joResponse["GeocodedAddress"]?["Location"]?["GcYCoord"]?.Value<double>() ?? 0;
                var longitude = joResponse["GeocodedAddress"]?["Location"]?["GcXCoord"]?.Value<double>() ?? 0;

                if (latitude == 0 || longitude == 0)
                {
                    return new JsonNetJObjectResult(new { ErrorStatus = "Unable to extract coordinates from geocode response." });
                }

                // Create DbGeometry point from latitude and longitude
                var point = System.Data.Entity.Spatial.DbGeometry.FromText($"POINT({longitude} {latitude})", 4326);

                // Call the UDF to get forester work units by point
                var foresterWorkUnits = HttpRequestStorage.DatabaseEntities.GetfGetForesterWorkUnitsByPoints(point).ToList();

                // Map the results to ForesterWorkUnitSimple objects
                var foresterWorkUnitSimples = foresterWorkUnits.Select(f => new ForesterWorkUnitSimple(
                    f.ForesterWorkUnitID,
                    f.ForesterRoleID,
                    f.ForesterRoleDisplayName,
                    f.PersonID,
                    f.FirstName,
                    f.LastName,
                    f.Email,
                    f.Phone,
                    f.ForesterWorkUnitName
                )).ToList();

                // Create the FindYourForesterSimple object
                var findYourForesterSimple = new FindYourForesterSimple(latitude, longitude, foresterWorkUnitSimples);

                return new JsonNetJObjectResult(findYourForesterSimple);
            }
            catch (Exception e)
            {
                Logger.Info("Error geocoding address on Project Map", e);
                return new JsonNetJObjectResult(new { ErrorStatus = "There was an error geocoding the provided address." });
            }
        }


    }
}
