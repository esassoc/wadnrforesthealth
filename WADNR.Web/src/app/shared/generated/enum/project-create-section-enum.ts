//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectCreateSection]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectCreateSectionEnum {
  Basics = 2,
  LocationSimple = 3,
  LocationDetailed = 4,
  ExpectedFunding = 8,
  Classifications = 11,
  Photos = 13,
  NotesAndDocuments = 14,
  Organizations = 15,
  Contacts = 16,
  DNRUplandRegions = 17,
  PriorityLandscapes = 18,
  Treatments = 20,
  Counties = 21
}

export const ProjectCreateSections: LookupTableEntry[]  = [
  { Name: "Basics", DisplayName: "Basics", Value: 2, SortOrder: 20 },
  { Name: "LocationSimple", DisplayName: "Location - Simple", Value: 3, SortOrder: 30 },
  { Name: "LocationDetailed", DisplayName: "Location - Detailed", Value: 4, SortOrder: 40 },
  { Name: "ExpectedFunding", DisplayName: "Expected Funding", Value: 8, SortOrder: 80 },
  { Name: "Classifications", DisplayName: "Classifications", Value: 11, SortOrder: 110 },
  { Name: "Photos", DisplayName: "Photos", Value: 13, SortOrder: 130 },
  { Name: "NotesAndDocuments", DisplayName: "Documents and Notes", Value: 14, SortOrder: 140 },
  { Name: "Organizations", DisplayName: "Organizations", Value: 15, SortOrder: 150 },
  { Name: "Contacts", DisplayName: "Contacts", Value: 16, SortOrder: 160 },
  { Name: "DNRUplandRegions", DisplayName: "DNR Upland Regions", Value: 17, SortOrder: 170 },
  { Name: "PriorityLandscapes", DisplayName: "Priority Landscapes", Value: 18, SortOrder: 180 },
  { Name: "Treatments", DisplayName: "Treatments", Value: 20, SortOrder: 200 },
  { Name: "Counties", DisplayName: "Counties", Value: 21, SortOrder: 210 }
];
export const ProjectCreateSectionsAsSelectDropdownOptions = ProjectCreateSections.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
