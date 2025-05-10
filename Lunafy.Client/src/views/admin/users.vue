<script setup lang="ts">
import { ref, watch } from 'vue'
import Pagination from '@/components/admin/Pagination.vue'
import { useAuthStore } from '@/stores/auth'
import { RouterLink, useRouter } from 'vue-router'
import { HTTP_STATUS } from '@/utils'
import type { HttpResponseModel, SearchResultModel } from '@/types/common'
import type { UserReadModel } from '@/types/admin'
import type { LoginResponseModel } from '@/types/user'
import Loader from '@/components/admin/Loader.vue'

const baseUrl = import.meta.env.VITE_API_URL

const authStore = useAuthStore()
const router = useRouter()

const loading = ref<boolean>(false)
const error = ref<string | null>(null)

const page = ref<number>(1)
const totalPages = ref<number>(0)
const pageSize = ref<number>(5)

const users = ref<UserReadModel[] | null>()
const userIdToDelete = ref<number>(0)

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

    const { data, errors }: HttpResponseModel<SearchResultModel<UserReadModel>> = await response.json()
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

const deleteUser = async () => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/user/${userIdToDelete.value}`, {
        method: 'DELETE', headers, credentials: 'include'
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    if (response.status === HTTP_STATUS.FORBIDDEN) {
        router.push('/forbidden')
    }

    if (response.status === HTTP_STATUS.BAD_REQUEST) {
        const { errors }: HttpResponseModel<null> = await response.json()
        const errorMsg = errors && errors.find(el => el.length > 0) || "Something went wrong. Please try again."
        error.value = errorMsg
        return response.status
    }

    return response.status
}

const onDeleteConfirmation = async () => {
    if (userIdToDelete.value <= 0) {
        return false
    }

    loading.value = true
    try {
        if (await deleteUser() === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })

            if (!response.ok) {
                router.push('/login')
            }

            if (await deleteUser() !== HTTP_STATUS.NO_CONTENT) {
                error.value = "Failed to delete user. Please try again."
            }
        }

        await fetchUsers(page.value, pageSize.value)
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        error.value = errorMessage
    } finally {
        loading.value = false
    }
}

watch([page, pageSize], async () => {
    loading.value = true
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
            <RouterLink to="/admin/users/create" class="btn btn-primary">
                <i class="bi bi-plus"></i>
                New User
            </RouterLink>
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
                        <th scope="row" style="width: 16rem;">
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <RouterLink :to="`/admin/users/${user.id}`" class="btn btn-primary w-100">
                                            <i class="bi bi-pencil-square"></i>
                                        </RouterLink>
                                    </div>
                                    <div class="col">
                                        <button class="btn btn-danger w-100" data-bs-toggle="modal"
                                            data-bs-target="#deleteConfirmation"
                                            @click.prevent="() => userIdToDelete = user.id">
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

    <!-- Modal -->
    <div class="modal fade" id="deleteConfirmation" tabindex="-1" aria-labelledby="deleteConfirmationLabel"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="deleteConfirmationLabel">Are you sture you want to delete the user?
                    </h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    The user will be soft deleted.
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-danger" data-bs-dismiss="modal"
                        @click="onDeleteConfirmation">Delete</button>
                </div>
            </div>
        </div>
    </div>
    <Loader :loading="loading" />
</template>