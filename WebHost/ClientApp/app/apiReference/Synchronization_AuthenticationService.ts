// ! Generated Code !

import {Http, Response, Headers, RequestOptions} from "@angular/http";
import {Injectable} from "@angular/core";
import {Observable} from "rxjs/Rx";
import 'rxjs/add/operator/map';

import {JiraAuthModel} from "./Synchronization_JiraAuthModel";
import {TogglAuthModel} from "./Synchronization_TogglAuthModel";
import {LoginResult} from "./Synchronization_LoginResult";

@Injectable()
export class AuthenticationService {
    constructor(private http: Http) {
    }

    public signInJira(model: JiraAuthModel):Observable<any> {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });
        return this.http.post(`api/authentication/signInJira`, JSON.stringify(model), options)
            .map((_res:Response) => null);
    }

    public signInToggl(model: TogglAuthModel):Observable<any> {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });
        return this.http.post(`api/authentication/signInToggl`, JSON.stringify(model), options)
            .map((_res:Response) => null);
    }

    public login(model: JiraAuthModel):Observable<LoginResult> {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });
        return this.http.post(`api/authentication/login`, JSON.stringify(model), options)
            .map((res:Response) => <LoginResult>(res.text() ? res.json() : null));
    }
}