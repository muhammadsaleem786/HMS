import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ipd_sentinel_event_register } from '../EventRegister/EventRegisterModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EventRegisterService } from './EventRegisterService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { GlobalVariable } from '../../../../../AngularConfig/global';
declare var $: any;
import { debounce } from 'lodash';
@Component({
    templateUrl: './EventRegisterComponentForm.html',
    moduleId: module.id,
    providers: [EventRegisterService],
})
export class EventRegisterComponentForm implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = [];
       public model = new ipd_sentinel_event_register();
    public IsAdmin: boolean = false;
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public Rights: any;
    public ControlRights: any;
    public AttachImage: string = '';
    public IsNewImage: boolean = true;
    public CompanyObj: any;
    constructor(public _CommonService: CommonService,public _fb: FormBuilder, public loader: LoaderService
        , public _EventRegisterService: EventRegisterService, private encrypt: EncryptionService, public commonservice: CommonService, public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("111");
         this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
    }
    ngOnInit() {
        this.model.EventDate = new Date(); 
        this.Form1 = this._fb.group({
            EventDate: ['', [Validators.required]],
            EventType: ['', [Validators.required]],
            EventDescription: [''],
            ImmediateActionTaken: [''],
            OutcomeDetails: [''],
            ReportedBy: [''],
            ReviewedByClinicalManager: [''],
            IsSigned: [],
            Attachment: [''],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._EventRegisterService.GetById(this.model.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.model = m.ResultSet.obj;
                            if (this.model.Attachment != null && this.model.Attachment != undefined && this.model.Attachment != "") {
                                this.getImageUrlName(this.model.Attachment);
                                this.IsNewImage = false;
                            } else this.IsNewImage = true;
                        }
                        this.loader.HideLoader();
                    });
                } else {
                }
            });
    }
    onDateChanged(event: any) {

    }
    ClearImageUrl() {
        this.IsNewImage = true;
        this.model.Attachment = '';
        this.AttachImage = '';
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
    }
    getImageUrlName(FName) {
        this.model.Attachment = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.AttachImage = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + FName;
        } else {
            this.AttachImage = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + FName;
        }
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; 
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._EventRegisterService.SaveOrUpdate(this.model).then(m => {
                    var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                        this._router.navigate(['/home/EventRegister']);
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
            this._EventRegisterService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/EventRegister']);
                    this.loader.HideLoader();
            });
        }
    }
    Close() {
        this.model = new ipd_sentinel_event_register();
        this.submitted = false;
        this.IsAdmin = false;
    }
}
