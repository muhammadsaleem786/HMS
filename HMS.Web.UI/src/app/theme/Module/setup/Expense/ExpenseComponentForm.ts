import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { emr_expense } from './ExpenseModel';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ExpenseService } from './ExpenseService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { IMyDateModel } from 'mydatepicker';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
@Component({
    selector: 'setup-ExpenseComponentForm',
    templateUrl: './ExpenseComponentForm.html',
    moduleId: module.id,
    providers: [ExpenseService],
})
export class ExpenseComponentForm implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = [];
    public IsSameModuleName: string;
    public IsChecked: boolean;
    public IsEmpExist: boolean = false;
    public IsAdmin: boolean = false;
    public IsUpdateText: boolean = false;
    public model = new emr_expense();
    public PayrollRegion: string;
    public PaymentList: any[] = [];
    public CompanyList: any[] = [];
    public CategoryList: any[] = [];
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public AttachImage: string = '';
    public IsNewImage: boolean = true;
    public Rights: any;
    public ControlRights: any;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _fb: FormBuilder, public loader: LoaderService
        , public _ExpenseService: ExpenseService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion =  this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("14");
        //this.Keywords = this.commonservice.GetKeywords("Expense");
    }
    ngOnInit() {
        this.loadDropdown();
        this.Form1 = this._fb.group({
            CategoryId: ['', [Validators.required]],
            CategoryDropdownId: [''],
            Date: ['', [Validators.required]],
            Remark: ['', [Validators.required]],
            Amount: ['', [Validators.required]],
            ClinicId: ['', [Validators.required]],
            InvoiceDate: [''],
            InvoiceNumber: [''],
            Vendor: [''],
            PaymentStatusId: [''],
            PaymentStatusDropdownId: [''],
            PaymentRemrks: [''],
            Attachment: [''],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.id = params.id;
                if (this.id > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this.loader.ShowLoader();
                    this._ExpenseService.GetById(this.id).then(m => {
                        this.model = m.ResultSet;
                        if (this.model.Attachment != null && this.model.Attachment != undefined && this.model.Attachment != "") {
                            this.getImageUrlName(this.model.Attachment);
                            this.IsNewImage = false;
                        } else this.IsNewImage = true;
                        this.loader.HideLoader();
                    });
                } else {
                    this.model.Date = new Date();
                    this.model.InvoiceDate = new Date();
                    this.loader.HideLoader();
                }
            });
    }
    loadDropdown() {
        this.loader.ShowLoader();
        this._ExpenseService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.CategoryList = m.ResultSet.categoryList;
                this.PaymentList = m.ResultSet.PaymentMethodList;
                this.CompanyList = m.ResultSet.CompanyList;
                if (this.CompanyList.length == 1)
                    this.model.ClinicId = this.CompanyList[0].ID;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._ExpenseService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['home/Expense']);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    onDateChanged(event: IMyDateModel) {
        if (event) {
            var dob = new Date(this.model.Date);
        }
    }
    onInvoiceDateChanged(event: IMyDateModel) {
        if (event) {
        }
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
    }
    getFileName(FName) {
        this.model.Attachment = FName;
    }
    ClearImageUrl() {
        this.IsNewImage = true;
        this.model.Attachment = '';
        this.AttachImage = '';
    }
    getImageUrlName(FName) {
        this.model.Attachment = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.AttachImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        } else {
            this.AttachImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._ExpenseService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/Expense']);
                this.loader.HideLoader();
            });
        }
    }
    Close() {
        this.model = new emr_expense();
        this.submitted = false;
        this.IsEmpExist = false;
        this.IsAdmin = false;
        this.IsUpdateText = false;
    }
}
