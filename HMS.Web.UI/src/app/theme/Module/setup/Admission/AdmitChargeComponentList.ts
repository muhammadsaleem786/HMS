import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { BillingService } from './../Billing/BillingService';

import { emr_patient, ipd_admission_vital, ipd_admission_charges } from './AdmitModel';
import { Observable } from 'rxjs';
import { AdmitService } from './AdmitService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    templateUrl: './AdmitChargeComponentList.html',
    moduleId: module.id,
    providers: [AdmitService, BillingService],
})

export class AdmitChargeComponentList implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string; public DocumentImage: string = '';
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new ipd_admission_charges();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public AdmitId: any;
    public sub: any;
    public IsEdit: boolean = false; public IsVital: boolean = false;
    public CompanyInfo: any[] = [];
    public ClinicName: string;
    public Rights: any;
    public ChargeRights: any;
    public PatientId: any;
    nextPage: number = 1; public CompanyObj: any;
    totalPages: number = 0; public InvoiceBillModel: any[] = [];
    public invoiceModel: any;
    public ProcedureItem: any[] = [];
    public ProcedureMedicine: any[] = [];
    pagesRange: number[] = []; previousPage: number = 1; public SubTotal: number = 0;
    public valueForUser: any;
    public Total: number = 0; public TotalDiscount: number = 0;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, public _BillingService: BillingService, public _CommonService: CommonService, private encrypt: EncryptionService, public route: ActivatedRoute, public _router: Router, public _AdmitService: AdmitService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ChargeRights = this._CommonService.ScreenRights("47");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this.GetCharge();

        this.Form1 = this._fb.group({
            AdmissionId: [''],
            AnnualPE: [''],
            General: [''],
            Medical: [''],
            ICUCharges: [''],
            ExamRoom: [''],
            PrivateWard: [''],
            RIP: [''],
            OtherAllCharges: [''],
        });


    }
    GetCharge() {
        this.loader.ShowLoader();
        this._AdmitService.ChargeGetById(this.AdmitId, this.PatientId).then(m => {
            if (m.ResultSet.Result != null)
                this.model = m.ResultSet.Result;
            this._router.navigate(['home/AdmitSummary/AdmitCharge/AdmitPro']);
            this.loader.HideLoader();
        });
    }
    ChargeSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.model.AdmissionId = this.AdmitId;
            this.model.PatientId = this.PatientId;
            this._AdmitService.ChargeSaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    openInvoiceModal(InvoiceModal: any) {
        this.loader.ShowLoader();
        let patientid = localStorage.getItem('PatientId');
        let id = localStorage.getItem('AdmitId');
        this._BillingService.GetIPDBillingList(patientid, id).then(m => {
            if (m.IsSuccess) {
                this.invoiceModel = m.ResultSet.result[0][0];
                if (this.invoiceModel == undefined)
                    this.invoiceModel = [];
                this.ProcedureItem = m.ResultSet.result[1];
                this.ProcedureMedicine = m.ResultSet.result[2];
                if (this.invoiceModel.length > 0 && this.invoiceModel.CompanyLogo != null)
                    this.invoiceModel.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + this.invoiceModel.CompanyLogo;

                this.modalService.open(InvoiceModal, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }

}
