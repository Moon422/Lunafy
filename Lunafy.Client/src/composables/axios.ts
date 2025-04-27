import { ref, type Ref, inject } from 'vue'
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios'

export function useAxios() {
    // Inject the axiosClient instance provided at the app level
    const axiosClient = inject<AxiosInstance>('axiosClient')
    if (!axiosClient) {
        throw new Error('axiosClient not provided. Ensure it is provided using app.provide("axiosClient", axiosClient).')
    }

    // Instance-specific reactive states
    const loading: Ref<boolean> = ref(false)
    const error: Ref<string | null> = ref(null)
    const data: Ref<any> = ref(null)

    // Generic request wrapper
    async function request<T>(
        method: 'get' | 'post' | 'put' | 'patch' | 'delete',
        url: string,
        payload?: any,
        config?: AxiosRequestConfig
    ): Promise<AxiosResponse<T>> {
        loading.value = true
        error.value = null
        data.value = null

        if (!axiosClient) {
            throw new Error('axiosClient not provided. Ensure it is provided using app.provide("axiosClient", axiosClient).')
        }

        try {
            let response: AxiosResponse<T>
            switch (method) {
                case 'get':
                    response = await axiosClient.get<T>(url, config)
                    break
                case 'post':
                    response = await axiosClient.post<T>(url, payload, config)
                    break
                case 'put':
                    response = await axiosClient.put<T>(url, payload, config)
                    break
                case 'patch':
                    response = await axiosClient.patch<T>(url, payload, config)
                    break
                case 'delete':
                    response = await axiosClient.delete<T>(url, config)
                    break
                default:
                    throw new Error(`Unsupported method: ${method} `)
            }
            data.value = response.data
            return response
        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : String(err)
            error.value = errorMessage
            throw err
        } finally {
            loading.value = false
        }
    }

    // HTTP methods using the injected axiosClient
    const get = <T>(url: string, config?: AxiosRequestConfig) =>
        request<T>('get', url, undefined, config)
    const post = <T>(url: string, payload?: any, config?: AxiosRequestConfig) =>
        request<T>('post', url, payload, config)
    const put = <T>(url: string, payload?: any, config?: AxiosRequestConfig) =>
        request<T>('put', url, payload, config)
    const patch = <T>(url: string, payload?: any, config?: AxiosRequestConfig) =>
        request<T>('patch', url, payload, config)
    const deleteRequest = <T>(url: string, config?: AxiosRequestConfig) =>
        request<T>('delete', url, undefined, config)

    return {
        loading,
        error,
        data,
        get,
        post,
        put,
        patch,
        delete: deleteRequest,
    }
}
