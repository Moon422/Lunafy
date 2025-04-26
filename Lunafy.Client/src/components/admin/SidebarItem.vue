<script setup lang="ts">
import { defineProps } from 'vue'
const { label, href, icon } = defineProps<{ label: string, href?: string, icon: string }>()
</script>

<template>
    <RouterLink :to="href || '#'" class="btn rounded-0 text-start bg-body-secondary border-bottom w-100"
        active-class="sidebar-active">
        <i :class="`bi ${icon} me-2`"></i>
        {{ label }}
    </RouterLink>
</template>

<style scoped></style>