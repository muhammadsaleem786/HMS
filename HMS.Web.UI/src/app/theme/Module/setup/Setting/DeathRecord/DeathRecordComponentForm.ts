import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ipd_death_record_register } from '../DeathRecord/DeathRecordModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { DeathRecordService } from './DeathRecordService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AppointmentService } from './../../Appointment/AppointmentService';
import { GlobalVariable } from '../../../../../AngularConfig/global';
declare var $: any;
import { debounce } from 'lodash';
@Component({
    templateUrl: './DeathRecordComponentForm.html',
    moduleId: module.id,
    providers: [DeathRecordService,AppointmentService],
})
export class DeathRecordComponentForm implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = [];
    public model = new ipd_death_record_register();
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
    public CompanyObj: any; public patientInfo: any;
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public loader: LoaderService
        , public _DeathRecordService: DeathRecordService,public _AppointmentService:AppointmentService, private encrypt: EncryptionService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ControlRights = this._CommonService.ScreenRights("108");
    }
    ngOnInit() {
        this.model.DeathDate = new Date();
        this.model.DateTimeOfDeath = new Date();
        this.Form1 = this._fb.group({
            DeathDate: ['', [Validators.required]],
            MRNo: [''],
            PatientName: [''],
            PatientId:[''],
            FatherHusbandName: [''],
            Age: [''],
            Gender: [''],
            Address: [''],
            IDCardNo: [''],
            ContractNo: [''],
            DateTimeOfDeath: ['', [Validators.required]],
            ProbableCauseOfDeath: [''],
            DeathCertificateHandedOverTo: [''],
            AttendantSignature: [''],
            DoctorSignature: [''],
            Attachment: [''],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._DeathRecordService.GetById(this.model.ID).then(m => {
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
    LoadPatient() {
        $('#PatientSearch').autocomplete({
            source: debounce((request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.searchByMRNO(request.term).then(m => {
                    this.patientInfo = m.ResultSet.PatientInfo;
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            }, 300),
            minLength: 3,
            select: (event, ui) => {
                this.model.PatientName = ui.item.PatientName;
                this.model.PatientId = ui.item.value;
                this.model.FatherHusbandName = ui.item.Father_Husband;
                this.model.Age = ui.item.Age;
                this.model.Gender = ui.item.Gender === 1 ? 'Male' : ui.item.Gender === 2 ?'Female':'Other';
                this.model.Address = ui.item.Address;
                this.model.IDCardNo = ui.item.CNIC;
                this.model.ContractNo = ui.item.Phone;
            }
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._DeathRecordService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['/home/DeathRecord']);
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
            debugger
            this._DeathRecordService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/DeathRecord']);
                this.loader.HideLoader();
            });
        }
    }
    Close() {
        this.model = new ipd_death_record_register();
        this.submitted = false;
        this.IsAdmin = false;
    }
}
