<script setup lang="ts">
import axios, { type AxiosRequestConfig } from 'axios'
import type { TotalUsersStatApiResponse } from '@/types/admin'
import { useAuthStore } from '@/stores/auth'
import { onMounted } from 'vue'

const apiUrl = import.meta.env.VITE_API_URL
const authStore = useAuthStore()

onMounted(async () => {
    const token = authStore.token
    const config: AxiosRequestConfig = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        timeout: 10000
    }

    try {
        const response = await axios.get<TotalUsersStatApiResponse>(`${apiUrl}/api/admin/home/get-total-users`, config)
        if (response.status === axios.HttpStatusCode.Ok) {

        } else if (response.status === axios.HttpStatusCode.Forbidden) {

        } else {

        }
    } catch (error) {
        if (axios.isAxiosError(error)) {
            console.error('Axios error:', error.response?.status)
        } else {
            console.error('Unexpected error:', error)
        }
    }
})


</script>

<template>
    <div class="card h-100">
        <div class="card-body d-flex">
            <div class="me-3" style="width: 4rem;">
                <img src="/image.png" alt="profile picture" width="50" class="w-100">
            </div>
            <div>
                <h6>Total Users <span class="d-none d-sm-inline fs-6 text-success"><i
                            class="bi bi-arrow-up-short"></i>12%</span></h6>
                <h3>
                    24,571
                </h3>
            </div>
        </div>
    </div>
</template>