/*-----------------------------------------------------------------------
<copyright file="ProjectBasicsViewData.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System.Web;
using LtInfo.Common.Views;
using ProjectFirma.Web.Models;

namespace ProjectFirma.Web.Views.Shared.ProjectControls
{
    public class ProjectBasicsViewData
    {
        public string ProjectStageDisplayName { get; }
        
        public string PlannedDateString { get; }
        
        public string CompletedDateString { get; }
        
        public string LeadImplementerDisplay { get; }
        
        public string ProjectDescription { get; }
        
        public string ExpirationDateString { get; }
        
        public HtmlString FocusAreaDisplayName { get; }
        
        public HtmlString ProjectProgramListDisplayString { get; }
        
        public string PercentageMatchFormatted { get; }
        
        public string ProjectGisIdentifier { get; }
        
        public bool ShowProjectGisIdentifier { get; }
        

        public ProjectBasicsViewData(Models.Project project)
        {
            ProjectStageDisplayName = project.ProjectStage.ProjectStageDisplayName;
            PlannedDateString = project.GetPlannedDate();
            CompletedDateString = project.GetCompletionDateFormatted();
            LeadImplementerDisplay = project.GetLeadImplementer()?.DisplayNameWithoutAbbreviation ?? "None Selected";
            ProjectDescription = project.ProjectDescription.HtmlEncodeWithBreaks();
            ExpirationDateString = project.GetExpirationDateFormatted();
            FocusAreaDisplayName = project.FocusArea.GetDisplayNameAsUrl();
            ProjectProgramListDisplayString = project.ProjectPrograms.ToProgramListDisplay(true);
            PercentageMatchFormatted = project.PercentageMatchFormatted;
            ProjectGisIdentifier = project.ProjectGisIdentifier;
            ShowProjectGisIdentifier = !string.IsNullOrEmpty(project.ProjectGisIdentifier);
        }

        public ProjectBasicsViewData(Models.ProjectUpdateBatch projectUpdateBatch)
        {
            ProjectStageDisplayName = projectUpdateBatch.ProjectUpdate.ProjectStage.ProjectStageDisplayName;
            PlannedDateString = projectUpdateBatch.ProjectUpdate.GetPlannedDate();
            CompletedDateString = projectUpdateBatch.ProjectUpdate.GetCompletionDateFormatted();
            LeadImplementerDisplay = projectUpdateBatch.GetLeadImplementer()?.DisplayNameWithoutAbbreviation ?? "None Selected";
            ProjectDescription = projectUpdateBatch.ProjectUpdate.ProjectDescription.HtmlEncodeWithBreaks();
            ExpirationDateString = projectUpdateBatch.ProjectUpdate.GetExpirationDateFormatted();
            FocusAreaDisplayName = projectUpdateBatch.ProjectUpdate.FocusArea.GetDisplayNameAsUrl();
            ProjectProgramListDisplayString = projectUpdateBatch.ProjectUpdatePrograms.ToProgramListDisplay(true);
            PercentageMatchFormatted = projectUpdateBatch.ProjectUpdate.PercentageMatchFormatted;
            ShowProjectGisIdentifier = false;


        }
    }
}
