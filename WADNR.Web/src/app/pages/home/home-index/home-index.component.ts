import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { Router, RouterLink } from "@angular/router";
import { Observable, map } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { AuthenticationService } from "src/app/services/authentication.service";
import { FirmaHomePageImageService } from "src/app/shared/generated/api/firma-home-page-image.service";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { IFeature } from "src/app/shared/generated/model/i-feature";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { ProjectFeatured } from "src/app/shared/generated/model/project-featured";
import { PROJECT_STAGE_LEGEND_COLORS } from "src/app/shared/models/legend-colors";
import { ImageCarouselItem } from "src/app/shared/components/image-carousel/image-carousel.component";

import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { ImageCarouselComponent } from "src/app/shared/components/image-carousel/image-carousel.component";
import { FeaturedCarouselComponent } from "src/app/shared/components/featured-carousel/featured-carousel.component";
import { WADNRMapComponent } from "src/app/shared/components/leaflet/wadnr-map/wadnr-map.component";
import { ProjectLocationsSimpleLayerComponent } from "src/app/shared/components/leaflet/layers/project-locations-simple-layer/project-locations-simple-layer.component";
import { ProjectStageMapLegendComponent } from "src/app/shared/components/project-stage-map-legend/project-stage-map-legend.component";
import * as L from "leaflet";

@Component({
    selector: "app-home-index",
    templateUrl: "./home-index.component.html",
    styleUrls: ["./home-index.component.scss"],
    imports: [
        AsyncPipe,
        RouterLink,
        CustomRichTextComponent,
        ImageCarouselComponent,
        FeaturedCarouselComponent,
        WADNRMapComponent,
        ProjectLocationsSimpleLayerComponent,
        ProjectStageMapLegendComponent,
    ],
})
export class HomeIndexComponent implements OnInit {
    // Rich text type IDs
    homePageTypeID = FirmaPageTypeEnum.HomePage;
    homeMapInfoTypeID = FirmaPageTypeEnum.HomeMapInfo;
    homeAdditionalInfoTypeID = FirmaPageTypeEnum.HomeAdditionalInfo;

    // Observables
    isAuthenticated$: Observable<boolean>;
    currentUser$: Observable<PersonDetail | null>;
    carouselImages$: Observable<ImageCarouselItem[]>;
    featuredProjects$: Observable<ProjectFeatured[]>;
    projectPoints$: Observable<IFeature[]>;

    // Map
    map: L.Map;
    layerControl: any;
    mapIsReady = false;
    legendColorsToUse = PROJECT_STAGE_LEGEND_COLORS;

    constructor(
        private authenticationService: AuthenticationService,
        private firmaHomePageImageService: FirmaHomePageImageService,
        private projectService: ProjectService,
        private dialogService: DialogService,
        private router: Router,
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.currentUserSetObservable;
        this.isAuthenticated$ = this.currentUser$.pipe(
            map((user) => user != null),
        );

        this.carouselImages$ = this.firmaHomePageImageService.listFirmaHomePageImage().pipe(
            map((images) =>
                images.map((img) => ({
                    FileResourceGUID: img.FileResourceGUID,
                    Caption: img.Caption,
                })),
            ),
        );

        this.featuredProjects$ = this.projectService.listFeaturedProject();
        this.projectPoints$ = this.projectService.listMappedPointsFeatureCollectionProject();
    }

    // Map
    handleMapReady(event: any): void {
        this.map = event.map;
        this.layerControl = event.layerControl;
        this.mapIsReady = true;
    }

    canCreateGisUpload(user: PersonDetail | null): boolean {
        return this.authenticationService.canCreateGisUpload(user);
    }

    openGisImportModal(): void {
        import("../../admin/gis-bulk-import/select-source-org-modal/select-source-org-modal.component").then(({ SelectSourceOrgModalComponent }) => {
            const ref = this.dialogService.open(SelectSourceOrgModalComponent, { size: "lg" });
            ref.afterClosed$.subscribe((attemptID) => {
                if (attemptID) {
                    this.router.navigate(["/gis-bulk-import", attemptID, "instructions"]);
                }
            });
        });
    }
}
