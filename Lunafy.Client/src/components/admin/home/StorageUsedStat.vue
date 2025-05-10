<script setup lang="ts">
import { ref } from 'vue'

const loading = ref<boolean>(false)
</script>

<template>
    <div class="card h-100">
        <div class="card-body d-flex justify-content-center align-items-center" v-if="loading">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
        <div class="card-body d-flex" v-else>
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
</template>