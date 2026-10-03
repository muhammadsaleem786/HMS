import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CustomerService } from './CustomerService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { AsidebarService } from '../../../../../CommonService/AsidebarService';
@Component({
    moduleId: module.id,
    templateUrl: 'CustomerComponentList.html',
    providers: [CustomerService],
})
export class CustomerComponentList {
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public Id: string;
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public CustomerList: any[] = [];   
    public ControlRights: any;

    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public _AsidebarService: AsidebarService, public _CustomerService: CustomerService, public toastr: CommonToastrService) {
        this.loader.ShowLoader();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.ControlRights = this._CommonService.ScreenRights("122");

    }
    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Customer';
        this.PConfig.Action.Add = false;
        this.PConfig.Fields = [
            { Name: "FirstName", Title: "First Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "LastName", Title: "Last Name", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "DisplayName", Title: "Display Name", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Phone", Title: "Phone", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Email", Title: "Email", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Address", Title: "Address", OrderNo: 6, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._CustomerService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.CustomerList = m.DataList;
                this.loader.HideLoader();
            });
    }

    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._CustomerService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }

    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Customer/savecustomer']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Customer/savecustomer'], { queryParams: { id: this.Id } });
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
            this._CustomerService.Delete(id).then(m => {
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