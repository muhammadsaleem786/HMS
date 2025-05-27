import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Router, NavigationStart, NavigationEnd } from '@angular/router';
import { Helpers } from "./helpers";
import { LoaderService } from './CommonService/LoaderService';
import { environment } from './../../src/environments/environment'
@Component({
    selector: 'body',
    templateUrl: './app.component.html',
    encapsulation: ViewEncapsulation.None,
})
export class AppComponent implements OnInit {
    title = 'app';
    globalBodyClass = '';
    name = environment.name;
    constructor(private _router: Router, public loader: LoaderService) {
        this.loader.ShowLoader()
    }
    ngOnInit() {
        this._router.events.subscribe((route) => {
            if (route instanceof NavigationStart) {
                this.loader.ShowLoader();
            }
            if (route instanceof NavigationEnd) {
                window.scroll(0, 0);
                this.loader.HideLoader();
            }
        });
    }
}