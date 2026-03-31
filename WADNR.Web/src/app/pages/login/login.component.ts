import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AuthenticationService } from "src/app/services/authentication.service";

@Component({
    selector: "app-login",
    standalone: true,
    template: "",
})
export class LoginComponent implements OnInit {
    constructor(
        private router: Router,
        private authService: AuthenticationService
    ) {}

    ngOnInit(): void {
        if (!this.authService.isAuthenticated()) {
            this.authService.login();
        } else {
            this.router.navigate(["/"]);
        }
    }
}
