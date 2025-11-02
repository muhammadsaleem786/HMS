import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { SessionService } from './SessionService';
import { SessionModel } from './SessionModel';
import { ActivatedRoute } from '@angular/router';
import { IMyDateModel } from 'mydatepicker';
import jsPDF from 'jspdf';
import { ValidationVariables, GlobalVariable } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { filter } from 'rxjs/operators';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

declare var $: any;

@Component({
    moduleId: module.id,
    templateUrl: 'SessionComponentForm.html',
    providers: [SessionService],
})
export class SessionComponentForm {
    public model = new SessionModel();
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public sub: any;
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    public SMSTypeList: any[] = [];
    public TimeList: any[] = [];
    public BeforeList: any[] = [];
    public submitted: boolean;
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _SessionService: SessionService, public _fb: FormBuilder
        , public route: ActivatedRoute, public toastr: CommonToastrService
        , private encrypt: EncryptionService
    ) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("67");
    }
    ngOnInit() {
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._SessionService.GetById(this.model.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.model = m.ResultSet;
                            this.loader.HideLoader();
                        } else
                            this.loader.HideLoader();
                    });
                } else {
                    this.model.ShiftStartTime = new Date();
                    this.SelectTime();
                }
            });
        this.Form1 = this._fb.group({
            ShiftStartTime: ['', [Validators.required]],
            StartTime: [''],
            ShiftEndTime: [''],
            StartingCash: [''],
            ClosingBalance: [''],
            OpeningComments: [''],
            ClosingComments: ['']
        });
    }
    LoadDropDown() {
        this.loader.ShowLoader();
        this._SessionService.LoadDropdown("65,66").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet.dropdownValues;
                this.SMSTypeList = list.filter(a => a.DropDownID == 66);
                this.TimeList = list.filter(a => a.DropDownID == 65);
                this.loader.HideLoader();
            }
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        if (this.model.ShiftStartTime == undefined || this.model.ShiftStartTime == null) {
            this.toastr.Error("Shift start date is required.");
            return;
        }

        if (this.model.StartTime == undefined || this.model.StartTime == null) {
            this.toastr.Error("Shift start time is required.");
            return;
        }
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();

            this._SessionService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this._router.navigate(['/home/session']);
                    this.toastr.Success(result.Message);
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }
    SelectTime() {
        let Step = 30;
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        const timeString = this.formatTime(hours, minutes);
        this.model.StartTime = timeString;
        $("#TimeRecorded").timepicker({
            'step': Step,
            'timeFormat': 'h:i A'
        });
        if ($("#TimeRecorded").val() != null && $("#TimeRecorded").val() != "")
            this.model.StartTime = $("#TimeRecorded").val();
    }
    formatTime(hours: number, minutes: number): string {
        const ampm = hours >= 12 ? 'PM' : 'AM';
        const hour = hours % 12 || 12;  // Convert to 12-hour format
        const minute = minutes < 10 ? '0' + minutes : minutes;
        return hour + ':' + minute + ' ' + ampm;
    }
    ChangeSelectTime() {
        let Step = 30;
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        $("#TimeRecorded").timepicker({
            'step': Step,
            'timeFormat': 'h:i A'
        });
        if ($("#TimeRecorded").val() != null && $("#TimeRecorded").val() != "")
            this.model.StartTime = $("#TimeRecorded").val();
        $('#TimeRecorded').timepicker('remove');
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._SessionService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.toastr.Success(m.Message);
                this._router.navigate(['/home/session']);
            });
        }
    }
}