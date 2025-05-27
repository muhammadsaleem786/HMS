import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { AccountService } from '../../../../../../app/Module/account/account.service';
import { strongPasswordValidator } from '../../../../../../app/common/passwordValidator';
import { Company } from '../../../models/company.model';
import { User } from '../../../../../../app/Module/model/User.model';
import { Login } from '../../../../../../app/Module/model/Login.model';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
@Component({
    selector: 'setup-RegisterComponentForm',
    templateUrl: './RegisterComponentForm.html',
    moduleId: module.id,
    providers: [],
})
export class RegisterComponentForm implements OnInit {
    model: any = {};
    public Companymodel = new Company();
    public usermodel = new User();
    public Form1: FormGroup;
    public Form2: FormGroup;
    public Form3: FormGroup;
    public submitted: boolean;
    public IsRegshow: boolean = false;
    public IsAlreadyUser: boolean = false;
    AccountType: string;
    public Loginmodel = new Login();
    public PayrollRegion: string;
    public Recaptcha: boolean = false;
    public Keywords: any[] = [];
    public captcha: any[];

    constructor(public _fb: FormBuilder,public _router: Router,public accountService: AccountService, public commonservice: CommonService,

        public loader: LoaderService, public toastr: CommonToastrService,
    ) {
        this.accountService.IsPayrollRegion();
        this.accountService.IsUserLogin();

        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.usermodel.MultilingualId = 1;
        this.model.MultilingualId = 1;
    }

    ngOnInit() {
       
        this.Form2 = this._fb.group({
            Name: ['', [Validators.pattern(ValidationVariables.AlphabetPattern), Validators.required]],
            CompanyName: ['', [Validators.pattern(ValidationVariables.AlphabetPattern), Validators.required]],
            Pwd: ['', [Validators.required, strongPasswordValidator()]],
            CPassword: [''],
            agree: [''],
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
            PhoneNo: ['', [Validators.required]],

            //PhoneNo: ['', [Validators.pattern(ValidationVariables.NumberPattern), <any>Validators.required, <any>Validators.minLength(6), <any>Validators.maxLength(20)]],
        });
     
    }
    resolved(captchaResponse: any[]) {
        this.captcha = captchaResponse;
        this.Recaptcha = true;
    }
   
    SaveOrUpdate(isValid: boolean): void {
        if (isValid)
            isValid = this.model.Pwd == this.model.CPassword;
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.loader.ShowLoader();
            this.model.Source = window.location.hash.split("=")[1];
            if (this.model.Source == undefined)
                this.model.Source = 0;
            this.submitted = false;
            if (this.AccountType)
                this.model.AccountType = this.AccountType;
            else
                this.model.AccountType = "A";
            this.model.CompanyName = this.model.CompanyName;
            this.model.Email = this.model.Email;
            this.model.ContactPersonFirstName = this.model.Name;
            this.model.ContactPersonLastName = this.model.Name;
            this.model.GenderID = 1;
            this.model.Phone = this.model.PhoneNo;
            this.model.Fax = this.model.Fax;
            this.model.LanguageID = 1;
            this.model.CompanyTypeID = 15;
            this.model.IsApproved = false;
            this.model.agree = true;
            this.accountService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['home/Admin']);
                    this.loader.HideLoader();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }

    get passwordControl() {
        return this.Form2.get('Pwd');
    }
}
