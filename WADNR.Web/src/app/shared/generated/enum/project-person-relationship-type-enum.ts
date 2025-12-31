//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectPersonRelationshipType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectPersonRelationshipTypeEnum {
  PrimaryContact = 1,
  PrivateLandowner = 2,
  Contractor = 3,
  ServiceForestryRegionalCoordinator = 4
}

export const ProjectPersonRelationshipTypes: LookupTableEntry[]  = [
  { Name: "PrimaryContact", DisplayName: "Primary Contact", Value: 1, SortOrder: 10 },
  { Name: "PrivateLandowner", DisplayName: "Private Landowner", Value: 2, SortOrder: 20 },
  { Name: "Contractor", DisplayName: "Contractor", Value: 3, SortOrder: 30 },
  { Name: "ServiceForestryRegionalCoordinator", DisplayName: "Service Forestry Regional Coordinator", Value: 4, SortOrder: 40 }
];
export const ProjectPersonRelationshipTypesAsSelectDropdownOptions = ProjectPersonRelationshipTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
