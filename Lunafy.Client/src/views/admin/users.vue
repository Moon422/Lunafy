<script setup lang="ts">
import Pagination from '@/components/admin/Pagination.vue'
import { ref } from 'vue'

const page = ref<number>(1)
</script>

<template>
    <!-- Header -->
    <div class="container">
        <div class="d-flex justify-content-between align-items-center">
            <h3>Users</h3>
            <button class="btn btn-primary">
                <i class="bi bi-plus"></i>
                New Report
            </button>
        </div>
    </div>

    <!-- User table -->
    <div class="container">
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
                <tr>
                    <th scope="row">1</th>
                    <th scope="row">
                        <h6>Mahfuzur Rahman</h6>
                        <p class="mb-0" style="font-size: smaller;">@mahfuzur_rahman</p>
                    </th>
                    <th scope="row">mahfuzurr.m35@gmail.com</th>
                    <th scope="row"><i class="bi bi-check-lg"></i></th>
                    <th scope="row"><i class="bi bi-x-lg"></i></th>
                    <th scope="row">
                        <div class="container">
                            <div class="row">
                                <div class="col">
                                    <button class="btn btn-primary w-100">
                                        <i class="bi bi-pencil-square"></i>
                                    </button>
                                </div>
                                <div class="col">
                                    <button class="btn btn-danger w-100">
                                        <i class="bi bi-trash3-fill"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </th>
                </tr>
            </tbody>
        </table>
        <Pagination :totalPages="12" :currentPage="page" />
        <button @click="page++">+1</button>
        <button @click="page--">-1</button>
    </div>
</template>