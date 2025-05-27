import { Component, OnInit } from '@angular/core';
import { LoaderService } from '../../CommonService/LoaderService';
import { CommonService } from '../../CommonService/CommonService';
@Component({
    selector: 'app-account',
    templateUrl: './account.component.html',
    styleUrls: ['./account.component.css'],
})
export class AccountComponent implements OnInit {
    public PayrollRegion: string;
    constructor(public Loader: LoaderService, public commonservice: CommonService) {
        //this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.PayrollRegion = this.commonservice.getPayrollRegion();
    }

    ngOnInit() {

    }

}
