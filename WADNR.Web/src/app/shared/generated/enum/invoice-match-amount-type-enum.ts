//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[InvoiceMatchAmountType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum InvoiceMatchAmountTypeEnum {
  DollarAmount = 1,
  N_A = 2,
  DNR = 3
}

export const InvoiceMatchAmountTypes: LookupTableEntry[]  = [
  { Name: "DollarAmount", DisplayName: "Dollar Amount (enter amount in input below)", Value: 1, SortOrder: 10 },
  { Name: "N/A", DisplayName: "N/A", Value: 2, SortOrder: 20 },
  { Name: "DNR", DisplayName: "DNR", Value: 3, SortOrder: 30 }
];
export const InvoiceMatchAmountTypesAsSelectDropdownOptions = InvoiceMatchAmountTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
