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
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
declare var $: any;
@Component({
    selector: 'ProfitLossRptForm',
    templateUrl: 'ProfitLossRptForm.html',
    moduleId: module.id,
    providers: [ReportService],
})
export class ProfitLossRptForm implements OnInit {
    @ViewChild('TABLE') TABLE: ElementRef;
    public model = new ReportModel();
    public Rights: any;
    public Keywords: any[] = [];
    public PayrollRegion: string;
    public DataList: any[];
    public IncomeList: any[];
    public ExpenseList: any[];
    public CompanyName: string; public CompanyObj: any;
    public Form1: FormGroup; public total = 0; public totalIncome = 0; public totalExpen = 0;
    constructor(public _fb: FormBuilder, public _CommonService: CommonService, public loader: LoaderService
        , public _router: Router, public reportservice: ReportService, private encrypt: EncryptionService) {
        this.loader.ShowLoader();
      this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.CompanyName = this.CompanyObj.CompanyName;
        this.PayrollRegion = this._CommonService.getPayrollRegion();
    }
    ngOnInit() {
        this.model.Type = 0;
        var date = new Date();
        var firstDay = new Date(Date.UTC(date.getFullYear(), date.getMonth(), 1));
        var lastDay = new Date(Date.UTC(date.getFullYear(), date.getMonth() + 1, 0));
        this.model.FromDate = firstDay;
        this.model.ToDate = lastDay;
        this.RunReport();
        this.Form1 = this._fb.group({
            ID: [''],
            FromDate: [''],
            ToDate: [''],
            PatientId: [''],
            DoctorId: [''],
            Type: [''],
        });
    }
   RunReport() {
        this.loader.ShowLoader();
       this.reportservice.ProfitLossRpt(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.total = 0;
                this.totalIncome = 0;
                this.totalExpen = 0;
                this.IncomeList=[];
                this.ExpenseList =[];
                this.DataList = result.ResultSet.datalist.Table;
                this.DataList.forEach(x => {
                    if (x.Category == "Expense")
                        this.totalExpen += x.TotalCost
                    if (x.Category == "Income")
                        this.totalIncome += x.TotalCost
                });
                this.total = this.totalIncome - this.totalExpen;
                this.IncomeList = this.DataList.filter(a => a.Category === 'Income');
                this.ExpenseList = this.DataList.filter(a => a.Category === 'Expense');
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
            pdf.save('ProfitLoss.pdf');
            this.loader.HideLoader();
        });
    }
    public DownloadExcel() {
        var data_type = 'data:application/vnd.ms-excel';
        var table_div = document.getElementById('RptPDF');
        var table_html = table_div.outerHTML.replace(/ /g, '%20');
        var a = document.createElement('a');
        a.href = data_type + ', ' + table_html;
        a.download = 'ProfitLoss.xls';
        a.click();
    }
}
