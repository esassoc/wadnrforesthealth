//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[OrganizationCode]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum OrganizationCodeEnum {
  ForestResilienceDivision = 1,
  NEregion = 2,
  SEregion = 3,
  NWregion = 4,
  SPSregion = 5,
  OLYregion = 6,
  PCregion = 7
}

export const OrganizationCodes: LookupTableEntry[]  = [
  { Name: "Forest Resilience Division", DisplayName: "Forest Resilience Division", Value: 1, SortOrder: 10 },
  { Name: "NE region", DisplayName: "NE region", Value: 2, SortOrder: 20 },
  { Name: "SE region", DisplayName: "SE region", Value: 3, SortOrder: 30 },
  { Name: "NW region", DisplayName: "NW region", Value: 4, SortOrder: 40 },
  { Name: "SPS region", DisplayName: "SPS region", Value: 5, SortOrder: 50 },
  { Name: "OLY region", DisplayName: "OLY region", Value: 6, SortOrder: 60 },
  { Name: "PC region", DisplayName: "PC region", Value: 7, SortOrder: 70 }
];
export const OrganizationCodesAsSelectDropdownOptions = OrganizationCodes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
