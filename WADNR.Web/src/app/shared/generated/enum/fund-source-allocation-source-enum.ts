//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FundSourceAllocationSource]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum FundSourceAllocationSourceEnum {
  State = 1,
  StateGFS = 2,
  StateCapital = 3,
  StateOther = 4,
  FederalWSFM = 5,
  FederaNFPWUINonFedWUI = 6,
  FederalCWDG = 7,
  FederalLSR = 8,
  FederalBipartisanInfrastructureLaw = 9,
  FederalInflationReductionAct = 10,
  FederalConsolidatedPaymentFundSource = 11,
  FederalCooperativeAgreements = 12,
  FederalDisasterRelief = 13,
  FederalForestHealthProtection = 14,
  FederalForestLegacy = 15,
  FederalWesternBarkBeetle = 16,
  FederalFEMA = 17,
  FederalBLM = 18,
  FederalOther = 19,
  Private = 20,
  Other = 21
}

export const FundSourceAllocationSources: LookupTableEntry[]  = [
  { Name: "State", DisplayName: "State", Value: 1, SortOrder: 10 },
  { Name: "StateGFS", DisplayName: "State - GFS", Value: 2, SortOrder: 20 },
  { Name: "StateCapital", DisplayName: "State - Capital", Value: 3, SortOrder: 30 },
  { Name: "StateOther", DisplayName: "State - Other", Value: 4, SortOrder: 40 },
  { Name: "FederalWSFM", DisplayName: "Federal - WSFM", Value: 5, SortOrder: 50 },
  { Name: "FederaNFPWUINonFedWUI", DisplayName: "Federal - NFP WUI (Non-fed WUI)", Value: 6, SortOrder: 60 },
  { Name: "FederalCWDG", DisplayName: "Federal - CWDG", Value: 7, SortOrder: 70 },
  { Name: "FederalLSR", DisplayName: "Federal - LSR", Value: 8, SortOrder: 80 },
  { Name: "FederalBipartisanInfrastructureLaw", DisplayName: "Federal - Bipartisan Infrastructure Law", Value: 9, SortOrder: 90 },
  { Name: "FederalInflationReductionAct", DisplayName: "Federal - Inflation Reduction Act", Value: 10, SortOrder: 100 },
  { Name: "FederalConsolidatedPaymentFundSource", DisplayName: "Federal - Consolidated Payment FundSource", Value: 11, SortOrder: 110 },
  { Name: "FederalCooperativeAgreements", DisplayName: "Federal - Cooperative Agreements", Value: 12, SortOrder: 120 },
  { Name: "FederalDisasterRelief", DisplayName: "Federal - Disaster Relief", Value: 13, SortOrder: 130 },
  { Name: "FederalForestHealthProtection", DisplayName: "Federal - Forest Health Protection", Value: 14, SortOrder: 140 },
  { Name: "FederalForestLegacy", DisplayName: "Federal - Forest Legacy", Value: 15, SortOrder: 150 },
  { Name: "FederalWesternBarkBeetle", DisplayName: "Federal - Western Bark Beetle", Value: 16, SortOrder: 160 },
  { Name: "FederalFEMA", DisplayName: "Federal - FEMA", Value: 17, SortOrder: 170 },
  { Name: "FederalBLM", DisplayName: "Federal - BLM", Value: 18, SortOrder: 180 },
  { Name: "FederalOther", DisplayName: "Federal - Other", Value: 19, SortOrder: 190 },
  { Name: "Private", DisplayName: "Private", Value: 20, SortOrder: 200 },
  { Name: "Other", DisplayName: "Other", Value: 21, SortOrder: 210 }
];
export const FundSourceAllocationSourcesAsSelectDropdownOptions = FundSourceAllocationSources.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
