//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[SupportRequestType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum SupportRequestTypeEnum {
  ReportBug = 1,
  HelpWithProjectUpdate = 2,
  ForgotLoginInfo = 3,
  NewOrganizationOrFundSourceAllocation = 4,
  ProvideFeedback = 5,
  RequestOrganizationNameChange = 6,
  Other = 7,
  RequestProjectPrimaryContactChange = 8,
  RequestPermissionToAddProjects = 9
}

export const SupportRequestTypes: LookupTableEntry[]  = [
  { Name: "ReportBug", DisplayName: "Ran into a bug or problem with this system", Value: 1, SortOrder: 7 },
  { Name: "HelpWithProjectUpdate", DisplayName: "Can't figure out how to update my project", Value: 2, SortOrder: 1 },
  { Name: "ForgotLoginInfo", DisplayName: "Can't log in (forgot my username or password, account is locked, etc.)", Value: 3, SortOrder: 2 },
  { Name: "NewOrganizationOrFundSourceAllocation", DisplayName: "Need an Organization or Fund Source Allocation added to the list", Value: 4, SortOrder: 4 },
  { Name: "ProvideFeedback", DisplayName: "Provide Feedback on the site", Value: 5, SortOrder: 6 },
  { Name: "RequestOrganizationNameChange", DisplayName: "Request a change to an Organization's name", Value: 6, SortOrder: 9 },
  { Name: "Other", DisplayName: "Other", Value: 7, SortOrder: 100 },
  { Name: "RequestProjectPrimaryContactChange", DisplayName: "Request a change to a Project's primary contact", Value: 8, SortOrder: 10 },
  { Name: "RequestPermissionToAddProjects", DisplayName: "Request permission to add projects", Value: 9, SortOrder: 11 }
];
export const SupportRequestTypesAsSelectDropdownOptions = SupportRequestTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
