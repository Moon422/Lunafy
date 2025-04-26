<script setup lang="ts">
import { defineProps, type Component } from 'vue'
const { label, icon } = defineProps<{ label: string, icon: Component }>()
</script>

<template>
    <a href="#">
        <icon />
        {{ label }}
    </a>
</template>

<style scoped></style>