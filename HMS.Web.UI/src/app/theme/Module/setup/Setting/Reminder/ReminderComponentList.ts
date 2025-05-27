import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, ViewChild, Output, EventEmitter, ViewContainerRef, Compiler, NgModule, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { ReminderService } from './ReminderService';
import { CommonModule } from '@angular/common';
import { IMyDateModel } from 'mydatepicker';
import { ReminderModel, adm_reminder_dt } from './ReminderModel';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
@Component({
    moduleId: module.id,
    templateUrl: 'ReminderComponentList.html',
    providers: [ReminderService],
})
export class ReminderComponentList {
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public ReminderList: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public ControlRights: any;
    public valueForUser: any; public Form1: FormGroup;
    public submitted: boolean;
    constructor(public _CommonService: CommonService, public loader: LoaderService, private encrypt: EncryptionService,
        public _router: Router, public _ReminderService: ReminderService, public _fb: FormBuilder,
        private compiler: Compiler, private modalService: NgbModal, public _AsidebarService: AsidebarService, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("67");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
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
        this._ReminderService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/reminder/savereminder']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/reminder/savereminder'], { queryParams: { id: this.Id } });
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
            this._ReminderService.Delete(id).then(m => {
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
    Refresh() {
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "Name#Name";
        this._ReminderService.GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.VisibleFilter).then(m => {
            this.PModel.TotalRecord = m.TotalRecord;
            this.ReminderList = m.DataList;
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