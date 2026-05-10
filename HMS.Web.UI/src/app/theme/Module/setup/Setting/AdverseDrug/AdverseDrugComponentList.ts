import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdverseDrugService } from './AdverseDrugService';
import { ipd_adverse_drug_reaction } from '../AdverseDrug/AdverseDrugModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    moduleId: module.id,
    templateUrl: 'AdverseDrugComponentList.html',
    providers: [AdverseDrugService],
})
export class AdverseDrugComponentList {
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public AdverseDrugList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public ControlRights: any;
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public loader: LoaderService, public _AdverseDrugService: AdverseDrugService
        , public _router: Router, public toastr: CommonToastrService, ) {
        this.loader.ShowLoader();
        //this.Keywords = this._CommonService.GetKeywords("Medicine");
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("109");
    }
    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'AdverseDrug';
        this.PConfig.Action.Add = false;
        this.PConfig.Fields = [
            { Name: "Date", Title: "Date", OrderNo: 1, SortingAllow: true, Visible: true, isDate: true, DateFormat: "dd/MM/yyyy" },
            { Name: "PatientName", Title: "Patient Name", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Age", Title: "Age", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Contact", Title: "Contact", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "MedicineGiven", Title: "Medicine Given", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "ReactionObserved", Title: "Reaction Observed", OrderNo: 6, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "ActionTaken", Title: "Action Taken", OrderNo: 7, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Remarks", Title: "Remarks", OrderNo: 8, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._AdverseDrugService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.AdverseDrugList = m.DataList;
                this.loader.HideLoader();
            });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/AdverseDrug/saveAdverseDrug']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/AdverseDrug/saveAdverseDrug'], { queryParams: { id: this.Id } });
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
                this._AdverseDrugService.Delete(id).then(m => {

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
        this._AdverseDrugService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
}