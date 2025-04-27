<script setup lang="ts">
import { TotalUsersStatApiResponse } from '@/types/admin'


</script>

<template>
    <div class="card h-100">
        <div class="card-body d-flex">
            <div class="me-3" style="width: 4rem;">
                <img src="/image.png" alt="profile picture" width="50" class="w-100">
            </div>
            <div>
                <h6>Total Users <span class="d-none d-sm-inline fs-6 text-success"><i
                            class="bi bi-arrow-up-short"></i>12%</span></h6>
                <h3>
                    24,571
                </h3>
            </div>
        </div>
    </div>
</template>