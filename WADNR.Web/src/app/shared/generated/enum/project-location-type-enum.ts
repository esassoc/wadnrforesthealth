//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectLocationType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectLocationTypeEnum {
  ProjectArea = 1,
  TreatmentArea = 2,
  ResearchPlot = 3,
  TestSite = 4,
  Other = 5
}

export const ProjectLocationTypes: LookupTableEntry[]  = [
  { Name: "ProjectArea", DisplayName: "Project Area", Value: 1, SortOrder: 10 },
  { Name: "TreatmentArea", DisplayName: "Treatment Area", Value: 2, SortOrder: 20 },
  { Name: "ResearchPlot", DisplayName: "Research Plot", Value: 3, SortOrder: 30 },
  { Name: "TestSite", DisplayName: "Test Site", Value: 4, SortOrder: 40 },
  { Name: "Other", DisplayName: "Other", Value: 5, SortOrder: 50 }
];
export const ProjectLocationTypesAsSelectDropdownOptions = ProjectLocationTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
