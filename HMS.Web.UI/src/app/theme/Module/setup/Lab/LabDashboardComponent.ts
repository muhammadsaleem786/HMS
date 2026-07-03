import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input, TemplateRef } from '@angular/core';
import { lab_patient_test, lab_result_mf, lab_result_dt } from '../Lab/CatalogModel';
import { FormGroup, FormControl, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { LabService } from './LabService';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { FileService } from '../../../../CommonService/FileService';
import { AppointmentService } from '../Appointment/AppointmentService';
import { emr_patient } from '../Appointment/AppointmentModel';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig'
declare var $: any;
import { debounce } from 'lodash';
import { forEach } from '@angular/router/src/utils/collection';
@Component({
    selector: 'app-LabDashboard',
    templateUrl: './LabDashboardComponent.html',
    providers: [LabService, AppointmentService],
})
export class LabDashboardComponent implements OnInit {
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public PatientTestmodel = new lab_patient_test();
    public Resultmodel = new lab_result_mf();
    public model = new emr_patient();
    public ReportModel: any;
    public InvoiceCompanyInfo: any;
    public PayrollRegion: string;
    public Rights: number[] = [];
    public Modules: number[] = [];
    public Keywords: any[] = [];
    public IsEdit: boolean = false;
    public IsNewImage: boolean = true;
    public CompanyObj: any;
    public patientTestForm: FormGroup;
    public resultForm: FormGroup;
    public Form1: FormGroup;
    public TestControlRights: any;
    public EnterResultControlRights: any;
    public ReportControlRights: any;
    public submitted: boolean;
    // Dashboard UI data
    public primaryActions: any[] = [];
    public summaryStats: any[] = [];
    public activeCases: any[] = [];
    public InvoiceTestModel: any[] = [];
    // Filters / controls
    public searchText: string = '';
    public categoryFilters: any[] = [];
    public sampleTypeFilters: any[] = [];
    public statusFilters: any[] = [];
    public GenderList: any[] = [];
    public selectedDepartment: string = '';
    public selectedStatus: string = '';
    public selectedSampleType: string = '';
    public SubTotal: number = 0;
    public Total: number = 0;
    public TotalDiscount: number = 0;
    public valueForUser: any;
    public DiscountTypeList = [];
    public DisType: number = 1;
    public DiscountAmount: number = 0;
    // Test catalogue, order & result dialogs
    @ViewChild('NewTestOrderModal') NewTestOrderModal: TemplateRef<any>;
    @ViewChild('EnterResultModal') EnterResultModal: TemplateRef<any>;
    @ViewChild('PrintLabelModal') PrintLabelModal: TemplateRef<any>;
    @ViewChild('InvoiceModal') InvoiceModal: TemplateRef<any>;
    @ViewChild('ReportModal') ReportModal: TemplateRef<any>;
    @ViewChild("content") PatientContent: TemplateRef<any>;

    private _newTestOrderRef: NgbModalRef;
    private _enterResultRef: NgbModalRef;
    private _printLabelRef: NgbModalRef;
    private _invoiceModalRef: NgbModalRef;
    private _reportModalRef: NgbModalRef;
    private _patientContentRef: NgbModalRef;
    public IsCNICMandatory: any;
    public BillTypeList: any[] = [];
    public TittleList: any[] = [];
    public testCatalogList: any[] = [];
    public selectedSample: any;
    public orderTestSearch: string = '';
    public filteredOrderTests: any[] = [];
    public groupedOrderSamples: any[] = [];
    public orderTotalAmount: number = 0;
    public orderSubTotalAmount: number = 0;
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    // Enter Result state (UI only)
    public enterResultSearchId: string = '';
    public enterResultSearchValue: string = '';
    public Comment: string = '';
    public selectedResultTest: any;
    public selectedResultParameters: any[] = [];
    // Result summary for current test (used by header cards in Enter Result dialog)
    public resultSummary = {
        critical: 0,
        abnormal: 0,
        normal: 0
    };

    constructor(public _fb: FormBuilder, public _router: Router
        , public commonservice: CommonService, public toastr: CommonToastrService
        , public loader: LoaderService, public _AppointmentService: AppointmentService, private encrypt: EncryptionService, public _labService: LabService
        , private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.TestControlRights = this.commonservice.ScreenRights("115");
        this.EnterResultControlRights = this.commonservice.ScreenRights("116");
        this.ReportControlRights = this.commonservice.ScreenRights("117");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
    }
    ngOnInit() {
        this.DiscountTypeList.push({ "id": 1, "Name": "%" });
        this.DiscountTypeList.push({ "id": 2, "Name": "PKR" });
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form1 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: ['', [Validators.required]],
            Email: [''],
            Mobile: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.NumberPattern), Validators.minLength(11), Validators.maxLength(11)]],
            CNIC: [''],
            Image: [''],
            Notes: [''],
            MRNO: [''],
            BillTypeId: [''],
            PrefixTittleId: [''],
            Father_Husband: [''],
            Address: [''],
        });
        this.patientTestForm = this._fb.group({
            PatientId: [''],
            StatusId: [''],
            PatientName: [''],
            PriorityId: [''],
            PriorityName: [''],
            ClinicalNotes: [''],
            CatalogIds: [''],
            Age: [''],
            Gender: [''],
            SampleDate: [''],
            CompletedDate: [''],
            InProgressDate: [''],
            ReferringPhysician: [''],
            DiscountAmount: [''],
            Discount: [''],
            DiscountType: [''],
        });
        this.resultForm = this._fb.group({
            PatientId: [''],
            CatalogId: [''],
            PatientTestId: [''],
            Comment: ['']
        });
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.Refresh();
    }
    getPages(totalRecord: number, recordPerPage: number) {
        if (!isNaN(totalRecord))
            this.totalPages = this.getTotalPages(totalRecord, recordPerPage);
        this.getpagesRange();
    }
    getpagesRange() {
        if (this.pagesRange.indexOf(this.PModel.CurrentPage) == -1 || this.totalPages <= 10)
            this.papulatePagesRange();
        else if (this.pagesRange.length == 1 && this.totalPages > 10)
            this.papulatePagesRange();
    }
    papulatePagesRange() {
        this.pagesRange = [];
        var Result = Math.ceil(Math.max(this.PModel.CurrentPage, 1) / Math.max(this.PModel.RecordPerPage, 1));
        this.previousPage = ((Result - 1) * this.PModel.RecordPerPage)
        this.nextPage = (Result * this.PModel.RecordPerPage) + 1;
        if (this.nextPage > this.totalPages)
            this.nextPage = this.totalPages;
        for (var i = 1; i <= 10; i++) {
            if ((this.previousPage + i) > this.totalPages) return;
            this.pagesRange.push(this.previousPage + i)
        }
    }
    getTotalPages(totalRecord: number, recordPerPage: number): number {
        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
    }
    Refresh() {
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "Test#Test,Patient#Patient,Sample#Sample";
        const department = this.selectedDepartment !== 'All Departments' ? this.selectedDepartment : '';
        const status = this.selectedStatus !== 'All Status' ? this.selectedStatus : '';
        const sampleType = this.selectedSampleType !== 'All Sample Types' ? this.selectedSampleType : '';
        this.PModel.FilterID = `${department},${status},${sampleType}`;
        this._labService
            .GetListWithParm(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID).then(m => {
                this.pagesRange = [];
                this.activeCases = [];
                this.PModel.TotalRecord = m.TotalRecord;
                this.activeCases = m.DataList;
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.categoryFilters = m.OtherData3Model.filter((v: any) => v.DropDownID === 68);
                this.statusFilters = m.OtherData3Model.filter((v: any) => v.DropDownID === 72);
                this.sampleTypeFilters = m.OtherData3Model.filter((v: any) => v.DropDownID === 71);
                this.GenderList = m.OtherData3Model.filter((v: any) => v.DropDownID === 2);
                const counts = m.OtherDataModel || {};
                this.primaryActions = [
                    this.TestControlRights.CreateRights ? { title: 'New Test', subtitle: 'Create a new lab order', icon: 'icon icon-plus' } : null,
                    this.EnterResultControlRights.CreateRights ? { title: 'Enter Results', subtitle: 'Update test findings', icon: 'icon icon-pencil2' } : null,
                ].filter(x => x !== null);;

                this.summaryStats = [
                    { label: 'Pending Tests', value: counts.PendingTests || 0, badgeText: 'Today', badgeClass: 'badge-primary-light' },
                    { label: 'In Progress', value: counts.InProgressTests || 0, badgeText: 'Live', badgeClass: 'badge-info' },
                    { label: 'Completed Today', value: counts.CompletedToday || 0, badgeText: 'Today', badgeClass: 'badge-success-light' },
                    { label: 'Urgent Tests', value: counts.UrgentTests || 0, badgeText: 'High', badgeClass: 'badge-danger' }
                ];
                this.loader.HideLoader();
            });
    }
    // New Test Order helpers
    public onOpenNewTestOrder(): void {
        // reset order state (UI only)
        this.loader.ShowLoader();
        this._labService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.testCatalogList = m.ResultSet.resultObj;
                this.filteredOrderTests = this.testCatalogList.map(t => ({ ...t, selected: false }));
                this.PatientTestmodel = new lab_patient_test();
                this.PatientTestmodel.PriorityId = 1;
                this.PatientTestmodel.PriorityName = 'Routine';
                this.orderTestSearch = '';
                this.groupedOrderSamples = [];
                this.orderTotalAmount = 0;
                this._newTestOrderRef = this.modalService.open(this.NewTestOrderModal, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    openPrintModal(sample: any): void {
        if (sample) {
            this.loader.ShowLoader();
            this._labService.GetPrintById(sample.testId, sample.serialNo).then(m => {
                if (m.IsSuccess) {
                    this.selectedSample = m.ResultSet.result.Header;
                    this._printLabelRef = this.modalService.open(this.PrintLabelModal, { size: 'md', centered: true, windowClass: 'DashboardWraper' });
                    this.loader.HideLoader();
                } else
                    this.loader.HideLoader();
            });
        }

    }
    LoadPatient() {
        $("#PatientSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._labService.SearchPatient(request.term).then(m => {
                    if (m.ResultSet.PatientInfo.length == 0)
                        $("#PatientSearch").val('');
                    if (m.ResultSet.PatientInfo.length != 0)
                        response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.PatientTestmodel.PatientId = ui.item.value;
                this.PatientTestmodel.PatientName = ui.item.label;
                this.PatientTestmodel.Age = ui.item.Age;
                this.PatientTestmodel.Gender = ui.item.Gender;
                this.SelectedPatientTestLoad(ui.item.value);
                return ui.item.label;
            }
        });
    }
    public SelectedPatientTestLoad(id: any): void {
        if (!id) {
            this.toastr.Error("Error", "Please select patient")
            return;
        }
        this.loader.ShowLoader();
        this._labService.SelectedPatientLoadTest(id).then(m => {
            if (m.IsSuccess) {
                debugger;
                var result = m.ResultSet.result;
                const selectedIds = new Set(result.map((r: any) => r.InvestigationId));
                this.filteredOrderTests = this.testCatalogList.map(t => ({
                    ...t,
                    selected: selectedIds.has(t.ID)
                }));
                this.PatientTestmodel.CatalogIds = '';
                this.groupedOrderSamples = [];

                // Call only for selected items
                this.filteredOrderTests
                    .filter(t => t.selected)
                    .forEach(t => this.onToggleTestSelection(t));
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    LoadPriority() {
        $("#PrioritySearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._labService.PrioritySearch(request.term).then(m => {
                    if (m.ResultSet.result.length == 0)
                        $("#PrioritySearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.PatientTestmodel.PriorityId = ui.item.value;
                this.PatientTestmodel.PriorityName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    LoadTestSearch() {
        $("#TestIdSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._labService.TestSearch(request.term).then(m => {
                    if (m.ResultSet.result.length == 0)
                        $("#TestIdSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.enterResultSearchId = ui.item.value;
                this.enterResultSearchValue = ui.item.label;
                this.onSearchResultByTestId();
                return ui.item.label;
            }
        });
    }
    // UI Events - dashboard
    public onSearch(): void {
        // Hook for future filtering logic (currently UI only)
    }
    public onPrimaryActionClick(action: any): void {
        if (!action || !action.title) {
            return;
        }
        if (action.title === 'New Test') {
            this.onOpenNewTestOrder();
        } else if (action.title === 'Enter Results') {
            this.onOpenEnterResult();
        }
    }
    public onSavePatient(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            if (!this.PatientTestmodel.CatalogIds.trim()) {
                this.toastr.Error('Error', 'Please select at least one test');
                return;
            }
            if (!this.PatientTestmodel.PatientId) {
                this.toastr.Error('Error', 'Please select the patient');
                return;
            }
            this.submitted = false;
            this.loader.ShowLoader();
            this.PatientTestmodel.DiscountAmount = this.DiscountAmount;
            if (this.DisType == 1)
                this.PatientTestmodel.DiscountType = "%";
            else if (this.DisType == 2)
                this.PatientTestmodel.DiscountType = "PKR";
            this.PatientTestmodel.Total = this.orderSubTotalAmount;
            this._labService.SaveOrUpdate(this.PatientTestmodel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    if (this._newTestOrderRef) {
                        this._newTestOrderRef.close();
                    }
                    this.Refresh();
                    this.loader.HideLoader();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
        else {
            this.toastr.Error('Error', 'Please select at least one test.');
        }
    }
    public onOrderTestSearchChange(): void {
        const search = (this.orderTestSearch || '').toLowerCase();
        this.filteredOrderTests = this.testCatalogList
            .map(t => {
                const selected = this.groupedOrderSamples
                    .some(g => g.tests.some(x => x.ID === t.ID));
                return { ...t, selected };
            })
            .filter(t => {
                if (!search) {
                    return true;
                }
                return t.name.toLowerCase().includes(search) ||
                    String(t.ID).includes(search);
            });
    }

    public onToggleTestSelection(test: any): void {
        if (!test) {
            return;
        }
        // Get existing ids
        const ids = new Set(
            (this.PatientTestmodel.CatalogIds || '')
                .split(',')
                .map((x: string) => x.trim())
                .filter((x: string) => x)
        );

        if (test.selected) {
            ids.add(String(test.ID));
        } else {
            ids.delete(String(test.ID));
        }

        this.PatientTestmodel.CatalogIds = Array.from(ids).join(',');

        const exists = this.groupedOrderSamples
            .some(g => g.tests.some(t => t.ID === test.ID));

        if (test.selected && !exists) {
            // add
            const sampleKey = test.sample || 'Sample';
            let group = this.groupedOrderSamples.find(g => g.sample === sampleKey);
            if (!group) {
                group = { sample: sampleKey, tests: [] };
                this.groupedOrderSamples.push(group);
            }
            group.tests.push({ ...test });
        } else if (!test.selected && exists) {
            // remove
            this.removeTestFromOrder(test);
        }

        this.recalculateOrderTotals();
    }

    public removeTestFromOrder(test: any): void {
        if (!test || !this.groupedOrderSamples) {
            return;
        }
        this.groupedOrderSamples.forEach(g => {
            g.tests = g.tests.filter(t => t.ID !== test.ID);
        });
        this.groupedOrderSamples = this.groupedOrderSamples.filter(g => g.tests.length > 0);

        // Remove from CatalogIds
        const ids = (this.PatientTestmodel.CatalogIds || '')
            .split(',')
            .map((x: string) => x.trim())
            .filter((x: string) => x && x !== String(test.ID));

        this.PatientTestmodel.CatalogIds = ids.join(',');

        // sync checkbox state
        this.filteredOrderTests = this.filteredOrderTests.map(t => {
            if (t.ID === test.ID) {
                return { ...t, selected: false };
            }
            return t;
        });

        this.recalculateOrderTotals();
    }

    private recalculateOrderTotals(): void {
        let total = 0;
        let SubTotal = 0;
        (this.groupedOrderSamples || []).forEach(g => {
            (g.tests || []).forEach(t => {
                total += Number(t.price) || 0;
                SubTotal += Number(t.price) || 0;
            });
        });
        if (this.PatientTestmodel.Discount != null && this.PatientTestmodel.Discount != undefined) {
            if (this.DisType == 1)
                this.DiscountAmount = (SubTotal) * this.PatientTestmodel.Discount / 100;
            else
                this.DiscountAmount = this.PatientTestmodel.Discount;

            total = total - this.DiscountAmount;
        }
        this.orderTotalAmount = total;
        this.orderSubTotalAmount = SubTotal;
    }
    // Enter Result dialog helpers
    public onOpenEnterResult(): void {
        this.enterResultSearchId = '';
        this.enterResultSearchValue = '';
        this.selectedResultTest = null;
        this.selectedResultParameters = [];
        this._enterResultRef = this.modalService.open(this.EnterResultModal, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });
    }

    public onOpenEnterResultFromCase(item: any): void {
        this.onOpenEnterResult();
        if (item && item.testId) {
            this.enterResultSearchId = item.testId;
            this.onSearchResultByTestId();
        }
    }

    public onSearchResultByTestId(): void {
        const id = (this.enterResultSearchId || '');
        if (!id) {
            this.selectedResultTest = null;
            this.selectedResultParameters = [];
            return;
        }
        this.loader.ShowLoader();
        this._labService.GetById(id).then(m => {
            if (m.IsSuccess) {
                this.selectedResultTest = null;
                this.selectedResultParameters = [];
                this.selectedResultTest = m.ResultSet.result;
                this.Comment = m.ResultSet.result.comment;
                const template = m.ResultSet.result.Parameter || [];
                // clone so we don't mutate templates
                this.selectedResultParameters = template.map(p => ({ ...p }));
                this.recalculateResultSummary();
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    onStatusChange(item: any): void {
        if (item.statusId === 4) {
            const confirmed = confirm("Are you sure you want to mark this test as completed?");
            if (!confirmed) {
                this.Refresh();
                return;
            }
        }
        this.updateTestStatus(item);
    }

    private updateTestStatus(item: any): void {
        this.loader.ShowLoader();
        this.PatientTestmodel = new lab_patient_test();
        this.PatientTestmodel = item;
        this.PatientTestmodel.ID = item.testId;
        this.PatientTestmodel.StatusId = item.statusId;
        this._labService.UpdateStatus(this.PatientTestmodel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.Refresh();
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    public onSaveResults(): void {
        this.submitted = true;
        if (!this.selectedResultTest || !this.selectedResultParameters || !this.selectedResultParameters.length) {
            return;
        }
        this.Resultmodel = new lab_result_mf();
        this.Resultmodel.ID = this.selectedResultTest.ID;
        this.Resultmodel.PatientId = this.selectedResultTest.patientid;
        this.Resultmodel.CatalogId = this.selectedResultTest.catalogId;
        this.Resultmodel.PatientTestId = this.selectedResultTest.testId;
        this.Resultmodel.Comment = this.Comment;

        this.Resultmodel.lab_result_dt = this.selectedResultParameters.map(param => {
            const obj = new lab_result_dt();
            obj.ParameterId = param.parmId;
            obj.Result = param.result;
            obj.Status = param.status;
            obj.Comment = param.comment;
            return obj;
        });
        this.submitted = false;
        this.loader.ShowLoader();
        this._labService.SaveOrUpdateResult(this.Resultmodel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                if (this._enterResultRef) {
                    this._enterResultRef.close();
                }
                this.Refresh();
                this.loader.HideLoader();
            }
            else {
                this.loader.HideLoader();
                this.toastr.Error('Error', result.ErrorMessage);
            }
        });
    }
    // --- Helpers for automatic result status calculation (Normal / Abnormal / Critical) ---
    public onChangeParameterResult(param: any, value: any): void {
        if (!param) {
            return;
        }
        param.result = value;
        this.updateParameterStatus(param);
        this.recalculateResultSummary();
    }

    private updateParameterStatus(param: any): void {
        const raw = (param && param.result != null) ? String(param.result).toString().replace(/,/g, '').trim() : '';
        const val = raw === '' ? NaN : Number(raw);

        if (isNaN(val) || param.normalMin == null || param.normalMax == null) {
            param.status = '';
            return;
        }
        const normalMin = Number(param.normalMin);
        const normalMax = Number(param.normalMax);
        const criticalLow = param.criticalLow != null
            ? Number(param.criticalLow)
            : null;
        const criticalHigh = param.criticalHigh != null
            ? Number(param.criticalHigh)
            : null;

        // Normal range
        if (val >= normalMin && val <= normalMax) {
            param.status = 'Normal';
            return;
        }

        // Below normal range
        if (val < normalMin) {
            if (criticalLow != null && val <= criticalLow) {
                param.status = 'Critical';
            } else {
                param.status = 'Abnormal';
            }
            return;
        }
        // Above normal range
        if (val > normalMax) {
            if (criticalHigh != null && val >= criticalHigh) {
                param.status = 'Critical';
            } else {
                param.status = 'Abnormal';
            }
            return;
        }
    }

    private recalculateResultSummary(): void {
        let critical = 0;
        let abnormal = 0;
        let normal = 0;

        (this.selectedResultParameters || []).forEach(p => {
            if (!p || !p.status) {
                return;
            }
            if (p.status === 'Critical') {
                critical++;
            } else if (p.status === 'Abnormal') {
                abnormal++;
            } else if (p.status === 'Normal') {
                normal++;
            }
        });

        this.resultSummary = {
            critical: critical,
            abnormal: abnormal,
            normal: normal
        };
    }

    public getStatusBadgeClass(param: any): string {
        if (!param || !param.status) {
            return 'badge-secondary-light';
        }
        if (param.status === 'Critical') {
            return 'badge-danger';
        }
        if (param.status === 'Abnormal') {
            return 'badge-warning';
        }
        if (param.status === 'Normal') {
            return 'badge-success';
        }
        return 'badge-secondary-light';
    }

    public getResultRowClass(param: any): any {
        if (!param || !param.status) {
            return {};
        }
        if (param.status === 'Critical') {
            return { 'table-danger': true };
        }
        if (param.status === 'Abnormal') {
            return { 'table-warning': true };
        }
        if (param.status === 'Normal') {
            return { 'table-success': true };
        }
        return {};
    }
    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._labService.Delete(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.Refresh();
            });
        }
    }
    openInvoiceModal(id: any) {
        if (id != null && id !== '') {
            this.loader.ShowLoader();
            this._labService.GetBillByInvoiceNo(id).then(m => {
                if (m.IsSuccess) {
                    this.InvoiceTestModel = [];
                    this.SubTotal = 0;
                    this.Total = 0;
                    this.TotalDiscount = 0
                    if (m.ResultSet.result != null) {
                        this.InvoiceTestModel = m.ResultSet.result[0].InvoiceModel;
                        this.InvoiceCompanyInfo = m.ResultSet.result[0];
                        if (m.ResultSet.result[0].CompanyLogo != null)
                            this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + m.ResultSet.result[0].CompanyLogo;
                        this.SubTotal = m.ResultSet.result[0].Total;
                        this.TotalDiscount = m.ResultSet.result[0].DiscountAmount;
                        this.Total = this.SubTotal - this.TotalDiscount;
                    }
                    this._invoiceModalRef = this.modalService.open(this.InvoiceModal, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });
                    this.loader.HideLoader();
                } else
                    this.loader.HideLoader();
            });
        } else {
            this.toastr.Error('Error', 'Invalid invoice no.');
            return;
        }
    }

    openReportModal(id: any) {
        this.ReportModel = null;
        if (id != null && id !== '') {
            this.loader.ShowLoader();
            this._labService.GenerateReport(id).then(m => {
                if (m.IsSuccess) {
                    this.ReportModel = m.ResultSet;
                    if (m.ResultSet.CollectionCenterImage != null)
                        this.ReportModel.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + m.ResultSet.CollectionCenterImage;
                    this._reportModalRef = this.modalService.open(this.ReportModal, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });
                    this.loader.HideLoader();
                } else
                    this.loader.HideLoader();
            });
        } else {
            this.toastr.Error('Error', 'Invalid invoice no.');
            return;
        }
    }
    DiscountValue(evt: any) {
        if (this.PatientTestmodel.Discount != null && this.PatientTestmodel.Discount != undefined) {
            if (this.DisType == 1 && this.PatientTestmodel.Discount > 100)
                this.PatientTestmodel.Discount = 0;
            this.recalculateOrderTotals();
        }
    }
    AgeChange() {
        this.model.DOB = null;
    }
    openNewPatient() {
        this.loader.ShowLoader();
        this._AppointmentService.GetEMRNO().then(m => {
            this.model = new emr_patient();
            this.model.PrefixTittleId = 1;
            debugger
            if (this.GenderList.length == 1) {
                this.model.Gender = this.GenderList[0].ID;
                if (this.model.Gender == 2)
                    this.model.PrefixTittleId = 1;
                if (this.model.Gender == 1)
                    this.model.PrefixTittleId = 2;
                this.GenderChnage(this.model.Gender);
            }
            this.model.MRNO = m.ResultSet.MRNO;
            this.BillTypeList = m.ResultSet.BillTypeList;
            this.TittleList = m.ResultSet.TittleList;
            this.model.BillTypeId = 1;
            this.loader.HideLoader();
            this._patientContentRef = this.modalService.open(this.PatientContent, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });

        });

    }
    GenderChnage(evnt: any) {
        var genderid = parseInt(evnt);
        if (genderid == 2 && this.model.PrefixTittleId == 1)
            this.model.PrefixTittleId = 2
        if (genderid == 1 && this.model.PrefixTittleId == 2)
            this.model.PrefixTittleId = 1
    }
    changeDiscountType(evt: any) {
        this.DisType = evt;
        if (this.PatientTestmodel.Discount != null && this.PatientTestmodel.Discount != undefined) {
            if (this.DisType == 1 && this.PatientTestmodel.Discount > 100)
                this.PatientTestmodel.Discount = 0;
            this.recalculateOrderTotals();
        }
    }
    PatientSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            if (this.IsCNICMandatory) {
                if (this.model.CNIC === null || this.model.CNIC === "") {
                    this.toastr.Error("Error", "Please enter CNIC.");
                    return;
                }
            }
            this.loader.ShowLoader();
            this._AppointmentService.IsPhoneExist(this.model.Mobile).then(m => {
                this.loader.HideLoader();
                if (m.IsSuccess) {
                    swal({
                        title: "Are you sure?",
                        text: "Are you sure want to add another patient against this " + this.model.Mobile + "",
                        icon: "warning",
                        buttons: ['Cancel', 'Yes'],
                        dangerMode: true,
                    })
                        .then((willDelete) => {
                            if (willDelete) {
                                this.AddUpdatePatient();
                            }
                        });
                } else {
                    this.AddUpdatePatient();
                }
            });

        }
    }
    AddUpdatePatient() {
        this.loader.ShowLoader();
        this._AppointmentService.SaveOrUpdate(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.PatientTestmodel.PatientId = result.ResultSet.CurrentModel.ID;
                this.PatientTestmodel.PatientName = result.ResultSet.CurrentModel.PatientName;
                this.PatientTestmodel.Age = result.ResultSet.CurrentModel.Age;
                this.PatientTestmodel.Gender = result.ResultSet.CurrentModel.Gender == 1 ? "Male" : result.ResultSet.CurrentModel.Gender == 2 ? "Female" : "Other"
                this.toastr.Success(result.Message);
                if (this._patientContentRef) {
                    this._patientContentRef.close();
                }
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    Close() {
        this.PatientTestmodel = new lab_patient_test();
    }
}
