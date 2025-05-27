import { Component, Input, OnInit, OnChanges, ViewChild, ComponentRef } from '@angular/core';
@Component({
    selector: 'Report-Viewer',
    templateUrl: './ViewerComponent.html',
})

export class ViewerComponent implements OnChanges {

    @Input() PageContent: ComponentRef<any>;
    @ViewChild('container') divContainer: any;
    constructor() {
    }
    ngOnChanges() {
        if (this.PageContent != undefined) {
            this.divContainer.nativeElement.innerHTML = "";
            this.divContainer.nativeElement.append(this.PageContent.location.nativeElement);

        }
    }

}