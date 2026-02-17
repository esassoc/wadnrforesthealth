import { CommonModule } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { BehaviorSubject, forkJoin, of } from "rxjs";
import { catchError } from "rxjs/operators";

import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { ProgramIndexService } from "src/app/shared/generated/api/program-index.service";
import { ProjectCodeService } from "src/app/shared/generated/api/project-code.service";
import { FundSourceAllocationProgramIndexProjectCodeItem } from "src/app/shared/generated/model/fund-source-allocation-program-index-project-code-item";
import { FundSourceAllocationProgramIndexProjectCodeSaveRequest } from "src/app/shared/generated/model/fund-source-allocation-program-index-project-code-save-request";

export interface EditProgramIndexProjectCodesModalData {
    fundSourceAllocationID: number;
    existingPairs: FundSourceAllocationProgramIndexProjectCodeItem[];
}

@Component({
    selector: "edit-program-index-project-codes-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent, IconComponent, ModalAlertsComponent, LoadingDirective],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h3>Edit Program Index / Project Codes</h3>
            </div>
            <div class="modal-body" [loadingSpinner]="{ isLoading: isLoading$ | async, loadingHeight: 200 }">
                <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>

                <p class="mb-3">Add or remove program index / project code pairs for this allocation.</p>

                <div class="grid-12 align-items-end mb-3">
                    <div class="g-col-5">
                        <form-field
                            [formControl]="programIndexToAdd"
                            fieldLabel="Program Index"
                            [type]="FormFieldType.Select"
                            [formInputOptions]="programIndexOptions"
                            placeholder="Select program index..."
                            [readOnly]="isSubmitting">
                        </form-field>
                    </div>
                    <div class="g-col-5">
                        <form-field
                            [formControl]="projectCodeToAdd"
                            fieldLabel="Project Code (optional)"
                            [type]="FormFieldType.Select"
                            [formInputOptions]="projectCodeOptions"
                            placeholder="Select project code..."
                            [readOnly]="isSubmitting">
                        </form-field>
                    </div>
                    <div class="g-col-2" style="margin-top: 1.5rem;">
                        <button
                            class="btn btn-sm btn-primary w-100"
                            (click)="addPair()"
                            [disabled]="!programIndexToAdd.value || isDuplicate() || isSubmitting">
                            Add
                        </button>
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
                                    <td>{{ pair.ProjectCodeName || '—' }}</td>
                                    <td class="text-end">
                                        <a href="#" class="text-danger" (click)="removePair(pair); $event.preventDefault()" [class.disabled]="isSubmitting" title="Remove">
                                            <icon icon="Delete"></icon>
                                        </a>
                                    </td>
                                </tr>
                            }
                        </tbody>
                    </table>
                } @else {
                    <div class="alert alert-info">
                        <em>No pairs assigned.</em>
                    </div>
                }
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" (click)="save()" [disabled]="isSubmitting">
                    {{ isSubmitting ? 'Saving...' : 'Save' }}
                </button>
                <button class="btn btn-secondary" (click)="cancel()" [disabled]="isSubmitting">
                    Cancel
                </button>
            </div>
        </div>
    `,
})
export class EditProgramIndexProjectCodesModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<EditProgramIndexProjectCodesModalData, FundSourceAllocationProgramIndexProjectCodeItem[] | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public isLoading$ = new BehaviorSubject<boolean>(true);
    public isSubmitting = false;

    public programIndexOptions: FormInputOption[] = [];
    public projectCodeOptions: FormInputOption[] = [];
    public selectedPairs: FundSourceAllocationProgramIndexProjectCodeItem[] = [];

    public programIndexToAdd = new FormControl<number | null>(null);
    public projectCodeToAdd = new FormControl<number | null>(null);

    private programIndexMap = new Map<number, string>();
    private projectCodeMap = new Map<number, string>();

    constructor(
        private fundSourceAllocationService: FundSourceAllocationService,
        private programIndexService: ProgramIndexService,
        private projectCodeService: ProjectCodeService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.selectedPairs = [...(data.existingPairs ?? [])];

        forkJoin({
            programIndices: this.programIndexService.listLookupProgramIndex().pipe(catchError(() => of([]))),
            projectCodes: this.projectCodeService.listLookupProjectCode().pipe(catchError(() => of([]))),
        }).subscribe(({ programIndices, projectCodes }) => {
            this.programIndexOptions = programIndices.map((pi: any) => ({
                Value: pi.ProgramIndexID,
                Label: pi.ProgramIndexCode ?? pi.DisplayName ?? `PI ${pi.ProgramIndexID}`,
                disabled: false,
            }));
            this.programIndexMap = new Map(programIndices.map((pi: any) => [pi.ProgramIndexID, pi.ProgramIndexCode ?? pi.DisplayName ?? ""] as [number, string]));

            this.projectCodeOptions = projectCodes.map((pc: any) => ({
                Value: pc.ProjectCodeID,
                Label: pc.ProjectCodeName ?? `PC ${pc.ProjectCodeID}`,
                disabled: false,
            }));
            this.projectCodeMap = new Map(projectCodes.map((pc: any) => [pc.ProjectCodeID, pc.ProjectCodeName ?? ""] as [number, string]));

            this.isLoading$.next(false);
        });
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
        if (!piID) return;
        const pcID = this.projectCodeToAdd.value ?? null;

        if (this.isDuplicate()) return;

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

    save(): void {
        this.isSubmitting = true;
        this.localAlerts = [];

        const request = new FundSourceAllocationProgramIndexProjectCodeSaveRequest({
            Pairs: this.selectedPairs.map((p) => ({
                ProgramIndexID: p.ProgramIndexID,
                ProjectCodeID: p.ProjectCodeID ?? null,
            })),
        });

        this.fundSourceAllocationService
            .saveProgramIndexProjectCodesFundSourceAllocation(this.ref.data.fundSourceAllocationID, request)
            .subscribe({
                next: (result) => {
                    this.pushGlobalSuccess("Program index / project codes saved successfully.");
                    this.ref.close(result);
                },
                error: (err) => {
                    this.isSubmitting = false;
                    const message = err?.error ?? err?.message ?? "An error occurred while saving.";
                    this.addLocalAlert(message, AlertContext.Danger, true);
                },
            });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
