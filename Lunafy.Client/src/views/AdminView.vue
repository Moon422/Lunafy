<script setup lang="ts">
import Sidebar from '@/components/admin/Sidebar.vue'
import { ref } from 'vue'

const showSidebar = ref(true)
</script>

<template>
    <div class="bg-gray-50 font-inter transition-all duration-300">
        <div class="bg-red-500">
            <!-- topbar -->
            <div
                :class="`fixed top-0 left-0 ${showSidebar ? 'lg:left-64' : 'lg:left-0'} right-0 h-20 z-50 flex transition-all duration-300 border-2 border-black bg-blue-500`">
                <h1>topbar</h1>
            </div>

            <!-- sidebar -->
            <div
                :class="`fixed top-0 hidden lg:block ${showSidebar ? 'left-0' : '-left-64'} w-64 h-screen z-50 flex transition-all duration-300`">
                <Sidebar />
            </div>

            <div
                :class="`mt-20 ms-0 ${showSidebar ? 'lg:ms-64' : 'lg:ms-0'} flex-1 overflow-auto p-5 transition-all duration-300 border-2 border-black`">
                <h1>fuck you</h1>
                <button v-on:click="showSidebar = !showSidebar">click me</button>
            </div>
        </div>
    </div>
</template>
