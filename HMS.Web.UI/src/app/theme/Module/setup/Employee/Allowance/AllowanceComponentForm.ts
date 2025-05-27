import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { AllowanceModel } from './AllowanceModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { AllowanceService } from './AllowanceService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
@Component({
    selector: 'setup-AllowanceComponentForm',
    templateUrl: './AllowanceComponentForm.html',
    moduleId: module.id,
    providers: [AllowanceService, FormBuilder],
})

export class AllowanceComponentForm implements OnInit, OnChanges {
    public AllowanceCategorytypes = [];

    public PayrollRegion: string;
    public DefaulCategoryID: number;
    public DefaulAllowanceType: string;
    public DefaultTaxable: boolean = true;
    public IsValidAllowValue: boolean = false;
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public IsOther: boolean = false;
    public IsEdit: boolean = false;
    public OtherID: number = 0;
    public Currency: string;
    public model = new AllowanceModel();
    @ViewChild('closeModal') closeModal: ElementRef;
    public Rights: any;
    public ControlRights: any;
    public WasInside: boolean = false;
    @HostListener('click', ['$event'],)
    Clickoutdocument(event) {
        if (!this.WasInside)
            this.Close();

        this.WasInside = false;
    }

    IsModalClick() {
        this.WasInside = true;
    }

    constructor(public _fb: FormBuilder, public loader: LoaderService
        , public commonservice: CommonService, public _allowanceService: AllowanceService
        , public toastr: CommonToastrService, private encrypt: EncryptionService,) {
        this.Currency = this.commonservice.getCurrency();
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this.commonservice.ScreenRights("51");
    }

    ngOnInit() {

        this.Form1 = this._fb.group({
            AllowanceName: ['', [Validators.required]],
            AllowanceValue: ['', [Validators.required]],
            AllowanceType: [''],
            Taxable: [''],
            Default: [''],
            CategoryID: [''],
        });

        this.DefaulAllowanceType = 'F';
        this.model.AllowanceName = 'a'
        this.model.Taxable = this.DefaultTaxable;
        this.model.AllowanceType = this.DefaulAllowanceType;
        //loading all dropdowns
        this.commonservice.LoadDropdown("52").then(m => {
            if (m.IsSuccess) {
                this.AllowanceCategorytypes = m.ResultSet;
                //this.OtherID = this.AllowanceCategorytypes.filter(x => { return x.ID == 7 })[0].ID;
                this.DefaulCategoryID = this.AllowanceCategorytypes[0].ID;
                this.model.CategoryID = this.DefaulCategoryID;
            }
        });
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
            this._allowanceService.GetById(this.id).then(m => {
                
                this.model = m.ResultSet;
                if (this.model.CategoryID == this.OtherID) {
                    this.IsOther = true;
                }
                this.loader.HideLoader();
            });
        }
        this.PayrollRegion = this.encrypt.decryptionAES(localStorage.getItem("PayrollRegion"));
    }

    IsAllowanceValid() {
        if (this.model.AllowanceType == 'P' && this.model.AllowanceValue > 100)
            this.IsValidAllowValue = true;
        else
            this.IsValidAllowValue = false;
    }

    SelectionChange() {
        if (this.model.CategoryID == this.OtherID) {
            this.IsOther = true;
            this.model.AllowanceName = '';
        }
        else {
            this.IsOther = false;
            this.model.AllowanceName = 'a';
        }
    }

    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.IsAllowanceValid();
            isValid = !this.IsValidAllowValue;
        }
        if (isValid) {
            if (this.model.AllowanceName === 'a' && this.model.CategoryID != this.OtherID)
                this.model.AllowanceName = '';

            this.submitted = false;
            this.loader.ShowLoader();
            if (this.PayrollRegion == 'SA')
                this.model.Taxable = true;

            this._allowanceService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.WasInside = true;
                    this.closeModal.nativeElement.click();
                }
                else {
                    this.SelectionChange();
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
            this._allowanceService.Delete(this.model.ID.toString()).then(m => {
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

        this.model = new AllowanceModel();
        this.model.CategoryID = this.DefaulCategoryID;
        this.model.Taxable = this.DefaultTaxable;
        this.model.AllowanceType = this.DefaulAllowanceType;
        this.submitted = false;
        this.IsOther = false;
        this.model.AllowanceName = 'a';
    }
}
