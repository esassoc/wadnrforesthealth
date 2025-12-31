//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProgramNotificationType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProgramNotificationTypeEnum {
  CompletedProjectsMaintenanceReminder = 1
}

export const ProgramNotificationTypes: LookupTableEntry[]  = [
  { Name: "CompletedProjectsMaintenanceReminder", DisplayName: "Completed Projects Maintenance Reminder", Value: 1, SortOrder: 10 }
];
export const ProgramNotificationTypesAsSelectDropdownOptions = ProgramNotificationTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
