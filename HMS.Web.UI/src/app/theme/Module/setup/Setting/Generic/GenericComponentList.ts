import { Component, OnInit, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { GenericService } from './GenericService';
import { adm_generic } from './GenericModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
@Component({
    moduleId: module.id,
    templateUrl: 'GenericComponentList.html',
    providers: [GenericService],
})
export class GenericComponentList {
    public ActiveToggle: boolean = false;
    public Id: string; public submitted: boolean;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public GenericList: any[] = [];
    public SpecialtyList: any[] = [];
    public CategoryList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10; public model = new adm_generic();
    public Rights: any;
    public Keywords: any[] = [];
    public Form1: FormGroup;
    public IsEdit: boolean = false;
    public ControlRights: any;
    @ViewChild("GenericModal") ModelContent: TemplateRef<any>;
    constructor(public _fb: FormBuilder, private encrypt: EncryptionService, private modalService: NgbModal, public _CommonService: CommonService, public loader: LoaderService, public _GenericService: GenericService, public _router: Router, public toastr: CommonToastrService,) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("112");
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            GenericName: ['', [Validators.required]],
            CommonUse: [''],
            SpecialtyId: ['', [Validators.required]],
            CategoryID: ['', [Validators.required]],
            CategoryDropDownID: [''],
            SpecialtyDropDownID: [''],
            IsActive: ['', [Validators.required]],
        });
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Generic';
        this.PConfig.Action.Add = false;
        this.PConfig.Fields = [
            { Name: "GenericName", Title: "Generic Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Specialty", Title: "Specialty", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Category", Title: "Category", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "IsActive", Title: "Is Active", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._GenericService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.GenericList = m.DataList;
                this.loader.HideLoader();
            });
    }

    AddRecord(id: string) {
        this.IsEdit = false;
        if (id != "0") {
            this.loader.ShowLoader();
            this._GenericService.GetById(id).then(m => {
                debugger
                if (m.ResultSet != null) {
                    this.IsEdit = true;
                    this.model = m.ResultSet.result;
                     this.SpecialtyList = m.ResultSet.dropdown.filter(a => a.DropDownID == 24);
                    this.CategoryList = m.ResultSet.dropdown.filter(a => a.DropDownID == 67);
               this.modalService.open(this.ModelContent, { size: 'md' });
                }
                this.loader.HideLoader();
            });
        }
        else {
            this.loader.ShowLoader();
            this._GenericService.FormLoad().then(m => {
                if (m.IsSuccess) {
                    this.loader.ShowLoader();
                    this.model = new adm_generic();
                    this.modalService.open(this.ModelContent, { size: 'md' });
                    this.loader.HideLoader();
                    this.SpecialtyList = m.ResultSet.result.filter(a => a.DropDownID == 24);
                    this.CategoryList = m.ResultSet.result.filter(a => a.DropDownID == 67);
                    this.loader.HideLoader();
                } else
                    this.loader.HideLoader();
            });
           
        }
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._GenericService.SaveOrUpdate(this.model).then(m => {
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
            this._GenericService.Delete(this.model.ID.toString()).then(m => {
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
        this._GenericService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
}