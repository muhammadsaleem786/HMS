import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ContactComponent } from './Module/account/contact/contact.component';
import { ForgotpasswordComponent } from './Module/account/forgotpassword/forgotpassword.component';
import { ResetpasswordComponent } from './Module/account/resetpassword/resetpassword.component';
import { ForgotpassconfirmationComponent } from './Module/account/forgotpassconfirmation/forgotpassconfirmation.component';
import { PasswordchangedComponent } from './Module/account/passwordchanged/passwordchanged.component';

import { LoginComponent } from './Module/account/login/login.component';
import { AccountComponent } from './Module/account/account.component';
import { MatFormFieldModule } from '@angular/material';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
const routes: Routes = [

    { path: 'home', loadChildren: './theme/theme-routing.module#ThemeRoutingModule' },
    { path: 'home', loadChildren: './theme/Module/HTMLReports/report.module#ReportModule' },
     {
        path: 'login', component: LoginComponent,
    },
    {
        path: 'contact', component: ContactComponent,
    },  
    {
        path: 'forgotpassword', component: AccountComponent,
        children: [{ path: '', component: ForgotpasswordComponent }]
    },
    {
        path: 'passconfirmation', component: AccountComponent,
        children: [{ path: '', component: ForgotpassconfirmationComponent }]
    },
    {
        path: 'forgotpassword', component: AccountComponent,
        children: [{ path: '', component: ForgotpasswordComponent }]
    },
    {
        path: 'resetpassword', component: AccountComponent,
        children: [{ path: '', component: ResetpasswordComponent }]
    },
    {
        path: 'passwordchanged', component: AccountComponent,
        children: [{ path: '', component: PasswordchangedComponent }]
    },
    { path: '', redirectTo: 'login', pathMatch: 'full' },
];
@NgModule({
    imports: [RouterModule.forRoot(routes, {
        onSameUrlNavigation: 'reload'
    }),
     MatFormFieldModule, NgbModule
    ],
   
    exports: [RouterModule]
})
export class AppRoutingModule { }