import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApplicationUserModel } from './ApplicationUserModel';
import { ApplicationUserService } from './ApplicationUserService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';

@Component({
    moduleId: module.id,
    templateUrl: 'ApplicationUserComponentList.html',
    providers: [ApplicationUserService],
})

export class ApplicationUserComponentList {

    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public AppUserList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = []; public ControlRights: any;
    constructor(public _CommonService: CommonService, public loader: LoaderService
        , public _ApplicationUserService: ApplicationUserService, public _router: Router) {
        this.loader.ShowLoader();
        this.ControlRights = this._CommonService.ScreenRights("3");
        //this.Keywords = this._CommonService.GetKeywords("ApplicationUser");
    }

    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = this.Keywords['Key_ApplicationUser_User'];
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "UserName", Title: "Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "RoleName", Title: "Role", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Email", Title: "Email", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "LastLogin", Title: "Last Login", OrderNo: 4, SortingAllow: true, Visible: true, isDate: true, DateFormat: "dd/MM/yyyy hh:mm:ss a" },
        ];
    }

    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();

        this.Id = "0";
        this._ApplicationUserService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.AppUserList = m.DataList;
                this.loader.HideLoader();
            });
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._ApplicationUserService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }


    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/aplicationUser/saveuser']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/aplicationUser/saveuser'], { queryParams: { id: this.Id } });
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
            this._ApplicationUserService.Delete(id).then(m => {

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