//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[AgreementPersonRole]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum AgreementPersonRoleEnum {
  ContractManager = 1,
  ProjectManager = 2,
  ProjectCoordinator = 3,
  Signer = 4,
  TechnicalContact = 5
}

export const AgreementPersonRoles: LookupTableEntry[]  = [
  { Name: "ContractManager", DisplayName: "Contract Manager", Value: 1, SortOrder: 10 },
  { Name: "ProjectManager", DisplayName: "Project Manager", Value: 2, SortOrder: 20 },
  { Name: "ProjectCoordinator", DisplayName: "Project Coordinator", Value: 3, SortOrder: 30 },
  { Name: "Signer", DisplayName: "Signer", Value: 4, SortOrder: 40 },
  { Name: "TechnicalContact", DisplayName: "Technical Contact", Value: 5, SortOrder: 50 }
];
export const AgreementPersonRolesAsSelectDropdownOptions = AgreementPersonRoles.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
