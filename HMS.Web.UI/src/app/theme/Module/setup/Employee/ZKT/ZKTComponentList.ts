import { Component, OnInit, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ZKTService } from './ZKTService';
import { pr_attendance } from './ZKTModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';

@Component({
    moduleId: module.id,
    templateUrl: 'ZKTComponentList.html',
    providers: [ZKTService],
})
export class ZKTComponentList {
    public ActiveToggle: boolean = false;
    public Id: string; public submitted: boolean;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public IsList: boolean = true;
    public ID: number = 10;
    public AttendanceModel = new pr_attendance();
    public Rights: any;
    public ZKTList: any[] = [];
    public Keywords: any[] = [];
    public Form2: FormGroup;
    public IsEdit: boolean = false;
    public ControlRights: any;
    @ViewChild("ZKTModal") ModelContent: TemplateRef<any>;
    constructor(public _fb: FormBuilder, private encrypt: EncryptionService, private modalService: NgbModal, public _CommonService: CommonService, public loader: LoaderService, public _ZKTService: ZKTService, public _router: Router, public toastr: CommonToastrService,) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("5");
    }
    ngOnInit() {
        this.Form2 = this._fb.group({
            IPAddress: ['', [Validators.required]],
            PortNo: ['', [Validators.required]],
            LastDataSync: [''],
            Password: [''],
            LocationCode: ['', [Validators.required]],
            DeviceSerialNo: [''],
            IsActive: [''],
        });
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Attendance';
        this.PConfig.Action.Add = false;
        this.PConfig.Fields = [
            { Name: "IPAddress", Title: "IP Address", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "PortNo", Title: "Port No", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "LastDataSync", Title: "Last Data Sync", OrderNo: 3, SortingAllow: true, Visible: true, isDate: true, DateFormat: "" },
            { Name: "IsActive", Title: "Active", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        this.PModel.VisibleColumnInfo = "IPAddress#IPAddress,PortNo#PortNo,LastDataSync#LastDataSync,LocationCode#LocationCode,IsActive#IsActive";
        this._ZKTService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.ZKTList = m.DataList;
                this.loader.HideLoader();
            });
    }

    AddRecord(id: string) {
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
            this.modalService.open(this.ModelContent, { size: 'md' });
            this.loader.HideLoader();
        }
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    SaveOrUpdate(isValid: boolean): void {
        debugger
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._ZKTService.SaveOrUpdate(this.AttendanceModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll(this.ModelContent);
                    this.GetList();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }

    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._ZKTService.Delete(this.AttendanceModel.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else {
                    this.modalService.dismissAll(this.ModelContent);
                    this.GetList();
                }
                this.loader.HideLoader();
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
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._ZKTService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
}