<script setup lang="ts">
import axios from 'axios'
import type { TotalUsersStatApiResponse } from '@/types/admin'
import { useAuthStore } from '@/stores/auth'
import { onMounted } from 'vue'
import { toast } from 'vue3-toastify'
import type { HttpResponseModel } from '@/types/common'
import { useAxios } from '@/composables/axios'

const authStore = useAuthStore()
const { loading, error, get } = useAxios()

onMounted(async () => {
    console.log('component mounted')
    try {
        console.log("makking call")
        const { data: { data, errors }, status } = await get<HttpResponseModel<TotalUsersStatApiResponse>>('/api/admin/home/get-total-users')
        console.log('get call completed')

        if (status !== axios.HttpStatusCode.Ok) {
            const errorMsg = errors.length ? `Failed to fetch data: ${errors}`
                : 'Failed to fetch data'

            toast.error(errorMsg)
            return
        }

        console.log('data:', data)
    } catch (error) {
        if (axios.isAxiosError(error)) {
            toast.error(`Axios error: ${error.response?.status}`)
        } else {
            toast.error(`Unexpected error: ${error}`)
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