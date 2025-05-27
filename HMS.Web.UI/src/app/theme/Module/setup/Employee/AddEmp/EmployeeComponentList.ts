import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeService } from './EmployeeService';
import { EmployeeModel, EmpBulkUpdateModel, BulkEmpSumAmountModel } from './EmployeeModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from 'angular-2-dropdown-multiselect';
import { MatPaginator, MatSort, MatTableDataSource, MatTable, PageEvent } from '@angular/material';
import { DataSource } from '@angular/cdk/table';
import { SelectionModel } from '@angular/cdk/collections';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
export interface Element {
    select: false,
    //ID: number,
    Employee: string,
    ExistingAmount: number,
    UseExistingAmount: boolean,
    Amount: number,
    AllwConDedBSalID: number,
    Category: string,
    Taxable: string,
    Remove: boolean,
}
@Component({
    moduleId: module.id,
    templateUrl: 'EmployeeComponentList.html',
    providers: [EmployeeService],
})
export class EmployeeComponentList {
    dataSource;
    displayedColumns = [];
    @ViewChild(MatSort) sort: MatSort;
    @ViewChild(MatPaginator) paginator: MatPaginator;
    IsRemoveAllChked: boolean = false;
    IsRemoveIntermediate: boolean = false;
    IsExistAmntAllChked: boolean = false;
    IsExistAmntIntermediate: boolean = false;
    IsTaxableAllChked: boolean = false;
    IsTaxableIntermediate: boolean = false;
    columnNames = [
        {
            id: "Employee",
            value: "Employee"
        },
        {
            id: "ExistingAmount",
            value: "ExistingAmount"
        },
        {
            id: "UseExistingAmount",
            value: "UseExistingAmount"
        },
        {
            id: "Amount",
            value: "Amount"
        },
        {
            id: "Taxable",
            value: "Taxable"
        },
        {
            id: "Remove",
            value: "Remove"
        },
    ];
    public BulkModel = new EmpBulkUpdateModel();
    public BulkEmpSumAmountModel = new BulkEmpSumAmountModel();
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public EmployeeList: any[] = [];
    public BulkFilterEmployeeList: any[] = [];
    public BulkEmployeeList: any[] = [];
    public BulkEmployeeTypeList: any[] = [];
    public BulkDepartmentsList: any[] = [];
    public BulkDesignationsList: any[] = [];
    public BulkLocationsList: any[] = [];
    public AllConDedTypes: any[] = [];
    public IsList: boolean = true;
    public PayrollRegion: string;
    public Currency: string;
    public IsAllConDedValueValid: boolean = false;
    public ID: number = 10;
    public Rights: any;
    public submitted: boolean; // keep track on whether form is submitted
    Form1: FormGroup;
    public ControlRights: any;
    public valueForUser: any;
    length = 100;
    pageSize = 5;
    pageSizeOptions: number[] = [5, 10, 25, 100];
    selection = new SelectionModel<Element>(true, []);
    pageEvent: PageEvent;
    setPageSizeOptions(setPageSizeOptionsInput: string) {
        this.pageSizeOptions = setPageSizeOptionsInput.split(',').map(str => +str);
    }
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
        maxHeight: '230px',
    };
    public ColumnTexts: IMultiSelectTexts = {
        checkAll: 'Select all',
        checked: 'checked',
        checkedPlural: 'checked',
        searchPlaceholder: 'Search...',
        defaultTitle: 'All',
    };
    constructor(public _fb: FormBuilder, public _CommonService: CommonService, public _router: Router
        , public loader: LoaderService, private encrypt: EncryptionService, public _EmployeeService: EmployeeService, public toastr: CommonToastrService
    ) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("50");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
        this.Currency = this._CommonService.getCurrency();
        this.PayrollRegion = this._CommonService.getPayrollRegion();
    }
    ngOnInit() {
        this.displayedColumns = this.columnNames.map(x => x.id);
        this.PConfig.PrimaryColumn = "ID";
        this.PConfig.ColumnVisibilityCookieName = "Calendar" + this.ID;
        this.PConfig.Action.Add = true;
        this.PConfig.Action.AddTextType = "T";
        this.PConfig.Action.AddText = "Add";
        this.PConfig.Fields = [
            { Name: "Name", Title: "Name", OrderNo: 1, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Designation", Title: "Designation", OrderNo: 2, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Location", Title: "Location", OrderNo: 3, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Department", Title: "Department", OrderNo: 4, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
            { Name: "Status", Title: "Status", OrderNo: 5, SortingAllow: true, Visible: true, isDate: false, DateFormat: "" },
        ];


        this.Form1 = this._fb.group({
            AllConDedId: [''],
            AllConDedValueType: [''],
            Amount: ['', [Validators.required]],
            Taxable: [''],
            LocationsIds: [''],
            DepartmentsIds: [''],
            DesignationsIds: [''],
            EmployeeTypeIds: [''],
            EmployeeIds: [''],
        });

    }
    createTable(dataList) {
        let tableArr: Element[] = dataList;
        this.dataSource = new MatTableDataSource(tableArr);
        this.dataSource.sort = this.sort;
        this.paginator.pageSize = 5
        this.paginator.pageIndex = 0
        this.dataSource.paginator = this.paginator;
    }

    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();

        this.Id = "0";
        this._EmployeeService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.EmployeeList = m.DataList;
                this.loader.HideLoader();
            });
    }

    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {

        this.loader.ShowLoader();
        this._EmployeeService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }

    AddRecord(id: string) {

        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Employee/saveemployee']);
        }

        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Employee/saveemployee'], { queryParams: { id: this.Id } });
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
            this._EmployeeService.Delete(id).then(m => {
                if (m.ErrorMessage != null) {
                    this.toastr.Error('Error', m.ErrorMessage);
                }
                this.Refresh();
            });
        }
    }

    GetList() {
        this.Refresh();
    }


    Close() {

        this.IsList = true;
        this.Refresh();
    }

    Next() {
        this.loader.ShowLoader();
        this._EmployeeService.GetBulkFilterData(this.BulkModel.BulkUpdateCategoryType).then(m => {
            this.AllConDedTypes = JSON.parse(JSON.stringify(m.ResultSet.AllConDedTypes));
            if (this.AllConDedTypes.length > 0) {
                this.BulkModel.AllConDedId = this.AllConDedTypes[0].ID;
                this.BulkModel.BulkUpdateCategoryType = this.AllConDedTypes[0].Category;
                if (this.BulkModel.BulkUpdateCategoryType != 'B') {
                    this.BulkModel.AllConDedValueType = this.AllConDedTypes[0].ValueType;
                    this.BulkModel.Taxable = this.AllConDedTypes[0].Taxable;
                    this.BulkModel.Amount = this.AllConDedTypes[0].Amount;
                } else this.BulkModel.Amount = 0;
            }
            this.BulkLocationsList = m.ResultSet.Locations;
            this.BulkDepartmentsList = m.ResultSet.Departments;
            this.BulkDesignationsList = m.ResultSet.Designations;
            this.BulkEmployeeTypeList = m.ResultSet.EmpTypes;
            this.BulkEmployeeList = m.ResultSet.Emplist;
            this.loader.HideLoader();
        });

        document.getElementById('CloseBulkUpdateModalId').click();
        document.getElementById('OpenBulkModalfilterId').click();
    }
    AllConDedTypesChange() {
        var item = this.AllConDedTypes.filter(x => x.ID == this.BulkModel.AllConDedId);
        if (item.length > 0) {
            this.BulkModel.Amount = item[0].Amount;
            this.BulkModel.AllConDedValueType = item[0].ValueType;
            this.BulkModel.BulkUpdateCategoryType = item[0].Category;
            this.BulkModel.Taxable = item[0].Taxable;
        }
    }

    IsAllConDedValid() {
        if (this.BulkModel.BulkUpdateCategoryType == 'B') {
            this.IsAllConDedValueValid = this.BulkModel.Amount <= 0 ? true : false;
        } else {
            if (this.BulkModel.AllConDedValueType == 'P' && this.BulkModel.Amount > 100)
                this.IsAllConDedValueValid = true;
            else
                this.IsAllConDedValueValid = false;
        }
    }

    BulkUpdateFilterPreviousClick() {
        this.BulkModel.DepartmentsIds = [];
        this.BulkModel.DesignationsIds = [];
        this.BulkModel.EmployeeIds = [];
        this.BulkModel.EmployeeTypeIds = [];
        this.BulkModel.LocationsIds = [];
        document.getElementById('CloseBulkModalfilterId').click();
        document.getElementById('btnBulkModalId').click();
    }
    BulkFilterEmpPreviousClick() {
        document.getElementById('CloseBFiltEmplyeeModaId').click();
        document.getElementById('OpenBulkModalfilterId').click();
    }
    GetBulkFilterEmployee(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.IsAllConDedValid();
            isValid = !this.IsAllConDedValueValid;

            var item = this.AllConDedTypes.filter(x => x.ID == this.BulkModel.AllConDedId);
            this.BulkModel.Name = item.length > 0 ? item[0].Name : '';



            if (this.BulkModel.BulkUpdateCategoryType == 'A' || this.BulkModel.BulkUpdateCategoryType == 'D') {
                if (this.displayedColumns.length < 5)
                    this.displayedColumns.push("Taxable", "Remove");
                else if (this.displayedColumns.length == 5)
                    this.displayedColumns.splice(4, 0, "Taxable");
                //this.displayedColumns.push("Taxable");
            }
            else if (this.BulkModel.BulkUpdateCategoryType == 'B') {
                if (this.displayedColumns.length > 5)
                    this.displayedColumns.splice(4, 2);
                else if (this.displayedColumns.length == 5)
                    this.displayedColumns.splice(4, 1);
            }
            else if (this.BulkModel.BulkUpdateCategoryType == 'C') {
                if (this.displayedColumns.length == 6)
                    this.displayedColumns.splice(4, 1);
                else if (this.displayedColumns.length == 4)
                    this.displayedColumns.splice(4, 0, "Remove");
            }

            if (this.PayrollRegion == 'SA') {
                if (this.displayedColumns.length == 6)
                    this.displayedColumns.splice(4, 1);
                else if (this.displayedColumns.length == 5)
                    this.displayedColumns.splice(4, 1, 'Remove');
            }
        }

        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._EmployeeService
                .GetBulkFilterEmployees(this.BulkModel).then(m => {
                    var result = JSON.parse(m._body);
                    this.BulkFilterEmployeeList = result.ResultSet.bulkFilterEmployees;
                    this.CalculateAmntAndSum(true);

                    this.createTable(this.BulkFilterEmployeeList);
                    this.TaxableChecked();
                    document.getElementById('CloseBulkModalfilterId').click();
                    document.getElementById('OpenBulkFilterEmplyeeModalId').click();
                    this.loader.HideLoader();
                });
        }
    }

    CalculateAmntAndSum(isCalculatePercentage: boolean) {
        this.BulkEmpSumAmountModel = new BulkEmpSumAmountModel();
        this.BulkFilterEmployeeList.forEach(x => {
            if (isCalculatePercentage && this.BulkModel.AllConDedValueType == 'P') {
                x.Amount = ((x.Amount / 100) * x.BasicSalary);
            }
            this.BulkEmpSumAmountModel.TotalExistingAmount += x.ExistingAmount;
            this.BulkEmpSumAmountModel.TotalAmount += x.Amount;
        });


    }

    applyFilter(filterValue: string) {
        this.dataSource.filter = filterValue.trim().toLowerCase();
        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
    }
    /** Gets the total cost of all transactions. */
    getTotalCost() {
        return this.dataSource.data.map(t => t.position).reduce((acc, value) => acc + value, 0);
    }

    /** Selects all rows if they are not all selected; otherwise clear selection. */
    BulkRemAllChkToggle() {
        if (this.IsRemoveAllChked) {
            this.dataSource.data.forEach(row => {
                row.Remove = true;

            });
        } else {
            this.dataSource.data.forEach(row => {
                row.Remove = false;
            });
        }
        this.IsRemoveIntermediate = false;

    }
    RemoveChecked(rw) {

        var RemChkCount = this.dataSource.data.filter(x => x.Remove == true).length;
        if (RemChkCount == 0) {
            this.IsRemoveIntermediate = false;
        } else this.IsRemoveIntermediate = true;

        if (RemChkCount == this.dataSource.data.length) {
            this.dataSource.data.forEach(row => {
                row.Remove = true;
            });
            this.IsRemoveIntermediate = false;
            this.IsRemoveAllChked = true;
        } else {
            this.IsRemoveAllChked = false;
        }
    }

    BulkExistamntAllChkToggle() {
        if (this.IsExistAmntAllChked) {

            this.dataSource.data.forEach(row => {
                row.UseExistingAmount = true;
                row.Amount = row.ExistingAmount;
            });
        } else {
            this.dataSource.data.forEach(row => {
                row.UseExistingAmount = false;
                row.Amount = this.BulkModel.AllConDedValueType == 'P' ? ((this.BulkModel.Amount / 100) * (this.dataSource.data.length > 0 ? (this.dataSource.data[0].BasicSalary) : 0)) : this.BulkModel.Amount;
            });
        }

        this.IsExistAmntIntermediate = false;

        this.CalculateAmntAndSum(false);
    }
    ExistamntChecked(rw) {
        var ChkCount = this.dataSource.data.filter(x => x.UseExistingAmount == true).length;
        if (ChkCount == 0) {
            this.IsExistAmntIntermediate = false;
        } else this.IsExistAmntIntermediate = true;

        var item = this.dataSource.data.filter(x => x.ID == rw.ID)[0];
        if (rw.UseExistingAmount)
            item.Amount = rw.ExistingAmount;
        else
            item.Amount = this.BulkModel.AllConDedValueType == 'P' ? ((this.BulkModel.Amount / 100) * (this.dataSource.data.length > 0 ? (this.dataSource.data[0].BasicSalary) : 0)) : this.BulkModel.Amount;

        if (ChkCount == this.dataSource.data.length) {
            this.dataSource.data.forEach(row => {
                row.UseExistingAmount = true;
            });
            this.IsExistAmntIntermediate = false;
            this.IsExistAmntAllChked = true;
        } else {
            this.IsExistAmntAllChked = false;
        }

        this.CalculateAmntAndSum(false);
    }

    BulkTaxableAllChkToggle() {
        if (this.IsTaxableAllChked) {
            this.dataSource.data.forEach(row => {
                row.Taxable = true;
            });
        } else {
            this.dataSource.data.forEach(row => {
                row.Taxable = false;
            });
        }
        this.IsExistAmntIntermediate = false;
    }
    TaxableChecked() {
        var ChkCount = this.dataSource.data.filter(x => x.Taxable == true).length;
        if (ChkCount == 0) {
            this.IsTaxableIntermediate = false;
        } else this.IsTaxableIntermediate = true;

        if (ChkCount == this.dataSource.data.length) {
            this.dataSource.data.forEach(row => {
                row.Taxable = true;
            });
            this.IsTaxableIntermediate = false;
            this.IsTaxableAllChked = true;
        } else {
            this.IsTaxableAllChked = false;
        }
    }

    SaveBulkEmployee() {
        this.dataSource.data;

        if (this.PayrollRegion == 'SA') {
            this.dataSource.data.forEach(x => {
                x.Taxable = true;
            });
        }

        this.loader.ShowLoader();
        this._EmployeeService
            .UpdateBulkEmp(this.dataSource.data).then(m => {

                var result = JSON.parse(m._body);
                if (result.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this.BulkModel = new EmpBulkUpdateModel();

                document.getElementById('CloseBFiltEmplyeeModaId').click();
                this.loader.HideLoader();
            });
    }

    BulkUpdateCloseClick() {
        this.BulkModel = new EmpBulkUpdateModel();
    }
}

