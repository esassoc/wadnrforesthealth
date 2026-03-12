import { AsyncPipe, CurrencyPipe, DatePipe } from "@angular/common";
import { Component, DestroyRef, inject } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { DomSanitizer, SafeHtml, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { DialogService } from "@ngneat/dialog";
import { ColDef } from "ag-grid-community";
import { combineLatest, distinctUntilChanged, filter, map, Observable, shareReplay, startWith, Subject, switchMap } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AgreementService } from "src/app/shared/generated/api/agreement.service";
import { AgreementContactGridRow } from "src/app/shared/generated/model/agreement-contact-grid-row";
import { AgreementDetail } from "src/app/shared/generated/model/agreement-detail";
import { FundSourceAllocationLookupItem } from "src/app/shared/generated/model/fund-source-allocation-lookup-item";
import { ProjectLookupItem } from "src/app/shared/generated/model/project-lookup-item";
import { getFileResourceUrlFromBase } from "src/app/shared/utils/file-resource-utils";
import { environment } from "src/environments/environment";

@Component({
    selector: "agreement-detail",
    standalone: true,
    imports: [AsyncPipe, RouterLink, DatePipe, CurrencyPipe, BreadcrumbComponent, FieldDefinitionComponent, PageHeaderComponent, WADNRGridComponent, LoadingDirective, IconComponent],
    templateUrl: "./agreement-detail.component.html",
    styleUrls: ["./agreement-detail.component.scss"],
})
export class AgreementDetailComponent {
    private destroyRef = inject(DestroyRef);

    public agreementID$: Observable<number>;
    private refreshData$ = new Subject<void>();

    public agreement$: Observable<AgreementDetail>;

    public fundSourceAllocations$: Observable<FundSourceAllocationLookupItem[]>;
    public fundSourceAllocationsIsLoading$: Observable<boolean>;

    public projects$: Observable<ProjectLookupItem[]>;
    public projectsIsLoading$: Observable<boolean>;

    public contacts$: Observable<AgreementContactGridRow[]>;
    public contactsIsLoading$: Observable<boolean>;
    public contactColumnDefs$: Observable<ColDef[]>;

    public canManageAgreements$: Observable<boolean>;
    private currentAgreementID: number | null = null;

    constructor(
        private route: ActivatedRoute,
        private agreementService: AgreementService,
        private utilityFunctions: UtilityFunctionsService,
        private sanitizer: DomSanitizer,
        private dialogService: DialogService,
        private authService: AuthenticationService,
        private alertService: AlertService,
        private confirmService: ConfirmService,
    ) {}

    ngOnInit(): void {
        this.canManageAgreements$ = this.authService.currentUserSetObservable.pipe(
            map((user) => this.authService.canManageFundSources(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.contactColumnDefs$ = this.canManageAgreements$.pipe(
            map((canManage) => this.buildContactColumnDefs(canManage)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.agreementID$ = this.route.paramMap.pipe(
            map((p) => (p.get("agreementID") ? Number(p.get("agreementID")) : null)),
            filter((agreementID): agreementID is number => agreementID != null && !Number.isNaN(agreementID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // Track current agreementID for use in grid action handlers
        this.agreementID$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((id) => (this.currentAgreementID = id));

        const refresh$ = this.refreshData$.pipe(startWith(undefined));

        this.agreement$ = combineLatest([this.agreementID$, refresh$]).pipe(
            switchMap(([agreementID]) => this.agreementService.getAgreement(agreementID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSourceAllocations$ = combineLatest([this.agreementID$, refresh$]).pipe(
            switchMap(([agreementID]) => this.agreementService.listFundSourceAllocationsAgreement(agreementID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = combineLatest([this.agreementID$, refresh$]).pipe(
            switchMap(([agreementID]) => this.agreementService.listProjectsAgreement(agreementID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.contacts$ = combineLatest([this.agreementID$, refresh$]).pipe(
            switchMap(([agreementID]) => this.agreementService.listContactsAgreement(agreementID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSourceAllocationsIsLoading$ = toLoadingState(this.fundSourceAllocations$);
        this.projectsIsLoading$ = toLoadingState(this.projects$);
        this.contactsIsLoading$ = toLoadingState(this.contacts$);
    }

    private buildContactColumnDefs(canManage: boolean): ColDef[] {
        const cols: ColDef[] = [];

        if (canManage) {
            cols.push(
                this.utilityFunctions.createActionsColumnDef((params) => {
                    const row = params.data as AgreementContactGridRow;
                    return [
                        { ActionName: "Edit", ActionHandler: () => this.openContactModal(this.currentAgreementID!, "edit", row), ActionIcon: "fa fa-edit" },
                        { ActionName: "Delete", ActionHandler: () => this.deleteContact(this.currentAgreementID!, row), ActionIcon: "fa fa-trash" },
                    ];
                })
            );
        }

        cols.push(
            this.utilityFunctions.createLinkColumnDef("First Name", "Person.FirstName", "Person.PersonID", {
                InRouterLink: "/people/",
                Width: 180,
                RequiresAuth: true,
            }),
            this.utilityFunctions.createLinkColumnDef("Last Name", "Person.LastName", "Person.PersonID", {
                InRouterLink: "/people/",
                Width: 200,
                RequiresAuth: true,
            }),
            this.utilityFunctions.createBasicColumnDef("Agreement Role", "AgreementRole.AgreementPersonRoleName", {
                Width: 220,
            }),
            this.utilityFunctions.createLinkColumnDef("Contributing Organization", "ContributingOrganization.OrganizationName", "ContributingOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "Organization",
                FieldDefinitionLabelOverride: "Contributing Organization",
                Width: 260,
            }),
        );

        return cols;
    }

    public sanitizeHtml(html: string | null | undefined): SafeHtml {
        return html ? this.sanitizer.bypassSecurityTrustHtml(html) : "";
    }

    public documentUrl(fileResourceGuid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, fileResourceGuid);
    }

    openEditModal(agreement: AgreementDetail): void {
        import("./agreement-edit-modal.component").then(({ AgreementEditModalComponent }) => {
            const dialogRef = this.dialogService.open(AgreementEditModalComponent, {
                data: {
                    mode: "edit" as const,
                    agreementID: agreement.AgreementID,
                    agreementTitle: agreement.AgreementTitle,
                    agreementNumber: agreement.AgreementNumber,
                    organizationID: agreement.ContributingOrganization?.OrganizationID,
                    agreementStatusID: agreement.AgreementStatus?.AgreementStatusID,
                    agreementTypeID: agreement.AgreementType?.AgreementTypeID,
                    agreementAmount: agreement.AgreementAmount,
                    startDate: agreement.StartDate,
                    endDate: agreement.EndDate,
                    notes: agreement.Notes,
                    agreementFileResourceID: agreement.FileResource?.FileResourceID,
                },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.alertService.pushAlert(new Alert("Agreement updated.", AlertContext.Success, true));
                    this.refreshData$.next();
                }
            });
        });
    }

    openFundSourceAllocationsModal(agreementID: number, currentAllocations: FundSourceAllocationLookupItem[]): void {
        import("./agreement-fund-source-allocations-modal.component").then(({ AgreementFundSourceAllocationsModalComponent }) => {
            const dialogRef = this.dialogService.open(AgreementFundSourceAllocationsModalComponent, {
                data: { agreementID, currentAllocations },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.alertService.pushAlert(new Alert("Fund source allocations updated.", AlertContext.Success, true));
                    this.refreshData$.next();
                }
            });
        });
    }

    openProjectsModal(agreementID: number, currentProjects: ProjectLookupItem[]): void {
        import("./agreement-projects-modal.component").then(({ AgreementProjectsModalComponent }) => {
            const dialogRef = this.dialogService.open(AgreementProjectsModalComponent, {
                data: { agreementID, currentProjects },
                size: "lg",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.alertService.pushAlert(new Alert("Projects updated.", AlertContext.Success, true));
                    this.refreshData$.next();
                }
            });
        });
    }

    openContactModal(agreementID: number, mode: "create" | "edit", contact?: AgreementContactGridRow): void {
        import("./agreement-contact-modal.component").then(({ AgreementContactModalComponent }) => {
            const dialogRef = this.dialogService.open(AgreementContactModalComponent, {
                data: {
                    mode,
                    agreementID,
                    agreementPersonID: contact?.AgreementPersonID,
                    personID: contact?.Person?.PersonID,
                    agreementPersonRoleID: contact?.AgreementRole?.AgreementPersonRoleID,
                },
                size: "md",
            });
            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.alertService.pushAlert(new Alert(mode === "create" ? "Contact added." : "Contact updated.", AlertContext.Success, true));
                    this.refreshData$.next();
                }
            });
        });
    }

    async deleteContact(agreementID: number, contact: AgreementContactGridRow): Promise<void> {
        const name = `${contact.Person?.FirstName} ${contact.Person?.LastName}`;
        const confirmed = await this.confirmService.confirm({
            title: "Confirm Remove",
            message: `Are you sure you want to remove ${name} as a contact from this agreement?`,
            buttonTextYes: "Remove",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        });

        if (confirmed) {
            this.agreementService.deleteContactAgreement(agreementID, contact.AgreementPersonID).subscribe({
                next: () => {
                    this.alertService.pushAlert(new Alert("Contact removed.", AlertContext.Success, true));
                    this.refreshData$.next();
                },
            });
        }
    }

}
