// ! Generated Code !

import {Http, Response, Headers, RequestOptions} from "@angular/http";
import {Injectable} from "@angular/core";
import {Observable} from "rxjs/Rx";
import 'rxjs/add/operator/map';

import {SyncPeriodModel} from "./Synchronization_SyncPeriodModel";
import {SyncResultModel} from "./Synchronization_SyncResultModel";

@Injectable()
export class SynchronizationService {
    constructor(private http: Http) {
    }

    public sync(period: SyncPeriodModel):Observable<SyncResultModel> {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });
        return this.http.post(`api/synchronization/sync`, JSON.stringify(period), options)
            .map((res:Response) => <SyncResultModel>(res.text() ? res.json() : null));
    }
}