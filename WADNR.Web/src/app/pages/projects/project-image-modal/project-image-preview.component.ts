import { Component, inject } from "@angular/core";
import { DialogRef } from "@ngneat/dialog";
import { ProjectImageGridRow } from "src/app/shared/generated/model/project-image-grid-row";
import { environment } from "src/environments/environment";

export interface ProjectImagePreviewData {
    photo: ProjectImageGridRow;
}

@Component({
    selector: "project-image-preview",
    standalone: true,
    imports: [],
    template: `
        <div class="preview-container" (click)="close()">
            <img [src]="getPhotoUrl()" [alt]="photo.Caption" (click)="$event.stopPropagation()" />
        </div>
        <div class="preview-caption">
            {{ getCaption() }}
        </div>
    `,
    styles: [`
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
    `]
})
export class ProjectImagePreviewComponent {
    public ref: DialogRef<ProjectImagePreviewData, void> = inject(DialogRef);
    public photo: ProjectImageGridRow;

    constructor() {
        this.photo = this.ref.data.photo;
    }

    getPhotoUrl(): string {
        return `${environment.mainAppApiUrl}/file-resources/${this.photo.FileResourceGuid}`;
    }

    getCaption(): string {
        let caption = this.photo.Caption;
        if (this.photo.ProjectImageTimingDisplayName) {
            caption += ` (Timing: ${this.photo.ProjectImageTimingDisplayName})`;
        }
        if (this.photo.ContentLength) {
            caption += ` (~${this.formatFileSize(this.photo.ContentLength)})`;
        }
        if (this.photo.Credit) {
            caption += ` Credit: ${this.photo.Credit}`;
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
