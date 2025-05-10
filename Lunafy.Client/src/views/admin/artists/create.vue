<script setup lang="ts">
import Loader from '@/components/admin/Loader.vue'
import { useAuthStore } from '@/stores/auth'
import type { ArtistCreateErrorModel, ArtistCreateModel, UserCreateErrorModel, UserCreateModel, UserReadModel } from '@/types/admin'
import type { HttpResponseModel } from '@/types/common'
import { HTTP_STATUS } from '@/utils'
import { computed, reactive, watch } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'

const baseUrl = import.meta.env.VITE_API_URL

const authStore = useAuthStore()
const router = useRouter()

const state = reactive<{
    loading: boolean,
    error?: string | null,
    userModel: ArtistCreateModel,
    userErrorModel: ArtistCreateErrorModel,
    musicBrainzIdValidating: boolean
}>({
    loading: false,
    userModel: {
        firstname: '',
        lastname: '',
        biography: null,
        musicBrainzId: null,
    },
    userErrorModel: {
        firstname: null,
        lastname: null,
        musicBrainzId: null,
    },
    musicBrainzIdValidating: false,
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
                state.error = "Failed to create new user. Please try again."
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

const validateFirstname = (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.userErrorModel.firstname = null

    state.userErrorModel.firstname = value.length <= 0 ? 'Firstname is required.' : null
    state.userModel.firstname = value
}

const validateLastname = (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.userErrorModel.lastname = null

    state.userErrorModel.lastname = value.length <= 0 ? 'Lastname is required.' : null
    state.userModel.lastname = value
}

const validateMusicBrainz = async (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.userErrorModel.musicBrainzId = null

    state.musicBrainzIdValidating = true
    if (!state.userErrorModel.musicBrainzId) {
        state.userErrorModel.musicBrainzId = value.length <= 0 ? 'Email is required.' : null
    }

    if (!state.userErrorModel.musicBrainzId) {
        const response = await fetch(`${baseUrl}/api/admin/user/email-availability?email=${value}`)
        if (!response.ok) {
            state.userErrorModel.email = 'Cannot use this email'
        } else {
            const payload: boolean = await response.json()
            state.userErrorModel.email = payload ? null : 'Cannot use this email. It is already in use.'
        }
    }

    state.userModel.email = value
    state.emailValidating = false
}

const isFirstnameValid = computed(() => !state.userErrorModel.firstname || state.userErrorModel.firstname.length <= 0)
const isLastnameValid = computed(() => !state.userErrorModel.lastname || state.userErrorModel.lastname.length <= 0)
const isEmailValid = computed(() => !state.userErrorModel.email || state.userErrorModel.email.length <= 0)
const isUsernameValid = computed(() => !state.userErrorModel.username || state.userErrorModel.username.length <= 0)
</script>

<template>
    <form @submit.prevent="onUserCreateSubmission">
        <!-- Header -->
        <div class="container">
            <div class="d-flex justify-content-between align-items-center">
                <h3>Create New User</h3>
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
                                            :value="state.userModel.firstname"
                                            @change="(e: Event) => validateFirstname(e)">
                                        <div
                                            :class="`${isFirstnameValid ? 'valid-feedback' : 'invalid-feedback d-block'}`">
                                            {{ isFirstnameValid ? '' : state.userErrorModel.firstname }}
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="lastname" class="form-label">Last Name</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="text" class="form-control" id="lastname" placeholder="Doe"
                                            :value="state.userModel.lastname" @change="validateLastname">
                                        <div
                                            :class="`${isLastnameValid ? 'valid-feedback' : 'invalid-feedback d-block'}`">
                                            {{ isLastnameValid ? '' : state.userErrorModel.lastname }}
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="email" class="form-label">Email</label>
                                    </div>
                                    <div class="col-9">
                                        <div class="d-flex align-items-center">
                                            <input type="email" class="form-control"
                                                :class="{ 'me-2': state.emailValidating, 'me-0': !state.emailValidating }"
                                                id="email" placeholder="johndoe@email.com"
                                                :value="state.userModel.email" @change="validateEmail">
                                            <div class="spinner-border spinner-border-sm"
                                                :class="{ 'd-block': state.emailValidating, 'd-none': !state.emailValidating }"
                                                role="status">
                                                <span class="visually-hidden">Loading...</span>
                                            </div>
                                        </div>
                                        <div :class="`${isEmailValid ? 'valid-feedback' : 'invalid-feedback d-block'}`">
                                            {{ isEmailValid ? '' : state.userErrorModel.email }}
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="username" class="form-label">Username</label>
                                    </div>
                                    <div class="col-9">
                                        <div class="d-flex align-items-center">
                                            <input type="username" class="form-control"
                                                :class="{ 'me-2': state.usernameValidating, 'me-0': !state.usernameValidating }"
                                                id="username" placeholder="john_doe" :value="state.userModel.username"
                                                @change="validateUsername">
                                            <div class="spinner-border spinner-border-sm"
                                                :class="{ 'd-block': state.usernameValidating, 'd-none': !state.usernameValidating }"
                                                role="status">
                                                <span class="visually-hidden">Loading...</span>
                                            </div>
                                        </div>
                                        <div
                                            :class="`${isUsernameValid ? 'valid-feedback' : 'invalid-feedback d-block'}`">
                                            {{ isUsernameValid ? '' : state.userErrorModel.username }}
                                        </div>
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