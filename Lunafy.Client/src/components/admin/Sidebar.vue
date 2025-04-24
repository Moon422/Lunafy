<script setup lang="ts">
import { MusicalNoteIcon, HomeIcon, UsersIcon, ArchiveBoxIcon, StarIcon, Bars4Icon, ChartBarSquareIcon, Cog6ToothIcon } from '@heroicons/vue/16/solid'
import SidebarItem from './SidebarItem.vue'
</script>

<template>
    <div class="w-full h-full flex flex-col bg-blue text-white">
        <h1 class=" h-20 flex gap-2 justify-center items-center text-2xl font-semibold border-b border-blue-grotto">
            <MusicalNoteIcon class="size-8" /> Lunafy
        </h1>
        <div class="flex-1 overflow-y-scroll p-4">
            <SidebarItem label="Dashboard" :icon="HomeIcon" />
            <SidebarItem label="Users" :icon="UsersIcon" />
            <div class="pt-3 mt-3 border-t border-blue-grotto">
                <h3 class="px-3 text-xs font-semibold text-gray-400 uppercase tracking-wider">Content</h3>
            </div>
            <SidebarItem label="Tracks" :icon="MusicalNoteIcon" />
            <SidebarItem label="Albums" :icon="ArchiveBoxIcon" />
            <SidebarItem label="Artists" :icon="StarIcon" />
            <SidebarItem label="Playlists" :icon="Bars4Icon" />
            <div class="pt-3 mt-3 border-t border-blue-grotto">
                <h3 class="px-3 text-xs font-semibold text-gray-400 uppercase tracking-wider">Reports</h3>
            </div>
            <SidebarItem label="Analytics" :icon="ChartBarSquareIcon" />
        </div>
        <div class="p-4 border-t border-blue-grotto">
            <SidebarItem label="Settings" :icon="Cog6ToothIcon" />
        </div>
    </div>
</template>

<style scoped></style>