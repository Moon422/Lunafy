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
    artistModel: ArtistCreateModel,
    artistErrorModel: ArtistCreateErrorModel,
    musicBrainzIdValidating: boolean
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
})

watch(() => state.error, () => {
    if (state.error && state.error.length > 0) {
        toast.error(state.error, { onClose: () => state.error = null })
    }
})

const createArtist = async (firstname: string, lastname: string, biography: string | null, musicBrainzId: string | null) => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/artist`, {
        method: 'POST', headers, credentials: 'include', body: JSON.stringify({ firstname, lastname, biography, musicBrainzId })
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

const onArtistCreateSubmission = async () => {
    state.loading = true
    try {
        if (await createArtist(state.artistModel.firstname, state.artistModel.lastname, state.artistModel.biography, state.artistModel.musicBrainzId) === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })
            const { data, errors }: HttpResponseModel<UserReadModel> = await response.json()

            if (!response.ok) {
                router.push('/login')
            }

            if (await createArtist(state.artistModel.firstname, state.artistModel.lastname, state.artistModel.biography, state.artistModel.musicBrainzId) !== HTTP_STATUS.CREATED) {
                state.error = "Failed to create new artist. Please try again."
            }
        }

        router.push('/admin/artists')
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
</script>

<template>
    <form @submit.prevent="onArtistCreateSubmission">
        <!-- Header -->
        <div class="container">
            <div class="d-flex justify-content-between align-items-center">
                <h3>Create New Artist</h3>
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
    <Loader :loading="state.loading" />
</template>

<style lang="css" scoped></style>