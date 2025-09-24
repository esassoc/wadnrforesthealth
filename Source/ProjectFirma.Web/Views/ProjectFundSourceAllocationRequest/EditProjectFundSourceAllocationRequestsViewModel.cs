/*-----------------------------------------------------------------------
<copyright file="EditProjectFundSourceAllocationRequestsViewModel.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using LtInfo.Common;
using LtInfo.Common.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Models;

namespace ProjectFirma.Web.Views.ProjectFundSourceAllocationRequest
{
    public class EditProjectFundSourceAllocationRequestsViewModel : FormViewModel, IValidatableObject
    {
        
        [FieldDefinitionDisplay(FieldDefinitionEnum.EstimatedTotalCost)]
        [JsonIgnore]
        [ValidateMoneyInRangeForSqlServer]
        public Money? ProjectEstimatedTotalCost { get; set; }

        [Required]
        public bool ForProject { get; set; }

        [Required]
        public int PrimaryKeyID { get; set; }

        public List<ProjectFundSourceAllocationRequestSimple> ProjectFundSourceAllocationRequests { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.FundingSource)]
        public List<int> FundingSourceIDs { get; set; }

        [FieldDefinitionDisplay(FieldDefinitionEnum.FundingSourceNote)]
        [StringLength(Models.Project.FieldLengths.ProjectFundingSourceNotes)]
        public string ProjectFundingSourceNotes { get; set; }

        //TODO: Get TotalAmount in here, pass to Angular Controller in EditProjectFundSourceAllocationRequests.cshtml

        /// <summary>
        /// Needed by the ModelBinder
        /// </summary>
        public EditProjectFundSourceAllocationRequestsViewModel()
        {
        }

        public EditProjectFundSourceAllocationRequestsViewModel(int primaryKeyID, List<Models.ProjectFundSourceAllocationRequest> projectFundSourceAllocationRequests, 
                                                           bool forProject, 
                                                           Money? projectEstimatedTotalCost,
                                                           string projectFundingSourceNotes,
                                                           List<ProjectFundingSource> projectFundingSources)
        {
            ProjectFundSourceAllocationRequests = projectFundSourceAllocationRequests
                .Select(x => new ProjectFundSourceAllocationRequestSimple(x)).ToList();
            ProjectEstimatedTotalCost = projectEstimatedTotalCost;
            ForProject = forProject;
            ProjectFundingSourceNotes = projectFundingSourceNotes;
            FundingSourceIDs = projectFundingSources.Select(x => x.FundingSourceID).ToList();
            PrimaryKeyID = primaryKeyID;
        }

        public void UpdateModel(List<Models.ProjectFundSourceAllocationRequest> currentProjectFundSourceAllocationRequests,
                                IList<Models.ProjectFundSourceAllocationRequest> allProjectFundSourceAllocationRequests, 
                                Models.Project project,
                                List<ProjectFundingSource> currentProjectFundingSources,
                                IList<ProjectFundingSource> allProjectFundingSources)
        {
            var dateNow = DateTime.Now;
            var projectFundSourceAllocationRequestsModified = new List<Models.ProjectFundSourceAllocationRequest>();
            if (ProjectFundSourceAllocationRequests != null)
            {

                foreach (var projectFundSourceAllocationRequestSimple in ProjectFundSourceAllocationRequests)
                {
                    var existingCurrentOne = currentProjectFundSourceAllocationRequests.SingleOrDefault(x =>
                        x.ProjectID == projectFundSourceAllocationRequestSimple.ProjectID &&
                        x.FundSourceAllocationID == projectFundSourceAllocationRequestSimple.FundSourceAllocationID);
                    if (existingCurrentOne != null)
                    {
                        var projectFundSourceAllocationRequestToAdd =
                            projectFundSourceAllocationRequestSimple.ToProjectFundSourceAllocationRequest(
                                existingCurrentOne.CreateDate, dateNow, existingCurrentOne.ImportedFromTabularData);
                        projectFundSourceAllocationRequestsModified.Add(projectFundSourceAllocationRequestToAdd);
                    }
                    else
                    {
                        var projectFundSourceAllocationRequestToAdd =
                            projectFundSourceAllocationRequestSimple.ToProjectFundSourceAllocationRequest(
                                dateNow, null, false);
                        projectFundSourceAllocationRequestsModified.Add(projectFundSourceAllocationRequestToAdd);
                    }
                }
            }

            if (ForProject) // never null
            {
                if (project == null)
                {
                    throw new InvalidOperationException(
                        $"Project is required to update {Models.FieldDefinition.FundSourceAllocation.GetFieldDefinitionLabel()} Requests for a Project");
                }

                //Update the ProjectFundingSources
                var projectFundingSourcesUpdated = new List<Models.ProjectFundingSource>();
                if (FundingSourceIDs != null && FundingSourceIDs.Any())
                {
                    // Completely rebuild the list
                    projectFundingSourcesUpdated = FundingSourceIDs.Select(x => new ProjectFundingSource(project.ProjectID, x)).ToList();
                }

                currentProjectFundingSources.Merge(projectFundingSourcesUpdated,
                    allProjectFundingSources,
                    (x, y) => x.ProjectID == y.ProjectID && x.FundingSourceID == y.FundingSourceID);

                //Update Project fields
                project.ProjectFundingSourceNotes = ProjectFundingSourceNotes;
                project.EstimatedTotalCost = ProjectEstimatedTotalCost;
            }

            currentProjectFundSourceAllocationRequests.Merge(projectFundSourceAllocationRequestsModified,
                allProjectFundSourceAllocationRequests,
                (x, y) => x.ProjectID == y.ProjectID && x.FundSourceAllocationID == y.FundSourceAllocationID,
                (x, y) =>
                {
                    x.TotalAmount = y.TotalAmount;
                    x.MatchAmount = y.MatchAmount;
                    x.PayAmount = y.PayAmount;
                    x.UpdateDate = y.UpdateDate;
                    x.CreateDate = y.CreateDate;
                    x.ImportedFromTabularData = y.ImportedFromTabularData;
                });
            
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (ForProject)
            {
                var project = HttpRequestStorage.DatabaseEntities.Projects.GetProject(PrimaryKeyID);
                var projectIsLoa = project.CreateGisUploadAttemptID.HasValue &&
                                   project.CreateGisUploadAttempt.GisUploadSourceOrganization
                                       .GisUploadSourceOrganizationName.Contains("LOA",
                                           StringComparison.InvariantCultureIgnoreCase);
                if(projectIsLoa && (ProjectFundSourceAllocationRequests == null || !ProjectFundSourceAllocationRequests.Any()))
                {
                    yield return new ValidationResult("LOA Projects must have at least one Fund Source Allocation");
                }
            }

            if (ProjectFundSourceAllocationRequests == null)
            {
                yield break;
            }

            if (ProjectFundSourceAllocationRequests.GroupBy(x => x.FundSourceAllocationID).Any(x => x.Count() > 1))
            {
                yield return new ValidationResult("Each Fund Source Allocation can only be used once.");
            }

            //foreach (var projectFundSourceAllocationRequest in ProjectFundSourceAllocationRequests)
            //{
            //    if (projectFundSourceAllocationRequest.AreBothValuesZero())
            //    {
            //        var fundSourceAllocation = HttpRequestStorage.DatabaseEntities.FundSourceAllocations.Single(x => x.FundSourceAllocationID == projectFundSourceAllocationRequest.FundSourceAllocationID);
            //        yield return new ValidationResult(
            //            $"Secured Funding and Unsecured Funding cannot both be zero for FundSource Allocation: {fundSourceAllocation.FundSourceAllocationName}. If the amount of secured or unsecured funding is unknown, you can leave the amounts blank.");
            //    }
            //}
        }
    }
}