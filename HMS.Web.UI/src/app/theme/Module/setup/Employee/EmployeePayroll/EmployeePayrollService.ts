import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { HttpService } from '../../../../../CommonService/HttpService';
import { EmployeePayrollModel, SalaryModel, AdjustmentModel, PrEmpDtFilterModel } from './EmployeePayrollModel';

@Injectable()
export class PayrollService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Employee/pr_employee_payroll_mf"
    constructor(private http: HttpService) { }

    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }

    GetDetList(UniqueId: string, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'UniqueId': UniqueId, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/PaginationDetail', data).then(e => e);
    }
    GetFilterDetList(FilterParams: string, UniqueId: string, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'FilterParams': FilterParams, 'UniqueId': UniqueId, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/FilterPaginationDetail', data).then(e => e);
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

    GetPayStubOfEmployeeByPayrollMfID(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetPayStubOfEmployeeByPayrollMfID', data).then(m => m);
    }

    GetEmployeesByPayScheduleID(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetEmployeesByPayScheduleID', data).then(m => m);
    }

    //SaveOrUpdate(entity: EmployeeModel): Promise<any> {
    //    if (isNaN(entity.ID) || entity.ID == 0)d
    //        return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
    //    else
    //        return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    //}

    RunPayroll(entity: SalaryModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/RunPayroll', entity).then(e => e);
    }


    SavePayStubOfSingleEmployee(entity: EmployeePayrollModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/SavePayStubOfSingleEmployee', entity).then(e => e);
    }

    DeleteEmployeeFromPayrollDetList(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/DeleteEmployeeFromPayrollDetList', data).then(e => e);
    }

    Delete(Id: string): Promise<any> {
        var data = { 'payScheduleId': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }
    SaveAdjustmentModel(entity: AdjustmentModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/SaveAdjustmentModel', entity).then(e => e);
    }
    DeleteAdjustment(prdtId: string, AllDedConBasicSalAmntType: string): Promise<any> {
        var data = { 'idd': prdtId, 'AllDedConBasicSalAmntType': AllDedConBasicSalAmntType };
        return this.http.Get(this.urlToApi + '/DeleteAdjustment', data).then(e => e);
    }
    PublishPayroll(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/PublishPayroll', data).then(m => m);
    }

    GetPaySchedules(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetPaySchedules', {}).then(e => e);
    }

    FormPayrollListLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/FormPayrollListLoad', {}).then(e => e);
    }

  
}