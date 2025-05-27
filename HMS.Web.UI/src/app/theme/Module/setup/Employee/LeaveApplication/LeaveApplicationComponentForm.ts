import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LeaveApplicationModel } from './LeaveApplicationModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CompanyService } from '../../../setup/Setting/Company/company.service';
import { LeaveApplicationService } from './LeaveApplicationService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { IMyDpOptions, IMyDateModel, MyDatePicker } from 'mydatepicker';
import { Router, ActivatedRoute } from '@angular/router';
import { filter } from 'rxjs/operators';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
@Component({
    selector: 'setup-LeaveApplicationComponentForm',
    templateUrl: './LeaveApplicationComponentForm.html',
    moduleId: module.id,
    providers: [LeaveApplicationService, FormBuilder, CompanyService],
})
export class LeaveApplicationComponentForm implements OnInit {
    public LeaveTypesList = [];
    EmpfilteredList = [];
    public TakenLeavesByEmp = [];
    public TotalLeavesOfEmp = [];
    public keyword: string;
    public WorkingHour: number = 0;
    public CountDays: number = 0;
    public DisplayMember: string;
    public LeaveType: string;
    public DefaulCategoryID: number;
    public DefaulAllowanceType: string;
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public IsEmpFound: boolean = false;
    public model = new LeaveApplicationModel();
    public WasInside: boolean = false;
    public sub: any;
    public IsEdit: boolean = false;
    public Rights: any;
    public ControlRights: any;
    constructor(public _fb: FormBuilder, public loader: LoaderService, public _router: Router
        , public _leaveApplicationService: LeaveApplicationService,
        public _AdmCompanyService: CompanyService, public toastr: CommonToastrService, public route: ActivatedRoute,
        private encrypt: EncryptionService, public commonservice: CommonService) {
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this.commonservice.ScreenRights("55");
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            EmployeeID: ['', <any>[Validators.required]],
            LeaveTypeID: ['', [Validators.required]],
            FromDate: ['', [Validators.required]],
            ToDate: ['', [Validators.required]],
            Hours: ['', [Validators.required]],
            LeaveType: [''],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.id = params.id;
                if (this.id > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._leaveApplicationService.GetById(this.id).then(m => {
                        this.model = m.ResultSet.LeaveObj.EmpLeave;
                        this.TakenLeavesByEmp = m.ResultSet.TotalAndTakenLeaves.TakenLeavesByEmp;
                        this.TotalLeavesOfEmp = m.ResultSet.TotalAndTakenLeaves.TotalLeavesOfEmp;
                        if (m.ResultSet.LeaveObj.pr_employee_mf) {
                            var employee = m.ResultSet.LeaveObj.pr_employee_mf
                            this.DisplayMember = employee.FirstName + ' ' + employee.LastName;
                            this.EmpfilteredList = employee;
                            this.LeaveType = m.ResultSet.LeaveObj.pr_leave_type ? m.ResultSet.LeaveObj.pr_leave_type.Category : '';
                            this.GetLeavesByLeaveType(true);
                        }
                        this.loader.HideLoader();
                    });
                }
                this.loader.HideLoader();
            });
    }
    onDateFromChanged(event: IMyDateModel) {
        if (event) {
            var date = new Date(event.jsdate);
            this.model.ToDate = date;
            this.CountDays = Math.round(Math.abs(date.getTime() - this.model.ToDate.getTime()) / (24 * 60 * 60 * 1000)) + 1;
            this.model.Hours = this.WorkingHour;
        } else {
            this.model.FromDate = null;
            this.model.ToDate = new Date(this.model.FromDate);
            this.model.Hours = this.WorkingHour;
        }

    }
    onDateToChanged(event: IMyDateModel) {
        if (event) {
            var ToDate = new Date(event.jsdate);
            var FromDate = new Date(this.model.FromDate);
            this.CountDays = Math.round(Math.abs(FromDate.getTime() - ToDate.getTime()) / (24 * 60 * 60 * 1000)) + 1;
            if (FromDate.toLocaleDateString() <= ToDate.toLocaleDateString()) {
                var oneday = 24 * 60 * 60 * 1000; //hourse*minutes*seconds*miliseconds
                var noOfDays = Math.round(Math.abs(FromDate.getTime() - ToDate.getTime()) / (oneday)) + 1;
                this.model.Hours = this.WorkingHour * noOfDays;
            }
            else this.model.Hours = this.WorkingHour;
        }
        else this.model.Hours = this.WorkingHour;
        this.IsValidHours();
    }
    GetFormatDate(dat) {
        var date = new Date(dat);
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getMonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return yyyy + '-' + mm + '-' + dd;
    };
    GetLeavesByLeaveType(IsEdit) {
        this.loader.ShowLoader();
        this._leaveApplicationService.GetLeavesByTypes(this.LeaveType).then(m => {
            if (m.ResultSet.length > 0) {
                this.LeaveTypesList = m.ResultSet;
                if (!IsEdit) this.model.LeaveTypeID = this.LeaveTypesList[0].ID;
            }
            this.loader.HideLoader();
        });
    }
    MatchesFound() {
        if (this.model.EmployeeID == undefined) {
            this.DisplayMember = ''; this.model.EmployeeID = undefined;
        }
    }
    FilterEmployees() {
        var self = this;
        this.model.EmployeeID = undefined;
        this.keyword = self.DisplayMember.toString();
        if (this.keyword.length >= 2) {
            self._leaveApplicationService.GetFilterEmployees(this.keyword).then((matches: any) => {
                self.EmpfilteredList = [];
                matches = matches.ResultSet;
                if (typeof matches !== 'undefined' && matches.length > 0) {
                    for (let i = 0; i < matches.length; i++) {
                        self.EmpfilteredList.push(matches[i]);
                        this.IsEmpFound = true;
                        if (self.EmpfilteredList.length > 20 - 1) {
                            break;
                        }
                    }
                } else { this.IsEmpFound = false; }
            });
        } else {
            self.EmpfilteredList = [];
        }
    }
    SelectItem(item: any) {
        if (item) {
            this.loader.ShowLoader();
            this._AdmCompanyService.GetLeaveWorkingHour().then(m => {
                this.WorkingHour = m.ResultSet;
            });
            this._leaveApplicationService.GetLeavesByEmpID(item.ID).then(m => {
                this.TakenLeavesByEmp = m.ResultSet.TakenLeavesByEmp;
                this.TotalLeavesOfEmp = m.ResultSet.TotalLeavesOfEmp;
                this.loader.HideLoader();
            });
            this.model.EmployeeID = item.ID;
            this.DisplayMember = item.FirstName + ' ' + item.LastName;
            this.EmpfilteredList = [];
            this.IsEmpFound = true;
        }
    }
    GetLeaveBalance(): number {
        var TotalLeaves = this.TotalLeavesOfEmp.length > 0 ? this.TotalLeavesOfEmp.filter(x => x.Category == this.LeaveType && x.LeaveTypeID == this.model.LeaveTypeID) : [];
        var TakenLeaves = this.TakenLeavesByEmp.length > 0 ? this.TakenLeavesByEmp.filter(x => x.Category == this.LeaveType && x.LeaveTypeID == this.model.LeaveTypeID) : [];
        var Total = 0;
        var Taken = 0;
        for (var i = 0; i < TotalLeaves.length; i++) {
            Total += TotalLeaves[i].TotalHours;
        }
        for (var i = 0; i < TakenLeaves.length; i++) {
            Taken += TakenLeaves[i].Hours;
        }
        return Total - Taken;
    }
    IsValidHours() {
        var bal = this.GetLeaveBalance();
        var Ltype = this.LeaveType;
        if (this.CountDays > 1) {
            this.model.disabledbox = true;
        } else {
            this.model.disabledbox = false;
        }
        if (Ltype != "L") {
            if (this.model.Hours > bal)
                this.toastr.Error('Error', 'Hours exceed remaining hours ' + bal);
        }
    }
    ValidDateAndHours(): boolean {
        var bal = this.GetLeaveBalance();
        this.model.FromDate = new Date(this.GetFormatDate(this.model.FromDate));
        this.model.ToDate = new Date(this.GetFormatDate(this.model.ToDate));
        if (this.model.FromDate > this.model.ToDate) {
            this.toastr.Error('Invalid Date', 'From date is greater than To date');
            return false;
        }
        else if (this.model.Hours <= 0) {
            this.toastr.Error('Error', 'Hours should be greater than 0');
            return false;
        }
        else if (this.model.Hours > bal) {
            this.toastr.Error('Error', 'Hours exceed remaining hours ' + bal);
            return false;
        }
        else
            return true;
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid)
            isValid = this.ValidDateAndHours();
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._leaveApplicationService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this._router.navigate(['/home/Leaveentry']);
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                }
                this.loader.HideLoader();
            });
        }
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._leaveApplicationService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                this._router.navigate(['/home/Leaveentry']);
            });
        }
    }
    Close() {
        if (!this.WasInside)
            this.pageClose.emit(0);
        else
            this.pageClose.emit(1);

        this.model = new LeaveApplicationModel();
        this.LeaveTypesList = [];
        this.LeaveType = '';
        this.EmpfilteredList = [];
        this.DisplayMember = '';
        this.submitted = false;
    }
}
