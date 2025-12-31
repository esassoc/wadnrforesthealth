//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[AuditLogEventType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum AuditLogEventTypeEnum {
  Added = 1,
  Deleted = 2,
  Modified = 3
}

export const AuditLogEventTypes: LookupTableEntry[]  = [
  { Name: "Added", DisplayName: "Added", Value: 1, SortOrder: 10 },
  { Name: "Deleted", DisplayName: "Deleted", Value: 2, SortOrder: 20 },
  { Name: "Modified", DisplayName: "Modified", Value: 3, SortOrder: 30 }
];
export const AuditLogEventTypesAsSelectDropdownOptions = AuditLogEventTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
