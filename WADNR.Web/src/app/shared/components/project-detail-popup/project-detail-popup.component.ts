import { Component, Input, OnInit, booleanAttribute } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable, of } from "rxjs";
import { tap } from "rxjs/operators";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { RouterLink } from "@angular/router";
import { ProjectService } from "../../generated/api/project.service";
import { ProjectMapPopup } from "../../generated/model/project-map-popup";
import { CommaJoinPipe } from "src/app/shared/pipes/comma-join.pipe";
import { PopupDataCacheService } from "src/app/shared/services/popup-data-cache.service";

@Component({
    selector: "app-project-detail-popup",
    standalone: true,
    imports: [CommonModule, LoadingDirective, RouterLink, CommaJoinPipe],
    templateUrl: "./project-detail-popup.component.html",
    styleUrls: ["./project-detail-popup.component.scss"],
    host: {
        "[class.project-detail-popup__compact]": "!showDetails",
    },
})
export class ProjectDetailPopupComponent implements OnInit {
    project$: Observable<ProjectMapPopup>;
    isLoading: boolean = true;

    @Input() projectId: number;
    @Input({ transform: booleanAttribute }) showDetails: boolean = true;

    // Provided by the map-layer popup binder; when set, the popup can render from cache immediately.
    @Input() cacheKey?: string;

    constructor(private projectService: ProjectService, private popupCache: PopupDataCacheService) {}

    ngOnInit(): void {
        const key = this.cacheKey;
        if (key) {
            const cached = this.popupCache.getCachedValue<ProjectMapPopup>(key);
            if (cached) {
                this.isLoading = false;
                this.project$ = of(cached);
                return;
            }

            this.project$ = this.popupCache
                .getOrFetch<ProjectMapPopup>(key, () => this.projectService.getAsMapPopupProject(this.projectId))
                .pipe(tap(() => (this.isLoading = false)));
            return;
        }

        // Fallback: if used outside the two-phase popup helper, fetch directly.
        this.project$ = this.projectService.getAsMapPopupProject(this.projectId).pipe(tap(() => (this.isLoading = false)));
    }
}
