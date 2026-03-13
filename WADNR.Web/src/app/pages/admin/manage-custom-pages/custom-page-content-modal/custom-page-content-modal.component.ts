import { Component, inject, OnInit, ViewChild, AfterViewChecked } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import TinyMCEHelpers from "src/app/shared/helpers/tiny-mce-helpers";

import { CustomPageService } from "src/app/shared/generated/api/custom-page.service";
import { CustomPageContentUpsertRequest } from "src/app/shared/generated/model/custom-page-content-upsert-request";

export interface CustomPageContentModalData {
    customPageID: number;
    currentContent: string;
}

@Component({
    selector: "custom-page-content-modal",
    standalone: true,
    imports: [FormsModule, EditorComponent, ModalAlertsComponent],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
    templateUrl: "./custom-page-content-modal.component.html",
})
export class CustomPageContentModalComponent extends BaseModal implements OnInit, AfterViewChecked {
    public ref: DialogRef<CustomPageContentModalData, boolean> = inject(DialogRef);

    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object = TinyMCEHelpers.DefaultInitConfig();
    public editedContent: string = "";
    public isSubmitting = false;

    private customPageID: number;

    constructor(
        private customPageService: CustomPageService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.customPageID = data.customPageID;
        this.editedContent = data.currentContent ?? "";
    }

    ngAfterViewChecked(): void {
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new CustomPageContentUpsertRequest({
            CustomPageContent: this.editedContent,
        });

        this.customPageService.updateContentCustomPage(this.customPageID, dto).subscribe({
            next: () => {
                this.pushGlobalSuccess("Page content updated successfully.");
                this.ref.close(true);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
