//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[NotificationType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum NotificationTypeEnum {
  ProjectUpdateReminder = 1,
  ProjectUpdateSubmitted = 2,
  ProjectUpdateReturned = 3,
  ProjectUpdateApproved = 4,
  Custom = 5,
  ProjectSubmitted = 6,
  ProjectApproved = 7,
  ProjectReturned = 8
}

export const NotificationTypes: LookupTableEntry[]  = [
  { Name: "ProjectUpdateReminder", DisplayName: "Project Update Reminder", Value: 1, SortOrder: 10 },
  { Name: "ProjectUpdateSubmitted", DisplayName: "Project Update Submitted", Value: 2, SortOrder: 20 },
  { Name: "ProjectUpdateReturned", DisplayName: "Project Update Returned", Value: 3, SortOrder: 30 },
  { Name: "ProjectUpdateApproved", DisplayName: "Project Update Approved", Value: 4, SortOrder: 40 },
  { Name: "Custom", DisplayName: "Custom Notification", Value: 5, SortOrder: 50 },
  { Name: "ProjectSubmitted", DisplayName: "Project Submitted", Value: 6, SortOrder: 60 },
  { Name: "ProjectApproved", DisplayName: "Project Approved", Value: 7, SortOrder: 70 },
  { Name: "ProjectReturned", DisplayName: "Project Returned", Value: 8, SortOrder: 80 }
];
export const NotificationTypesAsSelectDropdownOptions = NotificationTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
