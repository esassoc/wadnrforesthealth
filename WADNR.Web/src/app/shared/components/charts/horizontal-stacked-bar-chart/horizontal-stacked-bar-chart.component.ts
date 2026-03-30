import { Component, Input, OnChanges } from "@angular/core";
import { default as vegaEmbed, VisualizationSpec } from "vega-embed";
import { LoadingDirective } from "../../../directives/loading.directive";
import { DecimalPipe, PercentPipe, CurrencyPipe } from "@angular/common";
import { DefaultOrderKeyvaluePipe } from "src/app/shared/pipes/default-order-key-value.pipe";
import { ChartData } from "src/app/shared/generated/model/models";

@Component({
    selector: "horizontal-stacked-bar-chart",
    imports: [LoadingDirective, DefaultOrderKeyvaluePipe],
    providers: [DecimalPipe, PercentPipe, CurrencyPipe],
    templateUrl: "./horizontal-stacked-bar-chart.component.html",
    styleUrl: "./horizontal-stacked-bar-chart.component.scss",
})
export class HorizontalStackedBarChartComponent implements OnChanges {
    @Input() chartID: string = "";
    @Input() chartData: ChartData[];
    @Input() chartHeight: number = 400;
    @Input() isLoading: boolean = false;

    private _tooltipEl: HTMLDivElement | null = null;
    private _viewRef: any = null;

    @Input() colorRange: Map<string, string> = new Map([
        ["Extent of Observed Water", "#9EBBD7"],
        ["Riparian Management Zone", "#CDF57A"],
        ["Floodplain", "#9ED7C2"],
    ]);

    constructor(private decimalPipe: DecimalPipe, private percentPipe: PercentPipe, private currencyPipe: CurrencyPipe) {}

    // Value formatting inputs (mirror vertical component)
    @Input() valueFormatter?: string | ((v: number, datum?: ChartData) => string);
    @Input() valueFractionDigits: number = 0;

    // Helper to format numeric values according to input settings
    private formatValue(v: number | null | undefined, datum?: ChartData): string {
        if (v == null) {
            return "";
        }
        // If a custom function is provided, use it
        if (typeof this.valueFormatter === "function") {
            try {
                return (this.valueFormatter as any)(v, datum);
            } catch {
                // fall through to default formatting
            }
        }

        const frac = Math.max(0, Math.floor(this.valueFractionDigits ?? 0));
        const fmtName = typeof this.valueFormatter === "string" ? (this.valueFormatter as string) : "numeric";
        const digits = `1.0-${frac}`;
        try {
            if (fmtName === "percent") {
                return this.percentPipe.transform(v, digits) ?? String(v);
            }
            if (fmtName === "currency") {
                return this.currencyPipe.transform(v, "USD", "symbol", digits) ?? String(v);
            }
            if (fmtName === "number") {
                return this.decimalPipe.transform(v, digits) ?? String(v);
            }
            return this.decimalPipe.transform(v, digits) ?? String(v);
        } catch (e) {
            return String(v);
        }
    }

    ngOnChanges() {
        // Preprocess chartData to create a breakdown string/html per XValue (year)
        if (this.chartData && Array.isArray(this.chartData)) {
            const byYear = new Map<number | string, any[]>();
            this.chartData.forEach((d) => {
                const key = d.XValue;
                if (!byYear.has(key)) {
                    byYear.set(key, []);
                }
                byYear.get(key).push(d);
            });

            byYear.forEach((arr, year) => {
                // Total treats null as 0
                const total = arr.reduce((s, a) => s + (a.YValue ?? 0), 0);
                // Only include lines for groups that have a non-null numeric value
                const rowsWithValues = arr.filter((a) => a.YValue != null);
                // Canonical breakdown rows
                const rows = rowsWithValues.map((a) => ({ label: a.Group, value: a.YValue }));
                // Derived forms
                const text = this.buildBreakdownTextFromRows(year, rows, total);
                arr.forEach((a) => {
                    (a as any).BreakdownRows = rows;
                    (a as any).BreakdownTotal = total;
                    (a as any).BreakdownYear = year;
                });
            });
        }

        const vegaSpec = {
            $schema: "https://vega.github.io/schema/vega-lite/v6.json",
            width: "container",
            height: this.chartHeight,
            autosize: {
                type: "fit-x",
                resize: true,
            },
            config: {
                view: { stroke: null },
                axisX: {
                    labelFont: "Arial",
                    labelFontSize: 14,
                    labelFontWeight: "bold",
                    labelPadding: 10,
                    offset: 2,
                    ticks: false,
                    domainColor: "#dcdcdc",
                },
                axisY: {
                    labelFont: "Arial",
                    labelFontSize: 14,
                    labelFontWeight: "bold",
                    titleFontSize: 14,
                    titleAnchor: "start",
                    titleBaseline: "line-bottom",
                    titleAlign: "right",
                    titlePadding: 0,
                    titleX: -10,
                    titleOpacity: 0.5,
                    labelOpacity: 0.5,
                    ticks: false,
                    domain: false,
                },
                legend: { disable: true },
                scale: { barBandPaddingInner: 0.2 },
            },

            data: {
                name: "table",
                values: this.chartData,
            },

            transform: [{ calculate: "datetime(datum.XValue, 0, 1)", as: "XYear" }],

            encoding: {
                // Horizontal stacked bars: x is quantitative (YValue), y is the year/ordinal
                x: { field: "YValue", title: "Total " + "Acres", type: "quantitative", axis: { gridDash: { value: [8, 8] } }, stack: "zero" },
                y: { field: "XYear", type: "temporal", timeUnit: "year", title: "", sort: { field: "SortOrder" }, axis: { labelAngle: 0, format: "%Y" } },
                color: {
                    sort: "ascending",
                    field: "Group",
                    title: "",
                    type: "nominal",
                    scale: {
                        domain: [...this.colorRange.keys()],
                        range: [...this.colorRange.values()],
                    },
                },
                order: { field: "SortOrder" },
                // disable built-in tooltip — we provide a custom HTML tooltip
                tooltip: null,
            },

            layer: [
                {
                    mark: {
                        type: "bar",
                        cornerRadiusEnd: 6,
                        stroke: "#CDF57A",
                    },
                },
            ],
        } as VisualizationSpec;

        // Render and wire custom tooltip
        // Clean up previous tooltip and view
        if (this._tooltipEl) {
            this._tooltipEl.remove();
            this._tooltipEl = null;
        }
        if (this._viewRef && typeof this._viewRef.finalize === "function") {
            try {
                this._viewRef.finalize();
            } catch {}
            this._viewRef = null;
        }

        vegaEmbed(`#${this.chartID}`, vegaSpec, { renderer: "svg" }).then((res) => {
            const view: any = (res as any).view;
            this._viewRef = view;

            const tooltip = document.createElement("div");
            tooltip.className = "vstack-tooltip";
            tooltip.setAttribute("role", "tooltip");
            tooltip.style.position = "fixed";
            tooltip.style.pointerEvents = "none";
            tooltip.style.background = "white";
            tooltip.style.border = "1px solid rgba(0,0,0,0.1)";
            tooltip.style.padding = "8px";
            tooltip.style.borderRadius = "6px";
            tooltip.style.boxShadow = "0 2px 6px rgba(0,0,0,0.15)";
            tooltip.style.zIndex = "1000";
            tooltip.style.display = "none";
            this._tooltipEl = tooltip;
            document.body.appendChild(tooltip);

            view.addEventListener("mouseover", (event: any, item: any) => {
                try {
                    const datum = item && item.datum ? item.datum : null;
                    if (!datum) {
                        return;
                    }
                    let innerHtml = "";
                    let ariaText = "";
                    if (datum && (datum as any).BreakdownRows) {
                        const rows = (datum as any).BreakdownRows as Array<{ label: string; value: number }>;
                        const year = (datum as any).BreakdownYear;
                        const total = (datum as any).BreakdownTotal;
                        innerHtml =
                            this.buildBreakdownHtmlFromRows(year, rows) +
                            `<div class="breakdown-total-sep"><div class="breakdown-row"><span class="breakdown-label breakdown-label-bold">Total</span><span class="breakdown-value">${this.formatValue(
                                total
                            )}</span></div></div>`;
                        ariaText = this.buildBreakdownTextFromRows(year, rows, total);
                    } else if (this.chartData) {
                        // Fallback: compute rows for this datum's XValue from chartData
                        const key = datum && datum.XValue != null ? datum.XValue : null;
                        const groupForKey = key != null ? this.chartData.filter((d) => d.XValue === key) : [];
                        const rows = groupForKey.filter((a) => a.YValue != null).map((a) => ({ label: a.Group, value: a.YValue }));
                        const year = key;
                        const total = groupForKey.reduce((s, a) => s + (a.YValue ?? 0), 0);
                        innerHtml =
                            this.buildBreakdownHtmlFromRows(year, rows) +
                            `<div class="breakdown-total-sep"><div class="breakdown-row"><span class="breakdown-label breakdown-label-bold">Total</span><span class="breakdown-value">${this.formatValue(
                                total
                            )}</span></div></div>`;
                        ariaText = this.buildBreakdownTextFromRows(year, rows, total);
                    } else {
                        innerHtml = "";
                        ariaText = "";
                    }
                    tooltip.innerHTML = `<div class='vstack-tooltip-inner'>${innerHtml}</div>`;
                    tooltip.setAttribute("aria-label", ariaText);
                    tooltip.style.display = "block";
                    const padding = 12;
                    const tooltipRect = tooltip.getBoundingClientRect();
                    const vpWidth = window.innerWidth;
                    const vpHeight = window.innerHeight;
                    let left = event.clientX + padding;
                    let top = event.clientY + padding;
                    if (left + tooltipRect.width + padding > vpWidth) {
                        left = event.clientX - tooltipRect.width - padding;
                    }
                    if (top + tooltipRect.height + padding > vpHeight) {
                        top = event.clientY - tooltipRect.height - padding;
                    }
                    tooltip.style.left = Math.max(8, left) + "px";
                    tooltip.style.top = Math.max(8, top) + "px";
                } catch (e) {}
            });

            view.addEventListener("mousemove", (event: any) => {
                if (!this._tooltipEl) {
                    return;
                }
                const tooltip = this._tooltipEl;
                const padding = 12;
                const tooltipRect = tooltip.getBoundingClientRect();
                const vpWidth = window.innerWidth;
                const vpHeight = window.innerHeight;
                let left = event.clientX + padding;
                let top = event.clientY + padding;
                if (left + tooltipRect.width + padding > vpWidth) {
                    left = event.clientX - tooltipRect.width - padding;
                }
                if (top + tooltipRect.height + padding > vpHeight) {
                    top = event.clientY - tooltipRect.height - padding;
                }
                tooltip.style.left = Math.max(8, left) + "px";
                tooltip.style.top = Math.max(8, top) + "px";
            });

            view.addEventListener("mouseout", () => {
                if (!this._tooltipEl) {
                    return;
                }
                this._tooltipEl.style.display = "none";
            });
        });
    }

    // Small helper to escape HTML text when injecting into tooltip markup
    private escapeHtml(input: any): string {
        if (input == null) {
            return "";
        }
        const s = String(input);
        return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/\"/g, "&quot;").replace(/'/g, "&#039;");
    }

    // Build HTML from canonical BreakdownRows
    private buildBreakdownHtmlFromRows(year: any, rows: Array<{ label: string; value: number }>): string {
        const header = `<p><b>${this.escapeHtml(year)}</b></p>`;
        const lines = rows
            .map(
                (r) =>
                    `<div class="breakdown-row"><span class="breakdown-label">${this.escapeHtml(r.label)}</span><span class="breakdown-value">${this.formatValue(
                        r.value
                    )}</span></div>`
            )
            .join("");
        return `${header}${lines}`;
    }

    // Build plain-text fallback from canonical rows
    private buildBreakdownTextFromRows(year: any, rows: Array<{ label: string; value: number }>, total: number): string {
        const lines = rows.map((r) => `${r.label}: ${this.formatValue(r.value)}`);
        return lines.length ? `${year}\n${lines.join("\n")}\nTotal: ${this.formatValue(total)}` : `${year}\nTotal: ${this.formatValue(total)}`;
    }
}
