import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin } from "rxjs";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { FederalFundCodeService } from "src/app/shared/generated/api/federal-fund-code.service";
import { FundSourceAllocationPriorityService } from "src/app/shared/generated/api/fund-source-allocation-priority.service";
import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { FundSourceAllocationDetail } from "src/app/shared/generated/model/fund-source-allocation-detail";
import { DivisionsAsSelectDropdownOptions } from "src/app/shared/generated/enum/division-enum";
import { FundSourceAllocationSourcesAsSelectDropdownOptions } from "src/app/shared/generated/enum/fund-source-allocation-source-enum";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

export interface EditModalInput {
    allocation: FundSourceAllocationDetail;
    mode: "edit" | "create";
}

@Component({
    selector: "fund-source-allocation-edit-modal",
    standalone: true,
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ data.mode === "create" ? "Create Fund Source Allocation" : "Edit Fund Source Allocation" }}</h3>
            </div>
            <div class="modal-body">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                @if (isLoadingLookups$ | async) {
                    <p>Loading...</p>
                } @else {
                <form [formGroup]="form">
                    <div class="grid-12">
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceID" fieldDefinitionName="FundSource"
                                [type]="FormFieldType.Select" [formInputOptions]="fundSourceOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceAllocationName" fieldDefinitionName="FundSourceAllocationName"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.organizationID" fieldDefinitionName="Organization"
                                [type]="FormFieldType.Select" [formInputOptions]="organizationOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceManagerID" fieldDefinitionName="FundSourceManager"
                                [type]="FormFieldType.Select" [formInputOptions]="personOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.programManagerPersonIDs" fieldDefinitionName="ProgramManager"
                                [type]="FormFieldType.Select" [formInputOptions]="personOptions"
                                [multiple]="true"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.federalFundCodeID" fieldDefinitionName="FederalFundCode"
                                [type]="FormFieldType.Select" [formInputOptions]="federalFundCodeOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.dnrUplandRegionID" fieldDefinitionName="DNRUplandRegion"
                                [type]="FormFieldType.Select" [formInputOptions]="regionOptions"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.divisionID" fieldDefinitionName="Division"
                                [type]="FormFieldType.Select" [formInputOptions]="divisionOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.allocationAmount" fieldDefinitionName="AllocationAmount"
                                [type]="FormFieldType.Number"></form-field>
                        </div>
                        @if (amountChanged) {
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.allocationAmountChangeNote"
                                fieldDefinitionName="FundSourceAllocationChangeLogNote"
                                [type]="FormFieldType.Textarea"></form-field>
                        </div>
                        }
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.startDate" fieldDefinitionName="FundSourceStartDate"
                                [type]="FormFieldType.Date"></form-field>
                        </div>
                        <div class="g-col-6">
                            <form-field [formControl]="form.controls.endDate" fieldDefinitionName="FundSourceEndDate"
                                [type]="FormFieldType.Date"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceAllocationPriorityID" fieldDefinitionName="FundSourceAllocationPriority"
                                [type]="FormFieldType.Select" [formInputOptions]="priorityOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.hasFundFSPs" fieldDefinitionName="FundSourceAllocationFundFSPs"
                                [type]="FormFieldType.Radio" [formInputOptions]="hasFundFSPsOptions" name="hasFundFSPs"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceAllocationSourceID" fieldDefinitionName="FundSourceAllocationSource"
                                [type]="FormFieldType.Select" [formInputOptions]="sourceOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.likelyToUse" fieldDefinitionName="FundSourceAllocationLikelyToUse"
                                [type]="FormFieldType.Radio" [formInputOptions]="likelyToUseOptions" name="likelyToUse"></form-field>
                        </div>
                        @if (form.controls.likelyToUse.value === true) {
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.likelyToUsePersonIDs" fieldLabel="Likely to Use People"
                                [type]="FormFieldType.Select" [formInputOptions]="personOptions"
                                [multiple]="true"></form-field>
                        </div>
                        }
                        @if (data.mode === "create") {
                        <div class="g-col-12">
                            <form-field [formControl]="filesControl" [type]="FormFieldType.File" [multiple]="true"
                                fieldLabel="Files"></form-field>
                        </div>
                        }
                    </div>
                </form>
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
                <button class="btn btn-primary" [disabled]="isSubmitting || (isLoadingLookups$ | async)" (click)="save()">
                    {{ isSubmitting ? "Saving..." : "Save" }}
                </button>
            </div>
        </div>
    `,
})
export class FundSourceAllocationEditModalComponent extends BaseModal implements OnInit {
    FormFieldType = FormFieldType;

    data: EditModalInput;
    isSubmitting = false;
    isLoadingLookups$ = new BehaviorSubject<boolean>(true);
    filesControl = new FormControl<File[]>([], { nonNullable: true });

    organizationOptions: FormInputOption[] = [];
    fundSourceOptions: FormInputOption[] = [];
    personOptions: FormInputOption[] = [];
    federalFundCodeOptions: FormInputOption[] = [];
    priorityOptions: FormInputOption[] = [];
    divisionOptions: FormInputOption[] = [];
    regionOptions: FormInputOption[] = [];
    sourceOptions: FormInputOption[] = [];
    hasFundFSPsOptions: FormInputOption[] = [
        { Value: true, Label: "Yes", disabled: false },
        { Value: false, Label: "No", disabled: false },
        { Value: null, Label: "N/A", disabled: false },
    ];
    likelyToUseOptions: FormInputOption[] = [
        { Value: false, Label: "Contractual Only", disabled: false },
        { Value: true, Label: "List of Users", disabled: false },
        { Value: null, Label: "N/A", disabled: false },
    ];

    form = new FormGroup({
        fundSourceAllocationName: new FormControl<string | null>(null, Validators.required),
        fundSourceID: new FormControl<number | null>(null, Validators.required),
        allocationAmount: new FormControl<number | null>(null),
        startDate: new FormControl<string | null>(null),
        endDate: new FormControl<string | null>(null),
        organizationID: new FormControl<number | null>(null, Validators.required),
        dnrUplandRegionID: new FormControl<number | null>(null),
        divisionID: new FormControl<number | null>(null),
        fundSourceManagerID: new FormControl<number | null>(null),
        federalFundCodeID: new FormControl<number | null>(null),
        fundSourceAllocationPriorityID: new FormControl<number | null>(null),
        fundSourceAllocationSourceID: new FormControl<number | null>(null),
        hasFundFSPs: new FormControl<boolean | null>(null),
        likelyToUse: new FormControl<boolean | null>(null),
        programManagerPersonIDs: new FormControl<number[]>([], { nonNullable: true }),
        likelyToUsePersonIDs: new FormControl<number[]>([], { nonNullable: true }),
        allocationAmountChangeNote: new FormControl<string | null>(null),
    });

    private originalAmount: number | null = null;

    constructor(
        public ref: DialogRef<EditModalInput, number | boolean>,
        private fundSourceAllocationService: FundSourceAllocationService,
        private organizationService: OrganizationService,
        private personService: PersonService,
        private fundSourceService: FundSourceService,
        private federalFundCodeService: FederalFundCodeService,
        private priorityService: FundSourceAllocationPriorityService,
        private regionService: DNRUplandRegionService,
    ) {
        super();
        this.data = ref.data!;
    }

    ngOnInit(): void {
        const a = this.data.allocation;
        this.originalAmount = a.AllocationAmount ?? null;

        this.form.patchValue({
            fundSourceAllocationName: a.FundSourceAllocationName,
            fundSourceID: a.FundSourceID,
            allocationAmount: a.AllocationAmount,
            startDate: a.StartDate ? new Date(a.StartDate).toISOString().split("T")[0] : null,
            endDate: a.EndDate ? new Date(a.EndDate).toISOString().split("T")[0] : null,
            organizationID: a.OrganizationID,
            dnrUplandRegionID: a.DNRUplandRegionID,
            divisionID: a.DivisionID,
            fundSourceManagerID: a.FundSourceManagerID,
            federalFundCodeID: a.FederalFundCodeID,
            fundSourceAllocationPriorityID: a.FundSourceAllocationPriorityID,
            fundSourceAllocationSourceID: a.FundSourceAllocationSourceID,
            hasFundFSPs: a.HasFundFSPs,
            likelyToUse: a.LikelyToUse,
            programManagerPersonIDs: a.ProgramManagers?.map(p => p.PersonID) ?? [],
            likelyToUsePersonIDs: a.LikelyToUsePeople?.map(p => p.PersonID) ?? [],
        });

        this.divisionOptions = DivisionsAsSelectDropdownOptions;
        this.sourceOptions = FundSourceAllocationSourcesAsSelectDropdownOptions;

        forkJoin({
            organizations: this.organizationService.listLookupOrganization(),
            fundSources: this.fundSourceService.listLookupFundSource(),
            people: this.personService.listLookupPerson(),
            federalFundCodes: this.federalFundCodeService.listLookupFederalFundCode(),
            priorities: this.priorityService.listLookupFundSourceAllocationPriority(),
            regions: this.regionService.listLookupDNRUplandRegion(),
        }).subscribe({
            next: ({ organizations, fundSources, people, federalFundCodes, priorities, regions }) => {
                this.organizationOptions = organizations.map(o => ({ Value: o.OrganizationID, Label: o.OrganizationName ?? "", disabled: false }));
                this.fundSourceOptions = fundSources.map(f => ({ Value: f.FundSourceID, Label: (f.FundSourceNumber ?? "") + (f.FundSourceName ? " — " + f.FundSourceName : ""), disabled: false }));
                this.personOptions = people.map(p => ({ Value: p.PersonID, Label: p.FullName ?? "", disabled: false }));
                this.federalFundCodeOptions = federalFundCodes.map(f => ({ Value: f.FederalFundCodeID, Label: f.FederalFundCodeAbbrev ?? "", disabled: false }));
                this.priorityOptions = priorities.map(p => ({ Value: p.FundSourceAllocationPriorityID, Label: String(p.FundSourceAllocationPriorityNumber), disabled: false }));
                this.regionOptions = regions.map(r => ({ Value: r.DNRUplandRegionID, Label: r.DNRUplandRegionName ?? "", disabled: false }));
                this.isLoadingLookups$.next(false);
            },
            error: () => {
                this.addLocalAlert("Failed to load lookups.", AlertContext.Danger, true);
                this.isLoadingLookups$.next(false);
            },
        });
    }

    get amountChanged(): boolean {
        if (this.data.mode === "create") return false;
        return this.form.controls.allocationAmount.value !== this.originalAmount;
    }

    save(): void {
        if (this.isSubmitting) return;

        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        if (this.amountChanged && !this.form.controls.allocationAmountChangeNote.value) {
            this.addLocalAlert("Please provide a reason for the allocation amount change.", AlertContext.Danger, true);
            return;
        }

        this.isSubmitting = true;

        const dto = {
            FundSourceAllocationName: this.form.controls.fundSourceAllocationName.value,
            FundSourceID: this.form.controls.fundSourceID.value!,
            AllocationAmount: this.form.controls.allocationAmount.value,
            StartDate: this.form.controls.startDate.value,
            EndDate: this.form.controls.endDate.value,
            OrganizationID: this.form.controls.organizationID.value,
            DNRUplandRegionID: this.form.controls.dnrUplandRegionID.value,
            DivisionID: this.form.controls.divisionID.value,
            FundSourceManagerID: this.form.controls.fundSourceManagerID.value,
            FederalFundCodeID: this.form.controls.federalFundCodeID.value,
            FundSourceAllocationPriorityID: this.form.controls.fundSourceAllocationPriorityID.value,
            FundSourceAllocationSourceID: this.form.controls.fundSourceAllocationSourceID.value,
            HasFundFSPs: this.form.controls.hasFundFSPs.value,
            LikelyToUse: this.form.controls.likelyToUse.value,
            ProgramManagerPersonIDs: this.form.controls.programManagerPersonIDs.value,
            LikelyToUsePersonIDs: this.form.controls.likelyToUsePersonIDs.value,
            AllocationAmountChangeNote: this.form.controls.allocationAmountChangeNote.value,
        };

        if (this.data.mode === "create") {
            this.fundSourceAllocationService.createFundSourceAllocation(dto).subscribe({
                next: (result) => {
                    const newID = result?.FundSourceAllocationID;
                    const files = this.filesControl.value;
                    if (newID && files.length > 0) {
                        const uploads = files.map(f => {
                            const name = f.name;
                            const dotIndex = name.lastIndexOf(".");
                            const displayName = dotIndex > 0 ? name.slice(0, dotIndex) : name;
                            return this.fundSourceAllocationService.uploadFileFundSourceAllocation(newID, displayName, undefined, f);
                        });
                        forkJoin(uploads).subscribe({
                            next: () => this.ref.close(newID),
                            error: () => this.ref.close(newID),
                        });
                    } else {
                        this.ref.close(newID ?? true);
                    }
                },
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        } else {
            this.fundSourceAllocationService.updateFundSourceAllocation(this.data.allocation.FundSourceAllocationID, dto).subscribe({
                next: () => this.ref.close(true),
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        }
    }
}
