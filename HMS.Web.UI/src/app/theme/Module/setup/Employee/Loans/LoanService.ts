import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { HttpService } from '../../../../../CommonService/HttpService';
import { LoanModel, pr_loan_payment_dt, AdjustmentModel } from './LoanModel';


@Injectable()
export class LoanService {


    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Employee/pr_loan"

    constructor(private http: HttpService) { }

    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }

    GetDetList(LoanID: string, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'LoanID': LoanID, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/PaginationDetail', data).then(e => e);
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

    GetLoanDetail(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetLoanDetail', data).then(e => e);
    }

    SaveOrUpdate(entity: LoanModel): Promise<any> {

        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }

    AddPayment(entity: pr_loan_payment_dt): Promise<any> {
        return this.http.Post(this.urlToApi + '/AddPayment', entity).then(e => e);
    }
    AddAdjustment(entity: AdjustmentModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/AddAdjustment', entity).then(e => e);
    }
    DeleteAdjustment(Id: string, LoanScreenName : string): Promise<any> {
        var data = { 'Id': Id, 'LoanScreenName': LoanScreenName};
        return this.http.Get(this.urlToApi + '/DeleteAdjustment', data).then(e => e);
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
        return this.http.Get(GlobalVariable.BASE_Api_URL + "/api/Payroll/pr_employee_mf" + '/GetFilterEmployees', data);
    }
}
