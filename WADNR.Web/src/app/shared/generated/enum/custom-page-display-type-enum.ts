//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[CustomPageDisplayType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum CustomPageDisplayTypeEnum {
  Disabled = 1,
  Public = 2,
  Protected = 3
}

export const CustomPageDisplayTypes: LookupTableEntry[]  = [
  { Name: "Disabled", DisplayName: "Disabled", Value: 1, SortOrder: 10 },
  { Name: "Public", DisplayName: "Public", Value: 2, SortOrder: 20 },
  { Name: "Protected", DisplayName: "Protected", Value: 3, SortOrder: 30 }
];
export const CustomPageDisplayTypesAsSelectDropdownOptions = CustomPageDisplayTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
