<script setup lang="ts">
import { MusicalNoteIcon, HomeIcon, UsersIcon, ArchiveBoxIcon, StarIcon, Bars4Icon, ChartBarSquareIcon, Cog6ToothIcon } from '@heroicons/vue/16/solid'
import SidebarItem from './SidebarItem.vue'
</script>

<template>
    <div class="sidebar bg-primary text-light">
        <h1 class="sidebar-logo border-bottom">
            <MusicalNoteIcon class="icon" /> Lunafy
        </h1>
        <div class="sidebar-main-area p-3">
            <SidebarItem label="Dashboard" :icon="HomeIcon" />
            <SidebarItem label="Users" :icon="UsersIcon" />
            <div class="divider border-top">
                <h3>Content</h3>
            </div>
            <SidebarItem label="Tracks" :icon="MusicalNoteIcon" />
            <SidebarItem label="Albums" :icon="ArchiveBoxIcon" />
            <SidebarItem label="Artists" :icon="StarIcon" />
            <SidebarItem label="Playlists" :icon="Bars4Icon" />
            <div class="divider border-top">
                <h3>Reports</h3>
            </div>
            <SidebarItem label="Analytics" :icon="ChartBarSquareIcon" />
        </div>
        <div class="sidebar-footer p-3 border-top">
            <SidebarItem label="Settings" :icon="Cog6ToothIcon" />
        </div>
    </div>
</template>

<style scoped>
.sidebar {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    border-color: var(--admin-medium-gray);
}

.sidebar-logo {
    height: 5rem;
    display: flex;
    gap: 0.5rem;
    justify-content: center;
    align-items: center;
}

.sidebar-logo.icon {
    width: 2rem;
    height: 2rem;
}

.sidebar-main-area {
    flex: 1;
    overflow-y: scroll;
}

.sidebar.divider {
    padding-top: 0.75rem;
    margin-top: 0.75rem;
    border-color: var(--admin-medium-gray);
}

.sidebar.divider h3 {
    padding-left: 0.75rem;
    padding-right: 0.75rem;
    /* font-size: 0.75rem;
    line-height: calc(1 / 0.75); */
    color: #99a1af;
    text-transform: uppercase;

}

.sidebar-footer {
    border-color: var(--admin-medium-gray);
}
</style>