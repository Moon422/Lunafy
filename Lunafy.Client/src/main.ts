import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import Vue3Toastify, { type ToastContainerOptions } from 'vue3-toastify'
import { createAxiosClient } from './api/axiosClient'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(Vue3Toastify, {
    autoclose: 3000,
    multiple: true,
    limit: 3,
    position: 'bottom-right',
    closeButton: true
} as ToastContainerOptions)

const axiosClient = createAxiosClient(router)
app.provide('axiosClient', axiosClient)

app.mount('#app')
