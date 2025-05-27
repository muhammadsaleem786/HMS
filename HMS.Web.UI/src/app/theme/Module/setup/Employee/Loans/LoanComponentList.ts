import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoanModel, pr_loan_payment_dt, PayrollTempDetailModel, AdjustmentModel, PaginationSumModel, PaginationDetailSumModel } from './LoanModel';
import { LoanService } from './LoanService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { MatPaginator, MatSort, MatTableDataSource, MatTable, PageEvent } from '@angular/material';
import { DataSource } from '@angular/cdk/table';
import { SelectionModel } from '@angular/cdk/collections';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

export interface Element {
    select: false,
    //ID: number,
    Transaction: string,
    LoanDate: string,
    LoanAmount: number,
    Payment: number,
    Balance: number,
}

@Component({
    moduleId: module.id,
    templateUrl: 'LoanComponentList.html',
    providers: [LoanService],
})

export class LoanComponentList {

    dataSource;
    displayedColumns = [];
    @ViewChild(MatSort) sort: MatSort;
    @ViewChild(MatPaginator) paginator: MatPaginator;
    public Rights: any;
    public ControlRights: any;
    columnNames = [
        {
            id: "Transaction",
            value: "Transaction"
        },
        {
            id: "LoanDate",
            value: "LoanDate"
        },
        {
            id: "LoanAmount",
            value: "LoanAmount"
        },
        {
            id: "Payment",
            value: "Payment"
        },
        {
            id: "Balance",
            value: "Balance"
        }
    ];
    public PaymentModel = new pr_loan_payment_dt();
    public AdjustmentModel = new AdjustmentModel();
    public PagSumModel = new PaginationSumModel();
    public PagDetailSumModel = new PaginationDetailSumModel();
    public Id: string;
    public LoanID: number = 0;
    public PRDModel = new PayrollTempDetailModel();
    public PRDListModel = new Array<PayrollTempDetailModel>();
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public LoanList: any[] = [];
    public PDetModel = new PaginationModel();
    public PDetConfig = new PaginationConfig();
    public LoanDetList: any[] = [];
    public IsShowLoanDetailModal: boolean = false;
    public IsList: boolean = true;
    public ID: number = 10;    
    public Currency: string = '';
    public PayrollRegion: string = '';
    public submitted: boolean = false;
    public Adjustmentsubmitted: boolean = false;
    IsCallShowHideFun: boolean = true;
    PaymentForm: FormGroup;
    AdjustmentForm: FormGroup;

    length = 100;
    pageSize = 5;
    pageSizeOptions: number[] = [5, 10, 25, 100];

    selection = new SelectionModel<Element>(true, []);

    /** Whether the number of selected elements matches the total number of rows. */
    // MatPaginator Output
    pageEvent: PageEvent;

    setPageSizeOptions(setPageSizeOptionsInput: string) {
        this.pageSizeOptions = setPageSizeOptionsInput.split(',').map(str => +str);
    }

    constructor(public _fb: FormBuilder, public _CommonService: CommonService, public loader: LoaderService, public _loanservice: LoanService
        , public _router: Router, public toastr: CommonToastrService, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.Currency = this._CommonService.getCurrency();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("60");
    }

    ngOnInit() {
        this.displayedColumns = this.columnNames.map(x => x.id);

        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = "Loans";
        this.PConfig.Action.Add = true;
        this.PConfig.Action.AddTextType = "T";
        this.PConfig.Action.AddText = "Add";
        this.PConfig.Action.Add = true;
        this.PConfig.Fields = [
            { Name: "EmpName", Title: "Employee", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: false },
            { Name: "Description", Title: "Description", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: false },
            { Name: "LoanDate", Title: "Loan Date", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: false },
            { Name: "LoanAmount", Title: "Loan", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
            { Name: "PaymentAmount", Title: "Payment", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
            { Name: "Balance", Title: "Balance", OrderNo: 6, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
        ];


        this.PDetConfig.PrimaryColumn = "ID";
        this.PDetConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PDetConfig.Action.ScreenName = "Loan Detail";
        // this.PConfig.Action.ScreenName = this.Rights['ScreenName'];
        // this.PConfig.Action.Add = this.Rights.Allow('Add');
        // this.PConfig.Action.Edit = this.Rights.Allow('Update');
        // this.PConfig.Action.View = this.Rights.Allow('View');
        // this.PConfig.Action.Delete = this.Rights.Allow('Delete');
        // this.PConfig.Action.Print = this.Rights.Allow('Print');
        this.PDetConfig.Action.Add = true;
        this.PDetConfig.Fields = [
            { Name: "Transaction", Title: "Transaction", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Date", Title: "Date", OrderNo: 2, SortingAllow: true, Visible: true, isDate: true, DateFormat: "" },
            { Name: "LoanAmount", Title: "Loan Amount", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Payment", Title: "Payment", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Balance", Title: "Balance", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];

        this.PaymentForm = this._fb.group({
            PaymentDate: ['', [Validators.required]],
            Amount: ['', [Validators.required]],
            Comment: ['', [Validators.required]],
        });

        this.AdjustmentForm = this._fb.group({
            AdjustmentDate: ['', [Validators.required]],
            AdjustmentType: ['', [Validators.required]],
            AdjustmentAmount: ['', [Validators.required]],
            AdjustmentComments: ['', [Validators.required]],
        });
    }

    createTable(dataList) {
        let tableArr: Element[] = dataList;
        this.dataSource = new MatTableDataSource(tableArr);
        this.dataSource.sort = this.sort;
        //if (this.dataSource.data.length > 3) {
        this.paginator.pageSize = 5
        this.paginator.pageIndex = 0
        this.dataSource.paginator = this.paginator;
        //}
    }

    Refresh() {
        this.loader.ShowLoader();
        this.Id = "0";
        this._loanservice
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.LoanList = m.DataList;
                this.PagSumModel = new PaginationSumModel();
                this.LoanList.forEach(x => {
                    this.PagSumModel.LoanSum += x.LoanAmount;
                    this.PagSumModel.PaymentSum += x.PaymentAmount;
                    this.PagSumModel.BalanceSum += x.Balance;
                });
                this.loader.HideLoader();
            });
    }

    DetRefresh() {
        this.loader.ShowLoader();
        this.Id = "0";
        this._loanservice
            .GetDetList(this.Id, this.PDetModel.CurrentPage, this.PDetModel.RecordPerPage, this.PDetModel.VisibleColumnInfo, this.PDetModel.SortName, this.PDetModel.SortOrder, this.PDetModel.SearchText).then(m => {
                this.PDetModel.TotalRecord = m.TotalRecord;
                this.LoanDetList = m.DataList;
                this.loader.HideLoader();
            });
    }


    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._loanservice.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }


    AddRecord(id: string) {
        //if (id != "0")
        if (id) {
            this.IsShowLoanDetailModal = true;
            this.loader.ShowLoader();
            this._loanservice.GetLoanDetail(id).then(m => {
                if (m.IsSuccess) {
                    this.PRDModel = m.ResultSet.length > 0 ? m.ResultSet[0] : 0
                    this.PaymentModel.Amount = this.PRDModel.DeductionValue;
                    this.PaymentModel.PaymentDate = new Date();
                    this.PaymentModel.LoanID = this.PRDModel.ID;
                    this.PRDListModel = m.ResultSet;
                    this.PRDListModel = this.PRDListModel.sort((a, b) => new Date(a.LoanDate).getTime() - new Date(b.LoanDate).getTime());
                    this.PagDetailSumModel = new PaginationDetailSumModel();

                    var RemBalance = 0.0;
                    var CBalance = 0.0;
                    this.PRDListModel.forEach(x => {
                        if (x.ScreenType == 'LMF') {
                            CBalance = x.LoanAmount + this.getAdjustment(x);
                            this.PagDetailSumModel.LoanSum += CBalance;
                            RemBalance += CBalance;
                        }
                        else {
                            CBalance = x.Payment + this.getAdjustment(x);
                            this.PagDetailSumModel.PaymentSum += CBalance;
                            RemBalance -= CBalance;
                        }
                        x.Balance = RemBalance;
                    });
                    this.PagDetailSumModel.BalanceSum = this.PagDetailSumModel.LoanSum - this.PagDetailSumModel.PaymentSum;
                    this.createTable(m.ResultSet);
                    if (this.IsCallShowHideFun)
                        this.ShowHideModal('HideAd');

                    this.IsCallShowHideFun = true;
                }
                this.loader.HideLoader();
            });
        } else {
            this.IsShowLoanDetailModal = false;
            this.Id = id;
        }
        this.IsList = false;
    }

    getAdjustment(item): number {
        if (item.AdjustmentBy) {
            var val = item.AdjustmentType == 'C' ? item.AdjustmentAmount : (item.AdjustmentAmount * -1);
            return val;
        }
        else return 0;
    }

    AddDetRecord(id: string) {


        if (id != "0")
            this.loader.ShowLoader();

        this.Id = id;
        this.IsList = false;
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
            this._loanservice.Delete(id).then(m => {

                if (m.ErrorMessage != null) {

                    alert(m.ErrorMessage);
                }
                this.Refresh();
            });
        }
    }

    IsValidPaymentValues(): boolean {
        var Paydate = new Date(this.PaymentModel.PaymentDate);
        var Loandate = new Date(this.PRDModel.LoanDate);
        if (Paydate < Loandate) {
            this.toastr.Error("Invalid date", "Payment date can't be less than the loan date(" + this.GetFormatDate(this.PRDModel.LoanDate) + ")");
            return false;
        }
        else if (this.PaymentModel.Amount == 0) {
            this.toastr.Error("Invalid amount", "Please enter valid recovery amount.");
            return false;
        }
        else if (this.PaymentModel.Amount > this.PRDModel.TotalBalance) {
            this.toastr.Error("Invalid amount", "Recovery Amount can't be greater than the remaining loan balance");
            return false;
        }
        return true;
    }

    AddPayment(isValid: boolean): void {
        this.submitted = true; // set form submit to true

        if (isValid)
            isValid = this.IsValidPaymentValues();



        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            //this.PaymentModel.LoanID = this.LoanID;
            this._loanservice.AddPayment(this.PaymentModel).then(m => {

                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    this.Refresh();
                    this.IsCallShowHideFun = false;
                    this.AddRecord(this.PaymentModel.LoanID.toString());
                    document.getElementById('CloseAddPaymentModal').click();
                    //this.ShowHideModal('HideAd');
                }
                else {
                    this.loader.HideLoader();
                    //this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }

    IsValidAdjustmentValues(): boolean {
        var AdjusDate = new Date(this.AdjustmentModel.AdjustmentDate);
        var TransDate = new Date(this.AdjustmentModel.TransactionDate);

        if (AdjusDate < TransDate) {
            this.toastr.Error("Invalid date", "Adjustment date can't be less than the transaction date(" + this.GetFormatDate(this.AdjustmentModel.TransactionDate) + ")");
            return false;
        }
        return true;
    }

    GetFormatDate(dat): string {
        var date = new Date(dat);  //or your date here
        return (date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear());
    };

    AddAdjustment(isValid: boolean): void {
        this.Adjustmentsubmitted = true; // set form submit to true
        if (isValid)
            isValid = this.IsValidAdjustmentValues();

        if (isValid) {
            this.Adjustmentsubmitted = false;
            this.loader.ShowLoader();
            this._loanservice.AddAdjustment(this.AdjustmentModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    //this.ShowHideModal('HideAd');
                    this.Refresh();
                    this.IsCallShowHideFun = false;
                    this.AddRecord(this.AdjustmentModel.LoanMfID.toString());
                    document.getElementById('CloseAdjustModal').click();
                }
                else {
                    this.loader.HideLoader();
                    //this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }


    GetAdjustmentModel(id: number, ScreenType: string) {
        ;
        if (id) {
            var item = this.PRDListModel.filter(x => x.ID == id && x.ScreenType == ScreenType);
            if (item.length > 0) {
                this.AdjustmentModel.LoanMfID = this.PRDModel.ID;
                this.AdjustmentModel.LoanDtID = item[0].ID;
                this.AdjustmentModel.LoanScreenType = item[0].ScreenType;
                //this.PRDModel = item[0];
                this.AdjustmentModel.AdjustmentTitle = item[0].EmpName;
                this.AdjustmentModel.AdjustmentText = item[0].ScreenType == 'LMF' ? 'Loan' : 'Paid';
                this.AdjustmentModel.AdjustmentValue = item[0].ScreenType == 'LMF' ? item[0].LoanAmount : item[0].Payment;
                this.AdjustmentModel.OrignalValue = item[0].ScreenType == 'LMF' ? item[0].LoanAmount : item[0].Payment;
                this.AdjustmentModel.TransactionDate = item[0].LoanDate;
                if (item[0].AdjustmentBy) {
                    this.AdjustmentModel.AdjustmentComments = item[0].AdjustmentComments;
                    this.AdjustmentModel.AdjustmentAmount = item[0].AdjustmentAmount;
                    this.AdjustmentModel.AdjustmentBy = item[0].AdjustmentBy;
                    this.AdjustmentModel.AdjustmentDate = item[0].AdjustmentDate;
                    this.AdjustmentModel.AdjustmentType = item[0].AdjustmentType;
                    this.AdjustmentModel.AdjustmentValue = this.AdjustmentModel.AdjustmentType == 'C' ? (this.AdjustmentModel.AdjustmentValue + item[0].AdjustmentAmount) : (this.AdjustmentModel.AdjustmentValue - item[0].AdjustmentAmount);
                } else {
                    this.AdjustmentModel.AdjustmentComments = '';
                    this.AdjustmentModel.AdjustmentAmount = 0;
                    this.AdjustmentModel.AdjustmentBy = 0;
                    this.AdjustmentModel.AdjustmentDate = new Date();
                    this.AdjustmentModel.AdjustmentType = 'C';
                }

                this.ShowHideModal('ShowAd');
            }
        }
    }

    ShowHideModal(ModalName) {

        if (ModalName == 'ShowAP') {
            document.getElementById('CloseLoanDetailModal').click();
        } else if (ModalName == 'ShowAd') {
            document.getElementById('CloseLoanDetailModal').click();
        }
        else if (ModalName == 'HideAP') {
            //document.getElementById('CloseAddPaymentModal').click();
            document.getElementById('ShowLoanDetailModalId').click();
        }
        else if (ModalName == 'HideAd') {
            //document.getElementById('CloseAdjustModal').click();
            document.getElementById('ShowLoanDetailModalId').click();
        }
        else if (ModalName == 'Cancel') {
            document.getElementById('ShowLoanDetailModalId').click();
        }
    }

    CreditOrDebitAmount() {
        this.AdjustmentModel.AdjustmentValue = this.AdjustmentModel.AdjustmentType == 'C' ? this.AdjustmentModel.OrignalValue + this.AdjustmentModel.AdjustmentAmount : this.AdjustmentModel.OrignalValue - this.AdjustmentModel.AdjustmentAmount;
    }

    GetList() {
        this.Refresh();
    }
    GetDetList() {
        this.DetRefresh();
    }

    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else
            this.Refresh();
    }



    //GetTotalBalance(element: any): number {
    //    if (element && element.AdjustmentType != undefined) {
    //        var result = element.AdjustmentType == 'C' ? (element.TotalBalance + element.AdjustmentAmount) : (element.TotalBalance - element.AdjustmentAmount);
    //        if (element.ScreenType == 'LDT' && this.PRDModel.AdjustmentBy) {
    //            result += this.PRDModel.AdjustmentType == 'C' ? (this.PRDModel.AdjustmentAmount) : (this.PRDModel.AdjustmentAmount * -1);
    //        }
    //        
    //        return result;
    //    }
    //    return 0;
    //}

    DeleteAdjustmentModel() {
        var res = confirm("Are you sure you want to delete this adjustment?");
        if (this.AdjustmentModel.AdjustmentBy && res) {
            this.loader.ShowLoader();
            this._loanservice
                .DeleteAdjustment(this.AdjustmentModel.LoanDtID.toString(), this.AdjustmentModel.LoanScreenType).then(m => {
                    this.loader.HideLoader();
                    this.Refresh();
                    this.IsCallShowHideFun = false;
                    this.AddRecord(this.AdjustmentModel.LoanMfID.toString());
                    document.getElementById('CloseAdjustModal').click();
                    this.AdjustmentModel = new AdjustmentModel();
                });
        }
    }

}