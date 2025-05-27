import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { PaymentModel } from './PaymentModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { PaymentService } from './PaymentService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { Observable } from 'rxjs';
import { IMyDateModel } from 'mydatepicker';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';

declare var $: any;
@Component({
    templateUrl: './ItemPaymentComponentForm.html',
    moduleId: module.id,
    providers: [PaymentService],
})
export class ItemPaymentComponentForm implements OnInit {
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
    public model = new PaymentModel();
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public PaymentMethodList: any[] = [];
    public Rights: any;
    public ControlRights: any;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public _fb: FormBuilder, public loader: LoaderService
        , public _PaymentService: PaymentService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("64");
    }
    ngOnInit() {
        this.Refresh();
        this.Form1 = this._fb.group({
            PaymentDate: ['', [Validators.required]],
            PaymentMethodID: ['', [Validators.required]],
            Amount: ['', [Validators.required]],
            Notes: ['', [Validators.required]],
            InvoiveId: [''],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._PaymentService.GetById(this.model.ID).then(m => {
                        this.model = m.ResultSet.result;
                        this.loader.HideLoader();
                    });
                }
            });
    }
    Refresh() {
        this.loader.ShowLoader();
        this._PaymentService.FormLoad().then(m => {
            this.PaymentMethodList = m.list;
            this.loader.HideLoader();
        });
    }
    onDateChanged(event: IMyDateModel) {
        if (event) {

        }
    }

    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._PaymentService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['/home/Payment']);
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
            this._PaymentService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/Payment']);
                this.loader.HideLoader();
                //this.WasInside = true;
                //this.closeModal.nativeElement.click();
            });
        }
    }
    onDOBChanged(event: IMyDateModel) {
        if (event) {
        }
    }
    Close() {
        this.model = new PaymentModel();
        this.submitted = false;
        this.IsEmpExist = false;
        this.IsAdmin = false;
        this.IsUpdateText = false;
    }
}
