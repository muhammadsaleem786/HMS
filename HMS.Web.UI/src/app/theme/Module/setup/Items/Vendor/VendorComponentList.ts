import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { VendorService } from './VendorService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
@Component({
    moduleId: module.id,
    templateUrl: 'VendorComponentList.html',
    providers: [VendorService],
})
export class VendorComponentList {
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public VendorList: any[] = [];   
    public ControlRights: any;

    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _AsidebarService: AsidebarService, public _VendorService: VendorService, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.ControlRights = this._CommonService.ScreenRights("66");

    }
    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Vendor';
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "FirstName", Title: "First Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "LastName", Title: "Last Name", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "CompanyName", Title: "Company Name", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "VendorPhone", Title: "Phone", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "VendorEmail", Title: "Email", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Address", Title: "Address", OrderNo: 6, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._VendorService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.VendorList = m.DataList;
                this.loader.HideLoader();
            });
    }

    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._VendorService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }

    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Vendor/savevendor']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Vendor/savevendor'], { queryParams: { id: this.Id } });
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
            this._VendorService.Delete(id).then(m => {
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
    }

}