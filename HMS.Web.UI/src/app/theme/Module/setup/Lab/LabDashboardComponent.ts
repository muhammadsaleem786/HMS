import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input } from '@angular/core';
import { Company } from '../../models/company.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CompanyService } from '../../setup/Setting/Company/company.service';
import { Router } from '@angular/router';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { LabDashboardService } from './LabDashboardService';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { FileService } from '../../../../CommonService/FileService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
@Component({
    selector: 'app-LabDashboard',
    templateUrl: './LabDashboardComponent.html',
    providers: [FormBuilder, CompanyService, LabDashboardService, FileService],
})
export class LabDashboardComponent implements OnInit {
    public model = new Company();
    public PayrollRegion: string;
    public Rights: number[] = [];
    public Modules: number[] = [];
    public Keywords: any[] = [];
    public IsEdit: boolean = false;
    public IsNewImage: boolean = true;   
    public CompanyObj: any; public UsersObj: any;
    constructor(public _fb: FormBuilder, public _router: Router, public companyService: CompanyService
        , public commonservice: CommonService, public toastr: CommonToastrService
        , public loader: LoaderService, private encrypt: EncryptionService, public _labService: LabDashboardService
        , private http: FileService) {
        this.Modules = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Modules')))
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')))
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.UsersObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
           
       
    }  
   
    Close() {
        this.model = new Company();
    }
}
