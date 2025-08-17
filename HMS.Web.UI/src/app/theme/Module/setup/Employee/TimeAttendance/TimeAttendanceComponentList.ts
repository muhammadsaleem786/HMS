import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TimeAttendanceModel } from './TimeAttendanceModel';
import { Company } from '../../../models/company.model';
import { TimeAttendanceService } from './TimeAttendanceService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { IMyDpOptions, IMyDateModel, MyDatePicker } from 'mydatepicker';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

import { MatPaginator, MatSort, MatTableDataSource, MatTable, PageEvent } from '@angular/material';
import { DataSource } from '@angular/cdk/table';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
    moduleId: module.id,
    templateUrl: 'TimeAttendanceComponentList.html',
    providers: [TimeAttendanceService],
})



export class TimeAttendanceComponentList {
    dataSource;
    displayedColumns = [];
    @ViewChild(MatSort) sort: MatSort;
    @ViewChild(MatPaginator) paginator: MatPaginator;

    length = 100;
    pageSize = 15;
    pageSizeOptions: number[] = [5, 10, 25, 100];

    selection = new SelectionModel<Element>(true, []);
    pageEvent: PageEvent;

    setPageSizeOptions(setPageSizeOptionsInput: string) {
        this.pageSizeOptions = setPageSizeOptionsInput.split(',').map(str => +str);
    }
    columnNames = [
        {
            id: "Employee",
            value: "Employee"
        }];


    public Id: string;
    public PModel = new PaginationModel();
    public MonthDays = [];
    public PConfig = new PaginationConfig();
    public AttendanceList: any[] = [];
    public EmployeeTimeList: any[] = [];
    public CompanyWorkingDays: any;
    public UserWorkingDays: any;
    public PublicHolidays: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public MonthFilterDate: Date = new Date();
    public txtSearch: string = '';
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _attendanceService: TimeAttendanceService
        , public _router: Router, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("58");
    }

    ngOnInit() {
        this.Refresh();
    }

    Refresh() {
        this.loader.ShowLoader();
        this.Id = "0";

        this._attendanceService
            .GetTimeAttendanceList().then(m => {
                this.AttendanceList = m.ResultSet.TimeAttList;
                this.CompanyWorkingDays = m.ResultSet.result.CompanyWorkingDays;
                this.PublicHolidays = m.ResultSet.result.PublicHolidays;
                this.createTable(this.AttendanceList);
                var date = new Date(this.MonthFilterDate);
                this.getDaysInMonth(date.getMonth(), date.getFullYear());
                this.loader.HideLoader();
            });
    }

    createTable(dataList) {
        let tableArr: Element[] = dataList;
        this.dataSource = new MatTableDataSource(tableArr);
        this.dataSource.sort = this.sort;
        //if (this.dataSource.data.length > 3) {
        this.paginator.pageSize = 15
        this.paginator.pageIndex = 0
        this.dataSource.paginator = this.paginator;
        //}
    }

    getDaysInMonth(month, year) {
        /**
         * @param {int} The month number, 0 based
         * @param {int} The year, not zero based, required to account for leap years
         * @return {Date[]} List with date objects for each day of the month
          */

        // Since no month has fewer than 28 days
        var date = new Date(year, month, 1);
        this.MonthDays = [];
        this.MonthDays.push({ 'id': 'Employee', 'value': 'Employee' });

        //for (let z = 0; z < this.dataSource.data.length; z++) {
        //    this.MonthDays.push({ 'id': 'Employee', 'value': this.dataSource.data[z].Employee });
        //}

        var i = 0;
        while (date.getMonth() === month) {
            this.MonthDays.push({ 'id': 'Col' + i, 'value': new Date(date) });
            date.setDate(date.getDate() + 1);
            i++;
        }

        //this.displayedColumns = this.columnNames.map(x => x.id);
        this.displayedColumns = this.MonthDays.map(x => x.id);
        //this.displayedColumns = this.MonthDays;
    }

    applyFilter(filterValue: string) {
        this.dataSource.filter = filterValue.trim().toLowerCase();
        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }

    AddAttendance(id: string) {
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Attendance/saveAttend'], { queryParams: { id: this.Id } });

    }

    IsWeekend(dat: Date): boolean {
        if (this.UserWorkingDays.length > 0) {
            var date = new Date(dat);
            var WEDays = [];
            if (this.UserWorkingDays[0].WDSunday)
                WEDays.push({ 'index': 0 });
            if (this.UserWorkingDays[0].WDMonday)
                WEDays.push({ 'index': 1 });
            if (this.UserWorkingDays[0].WDTuesday)
                WEDays.push({ 'index': 2 });
            if (this.UserWorkingDays[0].WDWednesday)
                WEDays.push({ 'index': 3 });
            if (this.UserWorkingDays[0].WDThursday)
                WEDays.push({ 'index': 4 });
            if (this.UserWorkingDays[0].WDFriday)
                WEDays.push({ 'index': 5 });
            if (this.UserWorkingDays[0].WDSaturday)
                WEDays.push({ 'index': 6 });
            var record = WEDays.filter(x => x.index == date.getDay());
            if (record.length > 0)
                return true;
            else return false;
        }
        else return false;
    }

    GetTime(index, dat): string {
        var date = new Date(dat);
        if (index == 0) {
        }
        var empId = this.dataSource.filteredData[index].EmployeeID;
        this.UserWorkingDays = this.CompanyWorkingDays.filter(a => a.EmployeeId == empId);
        var att = this.dataSource.filteredData[index].TimeAttendanceListdt.filter(x => +this.GetTimeInDate(x.TimeIn) === +(new Date(date.toDateString())));
        if (att.length > 0)
            return this.GetAttStatus(att[0]);
        var holiday = this.PublicHolidays.filter(x => (new Date(x.FromDate)) <= (new Date(date.toDateString())) && (new Date(x.ToDate)) >= (new Date(date.toDateString())));
        if (holiday.length > 0)
            return 'PH';
        if (this.IsWeekend(date))
            return 'W';
        else {
            var TodayDate = new Date();
            var date = new Date(dat);
            var jdate = new Date(this.dataSource.filteredData[index].JoiningDate);
            if (date >= jdate && date <= TodayDate)
                return 'A';
        }
        return '';
    }

    GetAttStatus(item): string {
        if (item.Status == 'On Time')
            return 'P';
        else if (item.Status == 'Absent')
            return 'A';
        else if (item.Status == 'Missing In')
            return 'MI';
        else if (item.Status == 'Early Out')
            return 'EO';
        else if (item.Status == 'Missing Out')
            return 'MO';
        else if (item.Status == 'Lateness')
            return 'LA';
        else if (item.Status == 'Leave')
            return 'L';
        else return '';
    }


    GetTimeInDate(dat): Date {
        var date = new Date(dat);
        return new Date(date.toDateString());
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


    onMonthFilterDateChanged(event: IMyDateModel) {
        this.txtSearch = '';
        if (event)
            this.Refresh();
        else {
            this.MonthFilterDate = new Date();
            this.Refresh();
        }
    }


    EditEvent(index) {
        var id = this.AttendanceList[index].EmployeeID;
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Timeattendance'], { queryParams: { id: this.Id, date: this.MonthFilterDate } });
    }

    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else
            this.Refresh();
    }

}