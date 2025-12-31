//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[GisUploadAttemptWorkflowSectionGrouping]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum GisUploadAttemptWorkflowSectionGroupingEnum {
  GeospatialValidation = 1,
  MetadataMapping = 2
}

export const GisUploadAttemptWorkflowSectionGroupings: LookupTableEntry[]  = [
  { Name: "GeospatialValidation", DisplayName: "Geospatial Validation", Value: 1, SortOrder: 10 },
  { Name: "MetadataMapping", DisplayName: "Metadata Mapping", Value: 2, SortOrder: 20 }
];
export const GisUploadAttemptWorkflowSectionGroupingsAsSelectDropdownOptions = GisUploadAttemptWorkflowSectionGroupings.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
