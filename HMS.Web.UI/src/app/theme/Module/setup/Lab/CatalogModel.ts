export class lab_patient_test {
    ID: number = 0;
    CompanyId: number;
    PatientId: number;
    PatientName: string;
    PriorityId: number;
    StatusId: number;
    PriorityName: string;
    ClinicalNotes: string;
    CatalogIds: string;
    Age: number;
    Gender: string;
    SampleDate: Date;
    CompletedDate: Date;
    InProgressDate: Date;
    ReferringPhysician: string;
    DiscountAmount: number = 0;
    Discount: number;
    DiscountType: string;
    Total: number;
}
export class lab_result_mf {
    constructor() {
        let objlab_result_dt = new lab_result_dt();
        this.lab_result_dt.push(objlab_result_dt);
    }
    ID: number;
    CompanyID: number;
    PatientId: number;
    CatalogId: number;
    PatientTestId: number;
    Comment: string;   
    lab_result_dt: Array<lab_result_dt> = [];
}
export class lab_result_dt {
    ID: number;
    CompanyID: number;
    ResultId: number;
    ParameterId: number;
    Result: number;
    Status: string;
    Comment: string;
}
export class LabReport {
    Header: LabReportHeader;
    Patient: PatientInfo;
    Results: LabResult[];
}

export class LabReportHeader {
    reportNo: string;
    reportDate: Date;
    departmentName: string;
    reportTitle: string;
}

export class PatientInfo {
    mrno: string;
    patientName: string;
    age: string;
    gender: string;
    phone: string;
    address: string;

    orderedBy: string;
    referringPhysician: string;
    collectionCenter: string;

    requestedDate: Date;
    collectedDate: Date;
    receivedDate: Date;
    reportedDate: Date;
}

export class LabResult {
    testName: string;
    normalRange: string;
    unit: string;

    currentResult: string;
    previousResult1?: string;
    previousResult2?: string;

    isAbnormal: boolean;
}
