import { Component, inject, OnInit, ViewChild, AfterViewChecked } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { EditorComponent, TINYMCE_SCRIPT_SRC } from "@tinymce/tinymce-angular";
import { Observable, of, map, tap, startWith, catchError, shareReplay } from "rxjs";

import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import TinyMCEHelpers from "src/app/shared/helpers/tiny-mce-helpers";

import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { CustomRichTextService } from "src/app/shared/generated/api/custom-rich-text.service";
import { FirmaPageUpsertRequest } from "src/app/shared/generated/model/firma-page-upsert-request";

export interface PageContentModalData {
    firmaPageTypeID: number;
    pageName: string;
}

interface ModalLoadState {
    loading: boolean;
    error: boolean;
}

@Component({
    selector: "page-content-modal",
    standalone: true,
    imports: [AsyncPipe, FormsModule, EditorComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
    templateUrl: "./page-content-modal.component.html",
})
export class PageContentModalComponent extends BaseModal implements OnInit, AfterViewChecked {
    public ref: DialogRef<PageContentModalData, boolean> = inject(DialogRef);

    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object = {};
    public editedContent: string = "";
    public isSubmitting = false;
    public pageName: string = "";
    public loadState$: Observable<ModalLoadState>;

    private firmaPageTypeID: number;

    constructor(
        private customRichTextService: CustomRichTextService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.firmaPageTypeID = data.firmaPageTypeID;
        this.pageName = data.pageName;

        this.loadState$ = this.customRichTextService.getCustomRichText(this.firmaPageTypeID).pipe(
            tap(detail => {
                this.editedContent = detail.FirmaPageContent ?? "";
            }),
            map(() => ({ loading: false, error: false })),
            catchError(err => {
                const message = err?.error?.message ?? err?.message ?? "An error occurred loading content.";
                this.addLocalAlert(message, AlertContext.Danger, true);
                return of({ loading: false, error: true });
            }),
            startWith({ loading: true, error: false }),
            shareReplay(1)
        );
    }

    ngAfterViewChecked(): void {
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new FirmaPageUpsertRequest({
            FirmaPageContent: this.editedContent,
        });

        this.customRichTextService.updateCustomRichText(this.firmaPageTypeID, dto).subscribe({
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
