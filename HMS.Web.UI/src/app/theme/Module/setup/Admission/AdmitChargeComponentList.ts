import { Component, OnInit, EventEmitter,ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { BillingService } from './../Billing/BillingService';
import { PatientAmountSummaryService } from './PatientAmountSummaryService';
import { emr_patient, PatientAmountSummary, ipd_admission_vital, ipd_admission_charges, ipd_payment } from './AdmitModel';
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
    public Form2: FormGroup;
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
    public PatientModel = new emr_patient();
    public paymentmodel = new ipd_payment();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public AdmitId: any;
    public sub: any;
    public IsEdit: boolean = false; public IsVital: boolean = false;
    public CompanyInfo: any[] = [];
    public ClinicName: string;
    public Rights: any;
    public ChargeRights: any;
    public PaymentRights: any;
    public PatientId: any;
    nextPage: number = 1; public CompanyObj: any;
    totalPages: number = 0; public InvoiceBillModel: any[] = [];
    public invoiceModel: any; public DischargeDate: any;
    public ipd_payment_dynamicArray = [];
    public ProcedureItem: any[] = [];
    public ProcedureMedicine: any[] = [];
    public ProcedurePayment: any[] = [];
    public ProcedureOtherCharges: any[] = [];
    public PaymentMethodList: any[] = [];
    public PaymentList: any[] = [];
    pagesRange: number[] = []; previousPage: number = 1; public SubTotal: number = 0;
    public valueForUser: any;
    public Total: number = 0; public TotalDiscount: number = 0;
    @ViewChild("NewPayment") NewPayment: TemplateRef<any>;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService,
        private patientAmountSummaryService:PatientAmountSummaryService
        , public toastr: CommonToastrService, public _BillingService: BillingService, public _CommonService: CommonService, private encrypt: EncryptionService, public route: ActivatedRoute, public _router: Router, public _AdmitService: AdmitService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ChargeRights = this._CommonService.ScreenRights("47");
        this.PaymentRights = this._CommonService.ScreenRights("75");
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
        this.Form2 = this._fb.group({
            AdmissionId: [''],
            PatientId: [''],
            PaymentDate: [''],
            PaymentMethodID: [''],
            Amount: [''],
            Notes: ['']
        });
    }

    GetCharge() {
        this.loader.ShowLoader();
        this._AdmitService.ChargeGetById(this.AdmitId, this.PatientId).then(m => {
            if (m.ResultSet.Result != null)
                this.model = m.ResultSet.Result;
            this.DischargeDate = m.ResultSet.dischargeInfo.DischargeDate
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
                    this.GetCharge();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    get TotalAmount(): number {
        return (this.ProcedurePayment || []).reduce(
            (sum, item) => sum + (Number(item.Amount) || 0),
            0
        );
    }
    openInvoiceModal(InvoiceModal: any) {
        this.loader.ShowLoader();
        let patientid = localStorage.getItem('PatientId');
        let id = localStorage.getItem('AdmitId');
        this._BillingService.GetIPDBillingList(patientid, id).then(m => {
            if (m.IsSuccess) {
                this.invoiceModel = m.ResultSet.result[0][0];
                if (this.invoiceModel == undefined)
                    this.invoiceModel = null;
                this.ProcedureItem = m.ResultSet.result[1];
                this.ProcedureMedicine = m.ResultSet.result[2];
                this.ProcedurePayment = m.ResultSet.result[3];
                if (this.invoiceModel != null && this.invoiceModel.CompanyLogo != null)
                    this.invoiceModel.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + this.invoiceModel.CompanyLogo;

                this.modalService.open(InvoiceModal, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    onChanged(event: any) {

    }
    AddNewPayment(NewPayment) {
        this.loader.ShowLoader();
        this._AdmitService.PaymentFormLoad().then(m => {
            if (m.IsSuccess) {
                this.PaymentMethodList = [];
                this.PaymentMethodList = m.ResultSet.paymentList;
                this.ipd_payment_dynamicArray = [];
                this.paymentmodel = new ipd_payment();
                this.paymentmodel.ID = 0;
                this.modalService.open(NewPayment, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    LoadPayment() {
        this.loader.ShowLoader();
        this._AdmitService.PaymentList(this.AdmitId, this.PatientId).then(m => {
            if (m.ResultSet.result != null) {
                this.PaymentList = [];
                this.PaymentList = m.ResultSet.result;
                this.patientAmountSummaryService.updateAmount({
                    procedureAmount: Number(m.ResultSet.ProcedureAmount || 0),
                    payAmount: Number(m.ResultSet.payAmount || 0),
                    discountAmount: Number(m.ResultSet.discountAmount || 0)
                });
                this.loader.HideLoader();
                this._router.navigate(['home/AdmitSummary/AdmitCharge/AdmitPro']);
            } else
                this.loader.HideLoader();
        });
    }
    Delete(id: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AdmitService.DeletePayment(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.LoadPayment();
                this.loader.HideLoader();
            });
        }
    }
    Edit(id: any) {
        this.ipd_payment_dynamicArray = [];
        this.paymentmodel = new ipd_payment();
        this.loader.ShowLoader();
        this._AdmitService.EditPaymentById(id).then(m => {
            if (m.IsSuccess) {
                this.paymentmodel = m.ResultSet.payment;
                this.PaymentMethodList = m.ResultSet.paymentList;
                this.ipd_payment_dynamicArray.push(m.ResultSet.payment);
                this.modalService.open(this.NewPayment, { size: 'lg' });
            }
            this.loader.HideLoader();
        });
    }
    AddPayment() {
        var obj = new ipd_payment();
        obj.PaymentDate = new Date();
        obj.PaymentMethodID = 2;//id 2 for cash
        this.ipd_payment_dynamicArray.push(obj);
    }
    Remove_payment_row(rowno: any) {
        this.ipd_payment_dynamicArray.splice(rowno, 1);
    }
    PaymentSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.loader.ShowLoader();
            this.submitted = false;
            let paymentArray: ipd_payment[] = [];
            this.ipd_payment_dynamicArray.forEach((item, index) => {
                const newPayment = new ipd_payment();
                if (item.PaymentDate == null || item.PaymentDate == undefined) {
                    isValid = false;
                    this.toastr.Error("Payment date is required at the row no. " + index++);
                    return true;
                } else {
                    newPayment.ID = item.ID;
                    newPayment.CompanyId = item.CompanyId;
                    newPayment.AdmissionId = this.AdmitId;
                    newPayment.PatientId = this.PatientId;
                    newPayment.PaymentMethodID = item.PaymentMethodID;
                    newPayment.PaymentDate = item.PaymentDate ? new Date(item.PaymentDate) : null;
                    newPayment.Amount = item.Amount;
                    newPayment.Notes = item.Notes;
                    paymentArray.push(newPayment);
                }
            })
            this._AdmitService.PaymentSaveOrUpdateList(paymentArray).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.LoadPayment();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
            this.loader.HideLoader();
            this.modalService.dismissAll();
        }
    }
}
