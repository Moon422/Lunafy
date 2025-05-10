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
            <div>
                <h6>Monthly Revenue <span class="d-none d-sm-inline fs-6 text-success"><i
                            class="bi bi-arrow-up-short"></i>5.4%</span></h6>
                <h3>
                    $234,567
                </h3>
            </div>
        </div>
    </div>
</template>