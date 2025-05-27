import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { DropDownService } from './DropDownService';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { sys_drop_down_value } from './DropDownModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    moduleId: module.id,
    templateUrl: 'DropDownComponentList.html',
    providers: [DropDownService],
})

export class DropDownComponentList {
    public Form1: FormGroup;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public departmentList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    @ViewChild('closeModal') closeModal: ElementRef;
    public Rights: any;
    public MFDropDownList = [];
    public DTDropDownList = [];
    public DepDropdownValuesList = [];
    public DropdownDependentValuesList = [];
    public PaginationList = [];
    public hideShowSecondDropDown = false;
    public submitted: boolean;
    public WasInside: boolean = false;
    public model = new sys_drop_down_value();
    constructor(public _fb: FormBuilder, public _CommonService: CommonService, public loader: LoaderService, public _dropdownService: DropDownService
        , public toastr: CommonToastrService, public _router: Router) {
        this.loader.ShowLoader();
    }

    ngOnInit() {
        this.Form1 = this._fb.group({
            Value: ['', [Validators.required]],
        });
        this.PConfig.Action.IsShowSearchAndAddButton = false;
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = "DropDowns";
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "Value", Title: "Value", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "SystemGenerated", Title: "System Generated", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
        this.FillDropDown();
    }
    FillDropDown() {
        this.loader.ShowLoader();
        this._dropdownService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.MFDropDownList = m.ResultSet.MFDropDownList;
                this.model.DropDownID = this.MFDropDownList.length > 0 ? this.MFDropDownList[0].ID : 0;
                this.Managedropdown(m.ResultSet.DTDropDownList);
            }
            this.loader.HideLoader();
        });
    }

    MFDDChange() {
        this.model.ID = 0;
        this.model.Value = '';
        this.DepDropdownValuesList = [];
        this.DropdownDependentValuesList = [];
        this.PaginationList = [];
        this.loader.ShowLoader();
        this._dropdownService
            .GetValueByMFID(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    //this.PModel.TotalRecord = result.ResultSet.DTList.length;
                    this.Managedropdown(result.ResultSet);
                }
                this.loader.HideLoader();
            });
    }

    Managedropdown(result) {
        
        this.DropdownDependentValuesList = [];
        this.DTDropDownList = result.DTList;
        this.DropdownDependentValuesList = result.DepList;
        if (this.DropdownDependentValuesList.length > 0) {
            this.model.DependedDropDownValueID = this.DropdownDependentValuesList[0].ID;
            this.model.DependedDropDownID = this.DropdownDependentValuesList[0].DropDownID;
            this.DepDropdownValuesList = this.DropdownDependentValuesList;
            this.hideShowSecondDropDown = true;
        } else {
            this.model.DependedDropDownValueID = null;
            this.hideShowSecondDropDown = false;
        }

        this.PaginationList = this.DTDropDownList;
    }
    Clear() {
        this.model.ID = 0;
        this.model.SystemGenerated = false;
        this.model.DependedDropDownID = null;
        this.model.Value = '';
        this.submitted = false;
    }
    DTDDChange() {
        this.loader.ShowLoader();
        this._dropdownService.GetDependentValueByDepID(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                
                this.DTDropDownList = result.ResultSet;
                this.PaginationList = this.DTDropDownList;
            }
            this.loader.HideLoader();
        });
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._dropdownService.GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.PModel.TotalRecord = m.TotalRecord;
            this.PaginationList = m.DataList;
            this.loader.HideLoader();
        });
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._dropdownService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.Id = id;
            this.Clear();
            var item = this.PaginationList.filter(x => x.ID == this.Id);
            debugger
            if (item.length > 0) {
                if (item[0].SystemGenerated == "Yes") {
                    this.toastr.Error('You can not change System Generated.');
                } else {
                    this.model.ID = item[0].ID;
                    this.model.Value = item[0].Value;
                    this.model.SystemGenerated = item[0].SystemGenerated;
                    this.model.DependedDropDownID = item[0].DependedDropDownID;
                }
            }
        }
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._dropdownService.Delete(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {                    
                    if (this.model.DependedDropDownValueID == null) {
                        this.PaginationList = result.ResultSet;
                    } else {
                        this.DTDropDownList = result.ResultSet;
                        this.DTDDChange();
                    }
                    this.model = new sys_drop_down_value();
                    this.Clear();
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

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
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._dropdownService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    if (this.model.DependedDropDownValueID == null) {
                        this.PaginationList = result.ResultSet;
                    } else {
                        this.DTDropDownList = result.ResultSet;
                        this.DTDDChange();
                    }
                    this.Clear();
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
}