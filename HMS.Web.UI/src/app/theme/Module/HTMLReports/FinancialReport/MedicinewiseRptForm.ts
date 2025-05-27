import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ReportService } from '../../../Module/Reports/ReportService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { IMyDateModel } from 'mydatepicker';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { ReportModel } from '../../Reports/ReportModel';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
declare var $: any;
@Component({
    selector: 'MedicinewiseRptForm',
    templateUrl: 'MedicinewiseRptForm.html',
    moduleId: module.id,
    providers: [ReportService],
})
export class MedicinewiseRptForm implements OnInit {
    @ViewChild('TABLE') TABLE: ElementRef;
    public model = new ReportModel();
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public DataList: any[];
    public CompanyName: string;
    public Form1: FormGroup; public CompanyObj: any;
    constructor(public _fb: FormBuilder,public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public reportservice: ReportService, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
      this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.CompanyName = this.CompanyObj.CompanyName;
        this.PayrollRegion = this._CommonService.getPayrollRegion();
    }
    ngOnInit() {
        var date = new Date();
        var firstDay = new Date(Date.UTC(date.getFullYear(), date.getMonth(), 1));
        var lastDay = new Date(Date.UTC(date.getFullYear(), date.getMonth() + 1, 0));
        this.model.FromDate = firstDay;
        this.model.ToDate = lastDay;
        this.Form1 = this._fb.group({
            ID: [''],
            FromDate: [''],
            ToDate: [''],
            PatientId: [''],
            DoctorId: [''],
        });
    }
    
    LoadDoctor() {
        $("#DoctorSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this.reportservice.DoctorSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.DoctorId = ui.item.value;
                this.model.DoctorName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    LoadPatient() {
        $('#PatientSearchByName').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this.reportservice.searchByName(request.term).then(m => {
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.PatientName = ui.item.label;
                this.model.PatientId = ui.item.value;
            }
        });
    }
    RunReport() {
        this.loader.ShowLoader();
        this.reportservice.MedicinewiseRpt(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.DataList = result.ResultSet.MedicineList;
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    cancelReport() {
     
    }
}
