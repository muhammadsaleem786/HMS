import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonService } from '../CommonService/CommonService';
import { DatePipe } from '@angular/common';
import { PaginationModel, PaginationConfig } from '../CommonComponent/PaginationComponentConfig';
//import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from '../AngularElement/multiselect-dropdown';
import { CookieService } from 'ngx-cookie-service'
import { HostListener } from '@angular/core';


@Component({
    selector: 'app-pagination',
    templateUrl: './PaginationComponent.html',
    //styleUrls: ['./app/AngularElement/multiselect-dropdown.css']
})

export class PaginationComponent implements OnInit, OnChanges {

    @Input() PModel: PaginationModel;
    @Input() PConfig: PaginationConfig;
    @Input() PData: any;
    @Input() FilterList: any[] = [];
    public Keywords: any[] = [];

    @Output() pageChange: EventEmitter<string> = new EventEmitter<string>();
    @Output() AddOrEditEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() PrintEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() GoBackEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() ViewEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() DeleteEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() CloneEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() TemplateEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() ExportEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() CopyToEvent: EventEmitter<string> = new EventEmitter<string>();
    @Output() CustomAction1Event: EventEmitter<string> = new EventEmitter<string>();
    @Output() CustomAction2Event: EventEmitter<string> = new EventEmitter<string>();
    @Output() CustomAction3Event: EventEmitter<string> = new EventEmitter<string>();
    @Output() BudgetvsActualsEvent: EventEmitter<string> = new EventEmitter<string>();


    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public IsRowcustomButton: boolean = true;
    DateFormat: string = "dd/MM/yyyy";

    public VisibleColumn: string[] = [];
    //public VisibleColumnOptions: IMultiSelectOption[] = [];

    //public ColumnSettings: IMultiSelectSettings = {
    //    pullRight: false,
    //    enableSearch: true,
    //    checkedStyle: 'checkboxes',
    //    buttonClasses: 'btn btn-default',
    //    selectionLimit: 0,
    //    closeOnSelect: false,
    //    showCheckAll: true,
    //    showUncheckAll: true,
    //    showSaveOption: true,
    //    dynamicTitleMaxItems: 0,
    //    maxHeight: '300px',
    //};

    //public ColumnTexts: IMultiSelectTexts = {
    //    checkAll: 'Check all',
    //    uncheckAll: 'Uncheck all',
    //    SaveOption: 'Save setting',
    //    checked: 'checked',
    //    checkedPlural: 'checked',
    //    searchPlaceholder: 'Search...',
    //    defaultTitle: 'Select',
    //};

    constructor(public _cookieService: CookieService, public _CommonService: CommonService) {
        //this.Keywords = this._CommonService.GetKeywords(window.location.href);
    }

    ngOnInit() {

        this.PModel.SortName = "";
        //this.ColumnTexts.SaveCookieName = this.PConfig.ColumnVisibilityCookieName;
        this.PConfig.Fields = this.PConfig.Fields.sort((OrderNo1, OrderNo2) => OrderNo1.OrderNo - OrderNo2.OrderNo);
        this.PapulateVisibleColumn();
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);


        //{ { ((PModel.CurrentPage - 1) * PModel.RecordPerPage) + 1 } } - {{((PModel.CurrentPage * PModel.RecordPerPage) > PModel.TotalRecord ? PModel.TotalRecord : (PModel.CurrentPage * PModel.RecordPerPage));
    }

    ngOnChanges() {

        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
    }



    CustomButton() {

        this.IsRowcustomButton = false;
    }
    RowButton(id: any) {
        if (this.IsRowcustomButton == true)
            this.AddOrEditEvent.emit(id);

        this.IsRowcustomButton = true;

    }

    getPages(totalRecord: number, recordPerPage: number) {

        if (!isNaN(totalRecord))
            this.totalPages = this.getTotalPages(totalRecord, recordPerPage);
        this.getpagesRange();
    }

    getTotalPages(totalRecord: number, recordPerPage: number): number {

        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
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

    isValidPageNumber(page: number, totalPages: number): boolean {

        return page > 0 && page <= totalPages;
    }

    selectPage(page: number) {

        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;

        this.PModel.CurrentPage = page;
        this.PModel.RecordPerPage = this.PModel.RecordPerPage;
        this.getpagesRange()
        this.pageChange.emit("");
    }
    ApplySorting(name: string) {

        if (this.PModel.SortName == name)
            this.PModel.SortOrder = (this.PModel.SortOrder == "Asc" ? "Desc" : "Asc");
        else {
            this.PModel.SortOrder = "Asc";
            this.PModel.SortName = name;
        }
        this.pageChange.emit("");
    }
    ShowSearch() {
        if (this.PModel.ShowSearch == true)
            this.PModel.ShowSearch = false;
        else
            this.PModel.ShowSearch = true;
    }
    RefreshList(Value: string) {

        this.PModel.VisibleColumnInfo = this.GetVisibleColumnInfo();
        this.PModel.CurrentPage = 1;
        this.pageChange.emit("");
    }

    PapulatePagingList() {

        this.pagesRange = [];
        this.selectPage(1);
    }

    PapulateVisibleColumn() {

        for (var i = 0; i <= this.PConfig.Fields.length - 1; i++) {
            if (this.PConfig.Fields[i].Visible == true)
                this.VisibleColumn.push(this.PConfig.Fields[i].Name);
            //this.VisibleColumnOptions.push({ id: this.PConfig.Fields[i].Name, name: this.PConfig.Fields[i].Title });
        }

        var VisibleColumn = this._cookieService.get(this.PConfig.ColumnVisibilityCookieName);
        if (typeof (VisibleColumn) != "undefined" && Object.keys(VisibleColumn).map(x => VisibleColumn[x]).length > 0)
            this.VisibleColumn = Object.keys(VisibleColumn).map(x => VisibleColumn[x]);

        this.PModel.VisibleColumnInfo = this.GetVisibleColumnInfo();


    }

    @HostListener('window:keyup', ['$event'])
    HostKeys(event: any) {
        if (this.PConfig.Action.Print != false) {
            if (event.key == 'p' && event.altKey == true && event.ctrlKey == true) {
                this.ExportType("2");
            }

            else if (event.key == 'p' && event.altKey == true) {
                this.ExportType("1");
            }
        }

        if (event.key == 'Delete' && event.altKey == true) {
            if (this.PConfig.Action.Delete != false) {
                this.Delete();
            }
        }

        else if (event.key == 'ArrowRight' && event.ctrlKey == true) {
            this.selectPage(this.nextPage);
        }

        else if (event.key == 'ArrowLeft' && event.ctrlKey == true) {
            this.selectPage(this.previousPage);
        }

        else if (event.key == 's' && event.altKey == true) {
            this.RefreshList('');
        }
    }

    Delete() {
        var Ids = $("input[name=ActionChkbox]:checked").map(
            function() { return this.nodeValue; }).get().join(",")

        if (Ids == "") {
            alert("Select atleast one record for delete.");
            return;
        }

        this.DeleteEvent.emit(Ids);
    }


    public GetVisibleColumnInfo(): string {

        var SelectedVisibleColumn = [];
        for (var i = 0; i <= this.PConfig.Fields.length - 1; i++) {
            if (this.VisibleColumn.indexOf(this.PConfig.Fields[i].Name) == -1) continue;
            SelectedVisibleColumn.push(this.PConfig.Fields[i].Name + "#" + this.PConfig.Fields[i].Title);
        }

        return SelectedVisibleColumn.join(",");
    }
    public convertToString(str: any): string {
        if (str == null)
            return "";
        else
            return str;
    }

    public ExportType(ExportType: string): void {
        this.PModel.VisibleColumnInfo = this.GetVisibleColumnInfo();
        this.ExportEvent.emit(ExportType);
    }
    //parseDate(dateString: string): Date | null {
    //    if (dateString != undefined) {
    //        const [day, month, year] = dateString.split('/').map(Number);
    //        return new Date(year, month - 1, day); // JavaScript months are 0-based
    //    }
    //}

    IsBoolean(ColumnName): boolean {
        if (typeof (ColumnName) === 'boolean')
            return true;
        else
            return false;
    }
    GetCellValueWithFormate(value: string, IsShowCurrency: boolean): any {
        if (IsShowCurrency && (typeof value == "number"))
            return IsShowCurrency ? (this._CommonService.getCurrency() + ' ' + this._CommonService.GetThousandCommaSepFormatDate(Math.round(value))) : this._CommonService.GetThousandCommaSepFormatDate(Math.round(value));
        else return value;
    }


}