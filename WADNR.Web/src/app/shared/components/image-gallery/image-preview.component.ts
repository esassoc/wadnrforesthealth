import { Component, inject } from "@angular/core";
import { DialogRef } from "@ngneat/dialog";
import { ImageGalleryItem } from "./image-gallery.component";

export interface ImagePreviewData {
    image: ImageGalleryItem;
    photoUrl: string;
}

@Component({
    selector: "image-preview",
    standalone: true,
    imports: [],
    template: `
        <div class="preview-container" (click)="close()">
            <img [src]="photoUrl" [alt]="image.caption" (click)="$event.stopPropagation()" />
        </div>
        <div class="preview-caption">
            {{ getCaption() }}
        </div>
    `,
    styles: [
        `
            :host {
                display: block;
            }

            .preview-container {
                display: flex;
                align-items: center;
                justify-content: center;
                min-height: 300px;
                max-height: calc(90vh - 60px);
                background: #000;
                cursor: pointer;

                img {
                    max-width: 100%;
                    max-height: calc(90vh - 60px);
                    object-fit: contain;
                }
            }

            .preview-caption {
                background: rgb(51, 68, 85);
                color: #fff;
                padding: 12px 20px;
                text-align: center;
                font-size: 14px;
            }
        `,
    ],
})
export class ImagePreviewComponent {
    public ref: DialogRef<ImagePreviewData, void> = inject(DialogRef);
    public image: ImageGalleryItem;
    public photoUrl: string;

    constructor() {
        this.image = this.ref.data.image;
        this.photoUrl = this.ref.data.photoUrl;
    }

    getCaption(): string {
        let caption = this.image.caption || "";
        if (this.image.timingDisplayName) {
            caption += ` (Timing: ${this.image.timingDisplayName})`;
        }
        if (this.image.contentLength) {
            caption += ` (~${this.formatFileSize(this.image.contentLength)})`;
        }
        if (this.image.credit) {
            caption += ` Credit: ${this.image.credit}`;
        }
        return caption;
    }

    private formatFileSize(bytes: number): string {
        if (bytes < 1024) return bytes + " B";
        if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
        return (bytes / (1024 * 1024)).toFixed(1) + " MB";
    }

    close(): void {
        this.ref.close();
    }
}
