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
                    this.total += x.TotalIncome
                });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
  public DownloadPDF(): void {
    this.loader.ShowLoader();
    const data = document.getElementById('RptPDF');

    html2canvas(data, {
        scale: 3,
        useCORS: true,
        scrollY: -window.scrollY
    }).then(canvas => {
        const imgData = canvas.toDataURL('image/png');

        const pdf = new jsPDF('p', 'mm', 'a4'); // Portrait mode
        const pageWidth = 210; // A4 width in mm
        const pageHeight = 297; // A4 height in mm
        const margin = 10;

        const imgWidth = pageWidth - margin * 2;
        const imgHeight = (canvas.height * imgWidth) / canvas.width;

        if (imgHeight < pageHeight - margin * 2) {
            // Fits in one page
            pdf.addImage(imgData, 'PNG', margin, margin, imgWidth, imgHeight);
        } else {
            // Multi-page logic
            const pageCanvasHeight = canvas.width * (pageHeight - 2 * margin) / imgWidth;
            const totalPages = Math.ceil(canvas.height / pageCanvasHeight);

            for (let i = 0; i < totalPages; i++) {
                const pageCanvas = document.createElement('canvas');
                pageCanvas.width = canvas.width;
                pageCanvas.height = pageCanvasHeight;

                const context = pageCanvas.getContext('2d');
                context!.drawImage(
                    canvas,
                    0,
                    i * pageCanvasHeight,
                    canvas.width,
                    pageCanvasHeight,
                    0,
                    0,
                    canvas.width,
                    pageCanvasHeight
                );

                const pageData = pageCanvas.toDataURL('image/png');
                if (i > 0) pdf.addPage();
                pdf.addImage(pageData, 'PNG', margin, margin, imgWidth, pageHeight - 2 * margin);
            }
        }

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
