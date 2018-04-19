import { NgModel } from '@angular/forms/src/directives';
import { Component } from '@angular/core';
import { TogglAuthModel } from "../../apiReference/Synchronization_TogglAuthModel";
import { AuthenticationService } from "../../apiReference/Synchronization_AuthenticationService";
import 'rxjs/add/operator/toPromise';

@Component({
    selector: 'config',
    templateUrl: './config.component.html',
    providers: [AuthenticationService]
})
export class ConfigComponent {
    public togglAuthModel: TogglAuthModel;

    constructor(
        private _authService: AuthenticationService
    ) {
        this.togglAuthModel = new TogglAuthModel();
    }

    public togglSignIn(): void {
        this._authService.signInToggl(this.togglAuthModel)
            .toPromise();
    }

    public shouldShowErrorMsg(model: NgModel) {
        return !model.valid && !model.pristine
    }
}
