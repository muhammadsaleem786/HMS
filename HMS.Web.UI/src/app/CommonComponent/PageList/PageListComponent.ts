import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonService } from '../../CommonService/CommonService';
import { PageListModel, PageListConfig } from '../../CommonComponent/PageList/PageListComponentConfig';
import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from '../../AngularElement/multiselect-dropdown';

@Component({
    selector: 'PageList-Component',
    templateUrl: './PageListComponent.html'
})

export class PageListComponent implements OnInit, OnChanges {

    public PModel = new PageListModel();
    public PConfig = new PageListConfig();
    public PData: any[] = [];
    public FilterList: any[] = [];
    public Keywords: any[] = [];

    @Output() AddEvent: EventEmitter<any[]> = new EventEmitter<any[]>();

    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];

    public VisibleColumn: string[] = [];
    public VisibleColumnOptions: IMultiSelectOption[] = [];

    public ColumnSettings: IMultiSelectSettings = {
        pullRight: false,
        enableSearch: true,
        checkedStyle: 'checkboxes',
        buttonClasses: 'btn btn-default',
        selectionLimit: 0,
        closeOnSelect: false,
        showCheckAll: true,
        showUncheckAll: true,
        showSaveOption: true,
        dynamicTitleMaxItems: 0,
        maxHeight: '300px',
    };

    public ColumnTexts: IMultiSelectTexts = {
        checkAll: 'Check all',
        uncheckAll: 'Uncheck all',
        SaveOption: 'Save setting',
        checked: 'checked',
        checkedPlural: 'checked',
        searchPlaceholder: 'Search...',
        defaultTitle: 'Select',
    };

    constructor(public _CommonService: CommonService) {
        this.Keywords = this._CommonService.GetKeywords(window.location.href);
    }

    ngOnInit() {
        ;
        this.PConfig.PrimaryColumn = "ProductMfID";
        this.PConfig.Fields = [
            { Name: "ProductCode", Title: "Code", Visible: true },
            { Name: "ProductName", Title: "Name", Visible: true },
            { Name: "ShortDescription", Title: "Short Desc", Visible: true }
        ];

        //this.PConfig.Fields = this.PConfig.Fields.sort((OrderNo1, OrderNo2) => OrderNo1.OrderNo - OrderNo2.OrderNo);
        this.PapulateVisibleColumn();
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);

    }

    ngOnChanges() {
        //this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
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
        this.Refresh();
    }

    RefreshList(Value: string) {
        this.PModel.VisibleColumnInfo = this.GetVisibleColumnInfo();
        this.PModel.CurrentPage = 1;
        this.Refresh();
    }

    PapulatePagingList() {
        this.pagesRange = [];
        this.selectPage(1);
    }

    PapulateVisibleColumn() {

        for (var i = 0; i <= this.PConfig.Fields.length - 1; i++) {
            if (this.PConfig.Fields[i].Visible == true)
                this.VisibleColumn.push(this.PConfig.Fields[i].Name);
            this.VisibleColumnOptions.push({ id: this.PConfig.Fields[i].Name, name: this.PConfig.Fields[i].Title });
        }

        //var VisibleColumn = this._cookieService.getObject(this.PConfig.ColumnVisibilityCookieName);
        //if (typeof (VisibleColumn) != "undefined" && Object.keys(VisibleColumn).map(x => VisibleColumn[x]).length > 0)
        //this.VisibleColumn = Object.keys(VisibleColumn).map(x => VisibleColumn[x]);

        this.PModel.VisibleColumnInfo = this.GetVisibleColumnInfo();


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

    Refresh() {

        ;
        this._CommonService.GetPaginationWithPageList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SearchText).then(m => {
            ;
            this.PModel.TotalRecord = m.TotalRecord;
            this.PData = m.DataList;
            this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        });
    }

    AddProduct() {

        ;
        var SelectedProduct: any[] = [];
        SelectedProduct = this.PData.filter(f => { return f.Mark == true });

        if (SelectedProduct.length == 0) {
            alert("Select atleast one record for Add.");
            return;
        }

        this.AddEvent.emit(SelectedProduct);
    }
}