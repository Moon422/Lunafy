<script setup lang="ts">
import SidebarItem from './SidebarItem.vue'
import SidebarItemGroup from './SidebarItemGroup.vue'
</script>

<template>
    <aside class="sidebar bg-body text-white-50">
        <div class="offcanvas-lg offcanvas-start h-100" tabindex="-1" id="offcanvasSidebar"
            aria-labelledby="offcanvasSidebarLabel">
            <div class="offcanvas-header">
                <h5 class="offcanvas-title" id="offcanvasSidebarLabel">Lunafy Admin</h5>
                <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"
                    data-bs-target="#offcanvasSidebar"></button>
            </div>
            <div class="offcanvas-body h-100">
                <nav class="w-100 h-100 d-flex flex-column">
                    <div class="flex-grow-1 overflow-y-auto d-flex flex-column">
                        <SidebarItem label="Dashboard" icon="bi-house-door" href="/admin/home" />
                        <SidebarItem label="Users" icon="bi-people" href="/admin/users" />
                        <SidebarItemGroup title="Content">
                            <SidebarItem label="Tracks" icon="bi-music-note" href="/admin/tracks" />
                            <SidebarItem label="Albums" icon="bi-cassette" href="/admin/Albums" />
                            <SidebarItem label="Artists" icon="bi-star" href="/admin/artists" />
                            <SidebarItem label="Playlists" icon="bi-music-note-list" href="/admin/playlists" />
                        </SidebarItemGroup>
                        <SidebarItemGroup title="Reports">
                            <SidebarItem label="Analytics" icon="bi-bar-chart" href="/admin/analytics" />
                        </SidebarItemGroup>
                    </div>
                    <div>
                        <SidebarItem label="Settings" icon="bi-gear" href="/admin/settings" />
                    </div>
                </nav>
            </div>
        </div>
    </aside>
</template>

<style scoped>
.sidebar-active,
.sidebar nav a:hover {
    background: rgb(99, 99, 99) !important;
    color: black;
}

@media (min-width: 992px) {
    .sidebar {
        position: fixed;
        display: block !important;
        height: calc(100vh - 3.5rem);
        width: 15rem;
        overflow-y: auto;
    }
}
</style>