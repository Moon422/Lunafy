<template>
    <div class="container">
        <div class="d-flex justify-content-between align-items-center">
            <h3>Dashboard</h3>
            <button class="btn btn-primary">
                <i class="bi bi-plus"></i>
                New Report
            </button>
        </div>
    </div>
    <div class="container">
        <div class="grid-container">
            <div class="grid-item">
                <div class="card h-100">
                    <div class="card-body d-flex">
                        <div class="me-3" style="width: 4rem;">
                            <img src="/image.png" alt="profile picture" width="50" class="w-100">
                        </div>
                        <div>
                            <h6>Total Users <span class="fs-6 text-success"><i
                                        class="bi bi-arrow-up-short"></i>12%</span></h6>
                            <h3>
                                24,571
                            </h3>
                        </div>
                    </div>
                </div>
            </div>
            <div class="grid-item">
                <div class="card h-100">
                    <div class="card-body d-flex">
                        <div class="me-3" style="width: 4rem;">
                            <img src="/image.png" alt="profile picture" width="50" class="w-100">
                        </div>
                        <div>
                            <h6>Total Tracks <span class="fs-6 text-success"><i
                                        class="bi bi-arrow-up-short"></i>8.1%</span></h6>
                            <h3>
                                156,392
                            </h3>
                        </div>
                    </div>
                </div>
            </div>
            <div class="grid-item">
                <div class="card h-100">
                    <div class="card-body d-flex">
                        <div class="me-3" style="width: 4rem;">
                            <img src="/image.png" alt="profile picture" width="50" class="w-100">
                        </div>
                        <div>
                            <h6>Monthly Revenue <span class="fs-6 text-success"><i
                                        class="bi bi-arrow-up-short"></i>5.4%</span></h6>
                            <h3>
                                $234,567
                            </h3>
                        </div>
                    </div>
                </div>
            </div>
            <div class="grid-item">
                <div class="card h-100">
                    <div class="card-body d-flex">
                        <div class="me-3" style="width: 4rem;">
                            <img src="/image.png" alt="profile picture" width="50" class="w-100">
                        </div>
                        <div class="col">
                            <h6>Storage Used</h6>
                            <h3>
                                85%
                            </h3>
                            <div class="progress" role="progressbar" aria-label="Basic example" aria-valuenow="100"
                                aria-valuemin="0" aria-valuemax="100">
                                <div class="progress-bar" style="width: 85%"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.grid-container {
    display: grid;
    grid-template-columns: 1fr;
    gap: 1rem;
}

/* Large devices (laptops, 992px and up) */
@media (min-width: 768px) {
    .grid-container {
        grid-template-columns: repeat(2, 1fr);
    }
}
</style>