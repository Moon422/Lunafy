import { type Router } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import axios, { AxiosError, type AxiosInstance, type AxiosRequestConfig, type AxiosResponse, type InternalAxiosRequestConfig } from 'axios'
import type { HttpResponseModel } from '@/types/common'
import type { LoginResponseModel } from '@/types/user'

export function createAxiosClient(router: Router): AxiosInstance {
    const axiosClient: AxiosInstance = axios.create({
        baseURL: import.meta.env.VITE_API_URL,
        timeout: 10000,
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json',
        },
    })

    let isRefreshing = false
    let refreshSubscribers: Array<(token: string | null) => void> = []

    function subscribeTokenRefresh(cb: (token: string | null) => void): void {
        refreshSubscribers.push(cb)
    }

    function onRefreshed(token: string): void {
        refreshSubscribers.forEach(cb => cb(token))
        refreshSubscribers = []
    }

    function onRefreshFailed(): void {
        refreshSubscribers.forEach(cb => cb(null))
        refreshSubscribers = []
    }

    function waitForTokenRefresh(): Promise<string> {
        return new Promise((resolve, reject) => {
            const timeout = 10000

            // const timeoutId = setTimeout(() => {
            //     reject(new Error('Refresh token timeout'))
            // }, timeout)

            subscribeTokenRefresh((newToken) => {
                clearTimeout(timeoutId)
                if (newToken) {
                    resolve(newToken)
                } else {
                    reject(new Error('Refresh token failed'))
                }
            })
        })
    }

    axiosClient.interceptors.request.use(
        (config: InternalAxiosRequestConfig): InternalAxiosRequestConfig => {
            const authStore = useAuthStore()
            if (authStore.token && config.headers) {
                config.headers = config.headers || {}
                config.headers['Authorization'] = `Bearer ${authStore.token}`
            }
            return config
        },
        (error: AxiosError): Promise<AxiosError> => {
            return Promise.reject(error)
        }
    )

    axiosClient.interceptors.response.use(
        (response: AxiosResponse) => response,
        async (error: AxiosError): Promise<any> => {
            console.log("What the fuck happened?")
            console.log('error:', error)

            const authStore = useAuthStore()
            const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean }

            if (error.response?.status === 401 && !originalRequest._retry) {
                originalRequest._retry = true

                if (!isRefreshing) {
                    isRefreshing = true
                    try {
                        const response = await axios.get<HttpResponseModel<LoginResponseModel>>(
                            `${import.meta.env.VITE_API_URL}/api/user/refresh-token`,
                            { withCredentials: true }
                        )

                        if (response.status !== axios.HttpStatusCode.Ok) {
                            throw new Error(response.data.errors.length ? response.data.errors[0] : 'Request failed.')
                        }

                        const { data } = response.data
                        if (!data) {
                            throw new Error('Invalid response.')
                        }

                        const { user, jwt } = data
                        const { firstname, lastname, email, isAdmin, isArtist } = user

                        authStore.setState({ token: jwt, firstname, lastname, email, isAdmin, isArtist })
                        axiosClient.defaults.headers.common['Authorization'] = `Bearer ${jwt}`
                        onRefreshed(jwt)
                    } catch (refreshError) {
                        console.error('Refresh token request failed:', refreshError)
                        onRefreshFailed()
                        authStore.clearState()

                        router.push('/login')
                        return Promise.reject(refreshError)
                    } finally {
                        isRefreshing = false
                    }
                }

                try {
                    const newToken = await waitForTokenRefresh()
                    if (originalRequest.headers) {
                        originalRequest.headers['Authorization'] = `Bearer ${newToken}`
                    }
                    return axiosClient(originalRequest)
                } catch (waitError) {
                    console.error('Wait for refresh failed:', waitError)
                    return Promise.reject(waitError)
                }
            }

            return Promise.reject(error)
        }
    )

    return axiosClient
}