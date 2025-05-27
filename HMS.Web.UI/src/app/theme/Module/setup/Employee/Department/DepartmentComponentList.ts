import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { DepartmentService } from './DepartmentService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
@Component({
    moduleId: module.id,
    templateUrl: 'DepartmentComponentList.html',
    providers: [DepartmentService],
})

export class DepartmentComponentList {

    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public departmentList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public ControlRights: any;

    constructor(public _CommonService: CommonService, public loader: LoaderService, public _DepartmentService: DepartmentService
        , public _router: Router, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("53");
    }



    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = "Departments";
        this.PConfig.Action.Add = true;
        this.PConfig.Action.AddTextType = "T";
        this.PConfig.Action.AddText = "Add";
        this.PConfig.Fields = [
            //{ Name: "ID", Title: "ID", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "DepartmentName", Title: "Name", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Employees", Title: "Employees", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" }
        ];
    }

    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();

        this.Id = "0";
        this._DepartmentService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.departmentList = m.DataList;
                this.loader.HideLoader();
            });
    }

    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }


    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._DepartmentService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }


    AddRecord(id: string) {
        ;
        if (id != "0")
            this.loader.ShowLoader();

        this.Id = id;
        this.IsList = false;
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
            this._DepartmentService.Delete(id).then(m => {

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
}