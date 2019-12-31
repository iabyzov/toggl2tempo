import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ClientAuthenticationService } from "../../services/client-authentication.service";
import { AlertService } from "../../services/alert.service";
import { AuthenticationService } from "../../apiReference/Synchronization_AuthenticationService";
import { LocalStorageService } from "angular-2-local-storage/dist";
import 'rxjs/add/operator/toPromise';
 
@Component({
    templateUrl: 'login.component.html',
    providers: [
        ClientAuthenticationService,
        AuthenticationService,
        AlertService,
        LocalStorageService
    ]
})
export class LoginComponent implements OnInit {
    model: any = {};
    loading = false;
    returnUrl: string;
 
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: ClientAuthenticationService,
        private alertService: AlertService) { }
 
    ngOnInit() {
        // // reset login status
        // this.authenticationService.logout();
 
        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }
 
    login() {
        this.loading = true;
        this.authenticationService.login(this.model.username, this.model.password)
            .then(data => {
                this.router.navigate([this.returnUrl]);
            })
            .catch(error => {
                this.alertService.error(error);
                this.loading = false;
            });
    }
}