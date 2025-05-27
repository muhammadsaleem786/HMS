import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ApplicationUserModel, user_payment } from '../../Setting/ApplicationUser/ApplicationUserModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { PaymentService } from './PaymentService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { IMyDateModel } from 'mydatepicker';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
@Component({
    selector: 'setup-PaymentComponentForm',
    templateUrl: './PaymentComponentForm.html',
    moduleId: module.id,
    providers: [PaymentService],
})
export class PaymentComponentForm implements OnInit {
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
    public model = new ApplicationUserModel();
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
    public user_paymentdynamicArray = [];
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _fb: FormBuilder, public loader: LoaderService
        , public _AdminService: PaymentService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion =  this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("14");
        //this.Keywords = this.commonservice.GetKeywords("Expense");
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            Name: [''],
            UserID: [''],
            CompanyID: [''],
            RoleID: [''],
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
            PhoneNo: ['', [Validators.required]],
            AdminID: [''],
            IsDefault: [''],
            SlotTime: [''],
            AppStartTime: [''],
            IsOverLap: [''],
            AppEndTime: [''],
            DocWorkingDay: [''],
            Designation: [''],
            Qualification: [''],
            Type: [''],
            SpecialtyId: [''],
            SpecialtyDropdownId: [''],
            IsActivated: [''],
            UserImage: [''],
            ExpiryDate: [''],
            Pwd: [''],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.id = params.id;
                if (this.id > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this.loader.ShowLoader();
                    this._AdminService.GetById(this.id).then(m => {
                        
                        this.model = m.ResultSet.UserObj;
                        this.user_paymentdynamicArray = [];
                        if (this.model.user_payment2.length > 0)
                            this.user_paymentdynamicArray=this.model.user_payment2;
                        this.loader.HideLoader();
                    });
                } else {
                    this.loader.HideLoader();
                }
            });
    }
    onDateChanged(event: IMyDateModel) {
        if (event) {
        }
    }
    
    AddPayment() {
        var obj = new user_payment();
        this.user_paymentdynamicArray.push(obj);
        this.loader.HideLoader();
    }
    RemoveRow(rowno: any) {
        if (this.user_paymentdynamicArray.length > 1) {
            this.user_paymentdynamicArray.splice(rowno, 1);
        }
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.model.user_payment2 = [];
            this.model.user_payment2 = this.user_paymentdynamicArray;
            this._AdminService.UpdatePaymnt(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['home/Admin']);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._AdminService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/Admin/setting']);
                this.loader.HideLoader();
            });
        }
    }
    Close() {
        this.model = new ApplicationUserModel();
        this.submitted = false;
        this.IsEmpExist = false;
        this.IsAdmin = false;
        this.IsUpdateText = false;
    }
}
