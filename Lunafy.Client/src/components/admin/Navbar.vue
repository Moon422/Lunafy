<script setup lang="ts">
</script>

<template>
    <nav class="navbar navbar-expand-lg bg-dark sticky-top">
        <div class="container-fluid">
            <button class="navbar-toggler border-0" type="button" data-bs-toggle="offcanvas"
                data-bs-target="#offcanvasSidebar" aria-controls="offcanvasSidebar">
                <i class="bi bi-list"></i>
            </button>
            <a class="navbar-brand" href="#">Navbar</a>
            <div>
                <button class="navbar-toggler border-0" type="button" data-bs-toggle="offcanvas"
                    data-bs-target="#offcanvasNotification" aria-controls="offcanvasNotification">
                    <i class="bi bi-bell"></i>
                </button>
                <div class="d-lg-none offcanvas offcanvas-end" tabindex="-1" id="offcanvasNotification"
                    aria-labelledby="offcanvasNotificationLabel">
                    <div class="offcanvas-header">
                        <h5 class="offcanvas-title" id="offcanvasNotificationLabel">Notifications</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                    </div>
                    <div class="offcanvas-body p-2">
                        <p class="text-center text-white-50">No new notifications!</p>
                    </div>
                </div>

                <button class="navbar-toggler border-0" type="button" data-bs-toggle="offcanvas"
                    data-bs-target="#offcanvasNavbar" aria-controls="offcanvasNavbar" aria-label="Toggle navigation">
                    <i class="bi bi-three-dots-vertical"></i>
                </button>

                <div class="offcanvas offcanvas-end" id="offcanvasNavbar" tabindex="-1"
                    aria-labelledby="offcanvasNavbarLabel">
                    <div class="offcanvas-header">
                        <h5 class="offcanvas-title" id="offcanvasNavbarLabel">Lunafy Admin</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                    </div>

                    <div class="offcanvas-body">
                        <ul class="navbar-nav flex-row flex-wrap ms-md-auto">
                            <li class="nav-item dropdown d-none d-lg-block">
                                <button class="btn btn-link nav-link px-0 px-lg-2 py-2 d-flex align-items-center"
                                    type="button" aria-expanded="false" data-bs-toggle="dropdown"
                                    data-bs-display="static" aria-label="Notification">
                                    <i class="bi bi-bell"></i>
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end p-2" style="width: 14rem;"
                                    aria-labelledby="lunafy-notification">
                                    <p class="text-center text-white-50">No new notifications!</p>
                                </ul>
                            </li>
                            <li class="nav-item dropdown">
                                <a class="nav-link" href="#" role="button" data-bs-toggle="dropdown"
                                    aria-expanded="false">
                                    Firstname <i class="bi bi-chevron-down ms-2"></i>
                                </a>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li class="p-2 d-flex align-items-center">
                                        <img src="/image.png" alt="profile picture" width="50" height="50">
                                        <div class="ms-2" style="max-width: 14rem;">
                                            <p class="mb-0 fw-semibold text-truncate">
                                                Firstname Lastname
                                            </p>
                                            <p class="mb-0 text-truncate">
                                                firstname_lastname@email.com
                                            </p>
                                        </div>
                                    </li>
                                    <li>
                                        <hr class="dropdown-divider">
                                    </li>
                                    <li>
                                        <RouterLink class="dropdown-item" to="/admin/settings">Settings</RouterLink>
                                    </li>
                                    <li>
                                        <RouterLink class="dropdown-item" to="/admin/account">Account</RouterLink>
                                    </li>
                                    <!-- <li>
                                        <a class="dropdown-item" href="#">
                                            Switch to Light Mode
                                        </a>
                                    </li> -->
                                    <li>
                                        <hr class="dropdown-divider">
                                    </li>
                                    <li>
                                        <RouterLink class="dropdown-item text-danger" to="/logout">
                                            Log out
                                        </RouterLink>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </nav>
</template>

<style scoped></style>