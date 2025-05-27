import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoaderService } from '../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../AngularConfig/global';
import { SetupPass } from '../models/setup-pass.model';
import { AccountService } from '../../../Module/account/account.service';
import { User } from '../../../Module/model/User.model';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
import { CommonService } from '../../../CommonService/CommonService';
import { strongPasswordValidator } from '../../../common/passwordValidator';

@Component({
    selector: 'app-changepassword',
    templateUrl: './changepassword.component.html',
    styleUrls: ['./changepassword.component.css'],
})
export class ChangepasswordComponent implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    public model = new User();
    public Keywords: any[] = [];
    public ControlRights: any;
    constructor(public _fb: FormBuilder, public _router: Router, public accountService: AccountService,
        public loader: LoaderService, public toastr: CommonToastrService, public _CommonService: CommonService) {
        this.ControlRights = this._CommonService.ScreenRights("19");
    }

    ngOnInit() {
        this.Form1 = this._fb.group({
            CurrentPassword: ['', [<any>Validators.required, <any>Validators.minLength(6)]],
            Pwd: ['',[Validators.required, strongPasswordValidator()]],
            CPassword: [''],
        });
    }
    get passwordControl() {
        return this.Form1.get('Pwd');
    }
    Save(isValid: boolean): void {
        if (isValid)
            isValid = this.model.Pwd == this.model.CPassword;
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.loader.ShowLoader();
            this.submitted = false;
            this.accountService.changePassword(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    this.toastr.Success(result.Message);
                    this.accountService.Logout();
                    this._router.navigate(['/login']);
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage)

                this.loader.HideLoader();
            });
        }
    }

}
