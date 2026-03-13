import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";
import { DialogService } from "@ngneat/dialog";
import { BehaviorSubject, combineLatest, distinctUntilChanged, filter, map, Observable, of, shareReplay, startWith, Subject, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";
import { environment } from "src/environments/environment";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { FundSourceDetail } from "src/app/shared/generated/model/fund-source-detail";
import { FundSourceProjectGridRow } from "src/app/shared/generated/model/fund-source-project-grid-row";
import { FundSourceAgreementGridRow } from "src/app/shared/generated/model/fund-source-agreement-grid-row";
import { FundSourceBudgetLineItemGridRow } from "src/app/shared/generated/model/fund-source-budget-line-item-grid-row";
import { FundSourceFileResourceGridRow } from "src/app/shared/generated/model/fund-source-file-resource-grid-row";
import { FundSourceNoteGridRow } from "src/app/shared/generated/model/fund-source-note-grid-row";
import { FundSourceNoteInternalGridRow } from "src/app/shared/generated/model/fund-source-note-internal-grid-row";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "fund-source-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, RouterLink, WADNRGridComponent, FieldDefinitionComponent, IconComponent, LoadingDirective],
    templateUrl: "./fund-source-detail.component.html",
    styleUrls: ["./fund-source-detail.component.scss"],
})
export class FundSourceDetailComponent {
    apiUrl = environment.mainAppApiUrl;
    @Input() set fundSourceID(value: string | number) {
        this._fundSourceID$.next(Number(value));
    }

    private _fundSourceID$ = new BehaviorSubject<number | null>(null);
    private refreshData$ = new Subject<void>();

    public fundSourceID$: Observable<number>;
    public fundSource$: Observable<FundSourceDetail>;
    public projects$: Observable<FundSourceProjectGridRow[]>;
    public agreements$: Observable<FundSourceAgreementGridRow[]>;
    public budgetLineItems$: Observable<FundSourceBudgetLineItemGridRow[]>;
    public files$: Observable<FundSourceFileResourceGridRow[]>;
    public notes$: Observable<FundSourceNoteGridRow[]>;
    public internalNotes$: Observable<FundSourceNoteInternalGridRow[]>;

    public projectColumnDefs: ColDef<FundSourceProjectGridRow>[] = [];
    public agreementColumnDefs: ColDef<FundSourceAgreementGridRow>[] = [];
    public budgetLineItemColumnDefs: ColDef<FundSourceBudgetLineItemGridRow>[] = [];

    public isUserLoggedIn$: Observable<boolean>;
    public canManageFundSources$: Observable<boolean>;

    constructor(
        private fundSourceService: FundSourceService,
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

        this.fundSourceID$ = this._fundSourceID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        const refresh$ = this.refreshData$.pipe(startWith(undefined));

        this.fundSource$ = combineLatest([this.fundSourceID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceService.getFundSource(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.listProjectsFundSource(fundSourceID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.agreements$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.listAgreementsFundSource(fundSourceID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.budgetLineItems$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.listBudgetLineItemsFundSource(fundSourceID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.files$ = combineLatest([this.fundSourceID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceService.listFilesFundSource(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.notes$ = combineLatest([this.fundSourceID$, refresh$]).pipe(
            switchMap(([id]) => this.fundSourceService.listNotesFundSource(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.internalNotes$ = combineLatest([this.fundSourceID$, refresh$, this.canManageFundSources$]).pipe(
            switchMap(([id, , canManage]) => canManage
                ? this.fundSourceService.listInternalNotesFundSource(id)
                : of([] as FundSourceNoteInternalGridRow[])
            ),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectColumnDefs = this.createProjectColumnDefs();
        this.agreementColumnDefs = this.createAgreementColumnDefs();
        this.budgetLineItemColumnDefs = this.createBudgetLineItemColumnDefs();
    }

    private createProjectColumnDefs(): ColDef<FundSourceProjectGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Allocation", "FundSourceAllocationName", "FundSourceAllocationID", {
                InRouterLink: "/fund-source-allocations/",
            }),
            this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
                InRouterLink: "/projects/",
            }),
            this.utilityFunctions.createBasicColumnDef("FHT #", "FhtProjectNumber"),
            this.utilityFunctions.createBasicColumnDef("Stage", "ProjectStageName", {
                CustomDropdownFilterField: "ProjectStageName",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Match Amount", "MatchAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Pay Amount", "PayAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total Amount", "TotalAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
        ];
    }

    private createAgreementColumnDefs(): ColDef<FundSourceAgreementGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Type", "AgreementTypeAbbrev", {
                CustomDropdownFilterField: "AgreementTypeAbbrev",
            }),
            this.utilityFunctions.createBasicColumnDef("Number", "AgreementNumber"),
            this.utilityFunctions.createLinkColumnDef("Organization", "OrganizationName", "OrganizationID", {
                InRouterLink: "/organizations/",
            }),
            this.utilityFunctions.createLinkColumnDef("Title", "AgreementTitle", "AgreementID", {
                InRouterLink: "/agreements/",
            }),
            this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy"),
            this.utilityFunctions.createDateColumnDef("End Date", "EndDate", "M/d/yyyy"),
            this.utilityFunctions.createCurrencyColumnDef("Amount", "AgreementAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createBasicColumnDef("Program Index", "ProgramIndices", {
                CustomDropdownFilterField: "ProgramIndices",
            }),
            this.utilityFunctions.createBasicColumnDef("Project Code", "ProjectCodes", {
                CustomDropdownFilterField: "ProjectCodes",
            }),
        ];
    }

    private createBudgetLineItemColumnDefs(): ColDef<FundSourceBudgetLineItemGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Allocation", "FundSourceAllocationName", "FundSourceAllocationID", {
                InRouterLink: "/fund-source-allocations/",
            }),
            this.utilityFunctions.createCurrencyColumnDef("Personnel", "PersonnelAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Benefits", "BenefitsAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Travel", "TravelAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Supplies", "SuppliesAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Contractual", "ContractualAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Indirect Costs", "IndirectCostsAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
            this.utilityFunctions.createCurrencyColumnDef("Total", "TotalAmount", {
                MaxDecimalPlacesToDisplay: 2,
            }),
        ];
    }

    // Modal & action methods
    openEditModal(fundSource: FundSourceDetail): void {
        import("../fund-source-edit-modal.component").then(({ FundSourceEditModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceEditModalComponent, {
                data: {
                    mode: "edit" as const,
                    fundSourceID: fundSource.FundSourceID,
                    fundSourceName: fundSource.FundSourceName,
                    shortName: fundSource.ShortName,
                    organizationID: fundSource.Organization?.OrganizationID,
                    fundSourceStatusID: fundSource.FundSourceStatus?.FundSourceStatusID,
                    fundSourceTypeID: fundSource.FundSourceTypeID,
                    fundSourceNumber: fundSource.FundSourceNumber,
                    cfdaNumber: fundSource.CFDANumber,
                    startDate: fundSource.StartDate,
                    endDate: fundSource.EndDate,
                    totalAwardAmount: fundSource.TotalAwardAmount,
                },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    openFileModal(fundSourceID: number): void {
        import("./fund-source-file-modal.component").then(({ FundSourceFileModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceFileModalComponent, {
                data: { fundSourceID },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    openEditFileModal(fundSourceID: number, file: FundSourceFileResourceGridRow): void {
        import("./fund-source-file-edit-modal.component").then(({ FundSourceFileEditModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceFileEditModalComponent, {
                data: { fundSourceID, file },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    async deleteFile(fundSourceID: number, fundSourceFileResourceID: number): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this file?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        this.fundSourceService.deleteFileFundSource(fundSourceID, fundSourceFileResourceID).subscribe(() => this.refreshData$.next());
    }

    openNoteModal(fundSourceID: number, isInternal: boolean, mode: "create" | "edit" = "create", noteID?: number, existingNote?: string): void {
        import("./fund-source-note-modal.component").then(({ FundSourceNoteModalComponent }) => {
            const dialogRef = this.dialogService.open(FundSourceNoteModalComponent, {
                data: { mode, fundSourceID, isInternal, noteID, existingNote },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) this.refreshData$.next();
            });
        });
    }

    async deleteNote(fundSourceID: number, noteID: number, isInternal: boolean): Promise<void> {
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Delete",
            message: "Are you sure you want to delete this note?",
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });
        if (!confirmed) return;
        if (isInternal) {
            this.fundSourceService.deleteNoteInternalFundSource(fundSourceID, noteID).subscribe(() => this.refreshData$.next());
        } else {
            this.fundSourceService.deleteNoteFundSource(fundSourceID, noteID).subscribe(() => this.refreshData$.next());
        }
    }

    formatCurrency(value: number | null | undefined): string {
        if (value == null) return "\u2014";
        return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value);
    }

    formatDate(value: string | null | undefined): string {
        if (!value) return "\u2014";
        const date = new Date(value);
        return `${date.getMonth() + 1}/${date.getDate()}/${date.getFullYear()}`;
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
}
