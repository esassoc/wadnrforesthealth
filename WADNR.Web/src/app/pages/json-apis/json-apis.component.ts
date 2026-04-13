import { Component } from "@angular/core";
import { CommonModule } from "@angular/common";
import { BehaviorSubject, Observable, combineLatest, switchMap, map, shareReplay } from "rxjs";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { CopyToClipboardDirective } from "src/app/shared/directives/copy-to-clipboard.directive";
import { PersonService } from "src/app/shared/generated/api/person.service";
import { PersonApiKey } from "src/app/shared/generated/model/person-api-key";
import { SystemInfoService } from "src/app/shared/generated/api/system-info.service";
import { AuthenticationService } from "src/app/services/authentication.service";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";


@Component({
    selector: "json-apis",
    standalone: true,
    imports: [CommonModule, PageHeaderComponent, IconComponent, CopyToClipboardDirective],
    templateUrl: "./json-apis.component.html",
    styleUrl: "./json-apis.component.scss",
})
export class JsonApisComponent {
    public scalarDocsUrl$: Observable<string | null> = this.systemInfoService.getSystemInfoSystemInfo().pipe(
        map((info) => (info.ScalarApiUrl ? `${info.ScalarApiUrl}/docs` : null)),
        shareReplay({ bufferSize: 1, refCount: true })
    );

    public currentUser$ = this.authenticationService.currentUserSetObservable;
    public apiKey$: Observable<PersonApiKey>;
    private refreshApiKey$ = new BehaviorSubject<void>(undefined);

    constructor(
        private personService: PersonService,
        private systemInfoService: SystemInfoService,
        private authenticationService: AuthenticationService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {
        this.apiKey$ = combineLatest([this.currentUser$, this.refreshApiKey$]).pipe(
            switchMap(([user]) => this.personService.getApiKeyPerson(user.PersonID)),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    async generateApiKey(personID: number, hasExistingKey: boolean): Promise<void> {
        if (hasExistingKey) {
            const confirmed = await this.confirmService.confirm({
                title: "Generate New API Key",
                message: "Are you sure you want to generate a new API key? This will invalidate your current API key.",
                buttonTextYes: "Generate",
                buttonClassYes: "btn-primary",
                buttonTextNo: "Cancel",
            });
            if (!confirmed) return;
        }
        this.personService.generateApiKeyPerson(personID).subscribe(() => {
            this.alertService.pushAlert(new Alert("API key generated successfully.", AlertContext.Success));
            this.refreshApiKey$.next();
        });
    }
}
