import { Injectable } from '@angular/core';
import { SaleModel } from './SaleModel';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { HttpService } from '../../../../../CommonService/HttpService';
@Injectable()
export class SaleService {

    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Admin/pur_sale_mf"
    private urlToHoldApi = GlobalVariable.BASE_Api_URL + "/api/Admin/pur_sale_hold_mf"
    constructor(private http: HttpService) { }
    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }
    searchByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/ServiceItemByName', data).then(e => e);
    }
    searchByNamePrescription(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/searchByNamePrescription', data).then(e => e);
    }

    SaleItemByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/SaleItemByName', data).then(e => e);
    }
    ServiceItemByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/ServiceItemByName', data).then(e => e);
    }

    SearchAllItemByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/SearchAllItemByName', data).then(e => e);
    }
    searchBatchNo(itemId: string, term: string): Promise<any> {
        var data = { 'itemId': itemId, 'term': term };
        return this.http.Get(this.urlToApi + '/searchBatchNo', data).then(e => e);
    }
    LoadDropdown(Ids: string): Promise<any> {
        var data = { 'DropdownIds': Ids };
        return this.http.Get(this.urlToApi + '/LoadDropdown', data).then(e => e);
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
    GetHoldSaleById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToHoldApi + '/GetByIdParam', data).then(e => e);
    }
    SaveOrUpdate(entity: SaleModel): Promise<any> {
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
