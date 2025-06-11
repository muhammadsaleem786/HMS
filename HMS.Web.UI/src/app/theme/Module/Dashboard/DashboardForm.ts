import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { GlobalVariable } from '../../../AngularConfig/global';
import { HttpService } from '../../../CommonService/HttpService';
import { LoaderService } from '../../../CommonService/LoaderService';
import { DashboardService } from './DashboardService';
import { AsidebarService } from '../../../CommonService/AsidebarService';
import { CommonService } from '../../../CommonService/CommonService';
import { EncryptionService } from '../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
import { DashboardModel } from './DashboardModel'
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ValidationVariables } from '../../../AngularConfig/global';
import { IMyDateModel } from 'mydatepicker';
import { ECharts } from 'echarts';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo, InvoiceCompanyInfo, emr_notes_favorite } from '../../Module/setup/Appointment/AppointmentModel';
import { AppointmentService } from '../setup/Appointment/AppointmentService';
declare const require: any;
import { DatePipe } from '@angular/common';
import { debug } from 'util';
import { concat } from 'rxjs';
declare var $: any;
@Component({
    selector: 'DashboardForm',
    templateUrl: './DashboardForm.html',
    moduleId: module.id,
    providers: [DashboardService, AppointmentService, DatePipe]
})
export class DashboardForm {
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public model = new DashboardModel();
    public AppointmentModel = new emr_Appointment();
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public Currency: string;
    public DeshboardData: any = [];
    public AppointmentData: any = [];
    public BirthDayData: any = [];
    public BirthDayList: any = [];
    public FollowUpData: any = [];
    public Form2: FormGroup;
    public ScreenRights: any;
    public ColorArr = ['#b0d3fb', '#fdd4af', '#ace4da', '#e49aa3', '#ccc', '#daa8ff'];
    public PieColorArr = ['#5e54d0', '#D4D7DC'];
    public options: any;
    public IsCustomDate: boolean = false;
    public Male: any;
    public Female: any;
    public other: any;
    public Child: any;
    public TodayDate: any;
    public submitted: boolean;
    appointmentOption: any;
    public Step: number = 30;
    incomeANDexpense: any;
    public DoctorList: any[] = [];
    public StatusList: any[] = [];
    public IncomeList = [];
    public ExpenseList = [];
    public MonthList = [];
    public ServiceType: any;
    public ClickValue: string;
    private subscription: Subscription | null = null;
    @ViewChild("longContent") ModelContent: TemplateRef<any>;
    openAppointmentModal(longContent,PatientId) {
        this.loader.ShowLoader();
        this.AppointmentModel = new emr_Appointment();
        this._AppointmentService.DeshboardLoadDropdown(PatientId).then(m => {
            if (m.IsSuccess) {
               

                this.DoctorList = m.ResultSet.DoctorList;
                this.StatusList = m.ResultSet.StatusList;
                this.AppointmentModel.PatientId = m.ResultSet.PatientObj.ID;
                this.AppointmentModel.PatientName = m.ResultSet.PatientObj.PatientName;
                this.AppointmentModel.CNIC = m.ResultSet.PatientObj.CNIC;
                this.AppointmentModel.Mobile = m.ResultSet.PatientObj.Mobile;
                if (this.Rights.indexOf(12) > -1) {
                    this.AppointmentModel.ServiceId = m.ResultSet.ServiceType.ID;
                    this.AppointmentModel.ServiceName = m.ResultSet.ServiceType.ServiceName;
                    this.AppointmentModel.Price = m.ResultSet.ServiceType.Price;
                    this.AppointmentModel.TokenNo = m.ResultSet.TokenNo;
                    this.AppointmentModel.BillDate = new Date();
                    this.AppointmentModel.AppointmentDate = new Date();
                    this.CalAmount();
                }
                this.modalService.open(longContent, { size: 'lg' });
                this.loader.HideLoader();
            }
        });
    }
    constructor(public _fb: FormBuilder, private encrypt: EncryptionService, private datePipe: DatePipe, public toastr: CommonToastrService, public _AppointmentService: AppointmentService, private modalService: NgbModal, public _CommonService: CommonService, public _DashboardService: DashboardService, public loader: LoaderService
        , public _AsidebarService: AsidebarService, public _router: Router) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.PayrollRegion = this._CommonService.getPayrollRegion();
    }
    ngOnInit() {  
        //debugger
        //(window as any).trackEvent('dashboard', {
        //    email: 'saleem@yahoo.com',
        //    timestamp: new Date().toISOString(),
        //});
        //this.subscription = interval(5000).pipe(switchMap(() => this._DashboardService.getData()))
        //    .subscribe({
        //        next: (response) => {},
        //        error: (err) => console.error('API Error:', err)
        //    });
        var date = new Date();
       var currentdateday = this.datePipe.transform(date, 'dd');
        const months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
        let days = parseInt(currentdateday);
        if (days <= 9)
            this.TodayDate = "0" + '' + (days) + ' ' + months[date.getMonth()];
        else
            this.TodayDate = days + ' ' + months[date.getMonth()];

        var firstDay = new Date(Date.UTC(date.getFullYear(), date.getMonth(), 1));
        var formdate = this._CommonService.GetFormatDate(firstDay);
        var lastDay = new Date(Date.UTC(date.getFullYear(), date.getMonth() + 1, 0));
        var todate = this._CommonService.GetFormatDate(date);
        this.model.FormDate = new Date(firstDay);
        this.model.ToDate = new Date(todate);
        this.Refresh(this.model.ToDate, this.model.ToDate);
       this.Form2 = this._fb.group({
            PatientName: [''],
            PatientId: ['', [Validators.required]],
            CNIC: ['', [Validators.required, Validators.pattern(ValidationVariables.CNICPattern)]],
            Mobile: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.NumberPattern)]],
            DoctorId: [''],
            PatientProblem: [''],
            Notes: [''],
            AppointmentDate: ['', [Validators.required]],
            AppointmentTime: [''],
            StatusId: [''],
            //bill field
            AppointmentId: [''],
            ServiceId: [''],
            BillDate: [''],
            Price: [''],
            Discount: [''],
            PaidAmount: [''],
            DoctorName: [''],
            ServiceName: [''],
            Remarks: [''],
            OutstandingBalance: [''],
        });
    }
    Refresh(fDate: any, ToDate: any) {
        this.loader.ShowLoader();
        fDate = this._CommonService.GetFormatDate(fDate);
        ToDate = this._CommonService.GetFormatDate(ToDate);
        this._DashboardService
            .DataLoad(fDate, ToDate).then(m => {
                this.DeshboardData = [];
                this.AppointmentData = [];
                this.BirthDayData = [];
                this.FollowUpData = [];
                this.IncomeList = [];
                this.ExpenseList = [];
                this.MonthList = [];
                this.BirthDayList = [];
                this.DeshboardData = m.ResultSet.DeshboardData;
                this.AppointmentData = m.ResultSet.Appointment;
                this.BirthDayData = m.ResultSet.BirthDay;
                this.BirthDayList = m.ResultSet.BirthDay;
                this.BirthDayData.forEach((item, index) => {
                    if (item.Image != null && item.Image != '')
                        item.Image = GlobalVariable.BASE_Temp_File_URL + '' + item.Image;
                });
                this.FollowUpData = m.ResultSet.FollowUp;
                m.ResultSet.IncomeAndExpense.forEach((item, index) => {
                    this.IncomeList.push(item.Income);
                    this.ExpenseList.push(item.ExAmount);
                    this.MonthList.push(item.MONTH_NAME);
                });
                this.Male = this.DeshboardData[0].Male;
                this.Female = this.DeshboardData[0].Female;
                this.other = this.DeshboardData[0].Other;
                this.Child = this.DeshboardData[0].Child;
                this.loadPiChart();
                this.LoadIncomeChart();
                this.loader.HideLoader();
            });
    }
    loadPiChart() {
        this.options = {
            tooltip: {
                trigger: 'item'
            },
            legend: {
                top: '5%',
                left: 'center'
            },
            series: [
                {
                    name: 'Access From',
                    type: 'pie',
                    radius: ['40%', '70%'],
                    avoidLabelOverlap: false,
                    itemStyle: {
                        borderRadius: 10,
                        borderColor: '#fff',
                        borderWidth: 2
                    },
                    label: {
                        show: false,
                        position: 'center'
                    },
                    emphasis: {
                        label: {
                            show: true,
                            fontSize: '20',
                            fontWeight: 'bold'
                        }
                    },
                    labelLine: {
                        show: false
                    },
                    data: [
                        { value: this.Male, name: 'Male' },
                        { value: this.Female, name: 'Female' },
                        { value: this.Child, name: 'Child' },
                        { value: this.other, name: 'Others' }
                    ]
                }
            ]
        };
    }
    LoadIncomeChart() {
        const colors = ['#5470C6', '#EE6666'];      
        this.incomeANDexpense = {
            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'cross',
                    crossStyle: {
                        color: '#999'
                    }
                }
            },

            legend: {
              
                data: ['Income', 'Expense']
            },
            color: ['#0d3b5a', '#83d0cf', '#c1932d', '#6ac395', '#ddd'],
            xAxis: [
                {
                    type: 'category',
                    data: this.MonthList,// ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', ],
                    axisPointer: {
                        type: 'shadow'
                    }
                }
            ],
            yAxis: [
                {
                    type: 'value',
                    //  name: 'Revenue (EUR Million)',
                    nameLocation: 'middle',
                    nameTextStyle: {
                        padding: [0, 0, 20]

                    },

                }

            ],
            series: [
                {
                    name: 'Income',
                    type: 'bar',
                    barWidth: '10',
                    label: {
                        //    show: true
                    },
                    data: this.IncomeList, // [3140.4969, 3295.398, 3425.6202, 3584.3879, 3718.5932, 3322.4434],
                    itemStyle: {
                        barBorderRadius: 5,
                        shadowColor: '#efefef',
                        shadowBlur: 3
                    }

                },
                {
                    name: 'Expense',
                    type: 'bar',
                    barWidth: '10',
                    label: {
                        //    show: true
                    },
                    data: this.ExpenseList,//[3115.408, 3189.078, 3277.8823, 3406.3439, 3528.0631, 3163.6963],
                    itemStyle: {
                        barBorderRadius: 5,
                        shadowColor: '#efefef',
                        shadowBlur: 3
                    }
                },
            ]
        };
    }
    onChange(Val: any) {
        var date = new Date();
        if (Val == "0") {
            this.IsCustomDate = false;
            var fDate = this._CommonService.GetFormatDate(date);
            var ToDate = this._CommonService.GetFormatDate(date);
            this.Refresh(fDate, ToDate);
        }
        if (Val == "1") {
            this.IsCustomDate = false;
            var date1 = new Date();
            var YestDate = date1.setDate(date1.getDate() - 1);
            var fDate = this._CommonService.GetFormatDate(YestDate);
            var ToDate = this._CommonService.GetFormatDate(date);
            this.Refresh(fDate, ToDate);
        }
        if (Val == "2") {
            this.IsCustomDate = false;
            var date2 = new Date();
            var Last15Date = date2.setDate(date2.getDate() - 15);
            var fDate = this._CommonService.GetFormatDate(Last15Date);
            var ToDate = this._CommonService.GetFormatDate(date);
            this.Refresh(fDate, ToDate);
        }
        if (Val == "3") {
            this.IsCustomDate = false;
            var date3 = new Date();
            var firstDay = new Date(date3.getFullYear(), date3.getMonth(), 1);
            var lastDay = new Date(date3.getFullYear(), date3.getMonth() + 1, 0);
            this.Refresh(firstDay, lastDay);
        }
        if (Val == "4")
            this.IsCustomDate = true;

    }
    onChangeType(Val: any) {
        this.loader.ShowLoader();
        var fDate = this._CommonService.GetFormatDate(this.model.FormDate);
        var ToDate = this._CommonService.GetFormatDate(this.model.ToDate);
        this._DashboardService
            .DataLoad(fDate, ToDate).then(m => {                
                this.FollowUpData = [];                
                this.FollowUpData = m.ResultSet.FollowUp;
                if (Val == 1) {
                    this.FollowUpData = m.ResultSet.FollowUp.filter(a => a.IsCreateAppointment == 1);
                }
                if (Val == 2) {
                    this.FollowUpData = m.ResultSet.FollowUp.filter(a => a.IsCreateAppointment == 0);
                }
                this.loader.HideLoader();
            });        
    }
    onDateChanged() {
        this.loader.ShowLoader();
        var fDate = this._CommonService.GetFormatDate(this.model.FormDate);
        var ToDate = this._CommonService.GetFormatDate(this.model.ToDate);
        this.Refresh(fDate, ToDate);       
    }

    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    GetList() {
        //this.Refresh();
    }
    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
    }
    LoadSearchableDropdown() {
        //Search By Name
        $('#PatientSearchByName').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.searchByName(request.term).then(m => {
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.AppointmentModel.PatientName = ui.item.label;
                this.AppointmentModel.PatientId = ui.item.value;
                this.AppointmentModel.CNIC = ui.item.CNIC;
                this.AppointmentModel.Mobile = ui.item.Phone;
            }
        });
    }
    CalAmount() {
        if (this.AppointmentModel.Discount == undefined || this.AppointmentModel.Discount == null)
            this.AppointmentModel.Discount = 0;
        if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null)
            this.AppointmentModel.Price = 0;

        this.AppointmentModel.PaidAmount = this.AppointmentModel.Price - this.AppointmentModel.Discount;
        this.AppointmentModel.OutstandingBalance = this.AppointmentModel.Price - this.AppointmentModel.Discount - this.AppointmentModel.PaidAmount;

    }
    PaidCalAmount() {
        if (this.AppointmentModel.Discount == undefined || this.AppointmentModel.Discount == null)
            this.AppointmentModel.Discount = 0;
        if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null)
            this.AppointmentModel.Price = 0;
        this.AppointmentModel.OutstandingBalance = this.AppointmentModel.Price - this.AppointmentModel.Discount - this.AppointmentModel.PaidAmount;
    }
    onPatientNameEvent(event: any) {
        if (event != null && event != undefined) {
            

        }
    }
    AppointmentSaveOrUpdate(isValid: boolean): void {
        let CurrentDate = this._CommonService.GetFormatDate(new Date());
        let Appdate = this._CommonService.GetFormatDate(this.AppointmentModel.AppointmentDate);
        if (Appdate < CurrentDate) {
            this.toastr.Error("You can not add appointment previous date.");
            return;
        }
        this.submitted = true; 
        if (this.AppointmentModel.PatientId == undefined || this.AppointmentModel.PatientId == null) {
            this.toastr.Error("Error", "Please select patient.");
            return;
        }
        if (this.AppointmentModel.AppointmentDate == undefined || this.AppointmentModel.AppointmentDate == null) {
            this.toastr.Error("Error", "Please select appointment date.");
            return;
        }
        if (this.Rights.indexOf(12) > -1) {
            if (this.AppointmentModel.ServiceId == undefined || this.AppointmentModel.ServiceId == null) {
                this.toastr.Error("Error", "Please select service.");
                return;
            }
            if (this.AppointmentModel.BillDate == undefined || this.AppointmentModel.BillDate == null) {
                this.toastr.Error("Error", "Please select bill date.");
                return;
            }
            if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null) {
                this.toastr.Error("Error", "Please select price.");
                return;
            }
            if (this.AppointmentModel.AppointmentTime == undefined || this.AppointmentModel.AppointmentTime == null) {
                this.toastr.Error("Error", "Please select time.");
                return;
            }
        }
        this.submitted = false;
        this.loader.ShowLoader();
        if (this.AppointmentModel.AppointmentTime == undefined)
            this.AppointmentModel.AppointmentTime = $('#basicExample').val();
        this.AppointmentModel.AppointmentTime = this.Convert12TO24(this.AppointmentModel.AppointmentTime);
        this._AppointmentService.AppSaveOrUpdate(this.AppointmentModel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.modalService.dismissAll(this.ModelContent);
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    Convert12TO24(time12h: any) {
        const [time, modifier] = time12h.split(' ');

        let [hours, minutes] = time.split(':');
        if (modifier === 'pm') {
            hours = parseInt(hours, 10) + 12;
        }

        return hours + ':' + minutes + ':00';
    }
    SelectTime() {
        if (this.AppointmentModel.DoctorId != null && this.AppointmentModel.DoctorId != undefined) {
            var doctorid = this.DoctorList.filter(a => a.ID == this.AppointmentModel.DoctorId);
            let minTime = this.formatAMPM(doctorid[0].StartTime);
            let maxTime = this.formatAMPM(doctorid[0].EndTime);
            if (doctorid[0].SlotTime != null) {
                this.Step = parseInt(doctorid[0].SlotTime.split(':')[1]);
            }
           
            $("#basicExample").timepicker({
                'minTime': minTime,
                'maxTime': maxTime,
                'step': this.Step,
                'timeFormat': 'h:i A'
            });
            //}
            if ($("#basicExample").val() != null && $("#basicExample").val() != "")
                this.AppointmentModel.AppointmentTime = $("#basicExample").val();
        }
        else {
            this.toastr.Error("please select doctor.")
        }
    }
    formatAMPM(time: any) {
        var hours = time.split(':')[0];
        var minutes = time.split(':')[1];
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }
    MoveToIncome(value: string) {
        //this._AsidebarService.SetClickValue(value);
        this._router.navigate(['/home/Income'], { queryParams: { id: value } });

        //this._router.navigate(['/home/Income']);
    }
}
