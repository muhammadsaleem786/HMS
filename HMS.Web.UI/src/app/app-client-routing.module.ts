//import { NgModule } from '@angular/core';
//import { Routes, RouterModule } from '@angular/router';
//import { PortalloginComponent } from './Module/account/PortalLogin/Portallogin.component';
//const routes: Routes = [

//    { path: 'Portal', loadChildren: './theme/theme-client-routing.module#ThemeClientRoutingModule' },
//    {
//        path: 'Portal/Login', component: PortalloginComponent,
//    },
//    {
//        path: 'Portal/Portalforgotpassword', component: PortalForgotpasswordComponent,
//        //children: [{ path: '', component: ForgotpasswordComponent }]
//    },
//    {
//        path: 'Portal/Portalresetpassword', component: PortalResetpasswordComponent,
//        //children: [{ path: '', component: ResetpasswordComponent }]
//    },
//    {
//        path: 'Portal/Resetpasswordchanged', component: PortalpasswordchangeComponent,
//        //children: [{ path: '', component: ResetpasswordComponent }]
//    },
//    {
//        path: 'passwordchanged', component: AccountComponent,
//        //children: [{ path: '', component: PasswordchangedComponent }]
//    },
//    { path: '', redirectTo: 'login', pathMatch: 'full' },

//];

//@NgModule({
//    imports: [RouterModule.forRoot(routes), MatFormFieldModule],
//    exports: [RouterModule]
//})
//export class AppClientRoutingModule { }