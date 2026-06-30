import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PaymentService } from './PaymentService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    moduleId: module.id,
    templateUrl: 'PaymentComponentList.html',
    providers: [PaymentService],
})
export class PaymentComponentList {
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public UserList: any[] = [];
    public StatusList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public ControlRights: any;
    selectedValue: string = "A";
    public valueForUser: any;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public loader: LoaderService, public _AdminService: PaymentService
        , public _router: Router, public toastr: CommonToastrService,) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("14");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.StatusList = [
            { "ID": 1, "Value": "All" },
            { "ID": 2, "Value": "Active" },
            { "ID": 3, "Value": "InActive" }
        ];
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'User';
        this.PConfig.Action.Add = false;
        this.PConfig.Fields = [
            { Name: "ClinicName", Title: "Clinic Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "UserName", Title: "User Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Date", Title: "Date", OrderNo: 1, SortingAllow: true, Visible: true, isDate: true, DateFormat: "dd/MM/yyyy" },
            { Name: "Email", Title: "Email", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "PhoneNo", Title: "Phone No", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Status", Title: "Status", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
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
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._AdminService
            .GetPaymentList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.UserList = m.DataList;
                this.loader.HideLoader();
            });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Admin/setting']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Admin/setting'], { queryParams: { id: this.Id } });
    }
    AddSignUp(id: string) {
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Admin/register']);
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
            this._AdminService.Delete(id).then(m => {
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
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._AdminService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
}