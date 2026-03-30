//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ProjectDocumentType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ProjectDocumentTypeEnum {
  CostShareApplication = 14,
  CostShareSheet = 15,
  TreatmentSpecs = 16,
  Map = 17,
  ApprovalLetter = 18,
  ClaimForm = 19,
  Other = 20,
  ManagementPlan = 21,
  MonitoringReport = 22,
  ProjectScoringMatrix = 23,
  SiteVisitNotes = 24,
  ApprovalChecklist = 25,
  SelfCostStatement = 26
}

export const ProjectDocumentTypes: LookupTableEntry[]  = [
  { Name: "CostShareApplication", DisplayName: "Cost Share Application", Value: 14, SortOrder: 140 },
  { Name: "CostShareSheet", DisplayName: "Cost Share Sheet", Value: 15, SortOrder: 150 },
  { Name: "TreatmentSpecs", DisplayName: "Treatment Specs", Value: 16, SortOrder: 160 },
  { Name: "Map", DisplayName: "Map", Value: 17, SortOrder: 170 },
  { Name: "ApprovalLetter", DisplayName: "Approval Letter", Value: 18, SortOrder: 180 },
  { Name: "ClaimForm", DisplayName: "Claim Form", Value: 19, SortOrder: 190 },
  { Name: "Other", DisplayName: "Other", Value: 20, SortOrder: 200 },
  { Name: "ManagementPlan", DisplayName: "Management Plan", Value: 21, SortOrder: 210 },
  { Name: "MonitoringReport", DisplayName: "Monitoring Report", Value: 22, SortOrder: 220 },
  { Name: "ProjectScoringMatrix", DisplayName: "Project Scoring Matrix", Value: 23, SortOrder: 230 },
  { Name: "SiteVisitNotes", DisplayName: "Site Visit Notes", Value: 24, SortOrder: 240 },
  { Name: "ApprovalChecklist", DisplayName: "Approval Checklist", Value: 25, SortOrder: 250 },
  { Name: "Self-CostStatement", DisplayName: "Self-Cost Statement", Value: 26, SortOrder: 260 }
];
export const ProjectDocumentTypesAsSelectDropdownOptions = ProjectDocumentTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
