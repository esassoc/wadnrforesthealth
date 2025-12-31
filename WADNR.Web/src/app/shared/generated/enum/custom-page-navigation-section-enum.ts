//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[CustomPageNavigationSection]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum CustomPageNavigationSectionEnum {
  About = 1,
  Projects = 2,
  Financials = 3,
  ProgramInfo = 4
}

export const CustomPageNavigationSections: LookupTableEntry[]  = [
  { Name: "About", DisplayName: "About", Value: 1, SortOrder: 10 },
  { Name: "Projects", DisplayName: "Projects", Value: 2, SortOrder: 20 },
  { Name: "Financials", DisplayName: "Financials", Value: 3, SortOrder: 30 },
  { Name: "ProgramInfo", DisplayName: "ProgramInfo", Value: 4, SortOrder: 40 }
];
export const CustomPageNavigationSectionsAsSelectDropdownOptions = CustomPageNavigationSections.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
