import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, TemplateRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { PayrollService } from './EmployeePayrollService';
import { RunPayrollModel, SalaryModel } from './EmployeePayrollModel';
import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from 'angular-2-dropdown-multiselect';
import '../../../../../AngularElement/multiselect-dropdown.css';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';


@Component({
    selector: 'PayrollComponent',
    templateUrl: './EmployeePayrollComponentForm.html',
    moduleId: module.id,
    providers: [PayrollService],
})

export class PayrollComponentForm implements OnInit, OnChanges {
    public model = new RunPayrollModel();
    public SalaryModel = new SalaryModel();
    public PayrollDetailList: any[] = [];
    public EmployeesList = [];
    public PayScheduleList = [];
    public UniqueId: number;
    @Input() id: number;
    public IsReadOnly = false;
    public IsCancelAfterRunPayroll: boolean = false;
    @ViewChild('closeModal') closeModal: ElementRef;
    @Output() pageClose: EventEmitter<string> = new EventEmitter<string>();
    public Rights: any;
    public ControlRights: any;
    public ColumnSettings: IMultiSelectSettings = {
        pullRight: false,
        enableSearch: true,
        checkedStyle: 'checkboxes',
        buttonClasses: 'btn btn-default',
        selectionLimit: 0,
        closeOnSelect: false,
        showCheckAll: true,
        showUncheckAll: true,
        dynamicTitleMaxItems: 1,
        maxHeight: '300px',
    };

    public ColumnTexts: IMultiSelectTexts = {
        checkAll: 'Select all',
        //uncheckAll: 'Uncheck all',
        checked: 'checked',
        checkedPlural: 'checked',
        searchPlaceholder: 'Search...',
        defaultTitle: 'All',
    };

    constructor(public _PayrollService: PayrollService, public loader: LoaderService, public toastr: CommonToastrService
        , private encrypt: EncryptionService, public commonservice: CommonService) {
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this.commonservice.ScreenRights("56");
    }



    ngOnChanges() {
        if (typeof (this.id) == "undefined") return;
        if (isNaN(this.id)) {
            this.IsReadOnly = true;
            this.id = +this.id.toString().substring(1); // (+) converts string 'id' to a number
        }
        else {
            this.id = +this.id; // (+) converts string 'id' to a number
            this.IsReadOnly = false;
        }
        if (this.id != 0) {
            this.loader.ShowLoader();
            this._PayrollService.GetPaySchedules().then(m => {
                if (m.IsSuccess) {
                    this.PayScheduleList = m.ResultSet.PayScheduleList;
                    this.loader.HideLoader();
                    if (this.PayScheduleList.length > 0) {
                        this.model.PayScheduleId = this.PayScheduleList[0].ID;
                        this.PayScheduleSelChange();
                        document.getElementById('btnParoll').click();
                    } else {
                        document.getElementById('CloseRunPayrollModal').click();
                        this.toastr.Warning('Warning', 'Please publish the opened payrolls before running the next payroll.');
                    }
                }
            });
        }
    }

    ngOnInit() {

    }

    RunPayroll() {
        this.SalaryModel.EmployeeIds = this.model.EmployeeIds.join(",");
        if (this.SalaryModel.EmployeeIds) {
            this.loader.ShowLoader();
            this.SalaryModel.PayScheduleId = parseInt(this.model.PayScheduleId.toString());
            //this.PayScheduleList = this.PayScheduleList.filter(x => x.ID == this.SalaryModel.PayScheduleId);
            this._PayrollService.RunPayroll(this.SalaryModel).then(m => {
                var result = JSON.parse(m._body);
                if (!result.IsSuccess)
                    this.loader.HideLoader();

                this.UniqueId = result.ResultSet.UniqueId;
                //this.PayScheduleList = result.ResultSet.PayScheduleList;
                this.IsCancelAfterRunPayroll = true;
                this.loader.HideLoader();
                this.closeModal.nativeElement.click();
            });
        } else this.toastr.Warning('Warning', 'Please select atleast one employee to run payroll.');
    }

    PayScheduleSelChange() {
        if (this.model.PayScheduleId) {
            this.model.EmployeeIds = [];
            this.loader.ShowLoader();
            this._PayrollService.GetEmployeesByPayScheduleID(this.model.PayScheduleId).then(m => {
                if (m.IsSuccess) {
                    ;
                    this.EmployeesList = m.ResultSet.EmployeeList;
                    this.EmployeesList.forEach(x => {
                        this.model.EmployeeIds.push(x.id);
                    });
                }
                this.loader.HideLoader();
            });
        }
    }
    GetFormatDate(dat, dat2): string {
        var date = new Date(dat);  //or your date here
        var date2 = new Date(dat2);  //or your date here
        var StartDate = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
        var EndDate = date2.getDate() + '/' + (date2.getMonth() + 1) + '/' + date2.getFullYear();
        return (StartDate + ' - ' + EndDate);
    };

    Close() {
        if (this.IsCancelAfterRunPayroll) {
            var item = this.PayScheduleList.filter(x => x.ID == this.model.PayScheduleId);
            var PayPeriod = this.GetFormatDate(item[0].PayPeriodStartDate, item[0].PayPeriodEndDate).toString();
            this.pageClose.emit(this.UniqueId + '@' + PayPeriod);
        }
        else
            this.pageClose.emit('');
    }
}
