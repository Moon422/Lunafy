<script setup lang="ts">
import Sidebar from '@/components/admin/Sidebar.vue'

</script>

<template>
    <div class="bg-gray-50  transition-colors duration-300">
        <div class="flex flex-col min-h-screen bg-red-500">
            <!-- topbar -->
            <div class="fixed top-0 left-0 right-0 h-14 z-50 flex">
                <h1>topbar</h1>
            </div>

            <!-- sidebar -->
            <Sidebar />

            <div class="mt-14 ms-64 flex-1 overflow-auto p-5">
                <h1>fuck you</h1>
            </div>
        </div>
    </div>
</template>
