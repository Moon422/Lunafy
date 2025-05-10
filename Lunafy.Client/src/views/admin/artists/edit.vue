<script setup lang="ts">
import Loader from '@/components/admin/Loader.vue'
import { useAuthStore } from '@/stores/auth'
import type { UserCreateErrorModel, UserCreateModel, UserEditModel, UserReadModel } from '@/types/admin'
import type { HttpResponseModel } from '@/types/common'
import type { LoginResponseModel } from '@/types/user'
import { HTTP_STATUS } from '@/utils'
import { computed, onMounted, reactive, watch } from 'vue'
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
    userModel?: UserReadModel | null,
    userErrorModel: UserCreateErrorModel,
    emailValidating: boolean,
    usernameValidating: boolean
}>({
    loading: false,
    userErrorModel: {
        firstname: null,
        lastname: null,
        email: null,
        username: null
    },
    emailValidating: false,
    usernameValidating: false
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

    state.userModel = data
    return response.status
}

const editUser = async (payload: UserCreateModel) => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/user`, {
        method: 'PUT', headers, credentials: 'include', body: JSON.stringify(payload)
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    if (response.status === HTTP_STATUS.BAD_REQUEST) {
        const { errors }: HttpResponseModel<null> = await response.json()
        const errorMsg = errors && errors.find(el => el.length > 0) || "Something went wrong. Please try again."
        state.error = errorMsg
        return response.status
    }

    return response.status
}

const submitUserEdit = async (e: Event) => {
    if (!isFirstnameValid || !isLastnameValid || !isUsernameValid || !isEmailValid) {
        state.error = "Some input fields are invalid. Please enter valid inputs."
        return false
    }

    const requestPayload: UserEditModel = {
        id: typeof userId === 'string' ? Number.isNaN(userId) ? 0 : parseInt(userId) : Number.isNaN(userId[0]) ? 0 : parseInt(userId[0]),
        firstname: state.userModel?.firstname || '',
        lastname: state.userModel?.lastname || '',
        username: state.userModel?.username || '',
        email: state.userModel?.email || '',
        isAdmin: state.userModel?.isAdmin || false,
        isArtist: state.userModel?.isArtist || false
    }

    state.loading = true

    try {
        if (await editUser(requestPayload) === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })

            if (!response.ok) {
                router.push('/login')
            }

            if (await editUser(requestPayload) !== HTTP_STATUS.NO_CONTENT) {
                state.error = "Failed to update user. Please try again."
            }
        }

        // router.push('/admin/users')
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        state.error = errorMessage
    } finally {
        state.loading = false
    }
}

const deleteUser = async () => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/user/${userId}`, {
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
        state.error = errorMsg
        return response.status
    }

    return response.status
}

const onDeleteConfirmation = async () => {
    state.loading = true
    try {
        if (await deleteUser() === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })

            if (!response.ok) {
                router.push('/login')
            }

            if (await deleteUser() !== HTTP_STATUS.NO_CONTENT) {
                state.error = "Failed to delete user. Please try again."
            }
        }
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        state.error = errorMessage
    } finally {
        state.loading = false
    }
}

const validateFirstname = (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.userErrorModel.firstname = null

    state.userErrorModel.firstname = value.length <= 0 ? 'Firstname is required.' : null
    if (state.userModel) state.userModel.firstname = value
}

const validateLastname = (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.userErrorModel.lastname = null

    state.userErrorModel.lastname = value.length <= 0 ? 'Lastname is required.' : null
    if (state.userModel) state.userModel.lastname = value
}

const validateEmail = async (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.userErrorModel.email = null

    state.emailValidating = true
    if (!state.userErrorModel.email) {
        state.userErrorModel.email = value.length <= 0 ? 'Email is required.' : null
    }

    if (!state.userErrorModel.email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        state.userErrorModel.email = !emailRegex.test(value) ? 'Invalid email format.' : null
    }

    if (!state.userErrorModel.email) {
        const response = await fetch(`${baseUrl}/api/admin/user/email-availability?email=${value}&userId=${state.userModel?.id}`)
        if (!response.ok) {
            state.userErrorModel.email = 'Cannot use this email'
        } else {
            const payload: boolean = await response.json()
            state.userErrorModel.email = payload ? null : 'Cannot use this email. It is already in use.'
        }
    }

    if (state.userModel) state.userModel.email = value
    state.emailValidating = false
}

const validateUsername = async (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.userErrorModel.email = null

    state.usernameValidating = true
    if (!state.userErrorModel.username) {
        state.userErrorModel.username = value.length <= 0 ? 'Username is required' : null
    }

    if (!state.userErrorModel.username) {
        const response = await fetch(`${baseUrl}/api/admin/user/username-availability?username=${value}&userId=${state.userModel?.id}`)
        if (!response.ok) {
            state.userErrorModel.username = 'Cannot use this username'
        } else {
            const payload: boolean = await response.json()
            state.userErrorModel.username = payload ? null : 'Cannot use this username. It is already in use.'
        }
    }

    if (state.userModel) state.userModel.username = value
    state.usernameValidating = false
}

const isFirstnameValid = computed(() => !state.userErrorModel.firstname || state.userErrorModel.firstname.length <= 0)
const isLastnameValid = computed(() => !state.userErrorModel.lastname || state.userErrorModel.lastname.length <= 0)
const isEmailValid = computed(() => !state.userErrorModel.email || state.userErrorModel.email.length <= 0)
const isUsernameValid = computed(() => !state.userErrorModel.username || state.userErrorModel.username.length <= 0)

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
                state.error = "Failed to load user data. Please try again."
            }
        }
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        state.error = errorMessage
    } finally {
        state.loading = false
    }
})
</script>

<template>
    <form @submit.prevent="submitUserEdit">
        <!-- Header -->
        <div class="container">
            <div class="d-flex justify-content-between align-items-center">
                <h3>Edit User - {{ state.userModel?.firstname }} {{ state.userModel?.lastname }}</h3>
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
                            <div class="container" v-if="state.userModel">
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
                                        <input type="username" class="form-control" id="username" placeholder="john_doe"
                                            :value="state.userModel.username" @change="validateUsername">
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
                                            :checked="state.userModel.isAdmin" @change="() => {
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
                                            :checked="state.userModel.isArtist" @change="() => {
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
                    <button type="button" class="btn btn-danger" @click="onDeleteConfirmation">Delete</button>
                </div>
            </div>
        </div>
    </div>

    <Loader :loading="state.loading" />
</template>

<style lang="css" scoped></style>