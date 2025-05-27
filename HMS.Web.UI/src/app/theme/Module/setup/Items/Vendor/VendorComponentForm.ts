import { FormGroup, FormControl, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Component, OnInit, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { VendorService } from './VendorService';
import { VendorModel } from './VendorModel';
import { ActivatedRoute } from '@angular/router';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { filter } from 'rxjs/operators';

@Component({
    moduleId: module.id,
    templateUrl: 'VendorComponentForm.html',
    providers: [VendorService],
})

export class VendorComponentForm {
    public model = new VendorModel();
    public Id: string;
    public submitted: boolean;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public sub: any;
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    public Image: string = '';
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _VendorService: VendorService, public _fb: FormBuilder
        , public route: ActivatedRoute, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        this.ControlRights = this._CommonService.ScreenRights("66");
        this.PayrollRegion = this._CommonService.getPayrollRegion();
    }
    ngOnInit() {
        this.LoadDropDown();
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._VendorService.GetById(this.model.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.model = m.ResultSet.result;
                            this.loader.HideLoader();
                        } else
                            this.loader.HideLoader();
                    });
                }
            });
        this.Form1 = this._fb.group({
            FirstName: ['', [Validators.required]],
            LastName: ['', [Validators.required]],
            CompanyName: [''],
            VendorPhone: ['', [Validators.required]],
            VendorEmail: [''],
            Address: [''],
            Address2: [''],
            City: [''],
            State: [''],
            ZipCode: [''],
            Phone: [''],
            Fax: ['']
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._VendorService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['/home/Vendor']);
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
        else {
            this.toastr.Error('Error', 'Please select all mandatory field.');
        }
    }
    LoadDropDown() {
        this.loader.ShowLoader();
        this._VendorService.LoadDropdown("57").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet.dropdownValues;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._VendorService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    this.toastr.Error('Error', m.ErrorMessage);
                    this.loader.HideLoader();
                } else {
                    this._router.navigate(['/home/Vendor']);
                }
            });
        }
    }
}