import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { RouterLink } from "@angular/router";
import { BehaviorSubject, distinctUntilChanged, filter, Observable, shareReplay, switchMap } from "rxjs";
import { ColDef } from "ag-grid-community";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { FundSourceService } from "src/app/shared/generated/api/fund-source.service";
import { FundSourceDetail } from "src/app/shared/generated/model/fund-source-detail";
import { FundSourceProjectGridRow } from "src/app/shared/generated/model/fund-source-project-grid-row";
import { FundSourceAgreementGridRow } from "src/app/shared/generated/model/fund-source-agreement-grid-row";
import { FundSourceBudgetLineItemGridRow } from "src/app/shared/generated/model/fund-source-budget-line-item-grid-row";
import { FundSourceFileResourceGridRow } from "src/app/shared/generated/model/fund-source-file-resource-grid-row";
import { FundSourceNoteGridRow } from "src/app/shared/generated/model/fund-source-note-grid-row";
import { FundSourceNoteInternalGridRow } from "src/app/shared/generated/model/fund-source-note-internal-grid-row";

@Component({
    selector: "fund-source-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, RouterLink, WADNRGridComponent],
    templateUrl: "./fund-source-detail.component.html",
    styleUrls: ["./fund-source-detail.component.scss"],
})
export class FundSourceDetailComponent {
    @Input() set fundSourceID(value: string | number) {
        this._fundSourceID$.next(Number(value));
    }

    private _fundSourceID$ = new BehaviorSubject<number | null>(null);

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
    public fileColumnDefs: ColDef<FundSourceFileResourceGridRow>[] = [];
    public noteColumnDefs: ColDef<FundSourceNoteGridRow>[] = [];
    public internalNoteColumnDefs: ColDef<FundSourceNoteInternalGridRow>[] = [];

    // TODO: Replace with actual authentication check when auth is implemented
    public isUserLoggedIn: boolean = true;
    public canViewInternalNotes: boolean = true;

    constructor(
        private fundSourceService: FundSourceService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.fundSourceID$ = this._fundSourceID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSource$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.getFundSource(fundSourceID)),
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

        this.files$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.listFilesFundSource(fundSourceID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.notes$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.listNotesFundSource(fundSourceID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.internalNotes$ = this.fundSourceID$.pipe(
            switchMap((fundSourceID) => this.fundSourceService.listInternalNotesFundSource(fundSourceID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectColumnDefs = this.createProjectColumnDefs();
        this.agreementColumnDefs = this.createAgreementColumnDefs();
        this.budgetLineItemColumnDefs = this.createBudgetLineItemColumnDefs();
        this.fileColumnDefs = this.createFileColumnDefs();
        this.noteColumnDefs = this.createNoteColumnDefs();
        this.internalNoteColumnDefs = this.createInternalNoteColumnDefs();
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
            this.utilityFunctions.createBasicColumnDef("Program Index", "ProgramIndices"),
            this.utilityFunctions.createBasicColumnDef("Project Code", "ProjectCodes"),
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

    private createFileColumnDefs(): ColDef<FundSourceFileResourceGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("File Name", "DisplayName"),
            this.utilityFunctions.createBasicColumnDef("Description", "Description"),
            this.utilityFunctions.createBasicColumnDef("Type", "FileResourceMimeTypeName"),
            this.utilityFunctions.createDateColumnDef("Uploaded", "CreateDate", "M/d/yyyy"),
        ];
    }

    private createNoteColumnDefs(): ColDef<FundSourceNoteGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Note", "Note"),
            this.utilityFunctions.createBasicColumnDef("Created By", "CreatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Created", "CreateDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Updated By", "UpdatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Updated", "UpdateDate", "M/d/yyyy"),
        ];
    }

    private createInternalNoteColumnDefs(): ColDef<FundSourceNoteInternalGridRow>[] {
        return [
            this.utilityFunctions.createBasicColumnDef("Note", "Note"),
            this.utilityFunctions.createBasicColumnDef("Created By", "CreatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Created", "CreateDate", "M/d/yyyy"),
            this.utilityFunctions.createBasicColumnDef("Updated By", "UpdatedByPersonName"),
            this.utilityFunctions.createDateColumnDef("Updated", "UpdateDate", "M/d/yyyy"),
        ];
    }

    formatCurrency(value: number | null | undefined): string {
        if (value == null) return "—";
        return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", minimumFractionDigits: 0, maximumFractionDigits: 0 }).format(value);
    }

    formatDate(value: string | null | undefined): string {
        if (!value) return "—";
        const date = new Date(value);
        return date.toLocaleDateString("en-US", { year: "numeric", month: "short", day: "numeric" });
    }
}
