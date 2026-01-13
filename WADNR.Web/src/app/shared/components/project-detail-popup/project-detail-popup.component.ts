import { Component, Input, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Observable } from "rxjs";
import { shareReplay, tap } from "rxjs/operators";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { RouterLink } from "@angular/router";
import { ProjectService } from "../../generated/api/project.service";
import { ProjectMapPopup } from "../../generated/model/project-map-popup";
import { CommaJoinPipe } from "src/app/shared/pipes/comma-join.pipe";

@Component({
    selector: "app-project-detail-popup",
    standalone: true,
    imports: [CommonModule, LoadingDirective, RouterLink, CommaJoinPipe],
    templateUrl: "./project-detail-popup.component.html",
    styleUrls: ["./project-detail-popup.component.scss"],
})
export class ProjectDetailPopupComponent implements OnInit {
    project$: Observable<ProjectMapPopup>;
    isLoading: boolean = true;

    @Input() projectId: number;

    constructor(private projectService: ProjectService) {}

    ngOnInit(): void {
        this.project$ = this.projectService.getAsMapPopupProject(this.projectId).pipe(
            tap(() => (this.isLoading = false)),
            shareReplay(1)
        );
    }
}
