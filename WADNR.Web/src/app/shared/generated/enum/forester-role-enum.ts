//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ForesterRole]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ForesterRoleEnum {
  ServiceForester = 1,
  ServiceForestrySpecialist = 2,
  ForestPracticesForester = 3,
  StewardshipFishAndWildlifeBiologist = 4,
  UrbanForestryTechnician = 5,
  CommunityResilienceCoordinator = 6,
  RegulationAssistanceForester = 7,
  FamilyForestFishPassageProgram = 8,
  ForestryRiparianEasementProgram = 9,
  RiversAndHabitatOpenSpaceProgramManager = 10,
  ServiceForestryProgramManager = 11,
  UcfStatewideSpecialist = 12,
  SmallForestLandownerOfficeProgramManager = 13,
  ForestRegulationFishAndWildlifeBiologist = 14
}

export const ForesterRoles: LookupTableEntry[]  = [
  { Name: "ServiceForester", DisplayName: "Service Forester", Value: 1, SortOrder: 10 },
  { Name: "ServiceForestrySpecialist", DisplayName: "Service Forestry Specialist", Value: 2, SortOrder: 20 },
  { Name: "ForestPracticesForester", DisplayName: "Forest Practices Forester", Value: 3, SortOrder: 30 },
  { Name: "StewardshipFishAndWildlifeBiologist", DisplayName: "Stewardship Fish & Wildlife Biologist", Value: 4, SortOrder: 40 },
  { Name: "UrbanForestryTechnician", DisplayName: "Urban Forestry Technician", Value: 5, SortOrder: 50 },
  { Name: "CommunityResilienceCoordinator", DisplayName: "Community Resilience Coordinator", Value: 6, SortOrder: 60 },
  { Name: "RegulationAssistanceForester", DisplayName: "Regulation Assistance Forester", Value: 7, SortOrder: 70 },
  { Name: "FamilyForestFishPassageProgram", DisplayName: "Family Forest Fish Passage Program", Value: 8, SortOrder: 80 },
  { Name: "ForestryRiparianEasementProgram", DisplayName: "Forestry Riparian Easement Program", Value: 9, SortOrder: 90 },
  { Name: "RiversAndHabitatOpenSpaceProgramManager", DisplayName: "Rivers and Habitat Open Space Program Manager", Value: 10, SortOrder: 100 },
  { Name: "ServiceForestryProgramManager", DisplayName: "Service Forestry Program Manager", Value: 11, SortOrder: 110 },
  { Name: "UcfStatewideSpecialist", DisplayName: "UCF Statewide Specialist", Value: 12, SortOrder: 120 },
  { Name: "SmallForestLandownerOfficeProgramManager", DisplayName: "Small Forest Landowner Office Program Manager", Value: 13, SortOrder: 130 },
  { Name: "ForestRegulationFishAndWildlifeBiologist", DisplayName: "Forest Regulation Fish & Wildlife Biologist", Value: 14, SortOrder: 140 }
];
export const ForesterRolesAsSelectDropdownOptions = ForesterRoles.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
