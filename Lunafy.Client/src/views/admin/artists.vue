<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import type { ArtistReadModel } from '@/types/admin'
import { ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import Pagination from '@/components/admin/Pagination.vue'
import Loader from '@/components/admin/Loader.vue'
import { HTTP_STATUS } from '@/utils'
import type { HttpResponseModel, SearchResultModel } from '@/types/common'
import type { LoginResponseModel } from '@/types/user'


const baseUrl = import.meta.env.VITE_API_URL

const authStore = useAuthStore()
const router = useRouter()

const loading = ref<boolean>(false)
const error = ref<string | null>(null)

const page = ref<number>(1)
const totalPages = ref<number>(0)
const pageSize = ref<number>(5)

const artists = ref<ArtistReadModel[] | null>()
const artistIdToDelete = ref<number>(0)

const fetchArtists = async (page: number, pageSize: number) => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const urlParams = new URLSearchParams({
        pageNumber: page.toString(),
        pageSize: pageSize.toString()
    })

    const response = await fetch(`${baseUrl}/api/admin/artist?${urlParams.toString()}`, {
        method: 'GET', headers, credentials: 'include'
    })

    if (response.status === HTTP_STATUS.UNAUTHORIZED) {
        return response.status
    }

    const { data, errors }: HttpResponseModel<SearchResultModel<ArtistReadModel>> = await response.json()
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
    artists.value = data.data

    return response.status
}

const deleteArtist = async () => {
    const headers: Headers = new Headers({ 'Content-Type': 'application/json' })
    if (authStore.token) {
        headers.append('Authorization', `Bearer ${authStore.token}`)
    }

    const response = await fetch(`${baseUrl}/api/admin/artist/${artistIdToDelete.value}`, {
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
    if (artistIdToDelete.value <= 0) {
        return false
    }

    loading.value = true
    try {
        if (await deleteArtist() === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })

            if (!response.ok) {
                router.push('/login')
            }

            if (await deleteArtist() !== HTTP_STATUS.NO_CONTENT) {
                error.value = "Failed to delete user. Please try again."
            }
        }

        await fetchArtists(page.value, pageSize.value)
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
        if (await fetchArtists(page.value, pageSize.value) === HTTP_STATUS.UNAUTHORIZED) {
            const response = await fetch(`${baseUrl}/api/user/refresh-token`, {
                credentials: 'include'
            })
            const { data, errors }: HttpResponseModel<LoginResponseModel> = await response.json()

            if (!response.ok) {
                router.push('/login')
            }

            if (await fetchArtists(page.value, pageSize.value) !== HTTP_STATUS.OK) {
                error.value = "Failed to load artists. Please try again."
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
            <h3>Artists</h3>
            <RouterLink to="/admin/artists/create" class="btn btn-primary">
                <i class="bi bi-plus"></i>
                New Artist
            </RouterLink>
        </div>
    </div>

    <!-- User table -->
    <div class="container mt-3">
        <table class="table table-striped">
            <thead>
                <tr>
                    <th scope="col">#</th>
                    <th scope="col" style="width: 64px;">Image</th>
                    <th scope="col">Name</th>
                    <th scope="col">MusicBrainz Id</th>
                    <!-- <th scope="col">Genres</th> -->
                    <!-- <th scope="col">Is Verified</th> -->
                    <!-- <th scope="col">Rating</th> -->
                    <th scope="col">Actions</th>
                </tr>
            </thead>
            <tbody>
                <template v-if="!artists || !artists.length">
                    <tr>
                        <td colspan="7" class="text-center">
                            No data
                        </td>
                    </tr>
                </template>
                <template v-else>
                    <tr v-for="artist in artists" :key="artist.id">
                        <th scope="row">{{ artist.id }}</th>
                        <th scope="row">
                            <img :src="artist.profilePicture?.profileImage64" alt="Artist profile picture" width="64"
                                height="64">
                        </th>
                        <th scope="row">{{ artist.firstname }} {{ artist.lastname }}</th>
                        <th scope="row">{{ artist.musicBrainzId }}</th>
                        <!-- <th scope="row">
                            Genres
                        </th> -->
                        <!-- <th scope="row">
                            <i class="bi bi-check-lg" v-if="artist.isArtist"></i>
                            <i class="bi bi-x-lg" v-else></i>
                        </th> -->
                        <!-- <th scope="row">
                            <i class="bi bi-check-lg" v-if="user.isArtist"></i>
                            <i class="bi bi-x-lg" v-else></i>
                        </th> -->
                        <th scope="row" style="width: 16rem;">
                            <div class="container">
                                <div class="row">
                                    <div class="col">
                                        <RouterLink :to="`/admin/artists/${artist.id}`" class="btn btn-primary w-100">
                                            <i class="bi bi-pencil-square"></i>
                                        </RouterLink>
                                    </div>
                                    <div class="col">
                                        <button class="btn btn-danger w-100" data-bs-toggle="modal"
                                            data-bs-target="#deleteConfirmation"
                                            @click.prevent="() => artistIdToDelete = artist.id">
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