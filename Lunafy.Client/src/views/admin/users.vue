<script setup lang="ts">
import { ref, watch } from 'vue'
import Pagination from '@/components/admin/Pagination.vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import { HTTP_STATUS } from '@/utils'
import type { HttpResponseModel, SearchResultModel } from '@/types/common'
import type { UserModel } from '@/types/admin'
import type { LoginResponseModel } from '@/types/user'

const baseUrl = import.meta.env.VITE_API_URL

const authStore = useAuthStore()
const router = useRouter()

const loading = ref<boolean>(false)
const error = ref<string | null>(null)

const page = ref<number>(1)
const totalPages = ref<number>(0)
const pageSize = ref<number>(5)

const users = ref<UserModel[] | null>()

const fetchUsers = async (page: number, pageSize: number) => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const urlParams = new URLSearchParams({
        pageNumber: page.toString(),
        pageSize: pageSize.toString()
    })

    const response = await fetch(`${baseUrl}/api/admin/user?${urlParams.toString()}`, {
        method: 'GET', headers, credentials: 'include'
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    const { data, errors }: HttpResponseModel<SearchResultModel<UserModel>> = await response.json()
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

    totalPages.value = data.totalPages
    users.value = data.data

    return response.status
}

watch([page, pageSize], async () => {
    try {
        if (await fetchUsers(page.value, pageSize.value) === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })
            const { data, errors }: HttpResponseModel<LoginResponseModel> = await response.json()

            if (!response.ok) {
                router.push('/login')
            }

            if (await fetchUsers(page.value, pageSize.value) !== HTTP_STATUS.OK) {
                error.value = "Failed to load User Stats. Please try again."
            }
        }
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        error.value = errorMessage
        throw err
    } finally {
        loading.value = false
    }
}, { immediate: true })
</script>

<template>
    <!-- Header -->
    <div class="container">
        <div class="d-flex justify-content-between align-items-center">
            <h3>Users</h3>
            <button class="btn btn-primary">
                <i class="bi bi-plus"></i>
                New Report
            </button>
        </div>
    </div>

    <!-- User table -->
    <div class="container  mt-3">
        <table class="table table-striped">
            <thead>
                <tr>
                    <th scope="col">#</th>
                    <th scope="col">User</th>
                    <th scope="col">Email</th>
                    <th scope="col">Is Admin</th>
                    <th scope="col">Is Artist</th>
                    <th scope="col">Actions</th>
                </tr>
            </thead>
            <tbody>
                <template v-if="!users || !users.length">
                    <tr>
                        <td colspan="6" class="text-center">
                            No data
                        </td>
                    </tr>
                </template>
                <template v-else>
                    <tr v-for="user in users" :key="user.id">
                        <th scope="row">{{ user.id }}</th>
                        <th scope="row">
                            <h6>{{ user.firstname }} {{ user.lastname }}</h6>
                            <p class="mb-0" style="font-size: smaller;">@{{ user.username }}</p>
                        </th>
                        <th scope="row">{{ user.email }}</th>
                        <th scope="row">
                            <i class="bi bi-check-lg" v-if="user.isAdmin"></i>
                            <i class="bi bi-x-lg" v-else></i>
                        </th>
                        <th scope="row">
                            <i class="bi bi-check-lg" v-if="user.isArtist"></i>
                            <i class="bi bi-x-lg" v-else></i>
                        </th>
                        <th scope="row">
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <RouterLink :to="`/admin/users/${user.id}`" class="btn btn-primary w-100">
                                            <i class="bi bi-pencil-square"></i>
                                        </RouterLink>
                                    </div>
                                    <div class="col">
                                        <button class="btn btn-danger w-100">
                                            <i class="bi bi-trash3-fill"></i>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </th>
                    </tr>
                </template>
            </tbody>
        </table>
        <Pagination :totalPages="totalPages" :currentPage="page" :pageSize="pageSize"
            @changePageSize="(x) => pageSize = x" @changePage="(x) => page = x" />
    </div>
</template>