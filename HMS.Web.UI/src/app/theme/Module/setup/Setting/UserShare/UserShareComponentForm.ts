import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { UserShareModel } from '../UserShare/UserShareModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { UserShareService } from './UserShareService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
@Component({
    templateUrl: './UserShareComponentForm.html',
    moduleId: module.id,
    providers: [UserShareService],
})
export class UserShareComponentForm implements OnInit {
    public Form1: FormGroup;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false; public submitted: boolean;
    public ScreenLists = [];
    public Modules = [];
    public model = new UserShareModel();
    public IsAdmin: boolean = false;
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public DoctorList: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public Rights: any;
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public loader: LoaderService
        , public _UserShareService: UserShareService, private encrypt: EncryptionService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("77");
    }
    ngOnInit() {
        this.loadDropdown();
        this.Form1 = this._fb.group({
            DoctorId: ['', [Validators.required]],
            GeneralDoctorShare: [''],
            GeneralOwnerShare: [''],
            ProcedureDoctorShare: [''],
            ProcedureOwnerShare: [''],
            DiscountDoctorShare: [''],
            DiscountOwnerShare: ['']
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._UserShareService.GetById(this.model.ID).then(m => {
                        if (m.IsSuccess) {
                            this.model = m.ResultSet.result;
                        }
                        this.loader.HideLoader();
                    });
                } else {
                }
            });
    }
    loadDropdown() {
        this.loader.ShowLoader();
        this._UserShareService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.DoctorList = m.ResultSet.result;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._UserShareService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['/home/share']);
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
            this._UserShareService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/share']);
                this.loader.HideLoader();
            });
        }
    }
    Close() {
        this.model = new UserShareModel();
        this.submitted = false;
        this.IsAdmin = false;
    }
    generalLimitShare(type: any) {
        if (type == 'D') {
            if (this.model.GeneralDoctorShare > 100) {
                this.toastr.Error('Error', 'Value not exceed 100.');
                this.model.GeneralDoctorShare = 0;
            }
        } else {
            if (this.model.GeneralOwnerShare > 100) {
                this.toastr.Error('Error', 'Value not exceed 100.');
                this.model.GeneralOwnerShare = 0;
            }
        }
    }
    procedureLimitShare(type: any) {
        if (type == 'D') {
            if (this.model.ProcedureDoctorShare > 100) {
                this.toastr.Error('Error', 'Value not exceed 100.');
                this.model.ProcedureDoctorShare = 0;
            }
        } else {
            if (this.model.ProcedureOwnerShare > 100) {
                this.toastr.Error('Error', 'Value not exceed 100.');
                this.model.ProcedureOwnerShare = 0;
            }
        }
    }
   
}
