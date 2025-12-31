//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[Division]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum DivisionEnum {
  ForestHealth = 1,
  Wildfire = 2
}

export const Divisions: LookupTableEntry[]  = [
  { Name: "ForestHealth", DisplayName: "DNR Headquarters - Forest Health", Value: 1, SortOrder: 10 },
  { Name: "Wildfire", DisplayName: "DNR Headquarters - Wildfire", Value: 2, SortOrder: 20 }
];
export const DivisionsAsSelectDropdownOptions = Divisions.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
