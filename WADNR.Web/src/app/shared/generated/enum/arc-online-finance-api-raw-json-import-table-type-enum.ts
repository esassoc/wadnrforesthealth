//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[ArcOnlineFinanceApiRawJsonImportTableType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum ArcOnlineFinanceApiRawJsonImportTableTypeEnum {
  Vendor = 1,
  ProgramIndex = 2,
  ProjectCode = 3,
  FundSourceExpenditure = 4
}

export const ArcOnlineFinanceApiRawJsonImportTableTypes: LookupTableEntry[]  = [
  { Name: "Vendor", DisplayName: "Vendor", Value: 1, SortOrder: 10 },
  { Name: "ProgramIndex", DisplayName: "ProgramIndex", Value: 2, SortOrder: 20 },
  { Name: "ProjectCode", DisplayName: "ProjectCode", Value: 3, SortOrder: 30 },
  { Name: "FundSourceExpenditure", DisplayName: "FundSourceExpenditure", Value: 4, SortOrder: 40 }
];
export const ArcOnlineFinanceApiRawJsonImportTableTypesAsSelectDropdownOptions = ArcOnlineFinanceApiRawJsonImportTableTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
