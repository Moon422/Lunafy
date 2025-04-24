<script setup lang="ts">
import Sidebar from '@/components/admin/Sidebar.vue'

</script>

<template>
    <div class="bg-gray-50 transition-colors duration-300">
        <div class="min-h-screen max-h-screen bg-red-500">
            <!-- topbar -->
            <div class="fixed top-0 left-16 right-0 h-14 z-50 flex bg-blue-500">
                <h1>topbar</h1>
            </div>

            <!-- sidebar -->
            <div class="fixed top-0 left-0 w-16 min-h-screen max-h-screen z-50 flex bg-green-500">
                <h1>sidebar</h1>
            </div>

            <div class="mt-14 ms-64 flex-1 overflow-auto p-5">
                <h1>fuck you</h1>
            </div>
        </div>
    </div>
</template>
