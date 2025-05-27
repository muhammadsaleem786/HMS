import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_patient, ipd_admission_medication } from './AdmitModel';
import { Observable } from 'rxjs';
import { AdmitService } from './AdmitService';
import { AppointmentService } from './../Appointment/AppointmentService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { SaleService } from './../Items/Sale/SaleService';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
declare var $: any;
@Component({
    templateUrl: './AdmitMedicationList.html',
    moduleId: module.id,
    providers: [AdmitService, AppointmentService, SaleService],
})

export class AdmitMedicationList implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new ipd_admission_medication();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public PatientId: any;
    public sub: any;
    public IsEdit: boolean = false; public IsVital: boolean = false;
    public CompanyInfo: any[] = [];
    public ClinicName: string;
    public Rights: any;
    public ControlRights: any;
    public MedicitionList: any[] = [];
    public PastMedicitionList: any[] = [];
    pagesRange: number[] = []; previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    public AdmitId: any; public CompanyObj: any;
    @ViewChild("Medication") Medication: TemplateRef<any>;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router,
        public _AdmitService: AdmitService, public _SaleService: SaleService, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("40");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {

        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem("PatientId");
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
        this.Form1 = this._fb.group({
            AdmissionId: [''],
            AppointmentId: [''],
            ItemId: [''],
            MedicineName: [''],
            Prescription: [''],
            PrescriptionDate: [''],
            QuantityRequested: [''],
            Refills: [''],
            IsRequestNow: [''],
            BillTo: [''],
            IsActive: ['']
        });

    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.GetMedicationList();
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
    GetMedicationList() {
        this.MedicationGridList();
    }
    MedicationGridList() {
        this.PModel.VisibleColumnInfo = "Date#Date,Name#Name,Prescription#Prescription,Requested#Requested";
        this.loader.ShowLoader();
        this.Id = "0";
        this._AdmitService.GetMedicationList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.AdmitId, this.PatientId).then(m => {
            this.PModel.TotalRecord = m.TotalRecord;
            this.MedicitionList = m.DataList.filter(a => a.IsActive);
            this.PastMedicitionList = m.DataList.filter(a => !a.IsActive);
            this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
            this.loader.HideLoader();
        });
    }
    OpenMedication(Medication) {
        this.loader.ShowLoader();
        this.model = new ipd_admission_medication();
        let PatientId = localStorage.getItem('PatientId');
        this.model.PrescriptionDate = new Date();
        this.modalService.open(Medication, { size: 'md' });
        this.loader.HideLoader();
        //this._AdmitService.GetAppointmentList(PatientId).then(m => {
        //    if (m.IsSuccess) {
        //        this.modalService.open(Medication, { size: 'md' });
        //    }
        //    this.loader.HideLoader();
        //});
    }
    MedicationSaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.model.AdmissionId = this.AdmitId;
            this.model.PatientId = this.PatientId;
            this._AdmitService.MedicationSaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.MedicationGridList();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    Delete(id: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AdmitService.DeleteMedication(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.MedicationGridList();
                this.loader.HideLoader();
            });
        }
    }
    Edit(id: any) {
        this.loader.ShowLoader();
        let PatientId = localStorage.getItem('PatientId');
        this._AdmitService.MedicationGetById(id, PatientId).then(m => {
            if (m.ResultSet != null) {
                this.IsEdit = true;
                this.model = m.ResultSet.Result;
                this.model.MedicineName = m.ResultSet.Result.adm_item.Name;
                this.model.ItemId = m.ResultSet.Result.adm_item.ID;
                this.modalService.open(this.Medication, { size: 'md' });
            }
            this.loader.HideLoader();
        });
    }
    LoadMedicine() {
        //Search By Name
        $('#txtMedicine').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._SaleService.searchByName(request.term).then(m => {
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
                this.model.MedicineName = ui.item.label;
                this.model.ItemId = ui.item.value;
                this.model.QuantityRequested = 1;
            }
        });
    }
}
