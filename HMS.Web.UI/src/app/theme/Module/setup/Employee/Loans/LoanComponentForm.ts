import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoanModel } from './LoanModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { LoanService } from './LoanService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService'
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

@Component({
    selector: 'LoanComponentForm',
    templateUrl: './LoanComponentForm.html',
    moduleId: module.id,
    providers: [LoanService],
})

export class LoanComponentForm implements OnInit, OnChanges {
    public Form1: FormGroup; // our model driven form
    public submitted: boolean = false; // keep track on whether form is submitted
    public EmpfilteredList = [];
    public PaymentMethodList: any[] = [];
    public LoanTypeList: any[] = [];
    public keyword: string;
    public DisplayMember: string;
    public BasicSalary: number = 0;
    public IsEmpFound: boolean = false;
    public IsValidLoanValue: boolean = false;
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public PayrollRegion: string;
    public Currency: string;
    public IsReadOnly = false;
    public DefaultPamentMethodID: number = 1;
    public DefaultLoanTypeID: number = 1;
    public model = new LoanModel();
    public Rights: any;
    public ControlRights: any;
    @ViewChild('closeModal') closeModal: ElementRef;
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
        , public _loanservice: LoanService, public toastr: CommonToastrService, public _commonService: CommonService,
        private encrypt: EncryptionService) {
        this.Currency = this._commonService.getCurrency();
        this.PayrollRegion = this._commonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._commonService.ScreenRights("60");
    }

    ngOnInit() {
        this.Form1 = this._fb.group({
            EmployeeID: ['', [Validators.required]],
            Description: ['', [Validators.required]],
            PaymentMethodID: ['', [Validators.required]],
            PaymentStartDate: ['', [Validators.required]],
            LoanDate: ['', [Validators.required]],
            LoanAmount: ['', [Validators.required]],
            DeductionType: ['', [Validators.required]],
            DeductionValue: ['', [Validators.required]],
            InstallmentByBaseSalary: [''],
            LoanTypeDropdownID: [''],
            LoanTypeID: [''],
            LoanCode: [''],
        });

        //loading all dropdowns
        this._commonService.LoadDropdown("41,56").then(m => {
            if (m.IsSuccess) {
                this.PaymentMethodList = m.ResultSet.filter(a => a.DropDownID == 41);;
                this.LoanTypeList = m.ResultSet.filter(a => a.DropDownID==56);
                this.model.PaymentMethodID = this.DefaultPamentMethodID;
                this.model.LoanTypeID = this.DefaultLoanTypeID;
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
            this._loanservice.GetById(this.id).then(m => {
                this.model = m.ResultSet;
                this.loader.HideLoader();
            });
        }
    }

    MatchesFound() {
        if (this.model.EmployeeID == undefined) {
            this.DisplayMember = ''; this.model.EmployeeID = undefined; this.BasicSalary = 0;
        }
    }
    //MatchesFound() {
    //    if (this.EmpfilteredList.length <= 0) { this.DisplayMember = ''; this.BasicSalary = 0; this.model.EmployeeID = undefined; }
    //}

    FilterEmployees() {
        var self = this;
        this.model.EmployeeID = undefined;
        this.keyword = self.DisplayMember.toString();
        if (this.keyword.length >= 2) {
            this._loanservice.GetFilterEmployees(this.keyword).then((matches: any) => {
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
            this.model.EmployeeID = item.ID;
            this.DisplayMember = item.FirstName + ' ' + item.LastName;
            this.BasicSalary = item.BasicSalary;
            this.EmpfilteredList = [];
            this.IsEmpFound = true;
        }
    }

    IsLoanValid() {
        if (this.model.DeductionType == 'P') {
            this.IsValidLoanValue = this.model.DeductionValue > 100 ? true : false;
            this.model.InstallmentByBaseSalary = (this.model.DeductionValue > 100) ? 0 : Math.round((this.model.DeductionValue / 100) * this.BasicSalary);
        } else this.model.InstallmentByBaseSalary = 0;
    }

    IsValidFormEntries(): boolean {
        var payStartDate = new Date(this.model.PaymentStartDate);
        var LoanDate = new Date(this.model.LoanDate);
        if (payStartDate < LoanDate) {
            this.toastr.Error('Invalid Date', 'Payment start date should be greater than or equal to the loan date');
            return false;
        } else if (this.model.LoanAmount <= 0) {
            this.toastr.Error('Invalid Amount', 'Loan amount should be greater than 0');
            return false;
        }
        var value = this.model.DeductionType == 'F' ? this.model.DeductionValue : this.model.InstallmentByBaseSalary;
        if (this.model.LoanAmount < value) {
            this.toastr.Error('Invalid Amount', 'Loan amount should be greater than or equal to the payment installment amount');
            return false;
        }

        return true;
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true

        if (isValid)
            isValid = this.IsValidFormEntries();

        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._loanservice.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    this.WasInside = true;
                    this.closeModal.nativeElement.click();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }

    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._loanservice.Delete(this.model.ID.toString()).then(m => {
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

        this.model = new LoanModel();
        this.DisplayMember = '';
        this.BasicSalary = 0;
        this.model.PaymentMethodID = this.DefaultPamentMethodID;
        this.submitted = false;

    }
}