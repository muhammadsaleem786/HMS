import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IncomeService } from './IncomeService';
import { emr_Income } from './IncomeModel';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../CommonService/CommonService';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { AsidebarService } from '../../../../CommonService/AsidebarService';
import { ActivatedRoute } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
    moduleId: module.id,
    templateUrl: 'IncomeComponentList.html',
    providers: [IncomeService],
})
export class IncomeComponentList implements OnInit {
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public IncomeList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public ControlRights: any;
    selectedValue: string = "A";
    public CategoryList: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    constructor(public _CommonService: CommonService, private encrypt: EncryptionService, public loader: LoaderService, public _IncomeService: IncomeService
        , public _router: Router, public route: ActivatedRoute, public toastr: CommonToastrService, public _AsidebarService: AsidebarService) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("20");
    }
    ngOnInit() {
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        let selectValue = localStorage.getItem('selectedValue');
        if (selectValue != "null" && selectValue != "") {
            this.selectedValue = selectValue;
            if (selectValue == "D") {
                this.PModel.FilterID = "0";
                this.PModel.VisibleFilter = true;
            } else if (selectValue == "A") {
                this.PModel.FilterID = "0";
                this.PModel.VisibleFilter = false;
            } else {
                this.PModel.FilterID = this.selectedValue
                this.PModel.VisibleFilter = false;
            }
        }
        this.PModel.VisibleColumnInfo = "Date#Date,Category#Category,DueAmount#DueAmount,ReceivedAmount#ReceivedAmount";
        this._IncomeService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID, this.PModel.VisibleFilter).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.IncomeList = m.DataList;
                this.CategoryList = m.OtherDataModel;
                this.CategoryList.push({ "ID": "A", "Value": "All" }, { "ID": "D", "Value": "Due" })
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    getFilterData() {
        localStorage.setItem('selectedValue', this.selectedValue);
        this.Refresh();
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Income/saveIncome']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Income/saveIncome'], { queryParams: { id: this.Id } });
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
            this._IncomeService.Delete(id).then(m => {
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
        this._IncomeService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.Refresh();
    }
    getPages(totalRecord: number, recordPerPage: number) {

        if (!isNaN(totalRecord))
            this.totalPages = this.getTotalPages(totalRecord, recordPerPage);
        this.getpagesRange();
    }
    getpagesRange() {
        if (this.pagesRange.indexOf(this.PModel.CurrentPage) == -1 || this.totalPages <= 10)
            this.papulatePagesRange();
        else if (this.pagesRange.length == 1 && this.totalPages > 10)
            this.papulatePagesRange();
    }
    papulatePagesRange() {
        this.pagesRange = [];
        var Result = Math.ceil(Math.max(this.PModel.CurrentPage, 1) / Math.max(this.PModel.RecordPerPage, 1));
        this.previousPage = ((Result - 1) * this.PModel.RecordPerPage)
        this.nextPage = (Result * this.PModel.RecordPerPage) + 1;
        if (this.nextPage > this.totalPages)
            this.nextPage = this.totalPages;
        for (var i = 1; i <= 10; i++) {
            if ((this.previousPage + i) > this.totalPages) return;
            this.pagesRange.push(this.previousPage + i)
        }
    }
    getTotalPages(totalRecord: number, recordPerPage: number): number {

        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
    }
    isToday(date: string | Date): boolean {
        const today = new Date();
        const givenDate = new Date(date);
        return (
            today.getFullYear() === givenDate.getFullYear() &&
            today.getMonth() === givenDate.getMonth() &&
            today.getDate() === givenDate.getDate()
        );
    }
}