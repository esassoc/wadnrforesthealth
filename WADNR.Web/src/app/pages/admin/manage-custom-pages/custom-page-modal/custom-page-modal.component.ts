import { Component, inject, OnInit, signal } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";

import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { CustomPageService } from "src/app/shared/generated/api/custom-page.service";
import { CustomPageGridRow } from "src/app/shared/generated/model/custom-page-grid-row";
import { CustomPageUpsertRequest } from "src/app/shared/generated/model/custom-page-upsert-request";
import { CustomPageDisplayTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/custom-page-display-type-enum";
import { CustomPageNavigationSectionsAsSelectDropdownOptions } from "src/app/shared/generated/enum/custom-page-navigation-section-enum";

export interface CustomPageModalData {
    mode: "create" | "edit";
    customPage?: CustomPageGridRow;
}

@Component({
    selector: "custom-page-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent, ButtonLoadingDirective],
    templateUrl: "./custom-page-modal.component.html",
})
export class CustomPageModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<CustomPageModalData, CustomPageGridRow | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public customPage?: CustomPageGridRow;
    public isSubmitting = signal(false);

    public displayTypeOptions: SelectDropdownOption[] = CustomPageDisplayTypesAsSelectDropdownOptions;
    public navSectionOptions: SelectDropdownOption[] = CustomPageNavigationSectionsAsSelectDropdownOptions;

    public form = new FormGroup({
        CustomPageDisplayName: new FormControl("", [Validators.required, Validators.maxLength(100)]),
        CustomPageVanityUrl: new FormControl("", [Validators.required, Validators.maxLength(100)]),
        CustomPageDisplayTypeID: new FormControl<number | null>(null, [Validators.required]),
        CustomPageNavigationSectionID: new FormControl<number | null>(null, [Validators.required]),
    });

    constructor(
        private customPageService: CustomPageService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.customPage = data?.customPage;

        if (this.mode === "edit" && this.customPage) {
            this.form.patchValue({
                CustomPageDisplayName: this.customPage.CustomPageDisplayName,
                CustomPageVanityUrl: this.customPage.CustomPageVanityUrl,
                CustomPageDisplayTypeID: this.customPage.CustomPageDisplayTypeID,
                CustomPageNavigationSectionID: this.customPage.CustomPageNavigationSectionID,
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Custom Page" : "Edit Custom Page";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting.set(true);
        this.localAlerts = [];

        const dto = new CustomPageUpsertRequest({
            CustomPageDisplayName: this.form.controls.CustomPageDisplayName.value,
            CustomPageVanityUrl: this.form.controls.CustomPageVanityUrl.value,
            CustomPageDisplayTypeID: this.form.controls.CustomPageDisplayTypeID.value,
            CustomPageNavigationSectionID: this.form.controls.CustomPageNavigationSectionID.value,
        });

        const request$ = this.mode === "create"
            ? this.customPageService.createCustomPage(dto)
            : this.customPageService.updateCustomPage(this.customPage!.CustomPageID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Custom page created successfully."
                    : "Custom page updated successfully.";
                this.pushGlobalSuccess(message);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting.set(false);
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            },
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
