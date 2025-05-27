import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { ItemService } from './ItemService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
@Component({
    moduleId: module.id,
    templateUrl: 'ExpireComponentList.html',
    providers: [ItemService],
})
export class ExpireComponentList {
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
    public ScreenRights: any;
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public IdsList: any = [];
    public IsShowSearch: boolean = false;
    @Output() pageChange: EventEmitter<string> = new EventEmitter<string>();
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _AsidebarService: AsidebarService, public _ItemService: ItemService, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        //this.ScreenRights = this._CommonService.ScreenRights("11");
    }
    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Items';
        this.PConfig.Action.Add = false;
        this.PConfig.Fields = [
            { Name: "Name", Title: "Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "CostPrice", Title: "Rate", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Unit", Title: "Usage Unit", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "BatchSarialNumber", Title: "Batch Number", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "ExpiredWarrantyDate", Title: "Expire Date", OrderNo: 5, SortingAllow: true, Visible: true, isDate: true, DateFormat: "dd/MM/yyyy" },
            { Name: "Stock", Title: "Stock", OrderNo: 6, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._ItemService
            .GetExpireItemList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.ItemList = m.DataList;
                this.loader.HideLoader();
            });
    }
    AddRecord(id: string) {

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
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
       
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
}