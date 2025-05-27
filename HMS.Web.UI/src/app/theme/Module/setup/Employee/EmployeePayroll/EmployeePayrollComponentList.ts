import { Component, OnInit, HostListener, ChangeDetectorRef, ViewChild, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PayrollService } from './EmployeePayrollService';
import { EmployeePayrollModel, pr_employee_payroll_dt, AdjustmentModel, ProrateModel, PrEmpDtSalaryTotalModel, PrEmpDtFilterModel } from './EmployeePayrollModel';
import { Salary, EmployeeModel, pr_employee_allowance, pr_employee_ded_contribution } from '../../../setup/Employee/AddEmp/EmployeeModel';
import { AllowanceModel } from '../../../setup/Employee/Allowance/AllowanceModel';
import { DeductionContributionModel } from '../../../setup/Employee/DeductionContribution/DeductionContributionModel';
import { AllowanceService } from '../../../setup/Employee/Allowance/AllowanceService';
import { DeductionContributionService } from '../../../setup/Employee/DeductionContribution/DeductionContributionService';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { IMyDateModel } from 'mydatepicker';
import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from 'angular-2-dropdown-multiselect';
import '../../../../../AngularElement/multiselect-dropdown';
import { EncryptionService } from '../../../../../CommonService/encryption.service';


@Component({
    moduleId: module.id,
    templateUrl: 'EmployeePayrollComponentList.html',
    providers: [PayrollService, AllowanceService, DeductionContributionService],
})

export class PayrollComponentList {
    public model = new EmployeePayrollModel();
    public SalaryModel = new Salary();
    public EmployeeModel = new EmployeeModel();
    public AllowanceModel = new AllowanceModel();
    public DedContModel = new DeductionContributionModel();
    public AdjustmentModel = new AdjustmentModel();
    public ProrateModel = new ProrateModel();
    public PrEmpDtFilterModel = new PrEmpDtFilterModel();
    public PrEmpDtSalaryTotalModel = new PrEmpDtSalaryTotalModel();
    public Id: string;
    public IsShowPayrollMasterList: boolean = true;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public PayrollList: any[] = [];
    public PayrollUniqueID: string;
    public PDetModel = new PaginationModel();
    public PDetConfig = new PaginationConfig();
    public PayrollDetList: any[] = [];
    public AllowancesList: any[] = [];
    public ContributionList: any[] = [];
    public DefaulAllowanceCategoryID: number;
    public DeductionList: any[] = [];
    public UnSelectedAllowancesList: any[] = [];
    public UnSelectedContributionList: any[] = [];
    public UnSelectedDeductionList: any[] = [];
    public AllowanceCategoryTypes = [];
    //public EmployeeAllowancesList = [];
    //public EmployeeDedConList: any[] = [];
    public PrEmployeeList: any[] = [];
    public PrLocationList: any[] = [];
    public PrDesignationList: any[] = [];
    public PrDepartmentList: any[] = [];
    public PrEmployeeTypeList: any[] = [];
    public IsList: boolean = true;
    public IsShowRunPayrollModal: boolean = true;
    public IsShowPayStubModal: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Currency: string;
    public PayrollRegion: string;
    public IsShowListPopModal: boolean = false;
    public IsAllowanceCategoryOtherID: boolean = false;
    public submitted: boolean = false;
    public IsValidAllowValue: boolean = false;
    public AllowanceOtherCategoryID: number;
    public IsValidDedConValue: boolean;
    public IsShowAllowancesListModal: boolean = false;;
    public IsShowDedConListModal: boolean = false;
    public IsShowProrateDateModal: boolean = false;
    public BasicSalary: number = 0;
    public IsInvalidDate: boolean = false;
    public FromDate: Date;
    public ToDate: Date;
    public IsStatusPublished: boolean = false;
    public AdjustmentValueClickIndex: number = 0;
    public IsSavedAdjustmentModel: boolean = false;
    public Status: string;
    public PayPeriod: string;
    public PayScheduleName: string;
    public IsSetTimeoutForLoader: boolean = true;
    public HouseRentIDForSA: number = 0;
    AllowanceForm: FormGroup;
    ConributionDeductionForm: FormGroup;
    ProrateForm: FormGroup;
    AdjustmentForm: FormGroup;
    public WasInside: boolean = false;
    public payrolShow: boolean = true;
    public ControlRights: any;
    public ColumnSettings: IMultiSelectSettings = {
        pullRight: false,
        enableSearch: true,
        checkedStyle: 'checkboxes',
        buttonClasses: 'btn btn-default',
        selectionLimit: 0,
        closeOnSelect: false,
        showCheckAll: true,
        showUncheckAll: true,
        dynamicTitleMaxItems: 1,
        maxHeight: '1000px',
    };
    public ColumnTexts: IMultiSelectTexts = {
        checkAll: 'Select all',
        //uncheckAll: 'Uncheck all',
        checked: 'checked',
        checkedPlural: 'checked',
        searchPlaceholder: 'Search...',
        defaultTitle: 'All',
    };

    @HostListener('click', ['$event'],)
    Clickoutdocument(event) {
        if (!this.WasInside)
            this.IsShowListPopModal = true;

        this.WasInside = false;
    }
    IsModalClick(form: any) {
        this.WasInside = true;
    }
    constructor(public _CommonService: CommonService, public _router: Router,
        public loader: LoaderService, public _PayrollService: PayrollService
        , public _fb: FormBuilder, public _allowanceService: AllowanceService
        , public _deductionContributionService: DeductionContributionService, public toastr: CommonToastrService,
        private cdr: ChangeDetectorRef, private encrypt: EncryptionService
    ) {

        this.Currency = this._CommonService.getCurrency();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("56");
    }
    ngOnInit() {

        this.loader.ShowLoader();
        this.PConfig.PrimaryColumn = "UniqueID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.ScreenName = "Payroll";

        this.PDetConfig.PrimaryColumn = "UniqueID";
        this.PDetConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PDetConfig.Action.ScreenName = "Payroll";

        this.PConfig.Action.Add = true;
        this.PConfig.Action.AddTextType = 'T';
        this.PConfig.Action.AddText = "Run Payroll";
        this.PConfig.Fields = [
            { Name: "ScheduleName", Title: "Pay Schedule", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: false },
            { Name: "Status", Title: "Status", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: false },
            { Name: "PayPeriod", Title: "Pay Period", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: false },
            { Name: "PayDate", Title: "Pay Date", OrderNo: 4, SortingAllow: true, Visible: true, isDate: true, DateFormat: "dd/MM/yyyy", IsShowCurrency: false },
            { Name: "NoOfEmp", Title: "Employees", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: false },
            { Name: "NetSalary", Title: "Net Salary", OrderNo: 6, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
        ];

        this.PDetConfig.Action.Add = false;
        //this.PDetConfig.Action.AddText = "fa fa-filter";
        this.PDetConfig.Action.IsShowScreenName = false;
        this.PDetConfig.Fields = [
            { Name: "EmployeeName", Title: "Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: false },
            { Name: "BaseSalary", Title: "Base Salary", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
            { Name: "Allowance", Title: "Allowance", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
            { Name: "Deduction", Title: "Deduction", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
            { Name: "GrossSalary", Title: "Gross Salary", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
            { Name: "NetSalary", Title: "Net Salary", OrderNo: 6, SortingAllow: true, Visible: true, isDate: false, DateFormat: "", IsShowCurrency: true },
        ];

        this.AllowanceForm = this._fb.group({
            AllowanceName: ['', [Validators.required]],
            AllowanceValue: ['', [Validators.required]],
            AllowanceType: [''],
            Taxable: [''],
            Default: [''],
            CategoryID: [''],
        });
        this.ConributionDeductionForm = this._fb.group({
            DeductionContributionName: ['', [<any>Validators.required]],
            DeductionContributionValue: ['', [Validators.required]],
            Category: [''],
            Taxable: [''],
            DeductionContributionType: [''],
            Default: [''],
        });
        this.ProrateForm = this._fb.group({
            FromDate: ['', [Validators.required]],
            ToDate: ['', [Validators.required]],
        });
        this.AdjustmentForm = this._fb.group({
            AdjustmentDate: ['', [Validators.required]],
            AdjustmentType: ['', [Validators.required]],
            AdjustmentAmount: ['', [Validators.required]],
            AdjustmentComments: ['', [Validators.required]],
        });

        if (this.PayrollRegion == 'SA')
            this.HouseRentIDForSA = 5;
        else
            this.HouseRentIDForSA = 0;


        this._PayrollService.FormPayrollListLoad().then(m => {
            this.AllowancesList = m.ResultSet.AllowancesList;
            this.ContributionList = m.ResultSet.ContributionList;
            this.DeductionList = m.ResultSet.DeductionList;
            this.AllowanceCategoryTypes = m.ResultSet.AllowanceCategoryTypes;
            if (this.AllowanceCategoryTypes.length > 0) {
                this.DefaulAllowanceCategoryID = this.AllowanceCategoryTypes[0].ID;
                this.AllowanceModel.CategoryID = this.DefaulAllowanceCategoryID;
                this.AllowanceModel.AllowanceName = 'a';
            }
            var FilterAllowCat = this.AllowanceCategoryTypes.filter(x => { return x.ID == 7 });
            if (FilterAllowCat.length > 0)
                this.AllowanceOtherCategoryID = FilterAllowCat[0].ID;

            this.loader.HideLoader();
        });
    }

    showpayrollWrap() {
        this.payrolShow = true;
    }

    GoBack(DefaultRoute) {
        if (this.IsShowPayrollMasterList)
            this._router.navigate([DefaultRoute]);

        this.IsList = true;
        this.IsShowPayrollMasterList = true;
    }

    AllowancSelectionChange() {

        if (this.AllowanceModel.CategoryID == this.AllowanceOtherCategoryID) {
            this.IsAllowanceCategoryOtherID = true;
            this.AllowanceModel.AllowanceName = '';
        }
        else {
            this.IsAllowanceCategoryOtherID = false;
            this.AllowanceModel.AllowanceName = 'a';
        }
    }

    RefreshPayrollMasterList() {
        this.loader.ShowLoader();
        //this.cdr.detectChanges();

        //PanictUtil.getRequestObservable().subscribe(data => setTimeout(() => this.requesting = data, 0));
        this.Id = "0";


        //this._PayrollService
        //    .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText)
        //    .then(m => setTimeout(()=> {
        //        this.PModel.TotalRecord = m.TotalRecord;
        //        this.PayrollList = m.DataList;
        //        this.loader.HideLoader();
        //    },0));

        this._PayrollService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText)
            .then(m => {
                
                this.PModel.TotalRecord = m.TotalRecord;
                this.PayrollList = m.DataList;
                this.loader.HideLoader();
            });
    }
    RefreshPayrollMasterDetailList() {
        this.loader.ShowLoader();
        //this.Status = this.Status == '' ? 'Open' : this.Status;
        this._PayrollService
            .GetDetList(this.PayrollUniqueID, this.PDetModel.CurrentPage, this.PDetModel.RecordPerPage, this.PDetModel.VisibleColumnInfo, this.PDetModel.SortName, this.PDetModel.SortOrder, this.PDetModel.SearchText).then(m => {
                
                this.PDetModel.TotalRecord = m.TotalRecord;
                this.PayrollDetList = m.DataList;
                this.PayScheduleName = this.PayrollDetList.length > 0 ? this.PayrollDetList[0].ScheduleName : '';
                this.Status = this.PayrollDetList.length > 0 ? (this.PayrollDetList[0].Status == 'O' ? 'Open' : 'Publish') : '';
                this.PrLocationList = m.OtherDataModel.Locations;
                this.PrDepartmentList = m.OtherDataModel.Departments;
                this.PrDesignationList = m.OtherDataModel.Designations;
                this.PrEmployeeTypeList = m.OtherDataModel.EmpTypes;
                this.PrEmployeeList = m.OtherDataModel.Emplist;
                if (this.PayrollDetList.length > 0) {
                    this.PrEmpDtSalaryTotalModel = new PrEmpDtSalaryTotalModel();
                    this.PayrollDetList.forEach(x => {
                        this.PrEmpDtSalaryTotalModel.TotalBaseSalary += x.BaseSalary;
                        this.PrEmpDtSalaryTotalModel.TotalAllowance += parseFloat(x.Allowance);
                        this.PrEmpDtSalaryTotalModel.TotalDeduction += x.Deduction;
                        this.PrEmpDtSalaryTotalModel.TotalGrossSalary += x.GrossSalary;
                        this.PrEmpDtSalaryTotalModel.TotalTax += x.Tax;
                        this.PrEmpDtSalaryTotalModel.TotalNetSalary += x.NetSalary;
                    });
                    //x.GrossSalary = this._CommonService.GetThousandCommaSepFormatDate(Math.round(x.GrossSalary));
                    //this.PrEmpDtSalaryTotalModel.TotalBaseSalary = this._CommonService.GetThousandCommaSepFormatDate(Math.round(this.PrEmpDtSalaryTotalModel.TotalBaseSalary));
                    //this.PrEmpDtSalaryTotalModel.TotalAllowance = this._CommonService.GetThousandCommaSepFormatDate(Math.round(this.PrEmpDtSalaryTotalModel.TotalAllowance));
                    //this.PrEmpDtSalaryTotalModel.TotalDeduction = this._CommonService.GetThousandCommaSepFormatDate(Math.round(Deduction;
                    //this.PrEmpDtSalaryTotalModel.TotalGrossSalary = this._CommonService.GetThousandCommaSepFormatDate(Math.round(GrossSalary;
                    //this.PrEmpDtSalaryTotalModel.TotalTax = this._CommonService.GetThousandCommaSepFormatDate(Math.round(Tax;
                    //this.PrEmpDtSalaryTotalModel.TotalNetSalary = this._CommonService.GetThousandCommaSepFormatDate(Math.round(NetSalary;

                    this.PrEmpDtSalaryTotalModel.TotalBaseSalary = Math.round(this.PrEmpDtSalaryTotalModel.TotalBaseSalary);
                    this.PrEmpDtSalaryTotalModel.TotalAllowance = Math.round(this.PrEmpDtSalaryTotalModel.TotalAllowance);
                    this.PrEmpDtSalaryTotalModel.TotalDeduction = Math.round(this.PrEmpDtSalaryTotalModel.TotalDeduction);
                    this.PrEmpDtSalaryTotalModel.TotalGrossSalary = Math.round(this.PrEmpDtSalaryTotalModel.TotalGrossSalary);
                    this.PrEmpDtSalaryTotalModel.TotalTax = Math.round(this.PrEmpDtSalaryTotalModel.TotalTax);
                    this.PrEmpDtSalaryTotalModel.TotalNetSalary = Math.round(this.PrEmpDtSalaryTotalModel.TotalNetSalary);
                }
                this.loader.HideLoader();
            });
    }
    DeletePayroll() {
        var res = confirm("Are you sure you want to delete " + this.PayScheduleName + " (" + this.PayPeriod + ")");
        if (this.PayrollUniqueID && res) {
            this.loader.ShowLoader();
            this._PayrollService
                .Delete(this.PayrollUniqueID).then(m => {
                    this.IsShowPayrollMasterList = true;
                    this.loader.HideLoader();
                });
        }
    }
    PublishPayroll() {
        if (this.PayrollDetList.length > 0) {
            //document.getElementById('ShowPublishPayrollConfirmationModal').click();
            var res = confirm("Are you sure? Please note you can’t make changes to payroll once you’ve published it.Would you like to proceed with publishing the " + this.PayPeriod + " payroll?");
            if (res) {
                this.loader.ShowLoader();
                this._PayrollService
                    .PublishPayroll(this.PayrollUniqueID).then(m => {
                        this.IsShowPayrollMasterList = true;
                        this.loader.HideLoader();
                    });
            }
        }
    }
    GetFormatDate(date) {
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getMonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return yyyy + '/' + mm + '/' + dd;
    };
    //CalculateGrossSalary(BasicSalary, Allowances, TaxableAllowances, Deduction, TaxableDeduction): number {
    //    return (BasicSalary - Deduction + TaxableAllowances + TaxableDeduction);
    //    //if (TaxableAllowances > 0) {
    //    //    GrossSalary = GrossSalary + TaxableAllowances;
    //    //}
    //    //if (TaxableDeduction > 0) {
    //    //    GrossSalary = GrossSalary + TaxableDeduction;
    //    //}
    //    //return GrossSalary
    //}
    AddRecord(id: string) {
        if (id != "0") {
            // this.loader.ShowLoader();
            //this.loader.HideLoader();
            this.IsShowPayrollMasterList = false;
            //this.IsShowRunPayrollModal = false;
            var item = this.PayrollList.filter(x => x.UniqueID == id);
            if (item.length > 0) {
                this.Status = item[0].Status;
                this.PayPeriod = item[0].PayPeriod;
            }
            this.Id = id;
            this.IsList = true;
            this.PayrollUniqueID = id;
            //this.Refresh();
        } else {
            //this.loader.ShowLoader();
            this.IsShowPayrollMasterList = true;
            this.IsShowRunPayrollModal = true;
            this.IsList = false;
            id = Math.random().toString();
        }

        this.Id = id;
        //this.IsList = false;
        //this.loader.HideLoader();
    }
    AddDetRecord(id: string) {
        if (id != "0") {
            this.Id = id;
            this.loader.ShowLoader();
            this.IsList = false;
            this.IsShowPayrollMasterList = false;

            this.PayScheduleName = this.PayrollList.length > 0 ? this.PayrollList[0].ScheduleName : ''
            this._PayrollService
                .GetPayStubOfEmployeeByPayrollMfID(this.Id).then(m => {
                    this.IsShowPayStubModal = false;
                    this.IsList = false;
                    this.IsShowPayrollMasterList = false;

                    this.model = m.ResultSet;
                    //this.EmployeeAllowancesList = m.ResultSet.EmpAllowances;
                    //this.EmployeeDedConList = m.ResultSet.EmpDedCon;
                    this.EmployeeModel = m.ResultSet.pr_employee_mf;

                    if (this.model.Status == 'P')
                        this.IsStatusPublished = true;
                    else
                        this.IsStatusPublished = false;


                    this.ProrateModel.FromDate = this.model.FromDate;
                    this.ProrateModel.ToDate = this.model.ToDate;

                    this.UnSelectedAllowancesList = [];
                    this.UnSelectedContributionList = [];
                    this.UnSelectedDeductionList = [];
                    this.AllowancesList.forEach(m => {
                        var item = this.model.pr_employee_payroll_dt.filter(x => x.AllowDedID == m.ID);
                        if (item.length == 0)
                            m.Default = false;
                        else
                            m.Default = true;

                        this.UnSelectedAllowancesList.push(JSON.parse(JSON.stringify(m)));
                    });
                    this.ContributionList.forEach(m => {
                        var item = this.model.pr_employee_payroll_dt.filter(x => x.AllowDedID == m.ID);
                        if (item.length == 0)
                            m.Default = false;
                        else
                            m.Default = true;

                        this.UnSelectedContributionList.push(JSON.parse(JSON.stringify(m)));
                    });
                    this.DeductionList.forEach(m => {
                        var item = this.model.pr_employee_payroll_dt.filter(x => x.AllowDedID == m.ID);
                        if (item.length == 0)
                            m.Default = false;
                        else
                            m.Default = true;

                        this.UnSelectedDeductionList.push(JSON.parse(JSON.stringify(m)));
                    });

                    this.BasicSalary = JSON.parse(JSON.stringify(this.model.BasicSalary));
                    this.UpdateUnselectedlistAmount();
                    this.IsSetTimeoutForLoader = false;
                    this.ReCalculateSalary();
                    this.IsSetTimeoutForLoader = true;
                    this.loader.HideLoader();
                    this.ShowPayStubModal();

                    this.cdr.detectChanges();
                });
        } else {
            document.getElementById('SearchButton').click();
        }

        //this.Id = id;
        //this.IsList = false;
        //this.loader.HideLoader();
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    GetList() {
        this.RefreshPayrollMasterList();
    }
    GetDetList() {
        this.RefreshPayrollMasterDetailList();
    }
    Close(id: string) {
        if (id != '') {
            //this.IsShowPayrollMasterList = false;
            this.Id = id.split('@')[0].toString();
            this.PayPeriod = id.split('@')[1].toString();
            this.PayrollUniqueID = this.Id;
            this.IsShowPayrollMasterList = false;
            //this.IsShowRunPayrollModal = true;
            this.IsList = true;
        }
        else {
            this.IsShowPayrollMasterList = true;
            this.IsList = false;
        }
    }
    ReCalculateSalary() {
        //this.loader.ShowLoader();

        //if (this.IsSetTimeoutForLoader) {
        //    this.loader.ShowLoader();
        //    setTimeout(() => {
        //        this.loader.HideLoader();
        //    }, '400');
        //}

        this.SalaryModel = new Salary();
        //this.SalaryModel.BasicSalary = this.BasicSalary;
        if (this.model.AdjustmentBy)
            this.SalaryModel.BasicSalary = this.GetAmount(this.model, true);
        else
            this.SalaryModel.BasicSalary = this.BasicSalary;

        this.model.pr_employee_payroll_dt.forEach(x => {
            //if (x.AdjustmentAmount > 0 || x.AdjustmentAmount != null)
            //    x.Amount += x.AdjustmentAmount;

            if (x.Type == 'A') {
                if (this.PayrollRegion == 'SA') {
                    this.SalaryModel.TaxableAllowanceTotal += this.GetAmount(x, false);
                } else if (x.Taxable)
                    this.SalaryModel.TaxableAllowanceTotal += this.GetAmount(x, false);

                this.SalaryModel.AllowanceTotal += this.GetAmount(x, false);
            }
            else {
                if (x.Type == 'C')
                    this.SalaryModel.ContributionTotal += this.GetAmount(x, false);
                else {
                    if (this.PayrollRegion == 'SA') {
                        this.SalaryModel.TaxableDeductionTotal += this.GetAmount(x, false);
                    } else if (x.Taxable)
                        this.SalaryModel.TaxableDeductionTotal += this.GetAmount(x, false);

                    this.SalaryModel.DeductionTotal += this.GetAmount(x, false);
                }
            }
        });
        this.SalaryModel.CalculateSalary();
        //this.loader.HideLoader();
    }
    GetAmount(item, IsBasicSal): number {
        if (IsBasicSal) return item.AdjustmentType == 'C' ? item.BasicSalary + (item.AdjustmentAmount == null ? 0 : item.AdjustmentAmount) : item.BasicSalary - (item.AdjustmentAmount == null ? 0 : item.AdjustmentAmount);
        else return item.AdjustmentType == 'C' ? item.Amount + (item.AdjustmentAmount == null ? 0 : item.AdjustmentAmount) : item.Amount - (item.AdjustmentAmount == null ? 0 : item.AdjustmentAmount);
    }
    UpdateUnselectedlistAmount() {
        this.UnSelectedAllowancesList.forEach(f => {
            var item = this.AllowancesList.filter(x => x.ID == f.ID && f.AllowanceType == 'P');
            if (item.length > 0) {
                f.AllowanceValue = (item[0].AllowanceValue / 100) * this.EmployeeModel.BasicSalary;
            }
        });
        this.UnSelectedContributionList.forEach(f => {
            var item = this.ContributionList.filter(x => x.ID == f.ID && f.DeductionContributionType == 'P');
            if (item.length > 0) { f.DeductionContributionValue = ((item[0].DeductionContributionValue / 100) * this.EmployeeModel.BasicSalary); }
        });
        this.UnSelectedDeductionList.forEach(f => {
            var item = this.DeductionList.filter(x => x.ID == f.ID && f.DeductionContributionType == 'P');
            if (item.length > 0) {
                if (item.length > 0) {
                    if (this.PayrollRegion == 'SA' && item[0].DeductionContributionName == 'GOSI')
                        f.DeductionContributionValue = ((item[0].DeductionContributionValue / 100) * (this.EmployeeModel.BasicSalary + this.GetAmountOfHouseRentForSA()));
                    else
                        f.DeductionContributionValue = ((item[0].DeductionContributionValue / 100) * this.EmployeeModel.BasicSalary);
                }
            }
        });
    }
    ShowListModal(ModalType) {
        
        //this.IsShowListPopModal = true;
        this.IsShowPayStubModal = false;
        //this.IsShowAllowancesListModal = false;
        //this.IsShowDedConListModal = false;
        //this.IsShowProrateDateModal = false;
        if (ModalType) {
            if (ModalType == 'C')
                this.DedContModel.Category = 'C';
            else if (ModalType == 'D')
                this.DedContModel.Category = 'D';
            else if (ModalType == 'A') {
                this.IsAllowanceCategoryOtherID = false;
                this.AllowanceModel.AllowanceName = 'a';
            }
            else if (ModalType == 'CProrate') {
                this.ProrateModel.FromDate = this.model.FromDate;
                this.ProrateModel.ToDate = this.model.ToDate;
                this.ShowPayStubModal();
            }
            else if (ModalType == 'cancel')
                this.ShowPayStubModal();

            if (ModalType != 'cancel' && ModalType != 'adj' && ModalType != 'CProrate') {
                document.getElementById('ClosePayStubModalId').click();
            }
        }
        
    }
    ShowPayStubModal() {
        this.IsShowPayStubModal = true;
        document.getElementById('ShowPayStubId').click();
    }
    ShowModal() {
        this.IntializeModels();
        this.IsShowListPopModal = false;
    }
    RemoveAllowanceFromSelList(id) {
        if (id > 0) {
            this.UnSelectedAllowancesList.forEach(f => {
                if (f.ID == id)
                    f.Default = false;
            });
            this.model.pr_employee_payroll_dt = this.model.pr_employee_payroll_dt.filter(x => x.AllowDedID != id);
        }
        this.ReCalculateSalary();
    }
    AddAlowanceToSelList(Id) {
        if (Id > 0) {
            this.UnSelectedAllowancesList.forEach(f => {
                if (f.ID == Id)
                    f.Default = true;
            });
            this.AddEmployeeAllowance(Id);
            this.ReCalculateSalary();
        }
    }
    AddContributionToSelList(Id) {
        if (Id > 0) {
            this.UnSelectedContributionList.forEach(f => {
                if (f.ID == Id)
                    f.Default = true;
            });
            this.AddEmployeeContributionDeduction(Id, 'C');
        }
        this.ReCalculateSalary();
    }
    RemoveContributionFromSelList(id) {
        if (id > 0) {
            this.UnSelectedContributionList.forEach(f => {
                if (f.ID == id)
                    f.Default = false;
            });
            this.model.pr_employee_payroll_dt = this.model.pr_employee_payroll_dt.filter(x => x.AllowDedID != id);
        }
        this.ReCalculateSalary();
    }
    AddDeductionToSelList(Id) {
        if (Id > 0) {
            this.UnSelectedDeductionList.forEach(f => {
                if (f.ID == Id)
                    f.Default = true;
            });
            this.AddEmployeeContributionDeduction(Id, 'D');
        }
        this.ReCalculateSalary();
    }
    RemoveDeductionFromSelList(id) {
        if (id > 0) {
            this.UnSelectedDeductionList.forEach(f => {
                if (f.ID == id)
                    f.Default = false;
            });
            this.model.pr_employee_payroll_dt = this.model.pr_employee_payroll_dt.filter(x => x.AllowDedID != id);
        }
        this.ReCalculateSalary();
    }
    IsAllowanceValueValid() {
        if (this.AllowanceModel.AllowanceType == 'P' && this.AllowanceModel.AllowanceValue > 100)
            this.IsValidAllowValue = true;
        else
            this.IsValidAllowValue = false;
    }
    IsDedContValValid() {
        if (this.DedContModel.DeductionContributionType == 'P' && this.DedContModel.DeductionContributionValue > 100)
            this.IsValidDedConValue = true;
        else
            this.IsValidDedConValue = false;
    }
    SavePayStubOfSingleEmp() {
        if (this.Id) {
            //this.model.ID = parseInt(this.Id);
            this.loader.ShowLoader();
            this.model.BasicSalary = this.BasicSalary;//this.CalculateProrateAmount(this.EmployeeModel.BasicSalary);
            this._PayrollService
                .SavePayStubOfSingleEmployee(this.model).then(m => {
                    this.loader.HideLoader();
                    this.PayrollUniqueID = this.model.PayScheduleID + '#' + this.model.PayDate;
                    document.getElementById('ClosePayStubModalId').click();
                    this.RefreshPayrollMasterDetailList();
                });
        }
    }
    DeletePayStubOfSingleEmp() {
        var res = confirm("Are you sure you want to delete " + this.EmployeeModel.FirstName + " " + this.EmployeeModel.LastName + " from payroll?");
        if (this.Id && res) {
            this.model.ID = parseInt(this.Id);
            this.loader.ShowLoader();
            this._PayrollService
                .DeleteEmployeeFromPayrollDetList(this.Id).then(m => {
                    this.loader.HideLoader();
                    this.PayrollUniqueID = this.model.PayScheduleID + '#' + this.model.PayDate;
                    document.getElementById('ClosePayStubModalId').click();
                    this.PrEmpDtSalaryTotalModel = new PrEmpDtSalaryTotalModel();
                    this.RefreshPayrollMasterDetailList();
                });
        }
    }
    SaveProrateDate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid && this.IsInvalidDate) {
            isValid = false;
            this.toastr.Warning('Incorrect Selection', 'Please select the date between batch Start and end date.');
        }
        if (isValid) {
            this.submitted = false;
            var Amount = 0;

            this.BasicSalary = this.CalculateProrateAmount(this.EmployeeModel.BasicSalary);
            this.model.pr_employee_payroll_dt.forEach(f => {
                Amount = 0;
                if (f.Type == 'A') {
                    var item = this.AllowancesList.filter(x => x.ID == f.AllowDedID);
                    if (item.length > 0) {
                        if (item[0].AllowanceType == 'P')
                            Amount = ((item[0].AllowanceValue / 100) * this.BasicSalary);
                        else
                            Amount = item[0].AllowanceValue;
                    }
                    f.Amount = this.CalculateProrateAmount(Amount);
                }
                else if (f.Type == 'C') {
                    var item = this.ContributionList.filter(x => x.ID == f.AllowDedID);
                    if (item.length > 0) {
                        if (item[0].DeductionContributionType == 'P')
                            Amount = ((item[0].DeductionContributionValue / 100) * this.EmployeeModel.BasicSalary);
                        else
                            Amount = item[0].DeductionContributionValue;
                    }
                    f.Amount = this.CalculateProrateAmount(Amount);
                }
                else if (f.Type == 'D') {
                    var item = this.DeductionList.filter(x => x.ID == f.AllowDedID);
                    if (item.length > 0) {
                        if (item[0].DeductionContributionType == 'P')
                            Amount = ((item[0].DeductionContributionValue / 100) * this.EmployeeModel.BasicSalary);
                        else
                            Amount = item[0].DeductionContributionValue;
                    }
                    f.Amount = this.CalculateProrateAmount(Amount);
                }
            });


            this.model.FromDate = this.ProrateModel.FromDate;
            this.model.ToDate = this.ProrateModel.ToDate;
            this.ReCalculateSalary();
        }
    }
    CalculateProrateAmount(Amount): number {
        //this.loader.ShowLoader();
        var FromDate = new Date(this.ProrateModel.FromDate);
        var ToDate = new Date(this.ProrateModel.ToDate);
        var daysDiff = Math.ceil((Date.parse(this.GetFormatDate(ToDate)) - Date.parse(this.GetFormatDate(FromDate))) / 86400000) + 1;
        var date = new Date(this.ProrateModel.FromDate);
        var monthEndDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
        var value = (Amount / monthEndDay.getDate()) * daysDiff;
        var Amnt = ((Amount / monthEndDay.getDate()) * daysDiff).toFixed(2);
        return (parseFloat(Amnt) == parseInt(Amnt)) ? parseInt(Amnt) : parseFloat(Amnt);
    }
    onBatchStartDateChanged(event: IMyDateModel) {

        if (event && this.ProrateModel.FromDate) {
            if (this.ProrateModel.FromDate < this.model.PayScheduleFromDate || this.ProrateModel.FromDate > this.model.PayScheduleToDate) {
                this.ProrateModel.FromDate = undefined;
                this.IsInvalidDate = true;
            } else
                this.IsInvalidDate = false;
        }
    }
    onBatchEndDateChanged(event: IMyDateModel) {
        if (event && this.ProrateModel.ToDate) {
            if (this.ProrateModel.ToDate > this.model.PayScheduleToDate || this.ProrateModel.ToDate < this.model.PayScheduleFromDate) {
                this.ProrateModel.ToDate = undefined;
                this.IsInvalidDate = true;
            } else
                this.IsInvalidDate = false;
        }
    }
    //onAdjustmentDateChanged(event: IMyDateModel) {
    //    if (event && this.AdjustmentModel.AdjustmentDate) {
    //        this.AdjustmentModel.AdjustmentDate = new Date()
    //        if (this.AdjustmentModel.AdjustmentDate < this.model.PayDate)
    //            this.AdjustmentModel.AdjustmentDate = undefined;
    //    }
    //}
    SaveAllowance(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.IsAllowanceValueValid();
            isValid = !this.IsValidAllowValue;
        }
        if (isValid) {
            if (this.AllowanceModel.AllowanceName === 'a' && this.AllowanceModel.CategoryID != this.AllowanceOtherCategoryID)
                this.AllowanceModel.AllowanceName = '';

            this.submitted = false;
            this.loader.ShowLoader();

            if (this.PayrollRegion == 'SA')
                this.AllowanceModel.Taxable = true;
            this._allowanceService.SaveAndReturnAllowance(this.AllowanceModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    var AllowItem = result.ResultSet;
                    this.AllowancesList.push(JSON.parse(JSON.stringify(AllowItem)));
                    var objEA = new pr_employee_allowance();
                    if (AllowItem.AllowanceType == 'P') {
                        AllowItem.AllowanceValue = ((AllowItem.AllowanceValue / 100) * this.model.BasicSalary);
                    }
                    AllowItem.Default = false;
                    this.UnSelectedAllowancesList.push(JSON.parse(JSON.stringify(AllowItem)));
                    this.IsShowListPopModal = true;
                    this.DefaulAllowanceCategoryID = this.AllowanceCategoryTypes[0].ID;
                    this.AllowanceModel.CategoryID = this.DefaulAllowanceCategoryID;
                    this.AllowancSelectionChange();
                }
                else {
                    if (this.AllowanceOtherCategoryID != this.AllowanceModel.CategoryID)
                        this.AllowanceModel.AllowanceName = 'a';

                    this.toastr.Error('Error', result.ErrorMessage);
                }

                this.loader.HideLoader();
            });
        }
    }
    SaveContributionDeduction(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.IsDedContValValid();
            isValid = !this.IsValidDedConValue;
        }
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            if (this.PayrollRegion == 'SA' && this.DedContModel.Category == 'D')
                this.DedContModel.Taxable = true;

            this._deductionContributionService.SaveDedContandReturnDedCont(this.DedContModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    var ContriItem = result.ResultSet;
                    if (ContriItem.Category == 'C')
                        this.ContributionList.push(JSON.parse(JSON.stringify(ContriItem)));
                    else
                        this.DeductionList.push(JSON.parse(JSON.stringify(ContriItem)));

                    if (ContriItem.DeductionContributionType == 'P') {
                        ContriItem.DeductionContributionValue = ((ContriItem.DeductionContributionValue / 100) * this.model.BasicSalary);
                    }
                    ContriItem.Default = false;
                    if (ContriItem.Category == 'C')
                        this.UnSelectedContributionList.push(JSON.parse(JSON.stringify(ContriItem)));
                    else
                        this.UnSelectedDeductionList.push(JSON.parse(JSON.stringify(ContriItem)));

                    this.IsShowListPopModal = true;
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    AddEmployeeAllowance(AllowanceID): void {
        var obj = this.AllowancesList.filter(f => f.ID == AllowanceID);
        if (obj.length > 0) {

            var result = false;
            var ObjEA = new pr_employee_payroll_dt();
            if (this.model.FromDate != this.model.PayScheduleFromDate || this.model.ToDate != this.model.PayScheduleToDate) {
                result = confirm("Do you want to prorate the selected allowance(s)?");
            }
            if (result) {
                if (obj[0].AllowanceType == 'P')
                    ObjEA.Amount = ((obj[0].AllowanceValue / 100) * this.BasicSalary);
                else
                    ObjEA.Amount = this.CalculateProrateAmount(obj[0].AllowanceValue);
            }
            else {
                if (obj[0].AllowanceType == 'P')
                    ObjEA.Amount = ((obj[0].AllowanceValue / 100) * this.EmployeeModel.BasicSalary);
                else
                    ObjEA.Amount = obj[0].AllowanceValue;
            }

            ObjEA.Type = 'A';
            ObjEA.AllowDedID = AllowanceID;
            ObjEA.Taxable = obj[0].Taxable;
            this.model.pr_employee_payroll_dt.push(ObjEA);
            this.UnSelectedAllowancesList.forEach(f => {
                if (f.ID == AllowanceID)
                    f.Default = true;
            });
        }
    }
    GetAllowanceTitle(AllowanceID): string {
        if (AllowanceID != undefined) {
            var obj = this.AllowancesList.filter(f => f.ID == AllowanceID);
            if (obj.length > 0)
                return obj[0].AllowanceName;
            else
                return "";
        } else
            return "";
    }
    AddEmployeeContributionDeduction(ContrDedID, Type): void {
        var obj = null;
        if (Type == 'C') {
            obj = this.ContributionList.filter(f => f.ID == ContrDedID);
        } else {
            obj = this.DeductionList.filter(f => f.ID == ContrDedID);
        }
        if (obj.length > 0) {
            var result = false;
            var ObjCD = new pr_employee_payroll_dt();

            if (this.model.FromDate != this.model.PayScheduleFromDate || this.model.ToDate != this.model.PayScheduleToDate) {
                result = confirm("Do you want to prorate the selected " + (Type == 'C' ? 'Contribution' : 'Deduction') + "(s)?");
            }
            if (result) {
                if (obj[0].DeductionContributionType == 'P') {
                    if (this.PayrollRegion == 'SA' && obj[0].Category == 'D' && obj[0].DeductionContributionName == 'GOSI') {
                        ObjCD.Amount = (obj[0].DeductionContributionValue / 100) * (this.BasicSalary + this.GetAmountOfHouseRentForSA());
                    } else {
                        ObjCD.Amount = ((obj[0].DeductionContributionValue / 100) * this.BasicSalary);
                    }
                }
                else
                    ObjCD.Amount = this.CalculateProrateAmount(obj[0].AllowanceValue);
            }
            else {
                if (obj[0].DeductionContributionType == 'P') {
                    if (this.PayrollRegion == 'SA' && obj[0].Category == 'D' && obj[0].DeductionContributionName == 'GOSI')
                        ObjCD.Amount = (obj[0].DeductionContributionValue / 100) * (this.EmployeeModel.BasicSalary + this.GetAmountOfHouseRentForSA());
                    else
                        ObjCD.Amount = (obj[0].DeductionContributionValue / 100) * this.EmployeeModel.BasicSalary;
                }
                else
                    ObjCD.Amount = obj[0].DeductionContributionValue;
            }

            ObjCD.AllowDedID = ContrDedID;
            ObjCD.Taxable = obj[0].Taxable;
            ObjCD.Type = obj[0].Category;
            this.model.pr_employee_payroll_dt.push(ObjCD);
            if (Type == 'C') {
                this.UnSelectedContributionList.forEach(f => {
                    if (f.ID == ContrDedID)
                        f.Default = true;
                });
            }
            else {
                this.UnSelectedDeductionList.forEach(f => {
                    if (f.ID == ContrDedID)
                        f.Default = true;
                });
            }
        }
    }
    GetContributionDeductionTitle(ContrDedID, Type): string {
        var obj = null;
        if (ContrDedID != undefined) {
            if (Type == 'C') {
                obj = this.ContributionList.filter(f => f.ID == ContrDedID);
            } else {
                obj = this.DeductionList.filter(f => f.ID == ContrDedID);
            }

            if (obj.length > 0)
                return obj[0].DeductionContributionName;
            else
                return "";
        }
        else
            return "";
    }
    AdjustmentEditClick(AllowDedIDOrVal, Type, RefID) {
        this.AdjustmentValueClickIndex = 0;
        this.AdjustmentModel = new AdjustmentModel();
        var prObj = null;
        if (Type == 'A' || Type == 'D' || Type == 'C') {
            prObj = this.model.pr_employee_payroll_dt.filter(x => x.AllowDedID == AllowDedIDOrVal && x.RefID == RefID && x.Type == Type);
        } else {
            prObj = this.model;
        }
        this.AdjustmentModel.AllDedContType = Type;
        this.PopulateAdjustmentModel(prObj, Type, AllowDedIDOrVal);
    }
    PopulateAdjustmentModel(obj, Type, AllowDedIDOrVal) {
        if (obj.length > 0 && (Type == 'A' || Type == 'D' || Type == 'C')) {
            obj = obj[0];
            this.AdjustmentModel.PayrollDtID = obj.ID
            this.AdjustmentModel.PayrollMfID = obj.PayrollID
            this.AdjustmentModel.EmployeeID = obj.EmployeeID
            this.AdjustmentModel.PayScheduleID = obj.PayScheduleID;
            this.AdjustmentModel.PayDate = obj.PayDate
        }
        else {
            this.AdjustmentModel.PayrollMfID = obj.ID;
            this.AdjustmentModel.EmployeeID = obj.EmployeeID
            this.AdjustmentModel.PayScheduleID = obj.PayScheduleID;
            this.AdjustmentModel.PayDate = obj.PayDate
        }



        if (obj.AdjustmentBy) {
            this.AdjustmentModel.AdjustmentAmount = obj.AdjustmentAmount;
            this.AdjustmentModel.AdjustmentBy = obj.AdjustmentBy;
            this.AdjustmentModel.AdjustmentComments = obj.AdjustmentComments;
            this.AdjustmentModel.AdjustmentDate = obj.AdjustmentDate;
            this.AdjustmentModel.AdjustmentType = obj.AdjustmentType;
        }

        var AllConDedTitle = '';
        var amount = 0;
        var Title = '';
        if (Type == 'A') {
            AllConDedTitle = this.GetAllowanceTitle(AllowDedIDOrVal);
            amount = obj.Amount;
            Title = 'Allowances - ' + AllConDedTitle;
        } else if (Type == 'C' || Type == 'D') {
            AllConDedTitle = this.GetContributionDeductionTitle(AllowDedIDOrVal, Type);
            amount = obj.Amount;
            Title = ((Type == 'C') ? 'Contributions - ' : 'Deductions - ') + AllConDedTitle;
        } else if (Type == 'B' || Type == 'T') {
            AllConDedTitle = (Type == 'B') ? 'Base Salary' : 'Tax Amount';
            amount = obj.BasicSalary;
            Title = AllConDedTitle;
        }

        this.AdjustmentModel.AdjustmentTitle = Title;
        this.AdjustmentModel.AdjustmentText = AllConDedTitle;
        this.AdjustmentModel.OrignalValue = amount;
        this.AdjustmentModel.OrignalText = AllConDedTitle;
        amount = obj.AdjustmentType == 'C' ? amount + obj.AdjustmentAmount : amount - obj.AdjustmentAmount;
        this.AdjustmentModel.AdjustmentValue = amount;

        if (!obj.AdjustmentBy) {
            this.IsSavedAdjustmentModel = false;
            this.ClearAdjustmentModel();
        } else
            this.IsSavedAdjustmentModel = true;

        this.ShowListModal(Type);
    }
    DeleteAdjustmentModel(index) {
        var res = confirm("Are you sure you want to delete this adjustment");
        if (res) {
            this.loader.ShowLoader();
            var idd = '';
            if (this.AdjustmentModel.AllDedContType == 'A' || this.AdjustmentModel.AllDedContType == 'C' || this.AdjustmentModel.AllDedContType == 'D')
                idd = this.AdjustmentModel.PayrollDtID.toString();
            else
                idd = this.AdjustmentModel.PayrollMfID.toString();

            this._PayrollService
                .DeleteAdjustment(idd, this.AdjustmentModel.AllDedContType).then(m => {
                    this.loader.HideLoader();
                    this.PayrollUniqueID = this.model.PayScheduleID + '#' + this.model.PayDate;

                    document.getElementById('CloseAdjustModal').click();
                    this.AddDetRecord(this.model.ID.toString());
                    this.RefreshPayrollMasterDetailList();
                });
        }
    }
    ClearAdjustmentModel() {
        this.AdjustmentModel.AdjustmentAmount = 0;
        this.AdjustmentModel.AdjustmentComments = '';
        this.AdjustmentModel.AdjustmentDate = new Date();
        this.AdjustmentModel.AdjustmentType = 'C';
    }
    CreditOrDebitAmount() {
        this.AdjustmentModel.AdjustmentValue = this.AdjustmentModel.AdjustmentType == 'C' ? this.AdjustmentModel.OrignalValue + this.AdjustmentModel.AdjustmentAmount : this.AdjustmentModel.OrignalValue - this.AdjustmentModel.AdjustmentAmount;
    }
    SavePayrollAdjustment(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        var Adjustmentdate = new Date(this.AdjustmentModel.AdjustmentDate);
        var Paydate = new Date(this.model.PayDate);
        if (isValid && (Adjustmentdate < Paydate)) {
            this.toastr.Warning('Incorrect Date', 'Adjustment date cannot be less than the transaction date');
            isValid = false;
        }

        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._PayrollService.SaveAdjustmentModel(this.AdjustmentModel).then(m => {
                this.loader.HideLoader();
                this.PayrollUniqueID = this.model.PayScheduleID + '#' + this.model.PayDate;
                this.AddDetRecord(this.model.ID.toString());
                this.RefreshPayrollMasterDetailList();
                document.getElementById('CloseAdjustModal').click();
            });
        }
    }
    FilterPrEmpDt() {
        //if (this.PrEmpDtFilterModel.LocationsIds.length > 0 || this.PrEmpDtFilterModel.DepartmentsIds.length > 0 || this.PrEmpDtFilterModel.DesignationsIds.length > 0 || this.PrEmpDtFilterModel.EmployeeTypeIds.length > 0 || this.PrEmpDtFilterModel.EmployeeIds.length > 0) {
        //var FilterParams = 'LocationsIds=' + this.PrEmpDtFilterModel.LocationsIds.join(',') + '#DepartmentsIds=' + this.PrEmpDtFilterModel.DepartmentsIds.join(',') + '#DesignationsIds=' + this.PrEmpDtFilterModel.DesignationsIds.join(',') + '#EmployeeTypeIds=' + this.PrEmpDtFilterModel.EmployeeTypeIds.join(',') + '#EmployeeIds=' + this.PrEmpDtFilterModel.EmployeeIds.join(',');
        var FilterParams = this.PrEmpDtFilterModel.LocationsIds.join(',') + '#' + this.PrEmpDtFilterModel.DepartmentsIds.join(',') + '#' + this.PrEmpDtFilterModel.DesignationsIds.join(',') + '#' + this.PrEmpDtFilterModel.EmployeeTypeIds.join(',') + '#' + this.PrEmpDtFilterModel.EmployeeIds.join(',');
        this.loader.ShowLoader();
        this._PayrollService
            .GetFilterDetList(FilterParams, this.PayrollUniqueID, this.PDetModel.CurrentPage, this.PDetModel.RecordPerPage, this.PDetModel.VisibleColumnInfo, this.PDetModel.SortName, this.PDetModel.SortOrder, this.PDetModel.SearchText).then(m => {
                this.PDetModel.TotalRecord = m.TotalRecord;
                this.PayrollDetList = m.DataList;
                //this.PrEmpDtFilterModel = m.OtherDataModel;
                if (this.PayrollDetList.length > 0) {
                    this.PrEmpDtSalaryTotalModel = new PrEmpDtSalaryTotalModel();
                    this.PayrollDetList.forEach(x => {
                        this.PrEmpDtSalaryTotalModel.TotalBaseSalary += x.BaseSalary;
                        this.PrEmpDtSalaryTotalModel.TotalAllowance += parseFloat(x.Allowance);
                        this.PrEmpDtSalaryTotalModel.TotalDeduction += x.Deduction;
                        this.PrEmpDtSalaryTotalModel.TotalGrossSalary += x.GrossSalary;
                        this.PrEmpDtSalaryTotalModel.TotalTax += x.Tax;
                        this.PrEmpDtSalaryTotalModel.TotalNetSalary += x.NetSalary;
                    });
                    this.PrEmpDtSalaryTotalModel.TotalBaseSalary = parseFloat(this.PrEmpDtSalaryTotalModel.TotalBaseSalary.toFixed(2));
                    this.PrEmpDtSalaryTotalModel.TotalAllowance = parseFloat(this.PrEmpDtSalaryTotalModel.TotalAllowance.toFixed(2));
                    this.PrEmpDtSalaryTotalModel.TotalDeduction = parseFloat(this.PrEmpDtSalaryTotalModel.TotalDeduction.toFixed(2));
                    this.PrEmpDtSalaryTotalModel.TotalGrossSalary = parseFloat(this.PrEmpDtSalaryTotalModel.TotalGrossSalary.toFixed(2));
                    this.PrEmpDtSalaryTotalModel.TotalTax = parseFloat(this.PrEmpDtSalaryTotalModel.TotalTax.toFixed(2));
                    this.PrEmpDtSalaryTotalModel.TotalNetSalary = parseFloat(this.PrEmpDtSalaryTotalModel.TotalNetSalary.toFixed(2));
                }
                this.loader.HideLoader();
            });
        //} else alert('Please select atleast one parameter or reset form');

        document.getElementById('CloseFilterModal').click();
    }
    ResetPrDtFilter() {
        this.PrEmpDtFilterModel.LocationsIds = [];
        this.PrEmpDtFilterModel.DepartmentsIds = [];
        this.PrEmpDtFilterModel.DesignationsIds = [];
        this.PrEmpDtFilterModel.EmployeeTypeIds = [];
        this.PrEmpDtFilterModel.EmployeeIds = [];
        this.RefreshPayrollMasterDetailList();
    }
    GetAmountOfHouseRentForSA(): number {
        var allow = this.AllowancesList.filter(x => x.CategoryID == this.HouseRentIDForSA);
        if (allow.length > 0 && allow[0].AllowanceType == 'P')
            return (allow[0].AllowanceValue / 100) * this.EmployeeModel.BasicSalary;
        else return 0;
    }
    IntializeModels() {
        this.AllowanceModel = new AllowanceModel();
        this.AllowanceModel.CategoryID = this.DefaulAllowanceCategoryID;
        this.AllowanceModel.AllowanceName = 'a';
        var DefaultContDedCategory = this.DedContModel.Category;
        this.DedContModel = new DeductionContributionModel();
        this.DedContModel.Category = DefaultContDedCategory;
        this.AllowanceModel.CategoryID = this.DefaulAllowanceCategoryID;
    }

    //ngAfterViewInit() {
    //    this.loader.ShowLoader = this.loader.IsLoading;
    //}


}