import { Component, EventEmitter, Input, Output } from "@angular/core";
import { DialogService } from "@ngneat/dialog";
import { TagSelectedProjectsModalComponent } from "src/app/shared/components/tag-selected-projects-modal/tag-selected-projects-modal.component";

@Component({
    selector: "tag-checked-projects-button",
    standalone: true,
    template: `<button class="btn btn-sm btn-primary" [disabled]="!selectedRows?.length" (click)="openTagModal()">Tag Checked Projects</button>`,
})
export class TagCheckedProjectsButtonComponent {
    @Input() selectedRows: any[] = [];
    @Output() tagged = new EventEmitter<void>();

    constructor(private dialogService: DialogService) {}

    openTagModal(): void {
        const ref = this.dialogService.open(TagSelectedProjectsModalComponent, {
            data: { projects: this.selectedRows },
            size: "md",
        });
        ref.afterClosed$.subscribe((result) => {
            if (result) {
                this.tagged.emit();
            }
        });
    }
}
