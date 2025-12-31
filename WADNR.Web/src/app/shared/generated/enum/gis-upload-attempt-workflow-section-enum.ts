//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[GisUploadAttemptWorkflowSection]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum GisUploadAttemptWorkflowSectionEnum {
  UploadGisFile = 2,
  ValidateFeatures = 3,
  ValidateMetadata = 4,
  ReviewMapping = 6,
  RviewStagedImport = 7
}

export const GisUploadAttemptWorkflowSections: LookupTableEntry[]  = [
  { Name: "UploadGisFile", DisplayName: "Upload GIS File", Value: 2, SortOrder: 20 },
  { Name: "ValidateFeatures", DisplayName: "Validate Features", Value: 3, SortOrder: 30 },
  { Name: "ValidateMetadata", DisplayName: "Validate Metadata", Value: 4, SortOrder: 40 },
  { Name: "ReviewMapping", DisplayName: "Review Mapping", Value: 6, SortOrder: 60 },
  { Name: "RviewStagedImport", DisplayName: "Review Staged Import", Value: 7, SortOrder: 70 }
];
export const GisUploadAttemptWorkflowSectionsAsSelectDropdownOptions = GisUploadAttemptWorkflowSections.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
