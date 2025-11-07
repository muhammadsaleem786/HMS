import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, ViewChild, Output, EventEmitter, ViewContainerRef, Compiler, NgModule, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { SessionService } from './SessionService';
import { CommonModule } from '@angular/common';
import { IMyDateModel } from 'mydatepicker';
import { SessionModel } from './SessionModel';
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
declare var $: any;
@Component({
    moduleId: module.id,
    templateUrl: 'SessionComponentList.html',
    providers: [SessionService],
})
export class SessionComponentList {
    public model = new SessionModel();
    public Form1: FormGroup;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public SessionList: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public ControlRights: any;
    public valueForUser: any;
    public submitted: boolean;
    @ViewChild("shiftClose") shiftClose: TemplateRef<any>;
    constructor(public _CommonService: CommonService, public loader: LoaderService, private encrypt: EncryptionService,
        public _router: Router, public _SessionService: SessionService, public _fb: FormBuilder,
        private compiler: Compiler, private modalService: NgbModal, public _AsidebarService: AsidebarService, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("76");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            ShiftStartTime: [''],
            StartTime: [''],
            ShiftEndTime: [''],
            StartingCash: [''],
            ClosingBalance: [''],
            OpeningComments: [''],
            ClosingComments: ['']
        });
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);

    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._SessionService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddRecord(id: string) {
        this._SessionService.FormLoad().then(m => {
            if (m.IsSuccess && m.ResultSet != null) {
                this.toastr.Error("Session already open.");
                return;
            } else {
                this.model = new SessionModel();
                this.model.ShiftStartTime = new Date();
                this.SelectTime();
                this.modalService.open(this.shiftClose, { size: 'md' });
            }
        });
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
            this._SessionService.Delete(id).then(m => {
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
        this.PModel.VisibleColumnInfo = "ShiftStartTime#ShiftStartTime,ShiftEndTime#ShiftEndTime";
        this._SessionService.GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.VisibleFilter).then(m => {
            this.PModel.TotalRecord = m.TotalRecord;
            this.SessionList = m.DataList;
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
    ShiftClose(shiftClose: any, id: any) {
        this.loader.ShowLoader();
        this.model = new SessionModel();
        this._SessionService.GetShiftCloseData(id).then(m => {
            if (m.IsSuccess) {
                this.model = new SessionModel();
                this.model = m.ResultSet;
                this.model.ShiftEndTime = new Date();
                this.SelectTime();
            }
            this.modalService.open(shiftClose, { size: 'md' });
            this.loader.HideLoader();
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            if (this.model.ID != 0 && this.model.TotalCollection != this.model.ClosingBalance) {
                this.loader.HideLoader();
                this.toastr.Error("Closing balance must be equal to total revenue.");
                return;
            }
            this._SessionService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this._router.navigate(['/home/session']);
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.Refresh();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }
    SelectTime() {
        let Step = 30;
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        const timeString = this.formatTime(hours, minutes);
        this.model.StartTime = timeString;
        $("#TimeRecorded").timepicker({
            'step': Step,
            'timeFormat': 'h:i A',
            'defaultTime': timeString
        });
        if ($("#TimeRecorded").val() != null && $("#TimeRecorded").val() != "")
            this.model.StartTime = $("#TimeRecorded").val();
    }
    formatTime(hours: number, minutes: number): string {
        const ampm = hours >= 12 ? 'PM' : 'AM';
        const hour = hours % 12 || 12;  // Convert to 12-hour format
        const minute = minutes < 10 ? '0' + minutes : minutes;
        return hour + ':' + minute + ' ' + ampm;
    }
    ChangeSelectTime() {
        let Step = 30;
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        $("#TimeRecorded").timepicker({
            'step': Step,
            'timeFormat': 'h:i A'
        });
        if ($("#TimeRecorded").val() != null && $("#TimeRecorded").val() != "")
            this.model.StartTime = $("#TimeRecorded").val();
        $('#TimeRecorded').timepicker('remove');
    }
}