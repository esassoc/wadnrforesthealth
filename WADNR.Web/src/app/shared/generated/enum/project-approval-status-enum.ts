//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectApprovalStatus]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectApprovalStatusEnum {
  Draft = 1,
  PendingApproval = 2,
  Approved = 3,
  Rejected = 4,
  Returned = 5
}

export const ProjectApprovalStatuses: LookupTableEntry[]  = [
  { Name: "Draft", DisplayName: "Draft", Value: 1, SortOrder: 10 },
  { Name: "PendingApproval", DisplayName: "Pending Approval", Value: 2, SortOrder: 20 },
  { Name: "Approved", DisplayName: "Approved and Archived", Value: 3, SortOrder: 30 },
  { Name: "Rejected", DisplayName: "Rejected", Value: 4, SortOrder: 40 },
  { Name: "Returned", DisplayName: "Returned", Value: 5, SortOrder: 50 }
];
export const ProjectApprovalStatusesAsSelectDropdownOptions = ProjectApprovalStatuses.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
