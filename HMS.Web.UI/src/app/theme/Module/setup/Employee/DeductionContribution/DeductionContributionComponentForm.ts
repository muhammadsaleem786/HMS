import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { DeductionContributionModel } from './DeductionContributionModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { DeductionContributionService } from './DeductionContributionService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

@Component({
    selector: 'setup-DeductionContributionComponentForm',
    templateUrl: './DeductionContributionComponentForm.html',
    moduleId: module.id,
    providers: [DeductionContributionService, FormBuilder],
})

export class DeductionContributionComponentForm implements OnInit, OnChanges {
    public DefaulCategory: string;
    public DefaulDeductionContributionType: string;
    public IsValidAllowValue: boolean;
    public IsTaxable: boolean = false;
    public IsEdit: boolean = false;
    public Form1: FormGroup; // our model driven form
    public submitted: boolean = false; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public IsAdmin: boolean = false;
    public Currency: string;
    public PayrollRegion: string;
    public model = new DeductionContributionModel();
    @ViewChild('closeModal') closeModal: ElementRef;
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


    constructor(public _fb: FormBuilder, public loader: LoaderService,
        public commonservice: CommonService, public _deductionContributionService: DeductionContributionService
        , public toastr: CommonToastrService, private encrypt: EncryptionService) {
        this.Currency = this.commonservice.getCurrency();
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this.commonservice.ScreenRights("52");
    }

    ngOnInit() {

        this.Form1 = this._fb.group({
            DeductionContributionName: ['', [<any>Validators.required]],
            DeductionContributionValue: ['', [Validators.required]],
            Category: [''],
            Taxable: [''],
            DeductionContributionType: [''],
            Default: [''],
        });

        this.DefaulCategory = "C";
        this.DefaulDeductionContributionType = "F";
        this.model.Category = this.DefaulCategory;
        this.model.DeductionContributionType = this.DefaulDeductionContributionType;
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
            this._deductionContributionService.GetById(this.id).then(m => {
                this.model = m.ResultSet;
                this.IsAdmin = this.model.SystemGenerated ? true : false;
                this.loader.HideLoader();
            });
        }
    }

    IsDedContValValid() {
        if (this.model.DeductionContributionType == "P" && this.model.DeductionContributionValue > 100)
            this.IsValidAllowValue = true;
        else
            this.IsValidAllowValue = false;
    }


    getState() {
        if (this.model.Category == "D")
            this.IsTaxable = true;
        else
            this.IsTaxable = false;
    }

    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.IsDedContValValid();
            isValid = !this.IsValidAllowValue;
        }
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();

            if (this.PayrollRegion == 'SA' && this.model.Category == 'D')
                this.model.Taxable = true;

            this._deductionContributionService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess == true) {
                    this.WasInside = true;
                    this.closeModal.nativeElement.click();
                }
                else {
                    this.toastr.Error('Error',result.ErrorMessage);
                }
                this.loader.HideLoader();
            });
        }
    }

    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._deductionContributionService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error',m.ErrorMessage);

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

        this.model = new DeductionContributionModel();
        this.model.Category = this.DefaulCategory;
        this.model.DeductionContributionType = this.DefaulDeductionContributionType;
        this.submitted = false;
        this.IsAdmin = false;

    }

}
