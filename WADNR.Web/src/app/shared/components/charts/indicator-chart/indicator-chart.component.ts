import { Component, Input, OnChanges, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IndicatorService } from "src/app/shared/generated/api/indicator.service";
import { FileResourceService } from "src/app/shared/generated/api/file-resource.service";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { getFileResourceUrl } from "src/app/shared/utils/file-resource-utils";
import { Observable, of } from "rxjs";
import { catchError, map, startWith, shareReplay } from "rxjs/operators";
import { VerticalStackedBarChartComponent } from "src/app/shared/components/charts/vertical-stacked-bar-chart/vertical-stacked-bar-chart.component";
import { BtnGroupRadioInputComponent } from "src/app/shared/components/inputs/btn-group-radio-input/btn-group-radio-input.component";
import { TrustHtmlPipe } from "src/app/shared/pipes/trust-html.pipe";
import { OneTimeSrcDirective } from "src/app/shared/directives/one-time-src.directive";
import { IndicatorChartTypeEnum } from "src/app/shared/generated/enum/indicator-chart-type-enum";

@Component({
    selector: "indicator-chart",
    templateUrl: "./indicator-chart.component.html",
    styleUrls: ["./indicator-chart.component.scss"],
    standalone: true,
    imports: [CommonModule, TrustHtmlPipe, VerticalStackedBarChartComponent, BtnGroupRadioInputComponent, OneTimeSrcDirective],
})
export class IndicatorChartComponent implements OnChanges {
    @Input() public indicatorID!: number;

    // Observable state for the chart DTO: undefined => loading, null => no-data, object => data
    public chartDto$: Observable<any> | null = null;

    // Selected subcategory index
    public selectedIndex = 0;

    // Tab state
    public selectedTab: string = "summary";

    // lightbox image
    public selectedImage: { src: SafeResourceUrl; alt?: string } | null = null;

    public tabOptionsCommon: Array<{ label: string; value: string }> = [
        { label: "Summary", value: "summary" },
        { label: "Sources", value: "sources" },
    ];

    // expose enum to the template
    public IndicatorChartTypeEnum = IndicatorChartTypeEnum;

    constructor(private indicatorService: IndicatorService, private fileResourceService: FileResourceService, private sanitizer: DomSanitizer) {}

    ngOnChanges(): void {
        this.loadChart();
    }

    private loadChart() {
        const id = this.indicatorID;
        if (!id && id !== 0) {
            this.chartDto$ = of(null);
            return;
        }

        this.chartDto$ = this.indicatorService.getChartDataIndicator(id).pipe(
            map((chartDto) => {
                if (chartDto && chartDto.IndicatorSubcategories && chartDto.IndicatorSubcategories.length > 0) {
                    const transformedArr = chartDto.IndicatorSubcategories.map((raw: any) => {
                        let colorRangeMap: Map<string, string> | null = null;
                        if (raw && raw.ColorRange) {
                            colorRangeMap = new Map<string, string>();
                            Object.keys(raw.ColorRange).forEach((k) => {
                                const v = raw.ColorRange[k];
                                if (v !== null && v !== undefined) {
                                    colorRangeMap!.set(k, v as string);
                                }
                            });
                        }
                        return Object.assign({}, raw, { ColorRange: colorRangeMap });
                    });
                    return Object.assign({}, chartDto, { IndicatorSubcategories: transformedArr });
                }
                return chartDto;
            }),
            catchError((err) => {
                console.error("Failed to load chart data for indicator", id, err);
                return of(null);
            }),
            startWith(undefined),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    public onTabChange(val: any) {
        try {
            this.selectedTab = String(val || "summary");
        } catch {
            this.selectedTab = "summary";
        }
    }

    public openImage(src: SafeResourceUrl | null, alt?: string) {
        if (!src) return;
        this.selectedImage = { src, alt };
    }

    public closeImage() {
        this.selectedImage = null;
    }

    public getFileResourceUrl(guid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrl(this.fileResourceService, this.sanitizer, guid);
    }

    public getSafeUrl(url?: string | null): SafeResourceUrl | null {
        if (!url) return null;
        try {
            return this.sanitizer.bypassSecurityTrustResourceUrl(url);
        } catch (e) {
            console.error("Failed to sanitize url", e);
            return null;
        }
    }
}
