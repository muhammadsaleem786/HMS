import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Payschedule } from './PayScheduleModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { PayscheduleService } from './PayScheduleService';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { IMyDpOptions, IMyDateModel, MyDatePicker } from 'mydatepicker';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

@Component({
    selector: 'app-payschedule',
    templateUrl: './PayScheduleComponentForm.html',
    moduleId: module.id,
    providers: [PayscheduleService],

})

export class PayScheduleComponentForm implements OnInit, OnChanges {
    public Paytypes = [];
    public FallInHolidaytypes = [];
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    @Input('show-modal') showModal: boolean;
    public IsReadOnly = false;
    public ChkState: boolean = false;
    public model = new Payschedule();
    public DefaultPayTypeID: number;
    public DefaultFallInHoliday: number;
    @ViewChild('closeModal') closeModal: ElementRef;
    @ViewChild('mydp') mydp: MyDatePicker;
    public Keywords: any[] = [];
    public Rights: any;
    public ControlRights: any;
    public WasInside: boolean = false;
    @HostListener('click', ['$event'], )
    Clickoutdocument(event) {
        if (!this.WasInside)
            this.Close();

        this.WasInside = false;
    }

    IsModalClick() {
        this.WasInside = true;
    }





    constructor(public _fb: FormBuilder, public loader: LoaderService
        , public _payScheduleService: PayscheduleService, public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService) {
        //this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        //this.ControlRights = this.commonservice.ScreenRights("56");
    }

    ngOnInit() {

        this.Form1 = this._fb.group({
            ScheduleName: ['', [Validators.required]],
            PeriodStartDate: ['', [Validators.required]],
            PeriodEndDate: [''],
            PayDate: ['', [Validators.required]],
            PayTypeID: [''],
            FallInHolidayID: [''],
            Active: [''],
        });


        //loading all dropdowns
        this.commonservice.LoadDropdown("6,13").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet;
                this.Paytypes = list.filter(f => f.DropDownID == 6);
                this.FallInHolidaytypes = list.filter(f => f.DropDownID == 13);
                this.model.FallInHolidayID = this.FallInHolidaytypes[0].ID;
                this.DefaultPayTypeID = this.Paytypes[0].ID;
                this.DefaultFallInHoliday = this.FallInHolidaytypes[0].ID;
                this.model.PayTypeID = this.DefaultPayTypeID;
                this.model.FallInHolidayID = this.DefaultFallInHoliday;
            }
        });
    }

    ngOnChanges() {
        if (typeof (this.id) == "undefined") return;
        this.model.PopUpVisible = true;
        if (isNaN(this.id)) {
            this.IsReadOnly = true;
            this.id = +this.id.toString().substring(1); // (+) converts string 'id' to a number
        }
        else {
            this.id = +this.id; // (+) converts string 'id' to a number
            this.IsReadOnly = false;
        }

        if (this.id != 0) {
            this._payScheduleService.GetById(this.id).then(m => {
                this.model = m.ResultSet;
                if (this.model.Active)
                    this.ChkState = true;

                this.model.PopUpVisible = true;
                this.loader.HideLoader();
            });
        }
    }


    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._payScheduleService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);

                this.WasInside = true;
                this.closeModal.nativeElement.click();
            });
        }
    }


    onDateChanged(event: IMyDateModel) {

        var date = new Date(event.jsdate);
        if (date.getMonth() != 12) {
            date.setMonth(date.getMonth() + 1);
        } else {
            date.setMonth(1);
            date.setFullYear(date.getFullYear() + 1);
        }
        date.setDate(date.getDate() - 1);
        this.model.PeriodEndDate = new Date(this.GetFormatDate(date));

    }

    GetFormatDate(date) {
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getMonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return yyyy + '-' + mm + '-' + dd;
    };

    //onToggleSelector(event: any) {
    //    event.stopPropagation();
    //    this.mydp.openBtnClicked();
    //}



    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true

        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._payScheduleService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.WasInside = true;
                    this.closeModal.nativeElement.click();
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }

    Close() {
        if (!this.WasInside)
            this.pageClose.emit(0);
        else
            this.pageClose.emit(1);

        this.model = new Payschedule();
        this.submitted = false;
        //setting default value of dropdown
        this.model.FallInHolidayID = this.DefaultFallInHoliday;
        this.model.PayTypeID = this.DefaultPayTypeID;
        this.ChkState = false;
    }

    getPayDate(event: IMyDateModel): void {
        var date = new Date(event.jsdate);
        var day = date.getDay();
        if (this.model.FallInHolidayID == 1) {
            if (date.getDay() == 0) {
                date.setDate(date.getDate() - 2);
            }
            else if (date.getDay() == 6) {
                date.setDate(date.getDate() - 1);
            }
        } else {
            if (date.getDay() == 0)
                date.setDate(date.getDate() + 1);
            else if (date.getDay() == 6)
                date.setDate(date.getDate() + 2);
        }

        this.model.PayDate = date;
    }
}
