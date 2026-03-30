//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[TreatmentCode]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum TreatmentCodeEnum {
  BR1 = 1,
  BR2 = 2,
  PL1New = 3,
  PL1Revised = 4,
  PL2New = 5,
  PL2Revised = 6,
  PL3New = 7,
  PL3Revised = 8,
  PL4New = 9,
  PL4Revised = 10,
  PL5New = 11,
  PL5Revised = 12,
  PR1 = 13,
  PR2 = 14,
  RX1 = 15,
  SL1 = 16,
  SL2 = 17,
  SL3 = 18,
  SL4 = 19,
  TH1 = 20,
  TH2 = 21,
  TH3 = 22,
  TH4 = 23
}

export const TreatmentCodes: LookupTableEntry[]  = [
  { Name: "BR-1", DisplayName: "BR-1: Brush Control", Value: 1, SortOrder: 10 },
  { Name: "BR-2", DisplayName: "BR-2: Brush Control", Value: 2, SortOrder: 20 },
  { Name: "PL-1-New", DisplayName: "PL-1: New Plan 20-100 acres", Value: 3, SortOrder: 30 },
  { Name: "PL-1-Revised", DisplayName: "PL-1: Revised Plan 20-100 acres", Value: 4, SortOrder: 40 },
  { Name: "PL-2-New", DisplayName: "PL-2: New Plan 101-250 acres", Value: 5, SortOrder: 50 },
  { Name: "PL-2-Revised", DisplayName: "PL-2: Revised Plan 101-250 acres", Value: 6, SortOrder: 60 },
  { Name: "PL-3-New", DisplayName: "PL-3: New Plan 251-500 acres", Value: 7, SortOrder: 70 },
  { Name: "PL-3-Revised", DisplayName: "PL-3: Revised Plan 251-500 acres", Value: 8, SortOrder: 80 },
  { Name: "PL-4-New", DisplayName: "PL-4: New Plan 501-1000 acres", Value: 9, SortOrder: 90 },
  { Name: "PL-4-Revised", DisplayName: "PL-4: Revised Plan 501-1000 acres", Value: 10, SortOrder: 100 },
  { Name: "PL-5-New", DisplayName: "PL-5: New Plan 1001+ acres", Value: 11, SortOrder: 110 },
  { Name: "PL-5-Revised", DisplayName: "PL-5: Revised Plan 1001+ acres", Value: 12, SortOrder: 120 },
  { Name: "PR-1", DisplayName: "PR-1: Pruning", Value: 13, SortOrder: 130 },
  { Name: "PR-2", DisplayName: "PR-2: Pruning", Value: 14, SortOrder: 140 },
  { Name: "RX-1", DisplayName: "RX-1: Prescribed Broadcast Burning", Value: 15, SortOrder: 150 },
  { Name: "SL-1", DisplayName: "SL-1: Slash Disposal", Value: 16, SortOrder: 160 },
  { Name: "SL-2", DisplayName: "SL-2: Slash Disposal", Value: 17, SortOrder: 170 },
  { Name: "SL-3", DisplayName: "SL-3: Slash Disposal", Value: 18, SortOrder: 180 },
  { Name: "SL-4", DisplayName: "SL-4: Slash Disposal", Value: 19, SortOrder: 190 },
  { Name: "TH-1", DisplayName: "TH-1: Thinning", Value: 20, SortOrder: 200 },
  { Name: "TH-2", DisplayName: "TH-2: Thinning", Value: 21, SortOrder: 210 },
  { Name: "TH-3", DisplayName: "TH-3: Thinning", Value: 22, SortOrder: 220 },
  { Name: "TH-4", DisplayName: "TH-4: Thinning", Value: 23, SortOrder: 230 }
];
export const TreatmentCodesAsSelectDropdownOptions = TreatmentCodes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
