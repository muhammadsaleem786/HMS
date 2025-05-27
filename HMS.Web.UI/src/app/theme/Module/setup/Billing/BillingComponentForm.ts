import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { emr_patient_bill } from './BillingModel';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { BillingService } from './BillingService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { Observable } from 'rxjs';
import { IMyDateModel } from 'mydatepicker';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { PatientService } from './../Patient/PatientService';
declare var $: any;
@Component({
    templateUrl: './BillingComponentForm.html',
    moduleId: module.id,
    providers: [BillingService, PatientService],
})
export class BillingComponentForm implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = [];
    public IsSameModuleName: string;
    public IsChecked: boolean;
    public IsEmpExist: boolean = false;
    public IsAdmin: boolean = false;
    public IsUpdateText: boolean = false;
    public model = new emr_patient_bill();
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public Rights: any;
    public ControlRights: any;

    @HostListener('document:keydown.control.s', ['$event'])

    onKeydownHandler(event: KeyboardEvent) {
        event.preventDefault();
        (this.Form1.value as { submitted: boolean }).submitted = true;
        this.Form1.controls.ngSubmit.value.emit(this.Form1);
    }

    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _fb: FormBuilder, public loader: LoaderService, public _PatientService: PatientService
        , public _BillingService: BillingService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion =this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("12");
    }
    ngOnInit() {
        this.Refresh();
        this.Form1 = this._fb.group({
            AppointmentId: [''],
            PatientId: ['', [Validators.required]],
            ServiceId: ['', [Validators.required]],
            BillDate: ['', [Validators.required]],
            Price: ['', [Validators.required]],
            Discount: [''],
            PaidAmount: ['', [Validators.required]],
            PatientName: [''],
            DoctorName: [''],
            ServiceName: [''],
            DoctorId: ['', [Validators.required]],
            OutstandingBalance: [''],
            Remarks: ['']
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._BillingService.GetById(this.model.ID).then(m => {
                        this.model = m.ResultSet.result;
                        this.loader.HideLoader();
                    });
                } else {
                    this.model.BillDate = new Date();
                }
            });
    }
    Refresh() {
        //this.loader.ShowLoader();
        //this._BillingService.FormLoad().then(m => {

        //    this.loader.HideLoader();
        //});
    }
    onDateChanged(event: IMyDateModel) {
        if (event) {

        }
    }

    LoadPatient() {
        $("#PatientSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._BillingService.PatientSearch(request.term).then(m => {

                    if (m.ResultSet.result.length == 0)
                        $("#PatientSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.PatientId = ui.item.value;
                this.model.PatientName = ui.item.label;

                return ui.item.label;
            }
        });
    }

    LoadService() {
        $("#ServiceSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._BillingService.ServiceSearch(request.term).then(m => {
                    if (m.ResultSet.result.length == 0)
                        $("#ServiceSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.ServiceId = ui.item.value;
                this.model.ServiceName = ui.item.label;
                this.model.Price = ui.item.Price;
                this.CalAmount();
                return ui.item.label;
            }
        });
    }
    LoadDoctor() {
        $("#DoctorSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._PatientService.DoctorSearch(request.term).then(m => {

                    if (m.ResultSet.result.length == 0)
                        $("#DoctorSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.DoctorId = ui.item.value;
                this.model.DoctorName = ui.item.label;

                return ui.item.label;
            }
        });
    }

    CalAmount() {
        if (this.model.Discount == undefined || this.model.Discount == null)
            this.model.Discount = 0;
        if (this.model.Price == undefined || this.model.Price == null)
            this.model.Price = 0;

        this.model.PaidAmount = this.model.Price - this.model.Discount;
        this.model.OutstandingBalance = this.model.Price - this.model.Discount - this.model.PaidAmount;
    }
    PaidCalAmount() {
        if (this.model.Discount == undefined || this.model.Discount == null)
            this.model.Discount = 0;
        if (this.model.Price == undefined || this.model.Price == null)
            this.model.Price = 0;
        this.model.OutstandingBalance = this.model.Price - this.model.Discount - this.model.PaidAmount;
    }


    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (this.model.DoctorId == 0 || this.model.DoctorId == null) {
            this.toastr.Error("Error", "please select coctor.");
            return;
        }
        if (this.model.PatientId == 0 || this.model.PatientId == null) {
            this.toastr.Error("Error", "please select Patient.");
            return;
        }
        if (this.model.Discount > this.model.Price) {
            this.toastr.Error("Error", "discount not greater price amount.");
            return;
        }
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._BillingService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['/home/Billing']);
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
        if (result) {
            this.loader.ShowLoader();
            this._BillingService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/Billing']);
                this.loader.HideLoader();
                //this.WasInside = true;
                //this.closeModal.nativeElement.click();
            });
        }
    }
    Close() {
        this.model = new emr_patient_bill();
        this.submitted = false;
        this.IsEmpExist = false;
        this.IsAdmin = false;
        this.IsUpdateText = false;
    }
}
