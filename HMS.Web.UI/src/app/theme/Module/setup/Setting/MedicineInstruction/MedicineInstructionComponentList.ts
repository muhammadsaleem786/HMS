import { Component, OnInit, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MedicineInstructionService } from './MedicineInstructionService';
import { emr_instruction } from './InstructionModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    moduleId: module.id,
    templateUrl: 'MedicineInstructionComponentList.html',
    providers: [MedicineInstructionService],
})
export class MedicineInstructionComponentList {
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public InstructionList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public Form1: FormGroup;
    public model = new emr_instruction();
    public submitted: boolean; public IsEdit: boolean = false;
    @ViewChild("InvestigationModal") ModelContent: TemplateRef<any>; public ControlRights: any;
    constructor(public _fb: FormBuilder, private modalService: NgbModal,public _CommonService: CommonService, public loader: LoaderService, public _MedicineInstructionService:MedicineInstructionService
        , public _router: Router, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        //this.Keywords = this._CommonService.GetKeywords("Instructions");
        this.ControlRights = this._CommonService.ScreenRights("10");
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            Instructions: ['', [Validators.required]],
        });
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Instructions';
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "Instructions", Title: "Instructions", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._MedicineInstructionService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.InstructionList = m.DataList;
                this.loader.HideLoader();
            }); }
    AddRecord(id: string) {
    this.IsEdit = false;
        if (id != "0") {
            this.loader.ShowLoader(); this.IsEdit = true;
            this._MedicineInstructionService.GetById(id).then(m => {
                if (m.ResultSet != null) {
                    this.model = m.ResultSet.Result;
                    this.modalService.open(this.ModelContent, { size: 'md' });
                }
                this.loader.HideLoader();
            });
        }
        else {
            this.loader.ShowLoader();
            this.model = new emr_instruction();
            this.modalService.open(this.ModelContent, { size: 'md' });
            this.loader.HideLoader();
        }
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._MedicineInstructionService.SaveOrUpdate(this.model).then(m => {
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
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
            var result = confirm("Are you sure you want to delete selected record.");
            if (result == true) {
                this.loader.ShowLoader();
                this._MedicineInstructionService.Delete(this.model.ID.toString()).then(m => {

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
        this._MedicineInstructionService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
}