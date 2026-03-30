import { Component, EventEmitter, Input, Output } from "@angular/core";
import { DialogService } from "@ngneat/dialog";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { environment } from "src/environments/environment";

export interface ImageGalleryItem {
    imageID: number;
    fileResourceGuid: string;
    caption?: string;
    credit?: string;
    isKeyPhoto: boolean;
    timingDisplayName?: string;
    contentLength?: number;
}

@Component({
    selector: "image-gallery",
    standalone: true,
    imports: [IconComponent],
    templateUrl: "./image-gallery.component.html",
    styleUrls: ["./image-gallery.component.scss"],
})
export class ImageGalleryComponent {
    @Input() images: ImageGalleryItem[] = [];
    @Input() canEdit: boolean = false;
    @Output() edit = new EventEmitter<ImageGalleryItem>();
    @Output() delete = new EventEmitter<ImageGalleryItem>();
    @Output() setKeyPhoto = new EventEmitter<ImageGalleryItem>();

    constructor(private dialogService: DialogService) {}

    getPhotoUrl(image: ImageGalleryItem): string {
        return `${environment.mainAppApiUrl}/file-resources/${image.fileResourceGuid}`;
    }

    openPreview(image: ImageGalleryItem): void {
        import("./image-preview.component").then((m) => {
            this.dialogService.open(m.ImagePreviewComponent, {
                data: { image, photoUrl: this.getPhotoUrl(image) },
                width: "auto",
                maxWidth: "95vw",
                closeButton: true,
                enableClose: { escape: true, backdrop: true },
            });
        });
    }
}
