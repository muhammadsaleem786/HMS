import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { HttpService } from '../../../../../CommonService/HttpService';
import { VendorModel } from './VendorModel';
@Injectable()
export class VendorService {

    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Admin/pur_vendor"
    constructor(private http: HttpService) { }

    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }
    LoadDropdown(Ids: string): Promise<any> {
        var data = { 'DropdownIds': Ids };
        return this.http.Get(this.urlToApi + '/LoadDropdown', data).then(e => e);
    }
    GetItemList(): Promise<any> {
        var data = {};
        return this.http.Get(this.urlToApi + '/GetList', data).then(e => e);
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
        return this.http.Get(this.urlToApi + '/GetById', data).then(e => e);
    }
    UpdateStatus(id: string, statusId: string): Promise<any> {
        var data = { 'id': id, 'statusId': statusId };
        return this.http.Get(this.urlToApi + '/UpdateStatus', data).then(e => e);
    }
   
    SaveOrUpdate(entity: VendorModel): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }
    ExportExcel(ExportType: number, FromDate: string, ToDate: string): Promise<any> {
        var data = { 'ExportType': ExportType, 'FromDate': FromDate, 'ToDate': ToDate};
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
    ChangeStatus(ids: any): Promise<any> {
        return this.http.Post(this.urlToApi + '/ChangeStatus', ids).then(m => m);
    }
    ExpItemsView(ExportType: number): Promise<any> {
        var data = { 'ExportType': ExportType };
        return this.http.Get(this.urlToApi + '/ExpItemList', data).then(e => {

            if (e.FilePath != "")
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);

            return e;
        });
    }
}
