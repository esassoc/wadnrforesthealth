//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[InvoiceStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum InvoiceStatusEnum {
  Pending = 1,
  Paid = 2,
  Canceled = 3
}

export const InvoiceStatuses: LookupTableEntry[]  = [
  { Name: "Pending", DisplayName: "Pending", Value: 1, SortOrder: 10 },
  { Name: "Paid", DisplayName: "Paid", Value: 2, SortOrder: 20 },
  { Name: "Canceled", DisplayName: "Canceled", Value: 3, SortOrder: 30 }
];
export const InvoiceStatusesAsSelectDropdownOptions = InvoiceStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
