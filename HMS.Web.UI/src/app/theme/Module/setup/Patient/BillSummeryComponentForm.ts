import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo, InvoiceCompanyInfo } from './../Appointment/AppointmentModel';
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
import { BillingService } from './../Billing/BillingService';
declare var $: any;
import { PatientService } from './../Patient/PatientService';
@Component({
    templateUrl: './BillSummeryComponentForm.html',
    moduleId: module.id,
    providers: [AppointmentService, BillingService, PatientService],
})

export class BillSummeryComponentForm implements OnInit {
    public Form4: FormGroup;
    public Form9: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = []; public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new emr_patient();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public BillModel = new emr_patient_bill();
    public BillingList: any[] = [];
    public PatientId: any;
    public ClinicName: string = ""; previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0; public InvoiceBillModel: any[] = [];
    pagesRange: number[] = []; public InvoiceCompanyInfo = new InvoiceCompanyInfo();
    public SubTotal: number = 0;
    public Total: number = 0; public CompanyObj: any;
    public TotalDiscount: number = 0; @ViewChild("BillContent") BillContent: TemplateRef<any>;
    @ViewChild("InvoiceModal") InvModal: TemplateRef<any>;
    public Rights: any;
    public ControlRights: any; public valueForUser: any;
    openBillModal(BillContent) {
        let id = parseInt(localStorage.getItem('PatientId'));
        this._BillingService.GetPatientById(id).then(m => {
            if (m.IsSuccess) {
                if (m.ResultSet != null) {
                    this.BillModel = new emr_patient_bill();
                    this.BillModel.BillDate = new Date();
                    this.BillModel.PatientId = parseInt(localStorage.getItem('PatientId'));
                    this.BillModel.PatientName = m.ResultSet.PatientName;
                }
                this.modalService.open(BillContent, { size: 'md' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });

       
       
    }
    openInvoiceModal(InvoiceModal: any, id: any) {
        this.loader.ShowLoader();
        this._BillingService.GetBillByPatient(id).then(m => {
            if (m.IsSuccess) {
                this.InvoiceBillModel = m.ResultSet.result;
                if (m.ResultSet.result != null) {
                    this.InvoiceCompanyInfo.CompanyName = m.ResultSet.result[0].CompanyName;
                    this.InvoiceCompanyInfo.CompanyAddress = m.ResultSet.result[0].CompanyAddress;
                    this.InvoiceCompanyInfo.CompanyPhone = m.ResultSet.result[0].CompanyPhone;
                    this.InvoiceCompanyInfo.CompanyEmail = m.ResultSet.result[0].CompanyEmail;

                    this.InvoiceCompanyInfo.PatientName = m.ResultSet.result[0].PatientName;
                    this.InvoiceCompanyInfo.PatientAddress = m.ResultSet.result[0].PatientAddress;
                    this.InvoiceCompanyInfo.PatientEmail = m.ResultSet.result[0].PatientEmail;
                    this.InvoiceCompanyInfo.PatientMobile = m.ResultSet.result[0].PatientMobile;
                    this.InvoiceCompanyInfo.BillDate = m.ResultSet.result[0].BillDate;
                    this.InvoiceCompanyInfo.invoiceNo = m.ResultSet.result[0].ID;
                    if (m.ResultSet.result[0].CompanyLogo != null)
                        this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.result[0].CompanyLogo;
                    this.Total = 0; this.SubTotal = 0; this.TotalDiscount = 0
                    m.ResultSet.result.forEach((item, index) => {
                        this.SubTotal += parseInt(item.Price);
                        this.TotalDiscount += parseInt(item.Discount);
                    });
                    this.Total = this.SubTotal - this.TotalDiscount;


                }
                this.modalService.open(InvoiceModal, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _PatientService: PatientService, public _BillingService: BillingService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("12");        
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));

    }
    ngOnInit() {
        this.PatientId = localStorage.getItem('PatientId');
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form9 = this._fb.group({
            AppointmentId: [''],
            PatientId: ['', [Validators.required]],
            ServiceId: ['', [Validators.required]],
            BillDate: ['', [Validators.required]],
            Price: ['', [Validators.required]],
            Discount: [''],
            PaidAmount: ['', [Validators.required]],
            PatientName: [''],
            DoctorName: [''],
            ServiceName: [''],
            DoctorId: ['', [Validators.required]],
            OutstandingBalance: [''],
            Remarks: [''],
        });
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.GetBillList();
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
  
    GetBillList() {
        this.BillingGridList();
    }
    BillingGridList() {    
        this.PModel.VisibleColumnInfo = "BillNO#BillNO,Patient#Patient,Doctor#Doctor,Date#Date,Amount#Amount";
            this.loader.ShowLoader();
        this.Id = "0";
        this._AppointmentService
            .GetBillingList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PatientId).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.BillingList = m.DataList;
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    UpdatebillRecord(id: any) {
        if (id > 0) {
            this.loader.ShowLoader();
            this.IsEdit = true;
            this._BillingService.GetById(id).then(m => {
                this.BillModel = m.ResultSet.result;
                this.modalService.open(this.BillContent, { size: 'md' });
                this.loader.HideLoader();
            });
        } else {
            this.BillModel.BillDate = new Date();
        }
    }
    SaveOrUpdateBill(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._AppointmentService.SaveOrUpdatebill(this.BillModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.modalService.dismissAll(this.BillContent);
                    this.BillingGridList();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    LoadBillDoctor() {
        $("#BillDoctorSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._PatientService.DoctorSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.BillModel.DoctorId = ui.item.value;
                this.BillModel.DoctorName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    LoadPatient() {
        $("#PatientSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._BillingService.PatientSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.BillModel.PatientId = ui.item.value;
                this.BillModel.PatientName = ui.item.label;

                return ui.item.label;
            }
        });
    }
    LoadBillService() {
        $("#BillServiceSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._BillingService.ServiceSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.BillModel.ServiceId = ui.item.value;
                this.BillModel.ServiceName = ui.item.label;
                this.BillModel.Price = ui.item.Price;
                this.BillCalAmount();
                return ui.item.label;
            }
        });
    }
    BillCalAmount() {
        if (this.BillModel.Discount == undefined || this.BillModel.Discount == null)
            this.BillModel.Discount = 0;
        if (this.BillModel.Price == undefined || this.BillModel.Price == null)
            this.BillModel.Price = 0;
        this.BillModel.PaidAmount = this.BillModel.Price - this.BillModel.Discount;
        this.BillModel.OutstandingBalance = this.BillModel.Price - this.BillModel.Discount - this.BillModel.PaidAmount;
    }
    PaidCalAmount() {
        if (this.BillModel.Discount == undefined || this.BillModel.Discount == null)
            this.BillModel.Discount = 0;
        if (this.BillModel.Price == undefined || this.BillModel.Price == null)
            this.BillModel.Price = 0;
        this.BillModel.OutstandingBalance = this.BillModel.Price - this.BillModel.Discount - this.BillModel.PaidAmount;
    }
    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._BillingService.Delete(id).then(m => {

                if (m.ErrorMessage != null) {

                    alert(m.ErrorMessage);
                }
                this.BillingGridList();
            });
        }
    }
}
