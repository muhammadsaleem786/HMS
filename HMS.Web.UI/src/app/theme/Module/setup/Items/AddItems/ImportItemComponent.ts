import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { ItemService } from './ItemService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ItemModel, ImpoertItemModel, ImportEmpDataModel, CheckImportExistModel, ImportRecordStatusModel } from './ItemModel';
import * as XLSX from 'xlsx';
type AOA = any[][];

@Component({
    moduleId: module.id,
    templateUrl: 'ImportItemComponent.html',
    providers: [ItemService],
})
export class ImportItemComponent {
    public ActiveToggle: boolean = false;
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public ScreenRights: any;
    public ControlRights: any;
    //for excel upload
    @ViewChild("ImportDataModal") ImportDataModal: TemplateRef<any>;
    public ImportList = new Array<ImpoertItemModel>();
    public ExcelDataList: any[] = [];
    public ExcelErrorList = new Array<ImpoertItemModel>();
    public ChkImpExistInDB = new Array<CheckImportExistModel>();
    public RecordStatusModel = new ImportRecordStatusModel();
    public importDataModel = new ImportEmpDataModel();
    data: AOA = [[1, 2], [3, 4]];
    wopts: XLSX.WritingOptions = { bookType: 'xlsx', type: 'array' };

    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _AsidebarService: AsidebarService, public _ItemService: ItemService,
        public toastr: CommonToastrService, private modalService: NgbModal, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("68");
    }
    ngOnInit() {
    }

    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }


    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    OpenImportModel(ImportModel) {
        this.modalService.open(ImportModel, { size: 'lg' });
    }
    CheckImportDataError() {
        if (this.ImportList.length > 0) {
            this.loader.ShowLoader();
            this.RecordStatusModel = new ImportRecordStatusModel();
            this.ExcelErrorList = [];
            this.ChkImpExistInDB = [];
            this.RecordStatusModel.TotalRecords = this.ExcelDataList.length;
            const validGroups = ['pharmacy', 'ot', 'lab'];
            const unitGroups = ['box', 'sqm', 'dz', 'box', 'ft', 'g', 'kg', 'km', 'mg', 'mt', 'pcs'];
            const typeGroups = ['goods', 'service', 'batch', 'sarial'];
            const categoryGroups = ['sachet', 'tablet', 'capsules', 'drops', 'injections', 'syrup', 'ot','drip','lab'];
            this.ImportList.forEach(x => {
                if (x.Group == undefined)
                    this.AddToErrorList(x, 'Grroup is missing', true);
                else if (!validGroups.includes(x.Group.toLowerCase()))
                    this.AddToErrorList(x, 'Invalid group name', true);
                else if (x.Type == undefined)
                    this.AddToErrorList(x, 'Type is missing', true);
                else if (!typeGroups.includes(x.Type.toLowerCase()))
                    this.AddToErrorList(x, 'Invalid Type name', true);
                else if (x.Name == undefined)
                    this.AddToErrorList(x, 'Name is missing', true);
                else if (x.Unit == undefined)
                    this.AddToErrorList(x, 'Unit is missing', true);
                else if (!unitGroups.includes(x.Unit.toLowerCase()))
                    this.AddToErrorList(x, 'Invalid Unit name', true);
                else if (x.Category == undefined)
                    this.AddToErrorList(x, 'Category is missing', true);
                else if (!categoryGroups.includes(x.Category.toLowerCase()))
                    this.AddToErrorList(x, 'Invalid Category name', true);
                else if (x.CostPrice == undefined)
                    this.AddToErrorList(x, 'Cost Price is missing', true);
                else if (x.SalePrice == undefined)
                    this.AddToErrorList(x, 'Sale Price is missing', true);
                else if (x.Status == undefined)
                    this.AddToErrorList(x, 'Status is missing', true);
                else if (x.TrackInventory == undefined)
                    this.AddToErrorList(x, 'Track Inventory is missing', true);
                else if (x.POSItem == undefined)
                    this.AddToErrorList(x, 'POS Item is missing', true);
                this.RecordStatusModel.NewRecords++;
            });
            if (this.ExcelErrorList.length > 0) {
                var unique = {};
                var distinct = [];
                for (var i in this.ExcelErrorList) {
                    if (typeof (unique[this.ExcelErrorList[i].ErrorDescription]) == "undefined") {
                        distinct.push(this.ExcelErrorList[i]);
                    }
                    unique[this.ExcelErrorList[i].ErrorDescription] = '';
                }
                this.ExcelErrorList = distinct;
                this.RecordStatusModel.Errors = distinct.length;
                this.modalService.open(this.ImportDataModal, { size: 'lg' });
                this.loader.HideLoader();
            }
            else {
                this.loader.ShowLoader();
                this._ItemService.ImportEmpData(this.ImportList).then(m => {
                    var result = JSON.parse(m._body);
                    if (result.IsSuccess) {
                        this.toastr.Success(result.Message);
                        this._router.navigate(['/home/Item']);
                    }
                    else {
                        this.loader.HideLoader();
                        this.toastr.Error('Error', result.ErrorMessage);
                    }
                    this.loader.HideLoader();
                });
            }
        }
    }
    AddToErrorList(x, messg, IsSolutionToErrorManadory) {
        var item = JSON.parse(JSON.stringify(x));
        item.ErrorDescription = messg;
        item.IsSolutionToErrorManadory = IsSolutionToErrorManadory;
        this.ExcelErrorList.push(item);
    }
    onFileChange(evt: any) {
        this.ImportList = [];
        const target: DataTransfer = <DataTransfer>(evt.target);
        if (target.files.length !== 1) throw new Error('Cannot use multiple files');
        const reader: FileReader = new FileReader();
        reader.onload = (e: any) => {
            const bstr: string = e.target.result;
            const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });
            const wsname: string = wb.SheetNames[0];
            const ws: XLSX.WorkSheet = wb.Sheets[wsname];
            var val = "";
            this.data = null;
            var dataList = XLSX.utils.sheet_to_json(ws, {});
            if (val != '') {
                alert(val);
                return;
            }
            const groupMap = new Map<string, number>([
                ['pharmacy', 1],
                ['ot', 2],
                ['lab', 3],
            ]);
            const typeMap = new Map<string, number>([
                ['goods', 1],
                ['service', 2],
                ['batch', 3],
                ['sarial', 4],
            ]);
            const unitMap = new Map<string, number>([
                ['box', 1],
                ['sqm', 2],
                ['dz', 3],
                ['ft', 5],
                ['g', 6],
                ['kg', 7],
                ['km', 8],
                ['mg', 9],
                ['mt', 10],
                ['pcs', 11],
            ]);
            const categoryMap = new Map<string, number>([
                ['sachet', 1],
                ['tablet', 2],
                ['capsules', 3],
                ['drops', 4],
                ['injections', 5],
                ['syrup', 6],
                ['drip', 7],
                ['ot', 8],
                ['lab', 9],

            ]);
            this.ExcelDataList = JSON.parse(JSON.stringify(dataList));
            this.ExcelDataList.forEach(x => {
                if (x['Group'] != undefined) {
                    const groupKey = x['Group'].toLowerCase();
                    if (groupMap.has(groupKey)) {
                        x['GroupId'] = groupMap.get(groupKey);
                    }
                }
                if (x['Type'] != undefined) {
                    const typeKey = x['Type'].toLowerCase(); if (typeMap.has(typeKey)) {
                        x['ItemTypeId'] = typeMap.get(typeKey);
                    }
                }
                if (x['Unit'] != undefined) {
                    const unitKey = x['Unit'].toLowerCase(); if (unitMap.has(unitKey)) {
                        x['UnitID'] = unitMap.get(unitKey);
                    }
                }
                if (x['Category'] != undefined) {
                    const categoryKey = x['Category'].toLowerCase(); if (categoryMap.has(categoryKey)) {
                        x['CategoryID'] = categoryMap.get(categoryKey);
                    }
                }
                if (x['Status'] != undefined) {
                    const statusKey = x['Status'].toLowerCase();
                    x['IsActive'] = statusKey == 'active' ? true : false;
                }
                if (x['ReorderStockQty'] != undefined) {
                    x['InventoryStockQuantity'] = x['ReorderStockQty'];
                }
                this.ImportList.push(JSON.parse(JSON.stringify(x)));
            });
        };
        reader.readAsBinaryString(target.files[0]);
    }
    ImportItem() {
        
        this.loader.ShowLoader();
        this._ItemService.ImportEmpData(this.ImportList).then(m => {
            var result = JSON.parse(m._body);
            if (result.ErrorMessage != null)
                this.toastr.Error('Error', result.ErrorMessage);
            this.modalService.dismissAll();
            this.loader.HideLoader();
        });
    }
}