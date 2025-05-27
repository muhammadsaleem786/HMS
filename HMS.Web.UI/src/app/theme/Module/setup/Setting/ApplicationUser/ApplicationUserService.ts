import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { HttpService } from '../../../../../CommonService/HttpService';
import { ApplicationUserModel } from './ApplicationUserModel';


@Injectable()
export class ApplicationUserService {


    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Admin/adm_user"

    constructor(private http: HttpService) { }

    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
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

    SaveOrUpdate(entity: ApplicationUserModel): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }

    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }

    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }

    GetFilterEmployees(Keyword: string): Promise<any> {
        var data = { 'Keyword': Keyword };
        return this.http.Get(this.urlToApi + '/GetFilterEmployees', data);
    }

    GetRoles(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetRoles', {});
    }

  
    IsImportDataExistInDB(EmpData: any): Promise<any> {
        return this.http.Post(this.urlToApi + '/IsImportDataExistInDB', EmpData).then(m => m);
    }
    ImportEmpData(EmpData: any): Promise<any> {
        return this.http.Post(this.urlToApi + '/ImportUserData', EmpData).then(m => m);
    }
    UserUpdate(Doctorids: any): Promise<any> {
        var data = { 'Doctorids': Doctorids };
        return this.http.Get(this.urlToApi + '/UserUpdate', data);
    }
}
