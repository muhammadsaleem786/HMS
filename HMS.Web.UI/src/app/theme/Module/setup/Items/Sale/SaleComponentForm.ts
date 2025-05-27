import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { SaleService } from './SaleService';
import { SaleHoldService } from './SaleHoldService';
import { SaleModel, pur_sale_dt, SaleHoldModel, pur_sale_hold_dt } from './SaleModel';
import { ActivatedRoute } from '@angular/router';
import { IMyDateModel } from 'mydatepicker';
import jsPDF from 'jspdf';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
import { ValidationVariables, GlobalVariable } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { emr_patient } from '../../Appointment/AppointmentModel';
import { AppointmentService } from '../../Appointment/AppointmentService';
import { ItemService } from '../../Items/AddItems/ItemService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { VendorModel } from '../Vendor/VendorModel';
import { VendorService } from '../Vendor/VendorService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
declare var $: any;
import swal from 'sweetalert';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
@Component({
    moduleId: module.id,
    templateUrl: 'SaleComponentForm.html',
    providers: [SaleService, SaleHoldService, VendorService, AppointmentService, ItemService],
})
export class SaleComponentForm {
    public vendorModel = new VendorModel();
    public model = new SaleModel();
    public saleHoldModel = new SaleHoldModel();
    public Patientmodel = new emr_patient();
    public IsCNICMandatory: any; public TittleList: any[] = [];
    public BillTypeList: any[] = [];
    public ItemInfo: any; public GenderList: any[] = [];
    public ItemList = []; public PatientName: any; public PatientId: any;
    public patientInfo: any;
    public Sale_itemdynamicArray = [];
    public PrintSale_itemdynamicArray = [];
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public showBankStatement: boolean = false;
    public sub: any;
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    public Form2: FormGroup;
    public submitted: boolean;
    pur_sale_dt = new Array<pur_sale_dt>();
    sale_order_itemArray: Array<pur_sale_dt> = [];
    public RowCount: number;
    public CategoryList: any[] = [];
    public SubTotal: number = 0;
    public Total: number = 0;
    public PSubTotal: number = 0;
    public PTotal: number = 0;
    public FileName: string;
    public IsNewImage: boolean = true;
    public AutoInvoiceNo: number;
    public DiscountType: string;
    public TransLevelType: string;
    public DiscountAmount: number = 0;
    public OverAllDiscountAmount: number = 0;
    public PDiscountAmount: number = 0;
    public DiscountTypeList = [];
    public VendorList: any[] = [];
    public ItemCategoryList: any[] = [];
    public DisType: number = 1;
    public DisTypeList = [];
    public CompanyInfo: any;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public CustomerId: any;
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public IsRefund: boolean;
    public ControlRights: any;
    public Payment: number = 0;
    public Change: number = 0;
    public PayableAmount: number = 0;
    public ReceivedAmount: number = 0;
    @ViewChild("content") PatientContent: TemplateRef<any>;
    @ViewChild("InvoicePopup") InvoicePopup: TemplateRef<any>;
    @ViewChild("ItemCategory") ItemCategory: TemplateRef<any>;
    @ViewChild('closeBatchModal') closeBatchModal: ElementRef;
    @ViewChild("PrintRx") PrintRxContent: TemplateRef<any>;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _SaleService: SaleService,
        public _SaleHoldService: SaleHoldService, public _fb: FormBuilder
        , public route: ActivatedRoute, public toastr: CommonToastrService, public _VendorService: VendorService,
        public _AppointmentService: AppointmentService, public _AsidebarService: AsidebarService, private modalService: NgbModal, private _ItemService: ItemService
        , private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("63");
        this.PayrollRegion = this._CommonService.getPayrollRegion();
    }
    ngOnInit() {
        this.LoadDropDown();
        this.DiscountTypeList.push({ "id": 1, "Name": "%" });
        this.DiscountTypeList.push({ "id": 2, "Name": "PKR" });
        this.DisTypeList.push({ "IsItemLevelDiscount": false, "Name": "At transaction level" });
        this.DisTypeList.push({ "IsItemLevelDiscount": true, "Name": "At line item level" });
        this.model.Date = new Date();
        //this.Add();
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    var saleholdId = this._AsidebarService.GetSaleHoldValue();
                    if (saleholdId == "0") {
                        this._SaleService.GetById(this.model.ID).then(m => {
                            if (m.ResultSet != null) {
                                this.Sale_itemdynamicArray = [];
                                this.model = m.ResultSet.result;
                                const ItemList = m.ResultSet.ItemList;
                                this.IsRefund = m.ResultSet.IsRefund;
                                if (this.model.Discount == null) {
                                    this.DisType = 2;
                                    this.model.Discount = this.model.DiscountAmount;
                                }
                                if (this.model.IsItemLevelDiscount)
                                    this.TransLevelType = "2";
                                else
                                    this.TransLevelType = "1";
                                this.Total = this.model.Total;
                                this.SubTotal = 0;
                                this.model.pur_sale_dt.forEach((item, index) => {
                                    if (item.Discount == null) {
                                        item.Discount = item.DiscountAmount;
                                    }
                                    let ItemObj = ItemList.filter(a => a.value == item.ItemID);
                                    item.ItemName = ItemObj[0].label;
                                    item.ItemTypeId = ItemObj[0].Type;
                                    item.TypeValue = ItemObj[0].TypeValue;
                                    item.Stock = ItemObj[0].stock;
                                    item.GroupId = ItemObj[0].GroupId;
                                    this.Sale_itemdynamicArray.push(item);
                                    this.SubTotal += item.TotalAmount;
                                });
                                this.loader.HideLoader();
                            } else
                                this.loader.HideLoader();
                        });

                    } else {
                        this._SaleService.GetHoldSaleById(this.model.ID).then(m => {
                            if (m.ResultSet != null) {
                                this.Sale_itemdynamicArray = [];
                                this.saleHoldModel = m.ResultSet.result;
                                const ItemList = m.ResultSet.ItemList;
                                this.IsRefund = m.ResultSet.IsRefund;
                                if (this.saleHoldModel.Discount == null) {
                                    this.DisType = 2;
                                    this.saleHoldModel.Discount = this.model.DiscountAmount;
                                }
                                if (this.saleHoldModel.IsItemLevelDiscount)
                                    this.TransLevelType = "2";
                                else
                                    this.TransLevelType = "1";
                                this.Total = this.saleHoldModel.Total;
                                this.SubTotal = 0;
                                this.saleHoldModel.pur_sale_hold_dt.forEach((item, index) => {
                                    if (item.Discount == null) {
                                        item.Discount = item.DiscountAmount;
                                    }
                                    let ItemObj = ItemList.filter(a => a.value == item.ItemID);
                                    item.ItemName = ItemObj[0].label;
                                    item.ItemTypeId = ItemObj[0].Type;
                                    item.TypeValue = ItemObj[0].TypeValue;
                                    item.Stock = ItemObj[0].stock;
                                    item.GroupId = ItemObj[0].GroupId;
                                    this.Sale_itemdynamicArray.push(item);
                                    this.SubTotal += item.TotalAmount;
                                });
                                this.saleHoldModel.SaleHoldId = this.model.ID;
                                this.saleHoldModel.ID = 0;
                                this.model = Object.assign(new SaleModel(), this.saleHoldModel);
                                this.loader.HideLoader();
                            } else
                                this.loader.HideLoader();
                        });

                    }

                }
            });
        this.Form1 = this._fb.group({
            Date: [''],
            SubTotal: [''],
            DueDate: [''],
            Total: [''],
            TaxAmount: [''],
            DiscountAmount: [''],
            DiscountType: [''],
            Discount: [''],
            IsItemLevelDiscount: [''],
            ItemName: [''],
            CustomerId: ['']
        });
        this.Form2 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: ['', [Validators.required]],
            Email: [''],
            Mobile: ['', [<any>Validators.required]],
            CNIC: [''],
            Image: [''],
            Notes: [''],
            MRNO: [''],
            BillTypeId: [''],
            PrefixTittleId: [''],
            Father_Husband: [''],
        });
    }
    LoadDropDown() {
        this.loader.ShowLoader();
        this._SaleService.LoadDropdown("58").then(m => {
            if (m.IsSuccess) {
                this.CategoryList = m.ResultSet.dropdownValues;
                let PatientInfo = m.ResultSet.PatientInfo;

                this.CompanyInfo = m.ResultSet.companyInfo;
                if (PatientInfo != null) {
                    this.PatientName = PatientInfo.label;
                    this.CustomerId = PatientInfo.value;
                    this.model.CustomerId = this.CustomerId;
                }
                if (this.model.IsItemLevelDiscount)
                    this.TransLevelType = "2";
                else
                    this.TransLevelType = "1";
                this.loader.HideLoader();
            }
        });
    }
    LoadSearchableDropdown() {
        //Search By Name
        $('#itemSearch').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._SaleService.SaleItemByName(request.term).then(m => {
                    this.ItemList = m.ResultSet.ItemInfo;
                    const items = m.ResultSet.ItemInfo.map(item => ({
                        label: item.label + " (qty:" + item.stock + ")",
                        value: item.value,
                        CostPrice: item.SalePrice,
                        ItemTypeId: item.Type,
                        stock: item.stock,
                        CategoryId: item.CategoryId,
                        TypeValue: item.TypeValue,
                        GroupId: item.GroupId,
                        TrackInventory: item.TrackInventory,
                    }));
                    response(items);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                if (ui.item.ItemTypeId == 3 || ui.item.ItemTypeId == 4) {
                    this.loadBatchAndSerialItem(ui.item.value);
                    ui.item.value = null;
                } else {
                    var obj = new pur_sale_dt();
                    obj.Quantity = 1;
                    obj.ItemID = ui.item.value;
                    obj.ItemName = ui.item.label;
                    obj.Rate = ui.item.CostPrice;
                    obj.DiscountType = 1;
                    obj.TotalAmount = obj.Quantity * obj.Rate;
                    obj.TypeValue = ui.item.TypeValue;
                    obj.Stock = ui.item.stock;
                    obj.GroupId = ui.item.GroupId;
                    ui.item.value = null;
                    var appendQty = this.Sale_itemdynamicArray.filter(a => a.ItemID == obj.ItemID).reduce((sum, a) => sum + parseInt(a.Quantity, 10), 0);
                    appendQty = appendQty + obj.Quantity;
                    if (ui.item.stock < appendQty && ui.item.TrackInventory == true) {
                        this.toastr.Error("Error", "The item is out of stock.");
                        return;
                    } else {
                        this.Sale_itemdynamicArray.push(obj);
                        this.CalculateSubTotal();
                    }
                }
            }
        });
    }
    SelectBatchAndSerialItem(itemid: any, BatchNumber: any) {
        let itemfind = this.ItemCategoryList.filter(a => a.ID == itemid && a.BatchSarialNumber == BatchNumber);
        var AlreadyBatchItem = this.Sale_itemdynamicArray.filter(a => a.ItemID == itemid && a.BatchSarialNumber == BatchNumber);
        if (AlreadyBatchItem.length > 0) {
            AlreadyBatchItem[0].Quantity += 1;
            AlreadyBatchItem[0].TotalAmount = AlreadyBatchItem[0].Quantity * itemfind[0].SalePrice;
        } else {
            var obj = new pur_sale_dt();
            obj.Quantity = 1;
            obj.ItemID = itemfind[0].ID;
            obj.ItemName = itemfind[0].Name;
            obj.Rate = itemfind[0].SalePrice;
            obj.DiscountType = 1;
            obj.TotalAmount = obj.Quantity * obj.Rate;
            obj.TypeValue = itemfind[0].TypeValue;
            obj.Stock = itemfind[0].Stock;
            obj.GroupId = itemfind[0].GroupId;
            obj.BatchSarialNumber = itemfind[0].BatchSarialNumber;
            obj.ExpiredWarrantyDate = itemfind[0].ExpiredWarrantyDate;
            var appendQty = this.Sale_itemdynamicArray.filter(a => a.ItemID == itemid && a.BatchSarialNumber == BatchNumber).reduce((sum, a) => sum + parseInt(a.Quantity, 10), 0);
            appendQty = appendQty + obj.Quantity;
            if (itemfind[0].Stock < appendQty) {
                this.toastr.Error("Error", "The item is out of stock.");
                return;
            } else {
                this.Sale_itemdynamicArray.push(obj);
            }
        }
        this.CalculateSubTotal();
        this.modalService.dismissAll();
    }
    AgeChange() {
        this.Patientmodel.DOB = null;
    }
    LoadPatentSearchableDropdown() {
        //Search By Name
        $('#customerSearch').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.searchByName(request.term).then(m => {
                    this.patientInfo = m.ResultSet.PatientInfo;
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.PatientName = ui.item.label;
                this.CustomerId = ui.item.value;
                this.model.CustomerId = this.CustomerId;
            }
        });
    }
    QuantityChange(rowno: number, Quantity: any, itemid: any) {
        if (Quantity == "")
            Quantity = 0;
        var findobj = this.Sale_itemdynamicArray[rowno];
        var appendQty = this.Sale_itemdynamicArray.filter(a => a.ItemID == itemid).reduce((sum, a) => sum + parseInt(a.Quantity, 10), 0);
        if (findobj.Stock < appendQty) {
            this.Sale_itemdynamicArray[rowno].Quantity = this.Sale_itemdynamicArray[rowno].Quantity - Quantity;
            findobj.TotalAmount = findobj.Rate * parseInt(Quantity, 10);
            this.toastr.Error("Error", "The item is out of stock.");
            return;
        } else {
            if (Quantity != null)
                findobj.TotalAmount = findobj.Rate * parseInt(Quantity, 10);
            this.CalculateSubTotal();
        }
    }
    RateChange(rowno: number, Rate: number) {
        var findobj = this.Sale_itemdynamicArray[rowno];
        if (Rate != null)
            findobj.TotalAmount = findobj.Quantity * Rate;
        this.CalculateSubTotal();
    }
    CalculateSubTotal() {
        this.SubTotal = 0;
        this.Total = 0;
        var TotalTax = 0;
        this.DiscountAmount = 0;
        this.Sale_itemdynamicArray.forEach((item, index) => {
            if (item.Discount != null && item.Discount != undefined) {
                if (this.TransLevelType == "1") {
                    if (item.DiscountType == 1) {
                        const DiscountAmountValue = ((parseInt(item.Quantity) * item.Rate)) * (item.Discount / 100);
                        item.TotalAmount = ((parseInt(item.Quantity) * item.Rate)) - DiscountAmountValue;
                        item.DiscountAmount = this.roundToTwoDecimal(DiscountAmountValue);
                        this.DiscountAmount += DiscountAmountValue;
                    }
                    else {
                        this.DiscountAmount += parseInt(item.Discount);
                        item.TotalAmount = (parseInt(item.Quantity) * item.Rate) - parseInt(item.Discount)
                        item.DiscountAmount = this.roundToTwoDecimal(parseFloat(item.Discount));
                    }
                } else {
                    if (item.DiscountType == 1)
                        this.DiscountAmount += (TotalTax + this.SubTotal) * item.Discount / 100;
                    else
                        this.DiscountAmount += parseInt(item.Discount);
                }
            }
            if (item.TotalAmount != undefined && item.TotalAmount != null) {
                TotalTax = 0;
                if (isNaN(parseInt(item.TotalAmount)) == false) {
                    var amm = this.roundToTwoDecimal(parseFloat(item.TotalAmount));
                    this.SubTotal += this.roundToTwoDecimal(parseFloat(item.TotalAmount));
                    this.SubTotal = this.roundToTwoDecimal(this.SubTotal);
                    this.Total += this.roundToTwoDecimal(parseFloat(item.TotalAmount));
                }
            }
        });
        this.CalculateOverAllDiscount();
    }
    roundToTwoDecimal(value: number): number {
        return Math.round(value * 100) / 100;
    }
    DiscountValue(evt: any, rowno: number) {
        var findobj = this.Sale_itemdynamicArray[rowno];
        if (findobj.Discount != null && findobj.Discount != undefined) {
            if (findobj.DiscountType == 1 && findobj.Discount > 100)
                findobj.Discount = 0;
            this.CalculateSubTotal();
        }
    }
    CalculateOverAllDiscount() {
        if (this.model.DiscountType == 1) {
            this.OverAllDiscountAmount = this.roundToTwoDecimal(this.SubTotal * (this.model.Discount / 100));
        } else {
            this.OverAllDiscountAmount = this.model.Discount;
        }
        this.Total = this.roundToTwoDecimal(this.SubTotal - this.OverAllDiscountAmount);
    }
    OverAllDiscountValue(value: any) {
        if (this.model.DiscountType == 1 && this.model.Discount > 100)
            this.model.Discount = 0;
        this.CalculateOverAllDiscount();

    }
    changeOverAllDiscountType(value: any) {
        if (this.model.DiscountType == 1 && this.model.Discount > 100)
            this.model.Discount = 0;
        this.CalculateOverAllDiscount();
    }
    changeDiscountType(evt: any, rowno: number) {
        var findobj = this.Sale_itemdynamicArray[rowno];
        if (findobj.Discount != null && findobj.Discount != undefined) {
            if (findobj.DiscountType == 1 && findobj.Discount > 100)
                this.model.Discount = 0;
            this.CalculateSubTotal();
        }
    }
    PatientSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            if (this.IsCNICMandatory) {
                if (this.Patientmodel.CNIC === null || this.Patientmodel.CNIC === "") {
                    this.toastr.Error("Error", "Please enter CNIC.");
                    return;
                }
            }
            this.loader.ShowLoader();
            this._AppointmentService.IsPhoneExist(this.Patientmodel.Mobile).then(m => {
                this.loader.HideLoader();
                if (m.IsSuccess) {
                    swal({
                        title: "Are you sure?",
                        text: "Are you sure want to add another patient against this " + this.Patientmodel.Mobile + "",
                        icon: "warning",
                        buttons: ['Cancel', 'Yes'],
                        dangerMode: true,
                    })
                        .then((willDelete) => {
                            if (willDelete) {
                                this.AddUpdatePatient();
                            }
                        });

                } else {
                    this.AddUpdatePatient();
                }
            });

        }
    }
    AddUpdatePatient() {
        this.loader.ShowLoader();
        this._AppointmentService.SaveOrUpdate(this.Patientmodel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.modalService.dismissAll();
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    openNewPatient(content) {
        this.loader.ShowLoader();
        this._AppointmentService.GetEMRNO().then(m => {
            this.Patientmodel = new emr_patient();
            this.Patientmodel.PrefixTittleId = 1;
            this.Patientmodel.MRNO = m.ResultSet.MRNO;
            this.TittleList = m.ResultSet.TittleList;
            this.BillTypeList = m.ResultSet.BillTypeList;
            this.GenderList = m.ResultSet.GenderList;
            this.Patientmodel.BillTypeId = 1;
            this.loader.HideLoader();
            this.modalService.open(content, { size: 'lg' });
        });

    }
    GenderChnage(evnt: any) {
        var genderid = parseInt(evnt);
        if (genderid == 2 && this.Patientmodel.PrefixTittleId == 1)
            this.Patientmodel.PrefixTittleId = 2
        if (genderid == 1 && this.Patientmodel.PrefixTittleId == 2)
            this.Patientmodel.PrefixTittleId = 1
    }
    ///////////////////
    IntializeModels(rowno: any) {
        this.RowCount = rowno;
    }
    SaveOrUpdate(isValid: boolean): void {
        if (this.DisType == 2)
            this.model.Discount = null;
        this.model.Total = this.Total;
        this.model.SubTotal = this.SubTotal;
        this.model.TaxAmount = 0;
        this.model.DiscountAmount = this.OverAllDiscountAmount;
        this.model.pur_sale_dt = this.Sale_itemdynamicArray;
        if (this.model.pur_sale_dt.length == 0) {
            this.toastr.Error("Error", "Please select at least one item.");
            return;
        }
        const distinctGroups = new Set(this.model.pur_sale_dt.map(item => item.GroupId));
        if (distinctGroups.size > 1) {
            this.toastr.Error("Error", "Please select items of the same group type.");
            return;
        }
        const saleTypeId = this.model.SaleTypeID;
        if (this.model.ID > 0)
            this.model.SaleTypeID = 2
        else
            this.model.SaleTypeID = 1
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._SaleService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this._router.navigate(['/home/Sale']);
                    this.toastr.Success(result.Message);
                    this.Sale_itemdynamicArray = [];
                }
                else {
                    this.model.SaleTypeID = saleTypeId;
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }
    CommingSoon() {
        this.toastr.Success('', "Comming Soon.");
    }
    Clear() {
        this.model = new SaleModel();
        this.Sale_itemdynamicArray = [];
        this.pur_sale_dt = [];
        this.model.ID = 0
        this.model.SaleTypeID = 1
        this.PTotal = this.Total;
        this.PSubTotal = this.SubTotal;
        this.PDiscountAmount = this.DiscountAmount;
        this.Total = 0;
        this.SubTotal = 0;
        this.DiscountAmount = 0;
        this.model.CustomerId = this.CustomerId;
    }
    SaleOnHold() {
        debugger
        var isValid: boolean = false;
        this.model.pur_sale_dt = this.Sale_itemdynamicArray;
        const distinctGroups = new Set(this.model.pur_sale_dt.map(item => item.GroupId));
        if (distinctGroups.size > 1) {
            this.toastr.Error("Error", "Please select items of the same group type.");
            isValid = false;
            return;
        }
        if (this.model.pur_sale_dt.length == 0) {
            this.toastr.Error("Error", "Please select at least one item.");
            isValid = false;
            return;
        } else {
            isValid = true;
            this.SaveSaleHold(isValid);
        }
    }
    SaveSaleHold(isValid: boolean): void {
        if (this.DisType == 2)
            this.model.Discount = null;
        this.model.Total = this.Total;
        this.model.SubTotal = this.SubTotal;
        this.model.TaxAmount = 0;
        this.model.DiscountAmount = this.OverAllDiscountAmount;
        this.model.pur_sale_dt = this.Sale_itemdynamicArray;
        this.model.SaleTypeID = 1
        this.saleHoldModel = Object.assign(new SaleHoldModel(), this.model);
        this.saleHoldModel.pur_sale_hold_dt = this.Sale_itemdynamicArray;
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._SaleHoldService.SaveOrUpdate(this.saleHoldModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.Clear();
                    this._router.navigate(['home/Sale/savesale'], { queryParams: { id: 0 } });
                    this.toastr.Success(result.Message);
                    this.Sale_itemdynamicArray = [];
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }
    SaleHoldDelete() {
        this.loader.ShowLoader();
        this._SaleHoldService.SaleHoldDelete().then(m => {
            if (m.IsSuccess) {
                this._router.navigate(['home/Salehold']);
                this.loader.HideLoader();
            }
        });
    }
      AppendTax(event: any, rowno: number) {
        this.CalculateSubTotal();
    }
    loadItemDetail(id: any, rowno: number) {
        var obj = this.Sale_itemdynamicArray.filter(a => a.ItemID == id);
        var finditem = this.ItemList.filter(f => f.ID == id);
        obj[0].ItemDescription = finditem[0].SaleDescription;
        obj[0].MenualItemDescription = true;
        obj[0].Quantity = 1;
        obj[0].Rate = finditem[0].PurchasePrice;
        obj[0].Amount = (1 * finditem[0].PurchasePrice);
        obj[0].PriceID = 0;
        obj[0].ItemType = finditem[0].Type;
        if (finditem[0].Type == "Batch" || finditem[0].Type == "Sarial")
            obj[0].IsBatch = true;
        this.CalculateSubTotal();

    }
    DiscountChange(rowno: number, Discount: any) {
        let discount;
        var findobj = this.Sale_itemdynamicArray[rowno];
        if (Discount != null && Discount != "")
            findobj.Discount = Discount;
        if (findobj.Amount != null && Discount != "") {
            var amount = findobj.Rate * findobj.Quantity;
            if (this.DisType == 1)
                discount = amount * Discount / 100;
            else
                discount = Discount;

            findobj.DiscountAmount = discount;
            findobj.Amount = amount - discount;
        }
        else if (findobj.Amount != null && Discount == "") {
            findobj.Amount = findobj.Rate * findobj.Quantity;
        }
        this.CalculateSubTotal();
    }
    ItemLevelDiscount(val: any) {
        if (val == true) {
            this.model.Discount = 0;
            this.TransLevelType = "2";
        }
        else {
            this.TransLevelType = "1";
            this.Sale_itemdynamicArray.forEach((item, index) => {
                item.Discount = 0;
                item.DiscountAmount = 0;
                item.Amount = item.Rate * item.Quantity;
            });
        }
        this.CalculateSubTotal();
    }
    InclusiveTax(val: any) {
        if (val == true)
            this.DiscountType = "I";
        else
            this.DiscountType = "T";
        this.CalculateSubTotal();
    }
    onDOBChanged(event: IMyDateModel) {
        if (event) {
            var dob = new Date(this.Patientmodel.DOB)
            var CurrentDate = new Date();
            var ageCalc = CurrentDate.getFullYear() - dob.getFullYear();
            this.Patientmodel.Age = ageCalc;
        }
    }
    RemoveRow(rowno: any) {
        this.Sale_itemdynamicArray.splice(rowno, 1);
        this.CalculateSubTotal();
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
    }
    getFileName(FName) {
        this.FileName = FName;
    }
    ClearImageUrl() {
        $(".ImgCropper").hide();
        this.IsNewImage = true;
        this.FileName = '';
    }
    getImageUrlName(FName) {
        var img = $('.ImgCropper');
        if (img.length > 0) {
            $('.ImgCropper')[0].classList.remove('hide');
        }
        this.FileName = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.FileName = GlobalVariable.BASE_File_URL + '' + FName;
        } else {
            this.FileName = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._SaleService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.toastr.Success(m.Message);
                this._router.navigate(['/home/Invoice']);
            });
        }
    }
    loadBatchAndSerialItem(id: any) {
        this.PModel.VisibleFilter = true;
        this.PModel.FilterID = id.toString();
        this.PModel.SortName = "";
        this.modalService.open(this.ItemCategory, { size: 'lg' });
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
    }
    loadItem(id: any) {
        this.PModel.VisibleFilter = false;
        this.PModel.FilterID = id.toString();
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.modalService.open(this.ItemCategory, { size: 'lg' });
    }
    Refresh() {
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "Name#Name,Stock#Stock";
        this._ItemService
            .GetItemByCategoryList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID, this.PModel.VisibleFilter).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.ItemCategoryList = m.DataList;
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.Refresh();
    }
    getPages(totalRecord: number, recordPerPage: number) {
        if (!isNaN(totalRecord))
            this.totalPages = this.getTotalPages(totalRecord, recordPerPage);
        this.getpagesRange();
    }
    getpagesRange() {
        if (this.pagesRange.indexOf(this.PModel.CurrentPage) == -1 || this.totalPages <= 10)
            this.papulatePagesRange();
        else if (this.pagesRange.length == 1 && this.totalPages > 10)
            this.papulatePagesRange();
    }
    papulatePagesRange() {
        this.pagesRange = [];
        var Result = Math.ceil(Math.max(this.PModel.CurrentPage, 1) / Math.max(this.PModel.RecordPerPage, 1));
        this.previousPage = ((Result - 1) * this.PModel.RecordPerPage)
        this.nextPage = (Result * this.PModel.RecordPerPage) + 1;
        if (this.nextPage > this.totalPages)
            this.nextPage = this.totalPages;
        for (var i = 1; i <= 10; i++) {
            if ((this.previousPage + i) > this.totalPages) return;
            this.pagesRange.push(this.previousPage + i)
        }
    }
    getTotalPages(totalRecord: number, recordPerPage: number): number {
        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
    }
    PrintRxform(isValid: boolean): void {
        this.PrintSale_itemdynamicArray = [];
        this.loader.ShowLoader();
        if (this.DisType == 2)
            this.model.Discount = null;
        this.model.Total = this.Total;
        this.model.SubTotal = this.SubTotal;
        this.model.TaxAmount = 0;
        this.model.DiscountAmount = this.OverAllDiscountAmount;
        this.model.pur_sale_dt = this.Sale_itemdynamicArray;
        this.PrintSale_itemdynamicArray = this.Sale_itemdynamicArray;
        if (this.CompanyInfo.CompanyLogo != null)
            this.CompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + this.CompanyInfo.CompanyLogo;
        if (this.model.pur_sale_dt.length == 0) {
            this.toastr.Error("Error", "Please select at least one item.");
            this.loader.HideLoader();
            return;
        }
        const saleTypeId = this.model.SaleTypeID;
        if (this.model.ID > 0)
            this.model.SaleTypeID = 2
        else
            this.model.SaleTypeID = 1
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            if (this.model.ID > 0) {
                this.loader.HideLoader();
                this.modalService.open(this.PrintRxContent, { size: 'sm' });
                return;
            } else {
                this.loader.ShowLoader();
                this._SaleService.SaveOrUpdate(this.model).then(m => {
                    var result = JSON.parse(m._body);
                    if (result.IsSuccess) {
                        this.loader.HideLoader();
                        this.modalService.open(this.PrintRxContent, { size: 'sm' });
                        this.toastr.Success(result.Message);
                        this.Clear();
                    }
                    else {
                        this.model.SaleTypeID = saleTypeId;
                        this.loader.HideLoader();
                        this.toastr.Error('Error', result.ErrorMessage);
                    }
                });
            }

        }
    }
    SavePopupInvice() {
        debugger
        this.Total = this.ReceivedAmount;
        if (this.ReceivedAmount < this.PayableAmount) {
            this.toastr.Error("Error", "Received amount greater than or equal to payable amount.");
            return;
        }
        this.SaveOrUpdate(true);
        this.modalService.dismissAll();
    }
    PaymentSave() {
        this.PayableAmount = 0;
        this.Payment = 0;
        this.ReceivedAmount = 0;
        this.Change = 0;
        this.model.pur_sale_dt = this.Sale_itemdynamicArray;
        if (this.model.pur_sale_dt.length == 0) {
            this.toastr.Error("Error", "Please select at least one item.");
            return;
        }
        const distinctGroups = new Set(this.model.pur_sale_dt.map(item => item.GroupId));
        if (distinctGroups.size > 1) {
            this.toastr.Error("Error", "Please select items of the same group type.");
            return;
        }
        this.PayableAmount = this.Total;
        this.Payment = this.Total;
        this.ReceivedAmount = this.Total;
        this.modalService.open(this.InvoicePopup, { size: 'lg' });

    }
    calculateChangeAmount() {
        this.ReceivedAmount = this.roundToTwoDecimal(this.Payment - this.Change);
    }
    calculatePaymentAmount() {
        this.Change = this.roundToTwoDecimal(this.Payment - this.PayableAmount);
        this.ReceivedAmount = this.roundToTwoDecimal(this.Payment - this.Change);
    }

}