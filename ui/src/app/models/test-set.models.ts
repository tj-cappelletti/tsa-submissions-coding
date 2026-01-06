export interface TestSetModel {
    id: string;
    inputs: TestSetValueModel[];
    isPublic: boolean;
    name: string;
    problemId: string;
}

export interface TestSetValueModel {
    id: string;
    dataType: string;
    index: number;
    isArray: boolean;
    valueAsJson: string;
}
