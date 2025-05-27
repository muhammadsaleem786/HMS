import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, ViewChild, Output, EventEmitter, ViewContainerRef, Compiler, NgModule, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { InvoiceService } from './InvoiceService';
import { CommonModule } from '@angular/common';
import { IMyDateModel } from 'mydatepicker';
import { InvoiceModel, pur_invoice_dt } from './InvoiceModel';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { PaymentModel } from '../Payment/PaymentModel';
import { PaymentService } from '../Payment/PaymentService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
@Component({
    moduleId: module.id,
    templateUrl: 'InvoiceComponentList.html',
    providers: [InvoiceService, PaymentService],
})
export class InvoiceComponentList {
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public InvoiveList: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public ControlRights: any;
    public PaymentControlRights: any;
    public valueForUser: any; public Form1: FormGroup;
    public paymentModel = new PaymentModel();
    public submitted: boolean;
    public PaymentMethodList: any[] = [];
    public selectedValue: string = "A";
    @ViewChild("content") PaymentContent: TemplateRef<any>;
    AddNewPayment(content, invoiceid: any) {
        this.loader.ShowLoader();
        this._PaymentService.FormLoad().then(m => {
            this.paymentModel = new PaymentModel();
            const objInvoice = this.InvoiveList.filter(a => a.ID == invoiceid);
            this.paymentModel.Amount = objInvoice[0].DueBalance;
            this.paymentModel.PaymentDate = new Date();
            this.paymentModel.InvoiveId = invoiceid;
            this.PaymentMethodList = m.ResultSet.list;
            this.paymentModel.PaymentMethodID = 2;//id 2 for cash
            this.modalService.open(this.PaymentContent, { size: 'lg', backdrop: 'static' });
            this.loader.HideLoader();
        });
    }
    constructor(public _CommonService: CommonService, public loader: LoaderService, private encrypt: EncryptionService,
        public _router: Router, public _InvoiceService: InvoiceService, public _fb: FormBuilder, public _PaymentService: PaymentService,
        private compiler: Compiler, private modalService: NgbModal, public _AsidebarService: AsidebarService, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("62");
        this.PaymentControlRights = this._CommonService.ScreenRights("64");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form1 = this._fb.group({
            PaymentDate: ['', [Validators.required]],
            PaymentMethodID: ['', [Validators.required]],
            Amount: ['', [Validators.required]],
            Notes: ['', [Validators.required]],
            InvoiveId: [''],
        });
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._InvoiceService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Invoice/saveinvoice']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Invoice/saveinvoice'], { queryParams: { id: this.Id } });
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
            this._InvoiceService.Delete(id).then(m => {
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
        this._InvoiceService
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
    //for payment
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true        
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._PaymentService.SaveOrUpdate(this.paymentModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.loader.HideLoader();
                    this.modalService.dismissAll(this.PaymentContent);
                    this.Refresh();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
}