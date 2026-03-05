import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType, SelectDropdownOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { SupportRequestService } from "src/app/shared/generated/api/support-request.service";
import { SupportRequestCreate } from "src/app/shared/generated/model/support-request-create";
import { SupportRequestTypeEnum, SupportRequestTypesAsSelectDropdownOptions } from "src/app/shared/generated/enum/support-request-type-enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AuthenticationService } from "src/app/services/authentication.service";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";

export interface FeedbackModalData {
    currentPageUrl?: string;
    defaultSupportRequestType?: SupportRequestTypeEnum;
}

@Component({
    selector: "feedback-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./feedback-modal.component.html",
    styleUrls: ["./feedback-modal.component.scss"]
})
export class FeedbackModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<FeedbackModalData, boolean> = inject(DialogRef);
    public FormFieldType = FormFieldType;
    public isSubmitting = false;
    public currentUser: PersonDetail | null = null;
    public maxDescriptionLength = 2000;

    public supportRequestTypeOptions: SelectDropdownOption[] = SupportRequestTypesAsSelectDropdownOptions
        .slice()
        .sort((a, b) => (a.SortOrder ?? 0) - (b.SortOrder ?? 0));

    public form = new FormGroup({
        SupportRequestTypeID: new FormControl<number>(SupportRequestTypeEnum.ProvideFeedback, { validators: [Validators.required] }),
        RequestDescription: new FormControl<string>("", { validators: [Validators.required, Validators.maxLength(2000)] }),
        RequestPersonOrganization: new FormControl<string>("", { validators: [Validators.maxLength(500)] }),
        RequestPersonPhone: new FormControl<string>("", { validators: [Validators.maxLength(50)] })
    });

    constructor(
        private supportRequestService: SupportRequestService,
        private authenticationService: AuthenticationService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        if (this.ref.data?.defaultSupportRequestType != null) {
            this.form.patchValue({ SupportRequestTypeID: this.ref.data.defaultSupportRequestType });
        }

        this.authenticationService.getCurrentUser().subscribe(user => {
            this.currentUser = user;
            // Pre-populate organization if available
            if (user?.OrganizationName && !this.form.value.RequestPersonOrganization) {
                this.form.patchValue({ RequestPersonOrganization: user.OrganizationName });
            }
        });
    }

    get descriptionLength(): number {
        return this.form.value.RequestDescription?.length ?? 0;
    }

    get charactersRemaining(): number {
        return this.maxDescriptionLength - this.descriptionLength;
    }

    submit(): void {
        if (this.form.invalid || this.isSubmitting) return;

        this.isSubmitting = true;
        this.localAlerts = [];

        const dto: SupportRequestCreate = {
            SupportRequestTypeID: this.form.value.SupportRequestTypeID!,
            RequestDescription: this.form.value.RequestDescription!,
            RequestPersonOrganization: this.form.value.RequestPersonOrganization || undefined,
            RequestPersonPhone: this.form.value.RequestPersonPhone || undefined,
            CurrentPageUrl: this.ref.data?.currentPageUrl || undefined
        };

        this.supportRequestService.createSupportRequest(dto).subscribe({
            next: () => {
                this.pushGlobalSuccess("Thank you for your feedback!");
                this.ref.close(true);
            },
            error: (err) => {
                const message = err?.error?.message ?? err?.message ?? "Failed to submit feedback. Please try again.";
                this.addLocalAlert(message, AlertContext.Danger, true);
                this.isSubmitting = false;
            }
        });
    }

    cancel(): void {
        this.ref.close(false);
    }
}
