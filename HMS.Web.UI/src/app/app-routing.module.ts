//app-routing.module.ts
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './Module/account/login/login.component';
import { AccountComponent } from './Module/account/account.component';
import { ForgotpasswordComponent } from './Module/account/forgotpassword/forgotpassword.component';
import { ResetpasswordComponent } from './Module/account/resetpassword/resetpassword.component';
import { ForgotpassconfirmationComponent } from './Module/account/forgotpassconfirmation/forgotpassconfirmation.component';
import { PasswordchangedComponent } from './Module/account/passwordchanged/passwordchanged.component';
const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'forgotpassword',
        component: AccountComponent,
        children: [{ path: '', component: ForgotpasswordComponent }]
    },
    {
        path: 'passconfirmation',
        component: AccountComponent,
        children: [{ path: '', component: ForgotpassconfirmationComponent }]
    },
    {
        path: 'resetpassword',
        component: AccountComponent,
        children: [{ path: '', component: ResetpasswordComponent }]
    },
    {
        path: 'passwordchanged',
        component: AccountComponent,
        children: [{ path: '', component: PasswordchangedComponent }]
    },
    {
        path: 'home',
        loadChildren: './theme/theme.module#ThemeModule'
    },
    { path: '**', redirectTo: 'login' }
];
@NgModule({
    imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
    exports: [RouterModule]
})
export class AppRoutingModule { }