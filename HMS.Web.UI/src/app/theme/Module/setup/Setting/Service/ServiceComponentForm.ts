import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { emr_service_mf, emr_service_item } from '../../Setting/Service/ServiceModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { Service } from './Service';
import { SaleService } from './../../Items/Sale/SaleService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
declare var $: any;
@Component({
    templateUrl: './ServiceComponentForm.html',
    moduleId: module.id,
    providers: [Service, SaleService],
})
export class ServiceComponentForm implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public ScreenLists = [];
    public SpecialityList: any[] = [];
    public model = new emr_service_mf();
    public IsAdmin: boolean = false;
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public itemdynamicArray = [];
    public Rights: any;
    public ControlRights: any;
    public isChecked: boolean = false;
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public loader: LoaderService
        , public _Service: Service, private encrypt: EncryptionService, public commonservice: CommonService
        , public toastr: CommonToastrService, public route: ActivatedRoute, public _router: Router,
        public _SaleService: SaleService) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("65");
    }
    ngOnInit() {
        this.loadDropdown();
        this.Form1 = this._fb.group({
            ServiceName: ['', [Validators.required]],
            Price: [''],
            IsItem: [''],
            RefCode: [''],
            SpecialityId: ['']
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._Service.GetById(this.model.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.model = m.ResultSet.Result;
                            const itemList = m.ResultSet.itemList;
                            this.model.emr_service_item.forEach((item, index) => {
                                let itemObj = itemList.filter(x => x.ID == item.ItemId);
                                item.ItemName = itemObj[0].Name;
                                this.itemdynamicArray.push(item);
                            });
                            if (this.itemdynamicArray.length > 0)
                                this.isChecked = true;
                            else
                                this.isChecked = false;
                        }
                        this.loader.HideLoader();
                    });
                } else {

                }
            });
    }
    loadDropdown() {
        this.loader.ShowLoader();
        this._Service.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.SpecialityList = m.ResultSet.Speciality;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            if (this.itemdynamicArray[0].ItemId != 0) {
                this.model.emr_service_item = this.itemdynamicArray;
            }
            this._Service.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['/home/Service']);
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    Add() {
        var obj = new emr_service_item();
        this.itemdynamicArray.push(obj);
    }
    toggleDiv(event: any) {
        this.isChecked = event.target.checked;
        if (this.itemdynamicArray.length == 0)
            this.Add();
    }
    RemoveRow(rowno: any) {
        if (this.itemdynamicArray.length > 1) {
            this.itemdynamicArray.splice(rowno, 1);
        }
    }
    LoadItem(val: any) {
        //Search By Name
        $('#txtDrug_' + val).autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._SaleService.ServiceItemByName(request.term).then(m => {
                    const items = m.ResultSet.ItemInfo.map(item => ({
                        label: item.label + " (qty:" + item.stock + ")",
                        value: item.value,
                        CostPrice: item.CostPrice,
                        ItemTypeId: item.Type,
                        stock: item.stock,
                        CategoryId: item.CategoryId,
                        TypeValue: item.TypeValue,
                    }));
                    response(items);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                if (ui.item.stock == 0) {
                    this.toastr.Error("Error", "Stock must be greater than zero");
                    ui.item.value = "";
                    ui.item.label = "";
                    return;
                }
                this.itemdynamicArray[val].ItemName = ui.item.label;
                this.itemdynamicArray[val].ItemId = ui.item.value;
                this.itemdynamicArray[val].Quantity = 1;
            }
        });
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._Service.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['/home/Service']);
                this.loader.HideLoader();
            });
        }
    }
    Close() {
        this.model = new emr_service_mf();
        this.submitted = false;
        this.IsAdmin = false;
    }
}
