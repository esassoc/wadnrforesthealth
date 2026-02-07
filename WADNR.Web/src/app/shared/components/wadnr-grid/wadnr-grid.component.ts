import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AgGridAngular, AgGridModule } from "ag-grid-angular";
import {
    ColDef,
    FilterChangedEvent,
    FirstDataRenderedEvent,
    GetRowIdFunc,
    GridApi,
    GridColumnsChangedEvent,
    GridOptions,
    GridReadyEvent,
    RowDataUpdatedEvent,
    RowSelectionMode,
    RowSelectionOptions,
    SelectionChangedEvent,
    SelectionColumnDef,
    Theme,
    themeBalham,
    iconOverrides,
    ModuleRegistry,
    AllCommunityModule,
    CellValueChangedEvent,
    CellEditingStartedEvent,
    CellEditingStoppedEvent,
} from "ag-grid-community";
import { AgGridHelper } from "src/app/shared/helpers/ag-grid-helper";
import { TooltipComponent } from "src/app/shared/components/ag-grid/tooltip/tooltip.component";
import { FormsModule } from "@angular/forms";
import { CsvDownloadButtonComponent } from "src/app/shared/components/csv-download-button/csv-download-button.component";
import { PaginationControlsComponent } from "src/app/shared/components/ag-grid/pagination-controls/pagination-controls.component";
import { WADNRGridHeaderComponent } from "../wadnr-grid-header/wadnr-grid-header.component";
import { MultiRowSelectionOptions } from "node_modules/ag-grid-community/dist/types/src/entities/gridOptions";
import { FullScreenButtonComponent } from "../full-screen-button/full-screen-button.component";

@Component({
    selector: "wadnr-grid",
    imports: [CommonModule, AgGridModule, FormsModule, CsvDownloadButtonComponent, PaginationControlsComponent, WADNRGridHeaderComponent, FullScreenButtonComponent],
    templateUrl: "./wadnr-grid.component.html",
    styleUrls: ["./wadnr-grid.component.scss"],
})
export class WADNRGridComponent implements OnInit, OnChanges {
    @ViewChild(AgGridAngular) gridref: AgGridAngular;

    // ag grid stuff
    @Output() selectionChanged: EventEmitter<SelectionChangedEvent<any>> = new EventEmitter<SelectionChangedEvent<any>>();
    @Output() filterChanged: EventEmitter<FilterChangedEvent<any>> = new EventEmitter<FilterChangedEvent<any>>();
    @Output() gridReady: EventEmitter<GridReadyEvent> = new EventEmitter<GridReadyEvent>();
    @Output() gridRefReady: EventEmitter<AgGridAngular> = new EventEmitter<AgGridAngular>();
    @Output() firstDataLoaded: EventEmitter<FirstDataRenderedEvent> = new EventEmitter<FirstDataRenderedEvent>();
    @Output() cellEditingStarted: EventEmitter<any> = new EventEmitter<any>();
    @Output() cellEditingStopped: EventEmitter<any> = new EventEmitter<any>();
    @Output() cellValueChanged: EventEmitter<CellValueChangedEvent> = new EventEmitter<CellValueChangedEvent>();

    @Input() rowData: any[];
    @Input() columnDefs: any[];
    @Input() defaultColDef: ColDef = {
        sortable: true,
        filter: true,
        resizable: true,
        tooltipComponent: TooltipComponent,
        tooltipValueGetter: (params) => params.value,
    };

    @Input() pagination: boolean = false;
    @Input() paginationPageSize: number = 100;
    @Input() getRowId: GetRowIdFunc;
    @Input() gridOptions: GridOptions;

    @Input() pinnedTotalsRow?: {
        /** Dot-path field(s) to sum (e.g. "AgreementAmount" or "Organization.OrganizationID"). */
        fields: string[];
        /** Optional label shown in the pinned row. If omitted, no label cell is set. */
        label?: string;
        /** Dot-path field to place the label in (defaults to first column field, if any). */
        labelField?: string;
        /** When true (default), totals reflect the currently filtered rows. */
        filteredOnly?: boolean;
    };

    @Input() rowSelection: RowSelectionOptions;
    // setting default will override passed rowSelectionOptions
    @Input() defaultRowSelection: RowSelectionMode;

    // our stuff
    @Input() width: string = "100%";
    @Input() height: string = "500px";
    @Input() downloadFileName: string = "grid-data";
    @Input() colIDsToExclude: string[] = [];
    @Input() hideDownloadButton: boolean = false;
    @Input() hideFullscreenButton: boolean = false;
    @Input() hideTooltips: boolean = false;
    @Input() hideGlobalFilter: boolean = false;
    @Input() disableGlobalFilter: boolean = false;
    @Input() sizeColumnsToFitGrid: boolean = false;
    @Input() suppressColumnSizing: boolean = false;
    @Input() overrideDefaultGridHeader: boolean = false;
    @Input() unsetHeaderGridActionWidth: boolean = false;
    @Input() showMultiRowSelectActions: boolean = true;
    @Input() gridTitle: string = "";

    private gridApi: GridApi;
    public gridLoaded: boolean = false;
    public agGridOverlay: string = AgGridHelper.gridSpinnerOverlay;
    public quickFilterText: string;
    public selectedRowsCount: number = 0;
    public allRowsSelected: boolean = false;
    public multiSelectEnabled: boolean;
    public selectionColumnDef: SelectionColumnDef;
    public anyFilterPresent: boolean = false;
    public filteredRowsCount: number;
    public autoSizeStrategy: { type: "fitCellContents" | "fitGridWidth" };

    public fullscreenTitleText = "Make grid full screen";
    private fontAwesomeIcons = iconOverrides({
        type: "font",
        family: "FontAwesome",
        icons: {
            filter: "\u{f0b0}",
            filterActive: "\u{f0b0}",
        },
    });

    public gridTheme: Theme = themeBalham.withPart(this.fontAwesomeIcons);
    public popupParent: HTMLElement | null = null;

    ngOnInit(): void {
        ModuleRegistry.registerModules([AllCommunityModule]);

        this.autoSizeStrategy = this.suppressColumnSizing ? null : { type: this.sizeColumnsToFitGrid ? "fitGridWidth" : "fitCellContents" };

        if (this.defaultRowSelection == "singleRow") {
            this.rowSelection = AgGridHelper.defaultSingleRowSelectionOptions;
        } else if (this.defaultRowSelection == "multiRow") {
            this.rowSelection = AgGridHelper.defaultMultiRowSelectionOptions;

            if (!this.showMultiRowSelectActions) {
                this.rowSelection.checkboxes = false;
                this.rowSelection.enableClickSelection = false;
                (this.rowSelection as MultiRowSelectionOptions).headerCheckbox = false;
            }
        }

        this.multiSelectEnabled = this.rowSelection?.mode == "multiRow";
        if (this.multiSelectEnabled && this.showMultiRowSelectActions) {
            this.selectionColumnDef = {
                pinned: true,
                sortable: true,
                resizable: true,
                width: 70,
                sort: "desc",
                suppressHeaderMenuButton: true,
            };
        }

        if (this.hideTooltips) {
            this.defaultColDef.tooltipValueGetter = null;
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.rowData) {
            this.gridApi?.setGridOption("loading", true);
            this.gridApi?.updateGridOptions({ rowData: this.rowData });
            this.gridApi?.setGridOption("loading", false);
            this.updatePinnedTotalsRow();
        }

        if (changes.columnDefs) {
            this.gridApi?.setGridOption("loading", true);
            this.gridApi?.updateGridOptions({ columnDefs: this.columnDefs });
            this.gridApi?.setGridOption("loading", false);
            this.updatePinnedTotalsRow();
        }

        if (changes.pinnedTotalsRow) {
            this.updatePinnedTotalsRow();
        }
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridReady.emit(event);

        this.gridApi = event.api;

        this.updatePinnedTotalsRow();
    }

    public onFirstDataRendered(event: FirstDataRenderedEvent) {
        this.firstDataLoaded.emit(event);
        this.gridRefReady.emit(this.gridref);
        this.resizeGridColumns();
        this.gridLoaded = true;

        this.updatePinnedTotalsRow();
    }

    public onGridColumnsChanged(event: GridColumnsChangedEvent) {
        this.resizeGridColumns();
    }

    public resizeGridColumns() {
        if (this.suppressColumnSizing) return;
        if (!this.gridApi || !this.autoSizeStrategy) return;

        if (this.autoSizeStrategy.type == "fitCellContents") {
            this.autoSizeColumnsRespectingExplicitWidths();
        } else if (this.autoSizeStrategy.type == "fitGridWidth") {
            // This will size columns to fit the grid width, but it may not be perfect
            // as it doesn't account for the number of columns and their widths.
            this.gridApi?.sizeColumnsToFit();
        }
    }

    private autoSizeColumnsRespectingExplicitWidths(): void {
        if (!this.gridApi) return;

        const explicitWidthKeys = new Set<string>();
        for (const def of this.columnDefs ?? []) {
            const key = (def as any)?.colId ?? (def as any)?.field;
            const explicitWidth = (def as any)?.width ?? (def as any)?.initialWidth;
            if (key && explicitWidth !== undefined && explicitWidth !== null) {
                explicitWidthKeys.add(String(key));
            }
        }

        const keysToAutoSize: string[] = [];
        for (const col of this.gridApi.getColumns() ?? []) {
            const def: any = col.getColDef();
            const key = def?.colId ?? def?.field;
            if (!key) continue;
            if (explicitWidthKeys.has(String(key))) continue;
            keysToAutoSize.push(String(key));
        }

        if (keysToAutoSize.length > 0) {
            this.gridApi.autoSizeColumns(keysToAutoSize);
        }
    }

    public onSelectionChanged(event: SelectionChangedEvent) {
        this.selectionChanged.emit(event);

        if (this.multiSelectEnabled) {
            this.selectedRowsCount = this.gridApi.getSelectedNodes().length;
            this.allRowsSelected = this.selectedRowsCount == this.rowData.length;
        }
    }

    public onFilterChanged(event: FilterChangedEvent) {
        this.filterChanged.emit(event);

        this.anyFilterPresent = event.api.isAnyFilterPresent();

        let filteredRowsCount = 0;
        this.gridApi.forEachNodeAfterFilter(() => {
            filteredRowsCount++;
        });
        this.filteredRowsCount = filteredRowsCount;

        this.updatePinnedTotalsRow();
    }

    public onRowDataUpdated(event: RowDataUpdatedEvent) {
        if (!this.suppressColumnSizing && this.autoSizeStrategy?.type === "fitCellContents") {
            this.autoSizeColumnsRespectingExplicitWidths();
        } else if (!this.suppressColumnSizing && this.autoSizeStrategy?.type === "fitGridWidth") {
            event.api.sizeColumnsToFit();
        }
        if (event.api.isRowDataEmpty()) {
            event.api.showNoRowsOverlay();
            if (!this.gridLoaded) {
                this.gridRefReady.emit(this.gridref);
                this.gridLoaded = true;
            }
        } else {
            event.api.hideOverlay();
        }

        this.updatePinnedTotalsRow();
    }

    oncellEditingStarted(event: CellEditingStartedEvent) {
        this.cellEditingStarted.emit(event);
    }

    oncellEditingStopped(event: CellEditingStoppedEvent) {
        this.cellEditingStopped.emit(event);
    }

    onCellValueChanged(event: CellValueChangedEvent) {
        this.cellValueChanged.emit(event);

        this.updatePinnedTotalsRow();
    }

    onSelectAll() {
        this.gridApi.selectAll('filtered');
    }

    onDeselectAll() {
        this.gridApi.deselectAll('filtered');
    }

    public onFiltersCleared() {
        if (this.hideGlobalFilter) return;
        this.quickFilterText = "";
    }

    public handleScreenSizeChangedEvent() {
        if (this.gridApi) {
            if (this.suppressColumnSizing) return;
            if (this.autoSizeStrategy?.type === "fitCellContents") {
                this.autoSizeColumnsRespectingExplicitWidths();
            } else if (this.autoSizeStrategy?.type === "fitGridWidth") {
                this.gridApi.sizeColumnsToFit();
            }
        }
    }

    private updatePinnedTotalsRow(): void {
        if (!this.gridApi) return;

        const config = this.pinnedTotalsRow;
        if (!config || !Array.isArray(config.fields) || config.fields.length === 0) {
            this.gridApi.setGridOption("pinnedBottomRowData", undefined);
            return;
        }

        const fieldsToSum = config.fields;
        const useFilteredOnly = config.filteredOnly !== false;

        const sums: Record<string, number> = {};
        for (const field of fieldsToSum) {
            sums[field] = 0;
        }

        const addRow = (data: any) => {
            if (!data) return;
            for (const field of fieldsToSum) {
                const rawValue = this.getValueByPath(data, field);
                const numericValue = this.toFiniteNumber(rawValue);
                if (numericValue === null) continue;
                sums[field] += numericValue;
            }
        };

        if (useFilteredOnly) {
            this.gridApi.forEachNodeAfterFilter((node) => {
                if (node.group) return;
                addRow(node.data);
            });
        } else {
            this.gridApi.forEachNode((node) => {
                if (node.group) return;
                addRow(node.data);
            });
        }

        const pinnedRow: Record<string, any> = {};

        if (config.label !== undefined) {
            const labelField = config.labelField ?? (Array.isArray(this.columnDefs) ? (this.columnDefs.find((cd: any) => !!cd?.field)?.field as string) : undefined);

            if (labelField) {
                pinnedRow[labelField] = config.label;
            }
        }

        for (const field of fieldsToSum) {
            pinnedRow[field] = sums[field];
        }

        this.gridApi.setGridOption("pinnedBottomRowData", [pinnedRow]);
    }

    private getValueByPath(obj: any, path: string): any {
        if (!obj || !path) return null;
        if (!path.includes(".")) return obj[path];
        return path.split(".").reduce((acc: any, key: string) => (acc == null ? null : acc[key]), obj);
    }

    private toFiniteNumber(value: any): number | null {
        if (value == null) return null;
        if (typeof value === "number") return Number.isFinite(value) ? value : null;
        if (typeof value === "string") {
            const trimmed = value.trim();
            if (!trimmed) return null;
            const normalized = trimmed.replace(/,/g, "");
            const num = Number(normalized);
            return Number.isFinite(num) ? num : null;
        }
        return null;
    }
}
