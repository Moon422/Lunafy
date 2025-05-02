<script setup lang="ts">
import type { TotalUsersStatApiResponse } from '@/types/admin'
import { useAuthStore } from '@/stores/auth'
import { ref, onMounted, watch } from 'vue'
import { toast } from 'vue3-toastify'
import type { HttpResponseModel } from '@/types/common'
import { HTTP_STATUS } from '@/utils'
import type { LoginResponseModel } from '@/types/user'

const baseUrl = import.meta.env.VITE_API_URL

const authStore = useAuthStore()

const loading = ref<boolean>(false)
const error = ref<string | null>(null)

const currentCount = ref<number>(0)
const infiniteIncrement = ref<boolean>(false)
const changePercentage = ref<number>(0)

const fetchUserStats = async () => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/home/get-total-users`, {
        method: 'GET', headers, credentials: 'include'
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    const { data, errors }: HttpResponseModel<TotalUsersStatApiResponse> = await response.json()
    if (!response.ok) {
        const errorMsg = errors.find(el => el.length > 0) || "Something went wrong. Please try again."
        error.value = errorMsg
        return response.status
    }

    if (!data) {
        const errorMsg = "Something went wrong. Please try again."
        error.value = errorMsg
        return response.status
    }

    currentCount.value = data.presentUserCount
    infiniteIncrement.value = data.infiniteIncrement
    changePercentage.value = data.changePercentage

    return response.status
}

watch(error, () => {
    if (error.value && error.value.length > 0) {
        toast.error(error.value, { onClose: () => error.value = null })
    }
})

onMounted(async () => {
    try {
        const status = await fetchUserStats()

        if (status === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`)
            const { data, errors }: HttpResponseModel<LoginResponseModel> = await response.json()

            if (response.status === HTTP_STATUS.BAD_REQUEST) {
                const errMsg = errors.length && errors.find(x => x.trim().length > 0) || 'Something went wrong. Try again'
                error.value = errMsg
            }
        }
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        error.value = errorMessage
        throw err
    } finally {
        loading.value = false
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
                <h6>Total Users
                    <span class="d-none d-sm-inline fs-6 text-success" v-if="!infiniteIncrement">
                        <i class="bi bi-arrow-up-short"></i>12%
                    </span>
                    <span class="d-none d-sm-inline fs-6 text-success" v-else>
                        <i class="bi bi-arrow-up-short"></i>
                        <i class="bi bi-infinity ms-1"></i>
                    </span>
                </h6>
                <h3>
                    {{ currentCount }}
                </h3>
            </div>
        </div>
    </div>
</template>