import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, of } from "rxjs";
import { catchError, switchMap } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { OrganizationService } from "src/app/shared/generated/api/organization.service";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { FederalFundCodeService } from "src/app/shared/generated/api/federal-fund-code.service";
import { FundSourceAllocationPriorityService } from "src/app/shared/generated/api/fund-source-allocation-priority.service";
import { DNRUplandRegionService } from "src/app/shared/generated/api/dnr-upland-region.service";
import { ProgramIndexService } from "src/app/shared/generated/api/program-index.service";
import { ProjectCodeService } from "src/app/shared/generated/api/project-code.service";
import { FundSourceAllocationDetail } from "src/app/shared/generated/model/fund-source-allocation-detail";
import { FundSourceAllocationProgramIndexProjectCodeItem } from "src/app/shared/generated/model/fund-source-allocation-program-index-project-code-item";
import { FundSourceAllocationProgramIndexProjectCodeSaveRequest } from "src/app/shared/generated/model/fund-source-allocation-program-index-project-code-save-request";
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
    imports: [AsyncPipe, ReactiveFormsModule, FormFieldComponent, IconComponent, FieldDefinitionComponent, ModalAlertsComponent, LoadingDirective, ButtonLoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>{{ data.mode === "create" ? "Create Fund Source Allocation" : "Edit Fund Source Allocation" }}</h3>
            </div>
            <div class="modal-body" [loadingSpinner]="{ isLoading: isLoadingLookups$ | async, loadingHeight: 200 }">
                <modal-alerts [alerts]="localAlerts()" (onClosed)="removeLocalAlert($event)"></modal-alerts>
                <p>If you do not see the contact listed for Fund Source Manager, you can <a href="/people" target="_blank">Add a Contact</a></p>
                @if (!(isLoadingLookups$ | async)) {
                <form [formGroup]="form">
                    <div class="grid-12">
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceID" fieldDefinitionName="FundSource"
                                [type]="FormFieldType.Select" [formInputOptions]="fundSourceOptions"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.fundSourceAllocationName" fieldDefinitionName="FundSourceAllocationName"
                                fieldDefinitionLabelOverride="Allocation Name"></form-field>
                        </div>
                        <div class="g-col-12">
                            <form-field [formControl]="form.controls.organizationID" fieldDefinitionName="Organization"
                                fieldDefinitionLabelOverride="Contributing Organization"
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
                                fieldDefinitionLabelOverride="Federal Job Code"
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
                                [type]="FormFieldType.Currency"></form-field>
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

                        <!-- Program Index / Project Code pairs -->
                        <div class="g-col-12">
                            <hr>
                            <div class="mb-2"><strong><field-definition fieldDefinition="ProgramIndexProjectCode"></field-definition></strong></div>
                            <div class="grid-12 align-items-end mb-2">
                                <div class="g-col-5">
                                    <form-field [formControl]="programIndexToAdd" fieldLabel="Program Index"
                                        [type]="FormFieldType.Select" [formInputOptions]="programIndexOptions"
                                        placeholder="Select program index..."></form-field>
                                </div>
                                <div class="g-col-5">
                                    <form-field [formControl]="projectCodeToAdd" fieldLabel="Project Code (optional)"
                                        [type]="FormFieldType.Select" [formInputOptions]="projectCodeOptions"
                                        placeholder="Select project code..."></form-field>
                                </div>
                                <div class="g-col-2" style="display: flex; align-items: flex-end; height: 100%;">
                                    <button class="btn btn-sm btn-primary w-100" (click)="addPair()"
                                        [disabled]="!programIndexToAdd.value || isDuplicate() || isSubmitting">Add</button>
                                </div>
                            </div>
                            @if (selectedPairs.length > 0) {
                                <table class="table table-sm table-striped">
                                    <thead>
                                        <tr>
                                            <th>Program Index</th>
                                            <th>Project Code</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        @for (pair of selectedPairs; track pairKey(pair)) {
                                            <tr>
                                                <td>{{ pair.ProgramIndexCode }}</td>
                                                <td>{{ pair.ProjectCodeName || '\u2014' }}</td>
                                                <td class="text-end">
                                                    <a href="#" class="text-danger" (click)="removePair(pair); $event.preventDefault()" title="Remove">
                                                        <icon icon="Delete"></icon>
                                                    </a>
                                                </td>
                                            </tr>
                                        }
                                    </tbody>
                                </table>
                            } @else {
                                <p class="text-muted"><em>No pairs assigned.</em></p>
                            }
                        </div>

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
                <button class="btn btn-primary" [disabled]="isSubmitting || (isLoadingLookups$ | async)" [buttonLoading]="isSubmitting" (click)="save()">Save</button>
                <button class="btn btn-secondary" (click)="ref.close(null)">Cancel</button>
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
    programIndexOptions: FormInputOption[] = [];
    projectCodeOptions: FormInputOption[] = [];

    selectedPairs: FundSourceAllocationProgramIndexProjectCodeItem[] = [];
    programIndexToAdd = new FormControl<number | null>(null);
    projectCodeToAdd = new FormControl<number | null>(null);
    private programIndexMap = new Map<number, string>();
    private projectCodeMap = new Map<number, string>();
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
        private programIndexService: ProgramIndexService,
        private projectCodeService: ProjectCodeService,
    ) {
        super();
        this.data = ref.data!;
    }

    ngOnInit(): void {
        const a = this.data.allocation;
        this.originalAmount = a.AllocationAmount ?? null;
        this.selectedPairs = [...(a.ProgramIndexProjectCodes ?? [])];

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

        // Division / Region mutual exclusion
        if (a.DivisionID) {
            this.form.controls.dnrUplandRegionID.disable({ emitEvent: false });
        } else if (a.DNRUplandRegionID) {
            this.form.controls.divisionID.disable({ emitEvent: false });
        }

        this.form.controls.divisionID.valueChanges.subscribe((value) => {
            if (value) {
                this.form.controls.dnrUplandRegionID.setValue(null, { emitEvent: false });
                this.form.controls.dnrUplandRegionID.disable({ emitEvent: false });
            } else {
                this.form.controls.dnrUplandRegionID.enable({ emitEvent: false });
            }
        });

        this.form.controls.dnrUplandRegionID.valueChanges.subscribe((value) => {
            if (value) {
                this.form.controls.divisionID.setValue(null, { emitEvent: false });
                this.form.controls.divisionID.disable({ emitEvent: false });
            } else {
                this.form.controls.divisionID.enable({ emitEvent: false });
            }
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
            programIndices: this.programIndexService.listLookupProgramIndex().pipe(catchError(() => of([]))),
            projectCodes: this.projectCodeService.listLookupProjectCode().pipe(catchError(() => of([]))),
        }).subscribe({
            next: ({ organizations, fundSources, people, federalFundCodes, priorities, regions, programIndices, projectCodes }) => {
                this.organizationOptions = organizations.map(o => ({ Value: o.OrganizationID, Label: o.OrganizationName ?? "", disabled: false }));
                this.fundSourceOptions = fundSources.map(f => ({ Value: f.FundSourceID, Label: (f.FundSourceNumber ?? "") + (f.FundSourceName ? " — " + f.FundSourceName : ""), disabled: false }));
                this.personOptions = people.map(p => ({ Value: p.PersonID, Label: p.FullName ?? "", disabled: false }));
                this.federalFundCodeOptions = federalFundCodes.map(f => ({ Value: f.FederalFundCodeID, Label: f.FederalFundCodeAbbrev ?? "", disabled: false }));
                this.priorityOptions = priorities.map(p => ({ Value: p.FundSourceAllocationPriorityID, Label: String(p.FundSourceAllocationPriorityNumber), disabled: false }));
                this.regionOptions = regions.map(r => ({ Value: r.DNRUplandRegionID, Label: r.DNRUplandRegionName ?? "", disabled: false }));
                this.programIndexOptions = programIndices.map((pi: any) => ({ Value: pi.ProgramIndexID, Label: pi.ProgramIndexCode ?? pi.DisplayName ?? `PI ${pi.ProgramIndexID}`, disabled: false }));
                this.programIndexMap = new Map(programIndices.map((pi: any) => [pi.ProgramIndexID, pi.ProgramIndexCode ?? pi.DisplayName ?? ""] as [number, string]));
                this.projectCodeOptions = projectCodes.map((pc: any) => ({ Value: pc.ProjectCodeID, Label: pc.ProjectCodeName ?? `PC ${pc.ProjectCodeID}`, disabled: false }));
                this.projectCodeMap = new Map(projectCodes.map((pc: any) => [pc.ProjectCodeID, pc.ProjectCodeName ?? ""] as [number, string]));
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

    pairKey(pair: FundSourceAllocationProgramIndexProjectCodeItem): string {
        return `${pair.ProgramIndexID}-${pair.ProjectCodeID ?? "null"}`;
    }

    isDuplicate(): boolean {
        const piID = this.programIndexToAdd.value;
        const pcID = this.projectCodeToAdd.value ?? null;
        if (!piID) return false;
        return this.selectedPairs.some((p) => p.ProgramIndexID === piID && (p.ProjectCodeID ?? null) === pcID);
    }

    addPair(): void {
        const piID = this.programIndexToAdd.value;
        if (!piID || this.isDuplicate()) return;
        const pcID = this.projectCodeToAdd.value ?? null;

        const newPair = new FundSourceAllocationProgramIndexProjectCodeItem({
            ProgramIndexID: piID,
            ProgramIndexCode: this.programIndexMap.get(piID) ?? "",
            ProjectCodeID: pcID,
            ProjectCodeName: pcID ? (this.projectCodeMap.get(pcID) ?? "") : null,
        });

        this.selectedPairs = [...this.selectedPairs, newPair];
        this.programIndexToAdd.setValue(null, { emitEvent: false });
        this.projectCodeToAdd.setValue(null, { emitEvent: false });
    }

    removePair(pair: FundSourceAllocationProgramIndexProjectCodeItem): void {
        this.selectedPairs = this.selectedPairs.filter(
            (p) => !(p.ProgramIndexID === pair.ProgramIndexID && (p.ProjectCodeID ?? null) === (pair.ProjectCodeID ?? null))
        );
    }

    private buildPipcSaveRequest(): FundSourceAllocationProgramIndexProjectCodeSaveRequest {
        return new FundSourceAllocationProgramIndexProjectCodeSaveRequest({
            Pairs: this.selectedPairs.map((p) => ({
                ProgramIndexID: p.ProgramIndexID,
                ProjectCodeID: p.ProjectCodeID ?? null,
            })),
        });
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

        const raw = this.form.getRawValue();
        const dto = {
            FundSourceAllocationName: raw.fundSourceAllocationName,
            FundSourceID: raw.fundSourceID!,
            AllocationAmount: raw.allocationAmount,
            StartDate: raw.startDate,
            EndDate: raw.endDate,
            OrganizationID: raw.organizationID,
            DNRUplandRegionID: raw.dnrUplandRegionID,
            DivisionID: raw.divisionID,
            FundSourceManagerID: raw.fundSourceManagerID,
            FederalFundCodeID: raw.federalFundCodeID,
            FundSourceAllocationPriorityID: raw.fundSourceAllocationPriorityID,
            FundSourceAllocationSourceID: raw.fundSourceAllocationSourceID,
            HasFundFSPs: raw.hasFundFSPs,
            LikelyToUse: raw.likelyToUse,
            ProgramManagerPersonIDs: raw.programManagerPersonIDs,
            LikelyToUsePersonIDs: raw.likelyToUsePersonIDs,
            AllocationAmountChangeNote: raw.allocationAmountChangeNote,
        };

        if (this.data.mode === "create") {
            this.fundSourceAllocationService.createFundSourceAllocation(dto).subscribe({
                next: (result) => {
                    const newID = result?.FundSourceAllocationID;
                    if (!newID) {
                        this.ref.close(true);
                        return;
                    }
                    const files = this.filesControl.value;
                    const followUps: any[] = [];
                    if (this.selectedPairs.length > 0) {
                        followUps.push(this.fundSourceAllocationService.saveProgramIndexProjectCodesFundSourceAllocation(newID, this.buildPipcSaveRequest()));
                    }
                    if (files.length > 0) {
                        const uploads = files.map(f => {
                            const name = f.name;
                            const dotIndex = name.lastIndexOf(".");
                            const displayName = dotIndex > 0 ? name.slice(0, dotIndex) : name;
                            return this.fundSourceAllocationService.uploadFileFundSourceAllocation(newID, displayName, undefined, f);
                        });
                        followUps.push(...uploads);
                    }
                    if (followUps.length > 0) {
                        forkJoin(followUps).subscribe({
                            next: () => this.ref.close(newID),
                            error: () => this.ref.close(newID),
                        });
                    } else {
                        this.ref.close(newID);
                    }
                },
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        } else {
            const allocationID = this.data.allocation.FundSourceAllocationID;
            this.fundSourceAllocationService.updateFundSourceAllocation(allocationID, dto).pipe(
                switchMap(() => this.fundSourceAllocationService.saveProgramIndexProjectCodesFundSourceAllocation(allocationID, this.buildPipcSaveRequest()))
            ).subscribe({
                next: () => this.ref.close(true),
                error: (err) => {
                    this.isSubmitting = false;
                    this.addLocalAlert(err?.error || "An error occurred.", AlertContext.Danger, true);
                },
            });
        }
    }
}
