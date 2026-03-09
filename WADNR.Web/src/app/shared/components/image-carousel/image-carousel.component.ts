import { Component, Input, OnInit, OnDestroy, OnChanges, SimpleChanges, signal } from "@angular/core";
import { DomSanitizer, SafeStyle } from "@angular/platform-browser";
import { environment } from "src/environments/environment";

export interface ImageCarouselItem {
    FileResourceGUID: string;
    Caption?: string | null;
}

@Component({
    selector: "image-carousel",
    templateUrl: "./image-carousel.component.html",
    styleUrls: ["./image-carousel.component.scss"],
    standalone: true,
})
export class ImageCarouselComponent implements OnInit, OnChanges, OnDestroy {
    @Input() images: ImageCarouselItem[] = [];
    @Input() noImagesMessage: string = "No images available.";

    /** Auto-advance interval in ms (0 = disabled) */
    @Input() autoAdvanceInterval: number = 0;

    /** Height of the carousel viewport */
    @Input() height: string = "450px";

    currentSlide = signal(0);
    hasMultiple = false;

    private timer: any;

    constructor(private sanitizer: DomSanitizer) {}

    ngOnInit(): void {
        this.startTimer();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes["images"]) {
            this.currentSlide.set(0);
            this.hasMultiple = this.images?.length > 1;
            this.restartTimer();
        }
    }

    ngOnDestroy(): void {
        this.stopTimer();
    }

    getSlideBackgroundUrl(image: ImageCarouselItem): SafeStyle {
        const url = `${environment.mainAppApiUrl}/file-resources/${encodeURIComponent(image.FileResourceGUID)}`;
        return this.sanitizer.bypassSecurityTrustStyle(`url('${url}')`);
    }

    goToSlide(index: number): void {
        if (!this.images?.length) return;
        this.currentSlide.set(index);
        this.restartTimer();
    }

    prevSlide(): void {
        if (!this.images?.length) return;
        this.currentSlide.update(i => (i - 1 + this.images.length) % this.images.length);
        this.restartTimer();
    }

    nextSlide(): void {
        if (!this.images?.length) return;
        this.currentSlide.update(i => (i + 1) % this.images.length);
        this.restartTimer();
    }

    pause(): void {
        this.stopTimer();
    }

    resume(): void {
        this.startTimer();
    }

    private startTimer(): void {
        if (!this.autoAdvanceInterval || !this.images || this.images.length <= 1) return;
        this.stopTimer();
        this.timer = setInterval(() => this.nextSlide(), this.autoAdvanceInterval);
    }

    private stopTimer(): void {
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
    }

    private restartTimer(): void {
        this.stopTimer();
        this.startTimer();
    }
}
