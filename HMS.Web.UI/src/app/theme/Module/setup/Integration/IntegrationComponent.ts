import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { IntegrationService } from './IntegrationService';
import { ZKTService } from '../Employee/ZKT/ZKTService';
import { pr_attendance } from '../Employee/ZKT/ZKTModel';
import { IntegrationModel } from './IntegrationModel';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    moduleId: module.id,
    templateUrl: 'IntegrationComponent.html',
    providers: [IntegrationService, ZKTService],
})
export class IntegrationComponent {
    public ActiveToggle: boolean = false;
    public isCollapsed = true;
    public Id: string;
    public Form1: FormGroup;
    public Form2: FormGroup;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public IsList: boolean = true;
    public ID: number = 10;
    public Keywords: any[] = [];
    public submitted: boolean;
    @ViewChild("adEmail") adEmail: TemplateRef<any>;
    @ViewChild("adSMS") adSMS: TemplateRef<any>;
    public model = new IntegrationModel();
    public AttendanceModel = new pr_attendance();
    public Rights: any;
    public IsEdit: boolean = false;
    public ControlRights: any;
    public valueForUser: any;
    public ZKTList: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    SMSModel: any = {};
    EmailModel: any = {};
    @ViewChild("ZKTModal") ModelContent: TemplateRef<any>;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _fb: FormBuilder, public loader: LoaderService, public _IntegrationService: IntegrationService
        , public _router: Router, public _ZKTService: ZKTService, public toastr: CommonToastrService, private modalService: NgbModal) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("72");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form1 = this._fb.group({
            UserName: [''],
            EmailFrom: [''],
            Password: [''],
            Masking: [''],
            SMTP: [''],
            PortNo: [''],
            IsActive: [''],
            Type: ['']
        });
        this.Form2 = this._fb.group({
            IPAddress: ['', [Validators.required]],
            PortNo: ['', [Validators.required]],
            LastDataSync: [''],
            Password: [''],
            LocationCode: ['', [Validators.required]],
            DeviceSerialNo: [''],
            IsActive: [''],
        });
        this.Refresh();
    }
    Refresh() {
        this.loader.ShowLoader();
        this._IntegrationService.FormLoad().then(m => {
            if (m.IsSuccess) {
                const integrations = m.ResultSet.Integrationlist;
                this.SMSModel = integrations.find((x: any) => x.Type === 'T') || {
                    ID: 0,
                    UserName: '',
                    Password: '',
                    Masking: '',
                    Type: 'T',
                    IsActive: false
                };
                this.EmailModel = integrations.find((x: any) => x.Type === 'E') || {
                    ID: 0,
                    UserName: '',
                    EmailFrom: '',
                    Password: '',
                    SMTP: '',
                    PortNo: '',
                    Type: 'E',
                    IsActive: false
                };
            }
            this.loader.HideLoader();
        });
    }
    openSMSModel(adSMS: any) {
        this.model = {} as IntegrationModel;
        this.model = this.SMSModel;
        this.modalService.open(adSMS, { size: 'lg' });
    }
    openEmailModel(adEmail: any) {
        this.model = {} as IntegrationModel;
        this.model = this.EmailModel;
        this.modalService.open(adEmail, { size: 'lg' });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._IntegrationService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.modalService.dismissAll(this.adSMS);
                    this.modalService.dismissAll(this.adEmail);
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
    SmsActivate() {
        this.model = this.SMSModel;
        this.model.IsActive = true;
        this.SaveOrUpdate(true);
    }
    EmailActivate() {
        this.model = this.EmailModel;
        this.model.IsActive = true;
        this.SaveOrUpdate(true);
    }
    SmsDectivate() {
        this.model = this.SMSModel;
        this.model.IsActive = false;
        this.SaveOrUpdate(true);
    }
    EmailDectivate() {
        this.model = this.EmailModel;
        this.model.IsActive = false;
        this.SaveOrUpdate(true);
    }
    AddRecord(id: string) {
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
            this._IntegrationService.Delete(id).then(m => {

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


    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.ZKTRefresh();
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
    SaveOrUpdateZKT(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._ZKTService.SaveOrUpdate(this.AttendanceModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll(this.ModelContent);
                    this.GetZKTList();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    GetZKTList() {
        this.ZKTRefresh();
    }
    ZKTRefresh() {
        this.PModel.VisibleColumnInfo = "IPAddress#IPAddress,PortNo#PortNo,LastDataSync#LastDataSync,LocationCode#LocationCode,IsActive#IsActive";

        this._ZKTService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                debugger
                this.PModel.TotalRecord = m.TotalRecord;
                this.ZKTList = m.DataList;
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    AddZKTRecord(id: string) {
        this.IsEdit = false;
        if (id != "0") {
            this.loader.ShowLoader();
            this._ZKTService.GetById(id).then(m => {
                if (m.ResultSet != null) {
                    this.IsEdit = true;
                    this.AttendanceModel = m.ResultSet.Result;
                    this.modalService.open(this.ModelContent, { size: 'md' });
                }
                this.loader.HideLoader();
            });
        }
        else {
            this.loader.ShowLoader();
            this.AttendanceModel = new pr_attendance();
            const date = new Date();
            const firstDay = new Date(Date.UTC(date.getFullYear(), date.getMonth(), 1));
            const month = (firstDay.getMonth() + 1).toString().padStart(2, '0');
            const day = firstDay.getDate().toString().padStart(2, '0');
            const year = firstDay.getFullYear();
            this.AttendanceModel.LastDataSync = `${month}/${day}/${year}`;
            this.AttendanceModel.DeviceSerialNo = "0";
            this.modalService.open(this.ModelContent, { size: 'md' });
            this.loader.HideLoader();
        }
    }
}