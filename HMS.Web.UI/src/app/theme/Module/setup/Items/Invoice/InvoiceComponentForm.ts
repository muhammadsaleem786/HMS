import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { InvoiceService } from './InvoiceService';
import { InvoiceModel, pur_invoice_dt, BatchModel } from './InvoiceModel';
import { ActivatedRoute } from '@angular/router';
import { IMyDateModel } from 'mydatepicker';
import jsPDF from 'jspdf';
import { ValidationVariables, GlobalVariable } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { VendorModel } from '../Vendor/VendorModel';
import { VendorService } from '../Vendor/VendorService';
import { PaymentService } from '../Payment/PaymentService';
import { filter } from 'rxjs/operators';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { SaleService } from './../../Items/Sale/SaleService';

declare var $: any;

@Component({
    moduleId: module.id,
    templateUrl: 'InvoiceComponentForm.html',
    providers: [InvoiceService, VendorService, PaymentService, SaleService],
})
export class InvoiceComponentForm {
    public vendorModel = new VendorModel();
    public model = new InvoiceModel();
    public batchModel = new BatchModel();
    public Invoice_itemdynamicArray = [];
    public Id: string;
    public IsList: boolean = true; public BranchList: any[] = [];
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public showBankStatement: boolean = false;
    public sub: any;
    public IsEdit: boolean = false;
    public Form1: FormGroup;
    public Form2: FormGroup;
    public Form3: FormGroup;
    public Form4: FormGroup;
    public Form5: FormGroup;
    public Form6: FormGroup;
    public IsAutoShow: boolean = true;
    public Customerlist: any[] = [];
    public PaymentTermlist: any[] = [];
    public TaxList: any[] = [];
    public GroupList: any[] = [];
    public CustomerList: any[] = [];
    public submitted: boolean;
    public IsShowMoreDetail: boolean = false;
    public IsShowButton: boolean = true;
    public IsDialogActive: boolean = false;
    public IsCustomerDialogActive: boolean = false;
    public Fname: string;
    public Lname: string;
    public Cname: string;
    public CurrencyList: any[] = [];
    public PortalLanguageList: any[] = [];
    public AddCurrencyList: any[] = [];
    public PaymentTermList: any[] = [];
    public CountryList: any[] = [];
    public StatusList: any[] = [];
    public IsCustomerCurrencyDialogActive: boolean = false;
    public FormatList: any = [];
    public IsCustomerDetail: boolean = false;
    public IsVendorDetail: boolean = false;
    public IsAddAddressDialogActive: boolean = false;
    public AddressTitle: string;
    public Isshipping: boolean = false;
    public Isbilling: boolean = false;
    public IsInvoiceDialogActive: boolean = false;
    pur_invoice_dt = new Array<pur_invoice_dt>();
    sale_order_itemArray: Array<pur_invoice_dt> = [];
    public fileCount: number;
    public RowCount: number;
    public ItemList: any[] = [];
    public AllTaxList: any[] = [];
    public SubTotal: number = 0;
    public Total: number = 0;
    public FileName: string;
    public IsNewImage: boolean = true;
    public AutoInvoiceNo: number;
    public AutoInvoicePrefix: string;
    public estimateCode: string;
    public StatusSendList: any[] = [];
    public DiscountType: string;
    public TransLevelType: string;
    public DiscountAmount: number = 0;
    public DiscountTypeList = [];
    public DisType: number = 1;
    public IsVendorDialogActive: boolean = false;
    public IsPurchaseInfo: boolean = false;
    public unique: string[];
    public groupedArray: any[];
    public TypeOptions: any[];
    public COAFirstLevelList: any[] = [];
    public COASecondLevelList: any[] = [];
    public PurchaseInfo: any[] = [];
    public TaxsList = [];
    public DisTypeList = []; public _IsApplyPriceList: any;
    public _IsEnablePriceList: any;
    public gen_IsAdjustment: any; public PriceList: any[] = [];
    public PriceRateId: any; public StructureList: any = [];
    public IsPriceDialogActive: boolean = false; public RoleBaseCombList: any = [];
    public InputSettingModal: boolean = false;
    public LastInputSettingModal: boolean = false;
    public CompanyNameInputSettingModal: boolean = false;
    public BatchType: string = "";
    public PaymentList: any[] = [];
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public ControlRights: any;
    @ViewChild('closeBatchModal') closeBatchModal: ElementRef;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _InvoiceService: InvoiceService, public _fb: FormBuilder
        , public route: ActivatedRoute, public toastr: CommonToastrService, public _VendorService: VendorService,
        public _PaymentService: PaymentService, public _SaleService: SaleService, private encrypt: EncryptionService
    ) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("62");
    }
    ngOnInit() {
        this.LoadDropDown();
        this.DiscountTypeList.push({ "id": 1, "Name": "%" });
        this.DiscountTypeList.push({ "id": 2, "Name": "PKR" });
        this.DisTypeList.push({ "IsItemLevelDiscount": false, "Name": "At transaction level" });
        this.DisTypeList.push({ "IsItemLevelDiscount": true, "Name": "At line item level" });
        this.model.BillDate = new Date();
        this.model.DueDate = new Date();
        this.Add();
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._InvoiceService.GetById(this.model.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.Invoice_itemdynamicArray = [];
                            this.model = m.ResultSet.result;
                            if (m.ResultSet.result.pur_vendor != null)
                                this.model.VendorName = m.ResultSet.result.pur_vendor.CompanyName;
                            let itemList = m.ResultSet.itemList;
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
                            this.IsCustomerDetail = true;
                            this.model.pur_invoice_dt.forEach((item, index) => {
                                let itemObj = itemList.filter(x => x.ID == item.ItemID);
                                if (item.Discount == null) {
                                    item.Discount = item.DiscountAmount;
                                    item.DiscountType = 2;
                                } else
                                    item.DiscountType = 1;

                                if (itemObj.length != 0)
                                    item.ItemType = itemObj[0].Value;
                                if (item.BatchSarialNumber != null && item.BatchSarialNumber != 0)
                                    item.IsBatch = true
                                else
                                    item.IsBatch = false;
                                item.Item = itemObj[0].Name;
                                this.Invoice_itemdynamicArray.push(item);
                            });
                            if (this.model.pur_invoice_dt.length > 0) {
                                for (var a = 0; a < this.model.pur_invoice_dt.length; a++) {
                                    this.SubTotal += this.model.pur_invoice_dt[a].Amount;
                                }
                                this.CalculateSubTotal();
                            } else {
                                this.Add();
                            }
                            this.PModel.SortName = "";
                            this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                            this.selectPage(this.PModel.CurrentPage);
                            this.loader.HideLoader();
                        } else
                            this.loader.HideLoader();
                    });

                }
            });
        this.Form1 = this._fb.group({
            VendorID: [''],
            VendorName: [''],
            BillNo: ['', [Validators.required]],
            OrderNo: [''],
            BillDate: [''],
            DueDate: ['', [Validators.required]],
            Total: [''],
            IsItemLevelDiscount: ['']
        });
        this.Form2 = this._fb.group({
            CustomerType: [''],
            Salutation: [''],
            FirstName: [''],
            LastName: [''],
            CompanyName: [''],
            CustomerDisplayName: ['', [Validators.required]],
            CustomerEmail: ['', [Validators.pattern(ValidationVariables.EmailPattern)]],
            WorkPhone: [''],
            MobileNo: [''],
            Skype: [''],
            Designation: [''],
            Department: [''],
            Website: [''],
            CurrencyID: ['', [Validators.required]],
            OpeningBalance: [''],
            PaymentTermID: [''],
            EnablePortal: [''],
            PortalLanguageID: [''],
            Facebook: [''],
            Twitter: [''],
            Billing_Attention: [''],
            Billing_CountryID: [''],
            Billing_Address: [''],
            Billing_Address2: [''],
            Billing_City: [''],
            Billing_State: [''],
            Billing_ZipCode: [''],
            Billing_Phone: [''],
            Billing_Fax: [''],
            Shipping_Attention: [''],
            Shipping_CountryID: [''],
            Shipping_Address: [''],
            Shipping_Address2: [''],
            Shipping_City: [''],
            Shipping_State: [''],
            Shipping_ZipCode: [''],
            Shipping_Phone: [''],
            Shipping_Fax: [''],
            Remarks: [''],
        })
        this.Form6 = this._fb.group({
            BatchSarialNumber: ['', [Validators.required]],
            ExpiredWarrantyDate: ['', [Validators.required]],
        })
    }
    ApplyRate(id: any) {
        this.PriceRateId = parseInt(id.target.value);
        this.IsPriceDialogActive = true;
    }
    LoadVendor() {
        //Search By Name
        $('#txtVandor').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._InvoiceService.SearchVandorByName(request.term).then(m => {
                    const vandors = m.ResultSet.VandorInfo.map(item => ({
                        label: item.label,
                        value: item.value
                    }));
                    response(vandors);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.VendorName = ui.item.label;
                this.model.VendorID = ui.item.value;
            }
        });
    }
    LoadDropDown() {
        this.loader.ShowLoader();
        this._InvoiceService.LoadDropdown("3,5,13,7,19,17").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet.dropdownValues;
                this.ItemList = m.ResultSet.ItemList;
                if (this.model.IsItemLevelDiscount)
                    this.TransLevelType = "2";
                else
                    this.TransLevelType = "1";
                this.loader.HideLoader();
            }
        });
    }
    IntializeModels(rowno: any) {
        this.RowCount = rowno;
        let obj = this.Invoice_itemdynamicArray[rowno];
        this.BatchType = obj.ItemType;
        this.batchModel = new BatchModel();
        this.batchModel.BatchSarialNumber = obj.BatchSarialNumber;
        this.batchModel.ExpiredWarrantyDate = obj.ExpiredWarrantyDate;
    }
    BatchSave(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            //this.loader.ShowLoader();
            this.Invoice_itemdynamicArray[this.RowCount].BatchSarialNumber = this.batchModel.BatchSarialNumber;
            this.Invoice_itemdynamicArray[this.RowCount].ExpiredWarrantyDate = this.batchModel.ExpiredWarrantyDate;
            this.closeBatchModal.nativeElement.click();
            //let obj = this.Invoice_itemdynamicArray[this.RowCount];
            //this.batchModel.ItemID = obj.ItemID;
            //this._InvoiceService.UpdateBatch(this.batchModel).then(m => {
            //    this.loader.HideLoader();
            //    var result = JSON.parse(m._body);
            //    if (result.IsSuccess) {
            //        this.closeBatchModal.nativeElement.click();
            //    }
            //    else {
            //        this.loader.HideLoader();
            //        this.toastr.Error('Error', result.ErrorMessage);
            //    }
            //});
        }
    }
    SaveOrUpdate(isValid: boolean): void {
        if (this.model.BillNo == undefined || this.model.BillNo == null || this.model.BillNo == "") {
            this.toastr.Error("Bill# is required.");
            return;
        }
        let dynamicList = this.Invoice_itemdynamicArray;
        this.Invoice_itemdynamicArray = [];
        dynamicList.forEach((item, index) => {
            if (item.ItemID != undefined && item.ItemID != null && item.ItemID != "")
                this.Invoice_itemdynamicArray.push(item);
        });
        var Formdate = this._CommonService.GetFormatDate(this.model.BillDate);

        if (this.DisType == 2)
            this.model.Discount = null;
        this.Invoice_itemdynamicArray.forEach((item, index) => {

            if (item.ItemType == "Batch" || item.ItemType == "Sarial") {
                if (item.BatchSarialNumber == "" || item.BatchSarialNumber == null) {
                    isValid = false;
                    this.toastr.Error("Batch# is required at the row no. " + index);
                    return true;
                }
            }
            if (item.DiscountType == 2)
                item.Discount = null;
        });
        this.model.Total = this.Total;
        this.model.DiscountAmount = this.DiscountAmount;
        this.model.pur_invoice_dt = this.Invoice_itemdynamicArray;
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            const buttonStatus = this.model.SaveStatus;
            if (this.model.ID.toString() === "0")
                this.model.SaveStatus = 1;
            else if (this.model.action === 'Update') {
                this.model.SaveStatus = 1;
            } else {
                this.model.SaveStatus = 2;
            }
            this._InvoiceService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this._router.navigate(['/home/Invoice']);
                    this.toastr.Success(result.Message);
                }
                else {
                    this.model.SaveStatus = buttonStatus;
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }
    ChangeStatus(isValid: boolean, ApprovalStatusID: any) {
        if (this.model.VendorID != null && this.model.BillDate != null) {
            isValid = true;
            this.SaveOrUpdate(isValid);
        }
    }
    AppendTax(event: any, rowno: number) {
        this.CalculateSubTotal();
    }
    //loadItemDetail(id: any, rowno: number) {
    //    var finditem = this.ItemList.filter(f => f.ID == id);
    //    this.Invoice_itemdynamicArray[rowno].ItemDescription = finditem[0].SaleDescription;
    //    this.Invoice_itemdynamicArray[rowno].MenualItemDescription = true;
    //    this.Invoice_itemdynamicArray[rowno].Quantity = 1;
    //    this.Invoice_itemdynamicArray[rowno].Rate = finditem[0].PurchasePrice;
    //    this.Invoice_itemdynamicArray[rowno].Amount = (1 * finditem[0].PurchasePrice);
    //    this.Invoice_itemdynamicArray[rowno].PriceID = 0;
    //    this.Invoice_itemdynamicArray[rowno].ItemType = finditem[0].Type;
    //    if (finditem[0].Type == "Batch" || finditem[0].Type == "Sarial")
    //        this.Invoice_itemdynamicArray[rowno].IsBatch = true;
    //    this.CalculateSubTotal();
    //}
    LoadItem(indx: any) {
        var id = ("#txtItem_" + indx);
        $(id).autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._SaleService.SearchAllItemByName(request.term).then(m => {
                    response(m.ResultSet.ItemInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.Invoice_itemdynamicArray[indx].ItemID = ui.item.value;
                this.Invoice_itemdynamicArray[indx].Item = ui.item.label;
                this.Invoice_itemdynamicArray[indx].MenualItemDescription = true;
                this.Invoice_itemdynamicArray[indx].Quantity = 1;
                this.Invoice_itemdynamicArray[indx].Rate = ui.item.PurchasePrice;
                this.Invoice_itemdynamicArray[indx].Amount = (1 * ui.item.PurchasePrice);
                this.Invoice_itemdynamicArray[indx].PriceID = 0;
                this.Invoice_itemdynamicArray[indx].ItemType = ui.item.TypeValue;
                if (ui.item.TypeValue == "Batch" || ui.item.TypeValue == "Sarial")
                    this.Invoice_itemdynamicArray[indx].IsBatch = true;
                this.CalculateSubTotal();
            }
        });
    }
    DiscountChange(rowno: number, Discount: any) {
        let discount;
        var findobj = this.Invoice_itemdynamicArray[rowno];
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
            this.Invoice_itemdynamicArray.forEach((item, index) => {
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
    changeDiscountType(evt: any) {
        this.DisType = evt;
        if (this.model.Discount != null && this.model.Discount != undefined) {
            if (this.DisType == 1 && this.model.Discount > 100)
                this.model.Discount = 0;

            this.CalculateSubTotal();
        }
    }
    DiscountValue(evt: any) {
        if (this.model.Discount != null && this.model.Discount != undefined) {
            if (this.DisType == 1 && this.model.Discount > 100)
                this.model.Discount = 0;
            this.CalculateSubTotal();
        }
    }
    changeItemDiscountType(rowno: number, evt: any) {
        let discount;
        this.DisType = evt;
        var findobj = this.Invoice_itemdynamicArray[rowno];
        if (findobj.Discount != null && findobj.Discount != "") {
            if (this.DisType == 1 && findobj.Discount > 100) {
                findobj.Discount = 0;
                findobj.DiscountAmount = 0;
                discount = 0;
            }
            else {
                if (findobj.Amount != null && findobj.Discount != "") {
                    var amount = findobj.Rate * findobj.Quantity;
                    if (this.DisType == 1)
                        discount = amount * findobj.Discount / 100;
                    else
                        discount = findobj.Discount;

                    findobj.DiscountAmount = discount;
                    findobj.Amount = amount - discount;
                } else if (findobj.Amount != null && findobj.Discount == "") {
                    findobj.Amount = findobj.Rate * findobj.Quantity;
                }
            }
            this.CalculateSubTotal();
        }
    }
    CalculateSubTotal() {
        this.SubTotal = 0;
        this.Total = 0;
        var TotalTax = 0;
        this.Invoice_itemdynamicArray.forEach((item, index) => {
            if (item.Amount != undefined && item.Amount != null) {
                TotalTax = 0;
                if (isNaN(parseInt(item.Amount)) == false) {
                    this.SubTotal += parseInt(item.Amount);
                    this.Total += parseInt(item.Amount);
                }
            }
        });
        if (this.model.Discount != null && this.model.Discount != undefined) {
            if (this.TransLevelType == "1") {
                if (this.DisType == 1)
                    this.DiscountAmount = this.SubTotal * this.model.Discount / 100;
                else
                    this.DiscountAmount = this.model.Discount;

                this.Total = this.Total - this.DiscountAmount;
            } else {
                if (this.DisType == 1)
                    this.DiscountAmount = (TotalTax + this.SubTotal) * this.model.Discount / 100;
                else
                    this.DiscountAmount = this.model.Discount;

                this.Total = this.Total - this.DiscountAmount;
            }
        }
    }
    onEstimateDate(event: IMyDateModel) {
        if (event) {
            this.model.BillDate = new Date(event.date.year, event.date.month - 1, event.date.day);
        }
    }
    onExpiryDate(event: IMyDateModel) {
        if (event) {
            this.model.DueDate = new Date(event.date.year, event.date.month - 1, event.date.day);
        }
    }
    QuantityChange(rowno: number, Quantity: number) {
        var findobj = this.Invoice_itemdynamicArray[rowno];
        if (findobj.ItemType == "Sarial") {
            this.Invoice_itemdynamicArray[rowno].Quantity = 1;
            Quantity = 1;
            this.toastr.Error("In the case of Sarial, do not alter the quantity.");
        }
        if (Quantity != null)
            findobj.Amount = findobj.Rate * Quantity;
        this.CalculateSubTotal();
    }
    RateChange(rowno: number, Rate: number) {
        var findobj = this.Invoice_itemdynamicArray[rowno];
        if (Rate != null)
            findobj.Amount = findobj.Quantity * Rate;
        this.CalculateSubTotal();
    }
    onDOBChanged(event: IMyDateModel) {
    }
    Add() {
        var obj = new pur_invoice_dt();
        this.Invoice_itemdynamicArray.push(obj);
    }
    RemoveRow(rowno: any) {
        if (this.Invoice_itemdynamicArray.length > 1) {
            this.Invoice_itemdynamicArray.splice(rowno, 1);
            this.CalculateSubTotal();
        }
    }
    ConfigureCurrency() {
        this.IsCustomerCurrencyDialogActive = true;
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
    //vandor
    //AddVendor() {
    //    this.IsVendorDialogActive = true;
    //}
    //VendorAdd() {
    //    var obj = new pur_vendor_contact();
    //    this.pur_vendor_dynamicArray.push(obj);
    //}
    //VendorRemoveRow(rowno: any) {
    //    if (this.pur_vendor_dynamicArray.length > 1) {
    //        this.pur_vendor_dynamicArray.splice(rowno, 1);
    //    }
    //}
    //VendorSave(isValid: boolean): void {
    //    this.vendorModel.pur_vendor_contact = this.pur_vendor_dynamicArray;
    //    this.submitted = true; // set form submit to true
    //    if (isValid && this.vendorModel.Facebook != '' && this.vendorModel.Facebook != undefined) {
    //        var regex = new RegExp(/^((ftp|http|https):\/\/)?(www.)?(?!.*(ftp|http|https|www.))[a-zA-Z0-9_-]+(\.[a-zA-Z]+)+((\/)[\w#]+)*(\/\w+\?[a-zA-Z0-9_]+=\w+(&[a-zA-Z0-9_]+=\w+)*)?$/gm);
    //        if (!regex.test(this.vendorModel.Facebook)) {
    //            this.toastr.Error('Incorrect Url', 'Please enter valid Facebook url. e.g www.facebook.com (OR) https://www/facebook.com')
    //            isValid = false;
    //        }
    //    }
    //    if (isValid && this.vendorModel.Twitter != '' && this.vendorModel.Twitter != undefined) {
    //        var regex = new RegExp(/^((ftp|http|https):\/\/)?(www.)?(?!.*(ftp|http|https|www.))[a-zA-Z0-9_-]+(\.[a-zA-Z]+)+((\/)[\w#]+)*(\/\w+\?[a-zA-Z0-9_]+=\w+(&[a-zA-Z0-9_]+=\w+)*)?$/gm);
    //        if (!regex.test(this.vendorModel.Twitter)) {
    //            this.toastr.Error('Incorrect Url', 'Please enter valid Twitter url. e.g www.twitter.com (OR) https://www/twitter.com')
    //            isValid = false;
    //        }
    //    }
    //    if (isValid) {
    //        this.submitted = false;
    //        this.loader.ShowLoader();
    //        this._VendorService.SaveOrUpdate(this.vendorModel).then(m => {
    //            var result = JSON.parse(m._body);
    //            if (result.IsSuccess) {
    //                this.toastr.Success(result.Message);
    //                this.IsAddAddressDialogActive = false;
    //                this.Isbilling = false;
    //                this.IsVendorDialogActive = false;
    //                this.LoadDropDown();
    //            }
    //            else {
    //                this.loader.HideLoader();
    //                this.toastr.Error('Error', result.ErrorMessage);
    //            }
    //        });
    //    }
    //}
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._InvoiceService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.toastr.Success(m.Message);
                this._router.navigate(['/home/Invoice']);
            });
        }
    }
    InputSetupModal() {
        this.InputSettingModal = true;
    }
    LastInputSetupModal() {
        this.LastInputSettingModal = true;
    }
    CompanyNameInputSetupModal() {
        this.CompanyNameInputSettingModal = true;
    }
    //for payment
    Refresh() {
        this.PModel.FilterID = this.model.ID.toString();
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "Amount#Amount,Notes#Notes,PaymentMethod#PaymentMethod";
        this._PaymentService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.PaymentList = m.DataList;
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
    DeletePayment(id: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._PaymentService.Delete(id.toString()).then(m => {
                if (m.IsSuccess) {
                    this.toastr.Success(m.Message);
                }
                this.loader.HideLoader();
                this.Refresh();
            });
        }
    }
}