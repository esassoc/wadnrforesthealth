import { Component, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, CurrencyPipe } from "@angular/common";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { combineLatest, map, Observable, of, shareReplay, startWith, switchMap } from "rxjs";
import { catchError } from "rxjs/operators";

import { UpdateWorkflowStepBase } from "src/app/shared/components/workflow/update-workflow-step-base";
import { WorkflowStepActionsComponent } from "src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component";
import { ProjectService } from "src/app/shared/generated/api/project.service";
import { LookupService } from "src/app/shared/generated/api/lookup.service";
import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { ProjectUpdateExpectedFundingStep } from "src/app/shared/generated/model/project-update-expected-funding-step";
import { ProjectUpdateExpectedFundingStepRequest } from "src/app/shared/generated/model/project-update-expected-funding-step-request";
import { FundSourceAllocationRequestUpdateItem } from "src/app/shared/generated/model/fund-source-allocation-request-update-item";
import { FundSourceAllocationRequestUpdateItemRequest } from "src/app/shared/generated/model/fund-source-allocation-request-update-item-request";
import { FundingSourceOption } from "src/app/shared/generated/model/funding-source-option";
import { FundSourceAllocationLookupItem } from "src/app/shared/generated/model/fund-source-allocation-lookup-item";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { FormFieldComponent, FormFieldType, FormInputOption } from "src/app/shared/components/forms/form-field/form-field.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";

interface FundingSourceCheckbox {
    id: number;
    name: string;
    checked: boolean;
}

interface AllocationRequest {
    projectFundSourceAllocationRequestUpdateID?: number;
    fundSourceAllocationID: number;
    fundSourceAllocationName: string;
    fundSourceName: string;
    amountControl: FormControl<number | null>;
}

interface ExpectedFundingViewModel {
    isLoading: boolean;
    data: ProjectUpdateExpectedFundingStep | null;
    fundingSourceCheckboxes: FundingSourceCheckbox[];
    allAllocationOptions: FormInputOption[];
    fundSourceAllocations: FundSourceAllocationLookupItem[];
}

@Component({
    selector: "update-expected-funding-step",
    standalone: true,
    imports: [
        CommonModule,
        AsyncPipe,
        CurrencyPipe,
        ReactiveFormsModule,
        FormFieldComponent,
        FieldDefinitionComponent,
        IconComponent,
        WorkflowStepActionsComponent
    ],
    templateUrl: "./update-expected-funding-step.component.html",
    styleUrls: ["./update-expected-funding-step.component.scss"]
})
export class UpdateExpectedFundingStepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "photos";
    readonly stepKey = "ExpectedFunding";

    public vm$: Observable<ExpectedFundingViewModel>;
    public FormFieldType = FormFieldType;

    public estimatedTotalCostControl = new FormControl<number | null>(null);
    public fundingSourceNotesControl = new FormControl<string>("");
    public allocationToAddControl = new FormControl<number | null>(null);

    private currentVm: ExpectedFundingViewModel | null = null;
    public allocationRequests: AllocationRequest[] = [];
    public availableAllocationOptions: FormInputOption[] = [];

    constructor(
        private projectService: ProjectService,
        private lookupService: LookupService,
        private fundSourceAllocationService: FundSourceAllocationService
    ) {
        super();
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();

        const fundingSources$ = this.lookupService.listFundingSourcesLookup().pipe(
            catchError((err) => {
                console.error("Failed to load funding sources:", err);
                return of([] as FundingSourceOption[]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const fundSourceAllocations$ = this.fundSourceAllocationService.listLookupFundSourceAllocation().pipe(
            catchError((err) => {
                console.error("Failed to load fund source allocations:", err);
                return of([] as FundSourceAllocationLookupItem[]);
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const stepData$ = this.stepRefresh$.pipe(
            switchMap((id) => {
                if (id == null || Number.isNaN(id)) {
                    return of(null);
                }
                return this.projectService.getUpdateExpectedFundingStepProject(id).pipe(
                    catchError((err) => {
                        console.error("Failed to load expected funding data:", err);
                        this.alertService.pushAlert(new Alert("Failed to load expected funding data.", AlertContext.Danger, true));
                        return of(null);
                    })
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = combineLatest([stepData$, fundingSources$, fundSourceAllocations$]).pipe(
            map(([data, fundingSources, fundSourceAllocations]) => {
                const fundingSourceCheckboxes: FundingSourceCheckbox[] = [];
                const selectedIDs = new Set(data?.SelectedFundingSourceIDs ?? []);

                for (const fs of fundingSources) {
                    fundingSourceCheckboxes.push({
                        id: fs.FundingSourceID!,
                        name: fs.FundingSourceName!,
                        checked: selectedIDs.has(fs.FundingSourceID!)
                    });
                }

                const allAllocationOptions: FormInputOption[] = fundSourceAllocations.map(fsa => ({
                    Value: fsa.FundSourceAllocationID,
                    Label: fsa.FundSourceAllocationName,
                    disabled: false
                }));

                if (data) {
                    this.estimatedTotalCostControl.setValue(data.EstimatedTotalCost ?? null);
                    this.fundingSourceNotesControl.setValue(data.ProjectFundingSourceNotes ?? "");
                    this.allocationRequests = (data.AllocationRequests ?? []).map(ar => ({
                        projectFundSourceAllocationRequestUpdateID: ar.ProjectFundSourceAllocationRequestUpdateID ?? undefined,
                        fundSourceAllocationID: ar.FundSourceAllocationID!,
                        fundSourceAllocationName: ar.FundSourceAllocationName!,
                        fundSourceName: ar.FundSourceName!,
                        amountControl: new FormControl<number | null>(ar.TotalAmount ?? null)
                    }));
                    this.updateAvailableAllocationOptions(allAllocationOptions);
                }

                const vm: ExpectedFundingViewModel = {
                    isLoading: false,
                    data,
                    fundingSourceCheckboxes,
                    allAllocationOptions,
                    fundSourceAllocations
                };
                this.currentVm = vm;
                return vm;
            }),
            startWith({ isLoading: true, data: null, fundingSourceCheckboxes: [], allAllocationOptions: [], fundSourceAllocations: [] } as ExpectedFundingViewModel),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    toggleFundingSource(checkbox: FundingSourceCheckbox): void {
        checkbox.checked = !checkbox.checked;
    }

    private updateAvailableAllocationOptions(allOptions?: FormInputOption[]): void {
        const options = allOptions ?? this.currentVm?.allAllocationOptions ?? [];
        const addedIDs = new Set(this.allocationRequests.map(ar => ar.fundSourceAllocationID));
        this.availableAllocationOptions = options.filter(opt => !addedIDs.has(opt.Value as number));
    }

    onAllocationSelect(event: any): void {
        const allocationID = event?.Value ?? event;
        if (allocationID == null || !this.currentVm) return;

        if (this.allocationRequests.some(ar => ar.fundSourceAllocationID === allocationID)) {
            this.allocationToAddControl.reset();
            return;
        }

        const option = this.currentVm.fundSourceAllocations?.find(
            fsa => fsa.FundSourceAllocationID === allocationID
        );
        if (!option) return;

        this.allocationRequests.push({
            projectFundSourceAllocationRequestUpdateID: undefined,
            fundSourceAllocationID: allocationID,
            fundSourceAllocationName: option.FundSourceAllocationName!,
            fundSourceName: option.FundSourceName!,
            amountControl: new FormControl<number | null>(null)
        });

        this.allocationToAddControl.reset();
        this.updateAvailableAllocationOptions();
    }

    removeAllocation(allocationID: number): void {
        this.allocationRequests = this.allocationRequests.filter(ar => ar.fundSourceAllocationID !== allocationID);
        this.updateAvailableAllocationOptions();
    }

    getTotalAmount(): number {
        return this.allocationRequests.reduce((sum, ar) => sum + (Number(ar.amountControl.value) || 0), 0);
    }

    onSave(navigate: boolean): void {
        if (!this.currentVm) return;

        const fundingSourceIDs = this.currentVm.fundingSourceCheckboxes
            .filter(cb => cb.checked)
            .map(cb => cb.id);

        const allocationRequestItems: FundSourceAllocationRequestUpdateItemRequest[] = this.allocationRequests.map(ar => ({
            ProjectFundSourceAllocationRequestUpdateID: ar.projectFundSourceAllocationRequestUpdateID,
            FundSourceAllocationID: ar.fundSourceAllocationID,
            TotalAmount: ar.amountControl.value
        }));

        const request: ProjectUpdateExpectedFundingStepRequest = {
            EstimatedTotalCost: this.estimatedTotalCostControl.value,
            ProjectFundingSourceNotes: this.fundingSourceNotesControl.value?.trim() || null,
            FundingSourceIDs: fundingSourceIDs,
            AllocationRequests: allocationRequestItems
        };

        this.saveStep(
            (projectID) => this.projectService.saveUpdateExpectedFundingStepProject(projectID, request),
            "Expected funding saved successfully.",
            "Failed to save expected funding.",
            navigate
        );
    }
}
