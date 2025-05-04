export interface BaseHttpResponseModel {
    statusCode: number
    message: string | null
    errors: string[]
}

export interface HttpResponseModel<T> extends BaseHttpResponseModel {
    data: T | null
}

export interface SearchResultModel<T> {
    data: T[],
    pageNumber: number,
    pageSize: number,
    totalItems: number,
    totalPages: number
}