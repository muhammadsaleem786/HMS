import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserRoleService } from './UserRoleService';
import { UserRoleModel } from './UserRoleModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    moduleId: module.id,
    templateUrl: 'UserRoleComponentList.html',
    providers: [UserRoleService],
})
export class UserRoleComponentList {
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public UserRoleList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public ControlRights: any;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public loader: LoaderService, public _UserRoleService: UserRoleService
        , public _router: Router, public toastr: CommonToastrService, ) {
        this.loader.ShowLoader();
        //this.Keywords = this._CommonService.GetKeywords("UserRole");
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("2");
    }
    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = "Role";
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "RoleName", Title: "Role Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Employees", Title: "Role Count", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    //Refresh() {
    //    this.loader.ShowLoader();
    //    this._UserRoleService.GetList().then(m => {
    //        //this.PModel.TotalRecord = m.TotalRecord;
    //        this.UserRoleList = m.ResultSet;
    //        this.loader.HideLoader();
    //    });
    //}
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._UserRoleService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                
                this.PModel.TotalRecord = m.TotalRecord;
                this.UserRoleList = m.DataList;
                this.loader.HideLoader();
            });
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._UserRoleService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddNewRecord(id: string, SystemGenerated: boolean) {
        if (SystemGenerated) {
            this.toastr.Error('Error', "Sorry, predefined roles cannot be edited or deleted. Clone the role instead.");
        } else {
            //if (id != "0")
            //    this.loader.ShowLoader();
            //this.Id = id;
            //this.IsList = false;
            if (id != "0") {
                this.loader.ShowLoader();
                this._router.navigate(['home/userrole/saverole']);
            }
            this.Id = id;
            this.IsList = false;
            this._router.navigate(['home/userrole/saverole'], { queryParams: { id: this.Id } });
        }
    }
    AddRecord(id: string) {     
            //if (id != "0")
            //    this.loader.ShowLoader();
            //this.Id = id;
            //this.IsList = false;
            if (id != "0") {
                this.loader.ShowLoader();
                this._router.navigate(['home/userrole/saverole']);
            }
            this.Id = id;
            this.IsList = false;
            this._router.navigate(['home/userrole/saverole'], { queryParams: { id: this.Id } });
    }
    
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
        //if (SystemGenerated) {
        //    this.toastr.Error('Error', "Sorry, predefined roles cannot be edited or deleted. Clone the role instead.");
        //} else {
            var result = confirm("Are you sure you want to delete selected record.");
            if (result == true) {
                this.loader.ShowLoader();
                this._UserRoleService.Delete(id).then(m => {

                    if (m.ErrorMessage != null) {

                        alert(m.ErrorMessage);
                    }
                    this.Refresh();
                });
            //}
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