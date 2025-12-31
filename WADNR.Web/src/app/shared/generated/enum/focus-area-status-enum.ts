//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FocusAreaStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum FocusAreaStatusEnum {
  Planned = 1,
  InProgress = 2,
  Completed = 3
}

export const FocusAreaStatuses: LookupTableEntry[]  = [
  { Name: "Planned", DisplayName: "Planned", Value: 1, SortOrder: 10 },
  { Name: "In Progress", DisplayName: "In Progress", Value: 2, SortOrder: 20 },
  { Name: "Completed", DisplayName: "Completed", Value: 3, SortOrder: 30 }
];
export const FocusAreaStatusesAsSelectDropdownOptions = FocusAreaStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
