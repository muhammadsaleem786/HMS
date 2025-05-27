import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_patient, ipd_admission_vital } from './AdmitModel';
import { Observable } from 'rxjs';
import { AdmitService } from './AdmitService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute, NavigationEnd } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
declare var $: any;
@Component({
    templateUrl: './AdmitVitalComponentList.html',
    moduleId: module.id,
    providers: [AdmitService],
})

export class AdmitVitalComponentList implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string; public DocumentImage: string = '';
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = []; public PatientVitalList: any[] = [];
    public IsSameModuleName: string;
    public IsChecked: boolean;
    public IsEmpExist: boolean = false; public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new emr_patient();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public AdmitId: any;
    public PatientId: any;
    public sub: any;
    public IsEdit: boolean = false; public IsVital: boolean = false;
    public CompanyInfo: any[] = [];
    public emr_vital_dynamicArray = [];
    public VitalList: any = [];
    public VitalModel = new ipd_admission_vital();
    public ClinicName: string;
    public Rights: any;
    public VitalRights: any; public Step: number = 30;
    nextPage: number = 1; public CompanyObj: any;
    totalPages: number = 0; public InvoiceBillModel: any[] = [];
    pagesRange: number[] = []; previousPage: number = 1;
    mySubscription;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AdmitService: AdmitService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.VitalRights = this._CommonService.ScreenRights("43");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
        route.params.subscribe(val => {
            this.PModel.SortName = "";
            this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
            this.selectPage(this.PModel.CurrentPage);
        });
    }
    ngOnInit() {
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');

        this.Form1 = this._fb.group({
            AdmissionId: [''],
            DateRecorded: ['', [Validators.required]],
            Temperature: [''],
            Weight: [''],
            Height: [''],
            BP: [''],
            SPO2: [''],
            Measure: [''],
            Measure2: [''],
            HeartRate: [''],
            RespiratoryRate: [''],
            TimeRecorded: ['']

        });
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.GetVitalList();
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
    GetVitalList() {
        this.VitalGridList();
    }
    VitalGridList() {
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this.PModel.VisibleColumnInfo = "DateRecorded#DateRecorded,Temperature#Temperature,Weight#Weight,Height#Height,SBP#SBP";
        this.loader.ShowLoader();
        this.Id = "0";

        this._AdmitService.GetVitalList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.AdmitId, this.PatientId, '0').then(m => {
            this.PModel.TotalRecord = m.TotalRecord;
            this.VitalList = m.DataList;
            this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
            this.loader.HideLoader();
        });
    }
    //Vital 
    VitalSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.VitalModel.AdmissionId = this.AdmitId;
            this.VitalModel.PatientId = this.PatientId;
            this.VitalModel.BP = this.VitalModel.Measure + "/" + this.VitalModel.Measure2;
            this._AdmitService.VitalSaveOrUpdate(this.VitalModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.VitalGridList();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    OpenVitalModal(VitalModal) {
        this.VitalModel = new ipd_admission_vital();
        this.VitalModel.DateRecorded = new Date();
        this.SelectTime();
        this.modalService.open(VitalModal, { size: 'md' });
    }
    Delete(id: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AdmitService.DeleteVital(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.VitalGridList();
                this.loader.HideLoader();
            });
        }
    }
    SelectTime() {
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        const timeString = this.formatTime(hours, minutes);
        this.VitalModel.TimeRecorded = timeString;
        $("#TimeRecorded").timepicker({
            'step': this.Step,
            'timeFormat': 'h:i A',
            'defaultTime': timeString
        });
    }
    formatTime(hours: number, minutes: number): string {
        const ampm = hours >= 12 ? 'PM' : 'AM';
        const hour = hours % 12 || 12;  // Convert to 12-hour format
        const minute = minutes < 10 ? '0' + minutes : minutes;
        return hour + ':' + minute + ' ' + ampm;
    }
}
