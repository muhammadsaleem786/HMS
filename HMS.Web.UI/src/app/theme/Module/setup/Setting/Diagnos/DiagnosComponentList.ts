import { Component, OnInit, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DiagnosService } from './DiagnosService';
import { emr_diagnos } from './DiagnosModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    moduleId: module.id,
    templateUrl: 'DiagnosComponentList.html',
    providers: [DiagnosService],
})
export class DiagnosComponentList {
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public DiagnosList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public Form1: FormGroup;
    public model = new emr_diagnos();
    public ControlRights: any;
    public submitted: boolean; public IsEdit: boolean = false;
    @ViewChild("DiagnosModal") ModelContent: TemplateRef<any>;
    constructor(public _fb: FormBuilder, private encrypt: EncryptionService, public _CommonService: CommonService, private modalService: NgbModal, public loader: LoaderService, public _DiagnosService: DiagnosService
        , public _router: Router, public toastr: CommonToastrService,) {
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("7");
        this.loader.ShowLoader();
        //this.Keywords = this._CommonService.GetKeywords("Diagnos");
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            Diagnos: ['', [Validators.required]],
        });
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Diagnos';
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "Diagnos", Title: "Diagnos", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();

        this.Id = "0";
        this._DiagnosService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.DiagnosList = m.DataList;
                this.loader.HideLoader();
            });
    }
    AddRecord(id: string) {
        this.IsEdit = false;
        if (id != "0") {
            this.IsEdit = true;
            this.loader.ShowLoader();
            this._DiagnosService.GetById(id).then(m => {
                if (m.ResultSet != null) {
                    this.model = m.ResultSet.Result;
                    this.modalService.open(this.ModelContent, { size: 'md' });
                }
                this.loader.HideLoader();
            });
        }
        else {
            this.loader.ShowLoader();
            this.model = new emr_diagnos();
            this.modalService.open(this.ModelContent, { size: 'md' });
            this.loader.HideLoader();
        }
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
            this._DiagnosService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                else {
                    this.modalService.dismissAll(this.ModelContent);
                    this.Refresh();
                }
                this.loader.HideLoader();

            });
        }
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._DiagnosService.SaveOrUpdate(this.model).then(m => {
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
        this._DiagnosService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
}