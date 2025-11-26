import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input, TemplateRef } from '@angular/core';
import { Company } from '../../models/company.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CompanyService } from '../../setup/Setting/Company/company.service';
import { Router } from '@angular/router';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { LabDashboardService } from './LabDashboardService';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { FileService } from '../../../../CommonService/FileService';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { EncryptionService } from '../../../../CommonService/encryption.service';
@Component({
    selector: 'app-LabDashboard',
    templateUrl: './LabDashboardComponent.html',
    providers: [FormBuilder, CompanyService, LabDashboardService, FileService],
})
export class LabDashboardComponent implements OnInit {
    public model = new Company();
    public PayrollRegion: string;
    public Rights: number[] = [];
    public Modules: number[] = [];
    public Keywords: any[] = [];
    public IsEdit: boolean = false;
    public IsNewImage: boolean = true;   
    public CompanyObj: any; public UsersObj: any;

    // Dashboard UI data
    public primaryActions: any[] = [];
    public summaryStats: any[] = [];
    public activeCases: any[] = [];

    // Filters / controls
    public searchText: string = '';
    public departmentFilters: string[] = ['Biochemistry', 'Clinical Pathology'];
    public statusFilters: string[] = ['Pending', 'In Progress', 'Sample Collected', 'Completed'];
    public sampleTypeFilters: string[] = ['Blood', 'Urine'];
    public selectedDepartment: string = '';
    public selectedStatus: string = '';
    public selectedSampleType: string = '';
    public actionStatusOptions: string[] = ['Pending', 'In Progress', 'Sample Collected', 'Completed'];

    // Test catalogue, order & result dialogs
    @ViewChild('TestCatalogueModal') TestCatalogueModal: TemplateRef<any>;
    @ViewChild('TestParameterModal') TestParameterModal: TemplateRef<any>;
    @ViewChild('NewTestOrderModal') NewTestOrderModal: TemplateRef<any>;
    @ViewChild('EnterResultModal') EnterResultModal: TemplateRef<any>;
    private _testCatalogueRef: NgbModalRef;
    private _testParameterRef: NgbModalRef;
    private _newTestOrderRef: NgbModalRef;
    private _enterResultRef: NgbModalRef;

    public testCatalogueSearch: string = '';
    public testCatalogueTabs: string[] = ['All Tests', 'Biochemistry', 'Hematology', 'Clinical Pathology', 'Microbiology', 'Immunology'];
    public selectedCatalogueTab: string = 'Biochemistry';
    public testCatalogueList: any[] = [];
    public filteredCatalogueList: any[] = [];
    public selectedTestForParameter: any;

    // Add parameter model
    public parameterModel: any = {};

    // Catalogue parameters (UI-only, per test code)
    public catalogueParameters: { [code: string]: any[] } = {};
    public expandedCatalogueCode: string | null = null;

    // Add new catalogue state (UI only)
    public isAddingCatalogue: boolean = false;
    public newCatalogue: any = {
        name: '',
        category: '',
        sample: '',
        tat: '',
        price: null
    };

    // New Test Order state (UI only)
    public newOrder: any = {
        patientId: '',
        patientName: '',
        age: '',
        gender: '',
        priority: '',
        clinicalNotes: ''
    };
    public orderPriorityOptions: string[] = ['Routine', 'Urgent', 'Stat'];
    public orderTestSearch: string = '';
    public filteredOrderTests: any[] = [];
    public groupedOrderSamples: any[] = [];
    public orderTotalAmount: number = 0;

    // Enter Result state (UI only)
    public enterResultSearchId: string = '';
    public selectedResultTest: any;
    public selectedResultParameters: any[] = [];
    private resultParameterTemplates: { [key: string]: any[] } = {};

    // Result summary for current test (used by header cards in Enter Result dialog)
    public resultSummary = {
        critical: 0,
        abnormal: 0,
        normal: 0
    };

    constructor(public _fb: FormBuilder, public _router: Router, public companyService: CompanyService
        , public commonservice: CommonService, public toastr: CommonToastrService
        , public loader: LoaderService, private encrypt: EncryptionService, public _labService: LabDashboardService
        , private http: FileService, private modalService: NgbModal) {
        this.Modules = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Modules')))
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')))
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.UsersObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
    }
    ngOnInit() {
        this.buildDashboardData();
        this.buildTestCatalogueData();
    }  

    // Mock data for dashboard UI (UI-only, can be replaced with API later)
    private buildDashboardData(): void {
        this.primaryActions = [
            { title: 'Test Catalog', subtitle: 'Configure and manage tests', icon: 'icon icon-lab' },
            { title: 'New Test Order', subtitle: 'Create a new lab order', icon: 'icon icon-plus' },
            { title: 'Enter Results', subtitle: 'Update test findings', icon: 'icon icon-pencil2' },
            { title: 'Generate Report', subtitle: 'Download patient reports', icon: 'icon icon-file-text' }
        ];

        this.summaryStats = [
            { label: 'Pending Tests', value: 1, badgeText: 'Today', badgeClass: 'badge-primary-light' },
            { label: 'In Progress', value: 1, badgeText: 'Live', badgeClass: 'badge-info' },
            { label: 'Completed Today', value: 1, badgeText: 'Today', badgeClass: 'badge-success-light' },
            { label: 'Urgent Tests', value: 1, badgeText: 'High', badgeClass: 'badge-danger' }
        ];

        this.activeCases = [
            {
                testId: 'TST-2024-006',
                patientName: 'Emily Davis',
                patientCode: 'PAT-2024-004',
                sampleId: 'SMP-BLD-003',
                testName: 'Lipid Profile',
                sampleType: 'Blood',
                department: 'Biochemistry',
                priority: 'Routine',
                priorityClass: 'badge-secondary',
                status: 'Pending',
                statusClass: 'badge-warning-light',
                orderedDate: new Date(2024, 0, 17),
                actionStatus: 'Pending'
            },
            {
                testId: 'TST-2024-001',
                patientName: 'John Doe',
                patientCode: 'PAT-2024-001',
                sampleId: 'SMP-BLD-001',
                testName: 'Complete Blood Count',
                sampleType: 'Blood',
                department: 'Biochemistry',
                priority: 'Routine',
                priorityClass: 'badge-secondary',
                status: 'In Progress',
                statusClass: 'badge-info',
                orderedDate: new Date(2024, 0, 15),
                actionStatus: 'In Progress'
            }
        ];

        // simple parameter templates keyed by test name
        this.resultParameterTemplates = {
            'Lipid Profile': [
                {
                    name: 'Total Cholesterol',
                    unit: 'mg/dL',
                    reference: '< 200',
                    normalMin: 0,
                    normalMax: 199,
                    criticalLow: null,
                    criticalHigh: 300,
                    result: '',
                    status: '',
                    comment: ''
                },
                {
                    name: 'HDL Cholesterol',
                    unit: 'mg/dL',
                    reference: '> 40',
                    normalMin: 40,
                    normalMax: 200,
                    criticalLow: 25,
                    criticalHigh: null,
                    result: '',
                    status: '',
                    comment: ''
                },
                {
                    name: 'LDL Cholesterol',
                    unit: 'mg/dL',
                    reference: '< 130',
                    normalMin: 0,
                    normalMax: 129,
                    criticalLow: null,
                    criticalHigh: 200,
                    result: '',
                    status: '',
                    comment: ''
                },
                {
                    name: 'Triglycerides',
                    unit: 'mg/dL',
                    reference: '< 150',
                    normalMin: 0,
                    normalMax: 149,
                    criticalLow: null,
                    criticalHigh: 300,
                    result: '',
                    status: '',
                    comment: ''
                }
            ],
            // Complete Blood Count (CBC) parameters with numeric ranges for automatic status calculation
            'Complete Blood Count': [
                {
                    name: 'White Blood Cells',
                    unit: 'cells/µL',
                    reference: '4,000 - 11,000',
                    normalMin: 4000,
                    normalMax: 11000,
                    criticalLow: 2000,
                    criticalHigh: 20000,
                    result: '',
                    status: '',
                    comment: ''
                },
                {
                    name: 'Red Blood Cells',
                    unit: 'cells/µL',
                    reference: '4,200,000 - 5,400,000',
                    normalMin: 4200000,
                    normalMax: 5400000,
                    criticalLow: 2500000,
                    criticalHigh: 7000000,
                    result: '',
                    status: '',
                    comment: ''
                },
                {
                    name: 'Hemoglobin',
                    unit: 'g/dL',
                    reference: '12.0 - 15.5',
                    normalMin: 12,
                    normalMax: 15.5,
                    criticalLow: 7,
                    criticalHigh: 20,
                    result: '',
                    status: '',
                    comment: ''
                },
                {
                    name: 'Hematocrit',
                    unit: '%',
                    reference: '36 - 46',
                    normalMin: 36,
                    normalMax: 46,
                    criticalLow: 20,
                    criticalHigh: 60,
                    result: '',
                    status: '',
                    comment: ''
                },
                {
                    name: 'Platelet Count',
                    unit: 'lakhs/µL',
                    reference: '1.5 - 4.5',
                    normalMin: 1.5,
                    normalMax: 4.5,
                    criticalLow: 0.5,
                    criticalHigh: 8,
                    result: '',
                    status: '',
                    comment: ''
                }
            ]
        };
    }

    private buildTestCatalogueData(): void {
        this.testCatalogueList = [
            {
                code: 'BC001',
                name: 'Blood Sugar - Fasting',
                price: 15,
                category: 'Biochemistry',
                sample: 'Blood',
                tat: '2-4 hours',
                parameterCount: 1
            },
            {
                code: 'BC002',
                name: 'Blood Sugar - Random',
                price: 12,
                category: 'Biochemistry',
                sample: 'Blood',
                tat: '2-4 hours',
                parameterCount: 1
            },
            {
                code: 'BC003',
                name: 'HbA1c (Glycated Hemoglobin)',
                price: 35,
                category: 'Biochemistry',
                sample: 'Blood',
                tat: '6-8 hours',
                parameterCount: 1
            },
            {
                code: 'CP001',
                name: 'Complete Blood Count (CBC)',
                price: 50,
                category: 'Hematology',
                sample: 'Blood',
                tat: '4-6 hours',
                parameterCount: 5
            },
            {
                code: 'UR001',
                name: 'Urine Routine Examination',
                price: 25,
                category: 'Clinical Pathology',
                sample: 'Urine',
                tat: '2-4 hours',
                parameterCount: 3
            }
        ];
        this.applyTestCatalogueFilter();
        // initialise order tests list with catalogue data
        this.filteredOrderTests = this.testCatalogueList.map(t => ({ ...t, selected: false }));

        // Mock parameter lists for catalogue cards
        this.catalogueParameters = {
            'BC001': [
                { name: 'Glucose (Fasting)', unit: 'mg/dL', range: '70 - 100' }
            ],
            'BC002': [
                { name: 'Glucose (Random)', unit: 'mg/dL', range: '70 - 140' }
            ],
            'BC003': [
                { name: 'HbA1c', unit: '%', range: '< 5.7' }
            ],
            'CP001': [
                { name: 'White Blood Cells', unit: 'cells/µL', range: '4,000 - 11,000' },
                { name: 'Hemoglobin', unit: 'g/dL', range: '12.0 - 15.5' }
            ],
            'UR001': [
                { name: 'pH', unit: '', range: '4.5 - 8.0' },
                { name: 'Specific Gravity', unit: '', range: '1.005 - 1.030' }
            ]
        };
    }

    private applyTestCatalogueFilter(): void {
        const tab = this.selectedCatalogueTab;
        const search = (this.testCatalogueSearch || '').toLowerCase();

        this.filteredCatalogueList = this.testCatalogueList.filter(t => {
            const matchesTab = tab === 'All Tests' || t.category === tab;
            const matchesSearch = !search ||
                t.name.toLowerCase().includes(search) ||
                t.code.toLowerCase().includes(search);
            return matchesTab && matchesSearch;
        });
    }

    public toggleCatalogueParameters(test: any): void {
        if (!test || !test.code) {
            return;
        }
        this.expandedCatalogueCode = this.expandedCatalogueCode === test.code ? null : test.code;
    }

    // --- Test catalogue: add new catalogue (UI only) ---
    public onStartAddCatalogue(): void {
        this.isAddingCatalogue = true;
        this.newCatalogue = {
            name: '',
            category: '',
            sample: '',
            tat: '',
            price: null
        };
    }

    public onCancelAddCatalogue(): void {
        this.isAddingCatalogue = false;
    }

    public onSaveCatalogue(isValid: boolean): void {
        if (!isValid) {
            return;
        }

        const nextCodeNumber = this.testCatalogueList.length + 1;
        const code = 'NEW' + nextCodeNumber.toString().padStart(3, '0');

        const newItem = {
            code: code,
            name: this.newCatalogue.name,
            price: Number(this.newCatalogue.price) || 0,
            category: this.newCatalogue.category,
            sample: this.newCatalogue.sample,
            tat: this.newCatalogue.tat,
            parameterCount: 0
        };

        this.testCatalogueList = [newItem, ...this.testCatalogueList];
        this.applyTestCatalogueFilter();
        this.isAddingCatalogue = false;
        this.toastr.Success('Test added to catalogue.');
    }

    // UI Events - dashboard
    public onSearch(): void {
        // Hook for future filtering logic (currently UI only)
    }

    public onPrimaryActionClick(action: any): void {
        if (!action || !action.title) {
            return;
        }

        if (action.title === 'Test Catalog') {
            this.onOpenTestCatalogue();
        } else if (action.title === 'New Test Order') {
            this.onOpenNewTestOrder();
        } else if (action.title === 'Enter Results') {
            this.onOpenEnterResult();
        }
    }

    // UI Events - Test catalogue
    public onOpenTestCatalogue(): void {
        this.applyTestCatalogueFilter();
        // Using 'lg' here to match available ng-bootstrap modal size typings in this project
        this._testCatalogueRef = this.modalService.open(this.TestCatalogueModal, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });
    }

    public onCatalogueSearchChange(): void {
        this.applyTestCatalogueFilter();
    }

    public onChangeCatalogueTab(tab: string): void {
        this.selectedCatalogueTab = tab;
        this.applyTestCatalogueFilter();
    }

    public onOpenParameterDialog(test: any): void {
        this.selectedTestForParameter = test;
        this.parameterModel = {
            name: '',
            unit: '',
            referenceRange: '',
            normalMin: '',
            normalMax: '',
            criticalLow: '',
            criticalHigh: '',
            description: ''
        };
        this._testParameterRef = this.modalService.open(this.TestParameterModal, { size: 'lg', centered: true });
    }

    public onSaveParameter(isValid: boolean): void {
        if (!isValid) {
            return;
        }
        // For now just close modal and show a toast; can be replaced with API call later
        this.toastr.Success('Parameter added successfully.');
        if (this._testParameterRef) {
            this._testParameterRef.close();
        }
    }

    // New Test Order helpers
    public onOpenNewTestOrder(): void {
        // reset order state (UI only)
        this.newOrder = {
            patientId: '',
            patientName: '',
            age: '',
            gender: '',
            priority: '',
            clinicalNotes: ''
        };
        this.orderTestSearch = '';
        this.filteredOrderTests = this.testCatalogueList.map(t => ({ ...t, selected: false }));
        this.groupedOrderSamples = [];
        this.orderTotalAmount = 0;

        // Use 'lg' size to comply with available NgbModal sizes in this project
        this._newTestOrderRef = this.modalService.open(this.NewTestOrderModal, { size: 'lg', centered: true, windowClass: 'DashboardWraper' });
    }

    public onOrderTestSearchChange(): void {
        const search = (this.orderTestSearch || '').toLowerCase();
        this.filteredOrderTests = this.testCatalogueList
            .map(t => {
                const selected = this.groupedOrderSamples
                    .some(g => g.tests.some(x => x.code === t.code));
                return { ...t, selected };
            })
            .filter(t => {
                if (!search) {
                    return true;
                }
                return t.name.toLowerCase().includes(search) ||
                    t.code.toLowerCase().includes(search);
            });
    }

    public onToggleTestSelection(test: any): void {
        if (!test) {
            return;
        }

        const exists = this.groupedOrderSamples
            .some(g => g.tests.some(t => t.code === test.code));

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
            g.tests = g.tests.filter(t => t.code !== test.code);
        });
        this.groupedOrderSamples = this.groupedOrderSamples.filter(g => g.tests.length > 0);

        // sync checkbox state
        this.filteredOrderTests = this.filteredOrderTests.map(t => {
            if (t.code === test.code) {
                return { ...t, selected: false };
            }
            return t;
        });

        this.recalculateOrderTotals();
    }

    private recalculateOrderTotals(): void {
        let total = 0;
        (this.groupedOrderSamples || []).forEach(g => {
            (g.tests || []).forEach(t => {
                total += Number(t.price) || 0;
            });
        });
        this.orderTotalAmount = total;
    }

    public canCreateOrder(): boolean {
        return !!this.newOrder &&
            !!this.newOrder.patientId &&
            !!this.newOrder.patientName &&
            !!this.newOrder.priority &&
            this.orderTotalAmount > 0;
    }

    // Enter Result dialog helpers
    public onOpenEnterResult(): void {
        this.enterResultSearchId = '';
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
        const id = (this.enterResultSearchId || '').trim();
        if (!id) {
            this.selectedResultTest = null;
            this.selectedResultParameters = [];
            return;
        }

        const match = this.activeCases.find(x => x.testId === id);
        if (!match) {
            this.selectedResultTest = null;
            this.selectedResultParameters = [];
            return;
        }

        this.selectedResultTest = match;
        const template = this.resultParameterTemplates[match.testName] || [];
        // clone so we don't mutate templates
        this.selectedResultParameters = template.map(p => ({ ...p }));
        this.recalculateResultSummary();
    }

    public onSaveResults(): void {
        if (!this.selectedResultTest || !this.selectedResultParameters || !this.selectedResultParameters.length) {
            return;
        }
        // UI-only: show a success toast, can be replaced with API call
        this.toastr.Success('Results saved successfully.');
        if (this._enterResultRef) {
            this._enterResultRef.close();
        }
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

        const hasCriticalLow = param.criticalLow != null && param.criticalLow !== undefined;
        const hasCriticalHigh = param.criticalHigh != null && param.criticalHigh !== undefined;

        if ((hasCriticalLow && val <= param.criticalLow) ||
            (hasCriticalHigh && val >= param.criticalHigh)) {
            param.status = 'Critical';
        } else if (val < param.normalMin || val > param.normalMax) {
            param.status = 'Abnormal';
        } else {
            param.status = 'Normal';
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

    Close() {
        this.model = new Company();
    }
}
