import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ReportService } from '../../../Module/Reports/ReportService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { IMyDateModel } from 'mydatepicker';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { ReportModel } from '../../Reports/ReportModel';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { forEach } from '@angular/router/src/utils/collection';
declare var $: any;
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import * as FileSaver from 'file-saver';
import { EncryptionService } from '../../../../CommonService/encryption.service';
@Component({
    selector: 'ClinicwiseComponentRptForm',
    templateUrl: 'ClinicwiseComponentRptForm.html',
    moduleId: module.id,
    providers: [ReportService],
})
export class ClinicwiseComponentRptForm implements OnInit {
    @ViewChild('TABLE') TABLE: ElementRef;
    public model = new ReportModel();
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public DataList: any[];
    public CompanyName: string;
    public Form1: FormGroup; public total = 0;
    public CompanyObj: any;
    @ViewChild("EmailModal") EmailModal: TemplateRef<any>;
    constructor(public _fb: FormBuilder, public toastr: CommonToastrService, private modalService: NgbModal, public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public reportservice: ReportService, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.CompanyName = this.CompanyObj.CompanyName;
        //this.Keywords = this._CommonService.GetKeywords("");
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
    LoadClinic() {
        $('#SearchByClinic').autocomplete({
            source: (request, response) => {
                this.model.PatientId = null;
                this.loader.ShowLoader();
                this.reportservice.searchByClinic(request.term).then(m => {
                    response(m.ResultSet.companyInfo);
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
        this.reportservice.ClinicwiseRpt(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.total = 0;
                this.DataList = result.ResultSet.Paymentlist;
                this.DataList.forEach(x => {
                    this.total += x.PaidAmount
                });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    public DownloadPDF(): void {
        this.loader.ShowLoader();
        var data = document.getElementById('RptPDF');
        html2canvas(data).then(canvas => {
            var imgWidth = 208;
            var pageHeight = 295;
            var imgHeight = canvas.height * imgWidth / canvas.width;
            var heightLeft = imgHeight;
            const contentDataURL = canvas.toDataURL('image/png')
           let pdf = new jsPDF('p', 'mm', 'a4');
            var position = 0;
            pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight)
            pdf.save('ClinicWise.pdf');
            this.loader.HideLoader();
        });
    }
    public DownloadExcel() {
        var data_type = 'data:application/vnd.ms-excel';
        var table_div = document.getElementById('RptPDF');
        var table_html = table_div.outerHTML.replace(/ /g, '%20');
        var a = document.createElement('a');
        a.href = data_type + ', ' + table_html;
        a.download = 'ClinicWise.xls';
        a.click();
    }
    SendEmail() {
        if (this.model.Email != null && this.model.Email != "") {
            this.modalService.dismissAll(this.EmailModal);
            var data = document.getElementById('RptPDF');
            html2canvas(data).then(canvas => {
                const contentDataURL = canvas.toDataURL('image/png');
                const base64String = contentDataURL.toString().replace("data:", "").replace(/^.+,/, "");
                this.model.base64 = base64String;
                this.model.FileName = "ClinicWise.png"
                this.reportservice.SendEmail(this.model).then(m => {
                    var result = JSON.parse(m._body);
                    if (result.IsSuccess) {
                        this.toastr.Success(result.Message);
                    }
                    else
                        this.toastr.Error('Error', result.ErrorMessage);

                    this.loader.HideLoader();
                });
            });
        } else {
            this.modalService.open(this.EmailModal, { size: 'sm' });
        }
    }

}
