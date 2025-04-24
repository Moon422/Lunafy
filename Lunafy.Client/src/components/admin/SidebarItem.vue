<script setup lang="ts">
import { defineProps, type Component } from 'vue'
const { label, icon } = defineProps<{ label: string, icon: Component }>()
</script>

<template>
    <a href="#"
        class="flex gap-4 items-center px-3 py-2 mb-2 text-sm font-medium rounded-md text-gray-300 hover:bg-blue-grotto transition-colors duration-150 ease-in-out">
        <icon class="size-5" />
        {{ label }}
    </a>
</template>