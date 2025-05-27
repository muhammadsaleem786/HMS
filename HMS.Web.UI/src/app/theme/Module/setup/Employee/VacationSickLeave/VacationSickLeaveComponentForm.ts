import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { VacationSickLeaveModel } from './VacationSickLeaveModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { VacationSickLeaveService } from './VacationSickLeaveService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    selector: 'setup-VacationSickLeaveComponentForm',
    templateUrl: './VacationSickLeaveComponentForm.html',
    moduleId: module.id,
    providers: [VacationSickLeaveService],
})

export class VacationSickLeaveComponentForm implements OnInit, OnChanges {
    public AccrualFrequencyTypes = [];
    public DefaulAccrualFrequencyID: any;
    public DefaulCategory: string ;
    public DefaulEarnedValue: number;
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public IsEdit: boolean = false;
    public Rights: any;
    public ControlRights: any;
    public PayrollRegion: string;
    public Currency: string;
    public model = new VacationSickLeaveModel();
    @ViewChild('closeModal') closeModal: ElementRef;

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

    constructor(public _fb: FormBuilder, public loader: LoaderService, public _commonService: CommonService,
        public commonservice: CommonService, public _vacationSickLeaveService: VacationSickLeaveService
        , public toastr: CommonToastrService, private encrypt: EncryptionService) {
        this.Currency = this._commonService.getCurrency();
        this.PayrollRegion = this._commonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._commonService.ScreenRights("60");
    }

    ngOnInit() {

        this.Form1 = this._fb.group({
            TypeName: ['', [Validators.required]],
            EarnedValue: ['', [Validators.required]],
            Category: [''],
            AccrualFrequencyID: [''],
        });

        this.DefaulCategory = "V";
        this.DefaulEarnedValue = 80;
        this.model.Category = this.DefaulCategory;
        this.model.EarnedValue = this.DefaulEarnedValue;

        //loading all dropdowns
        //this.loader.ShowLoader();
        this.commonservice.LoadDropdown("11").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet;
                this.AccrualFrequencyTypes = list.filter(f => f.DropDownID == 11);
                this.DefaulAccrualFrequencyID = this.AccrualFrequencyTypes[1].ID;
                this.model.AccrualFrequencyID = this.DefaulAccrualFrequencyID;
            }
        });
        //this.loader.HideLoader();
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
            this._vacationSickLeaveService.GetById(this.id).then(m => {
                this.model = m.ResultSet;
                this.loader.HideLoader();
            });
        }
    }

  

    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._vacationSickLeaveService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    //alert(result.Message);
                    this.closeModal.nativeElement.click();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error',result.ErrorMessage);
                }
            });
        }
    }

    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._vacationSickLeaveService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error',m.ErrorMessage);
                
                this.closeModal.nativeElement.click();
            });
        }
    }

    Close() {
        if (!this.WasInside)
            this.pageClose.emit(0);
        else
            this.pageClose.emit(1);

        this.model = new VacationSickLeaveModel();
        this.model.AccrualFrequencyID = this.DefaulAccrualFrequencyID;
        this.model.Category = this.DefaulCategory;
        this.model.EarnedValue = this.DefaulEarnedValue;
        this.submitted = false;
       
    }

}
