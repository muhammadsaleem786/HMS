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
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    templateUrl: './AppointComponentList.html',
    moduleId: module.id,
    providers: [AppointmentService],
})

export class AppointComponentList implements OnInit {
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
    totalPages: number = 0; public CompanyObj: any;
    pagesRange: number[] = [];
    public Rights: any;
    public ControlRights: any;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
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
                this.FutureAppList = m.OtherDataModel;
                this.PreviousAppList = m.DataList; this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
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
