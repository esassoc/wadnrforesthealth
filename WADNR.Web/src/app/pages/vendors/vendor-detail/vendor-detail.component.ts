import { AsyncPipe } from "@angular/common";
import { Component, Input } from "@angular/core";
import { BehaviorSubject, distinctUntilChanged, filter, Observable, shareReplay, switchMap } from "rxjs";
import { toLoadingState } from "src/app/shared/interfaces/page-loading.interface";
import { ColDef } from "ag-grid-community";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";

import { VendorService } from "src/app/shared/generated/api/vendor.service";
import { VendorDetail } from "src/app/shared/generated/model/vendor-detail";
import { VendorPersonGridRow } from "src/app/shared/generated/model/vendor-person-grid-row";
import { VendorOrganizationGridRow } from "src/app/shared/generated/model/vendor-organization-grid-row";

@Component({
    selector: "vendor-detail",
    standalone: true,
    imports: [PageHeaderComponent, AsyncPipe, BreadcrumbComponent, WADNRGridComponent, LoadingDirective],
    templateUrl: "./vendor-detail.component.html",
    styleUrls: ["./vendor-detail.component.scss"],
})
export class VendorDetailComponent {
    @Input() set vendorID(value: string | number) {
        this._vendorID$.next(Number(value));
    }

    private _vendorID$ = new BehaviorSubject<number | null>(null);

    public vendorID$: Observable<number>;
    public vendor$: Observable<VendorDetail>;
    public people$: Observable<VendorPersonGridRow[]>;
    public peopleIsLoading$: Observable<boolean>;
    public organizations$: Observable<VendorOrganizationGridRow[]>;
    public organizationsIsLoading$: Observable<boolean>;

    public personColumnDefs: ColDef<VendorPersonGridRow>[] = [];
    public organizationColumnDefs: ColDef<VendorOrganizationGridRow>[] = [];

    constructor(
        private vendorService: VendorService,
        private utilityFunctions: UtilityFunctionsService
    ) {}

    ngOnInit(): void {
        this.vendorID$ = this._vendorID$.pipe(
            filter((id): id is number => id != null && !Number.isNaN(id)),
            distinctUntilChanged(),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vendor$ = this.vendorID$.pipe(
            switchMap((vendorID) => this.vendorService.getVendor(vendorID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.people$ = this.vendorID$.pipe(
            switchMap((vendorID) => this.vendorService.listPeopleVendor(vendorID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.organizations$ = this.vendorID$.pipe(
            switchMap((vendorID) => this.vendorService.listOrganizationsVendor(vendorID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.peopleIsLoading$ = toLoadingState(this.people$);
        this.organizationsIsLoading$ = toLoadingState(this.organizations$);

        this.personColumnDefs = this.createPersonColumnDefs();
        this.organizationColumnDefs = this.createOrganizationColumnDefs();
    }

    private createPersonColumnDefs(): ColDef<VendorPersonGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Name", "FullName", "PersonID", {
                InRouterLink: "/people/",
                RequiresAuth: true,
            }),
            this.utilityFunctions.createBasicColumnDef("Email", "Email"),
            this.utilityFunctions.createBasicColumnDef("Phone", "Phone"),
        ];
    }

    private createOrganizationColumnDefs(): ColDef<VendorOrganizationGridRow>[] {
        return [
            this.utilityFunctions.createLinkColumnDef("Organization", "OrganizationName", "OrganizationID", {
                InRouterLink: "/organizations/",
            }),
            this.utilityFunctions.createBasicColumnDef("Short Name", "OrganizationShortName"),
        ];
    }

    formatAddress(vendor: VendorDetail): string {
        const parts: string[] = [];
        if (vendor.VendorAddressLine1) parts.push(vendor.VendorAddressLine1);
        if (vendor.VendorAddressLine2) parts.push(vendor.VendorAddressLine2);
        if (vendor.VendorAddressLine3) parts.push(vendor.VendorAddressLine3);
        return parts.join(", ");
    }
}
