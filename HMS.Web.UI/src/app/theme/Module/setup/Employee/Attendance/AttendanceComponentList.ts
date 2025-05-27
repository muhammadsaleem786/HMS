import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AttendanceModel } from './AttendanceModel';
import { AttendanceService } from './AttendanceService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

@Component({
    moduleId: module.id,
    templateUrl: 'AttendanceComponentList.html',
    providers: [AttendanceService],
})

export class AttendanceComponentList {

    public Id: string;
    public PModel = new PaginationModel();
   
    public PConfig = new PaginationConfig();
    public AttendanceList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _attendanceService: AttendanceService
        , public _router: Router, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("57");
    }

    ngOnInit() {

        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = "Employee Attendance Entry";
        this.PConfig.Action.Add = true;
        this.PConfig.Action.AddTextType = "T";
        this.PConfig.Action.AddText = "Add";
        this.PConfig.Fields = [
            { Name: "Employee", Title: "Employee", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Date", Title: "Date", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "TimeIn", Title: "Time-In", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "TimeOut", Title: "Time-Out", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Status", Title: "Status", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];



    }

    Refresh() {
        this.loader.ShowLoader();
        this.Id = "0";
        this._attendanceService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.AttendanceList = m.DataList;
                this.loader.HideLoader();
            });
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._attendanceService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }


    AddRecord(id: string) {
        if (id == "0"){
            this.loader.ShowLoader();
            this._router.navigate(['home/Attendance/saveAttend']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Attendance/saveAttend'], { queryParams: { id: this.Id } });

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
            this._attendanceService.Delete(id).then(m => {

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