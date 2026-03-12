import { Component, OnInit } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { ColDef, GridOptions } from "ag-grid-community";
import { BehaviorSubject, Observable, of, switchMap, map, startWith, shareReplay, first } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WADNRGridComponent } from "src/app/shared/components/wadnr-grid/wadnr-grid.component";
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

import { CustomRichTextService } from "src/app/shared/generated/api/custom-rich-text.service";
import { FirmaPageGridRow } from "src/app/shared/generated/model/firma-page-grid-row";
import { PageContentModalComponent, PageContentModalData } from "./page-content-modal/page-content-modal.component";

interface PreviewState {
    loading: boolean;
    html: SafeHtml | null;
}

@Component({
    selector: "manage-page-content",
    standalone: true,
    imports: [PageHeaderComponent, WADNRGridComponent, AsyncPipe, LoadingDirective],
    templateUrl: "./manage-page-content.component.html",
    styleUrl: "./manage-page-content.component.scss",
})
export class ManagePageContentComponent implements OnInit {
    public firmaPages$: Observable<FirmaPageGridRow[]>;
    public columnDefs: ColDef<FirmaPageGridRow>[] = [];
    public gridOptions: GridOptions = {};

    public selectedPage: FirmaPageGridRow | null = null;
    public previewContent$: Observable<PreviewState>;

    private refreshPages$ = new BehaviorSubject<void>(undefined);
    private selectedPageTypeID$ = new BehaviorSubject<number | null>(null);

    private canManagePageContent = false;

    constructor(
        private customRichTextService: CustomRichTextService,
        private utilityFunctions: UtilityFunctionsService,
        private dialogService: DialogService,
        private sanitizer: DomSanitizer,
        private authenticationService: AuthenticationService,
    ) {}

    ngOnInit(): void {
        this.authenticationService.currentUserSetObservable.pipe(first()).subscribe((user) => {
            this.canManagePageContent = this.authenticationService.canManagePageContent(user);
            this.buildColumnDefs();
        });

        this.firmaPages$ = this.refreshPages$.pipe(
            switchMap(() => this.customRichTextService.listCustomRichText())
        );
        this.gridOptions = {
            onRowClicked: (event) => this.onRowClicked(event),
        };

        this.previewContent$ = this.selectedPageTypeID$.pipe(
            switchMap(id => {
                if (id == null) return of(null);
                return this.customRichTextService.getCustomRichText(id).pipe(
                    map(detail => ({
                        loading: false,
                        html: detail.FirmaPageContent
                            ? this.sanitizer.bypassSecurityTrustHtml(detail.FirmaPageContent)
                            : null,
                    } as PreviewState)),
                    startWith({ loading: true, html: null } as PreviewState)
                );
            }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    private buildColumnDefs(): void {
        this.columnDefs = [
            ...(this.canManagePageContent
                ? [
                      this.utilityFunctions.createActionsColumnDef((params) => {
                          const page = params.data as FirmaPageGridRow;
                          return [{ ActionName: "Edit Content", ActionHandler: () => this.editContent(page), ActionIcon: "fa fa-pencil" }];
                      }),
                  ]
                : []),
            this.utilityFunctions.createBasicColumnDef("Page Name", "FirmaPageTypeDisplayName", { Width: 300 }),
            this.utilityFunctions.createBasicColumnDef("Type", "FirmaPageRenderTypeDisplayName", { Width: 110 }),
            this.utilityFunctions.createBooleanColumnDef("Has Content", "HasContent", { Width: 120, CustomDropdownFilterField: "HasContent" }),
        ];
    }

    onRowClicked(event: any): void {
        const page = event.data as FirmaPageGridRow;
        if (!page) return;

        this.selectedPage = page;
        this.selectedPageTypeID$.next(page.FirmaPageTypeID);
    }

    editContent(page: FirmaPageGridRow): void {
        const dialogRef = this.dialogService.open(PageContentModalComponent, {
            data: { firmaPageTypeID: page.FirmaPageTypeID, pageName: page.FirmaPageTypeDisplayName } as PageContentModalData,
            width: "900px",
        });
        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshPages$.next();
                // Re-push the ID to refresh the preview panel
                if (this.selectedPage?.FirmaPageTypeID === page.FirmaPageTypeID) {
                    this.selectedPageTypeID$.next(page.FirmaPageTypeID);
                }
            }
        });
    }
}
