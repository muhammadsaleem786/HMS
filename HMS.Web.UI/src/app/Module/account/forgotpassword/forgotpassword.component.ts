import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { AuthenticationService } from '../../../CommonService/AuthenticationService';
import { Router } from '@angular/router';
import { ValidationVariables } from '../../../AngularConfig/global';
import { LoaderService } from '../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
@Component({
    selector: 'app-forgotpassword',
    templateUrl: './forgotpassword.component.html',
    styleUrls: ['../account.component.css'],
    providers: [FormBuilder, AccountService, AuthenticationService]
})
export class ForgotpasswordComponent implements OnInit {
    model: any = {};
    public Form: FormGroup;
    public submitted: boolean;
    constructor(private _fb: FormBuilder, private _router: Router,
        private _authService: AuthenticationService, private accountService: AccountService
        , public loader: LoaderService, private toastr: CommonToastrService
    ) { }

    ngOnInit() {
        this.Form = this._fb.group({
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
        });
    }

    forgotpass(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();

            this.accountService.forgotpasswd(this.model.Email).then(
                data => {
                    debugger
                    if (data.IsSuccess) {
                        this._router.navigate(['/passconfirmation']);
                    } else {
                        this.toastr.Error('Error', data.ErrorMessage);
                    }
                    this.loader.HideLoader();
                },
                error => {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', error);
                });

        }
    }

}
