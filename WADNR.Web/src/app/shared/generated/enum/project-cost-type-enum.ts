//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectCostType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectCostTypeEnum {
  PreliminaryEngineering = 1,
  RightOfWay = 2,
  Construction = 3
}

export const ProjectCostTypes: LookupTableEntry[]  = [
  { Name: "PreliminaryEngineering", DisplayName: "Preliminary Engineering", Value: 1, SortOrder: 10 },
  { Name: "RightOfWay", DisplayName: "Right of Way (aka Land Acquisition)", Value: 2, SortOrder: 20 },
  { Name: "Construction", DisplayName: "Construction", Value: 3, SortOrder: 30 }
];
export const ProjectCostTypesAsSelectDropdownOptions = ProjectCostTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
