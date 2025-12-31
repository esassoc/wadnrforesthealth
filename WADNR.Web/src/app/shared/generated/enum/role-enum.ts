//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[Role]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum RoleEnum {
  Admin = 1,
  Normal = 2,
  Unassigned = 7,
  EsaAdmin = 8,
  ProjectSteward = 9,
  CanEditProgram = 10,
  CanManagePageContent = 11,
  CanViewLandownerInfo = 12,
  CanManageFundSourcesAndAgreements = 13,
  CanAddEditUsersContactsOrganizations = 14
}

export const Roles: LookupTableEntry[]  = [
  { Name: "Admin", DisplayName: "Administrator", Value: 1, SortOrder: 10 },
  { Name: "Normal", DisplayName: "Normal User", Value: 2, SortOrder: 20 },
  { Name: "Unassigned", DisplayName: "Unassigned", Value: 7, SortOrder: 70 },
  { Name: "EsaAdmin", DisplayName: "ESA Administrator", Value: 8, SortOrder: 80 },
  { Name: "ProjectSteward", DisplayName: "Project Steward", Value: 9, SortOrder: 90 },
  { Name: "CanEditProgram", DisplayName: "Can Edit Program", Value: 10, SortOrder: 100 },
  { Name: "CanManagePageContent", DisplayName: "Can Manage Page Content", Value: 11, SortOrder: 110 },
  { Name: "CanViewLandownerInfo", DisplayName: "Can View Landowner Info", Value: 12, SortOrder: 120 },
  { Name: "CanManageFundSourcesAndAgreements", DisplayName: "Can Manage Fund Sources and Agreements", Value: 13, SortOrder: 130 },
  { Name: "CanAddEditUsersContactsOrganizations", DisplayName: "Can Add/Edit Users, Contacts, Organizations", Value: 14, SortOrder: 140 }
];
export const RolesAsSelectDropdownOptions = Roles.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
