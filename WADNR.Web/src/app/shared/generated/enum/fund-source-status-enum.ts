//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FundSourceStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum FundSourceStatusEnum {
  Active = 1,
  Pending = 2,
  Planned = 3,
  Closeout = 4
}

export const FundSourceStatuses: LookupTableEntry[]  = [
  { Name: "Active", DisplayName: "Active", Value: 1, SortOrder: 10 },
  { Name: "Pending", DisplayName: "Pending", Value: 2, SortOrder: 20 },
  { Name: "Planned", DisplayName: "Planned", Value: 3, SortOrder: 30 },
  { Name: "Closeout", DisplayName: "Closeout", Value: 4, SortOrder: 40 }
];
export const FundSourceStatusesAsSelectDropdownOptions = FundSourceStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
