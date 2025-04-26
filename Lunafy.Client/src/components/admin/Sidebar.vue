<script setup lang="ts">
import { MusicalNoteIcon, HomeIcon, UsersIcon, ArchiveBoxIcon, StarIcon, Bars4Icon, ChartBarSquareIcon, Cog6ToothIcon } from '@heroicons/vue/16/solid'
import SidebarItem from './SidebarItem.vue'
import SidebarItemGroup from './SidebarItemGroup.vue'
</script>

<template>
    <div class="sidebar bg-primary text-light">
        <div class="">
            <SidebarItem label="Dashboard" :icon="HomeIcon" />
            <SidebarItem label="Users" :icon="UsersIcon" />
            <SidebarItemGroup title="Content">
                <SidebarItem label="Tracks" :icon="MusicalNoteIcon" />
                <SidebarItem label="Albums" :icon="ArchiveBoxIcon" />
                <SidebarItem label="Artists" :icon="StarIcon" />
                <SidebarItem label="Playlists" :icon="Bars4Icon" />
            </SidebarItemGroup>
            <SidebarItemGroup title="Reports">
                <SidebarItem label="Analytics" :icon="ChartBarSquareIcon" />
            </SidebarItemGroup>
        </div>
        <div class="sidebar-footer p-3 border-top">
            <SidebarItem label="Settings" :icon="Cog6ToothIcon" />
        </div>
    </div>
</template>

<style scoped></style>