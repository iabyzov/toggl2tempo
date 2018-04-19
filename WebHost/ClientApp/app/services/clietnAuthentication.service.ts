import { Inject, Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { AuthenticationService } from "../apiReference/Synchronization_AuthenticationService";
import { JiraAuthModel } from "../apiReference/Synchronization_JiraAuthModel";
import 'rxjs/add/operator/toPromise';
import { LoginResult } from "../apiReference/Synchronization_LoginResult";
import { ObjectUtils } from "../ObjectUtils";
import { LocalStorageService } from "angular-2-local-storage/dist";
 
@Injectable()
export class ClientAuthenticationService {
    constructor(
        private _authenticationService: AuthenticationService,
        private _localStorage: LocalStorageService,
        ) { }
 
    login(username: string, password: string): Promise<void> {
        // TODO [as]: Rework it
        const model = new JiraAuthModel();
        model.user = username;
        model.password = password;

        return this._authenticationService.login(model)
            .toPromise()
            .then((r: LoginResult) => {
                // login successful if there's a jwt token in the response
                if (ObjectUtils.isNotEmpty(r) && r.username) {
                    // store user details and jwt token in local storage to keep user logged in between page refreshes
                    this._localStorage.set('currentUser', JSON.stringify(r.username));
                }
            });
    }
}