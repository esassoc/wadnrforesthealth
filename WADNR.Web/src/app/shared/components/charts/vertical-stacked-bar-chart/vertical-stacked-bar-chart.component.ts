import { Component, Input, OnChanges, ElementRef, OnDestroy, Renderer2 } from "@angular/core";
import { default as vegaEmbed, VisualizationSpec } from "vega-embed";
import { LoadingDirective } from "../../../directives/loading.directive";
import { DecimalPipe, PercentPipe, CurrencyPipe } from "@angular/common";
import { DefaultOrderKeyvaluePipe } from "src/app/shared/pipes/default-order-key-value.pipe";
import { ChartDatum } from "../chart-datum";

// Augmented datum used internally: include computed breakdown fields
interface ChartDatumAugmented extends ChartDatum {
    BreakdownRows?: Array<{ label: string; value: number }>;
    BreakdownTotal?: number;
    BreakdownYear?: any;
}

@Component({
    selector: "vertical-stacked-bar-chart",
    imports: [LoadingDirective, DefaultOrderKeyvaluePipe],
    providers: [DecimalPipe, PercentPipe, CurrencyPipe],
    templateUrl: "./vertical-stacked-bar-chart.component.html",
    styleUrl: "./vertical-stacked-bar-chart.component.scss",
})
export class VerticalStackedBarChartComponent implements OnChanges, OnDestroy {
    // Simple local toggle state (no reactive forms)
    private _tooltipEl: HTMLDivElement | null = null;
    private _viewRef: any = null;
    // Track Vega event listeners for removal when finalizing the view
    private _vegaEventHandlers: Array<{ view: any; type: string; handler: any }> = [];
    // Store remover callbacks from renderer.listen for DOM listeners so we can dispose them
    private _domListenerRemovers: Array<() => void> = [];
    // Track hidden groups (toggled off)
    public hiddenGroups: Set<string> = new Set<string>();
    // Highlighted group on hover (reserved for future use)
    public highlightedGroup: string | null = null;

    constructor(private _el: ElementRef, private decimalPipe: DecimalPipe, private percentPipe: PercentPipe, private currencyPipe: CurrencyPipe, private renderer: Renderer2) {}
    @Input() chartID: string = "";
    @Input() chartData: ChartDatum[];
    @Input() chartHeight: number = 400;
    @Input() isLoading: boolean = false;
    @Input() xFieldName: string = "X Value";
    @Input() yFieldName: string = "Y Value";
    @Input() groupFieldName: string = "Group";

    @Input() colorRange: Map<string, string> = new Map([
        ["Extent of Observed Water", "#9EBBD7"],
        ["Riparian Management Zone", "#CDF57A"],
        ["Floodplain", "#9ED7C2"],
    ]);

    // Value formatting inputs: can be a canned format name or a custom function
    // canned names: 'numeric' | 'decimal' | 'currency' | 'percent'
    @Input() valueFormatter?: string | ((v: number, datum?: ChartDatum) => string);
    @Input() valueFractionDigits: number = 2;

    // Optional target line inputs (kept for backward compatibility: single fixed target)
    @Input() targetValue?: number | null = null; // if set, draw a horizontal rule at this y-value
    @Input() targetLabel?: string | null = null; // optional label used in tooltip
    // Default target color changed from red to green (non-negative default)
    @Input() targetColor: string = "#2E8B57";
    @Input() targetStrokeWidth: number = 2;
    @Input() targetStrokeDash?: number[] | null = [4, 4];

    // New inputs: whether to show point markers and rendering style for the target
    @Input() showTargetPoints: boolean = false;
    @Input() targetStyle: "solid" | "dashed" = "solid";
    // Pixel size for tick marks when rendering target as ticks/points
    @Input() targetTickSize: number = 250;
    // New input: `targetDisplay` (trend | markers).
    // targetDisplay: 'trend' = connected line across X; 'markers' = isolated per-X markers
    @Input() targetDisplay?: "trend" | "markers";

    // Sentinel Group name used to mark target rows if targets are appended to chartData
    @Input() targetSentinelGroup: string = "TARGET";

    // Optional override: force X axis to be treated as temporal (true) or nominal (false).
    // If undefined, component will auto-detect.
    @Input() isTemporalX?: boolean;

    // If true, show a cumulative toggle control on the chart and default to cumulative values
    // When false the toggle is hidden and the chart stays non-cumulative
    @Input() showCumulativeToggle: boolean = false;

    // Local toggle state (bindable from template) so consumers can flip without changing input binding
    // When `showCumulativeToggle` is true the default state will be cumulative (true).
    public isCumulativeLocal: boolean = false;
    // simple local checkbox value used by the inline toggle UI
    public cumulativeCheckbox: boolean = false;

    // Internal: chart data stripped of sentinel target rows and extracted target series
    private _chartDataForSpec: ChartDatumAugmented[] = [];
    private _targetSeries: Array<{ XValue: any; Target: number }> = [];
    // track last applied target tick size to avoid recursive re-render
    private _lastTargetTickSize: number | null = null;
    // pending RAF id used to throttle tooltip moves
    private _pendingRaf: number | null = null;
    // whether XValue should be treated as temporal (year/date) or nominal (string/categories)
    private _isTemporalX: boolean = false;

    ngOnChanges() {
        // Prepare internal data and render. If `showCumulativeToggle` is true we default to cumulative mode;
        // otherwise the chart remains non-cumulative and the toggle is hidden.
        this.isCumulativeLocal = !!this.showCumulativeToggle;
        this.cumulativeCheckbox = this.isCumulativeLocal;
        this.prepareChartDataForSpec(this.isCumulativeLocal);
        const vegaSpec = this.buildVegaSpec();
        this.renderChart(vegaSpec);
    }

    ngOnDestroy(): void {
        // Remove tooltip DOM and detach Vega view listeners and renderer DOM listeners
        this._removeVegaEventListeners();
        if (this._tooltipEl) {
            this._tooltipEl.remove();
            this._tooltipEl = null;
        }
    }

    // Remove previously-registered Vega event listeners
    private _removeVegaEventListeners() {
        for (const rec of this._vegaEventHandlers) {
            if (rec.view && typeof rec.view.removeEventListener === "function") {
                rec.view.removeEventListener(rec.type, rec.handler);
            }
        }
        this._vegaEventHandlers.length = 0;
        // remove any DOM listeners created via renderer.listen
        for (const remover of this._domListenerRemovers) {
            remover();
        }
        this._domListenerRemovers.length = 0;
        // cancel any pending tooltip RAF
        if (this._pendingRaf) {
            cancelAnimationFrame(this._pendingRaf as any);
            this._pendingRaf = null;
        }
    }

    // Position tooltip near the provided client coordinates, with collision handling
    private positionTooltip(clientX: number, clientY: number) {
        if (!this._tooltipEl) {
            return;
        }
        const tooltip = this._tooltipEl as HTMLDivElement;
        const padding = 12;
        const tooltipRect = tooltip.getBoundingClientRect();
        const vpWidth = window.innerWidth;
        const vpHeight = window.innerHeight;
        let left = clientX + padding;
        let top = clientY + padding;
        if (left + tooltipRect.width + padding > vpWidth) {
            left = clientX - tooltipRect.width - padding;
        }
        if (top + tooltipRect.height + padding > vpHeight) {
            top = clientY - tooltipRect.height - padding;
        }
        const tx = Math.max(8, left);
        const ty = Math.max(8, top);
        tooltip.style.transform = `translate(${tx}px, ${ty}px)`;
    }

    // Prepare internal _chartDataForSpec and _targetSeries from chartData; strip sentinel target rows.
    // If useCumulative is true, convert YValue into cumulative sums across increasing XValue order.
    private prepareChartDataForSpec(useCumulative: boolean) {
        this._chartDataForSpec = [];
        this._targetSeries = [];
        if (!this.chartData || !Array.isArray(this.chartData)) {
            return;
        }

        const sentinel = this.targetSentinelGroup ?? "TARGET";
        // Separate sentinel rows
        for (const d of this.chartData) {
            if (d && d.Group === sentinel && d.YValue != null) {
                this._targetSeries.push({ XValue: d.XValue, Target: d.YValue });
            } else {
                // shallow copy so we don't mutate original input objects
                this._chartDataForSpec.push(Object.assign({}, d) as ChartDatumAugmented);
            }
        }

        // Determine whether XValue should be treated as temporal (year/date/number) or nominal (string categories)
        const uniqueXUnsorted = Array.from(new Set(this._chartDataForSpec.map((d) => d.XValue)));
        let temporalGuess = true;
        for (const v of uniqueXUnsorted) {
            if (v == null) {
                continue;
            }
            if (typeof v === "number") {
                continue;
            }
            const s = String(v).trim();
            if (/^\d{4}$/.test(s)) {
                continue; // 4-digit year
            }
            const parsed = Date.parse(s);
            if (!isNaN(parsed)) {
                continue;
            }
            temporalGuess = false;
            break;
        }
        // Allow consumer override via @Input() isTemporalX
        this._isTemporalX = typeof this.isTemporalX === "boolean" ? !!this.isTemporalX : temporalGuess;

        // If cumulative requested, compute cumulative sums per group across sorted XValue keys.
        if (useCumulative) {
            const uniqueX = this._isTemporalX
                ? uniqueXUnsorted.slice().sort((a: any, b: any) => {
                      const na = Number(a);
                      const nb = Number(b);
                      if (!isNaN(na) && !isNaN(nb)) {
                          return na - nb;
                      }
                      return String(a).localeCompare(String(b));
                  })
                : uniqueXUnsorted;

            // For each group label, compute cumulative sum across increasing X values
            const groups = Array.from(new Set(this._chartDataForSpec.map((d) => d.Group)));
            const runningByGroup: Record<string, number> = {};
            for (const g of groups) {
                runningByGroup[g] = 0;
            }

            for (const x of uniqueX) {
                const rows = this._chartDataForSpec.filter((d) => d.XValue === x);
                for (const r of rows) {
                    const prev = r.Group ? runningByGroup[r.Group] ?? 0 : runningByGroup[""] ?? 0;
                    const add = r.YValue ?? 0;
                    const cumul = prev + add;
                    runningByGroup[r.Group ?? ""] = cumul;
                    // write back cumulative value on typed datum
                    (r as ChartDatumAugmented).YValue = cumul;
                }
            }

            // Also make target series cumulative across sorted X values if present
            if (this._targetSeries && this._targetSeries.length) {
                const targetByX = this._isTemporalX
                    ? this._targetSeries.slice().sort((a, b) => {
                          const na = Number(a.XValue);
                          const nb = Number(b.XValue);
                          if (!isNaN(na) && !isNaN(nb)) {
                              return na - nb;
                          }
                          return String(a.XValue).localeCompare(String(b.XValue));
                      })
                    : this._targetSeries.slice();

                let runningTarget = 0;
                for (const t of targetByX) {
                    runningTarget += t.Target ?? 0;
                    t.Target = runningTarget;
                }

                const mapByX: Record<string, number> = {};
                for (const t of targetByX) {
                    mapByX[String(t.XValue)] = t.Target;
                }
                this._targetSeries = this._targetSeries.map((t) => ({ XValue: t.XValue, Target: mapByX[String(t.XValue)] ?? t.Target }));
            }
        }

        // Compute BreakdownRows/Total per XValue
        const byYear = new Map<number | string, any[]>();
        this._chartDataForSpec.forEach((d) => {
            const key = d.XValue;
            if (!byYear.has(key)) {
                byYear.set(key, []);
            }
            byYear.get(key).push(d);
        });

        byYear.forEach((arr, year) => {
            const total = arr.reduce((s, a) => s + (a.YValue ?? 0), 0);
            const rowsWithValues = arr.filter((a) => a.YValue != null);
            const rows = rowsWithValues.map((a) => ({ label: a.Group, value: a.YValue }));
            arr.forEach((a) => {
                const aa = a as ChartDatumAugmented;
                aa.BreakdownRows = rows;
                aa.BreakdownTotal = total;
                aa.BreakdownYear = year;
            });
        });
    }

    // Public method to toggle cumulative mode from UI; re-prepares data and re-renders
    public toggleCumulative() {
        this.toggleCumulativeInline();
    }

    // Inline toggle handler without reactive forms
    public toggleCumulativeInline() {
        this.cumulativeCheckbox = !this.cumulativeCheckbox;
        this.isCumulativeLocal = this.cumulativeCheckbox;
        this.prepareChartDataForSpec(this.isCumulativeLocal);
        const spec = this.buildVegaSpec();
        this.renderChart(spec);
    }

    // Helper to format numeric values according to input settings
    private formatValue(v: number | null | undefined, datum?: ChartDatum): string {
        if (v == null) {
            return "";
        }
        // If a custom function is provided, use it (call with a typed signature)
        if (typeof this.valueFormatter === "function") {
            const vf = this.valueFormatter as (v: number, datum?: ChartDatum) => string;
            return vf(v, datum);
        }

        const frac = Math.max(0, Math.floor(this.valueFractionDigits ?? 0));
        const fmtName = typeof this.valueFormatter === "string" ? (this.valueFormatter as string) : "numeric";
        const digits = `1.${frac}-${frac}`;
        if (fmtName === "percent") {
            // PercentPipe expects a fraction (0.12 => 12%) — input should be raw fraction
            return this.percentPipe.transform(v, digits) ?? String(v);
        }
        if (fmtName === "currency") {
            return this.currencyPipe.transform(v, "USD", "symbol", digits) ?? String(v);
        }
        if (fmtName === "number") {
            return this.decimalPipe.transform(v, digits) ?? String(v);
        }
        // numeric / decimal
        return this.decimalPipe.transform(v, digits) ?? String(v);
    }

    // Build the vega spec; encoding includes opacity conditioned on hiddenGroups
    private buildVegaSpec(): VisualizationSpec {
        const hidden = [...this.hiddenGroups];

        const spec: any = {
            $schema: "https://vega.github.io/schema/vega-lite/v6.json",
            width: "container",
            height: this.chartHeight,
            autosize: { type: "fit-x", resize: true },
            config: {
                view: { stroke: null },
                axisY: {
                    labelFont: "Arial",
                    labelFontSize: 14,
                    labelFontWeight: "bold",
                    labelPadding: 10,
                    offset: 2,
                    ticks: false,
                    domainColor: "#dcdcdc",
                },
                axisX: {
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
            data: { name: "table", values: this._chartDataForSpec },
            transform: this._isTemporalX ? [{ calculate: "datetime(datum.XValue, 0, 1)", as: "XYear" }] : [],
            encoding: {
                x: this._isTemporalX
                    ? { field: "XYear", title: "", sort: { field: "SortOrder" }, type: "temporal", timeUnit: "year", axis: { labelAngle: 0, format: "%Y", labelOpacity: 1 } }
                    : { field: "XValue", title: "", sort: { field: "SortOrder" }, type: "nominal", axis: { labelAngle: 0, labelOpacity: 1 } },
                y: { field: "YValue", title: this.yFieldName, type: "quantitative", axis: { gridDash: { value: [8, 8] } }, stack: "zero" },
                color: {
                    sort: "ascending",
                    field: "Group",
                    title: "",
                    type: "nominal",
                    scale: { domain: [...this.colorRange.keys()], range: [...this.colorRange.values()] },
                },
                order: { field: "SortOrder" },
                // opacity: hide groups that are toggled off
                opacity: {
                    condition: {
                        test: `indexof(${JSON.stringify(hidden)}, datum.Group) === -1`,
                        value: 1,
                    },
                    value: 0.08,
                },
                tooltip: null,
            },
            // combined layers: bars + precomputed label layer
            layer: [{ mark: { type: "bar", cornerRadius: 6, stroke: "#cecece" } }],
        };

        // Precompute per-X totals and target mapping so caps can be placed above the taller of the stack or the target
        const totalsMap: Record<string, number> = {};
        const repXValue: Record<string, any> = {};
        for (const d of this._chartDataForSpec) {
            const k = String(d.XValue);
            if (!(k in repXValue)) {
                repXValue[k] = d.XValue;
            }
            if (d.BreakdownTotal != null) {
                totalsMap[k] = d.BreakdownTotal as number;
            } else {
                totalsMap[k] = (totalsMap[k] ?? 0) + (d.YValue ?? 0);
            }
        }
        const targetMap: Record<string, number> = {};
        for (const t of this._targetSeries || []) {
            const k = String(t.XValue);
            if (!(k in repXValue)) {
                repXValue[k] = t.XValue;
            }
            targetMap[k] = t.Target ?? 0;
        }
        // precompute per-X totals; targetMap already contains per-X target values

        // If a target series was extracted from the chartData, append a line layer that follows year->target
        if (this._targetSeries && this._targetSeries.length > 0) {
            const targetData = this.makeTargetData(this._targetSeries || []);
            const effectiveDisplay: "trend" | "markers" | undefined = this.targetDisplay;
            const showTrend = effectiveDisplay ? effectiveDisplay === "trend" : true;
            const showMarkers = effectiveDisplay ? effectiveDisplay === "markers" : !!this.showTargetPoints;

            if (showTrend) {
                this.addTrendLayer(spec, targetData);
            }
            if (showMarkers) {
                this.addMarkerLayers(spec, targetData);
            }
        }

        return spec as VisualizationSpec;
    }

    // Small helper to escape HTML text when injecting into tooltip markup
    private escapeHtml(input: any): string {
        if (input == null) {
            return "";
        }
        const s = String(input);
        return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/\"/g, "&quot;").replace(/'/g, "&#039;");
    }

    // Private helper: return x encoding object for layers (temporal vs nominal)
    private xEncoding(useTemporal: boolean, field: string, axisNull = true) {
        return useTemporal
            ? { x: { field, type: "temporal", timeUnit: "year", axis: axisNull ? null : { labelAngle: 0 } } }
            : { x: { field, type: "nominal", axis: axisNull ? null : { labelAngle: 0 } } };
    }

    // Private helper: prepare target data block (includes datetime transform when needed)
    private makeTargetData(series: Array<any>) {
        const labelledTargetSeries = (series || []).map((t) => ({ ...t, LabelText: this.formatValue(t.Target) }));
        return this._isTemporalX
            ? { data: { values: labelledTargetSeries }, transform: [{ calculate: "datetime(datum.XValue, 0, 1)", as: "XYear" }] }
            : { data: { values: labelledTargetSeries } };
    }

    // Private helper: add trend (line + final-value label) layers
    private addTrendLayer(specObj: any, targetDataObj: any) {
        const lineMark: any = {
            type: "line",
            stroke: this.targetColor,
            strokeWidth: Math.max(3, this.targetStrokeWidth || 0),
            opacity: 0.95,
            interpolate: "monotone",
        };
        if (this.targetStyle === "dashed") {
            lineMark.strokeDash = this.targetStrokeDash && this.targetStrokeDash.length ? this.targetStrokeDash : [4, 4];
        }
        const xEncForMarks = this._isTemporalX ? this.xEncoding(true, "XYear", true) : this.xEncoding(false, "XValue", true);
        const lineLayer: any = {
            ...targetDataObj,
            mark: lineMark,
            encoding: {
                ...xEncForMarks,
                y: { field: "Target", type: "quantitative" },
            },
        };
        specObj.layer.push(lineLayer);

        // final value label (halo + main)
        let latest = this._targetSeries[0];
        for (let i = 1; i < this._targetSeries.length; i++) {
            const a = Number(this._targetSeries[i].XValue);
            const b = Number(latest.XValue);
            if (!isNaN(a) && !isNaN(b) ? a > b : String(this._targetSeries[i].XValue) > String(latest.XValue)) {
                latest = this._targetSeries[i];
            }
        }

        const haloLayer: any = {
            data: { values: [latest] },
            transform: this._isTemporalX ? [{ calculate: "datetime(datum.XValue, 0, 1)", as: "XYear" }] : [],
            mark: {
                type: "text",
                dy: -18,
                align: "center",
                baseline: "bottom",
                fontWeight: "bold",
                fontSize: 13,
                fill: "#ffffff",
                stroke: "#ffffff",
                strokeWidth: 3,
            },
            encoding: {
                ...(this._isTemporalX ? this.xEncoding(true, "XYear", true) : this.xEncoding(false, "XValue", true)),
                y: { field: "Target", type: "quantitative" },
                text: { value: this.formatValue(latest.Target) },
            },
        };
        const mainLayer: any = {
            data: { values: [latest] },
            transform: this._isTemporalX ? [{ calculate: "datetime(datum.XValue, 0, 1)", as: "XYear" }] : [],
            mark: {
                type: "text",
                dy: -18,
                align: "center",
                baseline: "bottom",
                fontWeight: "bold",
                fontSize: 13,
                fill: this.targetColor,
            },
            encoding: {
                ...(this._isTemporalX ? this.xEncoding(true, "XYear", true) : this.xEncoding(false, "XValue", true)),
                y: { field: "Target", type: "quantitative" },
                text: { value: this.formatValue(latest.Target) },
            },
        };
        specObj.layer.push(haloLayer);
        specObj.layer.push(mainLayer);
    }

    // Private helper: add marker (tick + per-point labels) layers
    private addMarkerLayers(specObj: any, targetDataObj: any) {
        const xEncForPoints = this._isTemporalX ? this.xEncoding(true, "XYear", true) : this.xEncoding(false, "XValue", true);
        const pointLayer: any = {
            ...targetDataObj,
            mark: { type: "tick", filled: true, size: this.targetTickSize, color: this.targetColor, stroke: this.targetColor, strokeWidth: 1.5 },
            encoding: {
                ...xEncForPoints,
                y: { field: "Target", type: "quantitative" },
                size: { value: this.targetTickSize },
            },
        };
        specObj.layer.push(pointLayer);

        const haloLayer: any = {
            ...targetDataObj,
            mark: { type: "text", dy: -4, align: "center", baseline: "bottom", fontWeight: "bold", fontSize: 12, fill: "#ffffff", stroke: "#ffffff", strokeWidth: 3 },
            encoding: {
                ...xEncForPoints,
                y: { field: "Target", type: "quantitative" },
                text: { field: "LabelText", type: "nominal" },
            },
        };
        const mainLayer: any = {
            ...targetDataObj,
            mark: { type: "text", dy: -4, align: "center", baseline: "bottom", fontWeight: "bold", fontSize: 12, fill: this.targetColor },
            encoding: {
                ...xEncForPoints,
                y: { field: "Target", type: "quantitative" },
                text: { field: "LabelText", type: "nominal" },
            },
        };
        specObj.layer.push(haloLayer);
        specObj.layer.push(mainLayer);
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

    // Create tooltip element and attach event handlers to the provided vega view
    private setupTooltipForView(view: any) {
        // Create tooltip element (Renderer2) and append to document.body to avoid clipping;
        // initialize transform baseline and ARIA attributes
        const tooltip = this.renderer.createElement("div") as HTMLDivElement;
        this.renderer.addClass(tooltip, "vstack-tooltip");
        // accessibility: give tooltip a stable id and mark hidden initially
        const tooltipId = `${this.chartID || "chart"}-vstack-tooltip`;
        this.renderer.setAttribute(tooltip, "id", tooltipId);
        this.renderer.setAttribute(tooltip, "role", "tooltip");
        this.renderer.setAttribute(tooltip, "aria-hidden", "true");
        this._tooltipEl = tooltip;
        // append to document body via renderer
        this.renderer.appendChild(document.body, tooltip);
        // initialize inline positioning baseline (left/top/transform/will-change); allow errors to propagate
        (tooltip as HTMLElement).style.left = "0px";
        (tooltip as HTMLElement).style.top = "0px";
        (tooltip as HTMLElement).style.transform = "translate(0px, 0px)";
        (tooltip as HTMLElement).style.willChange = "transform";

        // Ensure the chart container is keyboard-focusable and wired to the tooltip by ARIA
        const hostEl = this._el && this._el.nativeElement ? (this._el.nativeElement as HTMLElement) : null;
        if (hostEl) {
            // set aria-describedby to point to the tooltip
            this.renderer.setAttribute(hostEl, "aria-describedby", tooltipId);
            // ensure focusable
            if (!hostEl.hasAttribute("tabindex")) {
                this.renderer.setAttribute(hostEl, "tabindex", "0");
            }

            // listen for focus to show a summary tooltip and blur to hide
            const focusRemover = this.renderer.listen(hostEl, "focus", (ev: FocusEvent) => {
                if (!this._tooltipEl) {
                    return;
                }
                // mark tooltip visible to assistive tech
                this.renderer.setAttribute(this._tooltipEl, "aria-hidden", "false");
                this.renderer.setAttribute(hostEl, "aria-expanded", "true");
                // if tooltip has no content, put a minimal label
                if (this._tooltipEl && !this._tooltipEl.innerHTML) {
                    this._tooltipEl.innerHTML = `<div class='vstack-tooltip-inner'>${this.escapeHtml(this.xFieldName)} ${this.escapeHtml(this.yFieldName)}</div>`;
                }
                // make tooltip visible
                (this._tooltipEl as HTMLElement).style.display = "block";
            });
            this._domListenerRemovers.push(focusRemover);

            const blurRemover = this.renderer.listen(hostEl, "blur", (ev: FocusEvent) => {
                if (!this._tooltipEl) {
                    return;
                }
                this.renderer.setAttribute(this._tooltipEl, "aria-hidden", "true");
                this.renderer.setAttribute(hostEl, "aria-expanded", "false");
                (this._tooltipEl as HTMLElement).style.display = "none";
            });
            this._domListenerRemovers.push(blurRemover);

            // keyboard: hide tooltip on Escape
            const keyRemover = this.renderer.listen(hostEl, "keydown", (ev: KeyboardEvent) => {
                if (ev.key === "Escape" || ev.key === "Esc") {
                    if (!this._tooltipEl) {
                        return;
                    }
                    this.renderer.setAttribute(this._tooltipEl, "aria-hidden", "true");
                    this.renderer.setAttribute(hostEl, "aria-expanded", "false");
                    (this._tooltipEl as HTMLElement).style.display = "none";
                }
            });
            this._domListenerRemovers.push(keyRemover);
            // Listeners registered via renderer.listen are stored in _domListenerRemovers for cleanup
            // in _removeVegaEventListeners()/ngOnDestroy
        }

        const onMouseOver = (event: any, item: any) => {
            // Find the deepest child item that contains a datum (some marks are grouped)
            const findDeepestWithDatum = (it: any): { item: any; depth: number } | null => {
                if (!it) {
                    return null;
                }
                let best: { item: any; depth: number } | null = it && it.datum ? { item: it, depth: 0 } : null;
                if (Array.isArray(it.items) && it.items.length) {
                    for (let i = 0; i < it.items.length; i++) {
                        const childBest = findDeepestWithDatum(it.items[i]);
                        if (childBest) {
                            const childDepth = childBest.depth + 1;
                            if (!best || childDepth > best.depth) {
                                best = { item: childBest.item, depth: childDepth };
                            }
                        }
                    }
                }
                return best;
            };

            const leafRes = findDeepestWithDatum(item);
            const leafItem = leafRes ? leafRes.item : item;
            const datum = leafItem && leafItem.datum ? leafItem.datum : null;
            const typedDatum = datum as ChartDatumAugmented | { Target?: number } | null as ChartDatumAugmented | { Target?: number } | null;
            if (!datum) {
                return;
            }

            // helper: normalize a datum X value/date/year into a canonical 4-digit year string where possible
            const normalizeYear = (v: any): string | null => {
                if (v == null) {
                    return null;
                }
                if (typeof v === "number") {
                    return String(v);
                }
                if (v instanceof Date) {
                    return String(v.getFullYear());
                }
                if (typeof v === "string") {
                    const s = v.trim();
                    if (/^\d{4}$/.test(s)) {
                        return s;
                    }
                    const parsed = Date.parse(s);
                    if (!isNaN(parsed)) {
                        return String(new Date(parsed).getFullYear());
                    }
                    const asNum = Number(s);
                    if (!isNaN(asNum)) {
                        return String(asNum);
                    }
                    return s;
                }
                const asNum = Number(v);
                if (!isNaN(asNum)) {
                    return String(asNum);
                }
                return String(v);
            };

            // Prefer canonical BreakdownRows if present, else use BreakdownHtml/text as fallback
            let innerHtml = "";
            let ariaText = "";

            // Prevent tooltip on blank space: require the event target to be inside the leaf item's SVG node (if present)
            const svgNode = leafItem && leafItem._svg ? leafItem._svg : null;
            if (svgNode && event && event.target && typeof svgNode.contains === "function") {
                if (!svgNode.contains(event.target)) {
                    return;
                }
            }

            // Only show tooltips for meaningful data rows
            const hasTarget = typedDatum && typeof (typedDatum as any).Target !== "undefined" && (typedDatum as any).Target != null;
            const hasY = typedDatum && typeof (typedDatum as any).YValue !== "undefined" && (typedDatum as any).YValue != null;
            const hasBreakdown = typedDatum && (typedDatum as ChartDatumAugmented).BreakdownRows && (typedDatum as ChartDatumAugmented).BreakdownRows!.length > 0;
            const hasMeaningfulDatum = !!(typedDatum && (hasTarget || hasY || hasBreakdown));
            if (!hasMeaningfulDatum) {
                return;
            }
            // Skip tooltip for target datums (line/points) — keep tooltips on bars only
            if (typedDatum && hasTarget) {
                return;
            }

            if (typedDatum && (typedDatum as ChartDatumAugmented).BreakdownRows) {
                const rows = (typedDatum as ChartDatumAugmented).BreakdownRows as Array<{ label: string; value: number }>;
                const year = (typedDatum as ChartDatumAugmented).BreakdownYear;
                const total = (typedDatum as ChartDatumAugmented).BreakdownTotal;
                const sep = '<div class="breakdown-sep"></div>';
                innerHtml =
                    this.buildBreakdownHtmlFromRows(year, rows) +
                    sep +
                    `<div class="breakdown-row"><span class="breakdown-label breakdown-label-bold">Total</span><span class="breakdown-value">${this.formatValue(
                        total
                    )}</span></div>` +
                    sep;
                ariaText = this.buildBreakdownTextFromRows(year, rows, total);

                // Append per-year target (if present) or fallback targetValue after the total
                let targetVal: number | null | undefined = null;
                if (this._targetSeries && Array.isArray(this._targetSeries)) {
                    const normYear = normalizeYear(year);
                    const found = this._targetSeries.find((t) => normalizeYear(t.XValue) === normYear);
                    if (found) {
                        targetVal = found.Target;
                    }
                }
                if (targetVal == null && this.targetValue != null) {
                    targetVal = this.targetValue;
                }
                if (targetVal != null) {
                    innerHtml += `<div class="breakdown-row"><span class="breakdown-label breakdown-label-bold">Target</span><span class="breakdown-value">${this.formatValue(
                        targetVal
                    )}</span></div>`;
                    ariaText += `\nTarget: ${this.formatValue(targetVal)}`;
                }
            } else if (this.chartData) {
                const rawKey = datum && datum.XValue != null ? datum.XValue : datum && (datum.XYear ?? datum.BreakdownYear) != null ? datum.XYear ?? datum.BreakdownYear : null;
                const normYear = normalizeYear(rawKey);
                const groupForKey = normYear != null ? this.chartData.filter((d) => normalizeYear(d.XValue) === normYear) : [];
                const rows = groupForKey.filter((a) => a.YValue != null).map((a) => ({ label: a.Group, value: a.YValue }));
                const year = rawKey;
                const total = groupForKey.reduce((s, a) => s + (a.YValue ?? 0), 0);
                const sep2 = '<div class="breakdown-sep"></div>';
                innerHtml =
                    this.buildBreakdownHtmlFromRows(year, rows) +
                    sep2 +
                    `<div class="breakdown-total-sep"><div class="breakdown-row"><span class="breakdown-label breakdown-label-bold">Total</span><span class="breakdown-value">${this.formatValue(
                        total
                    )}</span></div></div>` +
                    sep2;
                ariaText = this.buildBreakdownTextFromRows(year, rows, total);

                let targetVal2: number | null | undefined = null;
                if (this._targetSeries && Array.isArray(this._targetSeries)) {
                    const found = this._targetSeries.find((t) => normalizeYear(t.XValue) === normYear);
                    if (found) {
                        targetVal2 = found.Target;
                    }
                }
                if (targetVal2 == null && this.targetValue != null) {
                    targetVal2 = this.targetValue;
                }
                if (targetVal2 != null) {
                    innerHtml += `<div class="breakdown-row"><span class="breakdown-label breakdown-label-bold">Target</span><span class="breakdown-value">${this.formatValue(
                        targetVal2
                    )}</span></div>`;
                    ariaText += `\nTarget: ${this.formatValue(targetVal2)}`;
                }
            } else {
                innerHtml = "";
                ariaText = "";
            }
            tooltip.innerHTML = `<div class='vstack-tooltip-inner'>${innerHtml}</div>`;
            tooltip.setAttribute("aria-label", ariaText);
            this.renderer.setAttribute(tooltip, "aria-hidden", "false");
            if (hostEl) {
                this.renderer.setAttribute(hostEl, "aria-expanded", "true");
            }
            tooltip.style.display = "block";
            // Position tooltip using shared helper
            this.positionTooltip(event.clientX, event.clientY);
        };
        view.addEventListener("mouseover", onMouseOver);
        this._vegaEventHandlers.push({ view, type: "mouseover", handler: onMouseOver });

        const onMouseMove = (event: any) => {
            if (!this._tooltipEl) {
                return;
            }
            // Throttle moves with requestAnimationFrame
            if (this._pendingRaf) {
                cancelAnimationFrame(this._pendingRaf);
                this._pendingRaf = null;
            }
            this._pendingRaf = requestAnimationFrame(() => {
                this.positionTooltip(event.clientX, event.clientY);
                this._pendingRaf = null;
            }) as unknown as number;
        };
        view.addEventListener("mousemove", onMouseMove);
        this._vegaEventHandlers.push({ view, type: "mousemove", handler: onMouseMove });

        const onMouseOut = () => {
            if (!this._tooltipEl) {
                return;
            }
            this.renderer.setAttribute(this._tooltipEl, "aria-hidden", "true");
            if (hostEl) {
                this.renderer.setAttribute(hostEl, "aria-expanded", "false");
            }
            this._tooltipEl.style.display = "none";
        };
        view.addEventListener("mouseout", onMouseOut);
        this._vegaEventHandlers.push({ view, type: "mouseout", handler: onMouseOut });
    }

    // Build plain-text fallback from canonical rows
    private buildBreakdownTextFromRows(year: any, rows: Array<{ label: string; value: number }>, total: number): string {
        const lines = rows.map((r) => `${r.label}: ${this.formatValue(r.value)}`);
        return lines.length ? `${year}\n${lines.join("\n")}\nTotal: ${this.formatValue(total)}` : `${year}\nTotal: ${this.formatValue(total)}`;
    }

    private renderChart(spec: VisualizationSpec) {
        // Clean up any previous tooltip element and remove vega listeners
        if (this._tooltipEl) {
            // use renderer to remove child from body where possible
            this.renderer.removeChild(document.body, this._tooltipEl);
            // fallback to direct remove if renderer removal somehow didn't remove it
            if ((this._tooltipEl as any).remove && (this._tooltipEl as any).parentNode) {
                this._tooltipEl.remove();
            }
            this._tooltipEl = null;
        }
        // Remove listeners attached to previous views and finalize previous view
        this._removeVegaEventListeners();
        if (this._viewRef && typeof this._viewRef.finalize === "function") {
            this._viewRef.finalize();
            this._viewRef = null;
        }

        vegaEmbed(`#${this.chartID}`, spec, { renderer: "svg" }).then((res) => {
            // Narrow the embed result's view into a minimal-typed object for local use
            const embedResult = res as { view: { scale?: (name: string) => any; finalize?: () => void; addEventListener?: (type: string, handler: any) => void } };
            const view = embedResult.view;
            this._viewRef = view;

            // Delay slightly to ensure scales are initialized; run in next microtask
            Promise.resolve().then(() => {
                const xscale = view.scale && typeof view.scale === "function" ? view.scale("x") : null;
                let band: number | null = null;
                if (xscale) {
                    if (typeof xscale.bandwidth === "function") {
                        band = xscale.bandwidth();
                    } else if (xscale.bandwidth != null) {
                        band = xscale.bandwidth;
                    }
                }
                const finalSize = Math.max(6, Math.round(band ?? this.targetTickSize));
                // If the computed size differs from current input and we haven't already applied this size
                // (tracked by _lastTargetTickSize), update once and re-render to apply sizes.
                if (finalSize !== this.targetTickSize && finalSize !== this._lastTargetTickSize) {
                    this._lastTargetTickSize = finalSize;
                    this.targetTickSize = finalSize;
                    // Rebuild spec and re-render chart once with updated size
                    const spec2 = this.buildVegaSpec();
                    this.renderChart(spec2);
                }
            });

            // Create tooltip element and attach handlers
            this.setupTooltipForView(view);
        });
    }

    // Toggle a group's visibility and re-render the chart
    public toggleGroup(groupKey: string) {
        if (this.hiddenGroups.has(groupKey)) {
            this.hiddenGroups.delete(groupKey);
        } else {
            this.hiddenGroups.add(groupKey);
        }
        // Re-render chart with updated hiddenGroups
        const spec = this.buildVegaSpec();
        this.renderChart(spec);
    }
}
