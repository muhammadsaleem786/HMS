import { Injectable } from '@angular/core';
import { InvoiceModel, BatchModel } from './InvoiceModel';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { HttpService } from '../../../../../CommonService/HttpService';
@Injectable()
export class InvoiceService {

    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Admin/pur_invoice_mf"
    constructor(private http: HttpService) { }
    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, IgnorePaging: boolean): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': IgnorePaging };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }
    LoadDropdown(Ids: string): Promise<any> {
        var data = { 'DropdownIds': Ids };
        return this.http.Get(this.urlToApi + '/LoadDropdown', data).then(e => e);
    }
    SearchVandorByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/SearchVandorByName', data).then(e => e);
    }
    ExportData(ExportType: number, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {

        var data = { 'ExportType': ExportType, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': true };
        return this.http.Get(this.urlToApi + '/ExportData', data).then(e => {

            if (e.FilePath != "")
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);
            return e;
        });
    }
    GetById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetByIdParam', data).then(e => e);
    }
    ExpCurrentView(ExportType: number): Promise<any> {
        var data = { 'ExportType': ExportType };
        return this.http.Get(this.urlToApi + '/ExpCurrentView', data).then(e => {
            if (e.FilePath != "")
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);
            return e;
        });
    }
    SaveOrUpdate(entity: InvoiceModel): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }
    ExportExcel(ExportType: number, FromDate: string, ToDate: string): Promise<any> {
        var data = { 'ExportType': ExportType, 'FromDate': FromDate, 'ToDate': ToDate };
        return this.http.Get(this.urlToApi + '/ExportExcel', data).then(e => {
            if (e.FilePath != "" && e.FilePath != null)
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);
            return e;
        });
    }
    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }
    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }
    DeleteMilti(ids: any): Promise<any> {
        return this.http.Post(this.urlToApi + '/DeleteMultiple', ids).then(m => m);
    }
}
