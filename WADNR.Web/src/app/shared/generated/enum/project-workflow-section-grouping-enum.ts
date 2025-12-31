//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectWorkflowSectionGrouping]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectWorkflowSectionGroupingEnum {
  Overview = 1,
  Location = 2,
  Expenditures = 4,
  AdditionalData = 5,
  ProjectSetup = 6
}

export const ProjectWorkflowSectionGroupings: LookupTableEntry[]  = [
  { Name: "Overview", DisplayName: "Overview", Value: 1, SortOrder: 10 },
  { Name: "Location", DisplayName: "Location", Value: 2, SortOrder: 20 },
  { Name: "Expenditures", DisplayName: "Expenditures", Value: 4, SortOrder: 40 },
  { Name: "AdditionalData", DisplayName: "Additional Data", Value: 5, SortOrder: 50 },
  { Name: "ProjectSetup", DisplayName: "Project Setup", Value: 6, SortOrder: 60 }
];
export const ProjectWorkflowSectionGroupingsAsSelectDropdownOptions = ProjectWorkflowSectionGroupings.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
