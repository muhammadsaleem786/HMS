import { Component, Input, forwardRef, Output, EventEmitter } from '@angular/core';
import { Response } from '@angular/http';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { FileService } from '../../CommonService/FileService';
import { CommonToastrService } from '../../CommonService/CommonToastrService';
import { ViewChild } from '@angular/core';
const noop = () => {
};

export const CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR: any = {
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => FileUploadComponent),
    multi: true,
};
@Component({
    selector: 'File-Component',
    moduleId: module.id,
    templateUrl: 'FileUploadComponent.html',
    providers: [CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR, FileService]
})
export class FileUploadComponent implements ControlValueAccessor {

    //The internal data model
    private innerValue: any = '';
    private selectedFiles: File[] = [];
    @Input() ImageWithAddress = false;
    @Output() ClearModel = new EventEmitter<string>();
    @Output() FileNameEvent = new EventEmitter<string>();
    @Output() IsNewFileEvent = new EventEmitter<boolean>();
    myInputVariable: any;


    public max: number = 0;
    public count: number = 0;
    public percentage: number = 0;
    public imagedata: string = '';
    public showbar: boolean = false;
    public size: number;


    //Placeholders for the callbacks which are later providesd
    //by the Control Value Accessor
    private onTouchedCallback: () => void = noop;
    private onChangeCallback: (_: any) => void = noop;

    constructor(private http: FileService, public toastr: CommonToastrService,) {

    }

    //get accessor
    get value(): any {
        return this.innerValue;
    };

    //set accessor including call the onchange callback
    set value(v: any) {
        if (v !== this.innerValue) {
            this.innerValue = v;
            this.onChangeCallback(v);
        }
    }

    //Set touched on blur
    onBlur() {
        this.onTouchedCallback();
    }
    clearValue() {
        $('#myFile').val('');
        this.imagedata = '';
        this.size = 0;
        this.showbar = false;
        this.value = '';
        this.ClearModel.emit();
    }
    ClearValue(data: any) {
        $("#myFile").val("");
        this.innerValue = '';
        this.imagedata = '';
        this.size = null;
        this.showbar = false;
        this.value = '';
    }

    //From ControlValueAccessor interface
    writeValue(value: any) {
        if (value !== this.innerValue) {
            if (!value) {
                this.innerValue = value;
                this.imagedata = '';
                this.size = null;
                this.showbar = false;
                this.value = '';
            }
        }

        if (value == "") {
            this.imagedata = '';
            this.size = null;
            this.showbar = false;
            this.value = '';

        }

        if (value == null) {
            this.imagedata = '';
            this.size = null;
            this.showbar = false;
            this.value = '';
        }

    }

    //From ControlValueAccessor interface
    registerOnChange(fn: any) {
        this.onChangeCallback = fn;
    }

    //From ControlValueAccessor interface
    registerOnTouched(fn: any) {
        this.onTouchedCallback = fn;
    }

    fileChangeEvent(data: any): void {
        const file = data.target.files[0];
        if (file) {
            const allowedExtensions = /(\.jpg|\.png|\.pdf)$/i;
            if (!allowedExtensions.exec(file.name)) {
                this.toastr.Error("Invalid file type. Only PDF, JPG, and PNG files are allowed.");
                return;
            }
        }

        this.IsNewFileEvent.emit(true);
        var self = this;
        self.size = data.target.files[0].size;
        self.size = self.size / 1024;
        self.size = self.size / 1024;
        self.size = Math.round(self.size * 100) / 100
        if (self.size > 10) {
            self.imagedata = 'File size is greater than 10 MB. File size is ' + self.size + ' MB';
            this.toastr.Error("Error", "File size is greater than 10 MB")
            self.value = '';
            return;
        }
        else {
            self.showbar = true;
            self.imagedata = 'File size is ' + self.size + ' MB';
            self.selectedFiles = data.target.files;
            if (self.selectedFiles.length == 0) return;
            self.max = 100;
            self.percentage = 0;
            self.http.Upload(self, self.selectedFiles, self.tempFunction).then(res => {
                var result = JSON.parse(JSON.stringify(res));
                if (result.IsSuccess == true) {
                    if (result.ResultSet.length = 1)
                        if (self.ImageWithAddress == true) {
                            self.value = result.ResultSet[0];

                            this.FileNameEvent.emit(result.ResultSet[0]);
                        }
                        else {
                            self.value = result.ResultSet[0]
                        }
                    else {
                        self.value = result.ResultSet
                    }
                }
            },
                error => error
            );
        }

        self.value = '';
        self.innerValue = '';
    }
    tempFunction(self: any, value: number) {
        self.percentage = value
    }

}
