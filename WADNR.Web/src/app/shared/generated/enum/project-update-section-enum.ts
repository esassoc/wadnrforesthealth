//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectUpdateSection]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectUpdateSectionEnum {
  Basics = 2,
  LocationSimple = 3,
  LocationDetailed = 4,
  ExpectedFunding = 7,
  Photos = 9,
  ExternalLinks = 10,
  NotesAndDocuments = 11,
  Organizations = 12,
  Contacts = 13,
  DNRUplandRegions = 14,
  PriorityLandscapes = 15,
  Treatments = 17,
  Counties = 18
}

export const ProjectUpdateSections: LookupTableEntry[]  = [
  { Name: "Basics", DisplayName: "Basics", Value: 2, SortOrder: 20 },
  { Name: "LocationSimple", DisplayName: "Location - Simple", Value: 3, SortOrder: 30 },
  { Name: "LocationDetailed", DisplayName: "Location - Detailed", Value: 4, SortOrder: 40 },
  { Name: "ExpectedFunding", DisplayName: "Expected Funding", Value: 7, SortOrder: 70 },
  { Name: "Photos", DisplayName: "Photos", Value: 9, SortOrder: 90 },
  { Name: "ExternalLinks", DisplayName: "External Links", Value: 10, SortOrder: 100 },
  { Name: "NotesAndDocuments", DisplayName: "Documents and Notes", Value: 11, SortOrder: 110 },
  { Name: "Organizations", DisplayName: "Organizations", Value: 12, SortOrder: 120 },
  { Name: "Contacts", DisplayName: "Contacts", Value: 13, SortOrder: 130 },
  { Name: "DNRUplandRegions", DisplayName: "DNR Upland Regions", Value: 14, SortOrder: 140 },
  { Name: "PriorityLandscapes", DisplayName: "Priority Landscapes", Value: 15, SortOrder: 150 },
  { Name: "Treatments", DisplayName: "Treatments", Value: 17, SortOrder: 170 },
  { Name: "Counties", DisplayName: "Counties", Value: 18, SortOrder: 180 }
];
export const ProjectUpdateSectionsAsSelectDropdownOptions = ProjectUpdateSections.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
