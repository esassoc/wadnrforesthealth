//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectLocationSimpleType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectLocationSimpleTypeEnum {
  PointOnMap = 1,
  LatLngInput = 2,
  None = 3
}

export const ProjectLocationSimpleTypes: LookupTableEntry[]  = [
  { Name: "PointOnMap", DisplayName: "PointOnMap", Value: 1, SortOrder: 10 },
  { Name: "LatLngInput", DisplayName: "LatLngInput", Value: 2, SortOrder: 20 },
  { Name: "None", DisplayName: "None", Value: 3, SortOrder: 30 }
];
export const ProjectLocationSimpleTypesAsSelectDropdownOptions = ProjectLocationSimpleTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
