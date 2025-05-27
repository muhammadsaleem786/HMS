import { Component, OnInit, ViewChild, ElementRef, Output, Input, EventEmitter, HostListener, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ValidationErrors, ValidatorFn } from '@angular/forms';
import { EmployeeService } from './EmployeeService';
import { Router } from '@angular/router';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import {
    EmployeeModel, pr_employee_allowance, pr_employee_ded_contribution
    , pr_employee_leave, Salary, pr_employee_document, pr_employee_Dependent
} from './EmployeeModel';
import { CommonService } from '../../../../../CommonService/CommonService';
import { LocationsService } from '../Locations/LocationsService';
import { DepartmentService } from '../Department/DepartmentService';
import { designationService } from '../Designation/designationService';
import { VacationSickLeaveService } from '../VacationSickLeave/VacationSickLeaveService';
import { DeductionContributionService } from '../DeductionContribution/DeductionContributionService';
import { AllowanceService } from '../Allowance/AllowanceService';
import { VacationSickLeaveModel } from '../VacationSickLeave/VacationSickLeaveModel';
import { AllowanceModel } from '../Allowance/AllowanceModel';
import { LocationsModel } from '../Locations/LocationsModel';
import { DepartmentModel } from '../Department/DepartmentModel';
import { designationModel } from '../Designation/designationModel';
import { MatStepper } from '@angular/material';
import { DeductionContributionModel } from '../DeductionContribution/DeductionContributionModel';
import { IMyDateModel } from 'mydatepicker';
import { ActivatedRoute } from '@angular/router';
import { GlobalVariable } from '../../../../../AngularConfig/global';
//import * as echarts from 'echarts';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { PayscheduleService } from '../Payschedule/PayScheduleService';
import { Payschedule } from '../Payschedule/PayScheduleModel';
import { Observable } from 'rxjs';
import { filter } from 'rxjs/operators';
import { debug } from 'util';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

@Component({
    selector: 'setup-EmployeeComponent',
    templateUrl: './EmployeeComponent.html',
    providers: [EmployeeService, DepartmentService, designationService, PayscheduleService
        , AllowanceService, DeductionContributionService
        , VacationSickLeaveService, LocationsService
    ],
})


export class EmployeeComponent implements OnInit, OnDestroy {
    public model = new EmployeeModel();
    public CopyOfEmployeeModel = new EmployeeModel();
    public AllowanceModel = new AllowanceModel();
    public payScheduleModel = new Payschedule();
    public locationModel = new LocationsModel();
    public DedContModel = new DeductionContributionModel();
    public VacSickLeaveModel = new VacationSickLeaveModel();
    public DepartModel = new DepartmentModel();
    public DesignModel = new designationModel();
    public SalaryModel = new Salary();
    public CopyOfSalaryModel = new Salary();
    public documentModel = new pr_employee_document();
    public EmpDependentModel = new pr_employee_Dependent();
    public Paytypes = [];
    public EmployeeTypes = [];
    public PaymentMethodTypes = [];
    public PayDayFallOnHolidayTypes = [];
    public AllowanceCategoryTypes = [];
    public TakenLeavesByEmp = [];
    public Locations = [];
    public Designations = [];
    public Departments = [];
    public AllCities = [];
    public FilterCities = [];
    public Countries = [];
    public OriginFilterCities = [];
    public OriginCountries = [];
    public DestinationFilterCities = [];
    public DestinationCountries = [];
    public SpecialtyTypeList = [];
    public ClassificationTypeList = [];

    public PaySchedules = [];
    public SelectedAllowancesList = [];
    public UnSelectedAllowancesList = [];
    public SelectedContributionList = [];
    public UnSelectedContributionList = [];
    public SelectedDeductionList = [];
    public UnSelectedDeductionList = [];
    public ContributionList = [];
    public DeductionList = [];
    public Allowanceslist = [];
    public Statuses = [];
    public Nationalities = [];
    public VacationAndSickLeaves = [];
    public AccrualFrequencies = [];
    public DocumentTypes = [];
    public DocumentList: any[] = [];
    public NeartoExpireDocs: any[] = [];
    public Rights: any;
    public ControlRights: any;
    public MaritalStatusList: any[] = [];
    public ContractTypeList: any[] = [];
    public RelationshipTypeList: any[] = [];
    public AirTicketClassTypeList: any[] = [];
    public AirTicketFrequencyList: any[] = [];
    public InsuranceClassTypeList: any[] = [];

    public DefaulAllowanceCategoryID: number;
    public DefaulAllowanceType: string;
    public IsValidAllowanceValue: boolean = false;
    public OtherID: number = 0;
    public IsOther: boolean = false;
    public IsValidAllowValue: boolean = false;
    public IsValidDedConValue: boolean = false;
    public DefaultEmployeetypeID: number;
    public DefaultPayTypeID: number;
    public DefaultPaymentTypeID: number;
    public DefaultStatusID: number;
    public IsCheck: boolean = false;
    public Islaststep: boolean;
    public submitted: boolean;
    public IsRegshow: boolean = false;
    public IsAlreadyUser: boolean = false;
    AccountType: string;
    public IsPermanentEmpType: boolean = false;
    public IsSalBnkTransfr: boolean = true;
    public IsShowTerminatedAndFinalDate: boolean = false;
    public AllowancePercentageVal: string;
    public HouseRentIDForSA: number = 0;
    public CityName: string;
    public CountryName: string;
    public PayTypeName: string;
    public EmployeeTypeName: string;
    public PaymentMethodName: string;
    public StatusName: string;
    public ScheduleName: string;
    public VacationHours: number;
    public SickLeaveHours: number;
    public EffectiveFrom: Date;
    public IsPayBaseSalaryChange: boolean = false;
    public IsPayWizardViewed: boolean = false;
    public IsLeaveWizardViewed: boolean = false;
    public IsDocsWizardViewed: boolean = false;
    public IsDependentWizardViewed: boolean = false;
    public IsReviewedWizard: boolean = false;
    public IsAdmin: boolean = false;
    public Currency: string;
    public PayrollRegion: string;
    public IsNewImage: boolean = true;
    public IsNewFile: boolean = true;
    public IsEmpCompletelyLoaded: boolean = false;
    public IsDrpdownDataCompletelyLoaded: boolean = false;
    firstFormGroup: FormGroup;
    secondFormGroup: FormGroup;
    thirdFormGroup: FormGroup;
    fourthFormGroup: FormGroup;
    LocationForm: FormGroup;
    DesignationForm: FormGroup;
    DepartmentForm: FormGroup;
    PayScheduleform: FormGroup;
    AllowanceForm: FormGroup;
    LeaveTypeForm: FormGroup;
    fifthFormGroup: FormGroup;
    sixthFormGroup: FormGroup;
    ConributionDeductionForm: FormGroup;
    DocumentUpldForm: FormGroup;
    EmpDependentForm: FormGroup;
    EmpAirTicketForm: FormGroup;
    MedicalInsuranceForm: FormGroup;
    AirTicketForm: FormGroup;
    isOptional = false;
    public DefaultFallInHoliday: number;
    public IsEdit: boolean = false;
    public IsEditBasicEmployeeInfo: boolean = false;
    public IsEditEmploymentEmployeeInfo: boolean = false;
    public IsEditPayEmployeeInfo: boolean = false;
    public IsEditLeaveEmployeeInfo: boolean = false;
    public IsEditDocEmployeeInfo: boolean = false;
    public IsEditDepEmployeeInfo: boolean = false;
    public IsEmployeeEditSumary: boolean = false;
    public IsShowListModal: boolean = true;
    public IsAdm_UserExist: boolean = false;
    public LeaveTypeID: number;
    public Hours: number;
    public sub: any;
    public StepperSelectedIndex: number = 0;
    public ExpiryDateModalType: string = '';
    public IsHouseOrTransportallowance: boolean = false;
    public Image: string = '';
    @ViewChild('closeLocationModal') closeLocationModal: ElementRef;
    @ViewChild('closeDesignationModal') closeDesignationModal: ElementRef;
    @ViewChild('closeDepartmentModal') closeDepartmentModal: ElementRef;
    @ViewChild('closePayScheduleModal') closePayScheduleModal: ElementRef;
    @ViewChild('closeDedContrModal') closeDedContrModal: ElementRef;
    @ViewChild('closeAllowanceModal') closeAllowanceModal: ElementRef;
    @ViewChild('closeLeaveTypeModal') closeLeaveTypeModal: ElementRef;
    @ViewChild('stepper') stepper;
    public WasInside: boolean = false;
    @HostListener('click', ['$event'],)
    Clickoutdocument(event) {
        if (!this.WasInside)
            this.IsShowListModal = true;
        this.WasInside = false;
    }
    IsModalClick() {
        this.WasInside = true;
    }
    constructor(public _fb: FormBuilder, private encrypt: EncryptionService, public _router: Router, public _employeeService: EmployeeService, public commonservice: CommonService
        , public loader: LoaderService, public _DepartmentService: DepartmentService
        , public _designationService: designationService, public _payscheduleService: PayscheduleService
        , public _allowanceService: AllowanceService, public _deductionContributionService: DeductionContributionService
        , public route: ActivatedRoute
        , public toastr: CommonToastrService
        , public _locationsService: LocationsService, public _vacationSickLeaveService: VacationSickLeaveService
    ) {
        this.Currency = this.commonservice.getCurrency();
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this.commonservice.ScreenRights("50");
    }

    ngOnInit() {
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._employeeService.GetById(this.model.ID).then(m => {
                        if (m.IsSuccess) {
                            this.NeartoExpireDocs = m.ResultSet.NeartoExpireDocs;
                            this.model = m.ResultSet.Employee;
                            this.TakenLeavesByEmp = m.ResultSet.TakenLeavesByEmp;
                            if (this.model.EmployeePic != null || this.model.EmployeePic != undefined) {
                                this.getImageUrlName(this.model.EmployeePic);
                                this.IsNewImage = false;
                            } else this.IsNewImage = true;
                            if (this.model.pr_employee_document.length > 0)
                                this.IsNewFile = false;
                            else
                                this.IsNewFile = true;
                            var user_company = this.model.adm_user_company;
                            if (user_company.length > 0) {
                                if (user_company[0].UserID == user_company[0].AdminID)
                                    this.IsAdmin = true;
                                else
                                    this.IsAdmin = false;
                                this.IsAdm_UserExist = true;
                            } else
                                this.IsAdm_UserExist = false;
                            this.IsEmpCompletelyLoaded = true;
                            if (this.IsEmpCompletelyLoaded && this.IsDrpdownDataCompletelyLoaded)
                                this.LoadDataOnPageLoad();
                            this.LoadForm();
                        }
                    });
                }
            });
        this.firstFormGroup = this._fb.group({
            FirstName: ['', [Validators.pattern(ValidationVariables.AlphabetPattern), Validators.required]],
            LastName: ['', [Validators.pattern(ValidationVariables.AlphabetPattern), Validators.required]],
            EmployeePic: [''],
            Gender: [''],
            DOB: [''],
            StreetAddress: ['', [Validators.required]],
            CityID: ['', [Validators.required]],
            ZipCode: [''],
            CountryID: ['', [Validators.required]],
            HomePhone: [''],
            Mobile: [''],
            EmergencyContactPerson: [''],
            EmergencyContactNo: [''],
            Email: ['', [Validators.pattern(ValidationVariables.EmailPattern)]],
            NationalityID: [''],
            MaritalStatusTypeID: [''],
        });
        this.secondFormGroup = this._fb.group({
            HireDate: ['', [<any>Validators.required]],
            JoiningDate: ['', [<any>Validators.required]],
            StatusID: [''],
            EmployeeTypeID: [''],
            LocationID: [''],
            DesignationID: [''],
            DepartmentID: [''],
            NICNo: [''],
            PassportNumber: [''],
            SCHSNO: [''],
            EmployeeCode: [''],
            PayTypeID: [''],
            BasicSalary: ['', [<any>Validators.required, Validators.min(1)]],
            PaymentMethodID: [''],
            BankName: [''],
            BranchName: [''],
            BranchCode: [''],
            AccountNo: [''],
            SwiftCode: [''],
            TypeStartDate: ['', [<any>Validators.required]],
            TypeEndDate: ['', [<any>Validators.required]],
            TerminatedDate: ['', [<any>Validators.required]],
            FinalSettlementDate: ['', [<any>Validators.required]],
            MedicalInsuranceProvided: [''],
            AirTicketProvided: [''],
            ContractTypeID: ['', [<any>Validators.required]],
            SubContractCompanyName: ['', [<any>Validators.required]],
            SpecialtyTypeID: [''],
            ClassificationTypeID: [''],
        });
        this.thirdFormGroup = this._fb.group({
            PayScheduleID: ['', [Validators.required]],
            EffectiveFrom: ['', [Validators.required]],
        });
        this.fourthFormGroup = this._fb.group({
            Category: [''],
            Hours: [''],
            TypeName: [''],
            AccrualFrequencyID: [''],
            LeaveTypeID: [''],
        });
        this.fifthFormGroup = this._fb.group({
            DocumentTypeID: [''],
            Description: [''],
            AttachmentPath: [''],
            ExpireDate: [''],
        });
        this.DocumentUpldForm = this._fb.group({
            DocumentName: [''],
            DocumentPath: [''],
            DocumentType: [''],
        });
        this.EmpDependentForm = this._fb.group({
            RelationshipTypeID: [''],
            IsEmergencyContact: [false],
            IsTicketEligible: [false],
            FirstName: [''],
            LastName: [''],
            Gender: [''],
            MaritalStatusTypeID: [''],
            IdentificationNumber: [''],
            PassportNumber: [''],
            DOB: [''],
            NationalityTypeID: [''],
            Remarks: [''],
        });
        this.EmpAirTicketForm = this._fb.group({
            DocumentName: ['', [Validators.required]],
            DocumentPath: ['', [Validators.required]],
            DocumentType: [''],
        });
        this.LocationForm = this._fb.group({
            LocationName: ['', [Validators.required]],
            Address: ['', [Validators.required]],
            CityID: ['', [Validators.required]],
            ZipCode: [''],
            CountryID: ['', [Validators.required]],
        });
        this.DesignationForm = this._fb.group({
            DesignationName: ['', [Validators.required]],
        });
        this.DepartmentForm = this._fb.group({
            DepartmentName: ['', [Validators.required]],
        });
        this.PayScheduleform = this._fb.group({
            ScheduleName: ['', [Validators.required]],
            PeriodStartDate: ['', [Validators.required]],
            PeriodEndDate: [''],
            PayDate: ['', [Validators.required]],
            PayTypeID: [''],
            FallInHolidayID: [''],
            Active: [''],
        });
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
        this.LeaveTypeForm = this._fb.group({
            TypeName: ['', [Validators.required]],
            EarnedValue: ['', [Validators.required]],
            Category: [''],
            AccrualFrequencyID: [''],
        });
        this.MedicalInsuranceForm = this._fb.group({
            InsuranceCardNo: ['', [Validators.required]],
            InsuranceCardNoExpiryDate: ['', [Validators.required]],
            InsuranceClassTypeID: [''],
            TotalPolicyAmountMonthly: [''],
        });
        this.AirTicketForm = this._fb.group({
            NoOfAirTicket: ['', [Validators.required]],
            AirTicketFrequencyTypeID: ['', [Validators.required]],
            OriginCountryTypeID: [''],
            DestinationCountryTypeID: [''],
            OriginCityTypeID: [''],
            DestinationCityTypeID: [''],
            AirTicketClassTypeID: [''],
            AirTicketRemarks: [''],

        });
        if (!this.IsEdit) {
            this.EffectiveFrom = this.model.JoiningDate;
        }
        if (this.PayrollRegion == 'SA')
            this.HouseRentIDForSA = 5;
        else
            this.HouseRentIDForSA = 0;
        //loading all dropdowns
        if (!this.IsEdit)
            this.LoadForm();
    }
    LoadForm() {
        this.loader.ShowLoader();
        this._employeeService.FormLoad().then(m => {
            var result = m.ResultSet;
            if (m.IsSuccess) {
                //if (this.IsEdit) {
                //    this.model = result.Employee;
                //    this.NeartoExpireDocs = result.NeartoExpireDocs;
                //    var user_company = this.model.adm_user_company;
                //    if (user_company.length > 0) {
                //        if (user_company[0].UserID == user_company[0].AdminID)
                //            this.IsAdmin = true;
                //        else
                //            this.IsAdmin = false;

                //        this.IsAdm_UserExist = true;
                //    } else
                //        this.IsAdm_UserExist = false;
                //}
                //this.NeartoExpireDocs = result.NeartoExpireDocs;
                this.Paytypes = result.Paytypes;
                this.EmployeeTypes = result.EmployeeTypes;
                this.Statuses = result.Statuses;
                this.PaymentMethodTypes = result.PaymentMethodTypes;
                this.PayDayFallOnHolidayTypes = result.PayDayFallOnHolidayTypes;
                if (this.PayDayFallOnHolidayTypes.length > 0) {
                    this.DefaultFallInHoliday = this.PayDayFallOnHolidayTypes[0].ID;
                    this.payScheduleModel.FallInHolidayID = this.DefaultFallInHoliday;
                }


                this.AllowanceCategoryTypes = result.AllowanceCategoryTypes;
                var FilterAllowCat = this.AllowanceCategoryTypes.filter(x => { return x.ID == 7 });
                if (FilterAllowCat.length > 0)
                    this.OtherID = FilterAllowCat[0].ID;
                else this.AllowanceModel.AllowanceName = 'a';

                var FilterPaytype = this.Paytypes.filter(x => x.ID == 1);
                if (FilterPaytype.length > 0) {
                    this.DefaultPayTypeID = FilterPaytype[0].ID;
                    this.payScheduleModel.PayTypeID = this.DefaultPayTypeID;
                }

                this.AccrualFrequencies = result.AccrualFrequencies;
                var AccFerq = this.AccrualFrequencies.filter(x => x.ID == 2)
                if (AccFerq.length > 0) {
                    this.VacSickLeaveModel.AccrualFrequencyID = AccFerq[0].ID;
                }

                this.DefaulAllowanceCategoryID = this.AllowanceCategoryTypes[0].ID;
                this.AllowanceModel.CategoryID = this.DefaulAllowanceCategoryID;
                this.PaySchedules = result.PaySchedules;
                this.Locations = result.Locations;
                this.Designations = result.Designations;
                this.Departments = result.Departments;
                this.AllCities = result.Cities;
                this.Countries = result.Countries;
                this.DocumentTypes = result.DocumentTypes;
                this.Allowanceslist = JSON.parse(JSON.stringify(result.DefaultAllowancesList));
                this.ContributionList = result.ContributionList;
                this.DeductionList = result.DeductionList;

                this.VacationAndSickLeaves = result.VacationAndSickLeaves;
                this.Nationalities = result.Nationalities;
                this.MaritalStatusList = result.MaritalStatusList;
                this.SpecialtyTypeList = result.SpecialtyTypeList;
                this.ClassificationTypeList = result.ClassificationTypeList;
                this.ContractTypeList = result.ContractTypeList;


                this.RelationshipTypeList = result.RelationshipTypeList;
                this.AirTicketClassTypeList = result.AirTicketClassTypeList;
                this.AirTicketFrequencyList = result.AirTicketFrequencyList;
                this.InsuranceClassTypeList = result.InsuranceClassTypeList;
                this.EmpDependentModel.MaritalStatusTypeID = result.MaritalStatusList.length > 0 ? result.MaritalStatusList[0].ID : 0;
                this.EmpDependentModel.RelationshipTypeID = result.RelationshipTypeList.length > 0 ? result.RelationshipTypeList[0].ID : 0;


                if (!this.IsEdit) {
                    this.UnSelectedAllowancesList = JSON.parse(JSON.stringify(result.DefaultAllowancesList));
                    this.UnSelectedContributionList = JSON.parse(JSON.stringify(result.ContributionList));
                    this.UnSelectedDeductionList = JSON.parse(JSON.stringify(result.DeductionList));
                    this.SelectedAllowancesList = this.Allowanceslist.filter(x => x.Default == true);
                    this.SelectedContributionList = this.ContributionList.filter(x => x.Default == true);
                    this.SelectedDeductionList = this.DeductionList.filter(x => x.Default == true);
                }

                this.IsDrpdownDataCompletelyLoaded = true;
                if (this.IsDrpdownDataCompletelyLoaded && (this.IsEmpCompletelyLoaded || !this.IsEdit))
                    this.LoadDataOnPageLoad();

            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
            }
            this.loader.HideLoader();
        });
    }
    calculateHours(category: number): number {
        var takenLeave = 0;
        if (this.TakenLeavesByEmp.length > 0) {
            var Takenleaveobj = this.TakenLeavesByEmp.filter(a => a.LeaveTypeID == category);
            if (Takenleaveobj.length > 0)
                takenLeave = Takenleaveobj[0].TotalHours;
            var balance = takenLeave;
            return balance;
        } else
            return 0;
    }

    LoadDataOnPageLoad() {
        if (!this.IsEdit)
            this.model.ContractTypeID = this.ContractTypeList.length > 0 ? this.ContractTypeList[0].ID : 0;
        else
            this.IsEmployeeEditSumary = true;


        this.model.MaritalStatusTypeID = this.MaritalStatusList.length > 0 ? this.MaritalStatusList[0].ID : 0;
        if (!this.IsEdit) {
            this.model.AirTicketClassTypeID = this.AirTicketClassTypeList.length > 0 ? this.AirTicketClassTypeList[0].ID : 0;
            this.model.AirTicketFrequencyTypeID = this.AirTicketFrequencyList.length > 0 ? this.AirTicketFrequencyList[0].ID : 0;
            this.model.InsuranceClassTypeID = this.InsuranceClassTypeList.length > 0 ? this.InsuranceClassTypeList[0].ID : 0;
        }


        var FilterStatus = this.Statuses.filter(x => x.ID == 1);
        if (FilterStatus.length > 0) {
            this.DefaultStatusID = FilterStatus[0].ID;
            if (!this.IsEdit)
                this.model.StatusID = this.DefaultStatusID;
            else {
                this.EmpStatusOnLoad();
            }
        }


        var FilterEmpTypes = this.EmployeeTypes.filter(x => x.ID == 1);
        if (FilterEmpTypes.length > 0) {
            this.DefaultEmployeetypeID = FilterEmpTypes[0].ID;
            if (!this.IsEdit)
                this.model.EmployeeTypeID = this.DefaultEmployeetypeID;
            else {
                this.EmpTypeOnLoad();
            }
        }


        var FilterPaymentType = this.PaymentMethodTypes.filter(x => x.ID == 1);
        if (FilterPaymentType.length > 0) {
            this.DefaultPaymentTypeID = FilterPaymentType[0].ID;
            if (!this.IsEdit)
                this.model.PaymentMethodID = this.DefaultPaymentTypeID;
            else {
                if (this.model.PaymentMethodID == this.DefaultPaymentTypeID)
                    this.PaymentSelectionchange();
                else
                    this.IsSalBnkTransfr = true;
            }
        }


        if (this.IsEdit) {
            if (this.model.pr_employee_allowance.length > 0) {
                this.model.PayScheduleID = this.model.pr_employee_allowance[0].PayScheduleID;
                this.EffectiveFrom = this.model.pr_employee_allowance[0].EffectiveFrom;
            } else if (this.model.pr_employee_ded_contribution.length > 0) {
                this.model.PayScheduleID = this.model.pr_employee_ded_contribution[0].PayScheduleID;
                this.EffectiveFrom = this.model.pr_employee_ded_contribution[0].EffectiveFrom;
            }
            else
                this.EffectiveFrom = this.model.JoiningDate;

            this.UnSelectedAllowancesList = [];
            this.UnSelectedAllowancesList = [];
            this.UnSelectedDeductionList = [];

            this.Allowanceslist.forEach(m => {
                var item = this.model.pr_employee_allowance.filter(x => x.AllowanceID == m.ID);
                if (item.length == 0)
                    m.Default = false;
                else
                    m.Default = true;

                this.UnSelectedAllowancesList.push(JSON.parse(JSON.stringify(m)));
            });
            this.ContributionList.forEach(m => {
                var item = this.model.pr_employee_ded_contribution.filter(x => x.DeductionContributionID == m.ID);
                if (item.length == 0)
                    m.Default = false;
                else
                    m.Default = true;

                this.UnSelectedContributionList.push(JSON.parse(JSON.stringify(m)));
            });
            this.DeductionList.forEach(m => {
                var item = this.model.pr_employee_ded_contribution.filter(x => x.DeductionContributionID == m.ID);
                if (item.length == 0)
                    m.Default = false;
                else
                    m.Default = true;

                this.UnSelectedDeductionList.push(JSON.parse(JSON.stringify(m)));
            });

            this.IsEmployeeEditSumary = true;
            this.CountrySelectionChange();

            this.FilterCities = this.AllCities.filter(x => x.DependedDropDownValueID == this.model.CountryID);
            this.OriginFilterCities = this.AllCities.filter(x => x.DependedDropDownValueID == this.model.OriginCountryTypeID);
            this.DestinationFilterCities = this.AllCities.filter(x => x.DependedDropDownValueID == this.model.DestinationCountryTypeID);
            this.OriginCountrySelChange();
            this.DestinationCountrySelChange();


            this.sixthWizardCall();
            this.CalculateSalary();
        }
        else {

            //this.UnSelectedAllowancesList = JSON.parse(JSON.stringify(result.DefaultAllowancesList));
            //this.UnSelectedContributionList = JSON.parse(JSON.stringify(result.ContributionList));
            //this.UnSelectedDeductionList = JSON.parse(JSON.stringify(result.DeductionList));
            //this.SelectedAllowancesList = this.Allowanceslist.filter(x => x.Default == true);
            //this.SelectedContributionList = this.ContributionList.filter(x => x.Default == true);
            //this.SelectedDeductionList = this.DeductionList.filter(x => x.Default == true);

            //assign default value on page load

            if (this.Countries.length > 0) {
                if (this.PayrollRegion == 'SA') {
                    //SA = 1;
                    this.model.CountryID = 1;
                    this.model.OriginCountryTypeID = 1;
                    this.model.DestinationCountryTypeID = 1;
                    this.locationModel.CountryID = 1;

                } else {
                    //PK=2
                    this.model.CountryID = 2;
                    this.model.OriginCountryTypeID = 2;
                    this.model.DestinationCountryTypeID = 2;
                    this.locationModel.CountryID = 1;
                }


                this.model.CountryID = this.Countries[0].ID;

                this.CountrySelectionChange();
                this.OriginCountrySelChange();
                this.DestinationCountrySelChange();

                this.AddLocationCountrySelChange();
            }

            if (this.VacationAndSickLeaves.length > 0) {

                this.LeaveTypeID = this.VacationAndSickLeaves[0].ID;
                this.Hours = this.VacationAndSickLeaves[0].EarnedValue;
            }

            var FilterPaytype = this.Paytypes.filter(x => x.ID == 1);
            if (FilterPaytype.length > 0) {
                this.DefaultPayTypeID = FilterPaytype[0].ID;
                this.model.PayTypeID = this.DefaultPayTypeID;
            }
            this.payScheduleModel.PayTypeID = this.DefaultPayTypeID;

            var FilterPaySchedule = this.PaySchedules.filter(x => { return x.Active == true });
            if (FilterPaySchedule.length > 0) {
                this.model.PayScheduleID = FilterPaySchedule[0].ID;
            }
        }
    }

    EmpStatusOnLoad() {

        //Inactive status = 2
        if (this.model.StatusID == 2) {
            this.IsShowTerminatedAndFinalDate = true;
            this.model.TypeStartDate = new Date();
            this.model.TypeEndDate = new Date();

        }
        else {
            this.IsShowTerminatedAndFinalDate = false;
            this.model.TerminatedDate = new Date();
            this.model.FinalSettlementDate = new Date();
        }

        if (this.model.ContractTypeID == 1)
            this.model.SubContractCompanyName = 'a';
    }
    EmpTypeOnLoad() {

        //Status Active = 1;
        if (this.model.StatusID == 1) {
            if (this.model.EmployeeTypeID == this.DefaultEmployeetypeID) {
                this.IsPermanentEmpType = false;
                this.model.TypeEndDate = new Date();
                this.model.TypeStartDate = new Date();
            }
        }
    }

    GetCityName() {
        if (this.model.CityID) {
            var city = this.FilterCities.filter(x => x.ID == this.model.CityID);
            if (city.length > 0)
                this.CityName = city[0].Value;
        }
    }
    GetCountryName() {
        if (this.model.CityID) {
            var list = this.Countries.filter(x => x.ID == this.model.CountryID);
            if (list.length > 0)
                this.CountryName = list[0].Value;
        }
    }
    CalculateSalary() {
        this.ValidBaseSalary();

        this.loader.ShowLoader();
        this.UpdateUnselectedlistAmount();

        if (!this.IsEdit) {
            if (!this.IsPayWizardViewed) {
                this.model.pr_employee_allowance = [];
                this.model.pr_employee_ded_contribution = [];

                this.SelectedAllowancesList.forEach(x => {
                    this.AddEmployeeAllowance(x.ID);
                });

                this.SelectedContributionList.forEach(x => {
                    this.AddEmployeeContributionDeduction(x.ID, 'C');
                });

                this.SelectedDeductionList.forEach(x => {
                    this.AddEmployeeContributionDeduction(x.ID, 'D');
                });
                this.ReCalculateSalary();
            }
            else {
                this.toastr.Info('Info', 'Your stubs will be re-calculated !');
                this.PayScreenBaseSalaryChangeCalculation();
            }
        } else {
            this.ReCalculateSalary();
        }

        this.loader.HideLoader();
    }
    ReCalculateSalary() {
        this.ValidBaseSalary();
        //this.loader.ShowLoader();
        this.SalaryModel = new Salary();
        this.SalaryModel.BasicSalary = this.model.BasicSalary ? this.model.BasicSalary : 0;
        this.model.pr_employee_allowance.forEach(x => {
            if (this.PayrollRegion == 'SA') {
                this.SalaryModel.TaxableAllowanceTotal += x.Amount ? x.Amount : 0;
            } else {
                if (x.Taxable)
                    this.SalaryModel.TaxableAllowanceTotal += x.Amount ? x.Amount : 0;
            }
            this.SalaryModel.AllowanceTotal += x.Amount ? x.Amount : 0;
        });
        this.model.pr_employee_ded_contribution.forEach(x => {
            if (x.Category == 'C')
                this.SalaryModel.ContributionTotal += x.Amount ? x.Amount : 0;
            else {
                if (this.PayrollRegion == 'SA') {
                    this.SalaryModel.TaxableDeductionTotal += x.Amount ? x.Amount : 0;
                } else {
                    if (x.Taxable)
                        this.SalaryModel.TaxableDeductionTotal += x.Amount ? x.Amount : 0;
                }
                this.SalaryModel.DeductionTotal += x.Amount ? x.Amount : 0;
            }
        });


        //this.loader.ShowLoader();
        //setTimeout(() => {
        //    this.loader.HideLoader();
        //}, '300');
        this.SalaryModel.CalculateSalary();
        //this.loader.HideLoader();
    }
    PayScreenBaseSalaryChangeCalculation() {
        this.ValidBaseSalary();
        //this.loader.ShowLoader();
        if (this.IsEdit && !this.IsEditPayEmployeeInfo)
            this.toastr.Info('Info', 'Your stubs will be re-calculated !');

        this.SalaryModel = new Salary();
        this.SalaryModel.BasicSalary = this.model.BasicSalary;
        this.UpdateUnselectedlistAmount();
        this.model.pr_employee_allowance.forEach(x => {
            this.IsPayBaseSalaryChange = true;
            this.AddEmployeeAllowance(x.AllowanceID);
            if (this.PayrollRegion == 'SA') {
                this.SalaryModel.TaxableAllowanceTotal += x.Amount ? x.Amount : 0;
            } else {
                if (x.Taxable)
                    this.SalaryModel.TaxableAllowanceTotal += x.Amount ? x.Amount : 0;
            }

            this.SalaryModel.AllowanceTotal += x.Amount ? x.Amount : 0;
        });
        this.model.pr_employee_ded_contribution.forEach(x => {
            this.IsPayBaseSalaryChange = true;
            this.AddEmployeeContributionDeduction(x.DeductionContributionID, x.Category);

            if (x.Category == 'C') {
                this.SalaryModel.ContributionTotal += x.Amount ? x.Amount : 0;
            }
            else {

                if (this.PayrollRegion == 'SA') {
                    this.SalaryModel.TaxableDeductionTotal += x.Amount ? x.Amount : 0;
                } else {
                    if (x.Taxable)
                        this.SalaryModel.TaxableDeductionTotal += x.Amount ? x.Amount : 0;
                }

                this.SalaryModel.DeductionTotal += x.Amount ? x.Amount : 0;
            }
        });

        this.SalaryModel.CalculateSalary();
        //this.loader.HideLoader();
    }
    UpdateUnselectedlistAmount() {
        this.UnSelectedAllowancesList.forEach(f => {
            var item = this.Allowanceslist.filter(x => x.ID == f.ID && f.AllowanceType == 'P');
            if (item.length > 0) { f.AllowanceValue = ((item[0].AllowanceValue / 100) * this.model.BasicSalary); }
        });
        this.UnSelectedContributionList.forEach(f => {
            var item = this.ContributionList.filter(x => x.ID == f.ID && f.DeductionContributionType == 'P');
            if (item.length > 0) { f.DeductionContributionValue = ((item[0].DeductionContributionValue / 100) * this.model.BasicSalary); }
        });
        this.UnSelectedDeductionList.forEach(f => {
            var item = this.DeductionList.filter(x => x.ID == f.ID && f.DeductionContributionType == 'P');
            if (item.length > 0) {
                if (this.PayrollRegion == 'SA' && item[0].DeductionContributionName == 'GOSI')
                    f.DeductionContributionValue = ((item[0].DeductionContributionValue / 100) * (this.model.BasicSalary + this.GetAmountOfHouseRentForSA()));
                else
                    f.DeductionContributionValue = ((item[0].DeductionContributionValue / 100) * this.model.BasicSalary);
            }
        });
    }
    ShowListModal(DedConCategory) {
        this.IsShowListModal = true;
        if (DedConCategory) {
            if (DedConCategory == 'C')
                this.DedContModel.Category = 'C';
            else
                this.DedContModel.Category = 'D';
        }
    }
    ShowModal() {
        this.IntializeModels();
        this.IsShowListModal = false;
    }
    EditEmployeeInfo(ScreenName) {

        if (this.IsEdit && ScreenName != 'save' && ScreenName != 'cancel') {
            this.CopyOfEmployeeModel = JSON.parse(JSON.stringify(this.model));
            this.CopyOfSalaryModel = JSON.parse(JSON.stringify(this.SalaryModel));
        }

        this.IsEmployeeEditSumary = false;
        this.IsEditBasicEmployeeInfo = false;
        this.IsEditEmploymentEmployeeInfo = false;
        this.IsEditPayEmployeeInfo = false;
        this.IsEditLeaveEmployeeInfo = false;
        this.IsEditDocEmployeeInfo = false;
        this.IsEditDepEmployeeInfo = false;
        if (ScreenName == 'basic') {
            this.IsEditBasicEmployeeInfo = true;
            this.CountrySelectionChange();
        }
        else if (ScreenName == 'employment')
            this.IsEditEmploymentEmployeeInfo = true;
        else if (ScreenName == 'pay')
            this.IsEditPayEmployeeInfo = true;
        else if (ScreenName == 'leave')
            this.IsEditLeaveEmployeeInfo = true;
        else if (ScreenName == 'doc')
            this.IsEditDocEmployeeInfo = true;
        else if (ScreenName == 'dep')
            this.IsEditDepEmployeeInfo = true;
        else if (ScreenName == 'cancel') {
            this.IsEmployeeEditSumary = true;
            this.model = JSON.parse(JSON.stringify(this.CopyOfEmployeeModel));
            this.SalaryModel = JSON.parse(JSON.stringify(this.CopyOfSalaryModel));
            if (this.model.pr_employee_allowance.length > 0)
                this.EffectiveFrom = this.model.pr_employee_allowance[0].EffectiveFrom;
            else if (this.model.pr_employee_ded_contribution.length > 0)
                this.EffectiveFrom = this.model.pr_employee_ded_contribution[0].EffectiveFrom;
            else
                this.EffectiveFrom = this.model.JoiningDate;
        }
        else
            this.IsEmployeeEditSumary = true;


    }
    gotostepper(stepper: MatStepper, index) {
        stepper.selectedIndex = index;
    }

    /**
    * Changes the step to the index specified
    * @param {number} index The index of the step
    */
    onStepChange(event: any) {

        if (event.selectedIndex == 2) {
            //if Pay Wizard Selected Index = 2 then 
            if (!this.IsEdit && !this.IsPayWizardViewed)
                this.model.pr_employee_leave = [];

            this.IsPayWizardViewed = true;

            if (this.model.NoOfAirTicket == 0 || this.model.NoOfAirTicket == undefined)
                this.model.AirTicketProvided = 'N';

            if (this.model.InsuranceCardNo == '' || this.model.InsuranceCardNo == '0' || this.model.InsuranceCardNoExpiryDate == null)
                this.model.MedicalInsuranceProvided = 'N';
        }
        else if (event.selectedIndex == 3) {
            //if Leave Wizard Selected Index = 3 then 
            this.IsLeaveWizardViewed = true;
            this.IsPayWizardViewed = true;
        }
        else if (event.selectedIndex == 4) {
            //if Docs Wizard Selected Index = 4 then 
            this.IsDocsWizardViewed = true;
            this.IsLeaveWizardViewed = true;
            this.IsPayWizardViewed = true;
        }
        else if (event.selectedIndex == 5) {
            //if Dependent Wizard Selected Index = 5 then 
            this.IsDocsWizardViewed = true;
            this.IsLeaveWizardViewed = true;
            this.IsPayWizardViewed = true;
            this.IsReviewedWizard = true
            this.sixthWizardCall();
        }
        this.StepperSelectedIndex = event.selectedIndex;
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
    AllowancSelectionChange() {
        if (this.AllowanceModel.CategoryID == this.OtherID) {
            this.IsOther = true;
            this.AllowanceModel.AllowanceName = '';
        }
        else {
            this.IsOther = false;
            this.AllowanceModel.AllowanceName = 'a';
        }
    }
    GotoLeaveWizard() {
        if (!this.IsEdit) this.IsLeaveWizardViewed = true;
    }
    GoToDocsWizard() {
        if (!this.IsEdit) this.IsDocsWizardViewed = true;
    }
    GoToDependentWizard() {
        if (!this.IsEdit) this.IsDependentWizardViewed = true;
    }
    sixthWizardCall() {
        if (!this.IsEdit)
            this.IsReviewedWizard = true;
        var list;
        this.GetCityName();
        this.GetCountryName();
        list = this.Paytypes.filter(x => x.ID == this.model.PayTypeID);
        if (list.length > 0) {
            this.PayTypeName = list[0].Value;
        }
        list = this.EmployeeTypes.filter(x => x.ID == this.model.EmployeeTypeID);
        if (list.length > 0) {
            this.EmployeeTypeName = list[0].Value;
        }
        list = this.Statuses.filter(x => x.ID == this.model.StatusID);
        if (list.length > 0) {
            this.StatusName = list[0].Value;
        }
        this.GetPaymentMethodName();
        list = this.Departments.filter(x => x.ID == this.model.DepartmentID);
        if (list.length > 0)
            this.DepartModel.DepartmentName = list[0].DepartmentName;
        else
            this.DepartModel.DepartmentName = '';
        list = this.Designations.filter(x => x.ID == this.model.DesignationID);
        if (list.length > 0)
            this.DesignModel.DesignationName = list[0].DesignationName;
        else
            this.DesignModel.DesignationName = '';
        // list = this.Locations.filter(x => x.ID == this.model.LocationID);
        //if (list.length > 0)
        //     this.locationModel.LocationName = list[0].LocationName;
        //else
        //     this.locationModel.LocationName = '';

        list = this.PaySchedules.filter(x => x.ID == this.model.PayScheduleID);
        if (list.length > 0) {
            this.ScheduleName = list[0].ScheduleName;
        }

        this.SumLeavesByType();
    }
    GetPaymentMethodName() {
        var list = this.PaymentMethodTypes.filter(x => x.ID == this.model.PaymentMethodID);
        if (list.length > 0)
            this.PaymentMethodName = list[0].Value;
    }
    SumLeavesByType() {
        this.VacationHours = 0;
        this.SickLeaveHours = 0;
        this.model.pr_employee_leave.forEach(x => {
            var item = this.VacationAndSickLeaves.filter(f => f.ID == x.LeaveTypeID);
            if (item.length > 0) {
                if (item[0].Category == 'V') {
                    this.VacationHours += x.Hours;
                }
                else this.SickLeaveHours += x.Hours;
            }
        });
    }
    StatusSelCahnge() {

        //Inactive status = 2
        if (this.model.StatusID == 2) {
            this.IsShowTerminatedAndFinalDate = true;
            this.model.TerminatedDate = null;
            this.model.FinalSettlementDate = null;
            this.model.TypeStartDate = new Date();
            this.model.TypeEndDate = new Date();
            //resigned = 6
            this.model.EmployeeTypeID = 6;
        }
        else {
            this.IsShowTerminatedAndFinalDate = false;
            this.model.TerminatedDate = new Date();
            this.model.FinalSettlementDate = new Date();
            //Permanent = 1
            this.model.EmployeeTypeID = this.DefaultEmployeetypeID
            this.model.TypeStartDate = new Date;
            this.model.TypeEndDate = new Date;

            //ContractTypeID = 1 => Company SponserShip
            this.model.ContractTypeID = 1;
            this.ContractTypeSelChange();
        }
    }

    getPayDate(event: IMyDateModel): void {
        var date = new Date(event.jsdate);
        var day = date.getDay();
        if (this.payScheduleModel.FallInHolidayID == 1) {
            if (date.getDay() == 0) {
                date.setDate(date.getDate() - 2);
            }
            else if (date.getDay() == 6) {
                date.setDate(date.getDate() - 1);
            }
        } else {
            if (date.getDay() == 0)
                date.setDate(date.getDate() + 1);
            else if (date.getDay() == 6)
                date.setDate(date.getDate() + 2);
        }

        this.payScheduleModel.PayDate = date;
    }

    EmpSelectionChange() {

        //Status Active = 1;
        this.IsPermanentEmpType;
        this.model.TypeEndDate;
        this.model.TypeStartDate;
        this.model.FinalSettlementDate;
        this.model.SubContractCompanyName;
        this.IsShowTerminatedAndFinalDate;
        this.model.TerminatedDate;
        if (this.model.StatusID == 1) {
            if (this.model.EmployeeTypeID == this.DefaultEmployeetypeID) {
                this.IsPermanentEmpType = false;
                this.model.TypeEndDate = new Date();
                this.model.TypeStartDate = new Date();
                this.model.ContractTypeID = this.ContractTypeList.length > 0 ? this.ContractTypeList[0].ID : 0;
            }
            else {
                //this.IsPermanentEmpType = true;
                this.model.TypeStartDate = null;
                this.model.TypeEndDate = null;
                this.model.ContractTypeID = this.ContractTypeList.length > 0 ? this.ContractTypeList[0].ID : 0;
                this.ContractTypeSelChange();
            }
        }

        //status Inactive = 2
        // Grater than 5 means the employee type is Resigned,Terminated and End Of Contract etc in case of Inactive Status
        if (this.model.StatusID == 2 && this.model.EmployeeTypeID > 5) {
            this.model.TerminatedDate = null;
            this.model.FinalSettlementDate = null;

        } else {
            this.model.TerminatedDate = new Date();
            this.model.FinalSettlementDate = new Date();
        }
    }
    ContractTypeSelChange() {
        //if Contract Type is Sub Company then ContractTypeID = 2
        if (this.model.ContractTypeID == 2) this.model.SubContractCompanyName = '';
        else this.model.SubContractCompanyName = 'a';
    }
    VacationandSickLeaveSelCahnge() {
        var list = this.VacationAndSickLeaves.filter(x => x.ID == this.LeaveTypeID);
        if (list.length > 0) {
            this.Hours = list[0].EarnedValue;
        }
    }
    PaymentSelectionchange() {
        if (this.model.PaymentMethodID == this.DefaultPaymentTypeID) {
            this.IsSalBnkTransfr = true;
        }
        else {
            this.model.BankName = '';
            this.model.BranchName = '';
            this.model.BranchCode = '';
            this.model.SwiftCode = '';
            this.model.AccountNo = '';
            this.IsSalBnkTransfr = false;
        }

    }
    onHireDateChanged(event: IMyDateModel) {
        if (event) {

            if (this.model.JoiningDate) {
                if (this.model.HireDate > this.model.JoiningDate) {
                    this.toastr.Error('Invalid Hire Date', 'The hire date should be less than or equal to joining date')
                    this.model.HireDate = undefined;
                }

            }
            else {
                if (!this.IsEdit && !this.IsPayWizardViewed)
                    this.EffectiveFrom = this.model.HireDate;

                this.model.JoiningDate = this.model.HireDate;
            }
        }
    }
    onDOBChanged(event: IMyDateModel) {
        if (event) {
            this.getValidDOBDate(this.model.DOB);
        }
    }
    getValidDOBDate(dat: Date) {
        var MiniEmpAge = 9;
        var date = new Date(dat)
        var DOBSelYear = date.getFullYear();
        var CurrentDate = new Date();
        var year = CurrentDate.getFullYear() - MiniEmpAge;
        if (DOBSelYear > year) {
            this.toastr.Error('Invalid Age', 'The employee age should be greater than 9 year');
            this.model.DOB = undefined;
        }
    }
    onDateJoiningChanged(event: IMyDateModel) {
        if (event) {

            if (this.model.HireDate) {
                if (this.model.JoiningDate <= this.model.HireDate) {
                    this.toastr.Error('Invalid Joining Date', 'The joining date should be greater than or equal to hire date');
                    this.model.HireDate = undefined;
                }
            }

            if (this.model.TypeStartDate && this.DefaultEmployeetypeID != this.model.EmployeeTypeID && this.model.JoiningDate > this.model.TypeStartDate) {
                this.toastr.Error('Invalid Date', 'The start date should be greater than  joining date');
                this.model.TypeStartDate = undefined;
            }

            //if (!this.IsEdit)
            this.EffectiveFrom = this.model.JoiningDate;

        }
    }
    onTypeStartDateChanged(event: IMyDateModel) {
        if (event) {
            if (this.model.JoiningDate && this.DefaultEmployeetypeID != this.model.EmployeeTypeID) {
                this.model.JoiningDate = new Date(this.model.JoiningDate);
                this.model.TypeStartDate = new Date(this.model.TypeStartDate);
                this.model.TypeEndDate = this.model.TypeEndDate ? new Date(this.model.TypeEndDate) : undefined;
                if (this.model.TypeStartDate < this.model.JoiningDate) {
                    this.toastr.Error('Incorrect Date', 'Start date should be greater than joining date');
                    this.model.TypeStartDate = undefined;
                }

                if (this.model.TypeStartDate > this.model.TypeEndDate) {
                    this.model.TypeEndDate = undefined;
                }
            }
        }
    }
    onTypeEndDateChanged(event: IMyDateModel) {
        if (event) {
            if (this.model.TypeStartDate && this.DefaultEmployeetypeID != this.model.EmployeeTypeID) {
                this.model.TypeEndDate = new Date(this.model.TypeEndDate);
                this.model.TypeStartDate = new Date(this.model.TypeStartDate);
                if (this.model.TypeStartDate > this.model.TypeEndDate) {
                    this.model.TypeEndDate = undefined;
                    this.toastr.Error('Incorrect Date', 'End date should be greater than start date');
                }
            }
        }
    }
    AddVacationAndSickLeaveToTable() {
        if (this.LeaveTypeID == 64) {
        } else {
            if (this.LeaveTypeID == null || this.LeaveTypeID <= 0 || this.Hours == null || this.Hours <= 0) {
                this.toastr.Error('Fields Required', '(*) fields are required');
                return false;
            }
        }

        if (this.IsEdit) {
            if (this.model.pr_employee_leave.length == 0)
                this.model.pr_employee_leave = [];
        } else {
            if (!this.IsPayWizardViewed)
                this.model.pr_employee_leave = [];
        }

        if (this.LeaveTypeID) {
            var IsAlreadyExist = this.model.pr_employee_leave.filter(x => x.LeaveTypeID == this.LeaveTypeID);
            if (IsAlreadyExist.length == 0) {
                var obj = this.VacationAndSickLeaves.filter(f => f.ID == this.LeaveTypeID);
                if (obj.length > 0) {
                    var ObjEL = new pr_employee_leave();
                    ObjEL.LeaveTypeID = this.LeaveTypeID;
                    ObjEL.Hours = this.Hours;

                    this.model.pr_employee_leave.push(ObjEL);
                }
            }
            else
                this.toastr.Warning('Already Exist', this.GetLeaveTypeName(this.LeaveTypeID) + ' already exist, please try another');
        }
    }
    DeleteVacationLeave(id) {
        var item = this.model.pr_employee_leave.filter(x => x.LeaveTypeID == id);
        if (item.length > 0)
            this.model.pr_employee_leave = this.model.pr_employee_leave.filter(x => x.LeaveTypeID != id);
    }
    CalAllowPercentage(id) {
        var AllowItem = this.model.pr_employee_allowance.filter(x => x.AllowanceID == id);
        if (AllowItem.length > 0) {
            if (AllowItem[0].Amount == null) {
                AllowItem[0].Amount = 0;
                AllowItem[0].Percentage = 0;
            }
            else {
                //AllowItem.Percentage = ((AllowItem.Amount / this.model.BasicSalary) * 100);
                var percentageEA = ((AllowItem[0].Amount / this.model.BasicSalary) * 100).toFixed(2);
                AllowItem[0].Percentage = (parseFloat(percentageEA) == parseInt(percentageEA)) ? parseInt(percentageEA) : parseFloat(percentageEA);
            }
        }
        this.ReCalculateSalary();
    }
    PayWizardViewed() {
        if (!this.IsEdit && !this.IsPayWizardViewed)
            this.model.pr_employee_leave = [];

        this.IsPayWizardViewed = true;

        if (this.model.NoOfAirTicket == 0 || this.model.NoOfAirTicket == undefined)
            this.model.AirTicketProvided = 'N';

        if (this.model.InsuranceCardNo == '' || this.model.InsuranceCardNo == '0' || this.model.InsuranceCardNoExpiryDate == null)
            this.model.MedicalInsuranceProvided = 'N';
    }
    ValidBaseSalary() {
        if (this.model.BasicSalary != null && this.model.BasicSalary != undefined && this.model.BasicSalary != 0) {
            if (this.model.BasicSalary.toString().length > 8)
                this.model.BasicSalary = 0;

            this.SalaryModel.BasicSalary = this.model.BasicSalary;
        }
        else {
            this.model.BasicSalary = this.SalaryModel.BasicSalary;
        }
    }
    CalDedContPercentage(id, category) {
        var item = this.model.pr_employee_ded_contribution.filter(x => x.DeductionContributionID == id);
        if (item.length > 0) {
            if (item[0].Amount == null) {
                item[0].Amount = 0;
                item[0].Percentage = 0;
            }
            else {
                var percentageDC = ((item[0].Amount / this.model.BasicSalary) * 100).toFixed(2);
                item[0].Percentage = (parseFloat(percentageDC) == parseInt(percentageDC)) ? parseInt(percentageDC) : parseFloat(percentageDC);
            }
        }
        this.ReCalculateSalary();
    }
    CountrySelectionChange() {
        this.FilterCities = this.AllCities.filter(x => x.DependedDropDownValueID == this.model.CountryID);
        if (this.FilterCities.length > 0)
            this.model.CityID = this.FilterCities[0].ID;
        else
            this.model.CityID = undefined;
    }
    OriginCountrySelChange() {
        this.OriginFilterCities = this.AllCities.filter(x => x.DependedDropDownValueID == this.model.OriginCountryTypeID);
        if (this.OriginFilterCities.length > 0)
            this.model.OriginCityTypeID = this.OriginFilterCities[0].ID;
        else
            this.model.OriginCityTypeID = undefined;
    }
    DestinationCountrySelChange() {
        this.DestinationFilterCities = this.AllCities.filter(x => x.DependedDropDownValueID == this.model.DestinationCountryTypeID);
        if (this.DestinationFilterCities.length > 0)
            this.model.DestinationCityTypeID = this.DestinationFilterCities[0].ID;
        else
            this.model.DestinationCityTypeID = undefined;
    }
    AddLocationCountrySelChange() {
        this.FilterCities = this.AllCities.filter(x => x.DependedDropDownValueID == this.locationModel.CountryID);
        if (this.FilterCities.length > 0)
            this.locationModel.CityID = this.FilterCities[0].ID;
        else
            this.locationModel.CityID = undefined;
    }
    RemoveAllowanceFromSelList(id) {

        if (id > 0) {
            this.UnSelectedAllowancesList.forEach(f => {
                if (f.ID == id)
                    f.Default = false;
            });
            this.model.pr_employee_allowance = this.model.pr_employee_allowance.filter(x => x.AllowanceID != id);
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
        }
        this.ReCalculateSalary();
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
            this.model.pr_employee_ded_contribution = this.model.pr_employee_ded_contribution.filter(x => x.DeductionContributionID != id);
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
            this.model.pr_employee_ded_contribution = this.model.pr_employee_ded_contribution.filter(x => x.DeductionContributionID != id);
        }
        this.ReCalculateSalary();
    }
    onPayScheduleDateChanged(event: IMyDateModel) {
        var date = new Date(event.jsdate);
        if (date.getMonth() != 12) {
            date.setMonth(date.getMonth() + 1);
        } else {
            date.setMonth(1);
            date.setFullYear(date.getFullYear() + 1);
        }
        date.setDate(date.getDate() - 1);
        this.payScheduleModel.PeriodEndDate = new Date(this.GetFormatDate(date));
    }
    GetFormatDate(date) {
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getMonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return yyyy + '-' + mm + '-' + dd;
    };
    SaveLocation(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.loader.ShowLoader();
            this.submitted = false;
            this._locationsService.SaveLocationAndReturnList(this.locationModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    alert(result.Message);
                    this.model.LocationID = result.ResultSet.ID;
                    this.Locations = result.ResultSet.LocationList;
                    this.closeLocationModal.nativeElement.click();
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    SaveAllowance(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.IsAllowanceValueValid();
            isValid = !this.IsValidAllowValue;
        }
        if (isValid) {
            if (this.AllowanceModel.AllowanceName === 'a' && this.AllowanceModel.CategoryID != this.OtherID)
                this.AllowanceModel.AllowanceName = '';

            this.submitted = false;
            this.loader.ShowLoader();

            if (this.PayrollRegion == 'SA')
                this.AllowanceModel.Taxable = true;

            this._allowanceService.SaveAndReturnAllowance(this.AllowanceModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    var AllowItem = result.ResultSet;
                    this.Allowanceslist.push(JSON.parse(JSON.stringify(AllowItem)));
                    //var objEA = new pr_employee_allowance();
                    if (AllowItem.AllowanceType == 'P') {
                        AllowItem.AllowanceValue = ((AllowItem.AllowanceValue / 100) * this.model.BasicSalary);
                    }
                    AllowItem.Default = false;
                    this.UnSelectedAllowancesList.push(JSON.parse(JSON.stringify(AllowItem)));
                    this.IsShowListModal = true;
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.AllowanceModel.CategoryID = this.DefaulAllowanceCategoryID;
                this.AllowancSelectionChange();
                this.loader.HideLoader();
            });
        }
    }
    SaveContributionDeduction(isValid: boolean): void {
        this.submitted = true; // set form submit to true
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

                    this.IsShowListModal = true;
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    SaveDesignation(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._designationService.SaveAndReturnDesignationList(this.DesignModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.model.DesignationID = result.ResultSet.ID;
                    this.Designations = result.ResultSet.DesignationList;
                    this.closeDesignationModal.nativeElement.click();
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            });
        }
    }
    SaveDepartment(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._DepartmentService.SaveAndReturnDeptList(this.DepartModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.model.DepartmentID = result.ResultSet.ID;
                    this.Departments = result.ResultSet.DepartmentList;
                    this.closeDepartmentModal.nativeElement.click();
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    SavePaySchedule(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._payscheduleService.SaveAndReturnPayScheduleList(this.payScheduleModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.model.PayScheduleID = result.ResultSet.ID;
                    this.PaySchedules = result.ResultSet.PaySchedules;
                    this.closePayScheduleModal.nativeElement.click();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                }
                this.loader.HideLoader();
            });
        }
    }
    SaveLeaveType(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._vacationSickLeaveService.SaveAndReturnLeaveTypeList(this.VacSickLeaveModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    //alert(result.Message);
                    this.model.pr_employee_leave;
                    this.VacationAndSickLeaves = result.ResultSet.LeaveTypesList;
                    this.LeaveTypeID = result.ResultSet.ID;
                    this.Hours = this.VacationAndSickLeaves.filter(f => f.ID == result.ResultSet.ID)[0].EarnedValue;
                    this.closeLeaveTypeModal.nativeElement.click();
                }
                else {
                    this.loader.HideLoader();
                    this.toastr.Error('Error', result.ErrorMessage);
                }
            });
        }
    }
    SaveEmployee(): void {
        this.ReCalculateSalary();
        if (this.IsEdit && this.model.MedicalInsuranceProvided == 'Y' && (this.model.InsuranceCardNo == '' || this.model.InsuranceCardNo == '0' || this.model.InsuranceCardNoExpiryDate == null)) {
            this.ResetMedInsuranceModal();
            this.model.MedicalInsuranceProvided = 'N';
        }
        if (this.IsEdit && this.model.AirTicketProvided == 'Y' && (this.model.NoOfAirTicket == 0 || this.model.NoOfAirTicket == undefined)) {
            this.ResetAirTicketModal();
            this.model.AirTicketProvided = 'N';
        }

        this.model.pr_employee_allowance.forEach(f => {
            f.EffectiveFrom = this.EffectiveFrom;
            f.PayScheduleID = this.model.PayScheduleID;
            if (this.PayrollRegion == 'SA')
                f.Taxable = true;
        });
        this.model.pr_employee_ded_contribution.forEach(f => {
            f.EffectiveFrom = this.EffectiveFrom;
            f.PayScheduleID = this.model.PayScheduleID;
            if (this.PayrollRegion == 'SA' && f.Category == 'D')
                f.Taxable = true;
        });

        //Inactive status = 2
        if (this.model.StatusID == 2) {
            this.model.TypeStartDate = null;
            this.model.TypeEndDate = null;
        }
        if (this.model.StatusID == 1) {
            // if status is active = 1
            this.model.TerminatedDate = null;
            this.model.FinalSettlementDate = null;

            if (this.model.EmployeeTypeID == this.DefaultEmployeetypeID) {
                this.model.TypeStartDate = null;
                this.model.TypeEndDate = null;
            }
        }

        //if (this.model.StatusID != 2) {

        //}

        if (this.model.ContractTypeID != 2 && this.model.SubContractCompanyName === 'a')
            this.model.SubContractCompanyName = '';

        this.model.CountryID = parseInt(this.model.CountryID.toString());

        this.loader.ShowLoader();
        this.submitted = false;
        this._employeeService.SaveOrUpdate(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.loader.HideLoader();
                this.toastr.Success(result.Message);
                //alert(result.Message);
                if (result.ResultSet.IsUpdate) {
                    this.NeartoExpireDocs = result.ResultSet.NeartoExpireDocs;
                    this.model = result.ResultSet.UpdatedModel;
                    if (this.model.EmployeePic) { this.IsNewImage = false; this.IsNewImage = true };
                    if (this.model.pr_employee_document.length > 0) { this.IsNewFile = false; this.IsNewFile = true };
                    this.EditEmployeeInfo('save');
                    this.sixthWizardCall();

                    if (this.model.ContractTypeID == 1)
                        this.model.SubContractCompanyName = 'a';

                    this.EmpStatusOnLoad();
                    this.EmpTypeOnLoad();
                    this._router.navigate(['home/Employee']);

                    //if (this.model.StatusID == 1)
                    //    this.StatusSelCahnge();
                    //if (this.DefaultEmployeetypeID == this.model.EmployeeTypeID)
                    //    this.EmpSelectionChange();
                }
                else
                    this._router.navigate(['home/Employee']);
            }
            else
                this.toastr.Error('Error', result.ErrorMessage);

            this.loader.HideLoader();
        });
    }
    SaveAirTicket(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            document.getElementById('CloseAirTicketModal').click();
        }
    }
    ResetAirTicketModal() {
        this.model.NoOfAirTicket = undefined;
        this.model.AirTicketClassTypeID = this.AirTicketClassTypeList.length > 0 ? this.AirTicketClassTypeList[0].ID : 0;
        this.model.AirTicketFrequencyTypeID = this.AirTicketFrequencyList.length > 0 ? this.AirTicketFrequencyList[0].ID : 0;

        if (this.PayrollRegion == 'SA') {
            //SA = 1;
            this.model.OriginCountryTypeID = 1;
            this.model.DestinationCountryTypeID = 1;
        } else {
            //PK=2
            this.model.OriginCountryTypeID = 2;
            this.model.DestinationCountryTypeID = 2;
        }

        this.OriginCountrySelChange();
        this.DestinationCountrySelChange();
        this.model.AirTicketRemarks = '';
        //this.model.AirTicketProvided = 'N';
    }
    AirTicketModalCrossClick() {
        if (this.model.NoOfAirTicket == 0 || this.model.NoOfAirTicket == undefined)
            this.ResetAirTicketModal();
    }

    SaveInsurance(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            document.getElementById('CloseMedInsuranceModal').click();
        }
    }
    InsuranceModalCrossClick() {
        if (this.model.InsuranceCardNo == '' || this.model.InsuranceCardNo == '0' || this.model.InsuranceCardNoExpiryDate == null)
            this.ResetMedInsuranceModal();
    }
    ResetMedInsuranceModal() {
        this.model.InsuranceCardNo = '';
        this.model.InsuranceClassTypeID = this.InsuranceClassTypeList.length > 0 ? this.InsuranceClassTypeList[0].ID : 0;
        this.model.InsuranceCardNoExpiryDate = null;
        this.model.TotalPolicyAmountMonthly = null;
        //this.model.MedicalInsuranceProvided = 'N';
        //document.getElementById('CloseMedInsuranceModal').click();
    }

    MedInsuranceRadioChange() {

        if (this.model.MedicalInsuranceProvided == 'N')
            this.ResetMedInsuranceModal();
    }
    AirTicktetRadioChange() {
        if (this.model.AirTicketProvided == 'N')
            this.ResetAirTicketModal();
    }

    DeleteEmployee() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._employeeService.Delete(this.model.ID.toString()).then(m => {
                if (m.IsSuccess)
                    this._router.navigate(['/employee']);

                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    AddEmployeeAllowance(AllowanceID): void {
        var obj = this.Allowanceslist.filter(f => f.ID == AllowanceID);
        if (obj.length > 0) {

            var ObjEA = new pr_employee_allowance();
            if (obj[0].AllowanceType == 'F') {
                var percentageEA = ((obj[0].AllowanceValue / this.model.BasicSalary) * 100).toFixed(2);
                ObjEA.Percentage = (parseFloat(percentageEA) == parseInt(percentageEA)) ? parseInt(percentageEA) : parseFloat(percentageEA);
                ObjEA.Amount = obj[0].AllowanceValue;
            } else {
                ObjEA.Percentage = obj[0].AllowanceValue;
                ObjEA.Amount = Math.round((obj[0].AllowanceValue / 100) * this.model.BasicSalary);
            }

            if (!this.IsPayBaseSalaryChange) {
                ObjEA.AllowanceID = AllowanceID;
                ObjEA.Taxable = obj[0].Taxable;
                // Allowance category TransportID = 4 or HouseRentID = 5;
                //if (obj[0].CategoryID == 4 || obj[0].CategoryID == 5)
                //    this.IsHouseOrTransportallowance = true;
                //else
                //    this.IsHouseOrTransportallowance = false;

                this.model.pr_employee_allowance.push(ObjEA);
                this.UnSelectedAllowancesList.forEach(f => {
                    if (f.ID == AllowanceID)
                        f.Default = true;
                });
            }
            else {
                var item = this.model.pr_employee_allowance.filter(x => x.AllowanceID == AllowanceID);
                if (item.length > 0) {
                    item[0].Percentage = ObjEA.Percentage;
                    item[0].Amount = ObjEA.Amount;
                }
            }
            this.IsPayBaseSalaryChange = false;
        }
    }
    GetAllowanceTitle(AllowanceID): string {
        if (AllowanceID != undefined) {
            var obj = this.Allowanceslist.filter(f => f.ID == AllowanceID);
            if (obj.length > 0)
                return obj[0].AllowanceName;
            else
                return "";
        } else
            return "";
    }

    IsHouseOrTranportAllow(AllowanceID): boolean {
        if (AllowanceID != undefined) {
            var CatID = this.Allowanceslist.filter(f => f.ID == AllowanceID)[0].CategoryID;
            // HouseAllow = 5 & TransportAll = 4
            if (CatID == 4 || CatID == 5)
                return true;
            else
                return false;
        } else
            return false;
    }

    AddEmployeeContributionDeduction(ContrDedID, Type): void {
        var obj = null;
        if (Type == 'C') {
            obj = this.ContributionList.filter(f => f.ID == ContrDedID);
        } else {
            obj = this.DeductionList.filter(f => f.ID == ContrDedID);
        }
        if (obj.length > 0) {
            var ObjCD = new pr_employee_ded_contribution();
            if (obj[0].DeductionContributionType == 'F') {
                var percentageDC = ((obj[0].DeductionContributionValue / this.model.BasicSalary) * 100).toFixed(2);
                ObjCD.Percentage = (parseFloat(percentageDC) == parseInt(percentageDC)) ? parseInt(percentageDC) : parseFloat(percentageDC);
                ObjCD.Amount = obj[0].DeductionContributionValue;
            } else {
                ObjCD.Percentage = obj[0].DeductionContributionValue;
                if (this.PayrollRegion == 'SA' && obj[0].Category == 'D' && obj[0].DeductionContributionName == 'GOSI') {
                    ObjCD.Amount = Math.round((obj[0].DeductionContributionValue / 100) * (this.model.BasicSalary + this.GetAmountOfHouseRentForSA()));
                } else {
                    ObjCD.Amount = Math.round((obj[0].DeductionContributionValue / 100) * this.model.BasicSalary);
                }
            }
            if (!this.IsPayBaseSalaryChange) {
                ObjCD.DeductionContributionID = ContrDedID;
                ObjCD.Taxable = obj[0].Taxable;
                ObjCD.Category = Type;
                ObjCD.StartingBalance = 0;
                this.model.pr_employee_ded_contribution.push(ObjCD);

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
            else {
                var item = this.model.pr_employee_ded_contribution.filter(x => x.DeductionContributionID == ContrDedID);
                if (item.length > 0) {
                    item[0].Percentage = ObjCD.Percentage;
                    item[0].Amount = ObjCD.Amount;
                }
            }
            this.IsPayBaseSalaryChange = false;
        }
    }
    GetAmountOfHouseRentForSA(): number {
        var allow = this.Allowanceslist.filter(x => x.CategoryID == this.HouseRentIDForSA);
        if (allow.length > 0 && allow[0].AllowanceType == 'P')
            return (allow[0].AllowanceValue / 100) * this.model.BasicSalary;
        else return 0;
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
    GetLeaveTypeName(LeaveTypeID) {
        if (LeaveTypeID != undefined) {
            var obj = this.VacationAndSickLeaves.filter(f => f.ID == LeaveTypeID);
            if (obj.length > 0)
                return obj[0].TypeName;
            else
                return "";
        } else
            return "";
    }
    GetLeaveTypeCategory(LeaveTypeID) {
        if (LeaveTypeID != undefined) {
            var obj = this.VacationAndSickLeaves.filter(f => f.ID == LeaveTypeID);
            if (obj.length > 0) {
                return obj[0].Category;
            }
            else
                return "";
        }
    }
    IntializeModels() {
        this.locationModel = new LocationsModel();
        if (this.Countries.length > 0) {
            this.locationModel.CountryID = this.Countries[0].ID;
            this.AddLocationCountrySelChange();
        }

        this.DesignModel = new designationModel();
        this.DepartModel = new DepartmentModel();
        this.payScheduleModel = new Payschedule();
        this.AllowanceModel = new AllowanceModel();
        var DefaultContDedCategory = this.DedContModel.Category;
        this.DedContModel = new DeductionContributionModel();
        this.DedContModel.Category = DefaultContDedCategory;
        this.payScheduleModel.FallInHolidayID = this.DefaultFallInHoliday;
        this.payScheduleModel.PayTypeID = this.DefaultPayTypeID;
        this.model.EmployeeTypeID = this.DefaultEmployeetypeID;
        this.model.PaymentMethodID = this.DefaultPaymentTypeID;
        this.AllowanceModel.CategoryID = this.DefaulAllowanceCategoryID;
    }
    ngOnDestroy(): void {
        this.sub.unsubscribe();
    }
    isImageExists(): boolean {

        if (this.documentModel.AttachmentPath == "") {
            return false;
        }
        else {
            return true;
        }
    }
    getFileName(FName) {
        this.documentModel.AttachmentPath = FName;
    }

    getImageUrlName(FName) {
        this.model.EmployeePic = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.Image = GlobalVariable.BASE_File_URL + '' + FName;
        } else {
            this.Image = GlobalVariable.BASE_Temp_File_URL + '' + FName
        }
    }
    Close() {

        this.submitted = false;
        this._router.navigateByUrl('Inv-WasteForm', { skipLocationChange: true }).then(() =>
            this._router.navigate(['Home']));
    }
    ClearValue() {
        this.documentModel.AttachmentPath = '';
        this.IsNewFile = true;
    }
    ClearImageUrl() {
        this.IsNewImage = true;
        this.model.EmployeePic = '';
        this.Image = '';
    }
    AddDocumentToList(): void {
        if (this.documentModel.DocumentTypeID > 0 && this.documentModel.Description != undefined && this.documentModel.AttachmentPath != '' && this.documentModel.ExpireDate != undefined) {
            this.submitted = false;
            this.model.pr_employee_document.push(this.documentModel);
            this.documentModel = new pr_employee_document();
        } else this.toastr.Error('Fields Required', '(*) fields are required');
    }
    AddDependentToList(): void {
        this.submitted = true; // set form submit to true
        if (this.EmpDependentModel.FirstName != '' && this.EmpDependentModel.LastName != '' && this.EmpDependentModel.IdentificationNumber != '' && this.EmpDependentModel.PassportNumber != '' && this.EmpDependentModel.DOB != undefined) {
            this.submitted = false;
            this.model.pr_employee_Dependent.push(this.EmpDependentModel);
            this.EmpDependentModel = new pr_employee_Dependent();
            this.EmpDependentModel.RelationshipTypeID = this.RelationshipTypeList.length > 0 ? this.RelationshipTypeList[0].ID : 0;
            this.EmpDependentModel.MaritalStatusTypeID = this.MaritalStatusList.length > 0 ? this.MaritalStatusList[0].ID : 0;
        } else this.toastr.Error('Fields Required', '(*) fields are required');
    }

    RemoveDocumnetFromList(index) {
        if (index != undefined) this.model.pr_employee_document.splice(index, 1);
    }
    RemoveDependentFromList(index) {
        if (index != undefined) this.model.pr_employee_Dependent.splice(index, 1);
    }
    OpenDocument(path) {
        if (path) window.open((this.IsEdit && !this.IsNewFile) ? (GlobalVariable.BASE_File_URL + '' + path) : (GlobalVariable.BASE_Temp_File_URL + '' + path));
    }
    GetDocumentType(id): string {
        var item = this.DocumentTypes.filter(x => x.ID == id);
        if (item.length > 0)
            return item[0].Value;
        else return '';
    }
    GetMaritalStatusTitle(id): string {
        var item = this.MaritalStatusList.filter(x => x.ID == id);
        if (item.length > 0)
            return item[0].Value;
        else return '';
    }

    GetRelaionshipTitle(id): string {
        var item = this.RelationshipTypeList.filter(x => x.ID == id);
        if (item.length > 0)
            return item[0].Value;
        else return '';
    }
    IsDocExpire(id: number): boolean {
        if (this.NeartoExpireDocs.filter(x => x.ID == id).length > 0) return true;
        else return false;
    }

    IsNewImageEvent() {
        this.IsNewImage = true;
    }
    IsNewFileEvent(FName) {
        this.IsNewFile = true;
    }

    AddExpiryClick(ExpiryType) {
        if (ExpiryType == 'N')
            this.ExpiryDateModalType = 'N';
        else if (ExpiryType == 'P')
            this.ExpiryDateModalType = 'P';
        else
            this.ExpiryDateModalType = 'S';
    }
    GetInActiveLeavingDateTitle(): string {
        var item = this.EmployeeTypes.filter(x => x.ID == this.model.EmployeeTypeID);
        return (item.length > 0) ? (item[0].ID == 8 ? 'EOCA' : item[0].Value) : '';
    }

    IsvalidPhone(txtbxName: string) {
        var re = new RegExp('^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$');
        if (txtbxName == 'HP' && !re.test(this.model.HomePhone))
            this.model.HomePhone = '';
        if (txtbxName == 'WP' && !re.test(this.model.Mobile))
            this.model.Mobile = '';
        if (txtbxName == 'EP' && !re.test(this.model.EmergencyContactNo))
            this.model.EmergencyContactNo = '';
    }

    ValidateName(txtbxName) {
        var re = new RegExp('^[A-Za-z ]+$');
        if (txtbxName == 'FN' && this.model.FirstName != '' && !re.test(this.model.FirstName))
            this.toastr.Error('Invalid Name', 'Please enter only alphabets in First Name');
        if (txtbxName == 'LN' && this.model.LastName != '' && !re.test(this.model.LastName))
            this.toastr.Error('Invalid Name', 'Please enter only alphabets in Last Name');
    }

    OnKeyPress(event): boolean {
        var inputValue = event.which;
        //allow letters and whitespaces only.
        if (!(inputValue >= 65 && inputValue <= 120) && (inputValue != 32 && inputValue != 0 && inputValue != 122 && inputValue != 121)) {
            this.toastr.Error('Invalid Name', 'Please enter only alphabets');
            event.preventDefault();
            return false;
        }
        return true;
    }



}
