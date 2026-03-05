import { Component } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Router } from "@angular/router";
import { ColDef, GridApi, GridReadyEvent } from "ag-grid-community";
import { Map } from "leaflet";
import { Observable, shareReplay, startWith, Subject, switchMap, take, tap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { HybridMapGridComponent } from "src/app/shared/components/hybrid-map-grid/hybrid-map-grid.component";
import { GenericFeatureCollectionLayerComponent } from "src/app/shared/components/leaflet/layers/generic-feature-collection-layer/generic-feature-collection-layer.component";
import { ExternalMapLayersComponent } from "src/app/shared/components/leaflet/layers/external-map-layers/external-map-layers.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";

import { FocusAreaService } from "src/app/shared/generated/api/focus-area.service";
import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { FocusAreaGridRow } from "src/app/shared/generated/model/focus-area-grid-row";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { FocusAreaEditModalComponent, FocusAreaEditModalData } from "./focus-area-detail/focus-area-edit-modal.component";

@Component({
    selector: "focus-areas",
    standalone: true,
    imports: [PageHeaderComponent, HybridMapGridComponent, GenericFeatureCollectionLayerComponent, ExternalMapLayersComponent, AsyncPipe, LoadingDirective],
    templateUrl: "./focus-areas.component.html",
})
export class FocusAreasComponent {
    public focusAreas$: Observable<FocusAreaGridRow[]>;
    public focusAreaLocations$: Observable<IFeature[]>;
    public columnDefs: ColDef[];
    public canManageFocusAreas = false;
    public isLoading = true;

    public map: Map;
    public layerControl: L.Control.Layers;
    public mapIsReady: boolean = false;
    public selectedFocusAreaID: number;
    public gridApi: GridApi;

    private refreshFocusAreas$ = new Subject<void>();
    private regionOptions: SelectDropdownOption[] = [];

    constructor(
        private focusAreaService: FocusAreaService,
        private dnrUplandRegionService: DNRUplandRegionService,
        private utilityFunctions: UtilityFunctionsService,
        private authService: AuthenticationService,
        private dialogService: DialogService,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.authService.currentUserSetObservable.pipe(take(1)).subscribe((user) => {
            this.canManageFocusAreas = this.authService.canManageFocusAreas(user);
            this.buildColumnDefs();
        });

        this.dnrUplandRegionService.listLookupDNRUplandRegion().subscribe((regions) => {
            this.regionOptions = regions.map((r) => ({
                Value: r.DNRUplandRegionID,
                Label: r.DNRUplandRegionName,
            } as SelectDropdownOption));
        });

        this.focusAreas$ = this.refreshFocusAreas$.pipe(
            startWith(undefined),
            switchMap(() => this.focusAreaService.listFocusArea()),
            tap(() => (this.isLoading = false)),
            shareReplay(1)
        );

        this.focusAreaLocations$ = this.focusAreaService.listLocationsFocusArea().pipe(shareReplay(1));
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            this.utilityFunctions.createLinkColumnDef("Focus Area", "FocusAreaName", "FocusAreaID", {
                InRouterLink: "/focus-areas/",
            }),
            this.utilityFunctions.createBasicColumnDef("Status", "FocusAreaStatusDisplayName", { Width: 120 }),
            this.utilityFunctions.createLinkColumnDef("Region", "DNRUplandRegionName", "DNRUplandRegionID", {
                InRouterLink: "/dnr-upland-regions/",
                Width: 150,
            }),
            this.utilityFunctions.createBasicColumnDef("# of Projects", "ProjectCount", { Width: 120 }),
        ];

        if (this.canManageFocusAreas) {
            this.columnDefs.push(
                this.utilityFunctions.createActionsColumnDef((params) => {
                    const row = params.data as FocusAreaGridRow;
                    return [
                        { ActionName: "Delete", ActionHandler: () => this.confirmDelete(row), ActionIcon: "fa fa-trash" },
                    ];
                })
            );
        }
    }

    handleMapReady(event: any) {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    public onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
    }

    public onSelectedFocusAreaIDChanged(selected: number | number[]) {
        const selectedFocusAreaID = Array.isArray(selected) ? (selected.length ? selected[0] : undefined) : selected;
        if (this.selectedFocusAreaID == selectedFocusAreaID) {
            return;
        }
        this.selectedFocusAreaID = selectedFocusAreaID as number;
    }

    openCreateModal(): void {
        const dialogRef = this.dialogService.open(FocusAreaEditModalComponent, {
            data: {
                mode: "create",
                regionOptions: this.regionOptions,
            } as FocusAreaEditModalData,
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.router.navigate(["/focus-areas", result.FocusAreaID]);
            }
        });
    }

    async confirmDelete(focusArea: FocusAreaGridRow): Promise<void> {
        if (focusArea.ProjectCount > 0) {
            await this.confirmService.confirm({
                title: "Cannot Delete Focus Area",
                message: `Cannot delete "${focusArea.FocusAreaName}" because it has ${focusArea.ProjectCount} associated project(s). Visit the <a href="/focus-areas/${focusArea.FocusAreaID}">Focus Area detail page</a> to manage its projects.`,
                buttonTextYes: "OK",
                buttonClassYes: "btn-primary",
                buttonTextNo: "Cancel",
            });
            return;
        }

        const confirmed = await this.confirmService.confirm({
            title: "Delete Focus Area",
            message: `Are you sure you want to delete "${focusArea.FocusAreaName}"? This action cannot be undone.`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.focusAreaService.deleteFocusArea(focusArea.FocusAreaID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Focus Area deleted successfully.", AlertContext.Success, true));
                    this.refreshFocusAreas$.next();
                },
                error: (err) => {
                    this.alertService.pushAlert(new Alert(err?.error?.ErrorMessage ?? "Failed to delete Focus Area.", AlertContext.Danger, true));
                },
            });
        }
    }
}
