import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { VacationSickLeaveService } from './VacationSickLeaveService';
import { VacationSickLeaveModel } from './VacationSickLeaveModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';

@Component({
    moduleId: module.id,
    templateUrl: 'VacationSickLeaveComponentList.html',
    providers: [VacationSickLeaveService],
})

export class VacationSickLeaveComponentList {

    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public departmentList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;

    constructor(public _CommonService: CommonService, public loader: LoaderService, public _vacationSickLeaveService: VacationSickLeaveService
        , public _router: Router) {
        this.loader.ShowLoader();
        // this.Rights = _CommonService.GetScreenRights("payscheduleListComponent");
    }

    ngOnInit() {

        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = "Vacation and Sick Leave Policy";
        // this.PConfig.Action.ScreenName = this.Rights['ScreenName'];
        // this.PConfig.Action.Add = this.Rights.Allow('Add');
        // this.PConfig.Action.Edit = this.Rights.Allow('Update');
        // this.PConfig.Action.View = this.Rights.Allow('View');
        // this.PConfig.Action.Delete = this.Rights.Allow('Delete');
        // this.PConfig.Action.Print = this.Rights.Allow('Print');
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            //{ Name: "ID", Title: "ID", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "TypeName", Title: "Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Category", Title: "Category", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "AccuruelFrequencyID", Title: "Accrual Frequency", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "EarndValue", Title: "Hours", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" }
        ];
    }

    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();

        this.Id = "0";
        this._vacationSickLeaveService
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
        this._vacationSickLeaveService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }


    AddRecord(id: string) {

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
            this._vacationSickLeaveService.Delete(id).then(m => {
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

        //this.IsList = true;
        //this.Refresh();

        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else
            this.Refresh();
    }

}