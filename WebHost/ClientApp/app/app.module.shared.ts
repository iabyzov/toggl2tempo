import {LocalStorageService} from 'angular-2-local-storage';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DatepickerModule } from 'angular2-material-datepicker';
import { NgProgressModule } from 'ngx-progressbar';

import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { SyncComponent } from './components/sync/sync.component';
import { ConfigComponent } from './components/config/config.component';
import { AlertComponent } from "./components/alert/alert.component";
import { LoginComponent } from "./components/login/login.component";
import { AuthGuard } from "./guards/auth.guard";
import { ClientAuthenticationService } from "./services/clietnAuthentication.service";
import { LocalStorageModule } from "angular-2-local-storage/dist";

export const sharedConfig: NgModule = {
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        ConfigComponent,
        SyncComponent,
        AlertComponent,
        LoginComponent
    ],
    imports: [
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full', canActivate: [AuthGuard] },
            { path: 'login', component: LoginComponent },
            { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
            { path: 'config', component: ConfigComponent, canActivate: [AuthGuard] },
            { path: 'sync', component: SyncComponent, canActivate: [AuthGuard]  },
            { path: '**', redirectTo: 'login' }
        ]),
        DatepickerModule,
        NgProgressModule,
        LocalStorageModule.withConfig({
            prefix: 'my-app',
            storageType: 'localStorage'
        })
    ],
    providers: [
        AuthGuard,
        ClientAuthenticationService,
        LocalStorageService
    ]
};
