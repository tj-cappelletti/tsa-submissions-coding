import { TestSetModel } from "./test-set.models";

export interface Problem {
    description: string;
    id?: string | null | undefined;
    isActive?: boolean | null | undefined;
    title: string;
    testSets?: TestSetModel[] | null | undefined;
}
