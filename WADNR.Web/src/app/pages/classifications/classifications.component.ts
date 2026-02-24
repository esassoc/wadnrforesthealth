import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import { map, Observable, shareReplay, Subject, startWith, switchMap } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { TruncateWordsPipe } from "src/app/shared/pipes/truncate-words.pipe";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ClassificationService } from "src/app/shared/generated/api/classification.service";
import { ClassificationSystemService } from "src/app/shared/generated/api/classification-system.service";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { ClassificationWithProjectCount } from "src/app/shared/generated/model/classification-with-project-count";
import { ClassificationModalComponent, ClassificationModalData } from "./classification-modal/classification-modal.component";

@Component({
    selector: "classifications",
    standalone: true,
    imports: [PageHeaderComponent, RouterModule, AsyncPipe, TruncateWordsPipe],
    templateUrl: "./classifications.component.html",
    styleUrls: ["./classifications.component.scss"],
})
export class ClassificationsComponent {
    public classifications$: Observable<ClassificationWithProjectCount[]>;

    public customRichTextTypeID: number = FirmaPageTypeEnum.Classifications;
    public isAdmin$: Observable<boolean>;

    private refreshData$ = new Subject<void>();

    constructor(
        private classificationService: ClassificationService,
        private classificationSystemService: ClassificationSystemService,
        private authenticationService: AuthenticationService,
        private dialogService: DialogService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.isAdmin$ = this.authenticationService.currentUserSetObservable.pipe(
            map((user) => this.authenticationService.isUserAnAdministrator(user)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.classifications$ = this.refreshData$.pipe(
            startWith(undefined),
            switchMap(() => this.classificationService.listWithProjectCountClassification())
        );
    }

    public tileColor(classification: ClassificationWithProjectCount): string {
        return classification?.ThemeColor || "var(--card-body-bg-color)";
    }

    openCreateModal(): void {
        this.classificationSystemService.listLookupClassificationSystem().subscribe((classificationSystems) => {
            const dialogRef = this.dialogService.open(ClassificationModalComponent, {
                data: {
                    mode: "create",
                    classificationSystems: classificationSystems,
                } as ClassificationModalData,
                size: "md",
            });

            dialogRef.afterClosed$.subscribe((result) => {
                if (result) {
                    this.router.navigate(["/classifications", result.ClassificationID]);
                }
            });
        });
    }
}
