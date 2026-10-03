import { Component, OnInit, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { CommonService } from '../../../CommonService/CommonService';
@Component({
    selector: 'app-header-user',
    templateUrl: "./header-user.component.html",
    encapsulation: ViewEncapsulation.None,
})
export class HeaderUserComponent implements OnInit {
    public PayrollRegion: string;
    constructor(public commonservice: CommonService) {
        this.commonservice.IsPayrollRegion();
        this.PayrollRegion = "PK";
    }
    ngOnInit() {
    }
}
