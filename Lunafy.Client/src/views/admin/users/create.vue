<script setup lang="ts">
import Loader from '@/components/admin/Loader.vue'
import { useAuthStore } from '@/stores/auth'
import type { UserCreateModel, UserReadModel } from '@/types/admin'
import type { HttpResponseModel } from '@/types/common'
import { HTTP_STATUS } from '@/utils'
import { reactive, watch } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'

const baseUrl = import.meta.env.VITE_API_URL

const authStore = useAuthStore()
const router = useRouter()

const state = reactive<{
    loading: boolean,
    error?: string | null,
    userModel: UserCreateModel
}>({
    loading: false,
    userModel: {
        firstname: '',
        lastname: '',
        username: '',
        email: '',
        isAdmin: false,
        isArtist: false
    }
})

watch(() => state.error, () => {
    if (state.error && state.error.length > 0) {
        toast.error(state.error, { onClose: () => state.error = null })
    }
})

const createUser = async (firstname: string, lastname: string, username: string, email: string, isAdmin: boolean, isArtist: boolean) => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/user`, {
        method: 'POST', headers, credentials: 'include', body: JSON.stringify({ firstname, lastname, username, email, isAdmin, isArtist })
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    const { data, errors }: HttpResponseModel<UserReadModel> = await response.json()
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

    return response.status
}

const onUserCreateSubmission = async () => {
    state.loading = true
    try {
        if (await createUser(state.userModel.firstname, state.userModel.lastname, state.userModel.username, state.userModel.email, state.userModel.isAdmin, state.userModel.isArtist) === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })
            const { data, errors }: HttpResponseModel<UserReadModel> = await response.json()

            if (!response.ok) {
                router.push('/login')
            }

            if (await createUser(state.userModel.firstname, state.userModel.lastname, state.userModel.username, state.userModel.email, state.userModel.isAdmin, state.userModel.isArtist) !== HTTP_STATUS.CREATED) {
                state.error = "Failed to load User Stats. Please try again."
            }
        }

        router.push('/admin/users')
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        state.error = errorMessage
        throw err
    } finally {
        state.loading = false
    }
}
</script>

<template>
    <form @submit.prevent="onUserCreateSubmission">
        <!-- Header -->
        <div class="container">
            <div class="d-flex justify-content-between align-items-center">
                <h3>Edit User - John Doe</h3>
                <div class="d-flex">
                    <button type="submit" class="btn btn-success me-2">
                        <i class="bi bi-floppy-fill"></i>
                        Save
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
                                            :value="state.userModel.firstname">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="lastname" class="form-label">Last Name</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="text" class="form-control" id="lastname" placeholder="Doe"
                                            :value="state.userModel.lastname">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="email" class="form-label">Email</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="email" class="form-control" id="email"
                                            placeholder="johndoe@email.com" :value="state.userModel.email">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="username" class="form-label">Username</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="username" class="form-control" id="username" placeholder="john_doe"
                                            :value="state.userModel.username">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="isadmin" class="form-label">Is Admin</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="checkbox" class="form-check-input" id="isadmin"
                                            :checked="state.userModel.isAdmin"
                                            @change="() => state.userModel.isAdmin = !state.userModel.isAdmin">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="isartist" class="form-label">Is Artist</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="checkbox" class="form-check-input" id="isartist"
                                            :checked="state.userModel.isArtist"
                                            @change="() => state.userModel.isArtist = !state.userModel.isArtist">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <Loader :loading="state.loading" />
</template>

<style lang="css" scoped></style>