import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { DNRUplandRegionDetail } from "src/app/shared/generated/model/dnr-upland-region-detail";
import {
    DNRUplandRegionUpsertRequest,
    DNRUplandRegionUpsertRequestForm,
    DNRUplandRegionUpsertRequestFormControls
} from "src/app/shared/generated/model/dnr-upland-region-upsert-request";

export interface DNRUplandRegionContactModalData {
    dnrUplandRegion: DNRUplandRegionDetail;
}

@Component({
    selector: "dnr-upland-region-contact-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit Contact Information</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts()" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                <form [formGroup]="form">
                    <form-field
                        [formControl]="form.controls.RegionAddress"
                        fieldLabel="Address"
                        [type]="FormFieldType.Text">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.RegionCity"
                        fieldLabel="City"
                        [type]="FormFieldType.Text">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.RegionState"
                        fieldLabel="State"
                        [type]="FormFieldType.Text">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.RegionZip"
                        fieldLabel="Zip"
                        [type]="FormFieldType.Text">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.RegionPhone"
                        fieldLabel="Phone"
                        [type]="FormFieldType.Text">
                    </form-field>

                    <form-field
                        [formControl]="form.controls.RegionEmail"
                        fieldLabel="Email"
                        [type]="FormFieldType.Text">
                    </form-field>

                    <hr>
                    <form-field
                        [formControl]="form.controls.DNRUplandRegionCoordinatorPersonID"
                        fieldDefinitionName="ServiceForestryRegionalCoordinator"
                        [type]="FormFieldType.Select"
                        [formInputOptions]="personOptions"
                        formInputOptionLabel="Label"
                        formInputOptionValue="Value"
                        placeholder="Select coordinator">
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
export class DNRUplandRegionContactModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<DNRUplandRegionContactModalData, DNRUplandRegionDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public personOptions: FormInputOption[] = [];

    public form = new FormGroup({
        RegionAddress: DNRUplandRegionUpsertRequestFormControls.RegionAddress(),
        RegionCity: DNRUplandRegionUpsertRequestFormControls.RegionCity(),
        RegionState: DNRUplandRegionUpsertRequestFormControls.RegionState(),
        RegionZip: DNRUplandRegionUpsertRequestFormControls.RegionZip(),
        RegionPhone: DNRUplandRegionUpsertRequestFormControls.RegionPhone(),
        RegionEmail: DNRUplandRegionUpsertRequestFormControls.RegionEmail(),
        DNRUplandRegionCoordinatorPersonID: new FormControl<number | null>(null),
    });

    private dnrUplandRegion: DNRUplandRegionDetail;

    constructor(
        private dnrUplandRegionService: DNRUplandRegionService,
        private personService: PersonService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.dnrUplandRegion = data.dnrUplandRegion;

        this.personService.listWadnrLookupPerson().subscribe({
            next: (people) => {
                this.personOptions = people.map(p => ({
                    Value: p.PersonID,
                    Label: (p.OrganizationShortName || p.OrganizationName) ? `${p.FullName} (${p.OrganizationShortName ?? p.OrganizationName})` : p.FullName,
                    disabled: false,
                }));
                // Patch coordinator value after options are loaded so select can match
                this.form.patchValue({
                    DNRUplandRegionCoordinatorPersonID: this.dnrUplandRegion.DNRUplandRegionCoordinatorID,
                });
            },
            error: () => {
            }
        });

        this.form.patchValue({
            RegionAddress: this.dnrUplandRegion.RegionAddress,
            RegionCity: this.dnrUplandRegion.RegionCity,
            RegionState: this.dnrUplandRegion.RegionState,
            RegionZip: this.dnrUplandRegion.RegionZip,
            RegionPhone: this.dnrUplandRegion.RegionPhone,
            RegionEmail: this.dnrUplandRegion.RegionEmail,
        });
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts.set([]);

        const dto = new DNRUplandRegionUpsertRequest({
            DNRUplandRegionName: this.dnrUplandRegion.DNRUplandRegionName,
            DNRUplandRegionAbbrev: this.dnrUplandRegion.DNRUplandRegionAbbrev,
            RegionContent: this.dnrUplandRegion.RegionContent,
            RegionAddress: this.form.value.RegionAddress,
            RegionCity: this.form.value.RegionCity,
            RegionState: this.form.value.RegionState,
            RegionZip: this.form.value.RegionZip,
            RegionPhone: this.form.value.RegionPhone,
            RegionEmail: this.form.value.RegionEmail,
            DNRUplandRegionCoordinatorPersonID: this.form.value.DNRUplandRegionCoordinatorPersonID,
        });

        this.dnrUplandRegionService.updateDNRUplandRegion(this.dnrUplandRegion.DNRUplandRegionID, dto).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Contact information updated successfully.");
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
