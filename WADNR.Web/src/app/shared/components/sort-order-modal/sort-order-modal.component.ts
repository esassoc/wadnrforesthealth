import { Component, inject } from "@angular/core";
import { CdkDropList, CdkDrag, CdkDragDrop, moveItemInArray } from "@angular/cdk/drag-drop";
import { DialogRef } from "@ngneat/dialog";

import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

export interface SortOrderItem {
    id: number;
    displayName: string;
}

export interface SortOrderModalData {
    items: SortOrderItem[];
    entityLabel: string;
}

export interface SortOrderResult {
    ID: number;
    SortOrder: number;
}

@Component({
    selector: "sort-order-modal",
    standalone: true,
    imports: [CdkDropList, CdkDrag, ButtonLoadingDirective],
    templateUrl: "./sort-order-modal.component.html",
    styleUrls: ["./sort-order-modal.component.scss"],
})
export class SortOrderModalComponent {
    public ref: DialogRef<SortOrderModalData, SortOrderResult[] | null> = inject(DialogRef);

    public items: SortOrderItem[] = [];
    public entityLabel = "items";
    public isSubmitting = false;

    ngOnInit(): void {
        const data = this.ref.data;
        this.items = [...(data?.items ?? [])];
        this.entityLabel = data?.entityLabel ?? "items";
    }

    drop(event: CdkDragDrop<SortOrderItem[]>): void {
        moveItemInArray(this.items, event.previousIndex, event.currentIndex);
    }

    resetToAlphabetical(): void {
        this.items.sort((a, b) => a.displayName.localeCompare(b.displayName));
    }

    save(): void {
        const result: SortOrderResult[] = this.items.map((item, index) => ({
            ID: item.id,
            SortOrder: (index + 1) * 10,
        }));
        this.ref.close(result);
    }

    cancel(): void {
        this.ref.close(null);
    }
}
