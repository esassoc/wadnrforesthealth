//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectStewardshipAreaType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectStewardshipAreaTypeEnum {
  ProjectStewardingOrganizations = 1,
  TaxonomyBranches = 2,
  Regions = 3
}

export const ProjectStewardshipAreaTypes: LookupTableEntry[]  = [
  { Name: "ProjectStewardingOrganizations", DisplayName: "Project Stewarding Organizations", Value: 1, SortOrder: 10 },
  { Name: "TaxonomyBranches", DisplayName: "Taxonomy Branches", Value: 2, SortOrder: 20 },
  { Name: "Regions", DisplayName: "Regions", Value: 3, SortOrder: 30 }
];
export const ProjectStewardshipAreaTypesAsSelectDropdownOptions = ProjectStewardshipAreaTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
