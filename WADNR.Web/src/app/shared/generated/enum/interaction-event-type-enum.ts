//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[InteractionEventType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum InteractionEventTypeEnum {
  Complaint = 1,
  FireSafetyPresentation = 2,
  ForestLandownerFieldDay = 3,
  Other = 4,
  Outreach = 5,
  PhoneCall = 6,
  SiteVisit = 7,
  TechnicalAssistance = 8,
  Workshop = 9,
  ResearchMonitoring = 10
}

export const InteractionEventTypes: LookupTableEntry[]  = [
  { Name: "Complaint", DisplayName: "Complaint", Value: 1, SortOrder: 10 },
  { Name: "FireSafetyPresentation", DisplayName: "Fire Safety Presentation", Value: 2, SortOrder: 20 },
  { Name: "ForestLandownerFieldDay", DisplayName: "Forest Landowner Field Day", Value: 3, SortOrder: 30 },
  { Name: "Other", DisplayName: "Other", Value: 4, SortOrder: 40 },
  { Name: "Outreach", DisplayName: "Education and Outreach", Value: 5, SortOrder: 50 },
  { Name: "PhoneCall", DisplayName: "Phone Call", Value: 6, SortOrder: 60 },
  { Name: "SiteVisit", DisplayName: "Site Visit or Field Trip", Value: 7, SortOrder: 70 },
  { Name: "TechnicalAssistance", DisplayName: "Technical Assistance", Value: 8, SortOrder: 80 },
  { Name: "Workshop", DisplayName: "Workshop", Value: 9, SortOrder: 90 },
  { Name: "ResearchMonitoring", DisplayName: "Research and Monitoring", Value: 10, SortOrder: 100 }
];
export const InteractionEventTypesAsSelectDropdownOptions = InteractionEventTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
