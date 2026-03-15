import { AsyncPipe } from "@angular/common";
import { Component, DestroyRef, inject, Input, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { RouterLink } from "@angular/router";
import { DialogService } from "@ngneat/dialog";
import { ColDef } from "ag-grid-community";
import { BehaviorSubject, combineLatest, debounceTime, distinctUntilChanged, filter, map, Observable, of, shareReplay, startWith, Subject, switchMap, take } from "rxjs";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { environment } from "src/environments/environment";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { PersonLinkComponent } from "src/app/shared/components/person-link/person-link.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { PieChartComponent } from "src/app/shared/components/charts/pie-chart/pie-chart.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertComponent } from "src/app/shared/components/alert/alert.component";

import { FundSourceAllocationService } from "src/app/shared/generated/api/fund-source-allocation.service";
import { FundSourceAllocationNoteService } from "src/app/shared/generated/api/fund-source-allocation-note.service";
import { FundSourceAllocationNoteInternalService } from "src/app/shared/generated/api/fund-source-allocation-note-internal.service";
import { FundSourceAllocationDetail } from "src/app/shared/generated/model/fund-source-allocation-detail";
import { FundSourceAllocationBudgetLineItemGridRow } from "src/app/shared/generated/model/fund-source-allocation-budget-line-item-grid-row";
import { FundSourceAllocationProjectGridRow } from "src/app/shared/generated/model/fund-source-allocation-project-grid-row";
import { FundSourceAllocationAgreementGridRow } from "src/app/shared/generated/model/fund-source-allocation-agreement-grid-row";
import { FundSourceAllocationChangeLogGridRow } from "src/app/shared/generated/model/fund-source-allocation-change-log-grid-row";
import { FundSourceAllocationNoteGridRow } from "src/app/shared/generated/model/fund-source-allocation-note-grid-row";
import { FundSourceAllocationNoteInternalGridRow } from "src/app/shared/generated/model/fund-source-allocation-note-internal-grid-row";
import { FundSourceAllocationFileGridRow } from "src/app/shared/generated/model/fund-source-allocation-file-grid-row";
import { FundSourceAllocationExpenditureGridRow } from "src/app/shared/generated/model/fund-source-allocation-expenditure-grid-row";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { LocalDatePipe } from "src/app/shared/pipes/local-date.pipe";
import { FundSourceAllocationExpenditureSummary } from "src/app/shared/generated/model/fund-source-allocation-expenditure-summary";

export interface BudgetVsActualsRow {
    CostTypeName: string;
    Budget: number;
    Expenditures: number;
    Difference: number;
}

@Component({
    selector: "fund-source-allocation-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, RouterLink, PersonLinkComponent, WADNRGridComponent, PieChartComponent, FieldDefinitionComponent, IconComponent, LoadingDirective, LocalDatePipe, ReactiveFormsModule, FormFieldComponent, AlertComponent],
    templateUrl: "./fund-source-allocation-detail.component.html",
    styleUrls: ["./fund-source-allocation-detail.component.scss"],
})
export class FundSourceAllocationDetailComponent {
    apiUrl = environment.mainAppApiUrl;
    private destroyRef = inject(DestroyRef);

    @Input() set fundSourceAllocationID(value: string | number) {
        this._fundSourceAllocationID$.next(Number(value));
    }

    private _fundSourceAllocationID$ = new BehaviorSubject<number | null>(null);
    private refreshData$ = new Subject<void>();
    private budgetSave$ = new Subject<void>();

    public fundSourceAllocationID$: Observable<number>;
    public fundSourceAllocation$: Observable<FundSourceAllocationDetail>;
    public budgetLineItems$: Observable<FundSourceAllocationBudgetLineItemGridRow[]>;
    public projects$: Observable<FundSourceAllocationProjectGridRow[]>;
    public agreements$: Observable<FundSourceAllocationAgreementGridRow[]>;
    public changeLogs$: Observable<FundSourceAllocationChangeLogGridRow[]>;
    public notes$: Observable<FundSourceAllocationNoteGridRow[]>;
    public internalNotes$: Observable<FundSourceAllocationNoteInternalGridRow[]>;
    public files$: Observable<FundSourceAllocationFileGridRow[]>;
    public expenditures$: Observable<FundSourceAllocationExpenditureGridRow[]>;
    public expenditureSummary$: Observable<FundSourceAllocationExpenditureSummary[]>;
    public allocationCurrentBalance$: Observable<number | null>;
    public budgetVsActuals$: Observable<BudgetVsActualsRow[]>;

    public budgetLineItemsIsLoading$: Observable<boolean>;
    public projectsIsLoading$: Observable<boolean>;
    public agreementsIsLoading$: Observable<boolean>;
    public expendituresIsLoading$: Observable<boolean>;
    public filesIsLoading$: Observable<boolean>;
    public expenditureSummaryIsLoading$: Observable<boolean>;
    public budgetVsActualsIsLoading$: Observable<boolean>;

    public isUserLoggedIn$: Observable<boolean>;
    public canManageFundSources$: Observable<boolean>;

    public currentAllocationID: number;
    public budgetLineItemRows: { costTypeID: number; costTypeName: string; amount: FormControl<number | null>; note: FormControl<string | null> }[] = [];
    public budgetLineItemSaveAlert = signal<Alert | null>(null);
    public canManageFundSourcesSync = false;
    public FormFieldType = FormFieldType;
    public projectColumnDefs: ColDef<FundSourceAllocationProjectGridRow>[];
    public expenditureColumnDefs: ColDef<FundSourceAllocationExpenditureGridRow>[];
    public budgetVsActualsColumnDefs: ColDef<BudgetVsActualsRow>[];

    constructor(
        private fundSourceAllocationService: FundSourceAllocationService,
        private noteService: FundSourceAllocationNoteService,
        private noteInternalService: FundSourceAllocationNoteInternalService,
        private dialogService: DialogService,
        private utilityFunctions: UtilityFunctionsService,
        private authService: AuthenticationService,
        private confirmService: ConfirmService,
    ) {}

    ngOnInit(): void {
        this.isUserLoggedIn$ = this.authService.currentUserSetObservable.pipe(
            map((user) => user != null),
        );
        this.canManageFundSources$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageFundSources(user)),
        );

        this.fundSourceAllocationID$ = this._fundSourceAllocationID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSourceAllocationID$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((id) => (this.currentAllocationID = id));

        const refresh$ = this.refreshData$.pipe(startWith(undefined));

        this.fundSourceAllocation$ = combineLatest([this.fundSourceAllocationID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceAllocationService.getByIDFundSourceAllocation(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.budgetLineItems$ = combineLatest([this.fundSourceAllocationID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceAllocationService.listBudgetLineItemsFundSourceAllocation(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.fundSourceAllocationID$.pipe(
            switchMap((id) => this.fundSourceAllocationService.listProjectsFundSourceAllocation(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.agreements$ = this.fundSourceAllocationID$.pipe(
            switchMap((id) => this.fundSourceAllocationService.listAgreementsFundSourceAllocation(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.changeLogs$ = combineLatest([this.fundSourceAllocationID$, refresh$, this.canManageFundSources$]).pipe(
            switchMap(([id, , canManage]) => canManage
                ? this.fundSourceAllocationService.listChangeLogsFundSourceAllocation(id)
                : of([] as FundSourceAllocationChangeLogGridRow[])
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.notes$ = combineLatest([this.fundSourceAllocationID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceAllocationService.listNotesFundSourceAllocation(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.internalNotes$ = combineLatest([this.fundSourceAllocationID$, refresh$, this.canManageFundSources$]).pipe(
            switchMap(([id, , canManage]) => canManage
                ? this.fundSourceAllocationService.listNotesInternalFundSourceAllocation(id)
                : of([] as FundSourceAllocationNoteInternalGridRow[])
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.files$ = combineLatest([this.fundSourceAllocationID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceAllocationService.listFilesFundSourceAllocation(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.expenditures$ = this.fundSourceAllocationID$.pipe(
            switchMap((id) => this.fundSourceAllocationService.listExpendituresFundSourceAllocation(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.expenditureSummary$ = this.fundSourceAllocationID$.pipe(
            switchMap((id) => this.fundSourceAllocationService.listExpenditureSummaryFundSourceAllocation(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.allocationCurrentBalance$ = combineLatest([this.budgetLineItems$, this.expenditureSummary$]).pipe(
            map(([budgetItems, expenditureSummary]) => {
                const totalBudget = budgetItems.reduce((sum, item) => sum + (item.FundSourceAllocationBudgetLineItemAmount ?? 0), 0);
                const totalExpenditure = expenditureSummary.reduce((sum, item) => sum + (item.TotalAmount ?? 0), 0);
                return totalBudget - totalExpenditure;
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.budgetVsActuals$ = combineLatest([this.budgetLineItems$, this.expenditureSummary$]).pipe(
            map(([budgetItems, expenditureSummary]) => {
                const costTypes = new Set<string>();
                budgetItems.forEach((item) => costTypes.add(item.CostTypeName ?? "Unknown"));
                expenditureSummary.forEach((item) => costTypes.add(item.CostTypeName ?? "Unknown"));

                return Array.from(costTypes).map((costType) => {
                    const budget = budgetItems
                        .filter((item) => (item.CostTypeName ?? "Unknown") === costType)
                        .reduce((sum, item) => sum + (item.FundSourceAllocationBudgetLineItemAmount ?? 0), 0);
                    const expenditures = expenditureSummary
                        .filter((item) => (item.CostTypeName ?? "Unknown") === costType)
                        .reduce((sum, item) => sum + (item.TotalAmount ?? 0), 0);
                    return { CostTypeName: costType, Budget: budget, Expenditures: expenditures, Difference: budget - expenditures };
                });
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.budgetLineItemsIsLoading$ = toLoadingState(this.budgetLineItems$);
        this.projectsIsLoading$ = toLoadingState(this.projects$);
        this.agreementsIsLoading$ = toLoadingState(this.agreements$);
        this.expendituresIsLoading$ = toLoadingState(this.expenditures$);
        this.filesIsLoading$ = toLoadingState(this.files$);
        this.expenditureSummaryIsLoading$ = toLoadingState(this.expenditureSummary$);
        this.budgetVsActualsIsLoading$ = toLoadingState(this.budgetVsActuals$);

        this.canManageFundSources$.pipe(take(1)).subscribe((canManage) => {
            this.canManageFundSourcesSync = canManage;
        });

        this.budgetLineItems$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((items) => {
            this.budgetLineItemRows = items.map((item) => ({
                costTypeID: item.CostTypeID,
                costTypeName: item.CostTypeName ?? "Unknown",
                amount: new FormControl<number | null>(item.FundSourceAllocationBudgetLineItemAmount ?? null),
                note: new FormControl<string | null>(item.FundSourceAllocationBudgetLineItemNote ?? null),
            }));
        });

        this.budgetSave$.pipe(
            debounceTime(300),
            switchMap(() => {
                const allItems = this.budgetLineItemRows.map((row) => ({
                    CostTypeID: row.costTypeID,
                    Amount: row.amount.value,
                    Note: row.note.value,
                }));
                return this.fundSourceAllocationService
                    .saveBudgetLineItemsFundSourceAllocation(this.currentAllocationID, { Items: allItems });
            }),
            takeUntilDestroyed(this.destroyRef),
        ).subscribe({
            next: () => {
                this.budgetLineItemSaveAlert.set(new Alert("Saved successfully.", AlertContext.Success, true));
                setTimeout(() => this.budgetLineItemSaveAlert.set(null), 5000);
                this.refreshData$.next();
            },
            error: (err) => {
                const message = typeof err?.error === "string" ? err.error : (err?.message ?? "An error occurred while saving.");
                this.budgetLineItemSaveAlert.set(new Alert(message, AlertContext.Danger, true));
            },
        });

        this.projectColumnDefs = [
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", { InRouterLink: "/projects/", FieldDefinitionType: "ProjectName", FieldDefinitionLabelOverride: "Project" }),
            this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStageName", { CustomDropdownFilterField: "ProjectStageName", FieldDefinitionType: "ProjectStage" }),
            this.utilityFunctions.createCurrencyColumnDef("Match Amount", "MatchAmount", { MaxDecimalPlacesToDisplay: 2, FieldDefinitionType: "MatchAmount" }),
            this.utilityFunctions.createCurrencyColumnDef("DNR Pay Amount", "PayAmount", { MaxDecimalPlacesToDisplay: 2, FieldDefinitionType: "ProjectFundSourceAllocationRequestPayAmount", FieldDefinitionLabelOverride: "DNR Pay Amount" }),
            this.utilityFunctions.createCurrencyColumnDef("Total Amount", "TotalAmount", { MaxDecimalPlacesToDisplay: 2, FieldDefinitionType: "ProjectFundSourceAllocationRequestTotalAmount" }),
        ];

        this.expenditureColumnDefs = [
            this.utilityFunctions.createBasicColumnDef("Cost Type", "CostTypeName", { CustomDropdownFilterField: "CostTypeName" }),
            this.utilityFunctions.createBasicColumnDef("Biennium", "Biennium"),
            this.utilityFunctions.createBasicColumnDef("Fiscal Month", "FiscalMonth"),
            this.utilityFunctions.createBasicColumnDef("Calendar Year", "CalendarYear"),
            this.utilityFunctions.createBasicColumnDef("Calendar Month", "CalendarMonth"),
            this.utilityFunctions.createCurrencyColumnDef("Expenditure", "ExpenditureAmount", { MaxDecimalPlacesToDisplay: 2 }),
        ];

        this.budgetVsActualsColumnDefs = [
            this.utilityFunctions.createBasicColumnDef("Cost Type", "CostTypeName"),
            this.utilityFunctions.createCurrencyColumnDef("Budget", "Budget", { MaxDecimalPlacesToDisplay: 2 }),
            this.utilityFunctions.createCurrencyColumnDef("Expenditures From Datamart", "Expenditures", { MaxDecimalPlacesToDisplay: 2 }),
            this.utilityFunctions.createCurrencyColumnDef("Budget Minus Expenditures", "Difference", { MaxDecimalPlacesToDisplay: 2 }),
        ];
    }

    openEditModal(allocation: FundSourceAllocationDetail): void {
        import("./fund-source-allocation-edit-modal.component").then(({ FundSourceAllocationEditModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceAllocationEditModalComponent, {
                data: { allocation, mode: "edit" as const },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    openNoteModal(fundSourceAllocationID: number, isInternal: boolean, mode: "create" | "edit" = "create", noteID?: number, existingNote?: string): void {
        import("./fund-source-allocation-note-modal.component").then(({ FundSourceAllocationNoteModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceAllocationNoteModalComponent, {
                data: { mode, fundSourceAllocationID, isInternal, noteID, existingNote },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    async deleteNote(noteID: number, isInternal: boolean): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this note?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        if (isInternal) {
            this.noteInternalService.deleteFundSourceAllocationNoteInternal(noteID).subscribe(() => this.refreshData$.next());
        } else {
            this.noteService.deleteFundSourceAllocationNote(noteID).subscribe(() => this.refreshData$.next());
        }
    }

    openFileModal(fundSourceAllocationID: number): void {
        import("./fund-source-allocation-file-modal.component").then(({ FundSourceAllocationFileModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceAllocationFileModalComponent, {
                data: { fundSourceAllocationID },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    openEditFileModal(fundSourceAllocationID: number, file: FundSourceAllocationFileGridRow): void {
        import("./fund-source-allocation-file-edit-modal.component").then(({ FundSourceAllocationFileEditModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceAllocationFileEditModalComponent, {
                data: { fundSourceAllocationID, file },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    async deleteFile(fundSourceAllocationID: number, fundSourceAllocationFileResourceID: number): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this file?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        this.fundSourceAllocationService.deleteFileFundSourceAllocation(fundSourceAllocationID, fundSourceAllocationFileResourceID).subscribe(() => this.refreshData$.next());
    }

    onBudgetLineItemBlur(): void {
        if (!this.canManageFundSourcesSync) return;
        this.budgetSave$.next();
    }

    getChartData(summary: FundSourceAllocationExpenditureSummary[]): Array<{ label: string; value: number }> {
        return summary.map((s) => ({ label: s.CostTypeName ?? "Unknown", value: s.TotalAmount ?? 0 }));
    }

    formatCurrency(value: number | null | undefined): string {
        if (value == null) return "\u2014";
        return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value);
    }

    formatDateTime(value: string | null | undefined): string {
        if (!value) return "\u2014";
        const date = new Date(value);
        const month = date.getMonth() + 1;
        const day = date.getDate();
        const year = date.getFullYear();
        let hours = date.getHours();
        const minutes = date.getMinutes().toString().padStart(2, "0");
        const ampm = hours >= 12 ? "PM" : "AM";
        hours = hours % 12 || 12;
        return `${month}/${day}/${year} ${hours}:${minutes} ${ampm}`;
    }

    formatBoolean(value: boolean | null | undefined): string {
        if (value == null) return "\u2014";
        return value ? "Yes" : "No";
    }

    formatPipc(allocation: FundSourceAllocationDetail): string {
        if (!allocation.ProgramIndexProjectCodes?.length) return "\u2014";
        return allocation.ProgramIndexProjectCodes.map((p) => `${p.ProgramIndexCode ?? ""}-(${p.ProjectCodeName ?? "not found"})`).join(", ");
    }
}
