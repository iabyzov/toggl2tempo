import { Component, OnInit } from '@angular/core';
import { SynchronizationService } from "../../apiReference/Synchronization_SynchronizationService";
import { SyncPeriodModel } from "../../apiReference/Synchronization_SyncPeriodModel";
import { NgProgressService } from 'ngx-progressbar';
import { ObjectUtils } from "../../ObjectUtils";
import 'rxjs/add/operator/toPromise';
import { SyncPeriodViewModel } from "./syncPeriod.viewmodel";
import { SyncResultModel } from "../../apiReference/Synchronization_SyncResultModel";

@Component({
    selector: 'sync',
    templateUrl: './sync.component.html',
    providers: [SynchronizationService]
})
export class SyncComponent {
    public syncPeriod: SyncPeriodViewModel;

    public isSyncInProgress: boolean = false;

    public result: SyncResultModel;

    constructor(
        private _synchronizationService: SynchronizationService,
        private _progressService: NgProgressService
    ) {
        this.syncPeriod = new SyncPeriodViewModel();
        this.result = null;
    }

    public isSyncDisabled(): boolean {
        if(this.isSyncInProgress) {
            return true;
        }

        if(ObjectUtils.isEmpty(this.syncPeriod.startTime) ||
           ObjectUtils.isEmpty(this.syncPeriod.endTime))
        {
            return true;
        }

        return this.syncPeriod.startTime > this.syncPeriod.endTime;
    }

    public sync(): void {
        this.result = null;
        this.isSyncInProgress = true;
        this._progressService.start();
        
        this._synchronizationService.sync(this.syncPeriod.getSyncPeriodModel())
            .toPromise()
            .then((r: SyncResultModel) => {
                this._progressService.done();
                this.isSyncInProgress = false;
                this.result = r;
            });
    }
}
