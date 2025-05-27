export class PageListModel {
    CurrentPage: number;
    RecordPerPage: number;
    TotalRecord: number;
    SearchText: string;
    VisibleColumnInfo: string;
    VisibleFilter: boolean;

    constructor() {
        this.CurrentPage = 1;
        this.RecordPerPage = 10;
        this.SearchText = "";
        this.VisibleColumnInfo = "";
        this.VisibleFilter = false;
    }
}
export class PageListConfig {
    PrimaryColumn: string;
    Fields: DataField[] = [];
}

export class DataField {
    Name: string;
    Title: string;
    Visible: boolean;
}