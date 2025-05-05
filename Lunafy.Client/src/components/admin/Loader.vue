<script setup lang="ts">
const props = defineProps<{ loading: boolean }>()
</script>

<template>
    <div v-if="loading"
        style="background: rgba(0.35,0.35,0.35,0.60); display: flex; justify-content: center; align-items: center; position: absolute; top: 0; right: 0; left: 0; bottom: 0; z-index: 999999;">
        <div class="spinner-border text-light" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    </div>
</template>