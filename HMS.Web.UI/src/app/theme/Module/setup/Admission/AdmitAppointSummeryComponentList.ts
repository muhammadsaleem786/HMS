import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo } from './../Appointment/AppointmentModel';
import { emr_patient_bill } from '../Billing/BillingModel';
import { emr_prescription_mf, emr_prescription_complaint, emr_prescription_diagnos, emr_prescription_investigation, emr_prescription_observation, emr_prescription_treatment, emr_prescription_treatment_template, emr_medicine } from './../Prescription/PrescriptionModel';
import { Observable } from 'rxjs';
import { AppointmentService } from './../Appointment/AppointmentService';
import { BillingService } from './../Billing/BillingService';
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
    templateUrl: './AdmitAppointSummeryComponentList.html',
    moduleId: module.id,
    providers: [AppointmentService, BillingService],
})

export class AdmitAppointSummeryComponentList implements OnInit {
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = []; public PatientVitalList: any[] = [];
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = []; public FutureAppList: any[] = []; public PreviousAppList: any[] = [];
    public ClinicName: string = "";
    public PatientId: any; previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public Form2: FormGroup;
    public Rights: any;
    public ControlRights: any;
    public patientInfo: any;
    public ServiceType: any;
    public TokenNo: any;
    public DisabledSlot: any;
    public Step: number = 30;
    public AppointmentModel = new emr_Appointment();
    public DoctorList: any[] = [];
    public StatusList: any[] = []; public CompanyObj: any;
    public DoctorClanderList: any[] = []; public AppointmentList: any[] = [];
    @ViewChild("longContent") ModelContent: TemplateRef<any>;
    openAppointmentModal(longContent) {
        this.loader.ShowLoader();
        this.AppointmentModel = new emr_Appointment();
        this._AppointmentService.GetAllDoctorList().then(m => {
            if (m.IsSuccess) {
                this.AppointmentList = m.ResultSet.AppointmentList;
                this.DoctorList = m.ResultSet.DoctorList;
                this.DoctorClanderList = m.ResultSet.DoctorCalander;
                this.StatusList = m.ResultSet.AllStatusList;
                this.ServiceType = m.ResultSet.ServiceType;
                this.TokenNo = m.ResultSet.TokenNo;
                if (this.Rights.indexOf(12) > -1) {
                    if (m.ResultSet.ServiceType != null) {
                        this.AppointmentModel.ServiceId = this.ServiceType.ID;
                        this.AppointmentModel.ServiceName = this.ServiceType.ServiceName;
                        this.AppointmentModel.Price = this.ServiceType.Price;
                        this.AppointmentModel.TokenNo = this.TokenNo;
                        this.AppointmentModel.AppointmentDate = new Date();
                        this.AppointmentModel.BillDate = new Date();
                        this.CalAmount();
                    }
                }
                this.modalService.open(longContent, { size: 'lg' });
            }
            this.loader.HideLoader();
        });
    }
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _BillingService: BillingService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("13");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        this.PatientId = localStorage.getItem('PatientId');
        this.PModel.ShowSearch == true
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form2 = this._fb.group({
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
        });
    }
    CalAmount() {
        if (this.AppointmentModel.Discount == undefined || this.AppointmentModel.Discount == null)
            this.AppointmentModel.Discount = 0;
        if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null)
            this.AppointmentModel.Price = 0;

        this.AppointmentModel.PaidAmount = this.AppointmentModel.Price - this.AppointmentModel.Discount;
        this.AppointmentModel.OutstandingBalance = this.AppointmentModel.Price - this.AppointmentModel.Discount - this.AppointmentModel.PaidAmount;

    }
    PaidCalAmount() {
        if (this.AppointmentModel.Discount == undefined || this.AppointmentModel.Discount == null)
            this.AppointmentModel.Discount = 0;
        if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null)
            this.AppointmentModel.Price = 0;
        this.AppointmentModel.OutstandingBalance = this.AppointmentModel.Price - this.AppointmentModel.Discount - this.AppointmentModel.PaidAmount;
    }
    AppointmentSaveOrUpdate(isValid: boolean): void {

        let CurrentDate = this._CommonService.GetFormatDate(new Date());
        let Appdate = this._CommonService.GetFormatDate(this.AppointmentModel.AppointmentDate);
        if (Appdate < CurrentDate) {
            this.toastr.Error("You can not add appointment previous date.");
            return;
        }
        this.submitted = true; // set form submit to true
        //if (isValid) {
        if (this.AppointmentModel.PatientId == undefined || this.AppointmentModel.PatientId == null) {
            this.toastr.Error("Error", "Please select patient.");
            return;
        }
        if (this.AppointmentModel.AppointmentDate == undefined || this.AppointmentModel.AppointmentDate == null) {
            this.toastr.Error("Error", "Please select appointment date.");
            return;
        }
        if (this.Rights.indexOf(12) > -1) {
            if (this.AppointmentModel.ServiceId == undefined || this.AppointmentModel.ServiceId == null) {
                this.toastr.Error("Error", "Please select service.");
                return;
            }
            if (this.AppointmentModel.BillDate == undefined || this.AppointmentModel.BillDate == null) {
                this.toastr.Error("Error", "Please select bill date.");
                return;
            }
            if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null) {
                this.toastr.Error("Error", "Please select price.");
                return;
            }
            if (this.AppointmentModel.AppointmentTime == undefined || this.AppointmentModel.AppointmentTime == null) {
                this.toastr.Error("Error", "Please select time.");
                return;
            }
        }
        this.submitted = false;
        this.loader.ShowLoader();
        if (this.AppointmentModel.AppointmentTime == undefined)
            this.AppointmentModel.AppointmentTime = $('#basicExample').val();
        this.AppointmentModel.AppointmentTime = this.Convert12TO24(this.AppointmentModel.AppointmentTime);
        this._AppointmentService.AppSaveOrUpdate(this.AppointmentModel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.modalService.dismissAll(this.ModelContent);
                this.PModel.SortName = "";
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.selectPage(this.PModel.CurrentPage);
                this.AppointmentModel = new emr_Appointment();
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    Convert12TO24(time12h: any) {
        const [time, modifier] = time12h.split(' ');

        let [hours, minutes] = time.split(':');

        //if (hours === '12') {
        //    hours = '00';
        //}

        if (modifier === 'pm') {
            hours = parseInt(hours, 10) + 12;
        }

        return hours + ':' + minutes + ':00';
    }
    onAppDateChanged(event: IMyDateModel) {
        let CurrentDate = this._CommonService.GetFormatDate(new Date());
        let date = this._CommonService.GetFormatDate(event.jsdate);
        if (date < CurrentDate) {
            this.toastr.Error("You can not add appointment previous date.");
            return;
        }
    }
    SelectTime() {
        if (this.AppointmentModel.DoctorId != null && this.AppointmentModel.DoctorId != undefined) {
            var doctorid = this.DoctorClanderList.filter(a => a.ID == this.AppointmentModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.AppointmentModel.DoctorId);
            let minTime = this.formatAMPM(doctorid[0].StartTime);
            let maxTime = this.formatAMPM(doctorid[0].EndTime);
            if (doctorid[0].SlotTime != null) {
                this.Step = parseInt(doctorid[0].SlotTime.split(':')[1]);
            }
            if (Appobj.length > 0) {
                this.FindDisabledSlot(Appobj);
                $("#basicExample").timepicker({
                    'disableTimeRanges': this.DisabledSlot,
                    'minTime': minTime,
                    'maxTime': maxTime,
                    'step': this.Step,
                    'timeFormat': 'h:i A'
                });
            } else {
                $("#basicExample").timepicker({
                    'minTime': minTime,
                    'maxTime': maxTime,
                    'step': this.Step,
                    'timeFormat': 'h:i A'
                });
            }
            if ($("#basicExample").val() != null && $("#basicExample").val() != "")
                this.AppointmentModel.AppointmentTime = $("#basicExample").val();
        }
        else {
            this.toastr.Error("please select doctor.")
        }
    }

    formatAMPM(time: any) {
        var hours = time.split(':')[0];
        var minutes = time.split(':')[1];
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        //minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }
    DoctorChange() {
        this.AppointmentModel.AppointmentTime = null;
    }
    FindDisabledSlot(Appobj: any) {
        this.DisabledSlot = "";
        Appobj.forEach(x => {
            var stime = this.calculateTimeFormateDisabled(x.StartDate.split('T')[1]);
            var etime = this.calculateTimeFormateDisabled(x.EndDate.split('T')[1]);
            this.DisabledSlot += stime + ',' + etime + ',';
        });
        var time_arr = this.DisabledSlot.split(","), new_arr = [];
        time_arr.forEach(function (v, k, arr) {
            if (k % 2 & 1) new_arr.push([arr[k - 1], arr[k]]);  // check for odd keys
        })
        this.DisabledSlot = new_arr;
    }
    calculateTimeFormateDisabled(time: any) {
        let hours = time.split(":")[0];
        let minutes = time.split(":")[1];
        let ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12;
        minutes = minutes.toString().padStart(2, '0');
        let strTime = hours + ':' + minutes + '' + ampm;
        return strTime;
    }
    LoadPatient() {
        //Search By Name
        $('#PatientSearchByName').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.searchByName(request.term).then(m => {
                    this.patientInfo = m.ResultSet.PatientInfo;
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.AppointmentModel.PatientName = ui.item.label;
                this.AppointmentModel.PatientId = ui.item.value;
                this.AppointmentModel.CNIC = this.patientInfo.filter(a => a.value == ui.item.value)[0].CNIC;
                this.AppointmentModel.Mobile = this.patientInfo.filter(a => a.value == ui.item.value)[0].Phone;
            }
        });
    }
    LoadService() {
        $("#ServiceSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._BillingService.ServiceSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.AppointmentModel.ServiceId = ui.item.value;
                this.AppointmentModel.ServiceName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    onPatientNameEvent(event: any) {
        if (event != null && event != undefined) {
            //var patientList = this.PatientList.filter(a => a.PatientName == event);
            //if (patientList.length == 0 || patientList.length == null) {
            //    swal({
            //        title: "Are you sure?",
            //        text: "Are you sure to add new patient.",
            //        icon: "warning",
            //        buttons: ['Cancel', 'Yes'],
            //        dangerMode: true,
            //    })
            //        .then((willDelete) => {
            //            if (willDelete) {
            //                this.loader.ShowLoader();
            //                this._AppointmentService.GetEMRNO().then(m => {
            //                    this.model.MRNO = m.ResultSet.MRNO;
            //                    this.model.PatientName = event;
            //                    this.modalService.open(this.PatientContent, { size: 'lg' });
            //                    this.loader.HideLoader();
            //                });
            //            }
            //        });
            //}

        }
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.GetPreviousAppList();
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

    GetPreviousAppList() {
        this.PreviousGridList();
    }
    PreviousGridList() {
        this.PModel.VisibleColumnInfo = "StartDate#StartDate,Doctor#Doctor,Note#Note,Status#Status,BillingStatus#BillingStatus";
        this.loader.ShowLoader();
        this.Id = "0";
        this._AppointmentService
            .GetPreviousList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PatientId).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.PreviousAppList = m.DataList;
                this.FutureAppList = m.OtherDataModel;
              
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
                this.GetPreviousAppList();
            });
        }
    }
}
