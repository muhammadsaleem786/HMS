import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PaymentService } from '../Payment/PaymentService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    moduleId: module.id,
    templateUrl: 'SubscriberComponentList.html',
    providers: [PaymentService],
})
export class SubscriberComponentList {
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public SubscriberList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public ControlRights: any;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public loader: LoaderService, public _AdminService: PaymentService
        , public _router: Router, public toastr: CommonToastrService, ) {
        this.loader.ShowLoader();
    }
    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Subscriber';
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "Name", Title: "Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Email", Title: "Email", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Phone", Title: "Phone No", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Speciality", Title: "Speciality", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "CreatedDate", Title: "Date", OrderNo: 3, SortingAllow: true, Visible: true, isDate: true, DateFormat: "dd/MM/yyyy" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._AdminService
            .GetSubscriberList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.SubscriberList = m.DataList;
                this.loader.HideLoader();
            });
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
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
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._AdminService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddRecord(id: string) {
    }
    Delete(id: string) {
    }
}