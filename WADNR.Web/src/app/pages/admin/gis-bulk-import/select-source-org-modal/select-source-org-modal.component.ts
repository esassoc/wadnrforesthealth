import { Component, inject, OnInit } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { AsyncPipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { Observable, tap } from "rxjs";

import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { GisBulkImportService } from "src/app/shared/generated/api/gis-bulk-import.service";

@Component({
    selector: "select-source-org-modal",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    templateUrl: "./select-source-org-modal.component.html",
})
export class SelectSourceOrgModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<void, number> = inject(DialogRef);
    public FormFieldType = FormFieldType;

    public sourceOrgs$: Observable<any>;
    public sourceOrgOptions: FormInputOption[] = [];
    public sourceOrgControl = new FormControl<number>(null);
    public isCreating = false;

    constructor(private gisBulkImportService: GisBulkImportService) {
        super();
    }

    ngOnInit(): void {
        this.sourceOrgs$ = this.gisBulkImportService.listSourceOrganizationsGisBulkImport().pipe(
            tap((orgs) => {
                this.sourceOrgOptions = orgs.map((o) => ({
                    Value: o.GisUploadSourceOrganizationID,
                    Label: o.ProgramDisplayName,
                    disabled: false,
                }));
            }),
        );
    }

    continue(): void {
        const sourceOrgID = this.sourceOrgControl.value;
        if (!sourceOrgID) {
            this.addLocalAlert("Please select a source organization.");
            return;
        }

        this.isCreating = true;
        this.localAlerts = [];

        this.gisBulkImportService.createAttemptGisBulkImport({ GisUploadSourceOrganizationID: sourceOrgID }).subscribe({
            next: (detail) => {
                this.ref.close(detail.GisUploadAttemptID);
            },
            error: () => {
                this.addLocalAlert("Failed to create import attempt.");
                this.isCreating = false;
            },
        });
    }
}
