<script setup lang="ts">
import Sidebar from '@/components/admin/Sidebar.vue'
import { ref } from 'vue'

const showSidebar = ref(true)
</script>

<template>
    <div class="">
        <!-- topbar -->
        <div class="bg-primary">
            <h1>topbar</h1>
        </div>

        <!-- sidebar -->
        <div class="bg-danger">
            <!-- <Sidebar /> -->
            <h1>sidebar</h1>
        </div>

        <div class="bg-success">
            <h1>fuck you</h1>
            <button class="btn btn-primary" v-on:click="showSidebar = !showSidebar">click me</button>
        </div>

        <header class="navbar navbar-expand-lg sticky-top">
            <nav class="container-xxl gutter flex-wrap flex-lg-nowrap" aria-label="Main navigation">
                <button class="navbar-toggler p-2" type="button" data-bs-toggle="collapse" data-bs-target="#sidebar"
                    aria-controls="sidebar" aria-label="Toggle sidebar">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <!-- <a href="/admin" class="navbar-brand p-0 me-0 me-lg-2"> -->
                <RouterLink to="/admin" class="navbar-brand p-0 me-0 me-lg-2">
                    Lunafy
                </RouterLink>
                </a>
            </nav>
        </header>
    </div>
</template>

<style scoped></style>