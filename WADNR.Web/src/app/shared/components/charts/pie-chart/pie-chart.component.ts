import { Component, Input, OnChanges, OnDestroy, ElementRef, Output, EventEmitter } from "@angular/core";
import { CurrencyPipe, DecimalPipe } from "@angular/common";
import { LoadingDirective } from "../../../directives/loading.directive";
import { DefaultOrderKeyvaluePipe } from "src/app/shared/pipes/default-order-key-value.pipe";
import { default as vegaEmbed, VisualizationSpec } from "vega-embed";

/**
 * Reusable pie/donut chart component using vega-lite.
 * API mirrors common patterns from other chart components in the app.
 */
@Component({
    selector: "pie-chart",
    standalone: true,
    templateUrl: "./pie-chart.component.html",
    styleUrls: ["./pie-chart.component.scss"],
    imports: [DefaultOrderKeyvaluePipe, LoadingDirective],
    providers: [CurrencyPipe, DecimalPipe],
})
export class PieChartComponent implements OnChanges, OnDestroy {
    @Input() public chartID: string = "pie-chart";
    @Input() public chartData: Array<{ label: string; value: number }> = [];
    @Input() public height: number = 480;
    @Input() public innerRadius: number = 80; // donut inner radius in px; 0 => pie
    @Input() public isLoading: boolean = false;
    @Input() public colors: Map<string, string> = new Map();
    @Input() public showLegend: boolean = true;
    @Input() public valueFormatter?: (v: number) => string;
    /**
     * Optional currency code for declarative formatting (e.g. 'USD', 'CAD').
     * Used when `valueFormatter` is not provided.
     */
    @Input() public currency?: string;
    /**
     * Formatting style when using declarative formatting. 'currency' formats as currency using `currency` value.
     * 'number' formats as a plain number with thousands separators.
     */
    @Input() public formatStyle: "currency" | "number" = "currency";
    // computed legend totals by label for display next to legend entries
    public legendValues: Map<string, number> = new Map<string, number>();
    // computed center total (sum of all legendValues)
    public centerTotal: number = 0;
    public centerTotalFormatted: string = "";

    private _viewRef: any = null;
    private _tooltipEl: HTMLDivElement | null = null;
    @Output() public sliceClick: EventEmitter<any> = new EventEmitter<any>();
    // hidden legend groups (toggled off)
    public hiddenGroups: Set<string> = new Set<string>();

    constructor(private _el: ElementRef, private _currencyPipe: CurrencyPipe, private _decimalPipe: DecimalPipe) {}

    ngOnChanges(): void {
        // update derived legend totals for template
        this.updateLegendValues();
        const spec = this.buildSpec();
        this.render(spec);
    }

    private updateLegendValues() {
        this.legendValues = new Map<string, number>();
        if (!Array.isArray(this.chartData)) {
            return;
        }
        for (const d of this.chartData) {
            const key = d && d.label ? d.label : "Other";
            const val = Number(d && d.value ? d.value : 0) || 0;
            const prev = this.legendValues.get(key) ?? 0;
            this.legendValues.set(key, prev + val);
        }
        // compute center total
        let total = 0;
        for (const v of this.legendValues.values()) total += v;
        this.centerTotal = total;
        this.centerTotalFormatted = this.formatLegendValue(this.centerTotal);
    }

    public formatLegendValue(v?: number | null): string {
        if (v == null) {
            return "";
        }
        if (typeof this.valueFormatter === "function") {
            try {
                return this.valueFormatter(v);
            } catch {}
        }
        // declarative formatting: honor formatStyle and currency when provided
        try {
            if (this.formatStyle === "number") {
                // try DecimalPipe first
                try {
                    const s = this._decimalPipe.transform(v, "1.0-2");
                    if (s != null) {
                        return s;
                    }
                } catch {}
                // fallback: use DecimalPipe with default locale
                try {
                    const s2 = this._decimalPipe.transform(v, "1.0-2");
                    if (s2 != null) {
                        return s2;
                    }
                } catch {}
            }
            // currency style: try CurrencyPipe with provided currency or default
            const currencyCode = this.currency || "USD";
            try {
                const s = this._currencyPipe.transform(v, currencyCode, "symbol", "1.0-2");
                if (s != null) {
                    return s;
                }
            } catch {}
            // fallback: use CurrencyPipe with defaults
            try {
                const s2 = this._currencyPipe.transform(v, currencyCode, "symbol", "1.0-2");
                if (s2 != null) {
                    return s2;
                }
            } catch {}
        } catch {
            return String(v);
        }
    }

    ngOnDestroy(): void {
        try {
            if (this._tooltipEl) {
                this._tooltipEl.remove();
            }
        } catch {}
        try {
            if (this._viewRef && typeof this._viewRef.finalize === "function") {
                this._viewRef.finalize();
            }
        } catch {}
    }

    private buildSpec(): VisualizationSpec {
        const colorDomain = [...this.colors.keys()];
        const colorRange = [...this.colors.values()];

        // Add a joinaggregate transform to compute total and percentage per slice
        const hidden = [...this.hiddenGroups];

        const spec: any = {
            $schema: "https://vega.github.io/schema/vega-lite/v6.json",
            width: "container",
            height: this.height,
            data: { name: "table", values: this.chartData },
            transform: [{ joinaggregate: [{ op: "sum", field: "value", as: "_total" }] }, { calculate: "datum.value / datum._total", as: "_pct" }],
            encoding: {
                theta: { field: "value", type: "quantitative" },
                color: {
                    field: "label",
                    type: "nominal",
                    ...(colorDomain.length ? { scale: { domain: colorDomain, range: colorRange } } : {}),
                    // Internal Vega-Lite legend disabled: we render a custom external legend in the component template.
                    // This forces the Vega spec to not emit a built-in legend.
                    legend: null,
                },
                // opacity: hide slices when toggled off via legend
                opacity: {
                    condition: { test: `indexof(${JSON.stringify(hidden)}, datum.label) === -1`, value: 1 },
                    value: 0.08,
                },
            },
            layer: [
                {
                    mark: {
                        type: "arc",
                        outerRadius: null,
                        cornerRadius: 4,
                        // visible slice separators
                        stroke: "#ffffff",
                        strokeWidth: 1.5,
                        strokeLinejoin: "round",
                    },
                },
                // percent label layer inside slices
                {
                    mark: { type: "text", radius: this.innerRadius ? this.innerRadius + 24 : 0, size: 11, fill: "#000" },
                    encoding: {
                        text: { field: "_pct", type: "quantitative", format: ".0%" },
                        // ensure rendering color is black for the text
                        color: { value: "#000" },
                    },
                },
                // center total removed from Vega spec; rendered as an HTML overlay for better control
            ],
            view: { stroke: null },
            config: { axis: null, legend: { labelFontSize: 12 } },
        };

        // If innerRadius supplied, convert to donut by providing 'innerRadius' via mark config
        if (this.innerRadius && this.innerRadius > 0) {
            spec.layer[0].mark.innerRadius = this.innerRadius;
        }

        return spec as VisualizationSpec;
    }

    private render(spec: VisualizationSpec) {
        // remove previous tooltip
        if (this._tooltipEl) {
            this._tooltipEl.remove();
            this._tooltipEl = null;
        }

        // finalize previous view
        if (this._viewRef && typeof this._viewRef.finalize === "function") {
            try {
                this._viewRef.finalize();
            } catch {}
            this._viewRef = null;
        }

        vegaEmbed(`#${this.chartID}`, spec, { renderer: "svg" }).then((res: any) => {
            const view = res.view;
            this._viewRef = view;

            // basic tooltip element appended to body
            const tooltip = document.createElement("div");
            tooltip.className = "pie-tooltip";
            tooltip.style.position = "fixed";
            tooltip.style.pointerEvents = "none";
            tooltip.style.background = "white";
            tooltip.style.border = "1px solid rgba(0,0,0,0.08)";
            tooltip.style.padding = "6px 8px";
            tooltip.style.borderRadius = "6px";
            tooltip.style.boxShadow = "0 2px 6px rgba(0,0,0,0.12)";
            tooltip.style.zIndex = "1000";
            tooltip.style.display = "none";
            document.body.appendChild(tooltip);
            this._tooltipEl = tooltip;

            view.addEventListener("mousemove", (evt: any, item: any) => {
                try {
                    const leaf = item && item.datum ? item : null;
                    if (!leaf || !leaf.datum) {
                        return;
                    }
                    const d = leaf.datum;
                    tooltip.innerHTML = `<div class="pie-tooltip-inner"><strong>${String(d.label)}</strong>: ${this.formatLegendValue(Number(d.value) || 0)}</div>`;
                    tooltip.style.display = "block";
                    tooltip.style.left = evt.clientX + 12 + "px";
                    tooltip.style.top = evt.clientY + 12 + "px";
                } catch (e) {}
            });

            // Emit slice datum when clicked
            view.addEventListener("click", (evt: any, item: any) => {
                try {
                    const leaf = item && item.datum ? item : null;
                    if (!leaf || !leaf.datum) return;
                    const d = leaf.datum;
                    // Emit the datum to parent components
                    this.sliceClick.emit(d);
                } catch (e) {}
            });

            view.addEventListener("mouseout", () => {
                if (!this._tooltipEl) {
                    return;
                }
                this._tooltipEl.style.display = "none";
            });

            // center total is rendered by the component template as an overlay; no DOM post-processing required
        });
    }

    // Toggle a group's visibility and re-render the chart
    public toggleGroup(groupKey: string) {
        if (this.hiddenGroups.has(groupKey)) {
            this.hiddenGroups.delete(groupKey);
        } else this.hiddenGroups.add(groupKey);
        const spec = this.buildSpec();
        this.render(spec);
    }
}
