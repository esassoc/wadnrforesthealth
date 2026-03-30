//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[JsonImportStatusType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum JsonImportStatusTypeEnum {
  NotYetProcessed = 1,
  ProcessingFailed = 2,
  ProcessingSuceeded = 3,
  ProcessingIndeterminate = 4
}

export const JsonImportStatusTypes: LookupTableEntry[]  = [
  { Name: "NotYetProcessed", DisplayName: "NotYetProcessed", Value: 1, SortOrder: 10 },
  { Name: "ProcessingFailed", DisplayName: "ProcessingFailed", Value: 2, SortOrder: 20 },
  { Name: "ProcessingSuceeded", DisplayName: "ProcessingSuceeded", Value: 3, SortOrder: 30 },
  { Name: "ProcessingIndeterminate", DisplayName: "ProcessingIndeterminate", Value: 4, SortOrder: 40 }
];
export const JsonImportStatusTypesAsSelectDropdownOptions = JsonImportStatusTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
