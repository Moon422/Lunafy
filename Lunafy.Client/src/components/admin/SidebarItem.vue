<script setup lang="ts">
import { defineProps, type Component } from 'vue'
const { label, icon } = defineProps<{ label: string, icon: Component }>()
</script>

<template>
    <a href="#" class="sidebar-item rounded-4">
        <icon class="icon" />
        {{ label }}
    </a>
</template>

<style scoped>
.sidebar-item {
    display: flex;
    gap: 1rem;
    align-items: center;
    padding-left: 0.75rem;
    padding-right: 0.75rem;
    padding-top: 0.5rem;
    padding-bottom: 0.5rem;
    margin-bottom: 0.5rem;
    color: #d1d5dc;
    transition: background-color 150ms cubic-bezier(0.4, 0, 0.2, 1);
}

.sidebar-item:hover {
    background: var(--admin-medium-gray);
}

.sidebar-item.icon {
    width: 1.25rem;
    height: 1.25rem;
}
</style>