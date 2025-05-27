import { Component, OnInit, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Service } from './Service';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { emr_service_mf } from '../../Setting/Service/ServiceModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
@Component({
    moduleId: module.id,
    templateUrl: 'ServiceComponentList.html',
    providers: [Service],
})
export class ServiceComponentList {
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public ServiceList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public model = new emr_service_mf();
    public ControlRights: any;
    constructor(public _fb: FormBuilder, private modalService: NgbModal, public _CommonService: CommonService, public loader: LoaderService, public _Service: Service
        , public _router: Router, public toastr: CommonToastrService, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.ControlRights = this._CommonService.ScreenRights("65");
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
    }
    ngOnInit() {
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = 'Service';
        this.PConfig.Action.Add = false;
        this.PConfig.Fields = [
            { Name: "ServiceName", Title: "Service Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Price", Title: "Price", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "RefCode", Title: "Ref Code", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._Service
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.ServiceList = m.DataList;
                this.loader.HideLoader();
            });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Service/saveservice']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Service/saveservice'], { queryParams: { id: this.Id } });
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
            this._Service.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                else {
                    this.Refresh();
                }
                this.loader.HideLoader();
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
        this._Service.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
}