import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { Map as LeafletMap } from "leaflet";
import * as L from "leaflet";
import { ColDef, GetRowIdFunc, GridApi, SelectionChangedEvent, ValueGetterFunc } from "ag-grid-community";
import { DialogService } from "@ngneat/dialog";
import { BehaviorSubject, filter, shareReplay, Subject, switchMap, takeUntil, tap } from "rxjs";

import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WfsService } from "src/app/shared/services/wfs.service";
import { FindYourForesterService } from "src/app/shared/generated/api/find-your-forester.service";
import { ForesterRoleLookupItem } from "src/app/shared/generated/model/forester-role-lookup-item";
import { ForesterWorkUnitGridRow } from "src/app/shared/generated/model/forester-work-unit-grid-row";
import { environment } from "src/environments/environment";
import { MAP_SELECTED_COLOR } from "src/app/shared/models/map-colors";
import { BulkAssignForestersModalComponent, BulkAssignModalData } from "./bulk-assign-foresters-modal.component";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";

@Component({
    selector: "manage-find-your-forester",
    standalone: true,
    imports: [CommonModule, PageHeaderComponent, WADNRMapComponent, WADNRGridComponent, GenericWmsWfsLayerComponent, LoadingDirective],
    templateUrl: "./manage-find-your-forester.component.html",
    styleUrl: "./manage-find-your-forester.component.scss",
})
export class ManageFindYourForesterComponent implements OnInit, OnDestroy {
    customRichTextTypeID = FirmaPageTypeEnum.ManageFindYourForester;

    private destroy$ = new Subject<void>();
    map: LeafletMap;
    layerControl: any;
    private gridApi: GridApi;
    private highlightLayers = new Map<number, L.GeoJSON>();

    activeRoles: ForesterRoleLookupItem[] = [];
    selectedRoleID: number | null = null;

    columnDefs: ColDef[] = [];
    selectedWorkUnits: ForesterWorkUnitGridRow[] = [];

    isLoadingGrid = false;
    getRowId: GetRowIdFunc = (params) => params.data.ForesterWorkUnitID;

    private selectedRoleID$ = new BehaviorSubject<number | null>(null);

    private readonly GEOSERVER_LAYER = "WADNRForestHealth:FindYourForester";
    private readonly WFS_FEATURE_TYPE = "WADNRForestHealth:FindYourForester";

    constructor(
        private findYourForesterService: FindYourForesterService,
        private wfsService: WfsService,
        private utilityFunctionsService: UtilityFunctionsService,
        private dialogService: DialogService,
        private alertService: AlertService
    ) {}

    roles$ = this.findYourForesterService.listActiveRolesFindYourForester().pipe(
        tap((roles) => {
            this.activeRoles = roles;
            if (roles.length > 0) {
                this.selectedRoleID = roles[0].ForesterRoleID;
                this.selectedRoleID$.next(roles[0].ForesterRoleID);
            }
        }),
        shareReplay({ bufferSize: 1, refCount: true })
    );

    workUnits$ = this.selectedRoleID$.pipe(
        filter((id): id is number => id != null),
        tap(() => (this.isLoadingGrid = true)),
        switchMap((id) => this.findYourForesterService.listWorkUnitsForRoleFindYourForester(id)),
        tap(() => (this.isLoadingGrid = false)),
        shareReplay({ bufferSize: 1, refCount: true })
    );

    ngOnInit(): void {
        const assignedCol = this.utilityFunctionsService.createLinkColumnDef("Assigned to Person", "AssignedPersonName", "PersonID", {
            InRouterLink: "/people/",
        });
        const origGetter = assignedCol.valueGetter as ValueGetterFunc;
        assignedCol.valueGetter = (params) => {
            const result = origGetter(params);
            if (result && !result.LinkDisplay) {
                result.LinkDisplay = "unassigned";
            }
            return result;
        };

        this.columnDefs = [
            this.utilityFunctionsService.createBasicColumnDef("Role", "ForesterRoleDisplayName"),
            this.utilityFunctionsService.createBasicColumnDef("Forester Work Unit Name", "ForesterWorkUnitName"),
            assignedCol,
        ];
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    onMapInit(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;

        this.map.on("click", (e: L.LeafletMouseEvent) => this.onMapClick(e));
    }

    onGridReady(event: any): void {
        this.gridApi = event.api;
    }

    onOverlayToggle(event: L.LayersControlEvent): void {
        const wmsLayer = event.layer as L.TileLayer.WMS;
        const cqlFilter = (wmsLayer?.wmsParams as any)?.cql_filter || "";
        const match = cqlFilter.match(/ForesterRoleID=(\d+)/);
        if (match) {
            const newRoleID = Number(match[1]);
            if (newRoleID !== this.selectedRoleID) {
                this.selectedRoleID = newRoleID;
                this.clearHighlights();
                this.selectedWorkUnits = [];
                this.selectedRoleID$.next(newRoleID);
            }
        }
    }

    onSelectionChanged(event: SelectionChangedEvent): void {
        if (!this.gridApi) return;
        this.selectedWorkUnits = this.gridApi.getSelectedRows();

        // If this was triggered by a user click (not API), highlight on map
        if (event.source === "uiSelectAll" || event.source === "checkboxSelected" || event.source === "rowClicked") {
            this.syncMapHighlightsFromGrid();
        }
    }

    openBulkAssignModal(): void {
        if (this.selectedWorkUnits.length === 0) return;

        const selectedRole = this.activeRoles.find((r) => r.ForesterRoleID === this.selectedRoleID);

        this.dialogService
            .open(BulkAssignForestersModalComponent, {
                data: {
                    selectedWorkUnits: this.selectedWorkUnits,
                    roleDisplayName: selectedRole?.ForesterRoleDisplayName || "",
                } as BulkAssignModalData,
                width: "600px",
            })
            .afterClosed$.pipe(takeUntil(this.destroy$))
            .subscribe((result) => {
                if (result) {
                    this.selectedRoleID$.next(this.selectedRoleID);
                    this.clearHighlights();
                    this.selectedWorkUnits = [];
                    this.gridApi?.deselectAll();
                }
            });
    }

    private async onMapClick(e: L.LeafletMouseEvent): Promise<void> {
        if (!this.selectedRoleID || !this.map) return;

        const wmsUrl = environment.geoserverMapServiceUrl + "/wms";
        const params: Record<string, string> = {
            service: "WMS",
            version: "1.1.1",
            request: "GetFeatureInfo",
            layers: this.GEOSERVER_LAYER,
            query_layers: this.GEOSERVER_LAYER,
            styles: "",
            bbox: this.map.getBounds().toBBoxString(),
            width: String(this.map.getSize().x),
            height: String(this.map.getSize().y),
            srs: "EPSG:4326",
            format: "image/png",
            info_format: "application/json",
            x: String(Math.round(this.map.layerPointToContainerPoint(e.layerPoint).x)),
            y: String(Math.round(this.map.layerPointToContainerPoint(e.layerPoint).y)),
            cql_filter: `ForesterRoleID=${this.selectedRoleID}`,
        };

        const urlParams = new URLSearchParams(params).toString();
        try {
            const response = await fetch(`${wmsUrl}?${urlParams}`);
            const data = await response.json();
            if (data.features && data.features.length > 0) {
                const props = data.features[0].properties;
                const workUnitID = props.PrimaryKey;

                // Build popup with custom element so field-definition renders with help icon
                const role = this.activeRoles.find((r) => r.ForesterRoleID === this.selectedRoleID);
                const attrs = [
                    role?.ForesterRoleName ? `role-name="${role.ForesterRoleName}"` : "",
                    `role-display-name="${props.ForesterRoleDisplayName || ""}"`,
                    props.FirstName ? `first-name="${props.FirstName}"` : "",
                    props.LastName ? `last-name="${props.LastName}"` : "",
                    props.Phone ? `phone="${props.Phone}"` : "",
                    props.Email ? `email="${props.Email}"` : "",
                ]
                    .filter(Boolean)
                    .join(" ");

                L.popup({ maxWidth: 300, maxHeight: 300 })
                    .setLatLng(e.latlng)
                    .setContent(`<forester-popup-custom-element ${attrs}></forester-popup-custom-element>`)
                    .openOn(this.map);

                if (workUnitID && this.gridApi) {
                    // Select the matching row in grid
                    this.gridApi.deselectAll();
                    this.gridApi.forEachNode((node) => {
                        if (node.data?.ForesterWorkUnitID === workUnitID) {
                            node.setSelected(true, false, "api");
                        }
                    });
                    // Highlight the polygon
                    this.clearHighlights();
                    this.addHighlightForWorkUnit(workUnitID);
                }
            }
        } catch {
            this.alertService.pushAlert(new Alert("Failed to query map features. Please try again.", AlertContext.Danger, true));
        }
    }

    private syncMapHighlightsFromGrid(): void {
        this.clearHighlights();
        for (const wu of this.selectedWorkUnits) {
            this.addHighlightForWorkUnit(wu.ForesterWorkUnitID);
        }
    }

    private addHighlightForWorkUnit(workUnitID: number): void {
        if (this.highlightLayers.has(workUnitID)) return;

        const cqlFilter = `PrimaryKey=${workUnitID}`;
        this.wfsService
            .getGeoserverWFSLayerWithCQLFilter(this.WFS_FEATURE_TYPE, cqlFilter, "PrimaryKey")
            .pipe(takeUntil(this.destroy$))
            .subscribe((features: any[]) => {
                if (!features || features.length === 0 || !this.map) return;
                const geoJson = L.geoJSON(features, {
                    style: {
                        color: MAP_SELECTED_COLOR,
                        weight: 2,
                        opacity: 0.65,
                        fillOpacity: 0.1,
                    },
                });
                geoJson.addTo(this.map);
                this.highlightLayers.set(workUnitID, geoJson);
            });
    }

    private clearHighlights(): void {
        for (const [, layer] of this.highlightLayers) {
            if (this.map) {
                this.map.removeLayer(layer);
            }
        }
        this.highlightLayers.clear();
    }
}
