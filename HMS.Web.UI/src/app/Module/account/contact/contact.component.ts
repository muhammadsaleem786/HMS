import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ValidationVariables } from '../../../AngularConfig/global';
import { AuthenticationService } from '../../../CommonService/AuthenticationService';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { LoaderService } from '../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
import { ContactService } from '../contact/contact.service';
import { AccountService } from '../account.service';
import { CommonService } from '../../../CommonService/CommonService';
@Component({
    selector: 'app-register',
    templateUrl: './contact.component.html',
    //styleUrls: ['../contact.component.css'],
    providers: [ContactService],

})
export class ContactComponent implements OnInit {
    model: any = {};
    public Form1: FormGroup;
    public submitted: boolean;
    public PayrollRegion: string;
    public Keywords: any[] = [];
    constructor(public _fb: FormBuilder, public contactService: ContactService, public _router: Router, public commonservice: CommonService, public accountService: AccountService,
        public loader: LoaderService, public toastr: CommonToastrService,
    ) {
        this.accountService.IsPayrollRegion();
        this.accountService.IsUserLogin();
        this.PayrollRegion = this.commonservice.getPayrollRegion();
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
        });
    }
  
    SaveOrUpdate(isValid: boolean): void {
        if (this.model.agree == false) {
            isValid = false;
            this.loader.HideLoader();
        }
        if (isValid) {
            this.loader.ShowLoader();
        }
    }
}
