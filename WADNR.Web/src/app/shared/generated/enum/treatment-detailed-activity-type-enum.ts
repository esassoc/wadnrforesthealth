//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[TreatmentDetailedActivityType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum TreatmentDetailedActivityTypeEnum {
  Chipping = 1,
  Pruning = 2,
  Thinning = 3,
  Mastication = 4,
  Grazing = 5,
  LopAndScatter = 6,
  BiomassRemoval = 7,
  HandPile = 8,
  BroadcastBurn = 9,
  HandPileBurn = 10,
  MachinePileBurn = 11,
  Slash = 12,
  Other = 13,
  JackpotBurn = 14,
  MachinePile = 15,
  FuelBreak = 16,
  Planting = 17,
  BrushControl = 18,
  Mowing = 19,
  Regen = 20,
  PileBurn = 21
}

export const TreatmentDetailedActivityTypes: LookupTableEntry[]  = [
  { Name: "Chipping", DisplayName: "Chipping", Value: 1, SortOrder: 10 },
  { Name: "Pruning", DisplayName: "Pruning", Value: 2, SortOrder: 20 },
  { Name: "Thinning", DisplayName: "Thinning", Value: 3, SortOrder: 30 },
  { Name: "Mastication", DisplayName: "Mastication", Value: 4, SortOrder: 40 },
  { Name: "Grazing", DisplayName: "Grazing", Value: 5, SortOrder: 50 },
  { Name: "LopAndScatter", DisplayName: "Lop and Scatter", Value: 6, SortOrder: 60 },
  { Name: "BiomassRemoval", DisplayName: "Biomass Removal", Value: 7, SortOrder: 70 },
  { Name: "HandPile", DisplayName: "Hand Pile", Value: 8, SortOrder: 80 },
  { Name: "BroadcastBurn", DisplayName: "Broadcast Burn", Value: 9, SortOrder: 90 },
  { Name: "HandPileBurn", DisplayName: "Hand Pile Burn", Value: 10, SortOrder: 100 },
  { Name: "MachinePileBurn", DisplayName: "Machine Pile Burn", Value: 11, SortOrder: 110 },
  { Name: "Slash", DisplayName: "Slash", Value: 12, SortOrder: 120 },
  { Name: "Other", DisplayName: "Other", Value: 13, SortOrder: 130 },
  { Name: "JackpotBurn", DisplayName: "Jackpot Burn", Value: 14, SortOrder: 140 },
  { Name: "MachinePile", DisplayName: "Machine Pile", Value: 15, SortOrder: 150 },
  { Name: "FuelBreak", DisplayName: "Fuel Break", Value: 16, SortOrder: 160 },
  { Name: "Planting", DisplayName: "Planting", Value: 17, SortOrder: 170 },
  { Name: "BrushControl", DisplayName: "Brush Control", Value: 18, SortOrder: 180 },
  { Name: "Mowing", DisplayName: "Mowing", Value: 19, SortOrder: 190 },
  { Name: "Regen", DisplayName: "Regen", Value: 20, SortOrder: 200 },
  { Name: "PileBurn", DisplayName: "Pile Burn", Value: 21, SortOrder: 210 }
];
export const TreatmentDetailedActivityTypesAsSelectDropdownOptions = TreatmentDetailedActivityTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
