import { FormGroup, FormControl, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Component, OnInit, ViewChild, ElementRef, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { ItemService } from './ItemService';
import { ItemModel } from './ItemModel';
import { ActivatedRoute } from '@angular/router';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { filter } from 'rxjs/operators';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
@Component({
    moduleId: module.id,
    templateUrl: 'ItemComponentForm.html',
    providers: [ItemService],
})
export class ItemComponentForm {
    public model = new ItemModel();
    public Id: string;
    public submitted: boolean;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public unitList: any[] = [];
    public CategoryList: any[] = [];
    public ItemTypeList: any[] = [];
    public GroupList: any[] = [];
    public InstructionsList: any[] = [];
    public PayrollRegion: string;
    public sub: any;
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    public Image: string = '';
    public IsNewImage: boolean = true;
    public isDisabled: boolean = false;
    public IsTrackInventory: boolean = false;
    public AttachImage: string = '';
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _ItemService: ItemService, public _fb: FormBuilder
        , public route: ActivatedRoute, public toastr: CommonToastrService,
        private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("61");
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
                    this._ItemService.GetById(this.model.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.model = m.ResultSet.result;
                            if (this.model.Image != null && this.model.Image != undefined && this.model.Image != "") {
                                this.IsNewImage = true;
                                this.getImageUrlName(this.model.Image);
                            }
                            if (this.model.TrackInventory === true)
                                this.IsTrackInventory = true;
                            else
                                this.IsTrackInventory = false;
                            if (this.model.SaveStatus == 2) {
                                this.Form1.get('ItemTypeId').disable();
                                this.Form1.get('Name').disable();
                                this.Form1.get('SKU').disable();
                                this.Form1.get('UnitID').disable();
                                this.Form1.get('CategoryID').disable();
                                this.Form1.get('TrackInventory').disable();
                                this.Form1.get('POSItem').disable();
                                this.Form1.get('InventoryOpeningStock').disable();
                                this.Form1.get('InventoryStockPerUnit').disable();
                                this.Form1.get('GroupId').disable();
                            } else {
                                this.Form1.get('GroupId').enable();
                                this.Form1.get('ItemTypeId').enable();
                                this.Form1.get('Name').enable();
                                this.Form1.get('SKU').enable();
                                this.Form1.get('UnitID').enable();
                                this.Form1.get('CategoryID').enable();
                                if (this.model.SaveStatus == 1 && this.model.ItemTypeId == 2) {
                                    this.Form1.get('TrackInventory').disable();
                                } else {
                                    this.Form1.get('TrackInventory').enable();
                                }
                                this.Form1.get('InventoryOpeningStock').enable();
                                this.Form1.get('InventoryStockPerUnit').enable();
                            }
                            this.loader.HideLoader();
                        } else
                            this.loader.HideLoader();
                    });
                }
            });
        this.Form1 = this._fb.group({
            ItemTypeId: [''],
            Name: [''],
            SKU: [''],
            Image: [''],
            UnitDropDownID: [''],
            UnitID: [''],
            CategoryDropDownID: [''],
            CategoryID: [''],
            InstructionId: [''],
            CostPrice: [''],
            SalePrice: [''],
            TrackInventory: [''],
            InventoryOpeningStock: [''],
            InventoryStockPerUnit: [''],
            InventoryStockQuantity: [''],
            IsActive: [''],
            GroupId: [''],
            POSItem: ['']
        });
    }
    ShowInventory(event: any) {
        if (event === false)
            this.IsTrackInventory = true;
        else
            this.IsTrackInventory = false;
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();

            const buttonStatus = this.model.SaveStatus;
            if (this.model.ID.toString() === "0")
                this.model.SaveStatus = 1;
            //if (this.model.action === 'Update')
            //    this.model.SaveStatus = 2;
            this._ItemService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['/home/Item']);
                }
                else {
                    this.model.SaveStatus = buttonStatus;
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
        else {
            this.toastr.Error('Error', 'Please select at least one item type.');
        }
    }
    PublishItem() {
        this.loader.ShowLoader();
        const buttonStatus = this.model.SaveStatus;
        if (this.model.ID.toString() === "0")
            this.model.SaveStatus = 1;
        else
            this.model.SaveStatus = 2;
        this._ItemService.PublishItem(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this._router.navigate(['/home/Item']);
            }
            else {
                this.model.SaveStatus = buttonStatus;
                this.loader.HideLoader();
                this.toastr.Error('Error', result.ErrorMessage);
            }
        });
    }
    ChangeItemType(evt: any) {
        if (this.model.ItemTypeId != null && this.model.ItemTypeId != undefined) {

            if (this.model.ItemTypeId.toString() === "2")
                $("#IsTrack").attr("disabled", "disabled");
            else
                $("#IsTrack").removeAttr("disabled");
        }
    }
    LoadDropDown() {
        this.loader.ShowLoader();
        this._ItemService.LoadDropdown("57,58,59,64").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet.dropdownValues;
                this.unitList = list.filter(f => f.DropDownID === 57);
                this.CategoryList = list.filter(f => f.DropDownID === 58);
                this.ItemTypeList = list.filter(f => f.DropDownID === 59);
                this.GroupList = list.filter(f => f.DropDownID === 64);
                this.InstructionsList = m.ResultSet.InstructionList;
                if (this.IsEdit == false)
                    this.model.ItemTypeId = 1;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
    }
    getFileName(FName) {
        this.model.Image = FName;
    }
    ClearImageUrl() {
        this.IsNewImage = true;
        this.model.Image = '';
        this.Image = '';
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result === true) {
            this.loader.ShowLoader();
            this._ItemService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                } else {
                    this._router.navigate(['/home/Item']);
                }

            });
        }
    }
    getImageUrlName(FName) {
        this.model.Image = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.AttachImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        } else {
            this.AttachImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }
}