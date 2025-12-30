//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[CostType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum CostTypeEnum {
  IndirectCosts = 1,
  Supplies = 2,
  Personnel = 3,
  Benefits = 4,
  Travel = 5,
  Contractual = 6,
  Agreements = 7,
  Equipment = 8,
  Other = 9
}

export const CostTypes: LookupTableEntry[]  = [
  { Name: "IndirectCosts", DisplayName: "Indirect Costs", Value: 1, SortOrder: 10 },
  { Name: "Supplies", DisplayName: "Supplies", Value: 2, SortOrder: 20 },
  { Name: "Personnel", DisplayName: "Personnel", Value: 3, SortOrder: 30 },
  { Name: "Benefits", DisplayName: "Benefits", Value: 4, SortOrder: 40 },
  { Name: "Travel", DisplayName: "Travel", Value: 5, SortOrder: 50 },
  { Name: "Contractual", DisplayName: "Contractual", Value: 6, SortOrder: 60 },
  { Name: "Agreements", DisplayName: "Agreements", Value: 7, SortOrder: 70 },
  { Name: "Equipment", DisplayName: "Equipment", Value: 8, SortOrder: 80 },
  { Name: "Other", DisplayName: "Other", Value: 9, SortOrder: 90 }
];
export const CostTypesAsSelectDropdownOptions = CostTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
