//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Source Table: [dbo].[FileResourceMimeType]

import { LookupTableEntry } from "src/app/shared/models/lookup-table-entry";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component"

export enum FileResourceMimeTypeEnum {
  PDF = 1,
  WordDOCX = 2,
  ExcelXLSX = 3,
  XPNG = 4,
  PNG = 5,
  TIFF = 6,
  BMP = 7,
  GIF = 8,
  JPEG = 9,
  PJPEG = 10,
  PowerpointPPTX = 11,
  PowerpointPPT = 12,
  ExcelXLS = 13,
  WordDOC = 14,
  xExcelXLSX = 15,
  CSS = 16,
  XZIP = 17,
  GZIP = 18,
  XGZIP = 19,
  TGZ = 20,
  TAR = 21,
  ZIP = 22,
  TXT = 23
}

export const FileResourceMimeTypes: LookupTableEntry[]  = [
  { Name: "PDF", DisplayName: "PDF", Value: 1, SortOrder: 10 },
  { Name: "Word (DOCX)", DisplayName: "Word (DOCX)", Value: 2, SortOrder: 20 },
  { Name: "Excel (XLSX)", DisplayName: "Excel (XLSX)", Value: 3, SortOrder: 30 },
  { Name: "X-PNG", DisplayName: "X-PNG", Value: 4, SortOrder: 40 },
  { Name: "PNG", DisplayName: "PNG", Value: 5, SortOrder: 50 },
  { Name: "TIFF", DisplayName: "TIFF", Value: 6, SortOrder: 60 },
  { Name: "BMP", DisplayName: "BMP", Value: 7, SortOrder: 70 },
  { Name: "GIF", DisplayName: "GIF", Value: 8, SortOrder: 80 },
  { Name: "JPEG", DisplayName: "JPEG", Value: 9, SortOrder: 90 },
  { Name: "PJPEG", DisplayName: "PJPEG", Value: 10, SortOrder: 100 },
  { Name: "Powerpoint (PPTX)", DisplayName: "Powerpoint (PPTX)", Value: 11, SortOrder: 110 },
  { Name: "Powerpoint (PPT)", DisplayName: "Powerpoint (PPT)", Value: 12, SortOrder: 120 },
  { Name: "Excel (XLS)", DisplayName: "Excel (XLS)", Value: 13, SortOrder: 130 },
  { Name: "Word (DOC)", DisplayName: "Word (DOC)", Value: 14, SortOrder: 140 },
  { Name: "x-Excel (XLSX)", DisplayName: "x-Excel (XLSX)", Value: 15, SortOrder: 150 },
  { Name: "CSS", DisplayName: "CSS", Value: 16, SortOrder: 160 },
  { Name: "X-ZIP", DisplayName: "X-ZIP", Value: 17, SortOrder: 170 },
  { Name: "GZIP", DisplayName: "GZIP", Value: 18, SortOrder: 180 },
  { Name: "X-GZIP", DisplayName: "X-GZIP", Value: 19, SortOrder: 190 },
  { Name: "TGZ", DisplayName: "TGZ", Value: 20, SortOrder: 200 },
  { Name: "TAR", DisplayName: "TAR", Value: 21, SortOrder: 210 },
  { Name: "ZIP", DisplayName: "ZIP", Value: 22, SortOrder: 220 },
  { Name: "TXT", DisplayName: "TXT", Value: 23, SortOrder: 230 }
];
export const FileResourceMimeTypesAsSelectDropdownOptions = FileResourceMimeTypes.map((x) => ({ Value: x.Value, Label: x.DisplayName, SortOrder: x.SortOrder } as SelectDropdownOption));
