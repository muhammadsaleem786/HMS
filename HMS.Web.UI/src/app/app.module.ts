import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { LayoutModule } from './theme/layouts/layout.module';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ThemeRoutingModule } from "./theme/theme-routing.module";
import { HttpClientModule } from '@angular/common/http';
import { HttpService } from './CommonService/HttpService';
import { CommonService } from './CommonService/CommonService';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ContactComponent } from './Module/account/contact/contact.component';
import { AccountComponent } from './Module/account/account.component';
import { LoginComponent } from './Module/account/login/login.component';
import { LoaderService } from './CommonService/LoaderService';
import { AsidebarService } from './CommonService/AsidebarService';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HttpModule } from '@angular/http';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { AccountService } from './Module/account/account.service';
import { CommonToastrService } from './CommonService/CommonToastrService';
import { ToastrModule } from 'ngx-toastr';
import { CommonUtilsModule } from './common/common-utils.module';
import { ForgotpasswordComponent } from './Module/account/forgotpassword/forgotpassword.component';
import { ResetpasswordComponent } from './Module/account/resetpassword/resetpassword.component';
import { ForgotpassconfirmationComponent } from './Module/account/forgotpassconfirmation/forgotpassconfirmation.component';
import { PasswordchangedComponent } from './Module/account/passwordchanged/passwordchanged.component';

//import { RecaptchaModule, RecaptchaComponent } from 'ng-recaptcha';
@NgModule({
    declarations: [
        AppComponent,
        ContactComponent,
        LoginComponent,
        AccountComponent,
        ForgotpasswordComponent,
        ResetpasswordComponent,
        ForgotpassconfirmationComponent,
        PasswordchangedComponent,
    ],
    imports: [
        CommonUtilsModule,
        LayoutModule,
        BrowserModule,
        BrowserAnimationsModule,
        AppRoutingModule,
        HttpModule,
        HttpClientModule,
        ToastrModule.forRoot(),
        ReactiveFormsModule, FormsModule,

        //RecaptchaModule,
        NgbModule,
    ],
    entryComponents: [
    ],
    providers: [HttpService, CommonService, AsidebarService, LoaderService, AccountService, CommonToastrService, /*RecaptchaModule,*/ { provide: LocationStrategy, useClass: HashLocationStrategy }],
    bootstrap: [AppComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }