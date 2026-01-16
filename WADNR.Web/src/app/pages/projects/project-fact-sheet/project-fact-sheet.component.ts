import { Component, OnInit, Input, SimpleChanges, HostListener } from "@angular/core";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { CommonModule, AsyncPipe } from "@angular/common";
import { RouterLink } from "@angular/router";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { Observable, of } from "rxjs";
import { switchMap, catchError, shareReplay, map, tap } from "rxjs/operators";
import { ProjectImageTimings } from "src/app/shared/generated/enum/project-image-timing-enum";
import { environment } from "src/environments/environment";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";
import { CapturePostData } from "src/app/shared/generated/model/capture-post-data";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { Title } from "@angular/platform-browser";
import { ProjectFactSheet } from "src/app/shared/generated/model/project-fact-sheet";
import { ProjectImage } from "src/app/shared/generated/model/project-image";
import { SitkaCaptureService } from "src/app/shared/generated/api/sitka-capture.service";
import { ClassificationLookupItem } from "src/app/shared/generated/model/classification-lookup-item";
import * as L from "leaflet";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { ProjectLocationsAsReferenceLayersComponent } from "src/app/shared/components/leaflet/orchestrators/project-locations-as-reference-layers/project-locations-as-reference-layers.component";

@Component({
    selector: "project-fact-sheet",
    standalone: true,
    templateUrl: "./project-fact-sheet.component.html",
    styleUrls: ["./project-fact-sheet.component.scss"],
    imports: [AlertDisplayComponent, CommonModule, AsyncPipe, RouterLink, ButtonLoadingDirective, WADNRMapComponent, ProjectLocationsAsReferenceLayersComponent],
})
export class ProjectFactSheetComponent implements OnInit {
    @Input() public projectID?: number | null;
    public project$!: Observable<ProjectFactSheet>;
    public images$!: Observable<Array<ProjectImage>>;
    public keyPhoto$!: Observable<ProjectImage | null>;
    public groupedImages$!: Observable<Array<{ timingId: number; timingName: string; images: ProjectImage[] }>>;
    public projectThemes$!: Observable<Array<ClassificationLookupItem>>;

    public selectedImage?: ProjectImage | null = null;
    // flat list of images displayed in the Photos section (excludes key photo)
    public displayedImages: ProjectImage[] = [];
    public selectedIndex: number = -1;
    public today: Date = new Date();

    public isLoadingPdf: boolean = false;
    public isPrintMode = false;

    public map?: L.Map;
    public layerControl?: any;

    constructor(private projectService: ProjectService, private sitkaCaptureService: SitkaCaptureService, private titleService: Title, private sanitizer: DomSanitizer) {}

    ngOnInit(): void {
        // Initial load if input was provided via withComponentInputBinding or embedding
        this.loadData(this.projectID ?? null);

        this.handlePrintMode();
    }

    public handleMapReady(event: WADNRMapInitEvent) {
        this.map = event.map;
        this.layerControl = event.layerControl;
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes["projectID"] && !changes["projectID"].firstChange) {
            this.loadData(this.projectID ?? null);
        }
    }

    private loadData(projectID?: number | null): void {
        this.project$ = this.projectService.getForFactSheetProject(projectID).pipe(
            tap((p) => {
                if (p) {
                    this.titleService.setTitle(`WADNR | ${p.ProjectName} | Fact Sheet`);
                }
            }),
            shareReplay(1)
        );
        // wire images$ to load when project is available
        this.images$ = this.project$.pipe(
            switchMap((p) => this.projectService.listImagesProject(p.ProjectID).pipe(catchError(() => of([] as Array<ProjectImage>)))),
            shareReplay(1)
        );

        // wire projectThemes$ to load when project is available
        this.projectThemes$ = this.project$.pipe(
            switchMap((p) => this.projectService.listClassificationsProject(p.ProjectID).pipe(catchError(() => of([])))),
            shareReplay(1)
        );

        this.keyPhoto$ = this.images$.pipe(
            map((images) => {
                if (!images || images.length === 0) {
                    return null;
                }
                const key = images.find((i) => !!i.IsKeyPhoto);
                return key ?? images[0];
            }),
            shareReplay(1)
        );

        this.groupedImages$ = this.images$.pipe(
            map((images) => {
                const imgs = images || [];
                // determine the key photo (IsKeyPhoto or first)
                const keyPhoto = imgs.find((i) => !!i.IsKeyPhoto) ?? imgs[0];
                // filter out excluded and the key photo itself so it isn't duplicated
                const filtered = imgs.filter((i) => !i.ExcludeFromFactSheet && i.ProjectImageID !== keyPhoto?.ProjectImageID);
                const groups: { [k: number]: ProjectImage[] } = {};
                for (const img of filtered) {
                    const key = img.ProjectImageTimingID ?? 0;
                    groups[key] = groups[key] || [];
                    groups[key].push(img);
                }
                // Convert to array with timing name resolution
                const result = Object.keys(groups).map((k) => ({
                    timingId: Number(k),
                    timingName: ProjectImageTimings.find((t) => t.Value === Number(k))?.DisplayName ?? "Other",
                    images: groups[Number(k)].sort((a, b) => {
                        // key photo first
                        if (a.IsKeyPhoto && !b.IsKeyPhoto) {
                            return -1;
                        }
                        if (!a.IsKeyPhoto && b.IsKeyPhoto) {
                            return 1;
                        }
                        // then by SortOrder if available
                        // const sa = a.SortOrder ?? 0;
                        // const sb = b.SortOrder ?? 0;
                        // return sa - sb;
                    }),
                }));
                // Custom order: Before (2) -> During (3) -> After (1) -> Unknown (4)
                const order = [2, 3, 1, 4];
                result.sort((x, y) => {
                    const xi = order.indexOf(x.timingId);
                    const yi = order.indexOf(y.timingId);
                    const xiSafe = xi === -1 ? order.length : xi;
                    const yiSafe = yi === -1 ? order.length : yi;
                    return xiSafe - yiSafe;
                });
                // update displayedImages flat list for keyboard navigation
                const flat: ProjectImage[] = [];
                for (const g of result) {
                    flat.push(...g.images);
                }
                this.displayedImages = flat;
                return result;
            }),
            shareReplay(1)
        );
    }

    public handlePrintMode() {
        this.isPrintMode = window.matchMedia("print").matches || navigator.userAgent.includes("HeadlessChrome"); // PDF generation services often use this

        // Listen for print events
        window.addEventListener("beforeprint", () => (this.isPrintMode = true));
        window.addEventListener("afterprint", () => (this.isPrintMode = false));

        // Listen for media query changes
        const printMediaQuery = window.matchMedia("print");
        printMediaQuery.addEventListener("change", (e) => (this.isPrintMode = e.matches));
    }

    public getMapHeight(): string {
        return this.isPrintMode ? "200px" : "480px";
    }

    // Build a file resource URL from a file-resource GUID string.
    // Caller should pass the FileResourceGuid value (string) from the DTO.

    public photoUrl(fileResourceGuid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, fileResourceGuid);
    }

    public openImage(img: ProjectImage) {
        this.selectedImage = img;
        // adjust selectedIndex to match the flattened displayedImages array
        const idx = this.displayedImages.findIndex((i) => i.ProjectImageID === img.ProjectImageID);
        this.selectedIndex = idx;
    }

    public closeImage() {
        this.selectedImage = null;
        this.selectedIndex = -1;
    }

    // navigate next/prev within displayedImages
    public nextImage() {
        if (!this.displayedImages || this.displayedImages.length === 0) {
            return;
        }
        if (this.selectedIndex < this.displayedImages.length - 1) {
            this.selectedIndex++;
        } else {
            this.selectedIndex = 0; // wrap
        }
        this.selectedImage = this.displayedImages[this.selectedIndex];
    }

    public prevImage() {
        if (!this.displayedImages || this.displayedImages.length === 0) {
            return;
        }
        if (this.selectedIndex > 0) {
            this.selectedIndex--;
        } else {
            this.selectedIndex = this.displayedImages.length - 1; // wrap
        }
        this.selectedImage = this.displayedImages[this.selectedIndex];
    }

    // Keyboard handler for Esc, ArrowLeft, ArrowRight
    @HostListener("window:keydown", ["$event"])
    public onKeydown(event: KeyboardEvent) {
        if (!this.selectedImage) {
            return;
        }
        if (event.key === "Escape") {
            this.closeImage();
            return;
        }
        if (event.key === "ArrowRight") {
            event.preventDefault();
            this.nextImage();
            return;
        }
        if (event.key === "ArrowLeft") {
            event.preventDefault();
            this.prevImage();
            return;
        }
    }

    public downloadFactSheetPdf(projectName: string | null | undefined) {
        this.isLoadingPdf = true;
        var requestDto = { url: window.location.href } as CapturePostData;
        this.sitkaCaptureService.generatePdfSitkaCapture(requestDto).subscribe({
            next: (blob) => {
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement("a");
                link.href = url;
                link.download = `${projectName}-fact-sheet.pdf`;
                link.click();
                window.URL.revokeObjectURL(url);

                this.isLoadingPdf = false;
            },
            error: () => (this.isLoadingPdf = false),
        });
    }
}
