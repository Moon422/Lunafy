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
    <div class="container  mt-3">
        <table class="table table-striped">
            <thead>
                <tr>
                    <th scope="col">#</th>
                    <th scope="col">Cover</th>
                    <th scope="col">Name</th>
                    <th scope="col">Genres</th>
                    <th scope="col">Is Verified</th>
                    <th scope="col">Rating</th>
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
</template>