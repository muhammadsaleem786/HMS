import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CatalogService } from './CatalogService';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { lab_catalog, lab_parameter } from '../Catalog/CatalogModel';
import { PaginationModel, PaginationConfig } from '../../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
declare var $: any;
@Component({
    moduleId: module.id,
    templateUrl: 'CatalogComponentList.html',
    providers: [CatalogService],
})
export class CatalogComponentList {
    public ActiveToggle: boolean = false;
    public Id: string;
    public IsEdit: boolean = false;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public CatalogList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    public ControlRights: any;

    public submitted: boolean;
    public Catalogmodel = new lab_catalog();
    public Parametermodel = new lab_parameter();
    public catalogForm: FormGroup;
    public parameterForm: FormGroup;
    public testCatalogTabs: any[] = [];
    public filteredCatalogList: any[] = [];
    public selectedCatalogTab: string = 'All';
    public testCatalogList: any[] = [];
    public filteredOrderTests: any[] = [];
    public testCatalogSearch: string = '';
    public expandedCatalogCode: string | null = null;
    public selectedTestForParameter: any;
    @ViewChild('TestCatalogModal') TestCatalogModal: TemplateRef<any>;
    @ViewChild('TestParameterModal') TestParameterModal: TemplateRef<any>;
    private _testCatalogRef: NgbModalRef;
    private _testParameterRef: NgbModalRef;

    constructor(public _fb: FormBuilder, public _CommonService: CommonService, private encrypt: EncryptionService, public loader: LoaderService, public _CatalogService: CatalogService
        , public _router: Router, private modalService: NgbModal, public toastr: CommonToastrService,) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("113");
    }
    ngOnInit() {
        this.TestCatalogData();
        this.catalogForm = this._fb.group({
            Name: [''],
            CategoryId: [''],
            CategoryName: [''],
            SampleId: [''],
            SampleName: [''],
            TAT: [''],
            Price: ['']
        });
        this.parameterForm = this._fb.group({
            ParameterName: [''],
            UnitId: [''],
            UnitName: [''],
            ReferenceRange: [''],
            NormalMinimum: [''],
            NormalMaximum: [''],
            CriticalHigh: [''],
            CriticalLow: [''],
            Description: [''],
            CatalogId: [''],
        });
    }
    private TestCatalogData(): void {
        this.loader.ShowLoader();
        this._CatalogService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.testCatalogTabs = m.ResultSet.result;
                this.testCatalogTabs.unshift({ ID: 0, Value: "All", DropDownID: 68 });
                this.selectedCatalogTab = "All";
                this.testCatalogList = m.ResultSet.resultObj;
                this.applyTestCatalogFilter();
                // initialise order tests list with catalogue data
                this.filteredOrderTests = this.testCatalogList.map(t => ({ ...t, selected: false }));
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });

    }
    public AddCatalog(): void {
        this.Catalogmodel = new lab_catalog();
        this._testCatalogRef = this.modalService.open(this.TestCatalogModal, { size: 'lg', centered: true });
        this.LoadCategory();
    }
    public onSaveCatalog(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._CatalogService.SaveOrUpdate(this.Catalogmodel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.testCatalogList = result.ResultSet.resultObj
                    this.applyTestCatalogFilter();
                    this.toastr.Success(result.Message);
                    if (this._testCatalogRef) {
                        this._testCatalogRef.close();
                    }
                    this.loader.HideLoader();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
        else {
            this.toastr.Error('Error', 'Please select at least one item type.');
        }
    }
    UpdateCatalog(test: any): void {
        this.loader.ShowLoader();
        this._CatalogService.GetById(test.code).then(m => {
            if (m.IsSuccess) {
                this.Catalogmodel = m.ResultSet.result;
                this.Catalogmodel.CategoryName = m.ResultSet.result.sys_drop_down_value.Value;
                this.Catalogmodel.SampleName = m.ResultSet.result.sys_drop_down_value1.Value;
                this._testCatalogRef = this.modalService.open(this.TestCatalogModal, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });
                this.LoadCategory();
                this.LoadSample();
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    LoadCategory() {
        $("#CategorySearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._CatalogService.CategorySearch(request.term, 68).then(m => {
                    if (m.ResultSet.result.length == 0)
                        $("#CategorySearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.Catalogmodel.CategoryId = ui.item.value;
                this.Catalogmodel.CategoryName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    LoadSample() {
        $("#SampleSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._CatalogService.CategorySearch(request.term, 71).then(m => {
                    if (m.ResultSet.result.length == 0)
                        $("#SampleSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.Catalogmodel.SampleId = ui.item.value;
                this.Catalogmodel.SampleName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    public onChangeCatalogTab(tab: any): void {
        this.loader.ShowLoader();
        this.IsEdit = true;
        this._CatalogService.GetByCategoryId(tab.ID).then(m => {
            if (m.IsSuccess) {
                this.filteredCatalogList = m.ResultSet.result;
                this.selectedCatalogTab = tab.Value;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });

    }
    public onCatalogueSearchChange(): void {
        this.applyTestCatalogFilter();
    }
    // Parameter method
    public AddParameterDialog(test: any): void {
        this.Parametermodel = new lab_parameter();
        this.Parametermodel.CatalogId = test.code;
        this.selectedTestForParameter = test;
        this._testParameterRef = this.modalService.open(this.TestParameterModal, { size: 'lg', centered: true });
        this.LoadUnit();
    }
    public onSaveParameter(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._CatalogService.SaveOrUpdateParameter(this.Parametermodel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.testCatalogList = result.ResultSet.resultObj;
                    this.selectedCatalogTab = "All";
                    this.applyTestCatalogFilter();
                    this.toastr.Success(result.Message);
                    if (this._testParameterRef) {
                        this._testParameterRef.close();
                    }
                    this.loader.HideLoader();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
        else {
            this.toastr.Error('Error', 'Please select at least one item type.');
        }
    }
    public UpdateParameter(id: any): void {
        this.loader.ShowLoader();
        this._CatalogService.GetParameterById(id).then(m => {
            if (m.IsSuccess) {
                this.Parametermodel = m.ResultSet.result;
                this.Parametermodel.UnitName = m.ResultSet.result.sys_drop_down_value.Value;
                this.Parametermodel.CatalogId = this.Parametermodel.CatalogId;
                this.selectedTestForParameter = this.testCatalogList.filter(a => a.code == this.Parametermodel.CatalogId)[0];
                this._testParameterRef = this.modalService.open(this.TestParameterModal, { size: 'lg', centered: true });
                this.LoadUnit();
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    LoadUnit() {
        $("#UnitSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._CatalogService.UnitSearch(request.term).then(m => {
                    if (m.ResultSet.result.length == 0)
                        $("#UnitSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 1,
            select: (event, ui) => {
                this.Parametermodel.UnitId = ui.item.value;
                this.Parametermodel.UnitName = ui.item.label;
                return ui.item.label;
            }
        });
    }

    // common method
    public toggleCatalogParameters(test: any): void {
        if (!test || !test.code) {
            return;
        }
        this.expandedCatalogCode = this.expandedCatalogCode === test.code ? null : test.code;
    }
    private applyTestCatalogFilter(): void {
        const tab = this.selectedCatalogTab;
        const search = (this.testCatalogSearch || '').toLowerCase();

        this.filteredCatalogList = this.testCatalogList.filter(t => {
            const matchesTab = tab === 'All' || t.category === tab;
            const matchesSearch = !search ||
                t.name.toLowerCase().includes(search) ||
                t.code.toLowerCase().includes(search);
            return matchesTab && matchesSearch;
        });
    }
    Refresh() {
        if (this.PModel.SearchText == '')
            this.loader.ShowLoader();
        this.Id = "0";
        this._CatalogService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.CatalogList = m.DataList;
                this.loader.HideLoader();
            });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Catalog/saveCatalog']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Catalog/saveCatalog'], { queryParams: { id: this.Id } });
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
            this._CatalogService.Delete(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.Refresh();
            });
        }
    }
    GetList() {
        this.Refresh();
    }
    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else
            this.Refresh();
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._CatalogService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
}