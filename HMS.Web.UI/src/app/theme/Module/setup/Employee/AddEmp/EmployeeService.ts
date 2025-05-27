import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { HttpService } from '../../../../../CommonService/HttpService';
import { EmployeeModel, EmpBulkUpdateModel, BulkEmpModel, ImportEmpDataModel } from './EmployeeModel';

@Injectable()
export class EmployeeService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Employee/pr_employee_mf"

    private _EmpID: string;

    set EmpID(value: string) {
        this._EmpID = value;
    }

    get EmpID(): string {
        return this._EmpID;
    }

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
        return this.http.Get(this.urlToApi + '/GetById', data).then(m => m);
    }
    GetAllEmps(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetAllEmps', {}).then(m => m);
    }

    ImportEmpData(EmpData: any): Promise<any> {
        return this.http.Post(this.urlToApi + '/ImportEmpData', EmpData).then(m => m);
    }
    IsImportDataExistInDB(EmpData: any): Promise<any> {
        return this.http.Post(this.urlToApi + '/IsImportDataExistInDB', EmpData).then(m => m);
    }

    GetCitesOfCountry(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetCitesOfCountry', data).then(m => m);
    }

    SaveOrUpdate(entity: EmployeeModel): Promise<any> {
        
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }
    UpdateBulkEmp(entity: Array<BulkEmpModel>): Promise<any> {
        return this.http.Post(this.urlToApi + '/UpdateBulkEmp', entity).then(e => e);
    }


    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }

    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/FormLoad', {}).then(e => e);
    }


    GetBulkFilterData(AllConDedType: string): Promise<any> {
        var data = { 'AllConDedType': AllConDedType };
        return this.http.Get(this.urlToApi + '/GetBulkFilterData', data).then(e => e);
    }

    GetBulkFilterEmployees(entity: EmpBulkUpdateModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/GetBulkFilterEmployees', entity).then(e => e);
    }


    GetBulkEmpPagination(BulkModel: EmpBulkUpdateModel, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'BulkModel': BulkModel, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/BulkEmpPagination', data).then(e => e);
    }
    downloadPDF(): any {
        
        return this.http.Get(this.urlToApi, { responseType: 'blob' })
            .then(res => {
                return new Blob([res], { type: 'application/pdf', });
            });
    }
}