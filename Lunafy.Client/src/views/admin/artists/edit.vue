<script setup lang="ts">
import Loader from '@/components/admin/Loader.vue'
import { useAuthStore } from '@/stores/auth'
import type { ArtistCreateErrorModel, ArtistCreateModel, ArtistEditModel, ArtistReadModel, ProfilePictureModel } from '@/types/admin'
import type { HttpResponseModel } from '@/types/common'
import type { LoginResponseModel } from '@/types/user'
import { HTTP_STATUS } from '@/utils'
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { toast } from 'vue3-toastify'

const baseUrl = import.meta.env.VITE_API_URL

const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()
const artistId = route.params.id

const state = reactive<{
    loading: boolean,
    error?: string | null,
    artistModel: ArtistCreateModel,
    artistErrorModel: ArtistCreateErrorModel,
    musicBrainzIdValidating: boolean,
    uploadProfileImage: File | null,
    uploadProfileImageSuccessMsg: string | null,
    uploadProfileImageFailMsg: string | null,

}>({
    loading: false,
    artistModel: {
        firstname: '',
        lastname: '',
        biography: null,
        musicBrainzId: null,
    },
    artistErrorModel: {
        firstname: null,
        lastname: null,
        musicBrainzId: null,
    },
    musicBrainzIdValidating: false,
    uploadProfileImage: null,
    uploadProfileImageSuccessMsg: null,
    uploadProfileImageFailMsg: null
})

const fileInput = ref<HTMLInputElement | null>(null)

const fetchArtist = async () => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/artist/${artistId}`, {
        method: 'GET', headers, credentials: 'include'
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    const { data, errors }: HttpResponseModel<ArtistReadModel> = await response.json()
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

    state.artistModel = data
    return response.status
}

const editArtist = async (payload: ArtistEditModel) => {
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

const submitArtistEdit = async (e: Event) => {
    if (!isFirstnameValid || !isLastnameValid || !isMusicBrainzIdValid) {
        state.error = "Some input fields are invalid. Please enter valid inputs."
        return false
    }

    const requestPayload: ArtistEditModel = {
        id: typeof artistId === 'string' ? Number.isNaN(artistId) ? 0 : parseInt(artistId) : Number.isNaN(artistId[0]) ? 0 : parseInt(artistId[0]),
        firstname: state.artistModel?.firstname || '',
        lastname: state.artistModel?.lastname || '',
        biography: state.artistModel?.biography || null,
        musicBrainzId: state.artistModel?.musicBrainzId || null
    }

    state.loading = true

    try {
        if (await editArtist(requestPayload) === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })

            if (!response.ok) {
                router.push('/login')
            }

            if (await editArtist(requestPayload) !== HTTP_STATUS.NO_CONTENT) {
                state.error = "Failed to update user. Please try again."
            }
        }

        router.push('/admin/artists')
    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        state.error = errorMessage
    } finally {
        state.loading = false
    }
}

const deleteArtist = async () => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/artist/${artistId}`, {
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
        if (await deleteArtist() === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })

            if (!response.ok) {
                router.push('/login')
            }

            if (await deleteArtist() !== HTTP_STATUS.NO_CONTENT) {
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
    state.artistErrorModel.firstname = null

    state.artistErrorModel.firstname = value.length <= 0 ? 'Firstname is required.' : null
    state.artistModel.firstname = value
}

const validateLastname = (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.artistErrorModel.lastname = null

    state.artistErrorModel.lastname = value.length <= 0 ? 'Lastname is required.' : null
    state.artistModel.lastname = value
}

const validateMusicBrainz = async (e: Event) => {
    const target = e.target as HTMLInputElement
    const value = target.value
    state.artistErrorModel.musicBrainzId = null

    if (value.length <= 0) {
        return
    }

    state.musicBrainzIdValidating = true

    if (!state.artistErrorModel.musicBrainzId) {
        const response = await fetch(`${baseUrl}/api/admin/artist/musicbrainz-id-availability?musicBrainzId=${value}`, {
            credentials: 'include'
        })
        if (!response.ok) {
            state.artistErrorModel.musicBrainzId = 'Cannot use this music brainz id'
        } else {
            const payload: boolean = await response.json()
            state.artistErrorModel.musicBrainzId = payload ? null : 'Cannot use this music brainz id. It is already in use.'
        }
    }

    state.artistModel.musicBrainzId = value
    state.musicBrainzIdValidating = false
}

const isFirstnameValid = computed(() => !state.artistErrorModel.firstname || state.artistErrorModel.firstname.length <= 0)
const isLastnameValid = computed(() => !state.artistErrorModel.lastname || state.artistErrorModel.lastname.length <= 0)
const isMusicBrainzIdValid = computed(() => !state.artistErrorModel.musicBrainzId || state.artistErrorModel.musicBrainzId.length <= 0)

const handleUploadProfileImage = (e: Event) => {
    const target = e.target as HTMLInputElement

    state.uploadProfileImage = null
    if (target.files && target.files.length > 0) {
        state.uploadProfileImage = target.files[0]
    }
}

const confirmUploadImage = async () => {
    if (!state.uploadProfileImage) {
        return
    }

    await uploadImage()
}

const sendUploadRequest = async () => {
    const formData = new FormData()
    formData.append("image", state.uploadProfileImage || new Blob([], { type: "application/octet-stream" }), state.uploadProfileImage?.name || '')

    const id = typeof artistId === 'string' ? Number.isNaN(artistId) ? 0 : parseInt(artistId) : Number.isNaN(artistId[0]) ? 0 : parseInt(artistId[0])
    const response = await fetch(`${baseUrl}/api/admin/artist/${id}/upload-profile-picture`, {
        method: 'POST',
        credentials: 'include',
        body: formData
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    const { errors }: HttpResponseModel<ProfilePictureModel> = await response.json()
    if (!response.ok) {
        const errorMsg = errors.find(el => el.length > 0) || "Something went wrong. Please try again."
        state.uploadProfileImageFailMsg = errorMsg
        return response.status
    }

    state.uploadProfileImage = null
    if (fileInput.value) fileInput.value.value = ''
    state.uploadProfileImageSuccessMsg = "Profile picture uploaded successfully."
    return response.status
}

const uploadImage = async () => {
    state.uploadProfileImageFailMsg = null
    state.uploadProfileImageSuccessMsg = null

    state.loading = true
    try {
        if (await sendUploadRequest() === HTTP_STATUS.UNAUTHORIZED) {
            const loginResponse = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })

            if (!loginResponse.ok) {
                router.push('/login')
            }

            await sendUploadRequest()
        }

    } catch (err: unknown) {
        const errorMessage = err instanceof Error ? err.message : String(err)
        state.error = errorMessage
    } finally {
        state.loading = false
    }
}

watch(() => state.error, () => {
    if (state.error && state.error.length > 0) {
        toast.error(state.error, { onClose: () => state.error = null })
    }
})

onMounted(async () => {
    state.loading = true
    try {
        if (await fetchArtist() === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })
            const { data, errors }: HttpResponseModel<LoginResponseModel> = await response.json()

            if (!response.ok) {
                router.push('/login')
            }

            if (await fetchArtist() !== HTTP_STATUS.OK) {
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
    <form @submit.prevent="submitArtistEdit">
        <!-- Header -->
        <div class="container">
            <div class="d-flex justify-content-between align-items-center">
                <h3>Edit User - {{ state.artistModel?.firstname }} {{ state.artistModel?.lastname }}</h3>
                <div class="d-flex">
                    <button type="submit" class="btn btn-success me-2">
                        <i class="bi bi-floppy-fill"></i>
                        Save
                    </button>
                    <button type="submit" class="btn btn-success me-2" data-bs-toggle="modal"
                        data-bs-target="#uploadProfileImage" @click.prevent="">
                        <i class="bi bi-image-fill"></i>
                        Upload Profile Image
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
                                            :value="state.artistModel.firstname"
                                            @change="(e: Event) => validateFirstname(e)">
                                        <div
                                            :class="`${isFirstnameValid ? 'valid-feedback' : 'invalid-feedback d-block'}`">
                                            {{ isFirstnameValid ? '' : state.artistErrorModel.firstname }}
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="lastname" class="form-label">Last Name</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="text" class="form-control" id="lastname" placeholder="Doe"
                                            :value="state.artistModel.lastname" @change="validateLastname">
                                        <div
                                            :class="`${isLastnameValid ? 'valid-feedback' : 'invalid-feedback d-block'}`">
                                            {{ isLastnameValid ? '' : state.artistErrorModel.lastname }}
                                        </div>
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="biography" class="form-label">Biography</label>
                                    </div>
                                    <div class="col-9">
                                        <input type="email" class="form-control" id="email" placeholder="Biography"
                                            v-model="state.artistModel.biography">
                                    </div>
                                </div>
                                <div class="row mt-3">
                                    <div class="col-3">
                                        <label for="musicBrainzId" class="form-label">Music Brainz Id</label>
                                    </div>
                                    <div class="col-9">
                                        <div class="d-flex align-items-center">
                                            <input type="musicBrainzId" class="form-control"
                                                :class="{ 'me-2': state.musicBrainzIdValidating, 'me-0': !state.musicBrainzIdValidating }"
                                                id="musicBrainzId" placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxxx"
                                                :value="state.artistModel.musicBrainzId" @change="validateMusicBrainz">
                                            <div class="spinner-border spinner-border-sm"
                                                :class="{ 'd-block': state.musicBrainzIdValidating, 'd-none': !state.musicBrainzIdValidating }"
                                                role="status">
                                                <span class="visually-hidden">Loading...</span>
                                            </div>
                                        </div>
                                        <div
                                            :class="`${isMusicBrainzIdValid ? 'valid-feedback' : 'invalid-feedback d-block'}`">
                                            {{ isMusicBrainzIdValid ? '' : state.artistErrorModel.musicBrainzId }}
                                        </div>
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

    <div class="modal fade" id="uploadProfileImage" tabindex="-1" aria-labelledby="uploadProfileImageLabel"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="uploadProfileImageLabel">
                        Upload new profile image
                    </h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label for="formFile" class="form-label">Profile Image</label>
                        <input class="form-control" type="file" @change="handleUploadProfileImage" ref="fileInput">
                        <div :class="`${state.uploadProfileImageSuccessMsg && state.uploadProfileImageSuccessMsg.length > 0
                            ? 'valid-feedback d-block'
                            : state.uploadProfileImageFailMsg && state.uploadProfileImageFailMsg.length > 0
                                ? 'invalid-feedback d-block'
                                : 'd-none'}`">
                            {{ state.uploadProfileImageSuccessMsg && state.uploadProfileImageSuccessMsg.length > 0
                                ? state.uploadProfileImageSuccessMsg
                                : state.uploadProfileImageFailMsg && state.uploadProfileImageFailMsg.length > 0
                                    ? state.uploadProfileImageFailMsg
                                    : '' }}
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary"
                        :data-bs-target="!state.uploadProfileImage ? '#removeProfilePicture' : null"
                        :data-bs-toggle="!state.uploadProfileImage ? 'modal' : null"
                        @click="confirmUploadImage">Upload</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="removeProfilePicture" tabindex="-1" aria-labelledby="removeProfilePictureLabel"
        aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="removeProfilePictureLabel">
                        Clear profile picture?
                    </h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p>You have not selected any image file. This is remove your existing profile picture. Do you want
                        continue?</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" @click="uploadImage">Clear Profile
                        Picture</button>
                </div>
            </div>
        </div>
    </div>

    <Loader :loading="state.loading" />
</template>

<style lang="css" scoped></style>