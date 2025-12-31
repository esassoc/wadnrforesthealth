import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { PersonDetail } from "src/app/shared/generated/model/person-detail";
import { FirmaPageTypeEnum } from "src/app/shared/generated/enum/firma-page-type-enum";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { Observable } from "rxjs";
import { Title } from "@angular/platform-browser";

@Component({
    selector: "app-home-index",
    templateUrl: "./home-index.component.html",
    styleUrls: ["./home-index.component.scss"],
    imports: [CustomRichTextComponent],
})
export class HomeIndexComponent implements OnInit {
    public currentUser$: Observable<PersonDetail>;

    public customRichTextTypeID: number = FirmaPageTypeEnum.HomePage;

    constructor(private router: Router, private route: ActivatedRoute, private titleService: Title) {}

    public ngOnInit(): void {
        // this.currentUser$ = this.authenticationService.getCurrentUser();
        // this.route.queryParams.subscribe((params) => {
        //     //We're logging in
        //     if (params.hasOwnProperty("code")) {
        //         this.router.navigate(["/signin-oidc"], { queryParams: params });
        //         return;
        //     }
        //     if (localStorage.getItem("loginOnReturn")) {
        //         localStorage.removeItem("loginOnReturn");
        //         this.authenticationService.login();
        //     }
        //     //We were forced to logout or were sent a link and just finished logging in
        //     if (sessionStorage.getItem("authRedirectUrl")) {
        //         this.router.navigateByUrl(sessionStorage.getItem("authRedirectUrl")).then(() => {
        //             sessionStorage.removeItem("authRedirectUrl");
        //         });
        //         return;
        //     }
        //     this.titleService.setTitle(`Lake Tahoe Info`);
        //     this.router.navigate(["./"]);
        // });
    }

    public login(): void {
        // this.authenticationService.login(true);
    }

    public createAccount(): void {
        // this.authenticationService.createAccount();
    }
}
