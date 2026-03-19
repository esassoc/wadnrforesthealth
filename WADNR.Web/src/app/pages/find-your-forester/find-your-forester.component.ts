import { AsyncPipe, NgTemplateOutlet } from "@angular/common";
import { Component, computed, ElementRef, signal, ViewChild, inject } from "@angular/core";
import { Map as LeafletMap } from "leaflet";
import * as L from "leaflet";
import { catchError, forkJoin, of, shareReplay, tap } from "rxjs";

import { GenericWmsWfsLayerComponent } from "src/app/shared/components/leaflet/layers/generic-wms-wfs-layer/generic-wms-wfs-layer.component";
import { WADNRMapComponent, WADNRMapInitEvent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { MapSearchComponent } from "src/app/shared/components/leaflet/map-search/map-search.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { FindYourForesterService } from "src/app/shared/generated/api/find-your-forester.service";
import { FindYourForesterQuestionTreeNode } from "src/app/shared/generated/model/find-your-forester-question-tree-node";
import { ForesterRoleLookupItem } from "src/app/shared/generated/model/forester-role-lookup-item";
import { MarkerHelper } from "src/app/shared/helpers/marker-helper";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { WamasService } from "src/app/shared/services/wamas.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";

export interface ForesterContactInfo {
    foresterRoleID: number;
    foresterRoleDisplayName: string;
    firstName: string | null;
    lastName: string | null;
    email: string | null;
    phone: string | null;
    foresterRoleDefinition: string | null;
}

@Component({
    selector: "find-your-forester",
    standalone: true,
    imports: [AsyncPipe, NgTemplateOutlet, WADNRMapComponent, MapSearchComponent, LoadingDirective, GenericWmsWfsLayerComponent, PageHeaderComponent, FieldDefinitionComponent],
    templateUrl: "./find-your-forester.component.html",
    styleUrl: "./find-your-forester.component.scss",
})
export class FindYourForesterComponent {
    @ViewChild(MapSearchComponent) mapSearch: MapSearchComponent;
    @ViewChild("sidebar") sidebarEl: ElementRef<HTMLElement>;

    richTextTypeID = FirmaPageTypeEnum.FindYourForester;

    map: LeafletMap;
    layerControl: L.Control.Layers;

    private alertService = inject(AlertService);

    activeRoles: ForesterRoleLookupItem[] = [];
    roleNameByID = new Map<number, string>();
    isLoading = true;
    private clickMarker: L.Marker | null = null;

    foresterInfo = signal(new Map<number, ForesterContactInfo>());
    hasQueried = signal(false);
    isQuerying = signal(false);
    showAllContacts = signal(false);

    visibleContacts = computed(() => {
        const all = Array.from(this.foresterInfo().values());
        return this.showAllContacts() ? all : all.slice(0, 6);
    });

    totalContacts = computed(() => this.foresterInfo().size);

    constructor(private findYourForesterService: FindYourForesterService, private wamasService: WamasService) {
        this.wamasService.warmUp();
    }

    data$ = forkJoin({
        questions: this.findYourForesterService.listQuestionsFindYourForester(),
        roles: this.findYourForesterService.listActiveRolesFindYourForester(),
    }).pipe(
        tap(({ roles }) => {
            this.activeRoles = roles;
            roles.forEach(r => this.roleNameByID.set(r.ForesterRoleID, r.ForesterRoleName));
            this.isLoading = false;
        }),
        catchError(() => {
            this.isLoading = false;
            return of({ questions: [] as FindYourForesterQuestionTreeNode[], roles: [] as ForesterRoleLookupItem[] });
        }),
        shareReplay({ bufferSize: 1, refCount: true })
    );

    onMapInit(event: WADNRMapInitEvent): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.map.on("click", (e: L.LeafletMouseEvent) => this.queryForestersAtPoint(e.latlng));
    }

    onGeocodeError(message: string): void {
        this.alertService.clearAlerts();
        this.alertService.pushAlert(new Alert(message, AlertContext.Warning));
        this.mapSearch.errorMessage = null;
    }

    onLocationFound(event: { lat: number; lng: number }): void {
        const latlng = L.latLng(event.lat, event.lng);
        this.queryForestersAtPoint(latlng);
    }

    private prepareForQuery(point: L.LatLng): void {
        this.alertService.clearAlerts();
        this.hasQueried.set(true);
        this.isQuerying.set(true);
        this.sidebarEl?.nativeElement.scrollTo({ top: 0, behavior: "smooth" });

        if (this.clickMarker) {
            this.clickMarker.setLatLng(point);
        } else {
            this.clickMarker = L.marker(point, { icon: MarkerHelper.iconDefault }).addTo(this.map);
        }
    }

    private queryForestersAtPoint(point: L.LatLng): void {
        if (!this.map) return;

        this.prepareForQuery(point);

        this.findYourForesterService.getForestersByPointFindYourForester({ Latitude: point.lat, Longitude: point.lng }).subscribe({
            next: (result) => {
                const infoMap = new Map<number, ForesterContactInfo>();
                for (const contact of result.ForesterContacts) {
                    infoMap.set(contact.ForesterRoleID, {
                        foresterRoleID: contact.ForesterRoleID,
                        foresterRoleDisplayName: contact.ForesterRoleDisplayName,
                        firstName: contact.FirstName,
                        lastName: contact.LastName,
                        email: contact.Email,
                        phone: contact.Phone,
                        foresterRoleDefinition: contact.ForesterRoleDefinition,
                    });
                }

                if (infoMap.size === 0) {
                    this.alertService.pushAlert(new Alert("No foresters were found for this location. The location may be outside Washington State or outside of DNR-managed areas.", AlertContext.Warning));
                }

                this.foresterInfo.set(infoMap);
                this.showAllContacts.set(false);
                this.isQuerying.set(false);
            },
            error: () => {
                this.isQuerying.set(false);
                this.alertService.pushAlert(new Alert("An error occurred while searching for foresters. Please try again.", AlertContext.Danger));
            },
        });
    }

    getContactForRole(roleID: number | undefined): ForesterContactInfo | null {
        if (!roleID) return null;
        return this.foresterInfo().get(roleID) || null;
    }

    toggleShowAll(): void {
        this.showAllContacts.update((v) => !v);
    }

    hasChildren(node: FindYourForesterQuestionTreeNode): boolean {
        return node.Children && node.Children.length > 0;
    }
}
