<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import type { UserModel } from '@/types/admin'
import type { HttpResponseModel } from '@/types/common'
import type { LoginResponseModel } from '@/types/user'
import { HTTP_STATUS } from '@/utils'
import { onMounted, reactive, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'

const baseUrl = import.meta.env.VITE_API_URL

const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()
const userId = route.params.id

const state = reactive<{
    loading: boolean,
    error?: string | null,
    userModel?: UserModel | null
}>({
    loading: false
})

const fetchUser = async () => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/user/${userId}`, {
        method: 'GET', headers, credentials: 'include'
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    const { data, errors }: HttpResponseModel<UserModel> = await response.json()
    if (!response.ok) {
        const errorMsg = errors.find(el => el.length > 0) || "Something went wrong. Please try again."
        state.error = errorMsg
        return response.status
    }

    if (!data) {
        const errorMsg = "Something went wrong. Please try again."
        state.error = errorMsg
        return response.status
    }

    state.userModel = data
    return response.status
}

watch(() => state.error, () => {
    if (state.error && state.error.length > 0) {
        toast.error(state.error, { onClose: () => state.error = null })
    }
})

onMounted(async () => {
    state.loading = true
    try {
        if (await fetchUser() === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })
            const { data, errors }: HttpResponseModel<LoginResponseModel> = await response.json()

            if (!response.ok) {
                router.push('/login')
            }

            if (await fetchUser() !== HTTP_STATUS.OK) {
                state.error = "Failed to load User Stats. Please try again."
            }
        }
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        state.error = errorMessage
        throw err
    } finally {
        state.loading = false
    }
})
</script>

<template>
    <form @submit.prevent="console.log('form submitted...')">
        <!-- Header -->
        <div class="container">
            <div class="d-flex justify-content-between align-items-center">
                <h3>Edit User - John Doe</h3>
                <div class="d-flex">
                    <button type="submit" class="btn btn-success me-2">
                        <i class="bi bi-floppy-fill"></i>
                        Save
                    </button>
                    <button type="button" class="btn btn-danger" data-bs-toggle="modal"
                        data-bs-target="#deleteConfirmation" @click.prevent="">
                        <i class="bi bi-trash-fill"></i>
                        Delete
                    </button>
                </div>
            </div>
        </div>

        <div class="container mt-3">
            <div class="accordion">
                <div class="accordion-item">
                    <h2 class="accordion-header">
                        <button class="accordion-button" type="button" data-bs-toggle="collapse"
                            data-bs-target="#panel-info" aria-expanded="true" aria-controls="panel-info">
                            User Info
                        </button>
                    </h2>
                    <div id="panel-info" class="accordion-collapse collapse show">
                        <div class="accordion-body">
                            <div class="container">
                                <div class="row">
                                    <div class="col-3">
                                        <label for="firstname" class="form-label">First Name</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="text" class="form-control" id="firstname" placeholder="John"
                                            :value="state.userModel?.firstname">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="lastname" class="form-label">Last Name</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="text" class="form-control" id="lastname" placeholder="Doe"
                                            :value="state.userModel?.lastname">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="email" class="form-label">Email</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="email" class="form-control" id="email"
                                            placeholder="johndoe@email.com" :value="state.userModel?.email">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="username" class="form-label">Username</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="username" class="form-control" id="username" placeholder="john_doe"
                                            :value="state.userModel?.username">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="isadmin" class="form-label">Is Admin</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="checkbox" class="form-check-input" id="isadmin"
                                            :checked="state.userModel?.isAdmin" @change="() => {
                                                if (state.userModel) state.userModel.isAdmin = !state.userModel.isAdmin
                                            }">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="isartist" class="form-label">Is Artist</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="checkbox" class="form-check-input" id="isartist"
                                            :checked="state.userModel?.isArtist" @change="() => {
                                                if (state.userModel) state.userModel.isArtist = !state.userModel.isArtist
                                            }">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

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
                    <button type="button" class="btn btn-danger">Delete</button>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped></style>