<script setup lang="ts">
import { HomeIcon, UserIcon } from '@heroicons/vue/16/solid'
</script>

<template>
    <button type="button" class="btn btn-text max-sm:btn-square sm:hidden" aria-haspopup="dialog" aria-expanded="false"
        aria-controls="multilevel-with-separator" data-overlay="#multilevel-with-separator">
        <span class="icon-[tabler--menu-2] size-5"></span>
    </button>

    <div class="fixed top-14 left-0 w-64 min-h-[calc(100vh-3.5rem)] z-50 bg-green-500">
        <button type="button" class="btn btn-text max-sm:btn-square sm:hidden" aria-haspopup="dialog"
            aria-expanded="false" aria-controls="multilevel-with-separator" data-overlay="#multilevel-with-separator">
            <span class="icon-[tabler--menu-2] size-5"></span>
        </button>

        <aside id="multilevel-with-separator"
            class="overlay [--auto-close:sm] overlay-open:translate-x-0 drawer drawer-start hidden max-w-64 sm:absolute sm:z-0 sm:flex sm:translate-x-0 sm:shadow-none"
            tabindex="-1">
            <div class="drawer-body px-2 pt-4">
                <ul class="menu space-y-0.5 p-0">
                    <li>
                        <a href="#">
                            <HomeIcon class="size-6" /> Home
                        </a>
                    </li>
                    <li>
                        <a href="#">
                            <UserIcon class="size-6" /> Users
                        </a>
                    </li>
                    <div class="divider text-base-content/50 py-6 after:border-0">Content</div>
                    <li>
                        <a href="#">
                            <HomeIcon class="size-6" /> Tracks
                        </a>
                    </li>
                    <li>
                        <a href="#">
                            <UserIcon class="size-6" /> Albums
                        </a>
                    </li>
                    <li>
                        <a href="#">
                            <HomeIcon class="size-6" /> Artists
                        </a>
                    </li>
                    <li>
                        <a href="#">
                            <UserIcon class="size-6" /> Playlists
                        </a>
                    </li>
                    <div class="divider text-base-content/50 py-6 after:border-0">Reports</div>
                    <li>
                        <a href="#">
                            <UserIcon class="size-6" /> Analytics
                        </a>
                    </li>
                </ul>
            </div>
        </aside>
    </div>
</template>

<style scoped></style>