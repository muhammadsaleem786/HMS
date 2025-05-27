import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { AdmitService } from './AdmitService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_Appointment, emr_document, patientInfo, DoctorInfo } from './../Appointment/AppointmentModel';
import { emr_patient_bill } from '../Billing/BillingModel';
import { Observable } from 'rxjs';
import { AppointmentService } from './../Appointment/AppointmentService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
declare var $: any;
@Component({
    templateUrl: './AdmitDetailComponentList.html',
    moduleId: module.id,
    providers: [AdmitService, AppointmentService],
})

export class AdmitDetailComponentList implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public model = new emr_Appointment();
    public AdmitId: any;
    public ClinicName: string = "";
    public IsPatient: boolean = false;
    public GenderName: string = "";
    public PatientLits: any[] = [];
    public VisitList: any[] = [];
    public DoctorList: any[] = [];
    public Rights: any;
    public PatientId: any;
    public VisitRights: any;
    public Step: any = 30;
    nextPage: number = 1; public CompanyObj: any;
    totalPages: number = 0; public InvoiceBillModel: any[] = [];
    pagesRange: number[] = []; previousPage: number = 1;
    @ViewChild("NewVisit") NewVisit: TemplateRef<any>;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService
        , public _AdmitService: AdmitService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.VisitRights = this._CommonService.ScreenRights("39");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form1 = this._fb.group({
            PatientName: [''],
            PatientId: ['', [Validators.required]],
            CNIC: ['', [Validators.required, Validators.pattern(ValidationVariables.CNICPattern)]],
            Mobile: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.NumberPattern)]],
            DoctorId: ['', [Validators.required]],
            PatientProblem: [''],
            Notes: [''],
            AppointmentDate: ['', [Validators.required]],
            AppointmentTime: [''],
            StatusId: [''],
            VisitId: [''],
            Location: [''],
            //bill field
            AppointmentId: [''],
            ServiceId: [''],
            BillDate: [''],
            Price: [''],
            Discount: [''],
            PaidAmount: [''],
            DoctorName: [''],
            ServiceName: [''],
            Remarks: [''],
            OutstandingBalance: [''],
            SecondaryDescription: [''],
            PrimaryDescription: [''],
        });
    }

    loadReport() {
    }
    OpenVisitModal(NewVisit) {
        this.loader.ShowLoader();
        this.model = new emr_Appointment();
        let PatientId = localStorage.getItem('PatientId');
        this._AppointmentService.GetAdmitAppointmentDropdown(PatientId).then(m => {
            if (m.IsSuccess) {
                this.SelectTime();
                this.model.AppointmentDate = new Date();
                this.VisitList = m.ResultSet.VisitList;
                this.DoctorList = m.ResultSet.DoctorList;
                this.modalService.open(NewVisit, { size: 'md' });
            }
            this.loader.HideLoader();
        });
    }
    SelectTime() {
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        const timeString = this.formatTime(hours, minutes);
        this.model.AppointmentTime = timeString;
        $("#basicExample").timepicker({
            'step': this.Step,
            'timeFormat': 'h:i A',
            'defaultTime': timeString
        });
    }
    formatAMPM(time: any) {
        var hours = time.split(':')[0];
        var minutes = time.split(':')[1];
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }
    formatTime(hours: number, minutes: number): string {
        const ampm = hours >= 12 ? 'PM' : 'AM';
        const hour = hours % 12 || 12;
        const minute = minutes < 10 ? '0' + minutes : minutes;
        return hour + ':' + minute + ' ' + ampm;
    }
    AppointmentSaveOrUpdate(isValid: boolean): void {
        this.model.PatientId = parseInt(localStorage.getItem('PatientId'));
        this.model.AdmissionId = this.AdmitId;
        this.model.IsAdmission = true;
        this.model.IsAdmit = false;
        this.submitted = true; // set form submit to true
        //if (isValid) {
        if (this.model.PatientId == undefined || this.model.PatientId == null) {
            this.toastr.Error("Error", "Please select patient.");
            return;
        }
        if (this.model.AppointmentDate == undefined || this.model.AppointmentDate == null) {
            this.toastr.Error("Error", "Please select appointment date.");
            return;
        }
        if (this.model.AppointmentTime == undefined || this.model.AppointmentTime == null) {
            this.toastr.Error("Error", "Please select time.");
            return;
        }
        this.submitted = false;
        this.loader.ShowLoader();
        if (this.model.AppointmentTime == undefined)
            this.model.AppointmentTime = $('#basicExample').val().toString();
        this.model.AppointmentTime = this.Convert12TO24(this.model.AppointmentTime);
        this._AppointmentService.AppSaveOrUpdate(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.modalService.dismissAll(this.NewVisit);
                this.GetPatientList();
                this.model = new emr_Appointment();
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    Convert12TO24(time12h: any) {
        var hours = Number(time12h.match(/^(\d+)/)[1]);
        var minutes = Number(time12h.match(/:(\d+)/)[1]);
        var AMPM = time12h.match(/\s(.*)$/)[1];
        if (AMPM == "PM" && hours < 12) hours = hours + 12;
        if (AMPM == "AM" && hours == 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        return hours + ':' + minutes + ':00';
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.GetPatientList();
    }
    getPages(totalRecord: number, recordPerPage: number) {

        if (!isNaN(totalRecord))
            this.totalPages = this.getTotalPages(totalRecord, recordPerPage);
        this.getpagesRange();
    }
    getpagesRange() {
        if (this.pagesRange.indexOf(this.PModel.CurrentPage) == -1 || this.totalPages <= 10)
            this.papulatePagesRange();
        else if (this.pagesRange.length == 1 && this.totalPages > 10)
            this.papulatePagesRange();
    }
    papulatePagesRange() {
        this.pagesRange = [];
        var Result = Math.ceil(Math.max(this.PModel.CurrentPage, 1) / Math.max(this.PModel.RecordPerPage, 1));
        this.previousPage = ((Result - 1) * this.PModel.RecordPerPage)
        this.nextPage = (Result * this.PModel.RecordPerPage) + 1;
        if (this.nextPage > this.totalPages)
            this.nextPage = this.totalPages;
        for (var i = 1; i <= 10; i++) {
            if ((this.previousPage + i) > this.totalPages) return;
            this.pagesRange.push(this.previousPage + i)
        }
    }
    getTotalPages(totalRecord: number, recordPerPage: number): number {

        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
    }
    GetPatientList() {

        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "PatientName#PatientName,AppointmentTime#AppointmentTime";
        this._AppointmentService
            .AdmitAppointmentLoad(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PatientId, this.AdmitId).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.PatientLits = m.PatientList;
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AppointmentService.DeleteAppoint(id).then(m => {

                if (m.ErrorMessage != null) {

                    alert(m.ErrorMessage);
                }
                this.GetPatientList();
            });
        }
    }
}

