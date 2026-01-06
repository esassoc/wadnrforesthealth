import { DatePipe, DecimalPipe, PercentPipe } from "@angular/common";
import { Injectable } from "@angular/core";
import { AgGridAngular } from "ag-grid-angular";
import {
    CellClassFunc,
    CellStyle,
    CellStyleFunc,
    ColDef,
    CsvExportParams,
    NumberFilter,
    EditableCallback,
    SortDirection,
    ValueFormatterFunc,
    ValueGetterFunc,
    ValueGetterParams,
} from "ag-grid-community";
import { CustomDropdownFilterComponent } from "src/app/shared/components/custom-dropdown-filter/custom-dropdown-filter.component";
import { FieldDefinitionGridHeaderComponent } from "src/app/shared/components/field-definition-grid-header/field-definition-grid-header.component";
import { LinkRendererComponent } from "src/app/shared/components/ag-grid/link-renderer/link-renderer.component";
import { ContextMenuRendererComponent } from "src/app/shared/components/ag-grid/context-menu/context-menu-renderer.component";
import { MultiLinkRendererComponent } from "src/app/shared/components/ag-grid/multi-link-renderer/multi-link-renderer.component";
import { PhonePipe } from "src/app/shared/pipes/phone.pipe";

@Injectable({
    providedIn: "root",
})
export class UtilityFunctionsService {
    public static readonly months: string[] = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    public static readonly millisecondsInADay = 86400000;

    public static readonly actionsColumnID = "actions";

    constructor(private datePipe: DatePipe, private decimalPipe: DecimalPipe, private phonePipe: PhonePipe, private percentPipe: PercentPipe) {}

    public getMonthName(monthNumber) {
        return UtilityFunctionsService.months[monthNumber - 1];
    }

    public getNumberFromMonth(month: string) {
        return UtilityFunctionsService.months.indexOf(month) + 1;
    }

    public booleanValueGetter(value: boolean, allowNullValues: boolean = true) {
        if (allowNullValues && value == null) {
            return null;
        }

        return value ? "Yes" : "No";
    }

    public stringToKebabCase(string: string): string {
        return string.replace(/[A-Z]+(?![a-z])|[A-Z]/g, ($, ofs) => (ofs ? "-" : "") + $.toLowerCase());
    }

    public formatDate(date: Date, format: string): string {
        const _datePipe = this.datePipe;
        return _datePipe.transform(date, format);
    }

    public minDate(): Date {
        return new Date("1900-01-01T00:00:00");
    }

    public maxDate(): Date {
        return new Date("2100-01-01T00:00:00");
    }

    public createActionsColumnDef(actionsValueGetter: ValueGetterFunc, hide: boolean = false): ColDef {
        return {
            headerName: "Actions",
            valueGetter: actionsValueGetter,
            cellRenderer: ContextMenuRendererComponent,
            colId: UtilityFunctionsService.actionsColumnID,
            cellClass: "context-menu-container",
            pinned: true,
            sortable: false,
            filter: false,
            suppressSizeToFit: true,
            suppressAutoSize: true,
            width: 100,
            maxWidth: 100,
            hide: hide,
        };
    }

    public createCheckboxSelectionColumnDef(): ColDef {
        return {
            checkboxSelection: true,
            headerCheckboxSelection: true,
            headerCheckboxSelectionFilteredOnly: true,
            headerCheckboxSelectionCurrentPageOnly: false,
            sortable: false,
            filter: false,
            resizable: false,
            pinned: true,
            suppressSizeToFit: true,
            suppressAutoSize: true,
            width: 50,
            maxWidth: 50,
        };
    }

    public defaultValueGetter(params: ValueGetterParams, fieldName: string, containingFieldName: string = "data") {
        const path = fieldName.split(".");
        return path.reduce((obj, key) => (obj != null ? obj[key] : null), containingFieldName ? params[containingFieldName] : params);
    }

    public createBasicColumnDef(headerName: string, fieldName: string, colDefParams?: LtinfoColumnDefParams): ColDef {
        const colDef: ColDef = {
            field: fieldName,
            headerName: headerName,
            valueGetter: (params) => this.defaultValueGetter(params, fieldName),
        };

        this.applyDefaultLtinfoColumnDefParams(colDef, colDefParams);
        return colDef;
    }

    public customDecimalValueGetter(value: number, decimalPlacesToDisplay: number = 2) {
        const _decimalPipe = this.decimalPipe;
        const formatString = `1.${decimalPlacesToDisplay}-${decimalPlacesToDisplay}`;

        return value != null ? _decimalPipe.transform(value, formatString) : null;
    }

    public decimalValueGetter(params: any, fieldName: string, defaultToZeroIfNull: boolean = true): number {
        const fieldNames = fieldName.split(".");

        // checks that each part of a nested field is not null
        let fieldValue = params.data;
        fieldNames.forEach((x) => {
            fieldValue = fieldValue[x];
            if (!fieldValue && defaultToZeroIfNull) {
                fieldValue = 0;
                return;
            }
        });

        return fieldValue;
    }

    public decimalComparator(id1: any, id2: any) {
        if (!id1) {
            return -1;
        }
        if (!id2) {
            return 1;
        }

        const id1Cleaned = typeof id1 === "string" ? id1.replace(/,/g, "") : id1;
        const id2Cleaned = typeof id2 === "string" ? id2.replace(/,/g, "") : id2;

        const value1 = parseFloat(id1Cleaned);
        const value2 = parseFloat(id2Cleaned);
        return value1 == value2 ? 0 : value1 > value2 ? 1 : -1;
    }

    public convertStringToDecimal(value: string): number {
        if (!value) {
            return null;
        }

        // accounting for parseFloat() function treating commas as decimals
        return parseFloat(value.replace(",", ""));
    }

    public createDecimalColumnDef(headerName: string, fieldName: string, decimalColumnDefParams?: DecimalColumnDefParams) {
        const _decimalPipe = this.decimalPipe;

        const decimalPlacesToDisplay = decimalColumnDefParams?.MaxDecimalPlacesToDisplay ?? 2;
        const minDecimalPlaces = decimalColumnDefParams?.MinDecimalPlacesToDisplay ?? decimalPlacesToDisplay;
        const decimalFormatString = "1." + minDecimalPlaces + "-" + decimalPlacesToDisplay;

        let decimalColDef: ColDef;

        if (decimalColumnDefParams?.Editable) {
            // Use field and valueFormatter for editable columns
            decimalColDef = {
                headerName: headerName,
                field: fieldName,
                cellStyle: { "justify-content": "flex-end" },
                editable: true,
                valueFormatter: (params) => {
                    const value = params.value?.value ?? params.value; // Handle object with `value` property
                    if (typeof value === "number") {
                        return _decimalPipe.transform(value, decimalFormatString);
                    }
                    return decimalColumnDefParams?.ZeroFillNullValues
                        ? _decimalPipe.transform(0, decimalFormatString)
                        : decimalColumnDefParams?.StringForNullValues
                        ? decimalColumnDefParams?.StringForNullValues
                        : null;
                },
                filter: "agNumberColumnFilter",
                filterValueGetter: (params) => {
                    const value = params.data?.[fieldName]?.value ?? params.data?.[fieldName]; // Handle object with `value` property
                    if (typeof value === "number") {
                        return value;
                    }
                    return null; // Return null if value is not a number
                },
                comparator: this.decimalComparator,
            };
        } else {
            // Use valueGetter for read-only columns
            decimalColDef = {
                headerName: headerName,
                cellStyle: { "justify-content": "flex-end" },
                valueGetter: (params) => {
                    const value = params.data?.[fieldName]?.value ?? params.data?.[fieldName]; // Handle object with `value` property
                    return value != null
                        ? _decimalPipe.transform(value, decimalFormatString)
                        : decimalColumnDefParams?.ZeroFillNullValues
                        ? _decimalPipe.transform(0, decimalFormatString)
                        : decimalColumnDefParams?.StringForNullValues
                        ? decimalColumnDefParams?.StringForNullValues
                        : null;
                },
                filter: "agNumberColumnFilter",
                filterValueGetter: (params) => {
                    const value = params.data?.[fieldName]?.value ?? params.data?.[fieldName]; // Handle object with `value` property
                    return this.convertStringToDecimal(_decimalPipe.transform(value, decimalFormatString));
                },
                comparator: this.decimalComparator,
            };
        }

        this.applyDefaultLtinfoColumnDefParams(decimalColDef, decimalColumnDefParams);
        return decimalColDef;
    }

    public createLatLonColumnDef(headerName: "Latitude" | "Longitude", fieldName: string) {
        return this.createDecimalColumnDef(headerName, fieldName, { MaxDecimalPlacesToDisplay: 5 });
    }

    /**
     * Create a currency column definition. Defaults to USD symbol and 0 decimal places.
     */
    public createCurrencyColumnDef(headerName: string, fieldName: string, currencyColumnDefParams?: CurrencyColumnDefParams) {
        const _decimalPipe = this.decimalPipe;
        const currencySymbol = currencyColumnDefParams?.CurrencySymbol ?? "$";

        const decimalPlacesToDisplay = currencyColumnDefParams?.MaxDecimalPlacesToDisplay ?? 0;
        const minDecimalPlaces = currencyColumnDefParams?.MinDecimalPlacesToDisplay ?? decimalPlacesToDisplay;
        const decimalFormatString = "1." + minDecimalPlaces + "-" + decimalPlacesToDisplay;

        let colDef: ColDef;

        if (currencyColumnDefParams?.Editable) {
            colDef = {
                headerName: headerName,
                field: fieldName,
                cellStyle: { "justify-content": "flex-end" },
                editable: true,
                valueFormatter: (params) => {
                    const value = params.value?.value ?? params.value;
                    if (typeof value === "number") {
                        // Use decimalPipe to format numeric with configured decimals, then prefix currency symbol
                        return currencySymbol + _decimalPipe.transform(value, decimalFormatString);
                    }
                    return currencyColumnDefParams?.ZeroFillNullValues
                        ? currencySymbol + _decimalPipe.transform(0, decimalFormatString)
                        : currencyColumnDefParams?.StringForNullValues
                        ? currencyColumnDefParams?.StringForNullValues
                        : null;
                },
                filter: "agNumberColumnFilter",
                filterValueGetter: (params) => {
                    const value = params.data?.[fieldName]?.value ?? params.data?.[fieldName];
                    if (typeof value === "number") return value;
                    return null;
                },
                comparator: this.decimalComparator,
            };
        } else {
            colDef = {
                headerName: headerName,
                cellStyle: { "justify-content": "flex-end" },
                valueGetter: (params) => {
                    const value = params.data?.[fieldName]?.value ?? params.data?.[fieldName];
                    return value != null
                        ? currencySymbol + _decimalPipe.transform(value, decimalFormatString)
                        : currencyColumnDefParams?.ZeroFillNullValues
                        ? currencySymbol + _decimalPipe.transform(0, decimalFormatString)
                        : currencyColumnDefParams?.StringForNullValues
                        ? currencyColumnDefParams?.StringForNullValues
                        : null;
                },
                filter: "agNumberColumnFilter",
                filterValueGetter: (params) => this.convertStringToDecimal(_decimalPipe.transform(params.data?.[fieldName], decimalFormatString)),
                comparator: this.decimalComparator,
            };
        }

        this.applyDefaultLtinfoColumnDefParams(colDef, currencyColumnDefParams);
        return colDef;
    }

    /**
     * Create a percent column definition. Uses PercentPipe and allows specifying decimal places similar to createDecimalColumnDef.
     */
    public createPercentColumnDef(headerName: string, fieldName: string, percentColumnDefParams?: PercentColumnDefParams) {
        const _percentPipe = this.percentPipe;

        const decimalPlacesToDisplay = percentColumnDefParams?.MaxDecimalPlacesToDisplay ?? 1;
        const minDecimalPlaces = percentColumnDefParams?.MinDecimalPlacesToDisplay ?? decimalPlacesToDisplay;
        // PercentPipe format: '1.2-2%' but PercentPipe expects number between 0 and 1 for 100% -> use transform directly
        const decimalFormatString = "1." + minDecimalPlaces + "-" + decimalPlacesToDisplay;

        let colDef: ColDef;

        if (percentColumnDefParams?.Editable) {
            colDef = {
                headerName: headerName,
                field: fieldName,
                cellStyle: { "justify-content": "flex-end" },
                editable: true,
                valueFormatter: (params) => {
                    const value = params.value?.value ?? params.value;
                    if (typeof value === "number") {
                        return _percentPipe.transform(value, decimalFormatString);
                    }
                    return percentColumnDefParams?.ZeroFillNullValues
                        ? _percentPipe.transform(0, decimalFormatString)
                        : percentColumnDefParams?.StringForNullValues
                        ? percentColumnDefParams?.StringForNullValues
                        : null;
                },
                filter: "agNumberColumnFilter",
                filterValueGetter: (params) => {
                    const value = params.data?.[fieldName]?.value ?? params.data?.[fieldName];
                    if (typeof value === "number") return value;
                    return null;
                },
                comparator: this.decimalComparator,
            };
        } else {
            colDef = {
                headerName: headerName,
                cellStyle: { "justify-content": "flex-end" },
                valueGetter: (params) => {
                    const value = params.data?.[fieldName]?.value ?? params.data?.[fieldName];
                    return value != null
                        ? _percentPipe.transform(value, decimalFormatString)
                        : percentColumnDefParams?.ZeroFillNullValues
                        ? _percentPipe.transform(0, decimalFormatString)
                        : percentColumnDefParams?.StringForNullValues
                        ? percentColumnDefParams?.StringForNullValues
                        : null;
                },
                filter: "agNumberColumnFilter",
                filterValueGetter: (params) => this.convertStringToDecimal(_percentPipe.transform(params.data?.[fieldName], decimalFormatString)),
                comparator: this.decimalComparator,
            };
        }

        this.applyDefaultLtinfoColumnDefParams(colDef, percentColumnDefParams);
        return colDef;
    }

    public createYearColumnDef(headerName: string, fieldName: string, colDefParams?: LtinfoColumnDefParams): ColDef {
        const colDef: ColDef = {
            headerName: headerName,
            valueGetter: (params) => this.decimalValueGetter(params, fieldName, false),
            comparator: this.decimalComparator,
            filter: "agNumberColumnFilter",
            cellStyle: { "justify-content": "flex-end" },
        };

        this.applyDefaultLtinfoColumnDefParams(colDef, colDefParams);
        return colDef;
    }

    public createPhoneNumberColumnDef(headerName: string, fieldName: string): ColDef {
        return {
            headerName: headerName,
            field: fieldName,
            valueFormatter: (params) => this.phonePipe.transform(params.value),
            filterParams: {
                textFormatter: this.phonePipe.gridFilterTextFormatter,
            },
        };
    }

    /**
     * Create a phone column definition that formats phone numbers using PhonePipe
     */
    public createPhoneColumnDef(headerName: string, fieldName: string, colDefParams?: LtinfoColumnDefParams): ColDef {
        const colDef: ColDef = {
            headerName: headerName,
            field: fieldName,
            valueGetter: (params) => this.defaultValueGetter(params, fieldName),
            valueFormatter: (params) => this.phonePipe.transform(params.value),
            filterParams: {
                textFormatter: this.phonePipe.gridFilterTextFormatter,
            },
        };

        this.applyDefaultLtinfoColumnDefParams(colDef, colDefParams);
        return colDef;
    }

    public linkRendererComparator(id1: any, id2: any) {
        if (id1.LinkDisplay == id2.LinkDisplay) {
            return 0;
        }
        return id1.LinkDisplay > id2.LinkDisplay ? 1 : -1;
    }

    public createLinkColumnDef(headerName: string, fieldName: string, linkValueField: string, linkColumnDefParams?: LinkColumnDefParams) {
        const colDef: ColDef = {
            headerName: headerName,
            field: fieldName,
            valueGetter: (params) => {
                const linkVal = this.defaultValueGetter(params, linkValueField);
                const displayVal = this.defaultValueGetter(params, linkColumnDefParams?.LinkDisplayField ?? fieldName);
                // Only include LinkValue if it resolves to a non-null/undefined/empty value
                if (linkVal === null || linkVal === undefined || linkVal === "") {
                    return { LinkDisplay: displayVal, LinkValue: null };
                }
                return { LinkValue: linkVal, LinkDisplay: displayVal };
            },
            filterValueGetter: (params) => this.defaultValueGetter(params, fieldName),
            comparator: this.linkRendererComparator,
            cellRenderer: LinkRendererComponent,
            cellRendererParams: {
                inRouterLink: linkColumnDefParams?.InRouterLink,
                inRouterLinkHandler: linkColumnDefParams?.InRouterLinkHandler,
                isExternalUrl: linkColumnDefParams?.IsExternalUrl,
            },
        };

        this.applyDefaultLtinfoColumnDefParams(colDef, linkColumnDefParams);
        return colDef;
    }

    public createLinkHrefColumnDef(headerName: string, fieldName: string, linkValueField: string, linkColumnDefParams?: LinkColumnDefParams) {
        const colDef: ColDef = {
            headerName: headerName,
            field: fieldName,
            valueGetter: (params) => {
                const linkVal = this.defaultValueGetter(params, linkValueField);
                const displayVal = this.defaultValueGetter(params, linkColumnDefParams?.LinkDisplayField ?? fieldName);
                if (linkVal === null || linkVal === undefined || linkVal === "") {
                    return { LinkDisplay: displayVal, LinkValue: null };
                }
                return { LinkValue: linkVal, LinkDisplay: displayVal, href: `${linkColumnDefParams.HrefTemplate}/${linkVal}` };
            },
            filterValueGetter: (params) => this.defaultValueGetter(params, fieldName),
            comparator: this.linkRendererComparator,
            cellRenderer: LinkRendererComponent,
        };
        this.applyDefaultLtinfoColumnDefParams(colDef, linkColumnDefParams);
        return colDef;
    }

    public multiLinkRendererComparator(id1: any, id2: any) {
        if (id1.downloadDisplay == id2.downloadDisplay) {
            return 0;
        }
        return id1.downloadDisplay > id2.downloadDisplay ? 1 : -1;
    }

    public createMultiLinkColumnDef(
        headerName: string,
        listField: string,
        linkValueField: string,
        linkDisplayField: string,
        multiLinkColumnDefParams?: MultiLinkColumnDefParams
    ): ColDef {
        const colDef: ColDef = {
            headerName: headerName,
            valueGetter: (params) => {
                const list = this.defaultValueGetter(params, listField) ?? [];
                const names = (list || [])
                    .map((x) => ({
                        LinkValue: this.defaultValueGetter(x, linkValueField, ""),
                        LinkDisplay: this.defaultValueGetter(x, linkDisplayField, ""),
                    }))
                    .filter((n) => n.LinkValue !== null && n.LinkValue !== undefined && n.LinkValue !== "");
                const downloadDisplay = names.map((x) => x.LinkDisplay).join(", ");
                return { links: names, downloadDisplay: downloadDisplay };
            },
            filterValueGetter: (params) =>
                this.defaultValueGetter(params, listField)
                    ?.map((x) => this.defaultValueGetter(x, linkDisplayField, ""))
                    .join(", "),
            comparator: this.multiLinkRendererComparator,
            cellRenderer: MultiLinkRendererComponent,
            cellRendererParams: {
                inRouterLink: multiLinkColumnDefParams?.InRouterLink,
                inRouterLinkHandler: multiLinkColumnDefParams?.InRouterLinkHandler,
            },
        };

        this.applyDefaultLtinfoColumnDefParams(colDef, multiLinkColumnDefParams);
        return colDef;
    }

    private dateFilterComparator(filterLocalDateAtMidnight, cellValue) {
        const filterDate = Date.parse(filterLocalDateAtMidnight);
        const cellDate = Date.parse(cellValue);

        return cellDate == filterDate ? 0 : cellDate < filterDate ? -1 : 1;
    }

    public dateSortComparator(id1: any, id2: any) {
        const date1 = id1 ? Date.parse(id1) : Date.parse("1/1/1900");
        const date2 = id2 ? Date.parse(id2) : Date.parse("1/1/1900");

        return date1 == date2 ? 0 : date1 > date2 ? 1 : -1;
    }

    public createDateColumnDef(headerName: string, fieldName: string, dateFormat: string, dateColumnDefParams?: DateColumnDefParams): ColDef {
        const _datePipe = this.datePipe;
        const timezone = dateColumnDefParams?.IgnoreLocalTimezone ? "+0000" : null;

        const dateColDef: ColDef = {
            headerName: headerName,
            valueGetter: (params) => {
                const value = this.defaultValueGetter(params, fieldName);
                return _datePipe.transform(value, dateFormat, timezone);
            },
            comparator: this.dateSortComparator,
            filter: "agDateColumnFilter",
            filterParams: {
                filterOptions: ["inRange"],
                comparator: this.dateFilterComparator,
            },
            sort: dateColumnDefParams?.Sort,
        };

        this.applyDefaultLtinfoColumnDefParams(dateColDef, dateColumnDefParams);
        return dateColDef;
    }

    public createBooleanColumnDef(headerName: string, fieldName: string, linkColumnDefParams?: LinkColumnDefParams): ColDef {
        const colDef: ColDef<any, any> = {
            headerName: headerName,
            valueGetter: (params) => this.defaultValueGetter(params, fieldName),
            valueFormatter: (params) => this.booleanValueGetter(params.value),
            filter: true,
        };
        this.applyDefaultLtinfoColumnDefParams(colDef, linkColumnDefParams);
        return colDef;
    }

    public createDaysPassedColumnDef(headerName: string, dateFieldName: string, colDefParams?: LtinfoColumnDefParams) {
        const colDef: ColDef = {
            field: "Days Open",
            valueGetter: (params) => {
                const currentDate = new Date();
                const dateCreated = new Date(params.data[dateFieldName]);

                const currentTime = currentDate.getTime();
                const startTime = dateCreated.getTime();

                const daysPassed = Math.round((currentTime - startTime) / UtilityFunctionsService.millisecondsInADay);
                return `${daysPassed} day${daysPassed > 1 ? "s" : ""}`;
            },
            cellStyle: { "justify-content": "flex-end" },
            filter: "agNumberColumnFilter",
        };

        this.applyDefaultLtinfoColumnDefParams(colDef, colDefParams);
        return colDef;
    }

    public applyDefaultLtinfoColumnDefParams(colDef: ColDef, params: LtinfoColumnDefParams) {
        if (!params) {
            return;
        }

        if (params.WrapHeaderText !== undefined) {
            colDef.wrapHeaderText = params.WrapHeaderText;
        }
        if (params.AutoHeaderHeight !== undefined) {
            colDef.autoHeaderHeight = params.AutoHeaderHeight;
        }

        if (params.FieldDefinitionType) {
            colDef.headerComponentParams = {
                innerHeaderComponent: FieldDefinitionGridHeaderComponent,
                innerHeaderComponentParams: {
                    fieldDefinitionType: params.FieldDefinitionType,
                    labelOverride: params.FieldDefinitionLabelOverride,
                },
            };
        }

        if (params.UseCustomDropdownFilter || params.CustomDropdownFilterField) {
            colDef.filter = CustomDropdownFilterComponent;
            colDef.filterParams = {
                field: params.CustomDropdownFilterField,
                columnContainsMultipleValues: params.ColumnContainsMultipleValues,
                filterPopup: true,
            };
            colDef.floatingFilter = false;
        }

        if (params.Width) {
            colDef.width = params.Width;
        }
        if (params.MaxWidth) {
            colDef.maxWidth = params.MaxWidth;
        }
        if (params.Hide) {
            colDef.hide = params.Hide;
        }
        if (params.ValueGetter) {
            colDef.valueGetter = params.ValueGetter;
        }
        if (params.FilterValueGetter) {
            colDef.filterValueGetter = params.FilterValueGetter;
        }
        if (params.ValueFormatter) {
            colDef.valueFormatter = params.ValueFormatter;
        }
        if (params.CellClass) {
            colDef.cellClass = params.CellClass;
        }
        if (params.CellStyle) {
            colDef.cellStyle = params.CellStyle;
        }
        if (params.Sort) {
            colDef.sort = params.Sort;
        }
        if (params.Editable) {
            colDef.editable = params.Editable;
        }
        if (params.CellRenderer) {
            colDef.cellRenderer = params.CellRenderer;
        }
    }

    public exportGridToCsv(grid: AgGridAngular, fileName: string, columnKeys: Array<string>) {
        const params = {
            skipHeader: false,
            columnGroups: false,
            skipFooters: true,
            skipRowGroups: true,
            skipPinnedTop: true,
            skipPinnedBottom: true,
            allColumns: true,
            onlySelected: false,
            suppressQuotes: false,
            fileName: fileName,
            processCellCallback: function (p) {
                if (p.column.getColDef().cellRenderer) {
                    if (p.value.downloadDisplay) {
                        return p.value.downloadDisplay;
                    } else {
                        return p.value.LinkDisplay;
                    }
                } else {
                    return p.value;
                }
            },
        } as CsvExportParams;
        if (columnKeys) {
            // exclude actions column from export
            params.columnKeys = columnKeys.filter((x) => x !== UtilityFunctionsService.actionsColumnID);
        }
        grid.api.exportDataAsCsv(params);
    }

    public deepEqual(obj1: any, obj2: any): boolean {
        if (obj1 === obj2) {
            return true;
        }

        if (typeof obj1 !== "object" || typeof obj2 !== "object" || obj1 == null || obj2 == null) {
            return false;
        }

        const keys1 = Object.keys(obj1);
        const keys2 = Object.keys(obj2);
        if (keys1.length !== keys2.length) {
            return false;
        }

        return keys1.every((key) => this.deepEqual(obj1[key], obj2[key]));
    }
}

export interface LtinfoColumnDefParams {
    Width?: number;
    MaxWidth?: number;
    Hide?: boolean;
    WrapHeaderText?: boolean; // When true, header text wraps (default: false)
    AutoHeaderHeight?: boolean; // When true, header row height grows to fit wrapped header text (default: false)
    FieldDefinitionType?: string;
    FieldDefinitionLabelOverride?: string;
    UseCustomDropdownFilter?: boolean; // use to enable CustomDropdownFilter without specifying a field
    CustomDropdownFilterField?: string;
    ColumnContainsMultipleValues?: boolean;
    ValueGetter?: ValueGetterFunc;
    FilterValueGetter?: ValueGetterFunc;
    ValueFormatter?: ValueFormatterFunc;
    CellClass?: string | string[] | CellClassFunc;
    CellStyle?: CellStyle | CellStyleFunc;
    Sort?: SortDirection;
    Editable?: boolean | EditableCallback;
    CellRenderer?: any;
}

export interface LinkColumnDefParams extends LtinfoColumnDefParams {
    Width?: number;
    InRouterLink?: string;
    /** Optional per-row router link builder. If provided it will be called with the renderer params to produce the inRouterLink string. */
    InRouterLinkHandler?: (params: any) => string;
    IsExternalUrl?: boolean;
    LinkDisplayField?: string;
    HrefTemplate?: string;
}

export interface MultiLinkColumnDefParams extends LtinfoColumnDefParams {
    InRouterLink?: string;
    InRouterLinkHandler?: (params: any) => string;
}

export interface DecimalColumnDefParams extends LtinfoColumnDefParams {
    MinDecimalPlacesToDisplay?: number;
    MaxDecimalPlacesToDisplay?: number;
    ZeroFillNullValues?: boolean;
    StringForNullValues?: string;
}

export interface CurrencyColumnDefParams extends DecimalColumnDefParams {
    /** Currency symbol to prefix (default: $) */
    CurrencySymbol?: string;
}

export interface DateColumnDefParams extends LtinfoColumnDefParams {
    Sort?: SortDirection;
    IgnoreLocalTimezone?: boolean;
}

export interface PercentColumnDefParams extends DecimalColumnDefParams {
    /** Inherits Min/Max decimal places and null handling options from DecimalColumnDefParams */
}
