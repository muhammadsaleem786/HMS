import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BillingService } from './BillingService';
import { emr_patient_bill } from './BillingModel';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo, InvoiceCompanyInfo } from './../Appointment/AppointmentModel';
@Component({
    moduleId: module.id,
    templateUrl: 'BillingComponentList.html',
    providers: [BillingService],
})
export class BillingComponentList {
    public ActiveToggle: boolean = false;
    public isCollapsed = true;
    public Id: string;
    public Form1: FormGroup;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public UserRoleList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public InvoiceBillModel: any[] = [];
    public PaymentRptData: any[] = [];
    public AdmitData: any[] = [];
    public TotalBill: number = 0;
    public AdvanceAmount: number = 0;
    public Keywords: any[] = [];
    public BillList: any[] = [];
    public SubTotal: number = 0;
    public submitted: boolean;
    public Total: number = 0;
    public TotalDiscount: number = 0;
    @ViewChild("BillContent") BillContent: TemplateRef<any>;
    @ViewChild("PaymentRptModal") PaymentRptModal: TemplateRef<any>;
    @ViewChild("RefundBill") RefundBill: TemplateRef<any>;
    @ViewChild("adBill") adBill: TemplateRef<any>;
    public InvoiceCompanyInfo = new InvoiceCompanyInfo();
    public model = new emr_patient_bill();
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public Rights: any;
    public ControlRights: any;
    public valueForUser: any;
    openInvoiceModal(InvoiceModal: any, id: any) {
        this.loader.ShowLoader();
        this._BillingService.GetBillByPatient(id).then(m => {
            if (m.IsSuccess) {
                this.InvoiceBillModel = [];
                this.SubTotal = 0;
                this.Total = 0;
                this.TotalDiscount = 0
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
                    this.InvoiceCompanyInfo.DoctorName = m.ResultSet.result[0].ID;
                    this.InvoiceCompanyInfo.Room = m.ResultSet.result[0].Room;
                    this.InvoiceCompanyInfo.Ward = m.ResultSet.result[0].Ward;
                    this.InvoiceCompanyInfo.Amount = m.ResultSet.result[0].Amount;
                    this.InvoiceCompanyInfo.AdmissionDate = m.ResultSet.result[0].AdmissionDate;
                    this.InvoiceCompanyInfo.MRNo = m.ResultSet.result[0].MRNo;
                    if (m.ResultSet.result[0].CompanyLogo != null)
                        this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.result[0].CompanyLogo;
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
    openinvoiceBillModal(DetailInvocie: any, AdmissionId: any, AppointmentId: any, PatientId: any) {
        this.loader.ShowLoader();
        this._BillingService.GetBillByPayment(AdmissionId, AppointmentId, PatientId).then(m => {
            if (m.IsSuccess) {
                this.InvoiceBillModel = [];
                this.AdmitData = [];
                this.TotalBill = 0;
                this.AdvanceAmount = 0;
                this.InvoiceBillModel = m.ResultSet.visitdata;
                this.AdmitData = m.ResultSet.admitdata;
                this.TotalBill = m.ResultSet.TotalBill;
                this.AdvanceAmount = m.ResultSet.AdvanceAmt;
                if (m.ResultSet.masterdata != null) {
                    this.InvoiceCompanyInfo.CompanyName = m.ResultSet.masterdata[0].CompanyName;
                    this.InvoiceCompanyInfo.CompanyAddress = m.ResultSet.masterdata[0].CompanyAddress1;
                    this.InvoiceCompanyInfo.CompanyPhone = m.ResultSet.masterdata[0].Phone;
                    this.InvoiceCompanyInfo.CompanyEmail = m.ResultSet.masterdata[0].Email;
                    this.InvoiceCompanyInfo.PatientName = m.ResultSet.masterdata[0].PatientName;
                    this.InvoiceCompanyInfo.PatientAddress = m.ResultSet.masterdata[0].Address;
                    this.InvoiceCompanyInfo.PatientEmail = m.ResultSet.masterdata[0].PatientEmail;
                    this.InvoiceCompanyInfo.PatientMobile = m.ResultSet.masterdata[0].Mobile;
                    this.InvoiceCompanyInfo.MRNo = m.ResultSet.masterdata[0].MRNO;
                    this.InvoiceCompanyInfo.invoiceNo = m.ResultSet.masterdata[0].ID;

                    this.InvoiceCompanyInfo.DoctorName = m.ResultSet.masterdata[0].Name;

                    this.InvoiceCompanyInfo.AdmissionDate = m.ResultSet.masterdata[0].AdmissionDate;
                    this.InvoiceCompanyInfo.DischargeDate = m.ResultSet.masterdata[0].DischargeDate;
                    this.InvoiceCompanyInfo.Father_Husband = m.ResultSet.masterdata[0].Father_Husband;


                    this.InvoiceCompanyInfo.Room = m.ResultSet.masterdata[0].Room;
                    this.InvoiceCompanyInfo.Ward = m.ResultSet.masterdata[0].Ward;
                    if (m.ResultSet.masterdata[0].CompanyLogo != null)
                        this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.masterdata[0].CompanyLogo;
                    //m.ResultSet.result.forEach((item, index) => {
                    //    this.SubTotal += parseInt(item.Price);
                    //    this.TotalDiscount += parseInt(item.Discount);
                    //});
                    //this.Total = this.SubTotal - this.TotalDiscount;


                }
                this.modalService.open(DetailInvocie, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    openPaymentModal(PaymentModal: any, id: any) {
        this.loader.ShowLoader();
        this._BillingService.GetPaymentByPatient(id).then(m => {
            if (m.IsSuccess) {
                this.InvoiceBillModel = [];
                this.SubTotal = 0;
                this.Total = 0;
                this.TotalDiscount = 0
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
                    this.InvoiceCompanyInfo.Amount = m.ResultSet.result[0].Amount;
                    this.InvoiceCompanyInfo.DoctorName = m.ResultSet.result[0].DoctorName;
                    if (m.ResultSet.result[0].CompanyLogo != null)
                        this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.result[0].CompanyLogo;
                }
                this.modalService.open(PaymentModal, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    PaymentRptModel(PaymentRptModal: any, id: any,paymentId:any) {
        this.loader.ShowLoader();
        this._BillingService.GetBillByPatient(id).then(m => {
            if (m.IsSuccess) {
                this.PaymentRptData = [];
                this.SubTotal = 0;
                this.Total = 0;
                this.PaymentRptData = m.ResultSet.result[0].patient_bill_payment[0].filter(x => x.ID == paymentId);
                if (m.ResultSet.result != null) {
                    this.InvoiceCompanyInfo.CompanyName = m.ResultSet.result[0].CompanyName;
                    this.InvoiceCompanyInfo.CompanyAddress = m.ResultSet.result[0].CompanyAddress;
                    this.InvoiceCompanyInfo.CompanyPhone = m.ResultSet.result[0].CompanyPhone;
                    this.InvoiceCompanyInfo.CompanyEmail = m.ResultSet.result[0].CompanyEmail;
                    this.InvoiceCompanyInfo.PatientName = m.ResultSet.result[0].PatientName;
                    this.InvoiceCompanyInfo.PatientAddress = m.ResultSet.result[0].PatientAddress;
                    this.InvoiceCompanyInfo.PatientEmail = m.ResultSet.result[0].PatientEmail;
                    this.InvoiceCompanyInfo.PatientMobile = m.ResultSet.result[0].PatientMobile;
                    this.InvoiceCompanyInfo.BillDate = this.PaymentRptData[0].PaymentDate;
                    this.InvoiceCompanyInfo.invoiceNo = this.PaymentRptData[0].ID;
                    this.InvoiceCompanyInfo.DoctorName = m.ResultSet.result[0].ID;
                    this.InvoiceCompanyInfo.Room = m.ResultSet.result[0].Room;
                    this.InvoiceCompanyInfo.Ward = m.ResultSet.result[0].Ward;
                    this.InvoiceCompanyInfo.Amount = m.ResultSet.result[0].Amount;
                    this.InvoiceCompanyInfo.AdmissionDate = m.ResultSet.result[0].AdmissionDate;
                    this.InvoiceCompanyInfo.MRNo = m.ResultSet.result[0].MRNo;
                    if (m.ResultSet.result[0].CompanyLogo != null)
                        this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.result[0].CompanyLogo;
                    this.PaymentRptData.forEach((item, index) => {
                        this.SubTotal += parseInt(item.Amount);
                    });
                    this.Total = this.SubTotal;
                }
                this.modalService.open(PaymentRptModal, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _fb: FormBuilder, public loader: LoaderService, public _BillingService: BillingService
        , public _router: Router, public toastr: CommonToastrService, private modalService: NgbModal) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("12");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            AppointmentId: [''],
            PatientId: [''],
            ServiceId: [''],
            BillDate: [''],
            Price: [''],
            Discount: [''],
            PaidAmount: [''],
            PatientName: [''],
            DoctorName: [''],
            ServiceName: [''],
            DoctorId: [''],
            OutstandingBalance: [''],
            Remarks: [''],
            PartialAmount: [''],
            RefundAmount: [''],
            RefundDate: [''],
            PaymentDate:[''],
        });
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
    }
    Refresh() {
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "BillNO#BillNO,Patient#Patient,Doctor#Doctor,Date#Date,Amount#Amount,AdmissionNo#AdmissionNo";
        this._BillingService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.BillList = m.DataList;
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    openAddBill(adBill: any, id: any) {
        this.loader.ShowLoader();
        this._BillingService.GetById(id).then(m => {
            this.modalService.open(adBill, { size: 'md' });
            this.model = m.ResultSet.result;
            this.model.PaymentDate = new Date();
            this.loader.HideLoader();
        });
    }
    openRefundBill(RefundBill: any, id: any) {
        this.loader.ShowLoader();
        this._BillingService.GetById(id).then(m => {
            this.modalService.open(RefundBill, { size: 'sm' });
            this.model = m.ResultSet.result;
            this.model.RefundDate = new Date();
            this.loader.HideLoader();
        });
    }

    PaidCalAmount() {
        if (this.model.Discount == undefined || this.model.Discount == null)
            this.model.Discount = 0;
        if (this.model.Price == undefined || this.model.Price == null)
            this.model.Price = 0;
        this.model.OutstandingBalance = this.model.Price - this.model.Discount - (this.model.PaidAmount + parseInt(this.model.PartialAmount));
    }
    RefundCalAmount() {
        this.model.OutstandingBalance = this.model.Price - this.model.Discount - this.model.PaidAmount + parseInt(this.model.RefundAmount);
    }
    RefundSaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (this.model.RefundAmount == undefined) {
            this.toastr.Error("Please enter refund amount.");
            return;
        }
        this.model.PaidAmount = this.model.PaidAmount - parseInt(this.model.RefundAmount);
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._BillingService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.modalService.dismissAll(this.adBill);
                    this.toastr.Success(result.Message);
                    this.Refresh();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (this.model.PartialAmount == undefined) {
            this.toastr.Error("Please enter amount.");
            return;
        }
        this.model.PaidAmount = this.model.PaidAmount + parseInt(this.model.PartialAmount);

        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();

            this._BillingService.SaveOrUpdateBill(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.modalService.dismissAll(this.adBill);
                    this.toastr.Success(result.Message);
                    this.Refresh();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.Refresh();
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

    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Billing/savebill']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Billing/savebill'], { queryParams: { id: this.Id } });
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {

        var result = this.toastr.DeleteAlert(); //confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._BillingService.Delete(id).then(m => {

                if (m.ErrorMessage != null) {

                    alert(m.ErrorMessage);
                }
                this.Refresh();
            });
        }
    }
    GetList() {
        this.Refresh();
    }
    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else
            this.Refresh();
    }

}