import { Component, Input, ChangeDetectorRef, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
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
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
@Component({
    selector: 'CurrentStockRptForm',
    templateUrl: 'CurrentStockRptForm.html',
    moduleId: module.id,
    providers: [ReportService],
})
export class CurrentStockRptForm implements OnInit {
    @ViewChild('TABLE') TABLE: ElementRef;
    public model = new ReportModel();
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public DataList: any[];
    public CompanyName: string; public CompanyObj: any;
    public Form1: FormGroup; public totalCostValue = 0; 
    public totalSaleValue = 0; 
    constructor(public _fb: FormBuilder,public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, private cdr: ChangeDetectorRef, public reportservice: ReportService, private encrypt: EncryptionService) {
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
            ProcedureId:['']
        });
    }
    LoadSearch() {
        $("#SearchByName").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this.reportservice.StockGroupSearch(request.term).then(m => {
                    response(m.ResultSet.categoryList);
                    this.loader.HideLoader();
                });
            },
            minLength: 2,
            select: (event, ui) => {
                this.model.ProcedureId = ui.item.value;
                this.model.ProcedureName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    LoadItem() {
        $("#ItemSearchByName").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this.reportservice.ItemSearch(request.term).then(m => {
                    response(m.ResultSet.ItemInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.ItemId = ui.item.value;
                this.model.ItemName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    RunReport() {
        this.loader.ShowLoader();
        if (this.model.ProcedureName == null || this.model.ProcedureName == "")
            this.model.ProcedureId = null;
        if (this.model.ItemName == null || this.model.ItemName == "")
            this.model.ItemId = null;
        this.reportservice.CurrentStockRpt(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.totalCostValue = 0;
                this.totalSaleValue = 0;
                const feeList = result.ResultSet.FeeList;
                this.DataList = Array.isArray(feeList)[0] ? feeList : Object.values(feeList)[0];
                this.totalCostValue = this.DataList.reduce((sum, item) => sum + item.CostValue, 0);
                this.totalSaleValue = this.DataList.reduce((sum, item) => sum + item.RetailValue, 0);

                this.cdr.detectChanges();
                //this.DataList.forEach(x => {
                //    this.total += x.PaidAmount
                //}); 

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
            pdf.save('CurrentStockRpt.pdf');
            this.loader.HideLoader();
        });
    }
    public DownloadExcel() {
        var data_type = 'data:application/vnd.ms-excel';
        var table_div = document.getElementById('RptPDF');
        var table_html = table_div.outerHTML.replace(/ /g, '%20');

        var a = document.createElement('a');
        a.href = data_type + ', ' + table_html;
        a.download = 'CurrentStockRpt.xls';
        a.click();
    }
}
