import { Component, Input, OnDestroy, OnInit, computed, signal } from "@angular/core";
import { NgClass } from "@angular/common";
import { RouterLink } from "@angular/router";
import { trigger, transition, style, animate, state } from "@angular/animations";
import { FeaturedProjectDto } from "../../generated/model/models";
import { environment } from "src/environments/environment.prod";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";

/** Match your DTO shape; extend if you add fields later */
export interface FeaturedProjectVm {
    ProjectID: number;
    ProjectName: string;
    ProjectNumber: string;
    ActionPriority: string;
    Implementers: string;
    Stage: string;
    Duration: string;
    ProjectDescription: string;
    KeyPhotoFileResourceGuid?: string | null;
    KeyPhotoCaption?: string | null;
}

@Component({
    selector: "featured-carousel",
    standalone: true,
    templateUrl: "./featured-carousel.component.html",
    styleUrls: ["./featured-carousel.component.scss"],
    imports: [RouterLink],
    animations: [
        trigger("slide", [
            state("next", style({ transform: "translateX(0)", opacity: 1 })),
            state("prev", style({ transform: "translateX(0)", opacity: 1 })),
            transition("void => next", [style({ transform: "translateX(100%)", opacity: 0 }), animate("450ms cubic-bezier(.35,0,.25,1)")]),
            transition("void => prev", [style({ transform: "translateX(-100%)", opacity: 0 }), animate("450ms cubic-bezier(.35,0,.25,1)")]),
            transition("next => void", [animate("450ms cubic-bezier(.35,0,.25,1)", style({ transform: "translateX(-100%)", opacity: 0 }))]),
            transition("prev => void", [animate("450ms cubic-bezier(.35,0,.25,1)", style({ transform: "translateX(100%)", opacity: 0 }))]),
        ]),
    ],
})
export class FeaturedCarouselComponent implements OnInit, OnDestroy {
    /** Projects to render */
    @Input({ required: true }) projects: FeaturedProjectDto[] = [];

    /** Provide a function to turn file GUID -> URL (so this component is app-agnostic) */
    @Input() imageUrlBuilder?: (guid: string) => string;

    /** Auto-rotate interval in ms (set 0 to disable) */
    @Input() interval = 10000;

    /** Rounded radius on image (px) – tweak if your DS has a token */
    @Input() imageRadius = 10;

    index = signal(0);
    dir = signal<"next" | "prev">("next");

    private timer: any;

    ngOnInit() {
        this.startTimer();
    }

    ngOnDestroy() {
        this.stopTimer();
    }

    get hasMany() {
        return (this.projects?.length || 0) > 1;
    }

    go(i: number) {
        if (!this.projects?.length) return;
        this.dir.set(i > this.index() ? "next" : "prev");
        this.index.set(i);
        this.restartTimer();
    }

    prev() {
        if (!this.projects?.length) return;
        this.dir.set("prev");
        const n = this.projects.length;
        this.index.update((i) => (i - 1 + n) % n);
        this.restartTimer();
    }

    next() {
        if (!this.projects?.length) return;
        this.dir.set("next");
        const n = this.projects.length;
        this.index.update((i) => (i + 1) % n);
        this.restartTimer();
    }

    constructor(private sanitizer: DomSanitizer) {}

    photoUrl(guid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, guid);
    }

    /** accessibility helpers */
    ariaSlideId = computed(() => `slide-${this.index()}`);

    pause() {
        this.stopTimer();
    }
    resume() {
        this.startTimer();
    }

    private startTimer() {
        if (!this.interval || !this.hasMany) return;
        this.stopTimer();
        this.timer = setInterval(() => this.next(), this.interval);
    }
    private stopTimer() {
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
    }
    private restartTimer() {
        this.stopTimer();
        this.startTimer();
    }
}
