import {
  Component,
  Input,
  Output,
  EventEmitter,
  forwardRef,
  ViewChild,
  ElementRef
} from '@angular/core';

import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { FileService } from '../../CommonService/FileService';
import { CommonToastrService } from '../../CommonService/CommonToastrService';

const noop = () => { };

export const CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR: any = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => FileUploadComponent),
  multi: true,
};

@Component({
  selector: 'File-Component',
  templateUrl: './FileUploadComponent.html',
  providers: [CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR, FileService]
})
export class FileUploadComponent implements ControlValueAccessor {

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  @Input() ImageWithAddress: boolean = false;

  @Output() ClearModel = new EventEmitter<void>();
  @Output() FileNameEvent = new EventEmitter<string>();
  @Output() IsNewFileEvent = new EventEmitter<boolean>();

  private innerValue: any = '';
  private onChangeCallback: (_: any) => void = noop;
  private onTouchedCallback: () => void = noop;

  selectedFiles: File[] = [];

  showbar: boolean = false;
  imagedata: string = '';
  percentage: number = 0;
  max: number = 100;
  size: number = 0;

  constructor(
    private http: FileService,
    public toastr: CommonToastrService
  ) { }

  // ---------------- VALUE ACCESSOR ----------------

  get value(): any {
    return this.innerValue;
  }

  set value(v: any) {
    if (v !== this.innerValue) {
      this.innerValue = v;
      this.onChangeCallback(v);
    }
  }

    writeValue(value: any): void {
        this.innerValue = value;

        if (value) {
            this.showbar = true;

            if (typeof value === 'string') {
                this.imagedata = value;
            } else {
                this.imagedata =
                    value.FileName ||
                    value.fileName ||
                    value.Name ||
                    'Existing File';
            }
        } else {
            this.resetUI();
        }
    }

  registerOnChange(fn: any): void {
    this.onChangeCallback = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouchedCallback = fn;
  }

  onBlur(): void {
    this.onTouchedCallback();
  }
    removeFile(event: Event): void {
        event.stopPropagation();

        this.selectedFiles = [];
        this.value = '';

        this.resetUI();

        this.IsNewFileEvent.emit(false);
        this.ClearModel.emit();
    }
  // ---------------- FILE CHANGE ----------------

fileChangeEvent(event: any): void {

  const input = event.target as HTMLInputElement;

  const fileList = input.files;
  if (!fileList || fileList.length === 0) return;

  // ✅ CRITICAL FIX: clone file reference immediately
  const file: File = new File([fileList[0]], fileList[0].name, {
    type: fileList[0].type
  });

  const allowedExtensions = /(\.jpg|\.png|\.pdf)$/i;

  if (!allowedExtensions.exec(file.name)) {
    this.toastr.Error("Invalid file type. Only JPG, PNG, PDF allowed.");
    return;
  }

  this.IsNewFileEvent.emit(true);

  this.size = Math.round((file.size / 1024 / 1024) * 100) / 100;

  if (this.size > 10) {
    this.toastr.Error("File size must be less than 10MB");
    this.resetUI();
    return;
  }

  this.showbar = true;
  this.imagedata = 'File size is ' + this.size + ' MB';

  // ✅ IMPORTANT: store safe copy
  this.selectedFiles = [file];

  // ❌ DO NOT clear input here
  // ❌ DO NOT touch fileInput yet

  this.startUpload();
}
private startUpload(): void {

  setTimeout(() => {

    this.http.Upload(this, this.selectedFiles, this.progressCallback)
      .then(res => {

        const result = JSON.parse(JSON.stringify(res));

        if (result.IsSuccess && result.ResultSet && result.ResultSet.length > 0) {

          const uploadedFile = result.ResultSet[0];

          this.value = uploadedFile;

          if (this.ImageWithAddress) {
            this.FileNameEvent.emit(uploadedFile);
          }
        }

        // ✅ ONLY clear AFTER upload completes
        this.clearInput();
      });

  }, 0);
}
  // ---------------- PROGRESS ----------------

  progressCallback(self: any, value: number): void {
    self.percentage = value;
  }

  // ---------------- OPEN FILE ----------------

  triggerFileBrowse(event?: Event): void {

  if (event) {
    event.stopPropagation();
  }

  setTimeout(() => {
    if (this.fileInput) {
      this.fileInput.nativeElement.click();
    }
  }, 0);
}

  // ---------------- CLEAR ----------------

  clearValue(): void {
    this.resetUI();
    this.value = '';
    this.ClearModel.emit();
  }

  // ---------------- RESET ----------------

  private resetUI(): void {
    this.showbar = false;
    this.imagedata = '';
    this.percentage = 0;
    this.size = 0;
    this.clearInput();
  }

  private clearInput(): void {
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }
}