<script setup lang="ts">
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css'
import '@/assets/admin.css'
import { ref } from 'vue'
import Navbar from '@/components/admin/Navbar.vue'
import Sidebar from '@/components/admin/Sidebar.vue'

const showSidebar = ref(true)
</script>

<template>
    <div data-bs-theme="dark">
        <Navbar />
        <div class="mt-3 my-md-4 d-block d-lg-flex gx-0 gx-lg-5">

        </div>
    </div>
</template>

<style scoped></style>