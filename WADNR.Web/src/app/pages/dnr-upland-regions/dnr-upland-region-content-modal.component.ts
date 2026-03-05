import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { DNRUplandRegionDetail } from "src/app/shared/generated/model/dnr-upland-region-detail";
import { DNRUplandRegionUpsertRequest } from "src/app/shared/generated/model/dnr-upland-region-upsert-request";

export interface DNRUplandRegionContentModalData {
    dnrUplandRegion: DNRUplandRegionDetail;
}

@Component({
    selector: "dnr-upland-region-content-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit Page Content</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                <form [formGroup]="form">
                    <form-field
                        [formControl]="form.controls.RegionContent"
                        fieldLabel="Page Content"
                        [type]="FormFieldType.RTE">
                    </form-field>
                </form>
            </div>
            <div class="modal-footer">
                <button
                    class="btn btn-primary"
                    (click)="save()"
                    [buttonLoading]="isSubmitting"
                    [disabled]="isSubmitting">
                    Save
                </button>
                <button
                    class="btn btn-secondary"
                    (click)="cancel()"
                    [disabled]="isSubmitting">
                    Cancel
                </button>
            </div>
        </div>
    `,
})
export class DNRUplandRegionContentModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<DNRUplandRegionContentModalData, DNRUplandRegionDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;

    public form = new FormGroup({
        RegionContent: new FormControl<string>(""),
    });

    private dnrUplandRegion: DNRUplandRegionDetail;

    constructor(
        private dnrUplandRegionService: DNRUplandRegionService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.dnrUplandRegion = data.dnrUplandRegion;

        this.form.patchValue({
            RegionContent: this.dnrUplandRegion.RegionContent ?? "",
        });
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const dto = new DNRUplandRegionUpsertRequest({
            DNRUplandRegionName: this.dnrUplandRegion.DNRUplandRegionName,
            DNRUplandRegionAbbrev: this.dnrUplandRegion.DNRUplandRegionAbbrev,
            RegionAddress: this.dnrUplandRegion.RegionAddress,
            RegionCity: this.dnrUplandRegion.RegionCity,
            RegionState: this.dnrUplandRegion.RegionState,
            RegionZip: this.dnrUplandRegion.RegionZip,
            RegionPhone: this.dnrUplandRegion.RegionPhone,
            RegionEmail: this.dnrUplandRegion.RegionEmail,
            RegionContent: this.form.value.RegionContent,
        });

        this.dnrUplandRegionService.updateDNRUplandRegion(this.dnrUplandRegion.DNRUplandRegionID, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Page content updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
