import { AsyncPipe, CurrencyPipe, DatePipe } from "@angular/common";
import { Component } from "@angular/core";
import { DomSanitizer, SafeHtml, SafeResourceUrl } from "@angular/platform-browser";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { ColDef } from "ag-grid-community";
import { distinctUntilChanged, filter, map, Observable, shareReplay, startWith, switchMap } from "rxjs";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { FieldDefinitionComponent } from "src/app/shared/components/field-definition/field-definition.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
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
    imports: [AsyncPipe, RouterLink, DatePipe, CurrencyPipe, BreadcrumbComponent, FieldDefinitionComponent, PageHeaderComponent, WADNRGridComponent, LoadingDirective],
    templateUrl: "./agreement-detail.component.html",
    styleUrls: ["./agreement-detail.component.scss"],
})
export class AgreementDetailComponent {
    public agreementID$: Observable<number>;

    public agreement$: Observable<AgreementDetail>;
    public agreementIsLoading$: Observable<boolean>;

    public fundSourceAllocations$: Observable<FundSourceAllocationLookupItem[]>;
    public fundSourceAllocationsIsLoading$: Observable<boolean>;

    public projects$: Observable<ProjectLookupItem[]>;
    public projectsIsLoading$: Observable<boolean>;

    public contacts$: Observable<AgreementContactGridRow[]>;
    public contactsIsLoading$: Observable<boolean>;
    public contactColumnDefs: ColDef[];

    constructor(private route: ActivatedRoute, private agreementService: AgreementService, private utilityFunctions: UtilityFunctionsService, private sanitizer: DomSanitizer) {}

    ngOnInit(): void {
        this.agreementID$ = this.route.paramMap.pipe(
            map((p) => (p.get("agreementID") ? Number(p.get("agreementID")) : null)),
            filter((agreementID): agreementID is number => agreementID != null && !Number.isNaN(agreementID)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.agreement$ = this.agreementID$.pipe(
            switchMap((agreementID) => this.agreementService.getAgreement(agreementID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSourceAllocations$ = this.agreementID$.pipe(
            switchMap((agreementID) => this.agreementService.listFundSourceAllocationsAgreement(agreementID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projects$ = this.agreementID$.pipe(
            switchMap((agreementID) => this.agreementService.listProjectsAgreement(agreementID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.contacts$ = this.agreementID$.pipe(
            switchMap((agreementID) => this.agreementService.listContactsAgreement(agreementID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.agreementIsLoading$ = this.agreement$.pipe(
            map(() => false),
            startWith(true),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.fundSourceAllocationsIsLoading$ = this.fundSourceAllocations$.pipe(
            map(() => false),
            startWith(true),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.projectsIsLoading$ = this.projects$.pipe(
            map(() => false),
            startWith(true),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.contactsIsLoading$ = this.contacts$.pipe(
            map(() => false),
            startWith(true),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.contactColumnDefs = [
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
            this.utilityFunctions.createBasicColumnDef("Agreement Role", "AgreementRole.AgreementPersonRoleDisplayName", {
                Width: 220,
            }),
            this.utilityFunctions.createLinkColumnDef("Contributing Organization", "ContributingOrganization.OrganizationName", "ContributingOrganization.OrganizationID", {
                InRouterLink: "/organizations/",
                FieldDefinitionType: "Organization",
                FieldDefinitionLabelOverride: "Contributing Organization",
                Width: 260,
            }),
        ];
    }

    public sanitizeHtml(html: string | null | undefined): SafeHtml {
        return html ? this.sanitizer.bypassSecurityTrustHtml(html) : "";
    }

    public documentUrl(fileResourceGuid?: string | null): SafeResourceUrl | null {
        return getFileResourceUrlFromBase(environment.mainAppApiUrl, this.sanitizer, fileResourceGuid);
    }
}
