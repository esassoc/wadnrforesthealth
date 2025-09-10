/*-----------------------------------------------------------------------
<copyright file="TreatmentUpdate.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System.Collections.Generic;
using System.Linq;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public partial class TreatmentUpdate : IAuditableEntity, ITreatment
    {
        public string AuditDescriptionString => $"TreatmentUpdateID:{this.TreatmentUpdateID} -  ProjectLocationUpdateID:{this.ProjectLocationUpdateID}  - TreatmentTypeID:{this.TreatmentTypeID} - :TreatmentStartDate: {this.TreatmentStartDate} - TreatmentEndDate: {this.TreatmentEndDate}";

        public static void CreateFromProject(ProjectUpdateBatch projectUpdateBatch)
        {
            var project = projectUpdateBatch.Project;
            projectUpdateBatch.TreatmentUpdates = project.Treatments.Select(t =>
            {
                var projectLocationUpdate = projectUpdateBatch.ProjectLocationUpdates.SingleOrDefault(plu => plu.ProjectLocationUpdateGeometry.SpatialEquals(t.ProjectLocation?.ProjectLocationGeometry) && plu.ProjectLocationUpdateName == t.ProjectLocation?.ProjectLocationName);
                int? projectLocationUpdateID = null;
                if (projectLocationUpdate != null)
                {
                    projectLocationUpdateID = projectLocationUpdate.ProjectLocationUpdateID;
                }
                return new TreatmentUpdate(projectUpdateBatch, t.TreatmentFootprintAcres, t.TreatmentType, t.TreatmentDetailedActivityType)
                {
                    ProjectUpdateBatchID = projectUpdateBatch.ProjectUpdateBatchID,
                    TreatmentStartDate = t.TreatmentStartDate,
                    TreatmentEndDate = t.TreatmentEndDate,
                    TreatmentNotes = t.TreatmentNotes,
                    TreatmentCodeID = t.TreatmentCodeID,
                    TreatmentTreatedAcres = t.TreatmentTreatedAcres,
                    CostPerAcre = t.CostPerAcre,
                    TreatmentTypeImportedText = t.TreatmentTypeImportedText,
                    CreateGisUploadAttemptID = t.CreateGisUploadAttemptID,
                    UpdateGisUploadAttemptID = t.UpdateGisUploadAttemptID,
                    TreatmentDetailedActivityTypeImportedText = t.TreatmentDetailedActivityTypeImportedText,
                    ProgramID = t.ProgramID,
                    ImportedFromGis = t.ImportedFromGis,
                    ProjectLocationUpdateID = projectLocationUpdateID,
                };
            }).ToList();
        }

        public static void CommitChangesToProject(ProjectUpdateBatch projectUpdateBatch, IList<Treatment> allTreatments)
        {
            var project = projectUpdateBatch.Project;
            var currentTreatments = project.Treatments.ToList();
            currentTreatments.ForEach(treatment =>
            {
                allTreatments.Remove(treatment);
            });
            currentTreatments.Clear();

            if (projectUpdateBatch.TreatmentUpdates.Any())
            {
                // Completely rebuild the list
                projectUpdateBatch.TreatmentUpdates.ToList().ForEach(tu =>
                {
                    var projectLocation = project.ProjectLocations
                        .SingleOrDefault(x => x.ProjectLocationGeometry.SpatialEquals(tu.ProjectLocationUpdate.ProjectLocationUpdateGeometry) && x.ProjectLocationName == tu.ProjectLocationUpdate.ProjectLocationUpdateName);
                    var treatment = new Treatment(
                        project.ProjectID,
                        tu.TreatmentStartDate,
                        tu.TreatmentEndDate,
                        tu.TreatmentFootprintAcres,
                        tu.TreatmentNotes,
                        tu.TreatmentTypeID,
                        tu.TreatmentTreatedAcres,
                        tu.TreatmentTypeImportedText,
                        tu.CreateGisUploadAttemptID,
                        tu.UpdateGisUploadAttemptID,
                        tu.TreatmentDetailedActivityTypeID,
                        tu.TreatmentDetailedActivityTypeImportedText,
                        tu.ProgramID,
                        tu.ImportedFromGis,
                        projectLocation,
                        tu.TreatmentCodeID,
                        tu.CostPerAcre
                    );
                    allTreatments.Add(treatment);
                });
            }

        }
    }
}
