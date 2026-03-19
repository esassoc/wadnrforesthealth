import { Component, inject, OnInit } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { PersonService } from "src/app/shared/generated/api/person.service";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { StewardshipAreaItem } from "src/app/shared/generated/model/stewardship-area-item";
import { SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";

export interface PersonEditStewardshipAreasModalData {
    person: PersonDetail;
    allRegions: StewardshipAreaItem[];
}

@Component({
    selector: "person-edit-stewardship-areas-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./person-edit-stewardship-areas-modal.component.html",
})
export class PersonEditStewardshipAreasModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<PersonEditStewardshipAreasModalData, PersonDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isSubmitting = false;

    public regionOptions: SelectDropdownOption[] = [];

    public form = new FormGroup({
        RegionIDs: new FormControl<number[]>([]),
    });

    constructor(
        private personService: PersonService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;

        this.regionOptions = data.allRegions.map((r) => ({
            Value: r.ID,
            Label: r.Name,
            disabled: false,
        }));

        const currentRegionIDs = (data.person.StewardRegions ?? []).map((r) => r.ID);
        this.form.patchValue({ RegionIDs: currentRegionIDs });
    }

    save(): void {
        this.isSubmitting = true;
        this.localAlerts.set([]);

        const rawValue = this.form.getRawValue();
        const request = {
            DNRUplandRegionIDs: rawValue.RegionIDs ?? [],
        };

        this.personService.updateStewardshipAreasPerson(this.ref.data.person.PersonID, request).subscribe({
            next: (result) => {
                this.pushGlobalSuccess("Stewardship areas updated successfully.");
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.ErrorMessage ?? err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
