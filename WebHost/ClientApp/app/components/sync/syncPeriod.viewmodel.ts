import { SyncPeriodModel } from "../../apiReference/Synchronization_SyncPeriodModel";
import { DateUtils } from "../../utils/date.utils";

export class SyncPeriodViewModel {
    private _startTime: Date;
    private _endTime: Date;

    constructor() {
        this.startTime = new Date();
        this.endTime = new Date();
    }

    public get startTime(): Date { return this._startTime; }
    public set startTime(value: Date) { this._startTime = value; }

    public get endTime(): Date { return this._endTime; }
    public set endTime(value: Date) { this._endTime = value; }

    public getSyncPeriodModel(): SyncPeriodModel {
        const model = new SyncPeriodModel();
        model.startTime = DateUtils.toIsoDateTimeStringIgnoringTimezone(this.startTime);
        model.endTime = DateUtils.toIsoDateTimeStringIgnoringTimezone(this.endTime);

        return model;
    }
}
