import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, ViewChild, Output, EventEmitter, ViewContainerRef, Compiler, NgModule, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { QuoteInvoiceService } from './QuoteInvoiceService';
import { CommonModule } from '@angular/common';
import { IMyDateModel } from 'mydatepicker';
import { QuoteInvoiceModel, quote_invoice_dt } from './QuoteInvoiceModel';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { InvoiceCompanyInfo } from './../../Appointment/AppointmentModel';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
@Component({
    moduleId: module.id,
    templateUrl: 'QuoteInvoiceComponentList.html',
    providers: [QuoteInvoiceService],
})
export class QuoteInvoiceComponentList {
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public InvoiceCompanyInfo = new InvoiceCompanyInfo();
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public InvoiceBillModel: any[] = [];
    public PayrollRegion: string;
    public InvoiveList: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public SubTotal: number = 0;
    public Total: number = 0;
    public TotalDiscount: number = 0;
    public ControlRights: any;
    public valueForUser: any; public Form1: FormGroup;
    public submitted: boolean;
    public selectedValue: string = "A";
    public CompanyObj: any;
    openInvoiceModal(InvoiceModal: any, id: any) {
        this.loader.ShowLoader();
        this._QuoteInvoiceService.GetBillByPatient(id).then(m => {
            if (m.IsSuccess) {
                this.InvoiceBillModel = [];
                this.SubTotal = 0;
                this.Total = 0;
                this.TotalDiscount = 0
                debugger;
                if (m.ResultSet.result != null) {
                    this.InvoiceCompanyInfo.CompanyName = m.ResultSet.result.adm_company.CompanyName;
                    this.InvoiceCompanyInfo.CompanyAddress = m.ResultSet.result.adm_company.CompanyAddress1;
                    this.InvoiceCompanyInfo.CompanyPhone = m.ResultSet.result.adm_company.Phone;
                    this.InvoiceCompanyInfo.CompanyEmail = m.ResultSet.result.adm_company.Email;
                    this.InvoiceCompanyInfo.PatientName = m.ResultSet.result.emr_patient_mf.PatientName;
                    this.InvoiceCompanyInfo.PatientAddress = m.ResultSet.result.emr_patient_mf.Address;
                    this.InvoiceCompanyInfo.PatientEmail = m.ResultSet.result.emr_patient_mf.Email;
                    this.InvoiceCompanyInfo.PatientMobile = m.ResultSet.result.emr_patient_mf.Mobile;
                    this.InvoiceCompanyInfo.BillDate = m.ResultSet.result.InvoiceDate;
                    this.InvoiceCompanyInfo.invoiceNo = m.ResultSet.result.InvoiceNo;
                    this.InvoiceCompanyInfo.Amount = m.ResultSet.result.Total;
                    if (m.ResultSet.result.adm_company.CompanyLogo != null)
                        this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + m.ResultSet.result.adm_company.CompanyLogo;
                    let itemList = m.ResultSet.itemList;
                    m.ResultSet.result.quote_invoice_dt.forEach((item, index) => {
                        let itemObj = itemList.find(x =>
                            x.ID == (item.ItemTypeId == 1 ? item.ItemID : item.ServiceID)
                        );
                        item.ItemName = itemObj.Name;
                        item.Quantity = item.Quantity;
                        item.Rate = item.Rate;
                        item.Amount = item.Amount;
                        this.InvoiceBillModel.push(item);
                    });
                    this.TotalDiscount = m.ResultSet.result.DiscountAmount;
                    this.SubTotal = m.ResultSet.result.Total;
                    this.Total = this.SubTotal - this.TotalDiscount;
                }
                this.modalService.open(InvoiceModal, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    constructor(public _CommonService: CommonService, public loader: LoaderService, private encrypt: EncryptionService,
        public _router: Router, public _QuoteInvoiceService: QuoteInvoiceService, public _fb: FormBuilder,
        private compiler: Compiler, private modalService: NgbModal, public _AsidebarService: AsidebarService, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("126");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
    }
    ngOnInit() {
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._QuoteInvoiceService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/quoteinv/saveinvoice']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/quoteinv/saveinvoice'], { queryParams: { id: this.Id } });
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._QuoteInvoiceService.Delete(id).then(m => {
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
    getFilterData() {
        if (this.selectedValue == "D")
            this.PModel.VisibleFilter = true;
        else
            this.PModel.VisibleFilter = false;

        this.Refresh();
    }
    Refresh() {
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "date#date,Invoiceno#Invoiceno,name#name,status#status,duedate#duedate,amount#amount";
        this._QuoteInvoiceService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.VisibleFilter).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.InvoiveList = m.DataList;
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
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
   
}