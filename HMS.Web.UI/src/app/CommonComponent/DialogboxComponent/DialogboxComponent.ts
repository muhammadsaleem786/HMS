import { Component, OnInit, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { ViewerComponent } from '../ReportViewer/ViewerComponent';

@Component({
    selector: 'Dialogbox-Component',
    moduleId: module.id,
    templateUrl: 'DialogboxComponent.html',
    styleUrls: ['DialogboxComponent.css'],
    entryComponents: [ViewerComponent],
    animations: [
        trigger('dialog', [
            transition('void => *', [
                style({ transform: 'scale3d(.3, .3, .3)' }),
                animate(100)
            ]),
            transition('* => void', [
                animate(100, style({ transform: 'scale3d(.0, .0, .0)' }))
            ])
        ])
    ]
})
export class DialogboxComponent implements OnInit {
    @Input() closable = true;
    @Input() visible: boolean;
    @Output() visibleChange: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Input() isoverridemt = false;

    constructor() { }

    ngOnInit() { }

    close() {
        this.visible = false;
        this.visibleChange.emit(this.visible);
    }
}