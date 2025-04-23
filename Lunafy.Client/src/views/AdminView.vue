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
            <div class="fixed top-14 left-0 w-64 min-h-[calc(100vh-3.5rem)] z-50 bg-green-500">
                <button type="button" class="btn btn-text max-sm:btn-square sm:hidden" aria-haspopup="dialog"
                    aria-expanded="false" aria-controls="multilevel-with-separator"
                    data-overlay="#multilevel-with-separator">
                    <span class="icon-[tabler--menu-2] size-5"></span>
                </button>

                <aside id="multilevel-with-separator"
                    class="overlay [--auto-close:sm] overlay-open:translate-x-0 drawer drawer-start hidden max-w-64 sm:absolute sm:z-0 sm:flex sm:translate-x-0 sm:shadow-none"
                    tabindex="-1">
                    <div class="drawer-body px-2 pt-4">
                        <ul class="menu space-y-0.5 p-0">
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--home] size-5"></span>
                                    Home
                                </a>
                            </li>
                            <li class="space-y-0.5">
                                <a class="collapse-toggle collapse-open:bg-base-content/10" id="menu-app"
                                    data-collapse="#menu-app-collapse">
                                    <span class="icon-[tabler--apps] size-5"></span>
                                    Apps
                                    <span
                                        class="icon-[tabler--chevron-down] collapse-open:rotate-180 size-4 transition-all duration-300"></span>
                                </a>
                                <ul id="menu-app-collapse"
                                    class="collapse hidden w-auto space-y-0.5 overflow-hidden transition-[height] duration-300"
                                    aria-labelledby="menu-app">
                                    <li>
                                        <a href="#">
                                            <span class="icon-[tabler--message] size-5"></span>
                                            Chat
                                        </a>
                                    </li>
                                    <li>
                                        <a href="#">
                                            <span class="icon-[tabler--calendar] size-5"></span>
                                            Calendar
                                        </a>
                                    </li>
                                    <li class="space-y-0.5">
                                        <a class="collapse-toggle collapse-open:bg-base-content/10"
                                            id="sub-menu-academy" data-collapse="#sub-menu-academy-collapse">
                                            <span class="icon-[tabler--book] size-5"></span>
                                            Academy
                                            <span
                                                class="icon-[tabler--chevron-down] collapse-open:rotate-180 size-4"></span>
                                        </a>
                                        <ul id="sub-menu-academy-collapse"
                                            class="collapse hidden w-auto space-y-0.5 overflow-hidden transition-[height] duration-300"
                                            aria-labelledby="sub-menu-academy">
                                            <li>
                                                <a href="#">
                                                    <span class="icon-[tabler--books] size-5"></span>
                                                    Courses
                                                </a>
                                            </li>
                                            <li>
                                                <a href="#">
                                                    <span class="icon-[tabler--list-details] size-5"></span>
                                                    Course details
                                                </a>
                                            </li>
                                            <li class="space-y-0.5">
                                                <a class="collapse-toggle collapse-open:bg-base-content/10"
                                                    id="sub-menu-academy-stats"
                                                    data-collapse="#sub-menu-academy-stats-collapse">
                                                    <span class="icon-[tabler--chart-bar] size-5"></span>
                                                    Stats
                                                    <span
                                                        class="icon-[tabler--chevron-down] collapse-open:rotate-180 size-4"></span>
                                                </a>
                                                <ul id="sub-menu-academy-stats-collapse"
                                                    class="collapse hidden w-auto space-y-0.5 overflow-hidden transition-[height] duration-300"
                                                    aria-labelledby="sub-menu-academy-stats">
                                                    <li>
                                                        <a href="#">
                                                            <span class="icon-[tabler--chart-donut] size-5"></span>
                                                            Goals
                                                        </a>
                                                    </li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </li>
                                </ul>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--settings] size-5"></span>
                                    Settings
                                </a>
                            </li>
                            <div class="divider text-base-content/50 py-6 after:border-0">Account</div>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--login] size-5"></span>
                                    Sign In
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--logout-2] size-5"></span>
                                    Sign Out
                                </a>
                            </li>
                            <div class="divider text-base-content/50 py-6 after:border-0">Miscellaneous</div>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--users-group] size-5"></span>
                                    Support
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--files] size-5"></span>
                                    Documentation
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--settings] size-5"></span>
                                    Settings
                                </a>
                            </li>
                            <div class="divider text-base-content/50 py-6 after:border-0">Account</div>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--login] size-5"></span>
                                    Sign In
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--logout-2] size-5"></span>
                                    Sign Out
                                </a>
                            </li>
                            <div class="divider text-base-content/50 py-6 after:border-0">Miscellaneous</div>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--users-group] size-5"></span>
                                    Support
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--files] size-5"></span>
                                    Documentation
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--settings] size-5"></span>
                                    Settings
                                </a>
                            </li>
                            <div class="divider text-base-content/50 py-6 after:border-0">Account</div>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--login] size-5"></span>
                                    Sign In
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--logout-2] size-5"></span>
                                    Sign Out
                                </a>
                            </li>
                            <div class="divider text-base-content/50 py-6 after:border-0">Miscellaneous</div>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--users-group] size-5"></span>
                                    Support
                                </a>
                            </li>
                            <li>
                                <a href="#">
                                    <span class="icon-[tabler--files] size-5"></span>
                                    Documentation
                                </a>
                            </li>
                        </ul>
                    </div>
                </aside>
            </div>

            <div class="mt-14 ms-64 flex-1 overflow-auto p-5">
                <h1>fuck you</h1>
            </div>
        </div>
    </div>
</template>
