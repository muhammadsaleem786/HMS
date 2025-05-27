import { Component, OnInit, Output, EventEmitter, OnChanges } from '@angular/core';
import { Router } from '@angular/router';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { ItemService } from './ItemService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';

@Component({
    moduleId: module.id,
    templateUrl: 'ItemComponentList.html',
    providers: [ItemService],
})
export class ItemComponentList{
    public ActiveToggle: boolean = false;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public ItemList: any[] = [];
    public ItemStockList: any[] = [];
    public ScreenRights: any;
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public IdsList: any = [];
    public IsShowSearch: boolean = false;
    public ControlRights: any;
    public ImportControlRights: any;
    public RestockControlRights: any;
    public ExpireControlRights: any;
    selectedValue: string = "A";
    public CategoryList: any[] = [];
    @Output() pageChange: EventEmitter<string> = new EventEmitter<string>();
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _AsidebarService: AsidebarService, public _ItemService: ItemService,
        public toastr: CommonToastrService, private modalService: NgbModal, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("61");
        this.ImportControlRights = this._CommonService.ScreenRights("68");
        this.RestockControlRights = this._CommonService.ScreenRights("69");
        this.ExpireControlRights = this._CommonService.ScreenRights("70");
    }
    ngOnInit() {
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
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
    RefreshList(Value: string) {
        this.PModel.VisibleColumnInfo = "Name#Name,Group#Group,Code#Code,CostPrice#CostPrice,Unit#Unit,Stock#Stock,Status#Status";
        this.PModel.CurrentPage = 1;
        this.Refresh();
    }
  
    Refresh() {
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "Name#Name,Group#Group,Code#Code,CostPrice#CostPrice,Unit#Unit,Stock#Stock,Status#Status";
        this._ItemService
            .GetItemByGroupList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID, this.PModel.VisibleFilter).then(m => {
                this.pagesRange = [];
                this.PModel.TotalRecord = m.TotalRecord;
                this.ItemList = m.DataList;
                this.CategoryList = m.OtherDataModel;
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.CategoryList.push({ "ID": "A", "Value": "All" })
                this.loader.HideLoader();
            });
    }
    getFilterData() {
        if (this.selectedValue == "A") {
            this.PModel.FilterID = "0";
            this.PModel.VisibleFilter = false;
        } else {
            this.PModel.FilterID = this.selectedValue
            this.PModel.VisibleFilter = false;
        }
        this.Refresh();
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._ItemService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Item/saveitem']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Item/saveitem'], { queryParams: { id: this.Id } });
    }
    MoveToNextTab() {
        this.loader.ShowLoader();
        this._router.navigate(['home/Item/saveitem']);
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._ItemService.Delete(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.Refresh();
            });
        }
    }
    GetList() {
        this.Refresh();
    }
    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else
            this.Refresh();
        //this.SpecificPeriod = '';
        //this.ExportType = '';
    }
    OpenStockModel(id: string, StockModel) {
        this.PModel.FilterID = id;
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "Name#Name,CostPrice#CostPrice,Unit#Unit,BatchSarialNumber#BatchSarialNumber,ExpiredWarrantyDate#ExpiredWarrantyDate,Stock#Stock";
         this._ItemService
            .GetItemStockList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID).then(m => {
                //this.PModel.TotalRecord = m.TotalRecord;
                this.ItemStockList = m.DataList;
                this.PModel.FilterID = "0";
                this.modalService.open(StockModel, { size: 'lg' });
                this.loader.HideLoader();
            });
    }
}