<script setup lang="ts">
import Sidebar from '@/components/admin/Sidebar.vue'
import { ref } from 'vue'

const showSidebar = ref(true)
</script>

<template>
    <div class="admin-layout">
        <!-- topbar -->
        <div :class="`topbar ${showSidebar ? 'topbar-open' : 'topbar-close'} bg-primary`">
            <h1>topbar</h1>
        </div>

        <!-- sidebar -->
        <!-- <div :class="`sidebar ${showSidebar ? 'sidebar-open' : 'sidebar-close'}`">
            <Sidebar />
        </div> -->

        <div :class="`main-content ${showSidebar ? 'main-content-open' : 'main-content-close'}`">
            <h1>fuck you</h1>
            <button v-on:click="showSidebar = !showSidebar">click me</button>
        </div>
    </div>
</template>

<style scoped>
.admin-layout {
    background: #f9fafb;
    font-family: 'Inter', 'Noto Sans', 'sans-serif';
    transition: all 300ms;
}

.admin-layout.topbar {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    height: 5rem;
    z-index: 50;
    display: flex;
    transition: all 300ms;
    border: 2px solid black;
}

.admin-layout.sidebar {
    position: fixed;
    top: 0;
    display: none;
    width: 16rem;
    height: 100vh;
    z-index: 50;
    display: flex;
    transition: all 300ms;
}

.admin-layout.main-content {
    margin-top: 80rem;
    margin-left: 0;
    flex: 1;
    overflow: auto;
    padding: 1.25rem;
    transition: all 300ms;
    border: 2px solid black;
}

@media (width >=64rem) {
    .admin-layout.topbar {
        left: 16rem;
    }

    .admin-layout.sidebar {
        display: block;
    }

    .admin-layout.topbar-open {
        margin-left: 16rem;
    }

    .admin-layout.topbar-close {
        left: -16rem;
    }

    .admin-layout.sidebar-open {
        left: 0;
    }

    .admin-layout.sidebar-close {
        margin-left: 0;
    }

    .admin-layout.main-content-open {
        margin-left: 16rem;
    }

    .admin-layout.main-content-close {
        margin-left: 0;
    }
}
</style>