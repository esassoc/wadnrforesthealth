//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectColorByType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectColorByTypeEnum {
  TaxonomyTrunk = 1,
  ProjectStage = 2,
  TaxonomyBranch = 3
}

export const ProjectColorByTypes: LookupTableEntry[]  = [
  { Name: "TaxonomyTrunk", DisplayName: "Taxonomy Trunk", Value: 1, SortOrder: 10 },
  { Name: "ProjectStage", DisplayName: "Stage", Value: 2, SortOrder: 20 },
  { Name: "TaxonomyBranch", DisplayName: "Taxonomy Branch", Value: 3, SortOrder: 30 }
];
export const ProjectColorByTypesAsSelectDropdownOptions = ProjectColorByTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
