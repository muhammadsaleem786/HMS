import { FormGroup, FormControl, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Component, OnInit, ViewChild, ElementRef, Output, EventEmitter, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { ItemService } from './ItemService';
import { ItemModel } from './ItemModel';
import { GenericService } from '../../Setting/Generic/GenericService';
import { adm_generic } from '../../Setting/Generic/GenericModel';
import { DropDownService } from '../../Setting/DropDown/DropDownService';
import { sys_drop_down_value } from '../../Setting/DropDown/DropDownModel';
import { ActivatedRoute } from '@angular/router';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { filter } from 'rxjs/operators';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
declare var $: any;
@Component({
    moduleId: module.id,
    templateUrl: 'ItemComponentForm.html',
    providers: [ItemService, GenericService, DropDownService],
})
export class ItemComponentForm {
    public model = new ItemModel();
    public genericmodel = new adm_generic();
    public Dropmodel = new sys_drop_down_value();
    public Id: string;
    public submitted: boolean;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public InstructionsList: any[] = [];
    public PayrollRegion: string;
    public sub: any;
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    public Form2: FormGroup;
    public Form3: FormGroup;
    public Image: string = '';
    public IsNewImage: boolean = true;
    public isDisabled: boolean = false;
    public IsTrackInventory: boolean = false;
    public AttachImage: string = '';
    public ControlRights: any; public CompanyObj: any;
    public SpecialtyList: any[] = [];
    public CategoryList: any[] = [];
    public IsBrand: boolean= false;
    public IsUnit: boolean = false;
    public IsCategory: boolean = false;
    public IsGroup: boolean = false;
    public IsType: boolean = false;

    @ViewChild("GenericModal") ModelContent: TemplateRef<any>;
    @ViewChild("BrandModal") BrandModal: TemplateRef<any>;
    constructor(public _CommonService: CommonService, public _dropdownService: DropDownService, private modalService: NgbModal, public loader: LoaderService
        , public _router: Router, public _GenericService: GenericService, public _ItemService: ItemService, public _fb: FormBuilder
        , public route: ActivatedRoute, public toastr: CommonToastrService,
        private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("61");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
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
                            this.model.VendorName = m.ResultSet.vendorName;
                            this.model.BrandName = m.ResultSet.result.BrandName;
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
                            this.initAllAutocomplete();
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
            IsAllowNegativeStock: [''],
            IsActive: [''],
            GroupId: [''],
            POSItem: [''],
            VendorId: [''],
            GenericId: [''],
            GroupName: [''],
            TypeName: [''],
            VendorName: [''],
            FormulationName: [''],
            CategoryName: [''],
            UnitName: [''],
            BrandId: [''],
            BrandName: [''],
        });
        this.Form2 = this._fb.group({
            GenericName: ['', [Validators.required]],
            CommonUse: [''],
            SpecialtyId: ['', [Validators.required]],
            CategoryID: ['', [Validators.required]],
            CategoryDropDownID: [''],
            SpecialtyDropDownID: [''],
            IsActive: ['', [Validators.required]],
        });
        this.Form3 = this._fb.group({
            Value: ['', [Validators.required]],
        });
    }
    ShowInventory(event: any) {
        if (event === false)
            this.IsTrackInventory = true;
        else
            this.IsTrackInventory = false;
    }
    AddGeneric(BrandModal: any) {
        this.modalService.open(BrandModal, { size: 'md' });
    }
    AddMasterData(GenericModal: any, addType: any) {
        this.IsUnit = addType === 'unit';
        this.IsBrand = addType === 'brand';
        this.IsCategory = addType === 'category';
        this.IsGroup = addType === 'group';
        this.IsType = addType === 'type';
        this.Form3.reset();
        this.modalService.open(GenericModal, { size: 'md' });
    }

    BrandSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.Dropmodel.DropDownID = this.IsBrand ? 73 : this.IsUnit ? 57 : this.IsCategory ? 58 : this.IsGroup ? 64 : this.IsType ? 59 : 0;
            this._dropdownService.SaveOrUpdate(this.Dropmodel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {                   
                    if (this.IsBrand) {
                        this.model.BrandId = result.ResultSet.ID;
                        this.model.BrandName = result.ResultSet.Value;
                    }
                    else if (this.IsCategory) {
                        this.model.CategoryID = result.ResultSet.ID;
                        this.model.CategoryName = result.ResultSet.Value;
                    }
                    else if (this.IsUnit) {
                        this.model.UnitID = result.ResultSet.ID;
                        this.model.UnitName = result.ResultSet.Value;
                    }
                    else if (this.IsGroup) {
                        this.model.GroupId = result.ResultSet.ID;
                        this.model.GroupName = result.ResultSet.Value;
                    }
                    else if (this.IsType) {
                        this.model.ItemTypeId = result.ResultSet.ID;
                        this.model.TypeName = result.ResultSet.Value;
                    }
                    this.Dropmodel = new sys_drop_down_value();
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll(this.BrandModal);
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            });
        }
    }
    FormulationSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._GenericService.SaveOrUpdate(this.genericmodel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.model.GenericId = result.ResultSet.ID;
                    this.model.FormulationName = result.ResultSet.GenericName;
                    this.genericmodel = new adm_generic();
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll(this.ModelContent);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
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
    LoadBrand() {
        $("#BrandSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._ItemService.BrandSearch(request.term).then(m => {

                    if (m.ResultSet.result.length == 0)
                        $("#BrandSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 2,
            select: (event, ui) => {
                this.model.BrandId = ui.item.value;
                this.model.BrandName = ui.item.label;

                return ui.item.label;
            }
        });
    }
    LoadVendor() {
        $("#VendorSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._ItemService.VendorSearch(request.term).then(m => {

                    if (m.ResultSet.result.length == 0)
                        $("#VendorSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            focus: (event, ui) => {
                event.preventDefault();
                $('#VendorSearch').val(ui.item.label);
            },
            select: (event, ui) => {
                this.model.VendorId = ui.item.value;
                this.model.VendorName = ui.item.label;

                return ui.item.label;
            }
        });
    }
    LoadFormulation() {
        $("#FormulationSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._ItemService.FormulationSearch(request.term).then(m => {
                    if (m.ResultSet.result.length == 0)
                        $("#FormulationSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            focus: (event, ui) => {
                event.preventDefault();
                $('#FormulationSearch').val(ui.item.label);
            },
            select: (event, ui) => {
                this.model.GenericId = ui.item.value;
                this.model.FormulationName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    LoadUnit() {
        $("#UnitSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._ItemService.UnitSearch(request.term).then(m => {

                    if (m.ResultSet.result.length == 0)
                        $("#UnitSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 1,
            select: (event, ui) => {
                this.model.UnitID = ui.item.value;
                this.model.UnitName = ui.item.label;

                return ui.item.label;
            }
        });
    }
    LoadType() {
        $("#TypeSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._ItemService.ItemTypeSearch(request.term).then(m => {

                    if (m.ResultSet.result.length == 0)
                        $("#TypeSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            focus: (event, ui) => {
                event.preventDefault();
                $('#TypeSearch').val(ui.item.label);
            },
            select: (event, ui) => {
                this.model.ItemTypeId = ui.item.value;
                this.model.TypeName = ui.item.label;

                return ui.item.label;
            }
        });
    }
    LoadGroup() {
        $("#GroupSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._ItemService.GroupSearch(request.term).then(m => {

                    if (m.ResultSet.result.length == 0)
                        $("#GroupSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 2,
            select: (event, ui) => {
                this.model.GroupId = ui.item.value;
                this.model.GroupName = ui.item.label;

                return ui.item.label;
            }
        });
    }
    LoadCategory() {
        $("#CategorySearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._ItemService.CategorySearch(request.term).then(m => {

                    if (m.ResultSet.result.length == 0)
                        $("#CategorySearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            focus: (event, ui) => {
                event.preventDefault();
                $('#CategorySearch').val(ui.item.label);
            },
            select: (event, ui) => {
                this.model.CategoryID = ui.item.value;
                this.model.CategoryName = ui.item.label;

                return ui.item.label;
            }
        });
    }
    LoadDropDown() {
        this.loader.ShowLoader();
        this._ItemService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.InstructionsList = m.ResultSet.InstructionList;
                this.SpecialtyList = m.ResultSet.Dropdown.filter(a => a.DropDownID == 24);
                this.CategoryList = m.ResultSet.Dropdown.filter(a => a.DropDownID == 67);
                if (this.IsEdit == false)
                    this.model.ItemTypeId = 1;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    initAllAutocomplete() {
        this.LoadGroup();
        this.LoadType();
        this.LoadUnit();
        this.LoadCategory();
        this.LoadVendor();
        this.LoadBrand();
        this.LoadFormulation();
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
            this.AttachImage = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + FName;
        } else {
            this.AttachImage = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + FName;
        }
    }
}