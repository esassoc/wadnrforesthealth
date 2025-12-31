//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[RecurrenceInterval]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum RecurrenceIntervalEnum {
  OneYear = 1,
  FiveYears = 2,
  TenYears = 3,
  FifteenYears = 4
}

export const RecurrenceIntervals: LookupTableEntry[]  = [
  { Name: "OneYear", DisplayName: "1 Year", Value: 1, SortOrder: 10 },
  { Name: "FiveYears", DisplayName: "5 Years", Value: 2, SortOrder: 20 },
  { Name: "TenYears", DisplayName: "10 Years", Value: 3, SortOrder: 30 },
  { Name: "FifteenYears", DisplayName: "15 Years", Value: 4, SortOrder: 40 }
];
export const RecurrenceIntervalsAsSelectDropdownOptions = RecurrenceIntervals.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
