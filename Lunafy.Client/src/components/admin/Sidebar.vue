<script setup lang="ts">
import SidebarItem from './SidebarItem.vue'
import SidebarItemGroup from './SidebarItemGroup.vue'
</script>

<template>
    <aside class="sidebar bg-primary">
        <div class="offcanvas-lg offcanvas-start" tabindex="-1" id="offcanvasSidebar"
            aria-labelledby="offcanvasSidebarLabel">
            <div class="offcanvas-header">
                <h5 class="offcanvas-title" id="offcanvasSidebarLabel">Lunafy Admin</h5>
                <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"
                    data-bs-target="#offcanvasSidebar"></button>
            </div>
            <div class="offcanvas-body">
                <nav class="w-100 w-lg-">
                    <div class="mb-0 pb-3 pb-md-2 pe-lg-2 d-flex flex-column">
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
                </nav>
            </div>
        </div>
    </aside>
</template>

<style scoped>
@media (min-width: 992px) {
    .sidebar {
        position: -webkit-sticky;
        position: sticky;
        top: 5rem;
        display: block !important;
        height: calc(100vh - 3.5rem);
        /* height: 100vh; */
        padding-left: .25rem;
        margin-left: -.25rem;
        overflow-y: auto;
    }
}
</style>