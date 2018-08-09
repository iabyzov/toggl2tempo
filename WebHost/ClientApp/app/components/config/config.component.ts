import { NgModel } from '@angular/forms/src/directives';
import { Component } from '@angular/core';
import { TogglAuthModel } from "../../apiReference/Synchronization_TogglAuthModel";
import { TempoAuthModel } from "../../apiReference/Synchronization_TempoAuthModel";
import { AuthenticationService } from "../../apiReference/Synchronization_AuthenticationService";
import 'rxjs/add/operator/toPromise';

@Component({
    selector: 'config',
    templateUrl: './config.component.html',
    providers: [AuthenticationService]
})
export class ConfigComponent {
    public togglAuthModel: TogglAuthModel;
    public tempoAuthModel: TempoAuthModel;

    constructor(
        private _authService: AuthenticationService
    ) {
        this.togglAuthModel = new TogglAuthModel();
        this.tempoAuthModel = new TempoAuthModel();
    }

    public signIn(): void {
        this._authService.signInToggl(this.togglAuthModel)
            .toPromise().then(() => this._authService.signInTempo(this.tempoAuthModel).toPromise());
    }

    public shouldShowErrorMsg(model: NgModel) {
        return !model.valid && !model.pristine;
    }
}
