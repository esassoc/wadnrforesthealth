//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FundingSource]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum FundingSourceEnum {
  Federal = 1,
  State = 2,
  Private = 3,
  Other = 4
}

export const FundingSources: LookupTableEntry[]  = [
  { Name: "Federal", DisplayName: "Federal", Value: 1, SortOrder: 10 },
  { Name: "State", DisplayName: "State", Value: 2, SortOrder: 20 },
  { Name: "Private", DisplayName: "Private", Value: 3, SortOrder: 30 },
  { Name: "Other", DisplayName: "Other", Value: 4, SortOrder: 40 }
];
export const FundingSourcesAsSelectDropdownOptions = FundingSources.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
