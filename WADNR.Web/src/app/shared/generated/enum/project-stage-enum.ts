//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectStage]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectStageEnum {
  Planned = 2,
  Implementation = 3,
  Completed = 4,
  Cancelled = 5
}

export const ProjectStages: LookupTableEntry[]  = [
  { Name: "Planned", DisplayName: "Planned", Value: 2, SortOrder: 20 },
  { Name: "Implementation", DisplayName: "Implementation", Value: 3, SortOrder: 30 },
  { Name: "Completed", DisplayName: "Completed", Value: 4, SortOrder: 40 },
  { Name: "Cancelled", DisplayName: "Cancelled", Value: 5, SortOrder: 50 }
];
export const ProjectStagesAsSelectDropdownOptions = ProjectStages.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
