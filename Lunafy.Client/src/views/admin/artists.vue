<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import type { ArtistReadModel } from '@/types/admin'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import Pagination from '@/components/admin/Pagination.vue'


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
</script>

<template>
    <!-- Header -->
    <div class="container">
        <div class="d-flex justify-content-between align-items-center">
            <h3>Artists</h3>
            <RouterLink to="/admin/users/create" class="btn btn-primary">
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
                    <th scope="col">Image</th>
                    <th scope="col">Name</th>
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
                            <img :src="`${baseUrl}/images/artists/profile/${artist.id}/64.webp`"
                                alt="Artist profile picture" width="64" height="64">
                        </th>
                        <th scope="row">{{ artist.firstname }} {{ artist.lastname }}</th>
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
                        <th scope="row">
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
</template>