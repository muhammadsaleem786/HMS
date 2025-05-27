import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AllowanceModel } from './AllowanceModel';
import { AllowanceService } from './AllowanceService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

@Component({
    moduleId: module.id,
    templateUrl: 'AllowanceComponentList.html',
    providers: [AllowanceService],
})

export class AllowanceComponentList {

    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public AllowanceList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public ControlRights: any;
    public valueForUser: any;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public loader: LoaderService, public _allowanceService: AllowanceService, public _router: Router) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("51");
    }

    ngOnInit() {

        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = "Allowances";
        // this.PConfig.Action.ScreenName = this.Rights['ScreenName'];
        // this.PConfig.Action.Add = this.Rights.Allow('Add');
        // this.PConfig.Action.Edit = this.Rights.Allow('Update');
        // this.PConfig.Action.View = this.Rights.Allow('View');
        // this.PConfig.Action.Delete = this.Rights.Allow('Delete');
        // this.PConfig.Action.Print = this.Rights.Allow('Print');
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "AllowanceName", Title: "Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "AllowanceValue", Title: "Amount / Percentage", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }

    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();

        this.Id = "0";
        this._allowanceService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.AllowanceList = m.DataList;
                this.loader.HideLoader();
            });
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._allowanceService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }


    AddRecord(id: string) {
        if (id != "0")
            this.loader.ShowLoader();

        this.Id = id;
        this.IsList = false;
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
            this._allowanceService.Delete(id).then(m => {

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