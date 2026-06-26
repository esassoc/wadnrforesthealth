# Service Forestry Tabular Import — Field Mappings

This document traces how Service Forestry data flows from the uploaded Excel file into
the live domain tables.

The import is a **two-stage pipeline**:

1. **Upload/parse** — the Angular upload (`/service-forestry-upload/import`) parses the
   Excel workbook into the `dbo.ServiceForestryStage` staging table via
   `WADNR.Common/ExcelWorkbookUtilities/ServiceForestryExcelParser.cs`. Each upload wipes
   and repopulates the staging table.
2. **Publish** — `/service-forestry-upload/publish` runs
   `dbo.pImportServiceForestryTabularData`, which maps staged rows into the live domain
   tables (`Project`, `ProjectFundSourceAllocationRequest`, `ProjectCounty`,
   `ProjectPerson`).

---

## Stage 1 — Excel column → `ServiceForestryStage` column

The parser reads the **`query`** worksheet, matches headers by trimmed name (header in
row 1, falling back to row 2), and skips any row with a blank `PROJECT ID`.

Required headers (used to validate the correct file was uploaded):
`PROJECT ID`, `Forester`, `FUND Source`, `DC Status`, `DC Project CODE`.

| Excel header | Staging column | Type handling |
|---|---|---|
| `Title` | `RegionTitle` | text |
| `PROJECT ID` | `ProjectIdentifier` | text (required; row skipped if blank) |
| `Approval Date` | `ApprovalDate` | date (OLE serial or parsed) |
| `County` | `County` | text |
| `Forester` | `Forester` | text |
| `Total Acres` | `TotalAcres` | decimal |
| `Stewardship Plan` | `StewardshipPlan` | bool (yes/no/1/0/true/false) |
| `Percent Match` | `PercentMatch` | decimal |
| `FUND Source` | `FundSource` | text |
| `DC Status` | `DCStatus` | text |
| `DC Allocated Amount` | `DCAllocatedAmount` | decimal ($/commas stripped) |
| `DC Letter Date` | `DCLetterDate` | date |
| `DC Expiration Date` | `DCExpirationDate` | date |
| `DC Treatment {1..6}` | `DCTreatment1..6` | text |
| `DC Cost {1..6}` | `DCCost1..6` | decimal |
| `DC $/ac{1..6}` | `DCCostPerAcre1..6` | decimal |
| `DC Acres Treatment {1..6}` (or no space) | `DCAcresTreatment1..6` | decimal |
| `DC Total Max Amount` | `DCTotalMaxAmount` | decimal |
| `DC Treated Acres` | `DCTreatedAcres` | decimal |
| `DC Contractor?` | `DCContractor` | bool |
| `DC Vendor Name1` | `DCVendorName1` | text |
| `DC Vendor Name 2` | `DCVendorName2` | text |
| `DC Vendor Address 1` | `DCVendorAddress1` | text |
| `DC Vendor Address2` | `DCVendorAddress2` | text |
| `DC SWV Number / Vendor Number` | `DCSwvVendorNumber` | text |
| `DC Invoice Date` | `DCInvoiceDate` | date |
| `DC Program INDEX` | `DCProgramIndex` | text |
| `DC Project CODE` | `DCProjectCode` | text |
| `DC Match Amount` | `DCMatchAmount` | decimal |
| `DC Pay Amount` | `DCPayAmount` | decimal |
| `Item Type` | `ItemType` | text |
| `Path` | `SourcePath` | text |

---

## Stage 2 — `ServiceForestryStage` → domain tables

All work in `dbo.pImportServiceForestryTabularData` is **scoped to the projects present in
the current staging snapshot** and applies only to **Landowner Assistance / Service
Forestry projects (`ProgramID = 3`)**. Scoping is what keeps this proc from clobbering the
separate LOA tabular import, which shares the same program, and makes the proc safe to
re-run (idempotent).

### Join key (how a staged row finds its project)

`ServiceForestryStage.ProjectIdentifier` → **`Project.ProjectGisIdentifier`**
(restricted to projects with a `ProjectProgram` row for ProgramID 3).

### Fund-source resolution (which allocation the money goes to)

The `vServiceForestryStage*` views resolve each staged row to a single
`FundSourceAllocationID`, in priority order:

1. **`FundSource`** → `FundSource.FundSourceNumber` — when that fund source has exactly one
   allocation (an optional 2-character prefix is stripped, mirroring the LOA view).
2. **`DCProgramIndex` + `DCProjectCode`** → `ProgramIndex` / `ProjectCode` →
   `FundSourceAllocationProgramIndexProjectCode`. A program-index-only match is allowed when
   the project code is blank / `---` / `N/A`.
3. **`99C` special cases** → hardcoded allocations (`WAE` → 313, `WAD` → 312, blank → 324),
   mirroring the LOA logic. Same program and NE/SE regions, so the IDs carry over.

### `ProjectFundSourceAllocationRequest` (inserted, `ImportedFromTabularData = 1`)

Aggregated/summed per (Project, resolved FundSourceAllocation):

| Source | Target column |
|---|---|
| `DCMatchAmount` | `MatchAmount` |
| `DCPayAmount` | `PayAmount` |
| `DCMatchAmount + DCPayAmount` | `TotalAmount` |
| resolved allocation | `FundSourceAllocationID` |

Prior `ImportedFromTabularData = 1` rows for the staged projects are deleted first; a
colliding user-entered row (same Project + Allocation) is also replaced.

### `Project` (updated, per project)

| Source (aggregate) | Target column |
|---|---|
| `SUM(DCMatchAmount + DCPayAmount)` | `EstimatedTotalCost` |
| `MAX(DCExpirationDate)` | `ExpirationDate` |
| `MIN(DCLetterDate)` | `PlannedDate` |
| `MIN(ApprovalDate)` | `ApprovalDate` |
| `MAX(ROUND(PercentMatch, 0))` cast to int | `PercentageMatch` |

> **`PercentMatch` assumption:** the proc treats the value as a whole percent
> (e.g. `25.0000` → `25`). If the source ever stores it as a fraction (`0.2500`), the proc
> needs a ×100 adjustment.

### `ProjectCounty` (insert-if-not-exists)

`County` → matched to `County.CountyName` within Washington
(`StateProvince.StateProvinceAbbreviation = 'WA'`). A new `ProjectCounty` row is added when
absent; existing counties are left intact.

### `ProjectPerson` — Primary Contact (**match only, never created**)

`Forester` is parsed as `"First Last"` **or** `"Last, First"` and matched to an existing
**active** `Person`. On a match, a `PrimaryContact` `ProjectPerson` row is upserted
(`CreatedAsPartOfBulkImport = 1`) and any existing different primary contact is replaced.

**If no `Person` matches, the contact is skipped — no `Person` is created.** The staged
forester is a name-only string with no email to dedupe on, so creating records from it
would risk duplicates. (This is the deliberate opposite of the LOA import, which
auto-creates Persons because it dedupes on email.)

---

## Staged columns NOT mapped by this proc

These are parsed into staging (for traceability / future use) but are **not** published by
`dbo.pImportServiceForestryTabularData`:

- **All six treatment blocks** (`DCTreatment` / `DCCost` / `DCCostPerAcre` /
  `DCAcresTreatment` 1–6) — treatments are created by the **ARC GIS import job**, not this
  tabular path.
- **Vendor / invoice fields** — `DCVendorName1/2`, `DCVendorAddress1/2`, `DCSwvVendorNumber`,
  `DCInvoiceDate`, `DCContractor` (out of scope).
- **`TotalAcres`, `StewardshipPlan`, `DCStatus`, `DCAllocatedAmount`, `DCTotalMaxAmount`,
  `DCTreatedAcres`, `ItemType`, `SourcePath`** — staged but with no target field in this proc.

---

## Related source files

- Parser: `WADNR.Common/ExcelWorkbookUtilities/ServiceForestryExcelParser.cs`
- Staging table: `WADNR.Database/dbo/Tables/dbo.ServiceForestryStage.sql`
- Publish proc: `WADNR.Database/dbo/Procs/dbo.pImportServiceForestryTabularData.sql`
- Fund-source views:
  - `WADNR.Database/dbo/Views/dbo.vServiceForestryStageFundSourceAllocation.sql`
  - `WADNR.Database/dbo/Views/dbo.vServiceForestryStageFundSourceAllocationByProgramIndexProjectCode.sql`
  - `WADNR.Database/dbo/Views/dbo.vServiceForestryStageProjectFundSourceAllocation.sql`
- API endpoints / orchestration:
  - `WADNR.API/Controllers/ServiceForestryUploadController.cs`
  - `WADNR.EFModels/Entities/ServiceForestryUpload.StaticHelpers.cs`
