import { Component, Input, Output, OnInit, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { TimeAttendanceModel, EmpTimeAttListModel, EmpAttendanceSumaryModel } from './TimeAttendanceModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { TimeAttendanceService } from './TimeAttendanceService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { Company } from '../../../models/company.model';
import { IMyDpOptions, IMyDateModel, MyDatePicker } from 'mydatepicker';
import { ActivatedRoute } from '@angular/router';

import { MatPaginator, MatSort, MatTableDataSource, MatTable, PageEvent } from '@angular/material';
import { DataSource } from '@angular/cdk/table';
import { SelectionModel } from '@angular/cdk/collections';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

@Component({
    selector: 'setup-TimeAttendanceComponentForm',
    templateUrl: './TimeAttendanceComponentForm.html',
    moduleId: module.id,
    providers: [TimeAttendanceService, FormBuilder],
})

export class TimeAttendanceComponentForm implements OnInit {

    dataSource;
    displayedColumns = [];
    @ViewChild(MatSort) sort: MatSort;
    @ViewChild(MatPaginator) paginator: MatPaginator;

    public Rights: any;
    public ControlRights: any;
    length = 100;
    pageSize = 20;
    pageSizeOptions: number[] = [5, 10, 25, 100];

    selection = new SelectionModel<Element>(true, []);

    /** Whether the number of selected elements matches the total number of rows. */
    // MatPaginator Output
    pageEvent: PageEvent;

    setPageSizeOptions(setPageSizeOptionsInput: string) {
        this.pageSizeOptions = setPageSizeOptionsInput.split(',').map(str => +str);
    }

    /**
   * Pre-defined columns list for user table
   */
    columnNames = [
        {
            id: "Date",
            value: "Date"
        },
        {
            id: "Status",
            value: "Status"
        },
        {
            id: "TimeIn",
            value: "TimeIn"
        },
        {
            id: "TimeOut",
            value: "TimeOut"
        },
        {
            id: "WorkingHours",
            value: "WorkingHours"
        },
        {
            id: "DelayTime",
            value: "DelayTime"
        },
        {
            id: "ExcusedTime",
            value: "ExcusedTime"
        },
    ];


    public LeaveTypesList = [];
    public EmpfilteredList = [];
    public AttendanceStatusList = [];
    public CompanyWorkingDays = new Company();
    public PublicHolidays: any[] = [];
    public EmployeeTimeList = new EmpTimeAttListModel();
    public MonthDays = [];
    public keyword: string;
    public DisplayMember: string;
    public LeaveType: string;
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public IsEmpFound: boolean = false;
    public DefaultStatusID: number = 0;
    public model = new TimeAttendanceModel();
    public attendSummary = new EmpAttendanceSumaryModel();
    public AttendDate: Date = new Date();;
    public TimeIn: string;
    public TimeOut: string;
    public WasInside: boolean = false;
    public MonthFilterDate: Date;
    public EmpID: string = '0';
    public txtSearch: string = '';

    @HostListener('click', ['$event'], )
    Clickoutdocument(event) {
        if (!this.WasInside)
            this.Close();

        this.WasInside = false;
    }

    IsModalClick() {
        this.WasInside = true;
    }
    @ViewChild('closeModal') closeModal: ElementRef;
    constructor(public _fb: FormBuilder, public loader: LoaderService
        , public _AttendanceService: TimeAttendanceService, public toastr: CommonToastrService
        , public _commonService: CommonService, private route: ActivatedRoute, private encrypt: EncryptionService) {
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._commonService.ScreenRights("58");
    }

    ngOnInit() {
        this.EmpID = this.route.snapshot.queryParamMap.get('id') == null ? '0' : this.route.snapshot.queryParamMap.get('id');
        var date = this.route.snapshot.queryParamMap.get('date') == null ? new Date() : new Date(this.route.snapshot.queryParamMap.get('date'));
        if (parseInt(this.EmpID) > 0) {
            this.Refresh(true, date);
        }
        this.displayedColumns = this.columnNames.map(x => x.id);
    }

    Refresh(IsFilterForMonth: boolean, dat?: any) {
        this.loader.ShowLoader();
        if (parseInt(this.EmpID) > 0) {
            this._AttendanceService
                .GetTimeAttendanceByEmployeeID(this.EmpID).then(m => {
                    this.EmployeeTimeList = m.ResultSet.obj == null ? new EmpTimeAttListModel() : m.ResultSet.obj;
                    this.PublicHolidays = m.ResultSet.result.PublicHolidays;
                    this.CompanyWorkingDays = m.ResultSet.result.CompanyWorkingDays;
                    this.EmployeeTimeList.TotalAbsents = 0;
                    this.EmployeeTimeList.TotalPresents = 0;
                    this.EmployeeTimeList.TotalLeaves = 0;
                    this.EmployeeTimeList.TotalHolidays = 0;
                    this.EmployeeTimeList.TotalWeekends = 0;
                    if (IsFilterForMonth) {
                        var date = new Date(dat);
                        this.getDaysInMonth(date.getFullYear(), date.getMonth());
                    }
                    else {
                        this.MonthDays = [];
                        var dt = new Date(this.MonthFilterDate);
                        this.MonthDays.push(
                            {
                                Date: this.getDayNameAndDate(dt),
                                Status: this.GetAttStatus(dt),
                                TimeIn: this.GetAttCellValues(dt, 'TI'),
                                TimeOut: this.GetAttCellValues(dt, 'TO'),
                                WorkingHours: this.GetAttCellValues(dt, 'WH'),
                                DelayTime: this.GetAttCellValues(dt, 'DL'),
                                ExcusedTime: this.GetAttCellValues(dt, 'EX'),
                            }
                        );
                        this.createTable(this.MonthDays);
                    }
                    this.loader.HideLoader();
                });
        }
    }

    getDaysInMonth(year, month) {
        /**
         * @param {int} The month number, 0 based
         * @param {int} The year, not zero based, required to account for leap years
         * @return {Date[]} List with date objects for each day of the month
          */

        // Since no month has fewer than 28 days
        var date = new Date(year, month, 1);
        this.MonthDays = [];

        if (this.EmployeeTimeList.AtttimeListDet.length > 0)
        {
            var i = 0;
            while (date.getMonth() === month) {
                //this.MonthDays.push(new Date(date));
                this.MonthDays.push({
                    Date: this.getDayNameAndDate(date),
                    Status: this.GetAttStatus(date),
                    TimeIn: this.GetAttCellValues(date, 'TI'),
                    TimeOut: this.GetAttCellValues(date, 'TO'),
                    WorkingHours: this.GetAttCellValues(date, 'WH'),
                    DelayTime: this.GetAttCellValues(date, 'DL'),
                    ExcusedTime: this.GetAttCellValues(date, 'EX'),
                });
                date.setDate(date.getDate() + 1);
                i++;
            }
        }

        this.createTable(this.MonthDays);
    }

    getDayNameAndDate(dateStr): string {
        var date = new Date(dateStr);
        return date.toLocaleDateString("en-PK", {
            weekday: 'long',
            year: 'numeric',
            month: 'numeric',
            day: 'numeric'
        });
    }



    IsWeekend(dat: Date): boolean {
        var date = new Date(dat);

        var WEDays = [];
        if (!this.CompanyWorkingDays.WDSunday)
            WEDays.push({ 'index': 0 });
        if (!this.CompanyWorkingDays.WDMonday)
            WEDays.push({ 'index': 1 });
        if (!this.CompanyWorkingDays.WDTuesday)
            WEDays.push({ 'index': 2 });
        if (!this.CompanyWorkingDays.WDWednesday)
            WEDays.push({ 'index': 3 });
        if (!this.CompanyWorkingDays.WDThursday)
            WEDays.push({ 'index': 4 });
        if (!this.CompanyWorkingDays.WDFriday)
            WEDays.push({ 'index': 5 });
        if (!this.CompanyWorkingDays.WDSatuday)
            WEDays.push({ 'index': 6 });

        // Sunday = 0;
        // Saturday = 6

        var record = WEDays.filter(x => x.index == date.getDay());
        if (record.length > 0)
            return true;
        else return false;

    }

    GetAttStatus(date): string {
        var itm = this.EmployeeTimeList.AtttimeListDet.filter(x => +this.GetDate(this.GetFormatDate(x.Date)) === +this.GetDate(date));
        if (itm.length > 0) {
            if (itm[0].Status == 'Present') {
                this.EmployeeTimeList.TotalPresents++;
                return 'P';
            }
            else if (itm[0].Status == 'Absent') {
                this.EmployeeTimeList.TotalAbsents++;
                return 'A';
            }
            else if (itm[0].Status == 'Leave') {
                this.EmployeeTimeList.TotalLeaves++;
                return 'L';
            }
        }

        var holiday = this.PublicHolidays.filter(x => (new Date(x.FromDate)) <= (new Date(date.toDateString())) && (new Date(x.ToDate)) >= (new Date(date.toDateString())));
        if (holiday.length > 0)
        {
            this.EmployeeTimeList.TotalHolidays++;
            return 'H';
        }
           
        if (this.IsWeekend(date))
        {
            this.EmployeeTimeList.TotalWeekends++;
            return 'W';  
        }
        else {
            var TodayDate = new Date();
            var dt = new Date(date);
            if (dt <= TodayDate) {
                this.EmployeeTimeList.TotalAbsents++;
                return 'A';
            }
        }

        return '';
    }


    GetFormatDate(dat: string) {
        var d = parseInt(dat.toString().split('/')[0]);
        var m = parseInt(dat.toString().split('/')[1]);
        var y = parseInt(dat.toString().split('/')[2]);

        var date = new Date(y, m - 1, d);
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getMonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return yyyy + '-' + mm + '-' + dd;
    };

    GetAttCellValues(date, CellName): string {
        var itm = this.EmployeeTimeList.AtttimeListDet.filter(x => +this.GetDate(this.GetFormatDate(x.Date)) === +this.GetDate(date));
        if (itm.length > 0) {
            if (CellName == 'TI') {
                if (itm[0].Status == 'Leave')
                    return 'Leave';
                else
                    return itm[0].TimeIn;
            }
            else if (CellName == 'TO')
                return itm[0].TimeOut;
            else if (CellName == 'WH')
                return itm[0].WorkingHours;
            else if (CellName == 'DL')
                return itm[0].DelayTime;
            else if (CellName == 'EX')
                return itm[0].ExcusedTime;
        }

        var holiday = this.PublicHolidays.filter(x => (new Date(x.FromDate)) <= (new Date(date.toDateString())) && ((new Date(x.ToDate)) >= new Date(date.toDateString())));
        if (holiday.length > 0 && CellName == 'TI')
            return 'Holiday';

        if (this.IsWeekend(date) && CellName == 'TI')
            return 'Weekend';
        else if (CellName == 'TI') {
            var TodayDate = new Date();
            ;
            if (date <= TodayDate)
                return 'Absent';
        }

        return '';
    }

    GetDate(dat): Date {
        var date = new Date(dat);
        return new Date(date.toDateString());
    }


    ondayFilterDateChanged(event: IMyDateModel) {
        this.txtSearch = '';
        if (event) {
            this.Refresh(false);
        } else {
            this.Refresh(true, new Date());
        }

    }

    createTable(dataList) {
        let tableArr: Element[] = dataList;
        this.dataSource = new MatTableDataSource(tableArr);
        this.dataSource.sort = this.sort;
        //if (this.dataSource.data.length > 3) {
        this.paginator.pageSize = 20
        this.paginator.pageIndex = 0
        this.dataSource.paginator = this.paginator;
        //}
    }

    applyFilter(filterValue: string) {
        this.dataSource.filter = filterValue.trim().toLowerCase();
        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }

    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true

        if (isValid) {
            this.submitted = false;

            this.loader.ShowLoader();

            this._AttendanceService.SaveOrUpdate(this.model).then(m => {

                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.WasInside = true;
                    this.closeModal.nativeElement.click();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }


    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AttendanceService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);

                this.WasInside = true;
                this.closeModal.nativeElement.click();
            });
        }
    }

    Close() {
        if (!this.WasInside)
            this.pageClose.emit(0);
        else
            this.pageClose.emit(1);
    }
}
