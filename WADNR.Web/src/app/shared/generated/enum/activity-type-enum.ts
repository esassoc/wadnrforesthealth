//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ActivityType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ActivityTypeEnum {
  Travel = 1,
  StaffTime = 2,
  Treatment = 3,
  ContractorTime = 4,
  Supplies = 5
}

export const ActivityTypes: LookupTableEntry[]  = [
  { Name: "Travel", DisplayName: "Travel", Value: 1, SortOrder: 10 },
  { Name: "StaffTime", DisplayName: "Staff Time", Value: 2, SortOrder: 20 },
  { Name: "Treatment", DisplayName: "Treatment", Value: 3, SortOrder: 30 },
  { Name: "ContractorTime", DisplayName: "Contractor Time", Value: 4, SortOrder: 40 },
  { Name: "Supplies", DisplayName: "Supplies", Value: 5, SortOrder: 50 }
];
export const ActivityTypesAsSelectDropdownOptions = ActivityTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
