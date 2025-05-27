import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { AdmitService } from './AdmitService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_Appointment, emr_document, patientInfo, DoctorInfo } from './../Appointment/AppointmentModel';
import { emr_patient_bill } from '../Billing/BillingModel';
import { emr_patient, ipd_admission_vital, ipd_admission_notes, ipd_diagnosis, ipd_input_output, ipd_medication_log } from './AdmitModel';
import { Observable } from 'rxjs';
import { AppointmentService } from './../Appointment/AppointmentService';
import { SaleService } from './../Items/Sale/SaleService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
declare var $: any;
@Component({
    templateUrl: './AdmitSummeryComponentList.html',
    moduleId: module.id,
    providers: [AdmitService, AppointmentService, SaleService],
})
export class AdmitSummeryComponentList implements OnInit {
    public Form7: FormGroup;
    public Form1: FormGroup;
    public Form2: FormGroup;
    public Form3: FormGroup;
    public Form4: FormGroup;
    public Form5: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string;
    public Modules = [];
    public PatientVitalList: any[] = [];
    public RouteList: any[] = [];
    public ID: number = 10;
    public model = new emr_patient();
    public diagnosisModel = new ipd_diagnosis();
    public InputModel = new ipd_input_output();
    public VitalModel = new ipd_admission_vital();
    public MedicationModel = new ipd_medication_log();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public IsNewPatientImage: boolean = true;
    public IsNewImage: boolean = true;
    public AdmitId: any;
    public PatientImage: string = '';
    public GenderIds: any;
    public IsCNICMandatory: any;
    public ClinicName: string = "";
    public emr_prescription_dynamicArray = [];
    public emr_prescription_complaint_dynamicArray = [];
    public emr_prescription_diagnos_dynamicArray = [];
    public emr_prescription_investigation_dynamicArray = [];
    public emr_prescription_observation_dynamicArray = [];
    public emr_prescription_MedicineArray = [];
    public emr_prescription_treatment_dynamicArray = [];
    public NotesList: any[] = [];
    public InputOutputList: any[] = [];
    public PrimaryList: any[] = [];
    public SecondaryList: any[] = [];
    public AllergyList: any[] = [];
    public DoctorInfo = new DoctorInfo();
    public PatientRXmodel = new patientInfo();
    public noteModel = new ipd_admission_notes();
    public IsPatient: boolean = false;
    public IsPatientTab: boolean = false;
    public CompanyObj: any;
    public GenderName: string = "";
    public PaidAmount: number = 0;
    public Rights: any;
    public VisitRights: any;
    public IsAllergy: boolean = false;
    public activeTab: string = 'Intake';
    public DynamicTimeHeaders: string[] = [];
    public IntakeList: any[] = [];
    public OutputList: any[] = [];
    public TableData: any[] = [];
    public OutputTableData: any[] = [];
    public MedicationDynamicTimeHeaders: string[] = [];
    public VitalArray: any[] = [];
    public MedicationData: any[] = [];
    public MedicationLogList: any[] = [];
    public PatientId: any;
    public chartOptions: any;
    public lineChartData: any[];
    public lineChartLabels: string[];
    public DocumentList: any[] = [];
    public lineChartOptions: any;
    public lineChartLegend: boolean;
    public lineChartType: string;
    chartInstance: any;
    public Step: number = 30;
    public ItemList: any[] = [];
    selectedVitals: Set<string> = new Set();
    vitals: string[] = ['Fever', 'BP', 'SPO2', 'Heart Rate'];
    @ViewChild("PrintRx") PrintRxContent: TemplateRef<any>;
    @ViewChild("PrimaryModal") PrimaryModal: TemplateRef<any>;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService
        , public _AdmitService: AdmitService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, public _SaleService: SaleService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.VisitRights = this._CommonService.ScreenRights("39");
        let Users = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {

        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this.LoadPatient();
        this.Form1 = this._fb.group({
            AdmissionId: [''],
            Date: [''],
            Description: ['', [Validators.required]],
        });
        this.Form2 = this._fb.group({
            AdmissionId: [''],
            AppointmentId: [''],
            OnBehalfOf: [''],
            Note: [''],
        });
        this.Form3 = this._fb.group({
            AdmissionId: [''],
            DateRecorded: ['', [Validators.required]],
            Temperature: [''],
            Weight: [''],
            Height: [''],
            BP: [''],
            SPO2: [''],
            Measure: [''],
            Measure2: [''],
            HeartRate: [''],
            RespiratoryRate: [''],
            TimeRecorded: ['']
        });
        this.Form4 = this._fb.group({
            AdmissionId: [''],
            AppointmentId: [''],
            PatientId: [''],
            IntakeValue: [''],
            IntakeId: [''],
            Date: [''],
            Time: [''],
            Value: [''],
            Output: [''],
            OutputStatus: [''],
            OutputId: ['']
        });
        this.Form5 = this._fb.group({
            AdmissionId: [''],
            AppointmentId: [''],
            PatientId: [''],
            Date: [''],
            Time: [''],
            DrugId: [''],
            RouteId: [''],
            Dose: [''],
            MedicineName: ['']
        });
    }
    HomePage() {
        this.IsPatient = true;
        this._router.navigate(['home/AdmitSummary']);
        this.LoadPatient();
    }
    PatientTab() {
        if (this.Rights.indexOf(23) > -1) {
            this.IsPatient = false;
            this._router.navigate(['home/AdmitSummary/PatientInfo']);
        }
    }
    ShowAppointment() {
        if (this.Rights.indexOf(13) > -1) {
            this.IsPatient = false;

            this._router.navigate(['home/AdmitSummary/AdmitAppointSummery']);
        }
    }

    LoadLab() {
        if (this.Rights.indexOf(42) > -1) {
            this.IsPatient = false;

            this._router.navigate(['home/AdmitSummary/AdmitLabs']);
        }
    }
    LoadMedication() {
        if (this.Rights.indexOf(40) > -1) {
            this.IsPatient = false;

            this._router.navigate(['home/AdmitSummary/AdmitMedication']);
        }
    }
    LoadImaging() {
        if (this.Rights.indexOf(41) > -1) {
            this.IsPatient = false;

            this._router.navigate(['home/AdmitSummary/AdmitImaging']);
        }
    }
    LoadDocuments() {
        if (this.Rights.indexOf(25) > -1) {
            this.IsPatient = false;

            this._router.navigate(['home/AdmitSummary/AdmitDocSummery']);
        }
    }
    LoadDischargeReport() {
        //if (this.Rights.indexOf(45) > -1) {
        this.IsPatient = false;

        this._router.navigate(['home/AdmitSummary/DischargeReport']);
        //}
    }
    LoadVisit() {
        if (this.Rights.indexOf(39) > -1) {

            this.IsPatient = false;
            this._router.navigate(['home/AdmitSummary/AdmitDetail']);
        }
    }
    loadNote() {
        if (this.Rights.indexOf(44) > -1) {
            this.IsPatient = false;
            this._router.navigate(['home/AdmitSummary/AdmitNote']);
        }
    }
    loadCharge() {
        if (this.Rights.indexOf(47) > -1) {
            this.IsPatient = false;
            this._router.navigate(['home/AdmitSummary/AdmitCharge']);
        }
    }
    LoadPatient() {
        this.loader.ShowLoader();
        this._AppointmentService.AdmitPatientLoadById(this.PatientId, this.AdmitId).then(m => {
            this.IsPatient = true;
            this.model = m.ResultSet.patientInfo[0];
            this.PrimaryList = m.ResultSet.Diagnosislist.filter(a => a.IsType == "P");
            this.SecondaryList = m.ResultSet.Diagnosislist.filter(a => a.IsType == "S");
            this.AllergyList = m.ResultSet.Diagnosislist.filter(a => a.IsType == "A");
            this.model.AdmissionNo = m.ResultSet.AdmissionNo;
            this.model.DischargeDate = m.ResultSet.DischargeDate;
            this.NotesList = m.ResultSet.NotesList;
            this.InputOutputList = m.ResultSet.InputOutputList;
            this.MedicationLogList = m.ResultSet.MedicationLogList;
            this.DocumentList = m.ResultSet.DocumentList;
            this.DocumentList.forEach(x => {
                if (x.DocumentUpload != null && x.DocumentUpload != undefined && x.DocumentUpload != "")
                    x.DocumentImage = GlobalVariable.BASE_Temp_File_URL + '' + x.DocumentUpload;
            });
            this.initChart();
            this.selectedVitals.add('Fever');
            this.updateChartWithData(m.ResultSet.ChartData);

            this.prepareTableData(this.InputOutputList);
            this.MedicationTableData(this.MedicationLogList);
            if (this.model.Gender == 1)
                this.GenderName = 'Male';
            if (this.model.Gender == 2)
                this.GenderName = 'Female';
            if (this.model.Gender == 3)
                this.GenderName = 'Other';
            if (this.model.Image != null || this.model.Image != undefined) {
                this.getPatientImageUrlName(this.model.Image);
                this.IsNewPatientImage = false;
            } else this.IsNewPatientImage = true;
            this.emr_prescription_dynamicArray = [];
            this.emr_prescription_complaint_dynamicArray = [];
            this.emr_prescription_observation_dynamicArray = [];
            this.emr_prescription_investigation_dynamicArray = [];
            this.emr_prescription_diagnos_dynamicArray = [];
            this.emr_prescription_MedicineArray = [];
            this.emr_prescription_dynamicArray = m.ResultSet.prescriptionresult;
            this.loader.HideLoader();
        });
    }
    getPatientImageUrlName(FName) {
        this.model.Image = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.PatientImage = GlobalVariable.BASE_File_URL + '' + FName;
        } else {
            this.PatientImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }
    initChart() {
        this.chartOptions = {
            title: {
                text: '',
                left: 'center',
                textStyle: {
                    fontSize: 16
                }
            },
            tooltip: {
                trigger: 'axis'
            },
            legend: {
                data: ['Fever', 'BP', 'SPO2', 'Heart Rate'],
                selected: {
                    'Fever': true,       
                    'BP': false,         
                    'SPO2': false,       
                    'Heart Rate': false  
                }
            },
            xAxis: {
                type: 'time',
                name: 'Time',
                nameLocation: 'middle',
                nameGap: 20,
                axisLabel: {
                    formatter: (value: any) => {
                        const date = new Date(value);
                        return date.toLocaleTimeString('en-US', {
                            hour: '2-digit',
                            minute: '2-digit',
                            hour12: false
                        });
                    }
                }
            },
            yAxis: {
                type: 'value',
                name: 'Measurements',
                min: 0
            },
            series: []
        };
            }

    updateChartWithData(response: any) {
        const rawData = response[0]['ChartData'];
        const data = JSON.parse(rawData);
        const feverData = Array.isArray(data.fever) ? data.fever.map((item: any) => [item[0], item[1]]) : [];
        const bpData = Array.isArray(data.bp) ? data.bp.map((item: any) => [item[0], item[1]]) : [];
        const heartRateData = Array.isArray(data.heartRate) ? data.heartRate.map((item: any) => [item[0], item[1]]) : [];
        const spo2Data = Array.isArray(data.SPO2) ? data.SPO2.map((item: any) => [item[0], item[1]]) : [];
        this.chartOptions.series = [
            {
                name: 'Fever',
                type: 'line',
                smooth: true,
                data: feverData,
                lineStyle: { color: 'green' },
                itemStyle: { color: 'green' },
                areaStyle: { color: 'rgba(0, 255, 0, 0.3)' }
            },
            {
                name: 'BP',
                type: 'line',
                smooth: false,
                data: bpData,
                lineStyle: { color: 'blue' },
                itemStyle: { color: 'blue' },
                areaStyle: { color: 'rgba(0, 0, 255, 0.3)' }
            },
            {
                name: 'SPO2',
                type: 'line',
                smooth: false,
                data: spo2Data,
                lineStyle: { color: 'red' },
                itemStyle: { color: 'red' },
                areaStyle: { color: 'rgba(255, 0, 0, 0.3)' }
            },
            {
                name: 'Heart Rate',
                type: 'line',
                smooth: false,
                data: heartRateData,
                lineStyle: { color: 'purple' },
                itemStyle: { color: 'purple' },
                areaStyle: { color: 'rgba(128, 0, 128, 0.3)' }
            }
        ];
    }
    OpenPrimaryModal(PrimaryModal) {
        this.diagnosisModel = new ipd_diagnosis();
        this.diagnosisModel.Date = new Date();
        this.IsAllergy = false;
        this.diagnosisModel.IsType = "P";
        this.modalService.open(PrimaryModal, { size: 'md' });
    }
    OpenSecondaryModal(PrimaryModal) {
        this.diagnosisModel = new ipd_diagnosis();
        this.diagnosisModel.Date = new Date();
        this.diagnosisModel.IsType = "S";
        this.IsAllergy = false;
        this.modalService.open(PrimaryModal, { size: 'md' });
    }
    OpenAllergyModal(PrimaryModal) {
        this.diagnosisModel = new ipd_diagnosis();
        this.diagnosisModel.Date = new Date();
        this.diagnosisModel.IsType = "A";
        this.IsAllergy = true;
        this.modalService.open(PrimaryModal, { size: 'md' });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.diagnosisModel.PatientId = this.PatientId;
            this.diagnosisModel.AdmissionId = this.AdmitId;
            this._AdmitService.DiagnosisSaveOrUpdate(this.diagnosisModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.PrimaryList = [];
                    this.SecondaryList = [];
                    this.AllergyList = [];
                    this.PrimaryList = result.ResultSet.Diagnosislist.filter(a => a.IsType == "P");
                    this.SecondaryList = result.ResultSet.Diagnosislist.filter(a => a.IsType == "S");
                    this.AllergyList = result.ResultSet.Diagnosislist.filter(a => a.IsType == "A");
                    this.toastr.Success(result.Message);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
                this.modalService.dismissAll(this.PrimaryModal);
            });
        }
    }
    //Notes
    NoteSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.noteModel.AdmissionId = this.AdmitId;
            this.noteModel.PatientId = this.PatientId;
            this._AdmitService.NoteSaveOrUpdate(this.noteModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.LoadPatient();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    AddNoteModal(NoteModal) {
        this.loader.ShowLoader();
        this.noteModel = new ipd_admission_notes();
        this.modalService.open(NoteModal, { size: 'md' });
        this.loader.HideLoader();
    }
    //Vital 
    VitalSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.VitalModel.AdmissionId = this.AdmitId;
            this.VitalModel.PatientId = this.PatientId;
            this.VitalModel.BP = this.VitalModel.Measure; //+ "/" + this.VitalModel.Measure2;
            this._AdmitService.VitalSaveOrUpdate(this.VitalModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.LoadPatient();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    OpenVitalList(VitalList) {        
        this.loader.ShowLoader();
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this._AdmitService.GetVilitListByAdmitId(this.AdmitId, this.PatientId).then(m => {
            this.VitalArray = m.ResultSet.result;
            this.modalService.open(VitalList, { size: 'lg' });
            this.loader.HideLoader();
        });
    }
    OpenVitalModal(VitalModal) {
        this.VitalModel = new ipd_admission_vital();
        this.VitalModel.DateRecorded = new Date();
        this.SelectTime();
        this.modalService.open(VitalModal, { size: 'md' });
    }
    SelectTime() {
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        const timeString = this.formatTime(hours, minutes);
        this.VitalModel.TimeRecorded = timeString;
        $("#TimeRecorded").timepicker({
            'step': this.Step,
            'timeFormat': 'h:i A',
            'defaultTime': timeString
        });
    }
    formatTime(hours: number, minutes: number): string {
        const ampm = hours >= 12 ? 'PM' : 'AM';
        const hour = hours % 12 || 12;  // Convert to 12-hour format
        const minute = minutes < 10 ? '0' + minutes : minutes;
        return hour + ':' + minute + ' ' + ampm;
    }
    //Input
    OpenInputModal(InputModal) {
        this.loader.ShowLoader();
        this.InputModel = new ipd_input_output();
        this._AdmitService.GetIntakeDropDown().then(m => {
            if (m.IsSuccess) {
                this.IntakeList = m.ResultSet.DropdownList.filter(a => a.DropDownID == 61);
                this.OutputList = m.ResultSet.DropdownList.filter(a => a.DropDownID == 62);
                this.InputModel.Date = new Date();
                this.SelectInputTime('I');
                this.modalService.open(InputModal, { size: 'md' });
            }
            this.loader.HideLoader();
        });
    }
    SelectInputTime(type: any) {
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        const timeString = this.formatTime(hours, minutes);
        if (type == 'M')
            this.MedicationModel.Time = timeString;
        else
            this.InputModel.Time = timeString;
        $("#basicExample").timepicker({
            'step': this.Step,
            'timeFormat': 'h:i A',
            'defaultTime': timeString
        });
    }
    IntakeSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.InputModel.AdmissionId = this.AdmitId;
            this.InputModel.PatientId = this.PatientId;
            this.InputModel.Time = $("#basicExample").val();
            this._AdmitService.InputSaveOrUpdate(this.InputModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.LoadPatient();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    selectTab(tabName: string) {
        this.activeTab = tabName;
        if (this.activeTab == 'Intake')
            this.InputModel.Type = 'I';
        else
            this.InputModel.Type = 'O';
    }
    prepareTableData(rawData: any) {
        const medicineMap: { [key: string]: any } = {};
        // Extract unique time values and build the medicine map
        rawData.forEach(item => {
            if (!this.DynamicTimeHeaders.includes(item.Time)) {
                this.DynamicTimeHeaders.push(item.Time);
            }

            if (!medicineMap[item.Medicine]) {
                medicineMap[item.Medicine] = { Medicine: item.Medicine, Type: item.Type.trim(), values: {} };
            }
            medicineMap[item.Medicine].values[item.Time] = item.Value;
        });
        const medicineArray = Object.values(medicineMap);
        this.TableData = medicineArray.filter(a => a.Type === 'I');
        this.OutputTableData = medicineArray.filter(a => a.Type === 'O');
        this.DynamicTimeHeaders.sort(this.timeSorter);
    }
    timeSorter(a: string, b: string): number {
        const timeA = new Date(`1970/01/01 ${a}`).getTime();
        const timeB = new Date(`1970/01/01 ${b}`).getTime();
        return timeA - timeB;
    }
    //Medication 
    ViewDoc(id: any) {
        window.open(id, '_blank');
    }
    LoadItem() {
        //Search By Name
        $('#txtDrug').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._SaleService.searchByName(request.term).then(m => {
                    this.ItemList = m.ResultSet.ItemInfo;
                    const items = m.ResultSet.ItemInfo.map(item => ({
                        label: item.label + " (qty:" + item.stock + ")",
                        value: item.value,
                        CostPrice: item.CostPrice,
                        ItemTypeId: item.Type,
                        stock: item.stock,
                        CategoryId: item.CategoryId,
                        TypeValue: item.TypeValue,
                    }));
                    response(items);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.MedicationModel.MedicineName = ui.item.label;
                this.MedicationModel.DrugId = ui.item.value;
            }
        });
    }
    OpenMedicationModal(MedicationModal) {
        this.loader.ShowLoader();
        this.MedicationModel = new ipd_medication_log();
        this._AdmitService.GetMedicationDropDown().then(m => {
            if (m.IsSuccess) {
                this.RouteList = m.ResultSet.DropdownList.filter(a => a.DropDownID == 63);
                this.MedicationModel.Date = new Date();
                this.MedicationModel.Dose = '1';
                this.SelectInputTime('M');
                this.modalService.open(MedicationModal, { size: 'md' });
            }
            this.loader.HideLoader();
        });
    }
    MedicationSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            var finditem = this.ItemList.filter(a => a.value == this.MedicationModel.DrugId);
            if (this.MedicationModel.Dose > finditem[0].stock) {
                swal({
                    title: "Are you sure?",
                    text: "The entered quantity exceeds the available stock.",
                    icon: "warning",
                    buttons: ['Cancel', 'Yes'],
                    dangerMode: true,
                }).then((willDelete) => {
                    if (willDelete) {
                        this.saveData();
                    }
                });

            } else {
                this.saveData();
            }

        }
    }
    saveData() {
        this.submitted = false;
        this.loader.ShowLoader();
        this.MedicationModel.AdmissionId = this.AdmitId;
        this.MedicationModel.PatientId = this.PatientId;
        this.MedicationModel.Time = $("#basicExample").val();
        this._AdmitService.MedicatioLogSaveOrUpdate(this.MedicationModel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.modalService.dismissAll();
                this.LoadPatient();
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    MedicationTableData(rawData: any) {
        const medicineMap: { [key: string]: any } = {};
        rawData.forEach(item => {
            if (!this.MedicationDynamicTimeHeaders.includes(item.Time)) {
                this.MedicationDynamicTimeHeaders.push(item.Time);
            }
            if (!medicineMap[item.Medicine]) {
                medicineMap[item.Medicine] = { Medicine: item.Medicine, RouteVal: item.RouteVal, values: {} };
            }
            medicineMap[item.Medicine].values[item.Time] = item.Value;
        });
        // Convert medicineMap to array for ngFor
        this.MedicationData = Object.values(medicineMap);
        // Sort time headers if needed
        this.MedicationDynamicTimeHeaders.sort(this.timeSorter);
    }
}


