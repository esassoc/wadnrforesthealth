import { Component, inject, OnInit, ViewChild, AfterViewChecked } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
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
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";

import { FieldDefinitionService } from "src/app/shared/generated/api/field-definition.service";
import { FieldDefinitionDatumUpsertRequest } from "src/app/shared/generated/model/models";

export interface FieldDefinitionEditModalData {
    fieldDefinitionID: number;
    fieldDefinitionDisplayName: string;
}

interface ModalLoadState {
    loading: boolean;
    error: boolean;
}

@Component({
    selector: "field-definition-edit",
    standalone: true,
    imports: [AsyncPipe, FormsModule, ReactiveFormsModule, EditorComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective, FormFieldComponent],
    providers: [{ provide: TINYMCE_SCRIPT_SRC, useValue: "tinymce/tinymce.min.js" }],
    templateUrl: "./field-definition-edit.component.html",
    styleUrls: ["./field-definition-edit.component.scss"],
})
export class FieldDefinitionEditComponent extends BaseModal implements OnInit, AfterViewChecked {
    public ref: DialogRef<FieldDefinitionEditModalData, boolean> = inject(DialogRef);

    @ViewChild("tinyMceEditor") tinyMceEditor: EditorComponent;
    public tinyMceConfig: object = {};
    public isSubmitting = false;
    public loadState$: Observable<ModalLoadState>;

    public FormFieldType = FormFieldType;
    public fieldDefinitionDisplayName: string = "";
    public defaultLabel: string = "";
    public defaultDefinition: string = "";
    public customLabelControl = new FormControl("");
    public customDefinition: string = "";

    private fieldDefinitionID: number;

    constructor(
        private fieldDefinitionService: FieldDefinitionService,
        alertService: AlertService,
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.fieldDefinitionID = data.fieldDefinitionID;
        this.fieldDefinitionDisplayName = data.fieldDefinitionDisplayName;

        this.loadState$ = this.fieldDefinitionService.getFieldDefinition(this.fieldDefinitionID).pipe(
            tap(datum => {
                this.defaultLabel = datum.FieldDefinition?.FieldDefinitionDisplayName ?? "";
                this.defaultDefinition = datum.FieldDefinition?.DefaultDefinition ?? "";
                this.customLabelControl.setValue(datum.FieldDefinitionLabel ?? "");
                this.customDefinition = datum.FieldDefinitionDatumValue ?? "";
            }),
            map(() => ({ loading: false, error: false })),
            catchError(err => {
                const message = err?.error?.message ?? err?.message ?? "An error occurred loading definition.";
                this.addLocalAlert(message, AlertContext.Danger, true);
                return of({ loading: false, error: true });
            }),
            startWith({ loading: true, error: false }),
            shareReplay(1),
        );
    }

    ngAfterViewChecked(): void {
        this.tinyMceConfig = TinyMCEHelpers.DefaultInitConfig(this.tinyMceEditor);
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts.set([]);

        const upsertRequest: FieldDefinitionDatumUpsertRequest = {
            FieldDefinitionDatumValue: this.customDefinition,
            FieldDefinitionLabel: this.customLabelControl.value,
        };

        this.fieldDefinitionService.updateFieldDefinition(this.fieldDefinitionID, upsertRequest).subscribe({
            next: () => {
                this.pushGlobalSuccess(`The definition for ${this.fieldDefinitionDisplayName} was successfully updated.`);
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
