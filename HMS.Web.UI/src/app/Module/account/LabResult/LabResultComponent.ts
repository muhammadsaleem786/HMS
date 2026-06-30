import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input, TemplateRef } from '@angular/core';
import { lab_result } from './LabResultModel';
import { FormGroup, FormControl, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LoaderService } from '../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
import { LabResultService } from './LabResultService';
import { GlobalVariable } from '../../../AngularConfig/global';
import { EncryptionService } from '../../../CommonService/encryption.service';

@Component({
    selector: 'app-lab-result',
    templateUrl: './LabResultComponent.html',
    styleUrls: ['./LabResultComponent.css'],
    providers: [LabResultService],
})
export class LabResultComponent implements OnInit {
    public model = new lab_result();
    public PayrollRegion: string;
    public ReportList: any[] = [];
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    captchaToken: string;
    siteKey = '6Lcd9j0tAAAAACd0nNQ2ZD6zDUYytxtwch07s-aN';
    constructor(public _fb: FormBuilder, public _router: Router
        , public toastr: CommonToastrService
        , public loader: LoaderService, private encrypt: EncryptionService, public _LabResultService: LabResultService) {
    }
    ngOnInit() {

        this.Form1 = this._fb.group({
            InvoiceNo: ['', [Validators.required]],
            AccessCode: ['', [Validators.required]]
        });
    }
    resolved(token: string) {
        this.captchaToken = token;
    }
    SearchReports(valid: boolean) {
        if (!valid) {
            return;
        }
        if (!this.captchaToken) {
            alert('Please complete the reCAPTCHA.');
            return;
        }
        this.ReportList = [];
        this.loader.ShowLoader();
        this._LabResultService.GetById(this.model.InvoiceNo, this.model.AccessCode).then(m => {
            if (m.IsSuccess) {
                this.ReportList = m.ResultSet.result;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    DownloadReport(item:any) {

    }
}
