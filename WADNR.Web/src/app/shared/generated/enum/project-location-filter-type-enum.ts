//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectLocationFilterType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectLocationFilterTypeEnum {
  TaxonomyTrunk = 1,
  TaxonomyBranch = 2,
  ProjectType = 3,
  Classification = 4,
  ProjectStage = 5,
  LeadImplementer = 6,
  Program = 7
}

export const ProjectLocationFilterTypes: LookupTableEntry[]  = [
  { Name: "TaxonomyTrunk", DisplayName: "Taxonomy Trunk", Value: 1, SortOrder: 10 },
  { Name: "TaxonomyBranch", DisplayName: "Taxonomy Branch", Value: 2, SortOrder: 20 },
  { Name: "ProjectType", DisplayName: "Project Type", Value: 3, SortOrder: 30 },
  { Name: "Classification", DisplayName: "Classification", Value: 4, SortOrder: 40 },
  { Name: "ProjectStage", DisplayName: "Project Stage", Value: 5, SortOrder: 50 },
  { Name: "LeadImplementer", DisplayName: "Lead Implementer", Value: 6, SortOrder: 60 },
  { Name: "Program", DisplayName: "Program", Value: 7, SortOrder: 70 }
];
export const ProjectLocationFilterTypesAsSelectDropdownOptions = ProjectLocationFilterTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
